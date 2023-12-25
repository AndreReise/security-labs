
const webSocket = require('ws');

const SERVER_URL = process.env.SERVER_URL || "localhost:8080";

let socket = new webSocket(`ws://${SERVER_URL}`);
    
socket.onopen = () => onSocketOpen();
socket.onmessage = (msg) => onSocketMessage(msg);
socket.onclose = () => onSocketClose();

function onSocketOpen(){
    console.log('Connection with server established!')
}

// Відкрите просте число. Отримується від серверу і зберігається локально.
let p;

// Відкрите просте число. Отримується від серверу і зберігається локально.
let g;

// Секретний ключ кліента. 
let secret = Math.floor(Math.random() * (100 - 2)) + 2;

// Відкритий ключ кліента.
let public;

// Спільне таємне число.
let sharedKey;

// Функція що оброблює повідомлення надісланні через сокет.
function onSocketMessage(message) {

    console.log('received: %s', message);

    const decoded = JSON.parse(message.data);

    // Шаг 1: сервер надіслав пару простих чисел, що використовуються для обчислення відкритого ключа кліента.
    if (decoded.p && decoded.g){
        p = decoded.p;
        g = decoded.g;

        public = Math.pow(g, secret) % p;
        
        let response = {
            request: "keys",
            a: public
        };

        // Клієент надсилає серверу публічний ключ.
        socket.send(JSON.stringify(response))
    }

    // Шаг 2: Сервер надіслав массив публічних ключів клієнтів, що використовується для обчислення спільного таємного числа.
    if (decoded.users){
        sharedKey = (decoded.users.reduce((acc, cur) => (acc * cur) % p) * public) % p;

        console.log("Shared key: " + sharedKey)
    }

  }

  function onSocketClose() {
  }