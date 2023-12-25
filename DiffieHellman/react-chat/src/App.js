import React, { useState, useEffect, useRef } from 'react';

import { AES, enc, lib } from "crypto-js";

const SERVER_URL = process.env.SERVER_URL || "localhost:8080";

function App() {  

  // Відкрите просте число. Отримується від серверу і зберігається локально.
  const p = useRef(undefined);

  // Відкрите просте число. Отримується від серверу і зберігається локально.
  const g = useRef(undefined);

  // Секретний ключ кліента. 
  const secret = useRef(Math.floor(Math.random() * (100 - 2)) + 2)

  // Відкритий ключ кліента
  const publicKey = useRef(undefined)

  // Спільне таємне число.
  const sharedKey = useRef(undefined)

  const socket = React.useRef(null)

  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState('');


  function onSocketOpen(){
    console.log('Connection with server established!')
  }

  // Функція що оброблює повідомлення надісланні через сокет.
  function onSocketMessage(message) {

    const decoded = JSON.parse(message.data);

    console.log(socket)
    console.log('received: %s', message.data);
  
    // 1: сервер надіслав пару простих чисел, що використовуються для обчислення відкритого ключа кліента.
    // Викликається при кожному підключенні до серверу
    if (decoded.p && decoded.g){
        p.current = decoded.p
        g.current = decoded.g

        let pk = Math.pow(g.current, secret.current) % p.current ;

        console.log(pk)
        publicKey.current = pk;
        
        let response = {
            request: "keys",
            a: pk
        };

        // Кліент надсилає серверу публічний ключ.
        socket.current.send(JSON.stringify(response))
    }

     // 2: Сервер надіслав массив публічних ключів клієнтів, що використовується для обчислення спільного таємного числа.
     // Викликається кожен раз при підключенні нового користувача, якщо загальна кількість користувачів > 1.
    if (decoded.users){
        let sk = (decoded.users.reduce((acc, cur) => (acc * cur) % p.current) * publicKey.current) % p.current;

        sharedKey.current = sk

        console.log("Shared key: " + sk)
    }

    // 3: Один з кліентів надіслав нове повідомлення серверу. Сервер переслав зашифрованне повідомлення усів кліентам.
    // Викликається на кожне нове повідомлення.
    if (decoded.message){
      // Згідно реалізації серверу, для шифрування використовується алгоритм AES.
      // Використовуємо спільне таємне число для дешифруванняю
      var iv = lib.WordArray.create(16);
      let decryptedMessage = AES.decrypt(decoded.message, sharedKey.current, { iv: iv }).toString(enc.Utf8);

      console.log("Received: " + decryptedMessage);

      setMessages((prevMessages) => [...prevMessages, decoded.author + ": " + decryptedMessage]);
    }
  }

  // Ініціалізація компоненту
  useEffect(() => {

    console.log("Opening socket")
  

    socket.current = new WebSocket(`ws://${SERVER_URL}`);
    socket.current.onopen = () => onSocketOpen();
    socket.current.onmessage = (msg) => onSocketMessage(msg);

    return () => {
      socket.current.close();
    };
  }, []);

  // Кліент надсилає повідомлення
  const handleSendMessage = () => {

    console.log("Original message:" + newMessage)

    var iv = lib.WordArray.create(16);
    var encryptedMessage = AES.encrypt(newMessage, sharedKey.current, { iv: iv }).toString();

    console.log("Encrypted message:" + encryptedMessage)

    let response = {
      request: "message",
      message: encryptedMessage
    };

    setNewMessage('');

    console.log(encryptedMessage)

    socket.current.send(JSON.stringify(response))

    setMessages((prevMessages) => [...prevMessages, "You: " + newMessage]);
  };

  return (
    <div className='flex-container'>
      <div className='chat-container'>
        <h1>My secret chat</h1>
        <div>
          {messages.map((message, index) => (
            <div key={index}>{message}</div>
          ))}
        </div>
        <div className='input-container'>
          <input
            type="text"
            className="input-box"
            value={newMessage}
            onChange={(e) => setNewMessage(e.target.value)}
          />
          <button className="send-button" onClick={handleSendMessage}>Send</button>
        </div>
      </div>
    </div>

  );
}

export default App;
