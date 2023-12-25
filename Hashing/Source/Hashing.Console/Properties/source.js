import { useEffect, useRef, useState } from "react"
import JSEncrypt from "jsencrypt"


function App() {
  const loginRef = useRef(null)
  const passwordRef = useRef(null)
  const [result, setResult] = useState({})
  const [publicKey, setPublicKey] = useState("");
  const [keysId, setKeysId] = useState("");
  const [clientKeys, setClientKeys] = useState({});
  const [recievedMessage, setRecievedMessage] = useState();
  const [recievedMessageD, setRecievedMessageD] = useState();

  useEffect(() => {
    callForSessionKey()
    createClientKeyPair()
  }, [])

  const callForSessionKey = async () => {
    const response = await fetch("http://localhost:5181/api/login", {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        }
      })
      let res = await response.json()
      setPublicKey(res.publicKey);
      setKeysId(res.keysId);
  }

  const createClientKeyPair = () => {
    const encrypt = new JSEncrypt();

    // Генеруємо пару ключів
    encrypt.getKey();

    // Отримуємо публічний і приватний ключі
    const newKeyPair = {
      public: encrypt.getPublicKey(),
      private: encrypt.getPrivateKey()
    }
    setClientKeys(newKeyPair)
  }

  // Encrypt request data
  const encryptData = data => {
    let encryptor = new JSEncrypt()
    encryptor.setPublicKey(publicKey)
    return encryptor.encrypt(data)
  }

  const decryptData = data => {
    let encryptor = new JSEncrypt()
    encryptor.setPrivateKey(clientKeys.private)
    console.log('data', data)
    console.log('encryptor.decrypt(data)', encryptor.decrypt(data))
    return encryptor.decrypt(data)
  }

  const onSubmit = async e => {
    try {
      e.preventDefault()
      setResult({})
      if (!loginRef.current?.value || !passwordRef.current?.value) {
        return
      }
      
      let requestBody = prepareRequest()
      let response = await fetch("http://localhost:5181/api/login", {
        method: "POST",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json"
        }
      });

      let respData = await response.json()
      console.log(respData)
      setRecievedMessage(respData.message)
      setRecievedMessageD(decryptData(respData.message))
      alert(decryptData(respData.message))

    } catch (err) {
      alert(String(err))
    }
  }

  const prepareRequest = () => {
    let data = JSON.stringify({
      login: loginRef.current.value,
      password: passwordRef.current.value
    })
    let encryptedData = encryptData(data)
    let requestBody = {
      keysId: keysId,
      encrypted: encryptedData,
      clientKey: clientKeys.public,
    }

    setResult({
      message: data,
      encrypted: encryptedData
    })

    return requestBody;
  }

  return (
    <div >
        <div class="row justify-content-center">
          <h2>
            Simple form to demonstrate RSA Encryption between a React and ASP.NET applications
          </h2>
            <div class="col-md-6">
                <div class="card">
                    <div class="card-header">Login form</div>
                    <div class="card-body">
                        <form onSubmit={onSubmit}>
                            <div class="form-group">
                                <label>Login</label>
                                <input type="text" class="form-control" ref={loginRef} placeholder="Enter login"/>
                            </div>
                            <div class="form-group">
                                <label>Password</label>
                                <input type="text" class="form-control" ref={passwordRef} placeholder="Enter password"/>
                            </div>
                            <button type="submit" class="btn btn-primary">Submit</button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
        <button type="submit" class="btn btn-warning" onClick={callForSessionKey}>Get new public key</button>
        <div style={{width: "90%", wordWrap: "break-word" }}>
          <p><strong>Public key:</strong> {publicKey}</p>
          <p><strong>Sent Message:</strong> {result.message}</p>
          <p><strong>Recieved Message:</strong> {recievedMessage}</p>
          <p><strong>Recieved Message (decrypted):</strong> {recievedMessageD}</p>
          <p><strong>Encrypted:</strong> {result.encrypted}</p>
          <p><strong>Client private key:</strong> {clientKeys.private}</p>
          <p><strong>Client public key:</strong> {clientKeys.public}</p>
      </div>
    </div>
  )
}

export default App