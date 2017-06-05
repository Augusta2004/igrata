let mongoose = require('mongoose');

let connection = 'mongodb://localhost:27017/game';

mongoose.Promise = global.Promise;
let Schema = mongoose.Schema;


/////MODELS///////////


let userSchema = new Schema({
    user_id: Number,
    username: String,
    password: String,
    mail: String,
    date_reg: Number
});

let User = mongoose.model('User', userSchema);

let Character = mongoose.model('Character', {
    id: Number,
    coins: Number
});

let Item = mongoose.model('Item', {
    item_id: Number,
    name: String,
    picture: String,
    price: Number,
    is_premium: Boolean,
    type: String
});

let Character_item = mongoose.model('Character_item', {
    character_id: Number,
    item_id: Number,
    picture: String,
    is_on: Boolean
});

let Counter = mongoose.model('Counter', {
    user_id: Number,
    item_id: Number
});

//////MODELS///////////


//const models = require('./models.js');

mongoose
	.connect(connection)
	.then(() => {
		console.log('Mongoose is running');

		/*new Counter({
			 user_id: 0,
			 item_id: 0
		}).save()*/

	});

const express = require('express'), LocalStrategy = require('passport').Strategy;

let app = express();


const server = require('http').createServer(app);
const io = require('socket.io').listen(server);
var async = require('async');

const port = 4567;

app.set('port', port);

let playerSpawnPoints = [];
let clients = [];

app.get('/', (req, res) => {
	res.send("hey you got back get '/'");
});

io.on('connection', socket => {
    
	let currentPlayer = {};
	currentPlayer.name = "unknown";

	socket.on('palyer connect', () => {
		console.log(currentPlayer.name+ 'recv: player connected');

		for(let i=0; i<clients.length; i++){
			let playerConnected = {
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

		if(clients.length === 0){
			playerSpawnPoints = [];
			data.playerSpawnPoints.forEach((_playerSpawnPoints) => {
				let playerSpawnPoint = {
					position: _playerSpawnPoints.position
				};
                playerSpawnPoints.push(playerSpawnPoint);
			});
		}

		let randomSpawnPoints = playerSpawnPoints[Math.floor(Math.random() * playerSpawnPoints.length)];
		currentPlayer = {
			name: data.name,
			position: randomSpawnPoints.position,
			animation: [0,-1]
		};
		clients.push(currentPlayer);
		console.log(currentPlayer.name + ' emit: play:' + JSON.stringify(currentPlayer));

		socket.emit('play', currentPlayer);
		socket.broadcast.emit('other player connected', currentPlayer);
	});

	socket.on('player move', (data) => {
		//console.log('recv: move: ' + JSON.stringify(data));
		currentPlayer.position = data.position;
		currentPlayer.animation = data.animation;
		socket.broadcast.emit('player move', currentPlayer);
	});

	socket.on('player stop animation', () =>{
        socket.broadcast.emit('player stop animation', currentPlayer);
	});

	socket.on('player chat', (data) => {
		data.player = currentPlayer.name;

		socket.emit('player chat', data);
		socket.broadcast.emit('player chat', data);
	});

	socket.on('user register', (data) => {

		let errors = new Array();

        async.parallel([
            function validateEmail(callback) {
                User.findOne({mail: data.mail}, function(err, existingEmail) {
                    if(existingEmail) {
                        errors.push('Email already exists');
                        //callback('Email already exists');
                    }
                })
            },
            function validateUsername(callback) {
                User.findOne({username: data.username}, function(err, existingUsername) {
                    if(existingUsername) {
                        errors.push('Username already exists');
                        //callback('Username already exists');
                    }

                    callback(errors)
                })
            }
        ], function(err) {

            if(err.length > 0) {
                console.log(err);
                //return next(err);
            } else {
                console.log('success');

                let lastUserId = Counter.findOne();
                lastUserId.select('user_id');

                lastUserId.exec((err, counter) =>{
                	console.log(counter.user_id);

                    new User({
                        user_id: counter.user_id+1,
                        username: data.username,
                        password: data.password,
                        mail: data.mail,
                        date_reg: 666
                    }).save()

                    counter.user_id++;
                    counter.save();
				})
            }
        });
	});

	socket.on('disconnect', () => {
		console.log(currentPlayer.name + "recv: disconnected");
		socket.broadcast.emit('other player disconnected', currentPlayer);

		for(let i=0; i<clients.length; i++){
			if(clients[i].name === currentPlayer.name){
				clients.splice(i,1);
			}
		}
	});
});

server.listen(app.get('port'), () => {
    console.log("Server is running on port: " + port);
});