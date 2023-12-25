import React, { useState, useEffect } from "react";
import LiqPayButton from "./LiqPayButton";
import {companyServerUrl, keys} from '../config';

  
  export function MainPage({ isConnected }) {
    const [data, setData] = useState([]);
  
    // при component-mount ообимо запит на сервер для отримання списку товарів
    // викликається 1 раз
    useEffect(() => {
    
      fetch(companyServerUrl + "/products", {
        Method: "GET",
        Headers: {
          Accept: "application.json",
          "Content-Type": "application/json",
        },
        Cache: "default",
      }).then((d) => {
        readData(d);
      });
    });
  
    const readData = async (data) => {
      let json = await data.text();
  
      console.log("first", json);
      setData(JSON.parse(json));
    };
  
    const utf8_to_b64 = (str) => {
      console.log("str", str);
      return window.btoa(unescape(encodeURIComponent(str)));
    };
  
    // відображаємо список товарів з кнопкої для редіректу на LiqPay віджет для оплати замовлення.
    return (
      <div>
        <h1>Товари</h1>
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Назва</th>
              <th>Опис</th>
              <th>Ціна</th>
              <th>Кількість</th>
            </tr>
          </thead>
          <tbody>
            {data.map((product) => (
              <tr key={product.id}>
                <td>{product.id}</td>
                <td>{product.name}</td>
                <td>{product.description}</td>
                <td>₴{product.price.toFixed(2)}</td>
                <td>{product.quantity}</td>
                <td>
                  <LiqPayButton
                    publicKey={keys.publicKey}
                    privateKey={keys.privateKey}
                    amount={product.price.toFixed(2)}
                    description={product.name}
                    currency="UAH"
                    orderId={Math.floor(1 + Math.random() * 900000000)}
                    product_description={utf8_to_b64(product.name)}
                    style={{ margin: "8px" }}
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }
  