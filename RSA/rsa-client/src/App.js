import React, { useState, useEffect, useRef } from 'react';
import JSEncrypt from 'jsencrypt'
import './App.css';


function App() {

  const [publicKey, setPublicKey] = useState('')

  const [login, setLogin] = useState('')
  const [password, setPassword] = useState('')
  const [useEncryption, setUseEncryption] = useState(false)

  const [decryptedPassword, setDecryptedPassword] = useState(undefined)
  const [decryptedLogin, setDecryptedLogin] = useState(undefined)

  // 0: Головний компонент монтується. Клієнт запитує у сервера публічний ключа для шифрування, який клієт надійно зберігає.
  // Функція виклакається один раз, але у разі page hard-refresh буде виконан component re-mount, що призведе до повторного виклику функції
  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch('http://localhost:8080/public-key');
        
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }

        const result = await response.json();
        const key = result.key;

        console.log(key)

        setPublicKey(key);
      } catch (error) {
        console.error('Error fetching data:', error.message);
      }
    };

    fetchData();
  }, [])

  // 1:  Користувач ввів логін-пароль та натиснув кнопку "надіслати". Використовуючи публічний ключ шифруємо повідомлення і надсилаємо його до серверу.
  // Функція виклається довільну кількість раз.
  const handleSumbitButtonClick = async () => {

    // Використовуємо JSEncrypt (бібліотеки різні для клієнта та сервера через те, що використовуються різні платформи).
    const jsEncrypt = new JSEncrypt();
    jsEncrypt.setPublicKey(publicKey);

    let kpassword;
    let klogin;

    let local_login = login;
    let local_password = password;

    // Користувач має змогу не використовувати шифрування, задля демонстрації packet sniffing техніки.
    if (useEncryption){
      local_login = jsEncrypt.encrypt(login)
      local_password = jsEncrypt.encrypt(password)
    }
  
    let request = {
      useEncryption: useEncryption,
      login: local_login,
      password: local_password
    };

    let response;

    try {
      response = await fetch('http://localhost:8080/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      });
    } catch (error) {
      console.error('Error during POST request:', error.message);
    }

    // Сервер відповів дешифрованою парою логін-пароль
    const result = await response.json();

    setDecryptedLogin(result.login)
    setDecryptedPassword(result.password)

    console.log(result)
  }

  return (
    <div className="App">

      <div className='flex-container'>
        <div className='input-container'>
          <label>Login</label>
          <input type='text' value={login} onChange={(e) => setLogin(e.target.value)} />

          <label>Password</label>
          <input type='text' value={password} onChange={(e) => setPassword(e.target.value)} />

          <label>Use RSA</label>
          <input type='checkbox' value={useEncryption} onChange={(e) => setUseEncryption(!useEncryption)}/>
        </div>

        <div className='input-container'>
          <p>Decrypted login: {decryptedLogin}</p>
        </div>

        <div className='input-container'>
          <p>Decrypted password: {decryptedPassword}</p>
        </div>
        <div>
          <button className="send-button" onClick={handleSumbitButtonClick}>Send</button>
        </div>
      </div>
    </div>
  );
}

export default App;
