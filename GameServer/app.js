const express = require('express');

let app = express();

const server = require('http').createServer(app);
const io = require('socket.io').listen(server);

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

	socket.on('player move', (data) =>{
		//console.log('recv: move: ' + JSON.stringify(data));
		currentPlayer.position = data.position;
		currentPlayer.animation = data.animation;
		socket.broadcast.emit('player move', currentPlayer);
	});

	socket.on('player stop animation', () =>{
        socket.broadcast.emit('player stop animation', currentPlayer);
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

	/*
	let currentUser;

    socket.on("USER_CONNECT", () => {
        console.log("User connectedddd");

        for (let i = 0; i < clients.length; i++) {
            socket.emit("USER_CONNECTED", { name: clients[i], position: clients[i].position });
            console.log("User: " + clients[i].name + " is connected!");
        }

    });


	socket.on("test", () =>{
		console.log("samo az vurvq");
	});

    socket.on("PLAY", data => {
        console.log(data);

        currentUser = {
            name: data.name,
            position: data.position
        }
		
        clients.push(currentUser);
        socket.emit("PLAY", currentUser);
		socket.emit("PLAYER_INFO", currentUser);
        socket.broadcast.emit("USER_CONNECTED", currentUser);
    });


    socket.on("MOVE", data => {
        currentUser.position = data.position;		
        socket.emit("MOVE", currentUser);
        socket.broadcast.emit("MOVE", currentUser);
        console.log("User: " + currentUser.name + " move to " + currentUser.position);
    });

    socket.on("disconnect", () => {
        socket.broadcast.emit("USER_DISCONNECTED", currentUser);
        for (let i = 0; i < clients.length; i++) {
            if (clients[i].name === currentUser.name) {
                console.log("User: " + clients[i].name + " disconnected!");
                clients.splice(i, 1);
            }
        }
    });

	*/

});

server.listen(app.get('port'), () => {
    console.log("Server is running on port: " + port);
});
