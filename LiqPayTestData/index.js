import express from "express";
import fs from "fs";

const app = express();

app.use((req, res, next) => {
  res.setHeader("Access-Control-Allow-Origin", "http://localhost:3000");
  res.setHeader(
    "Access-Control-Allow-Methods",
    "GET, POST, OPTIONS, PUT, PATCH, DELETE",
  );
  res.setHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");
  res.setHeader("Access-Control-Allow-Credentials", "true");
  next();
});

app.use(express.urlencoded({ extended: false }));

// Endpoint для отримання списку товарів. Товари представленні у JSON форматі та зберігаються у файлі data.json.
// Може викликатися довільну кількість разів.
app.get("/", (req, res) => {
  var text = fs.readFileSync("data.json");
  res.send(text);
});

// Після обробки операції процесінгом LiqPay і отриманням кінцевого статусу, на сервер буде відправлений
// POST запит з двома параметрами data і signature, де:
// data - json рядок з параметрами APIs закодований функцією base64, base64_encode( json_string ),
// signature - унікальний підпис кожного запиту base64_encode( sha1( private_key + data + private_key) ),
// функція виклидається довільну кількість раз (1 раз на процесинг оплати)
app.post("/liqpay/result", (req, res) => {
  var jsonString = atob(req.body.data);
  var response = JSON.parse(jsonString);

  // Зберігаємо усі успішну транзакції у processed.json файлі
  var processed = JSON.parse(fs.readFileSync("processed.json"));


  console.log("Request accepted for: " + response.order_id);
  
  if (response.status !== "success") 
  {
    console.log("Status not success");
  }
  else if (!processed[response.order_id]) 
  {
    processed[response.order_id] = response.order_id;

    fs.writeFileSync("processed.json", JSON.stringify(processed));
  }
  else 
  {
    console.log("Such order has already been handled");

    return;
  }

  var dataText = fs.readFileSync("data.json");
  var data = JSON.parse(dataText);

  var item = data.find(
    (e) => utf8_to_b64(e.name) === response.product_description,
  );
  item.quantity--;

  fs.writeFileSync("data.json", JSON.stringify(data));
});

app.listen({ port: 3000, host: "0.0.0.0" }, () => {
  console.log("Express server initialized");
});

const utf8_to_b64 = (str) => {
  return btoa(unescape(encodeURIComponent(str)));
};
