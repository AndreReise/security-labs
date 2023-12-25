import React from "react";
import { SHA1, enc } from "crypto-js";
import companyServerUrl from "../config";

const LiqPayButton = ({
    publicKey,
    privateKey,
    amount,
    currency,
    description = "test",
    orderId = Math.floor(1 + Math.random() * 900000000), // формуємо випадковий ідентифікатор замовлення
    title = "Payment",
    style,
    disabled,
    extra,
    ...props
  }) => {
    
    // Формуємо JSON документ згідно специфікації https://www.liqpay.ua/documentation/api/aquiring/widget/doc
    // Тип оплати - pay. Можливі значення: pay - платіж, hold - блокування коштів на рахунку відправника, subscribe - регулярний платіж, paydonate - пожертва
    // server_url: companyServerUrl + "/liqpay/result": URL API магазина для повідомлень про зміну статусу платежу
    const jsonString = {
      public_key: publicKey,
      version: "3",
      action: "pay",
      amount: amount,
      currency: currency,
      description: description,
      order_id: orderId,
      server_url: companyServerUrl + "/liqpay/result",
      ...props,
    };
  
    const utf8_to_b64 = (str) => {
      console.log("str", str);
      return window.btoa(unescape(encodeURIComponent(str)));
    };
  
    // за специфікацією data має бути закодована у BASE64 
    // https://www.liqpay.ua/documentation/api/aquiring/widget/doc
    const data = utf8_to_b64(JSON.stringify(jsonString));

    // Унікальний підпис кожного запиту base64_encode( sha1( private_key + data + private_key) 
    // https://www.liqpay.ua/documentation/api/aquiring/widget/doc
    const signString = privateKey + data + privateKey;
    const signature = SHA1(signString).toString(enc.Base64);  
  
    return (
      <form
        method="POST"
        action="https://www.liqpay.ua/api/3/checkout"
        acceptCharset="utf-8"
        style={{ ...style }}
      >
        <input type="hidden" name="data" value={data} />
        <input type="hidden" name="signature" value={signature} />
        {extra || (
          <button
            disabled={disabled}
          >
            <img
              src="https://static.liqpay.ua/buttons/logo-small.png"
              name="btn_text"
            />
            <span>
              {title} {amount} {currency}
            </span>
          </button>
        )}
      </form>
    );
  };
  
  export default LiqPayButton;