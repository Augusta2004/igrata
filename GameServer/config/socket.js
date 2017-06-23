const mongoose = require('mongoose');
const User = mongoose.model('User');
const Character = mongoose.model('Character');
const Item = mongoose.model('Item');
const Character_item = mongoose.model('Character_item');
const Counter = mongoose.model('Counter');

module.exports = (server) =>{
    const io = require('socket.io').listen(server);
    let async = require('async');
    let playerSpawnPoints = [];
    let clients = [];

    //let username = "";
    //let user_id = 0;
    //let coins = 0;

    io.on('connection', socket => {

        console.log('a user connected');

        let currentPlayer = {};
        currentPlayer.name = "unknown";

        socket.on('player connect', () => {

            console.log(currentPlayer.name + 'recv: player connected');

            for (let i = 0; i < clients.length; i++) {
                let playerConnected = {
                    id: clients[i].id,
                    name: clients[i].name,
                    position: clients[i].position
                };

                //in your current game we need to tell you about the other players
                socket.emit('other player connected', playerConnected);
                console.log(currentPlayer.name + ' emit: other player connected:' + JSON.stringify(playerConnected));
            }
        });

        socket.on('play', (data) => {
            console.log(currentPlayer.name + 'recv play: ' + JSON.stringify(data));

            if (clients.length === 0) {
                playerSpawnPoints = [];
                data.playerSpawnPoints.forEach((_playerSpawnPoints) => {
                    let playerSpawnPoint = {
                        position: _playerSpawnPoints.position
                    };
                    playerSpawnPoints.push(playerSpawnPoint);
                });
            }

            let randomSpawnPoints = playerSpawnPoints[Math.floor(Math.random() * playerSpawnPoints.length)];
            currentPlayer.position = randomSpawnPoints.position;
            currentPlayer.animation = [0, -1];

            clients.push(currentPlayer);
            console.log(currentPlayer.name + ' emit: play:' + JSON.stringify(currentPlayer));

            socket.emit('play', currentPlayer);
            socket.broadcast.emit('other player connected', currentPlayer);
        });

        socket.on('player move', (data) => {
            //console.log('recv: move: ' + JSON.stringify(data));
            currentPlayer.position = data.position;
            currentPlayer.animation = data.animation;
            currentPlayer.numAnimation = data.numAnimation;
            socket.broadcast.emit('player move', currentPlayer);
        });

        socket.on('player stop animation', (data) => {
            currentPlayer.numAnimation = data;
            socket.broadcast.emit('player stop animation', currentPlayer);
            //console.log(data);
        });

        socket.on('player chat', (data) => {
            data.player = currentPlayer.name;

            socket.emit('player chat', data);
            socket.broadcast.emit('player chat', data);
        });

        socket.on('user register', (data) => {

            let errors = new Array();


            let checkMail = function () {

                User.findOne({mail: data.mail}, function (err, existingEmail) {
                    console.log("1");

                    if (existingEmail) {
                        errors.push('Email already exists');
                        //callback('Email already exists');
                    }
                })

            };


            let checkUsername = function () {
                return new Promise(() => {
                    User.findOne({username: data.username}, function (err, existingUsername) {
                        console.log("2");
                        if (existingUsername) {
                            errors.push('Username already exists');
                            //callback('Username already exists');
                        }
                    })
                })
            };

            let registerUser = function () {
                console.log("3");
                return new Promise(() => {

                    if (errors.length > 0) {
                        console.log(errors);

                        let errObj = {
                            errors: errors
                        };

                        socket.emit('user register', errObj);
                    } else {
                        console.log('success');

                        let lastUserId = Counter.findOne();
                        lastUserId.select('user_id');

                        lastUserId.exec((err, counter) => {
                            console.log(counter.user_id);

                            new User({
                                user_id: counter.user_id + 1,
                                username: data.username,
                                password: data.password,
                                mail: data.mail,
                                date_reg: 666
                            }).save();

                            new Character({
                                user_id: counter.user_id + 1,
                                coins: 666
                            }).save();

                            counter.user_id++;
                            counter.save().then(() => {
                                let errObj = {
                                    errors: errors
                                };

                                socket.emit('user register', errObj);
                            });
                        })
                    }
                })
            };

            function asyncFunction(item, cb) {
                setTimeout(() => {
                    item();
                    //console.log('done with', cb);
                    cb();
                }, 15);
            }

            let requests = [checkMail, checkUsername, registerUser].reduce((promiseChain, item) => {
                return promiseChain.then(() => new Promise((resolve) => {
                    asyncFunction(item, resolve);
                }));
            }, Promise.resolve());

        });

        socket.on('user login', (data) => {

            let errors = new Array();

            User.findOne({username: data.username, password: data.password}, function (err, existingUser) {


                if (!existingUser) {
                    errors.push('Username or password is wrong!');
                    console.log(errors)
                }
                else {
                    for(let i = 0; i < clients.length ; i++) {
                        if (clients[i].name == data.username) {
                            errors.push('User with this username is already logged in!');
                        }
                    }

                    if(errors.length == 0){
                        Character.findOne({user_id: existingUser.user_id}, function (err, character) {
                            //username = existingUser.username;
                            //user_id = existingUser.user_id;
                            //coins = character.coins;
                            currentPlayer.name = existingUser.username;
                            currentPlayer.id = existingUser.user_id;
                            currentPlayer.coins = character.coins;

                            console.log(currentPlayer + "fdklgjdflkgj" + {clients});
                        });

                        console.log("Successfull login");
                    }
                }

                let errObj = {
                    errors: errors
                };

                socket.emit('user login', errObj);
            })
        });

        socket.on('collect item', (data) => {

            Character_item.findOne({item_id: data, user_id: currentPlayer.id}, function (err, hasItem) {
                if (hasItem) {
                    console.log("has item");
                    let itemObj = {
                        id: -1,
                        name: "You already have this item"
                    };

                    socket.emit('collect item', itemObj);
                }
                else {
                    Item.findOne({item_id: data}, function (err, itemExists) {
                        if (itemExists) {
                            new Character_item({
                                user_id: currentPlayer.id,
                                username: currentPlayer.name,
                                item_id: data,
                                name: itemExists.name,
                                picture: itemExists.picture,
                                type: itemExists.type,
                                is_on: false
                            }).save().then(() => {
                                let itemObj = {
                                    id: data,
                                    name: "You have successful collected " + itemExists.name
                                };

                                socket.emit('collect item', itemObj);
                            })
                        }
                        else {
                            let itemObj = {
                                id: -1,
                                name: "Item does not exist"
                            };

                            socket.emit('collect item', itemObj);
                            console.log("Item does not exist");
                        }
                    });
                }
            });

        });

        socket.on('get player items', () => {
            Character_item.find({user_id: currentPlayer.id}, function (err, items) {
                console.log(currentPlayer);
                socket.emit('get player items', {items});
            })
        });

        socket.on('get other player items', (data) => {
            Character_item.find({username: data.name, is_on: true}, function (err, items) {

                socket.emit('get other player items', {items});
            })
        });

        socket.on('change item', (data) => {
            //console.log(data);
            console.log("Changing items");
            Character_item.findOne({user_id: currentPlayer.id, item_id: data.id}, function (err, item) {
                let conditions = {user_id: currentPlayer.id, type: data.type}
                    , update = {$set: {is_on: false}}
                    , options = {multi: true};

                Character_item.update(conditions, update, options, callback);

                function callback(err, numAffected) {


                    item.is_on = true;
                    item.save().then(() => {
                        Character_item.find({user_id: currentPlayer.id, is_on: true}, function (err, items) {
                            console.log("gg wp");
                            socket.broadcast.emit('get on other player items', {username: currentPlayer.name, items})
                            socket.emit('get on items', {items});
                        })
                    });
                }
            });
        });

        socket.on('get on items', () => {
            Character_item.find({user_id: currentPlayer.id, is_on: true}, function (err, items) {
                //console.log({items});
                socket.emit('get on items', {items});
            })
        });

        socket.on('get on other player items', (data) => {
            Character_item.find({user_id: data.id, is_on: true}, function (err, items) {
                //console.log({items});
                socket.emit('get on other player items', {username:data.type, items});
            })
        });

        socket.on('buy item', (data) => {
            let errors = new Array();
            let itemObj = {};

            let checkHasItem = function () {

                Character_item.findOne({item_id: data, user_id: currentPlayer.id}, function (err, item) {
                    console.log("1");

                    if (item) {
                        errors.push('You already have this item!');
                    }
                })

            };

            let checkCoins = function () {
                return new Promise(() => {
                    Character.findOne({user_id: currentPlayer.id}, function (err, character) {
                        console.log("2");
                        Item.findOne({item_id: data}, function (err, item) {
                            itemObj = item;
                            if (character.coins < item.price) {
                                console.log(character.coins + " | " + item.price)
                                errors.push('You do not have enough coins!');
                            }
                        })
                    })
                })
            };


            //check if item is premium
            /*
            let checkPremium = function () {
                return new Promise(() => {
                    .findOne({username: data.username}, function (err, existingUsername) {
                        console.log("2");
                        if (existingUsername) {
                            errors.push('Username already exists');
                            //callback('Username already exists');
                        }
                    })
                })
            };
            */

            let buyItem = function () {
                console.log("3");
                return new Promise(() => {

                    if (errors.length > 0) {
                        console.log(errors);

                        let errObj = {
                            errors: errors
                        };

                        socket.emit('buy item', errObj);
                    } else {
                        console.log(itemObj);

                            new Character_item({
                                user_id: currentPlayer.id,
                                username: currentPlayer.name,
                                item_id: data,
                                name: itemObj.name,
                                picture: itemObj.picture,
                                type: itemObj.type,
                                is_on: false
                            }).save();

                        currentPlayer.coins -= itemObj.price;

                        let conditions = {user_id: currentPlayer.id, type: data.type}
                            , update = {$set: {coins: currentPlayer.coins}}
                            , options = {multi: true};

                        Character.update(conditions, update, options, callback);


                        function callback(err, numAffected) {
                            console.log("bought item");

                            let errObj = {
                                errors: errors
                            };

                            socket.emit('buy item', errObj);
                        }

                    }
                })
            };

            function asyncFunction(item, cb) {
                setTimeout(() => {
                    item();
                    //console.log('done with', cb);
                    cb();
                }, 15);
            }

            let requests = [checkHasItem, checkCoins, buyItem].reduce((promiseChain, item) => {
                return promiseChain.then(() => new Promise((resolve) => {
                    asyncFunction(item, resolve);
                }));
            }, Promise.resolve());

        });

        socket.on('remove item', (data) => {
            Character_item.findOne({item_id: data, user_id: currentPlayer.id, is_on: true}, function (err, item) {

                if (item) {
                    let conditions = {user_id: currentPlayer.id, item_id: data}
                        , update = {$set: {is_on: false}}
                        , options = {multi: false};

                    Character_item.update(conditions, update, options, callback);

                    function callback(err, numAffected) {
                        Character_item.find({user_id: currentPlayer.id, is_on: true}, function (err, items) {
                            //console.log({items});
                            socket.broadcast.emit('get on other player items', {username: currentPlayer.name, items})
                            socket.emit('get on items', {items});
                        })
                    }
                }
            })


        });

        socket.on('disconnect', () => {
            console.log(currentPlayer.name + "recv: disconnected");
            socket.broadcast.emit('other player disconnected', currentPlayer);

            for (let i = 0; i < clients.length; i++) {
                if (clients[i].name === currentPlayer.name) {
                    clients.splice(i, 1);
                }
            }
        });
    });
};