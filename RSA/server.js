const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');

const crypto = require('crypto');

const app = express();
const port = 8080;

const { publicKey, privateKey } = crypto.generateKeyPairSync('rsa', {
    modulusLength: 2048,
    publicKeyEncoding: {
        type: 'spki', // simple pk infrastructure
        format: 'pem'
    },
    privateKeyEncoding: {
        type: 'pkcs8',
        format: 'pem'
    }
});

app.use(cors());

app.use(bodyParser.json());

app.get('/public-key', (req, res) => {

    console.log(req.body);

    let response = {
        key: publicKey
    }

    res.json(response);
});

app.post('/login', (req, res) => {

    console.log(req.body);

    const useEncryption = req.body.useEncryption;
    const login = req.body.login;
    const password = req.body.password;

    let response;

    if (!useEncryption){
        response = {
            login: login,
            password: password
        }
    }else{
        const decryptedLogin = crypto.privateDecrypt({ key: privateKey, padding: crypto.constants.RSA_PKCS1_PADDING }, Buffer.from(login, 'base64')).toString('utf-8');
        const decryptedPassword = crypto.privateDecrypt({ key: privateKey, padding: crypto.constants.RSA_PKCS1_PADDING }, Buffer.from(password, 'base64')).toString('utf-8');
    
        response = {
            login: decryptedLogin,
            password: decryptedPassword
        }
    }

 
    res.json(response)
});

app.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
});
