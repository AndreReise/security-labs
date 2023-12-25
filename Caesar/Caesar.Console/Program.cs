using System.Text;
using System.Text.Unicode;
using Caesar.Console;
using Caesar.Console.Alphabets;


#region block1

//var encryptedText = "жиш лмикизн! Жязщ чми мъе ьивжнмгёи. Лзъсъёъ щ ь йген яжн лмъёъ ьиьлш оёгкмиьъмц л";
//var key = 27;

//var caesarCipher = CaesarCipher.Create(RussianAlphabet.Instance);

//var decryptedText = caesarCipher.DecryptString(encryptedText, key);
//var encryptionVerification = caesarCipher.EncryptString(decryptedText, 27);

//Console.WriteLine($"Original text: {encryptedText}");
//Console.WriteLine($"Decrypted text: {decryptedText}");
//Console.WriteLine($"Encrypted again text: {encryptionVerification}");

#endregion


#region block2
//var vigenereCipher = VigenereCipher.Create(RussianAlphabet.Instance);

////var originalText = "Асимметричная криптография стала элегантным решением задачи распределения ключей. Но как это часто бывает, то что помогло устранить одну ";
//var encryptedText = "прщвжх ом ьпьчщрциф ётът сбнмж рапьмлфск идф башср н 1984 лопь. ощ шррмпъфотсл, гыо рълф йы ычянсллъь нчзшчжщчсюе иэшочезъкаюе в цичрътнн оюуржыооч кчжчл смк слф шогыондй лмрръ ачссж, ыо иыо чсшффо мд сччжщью ыщовндящу льтрцтфэицицфс вэзкъло эхыэфа. пчлоче нщешз ипня димфща ъътлкачисз ксрло чсшз урлъинчй цщиыыоощаасчръкът гъфончлъхкът, нъ к 2000 гъму, мфаочдлщя ъмнът иукеэынът укрвфхоэыи н ёлчспюсчръкът кьспючгьиффс, ипню ямаччсз коыфоюстз к жфрнз.";

//var key = "Али";

//var decryptedText = vigenereCipher.DecryptString(encryptedText, key);
////var encryptionVerification = vigenereCipher.EncryptString(originalText, key);


//Console.WriteLine($"Original text: {encryptedText}");
//Console.WriteLine($"Decrypted text: {decryptedText}");
////Console.WriteLine($"Encrypted again text: {encryptionVerification}");
#endregion

#region block3

var vigenereCipher = VigenereCipher.Create(RussianAlphabet.Instance);

var encryptedText = "ыникжы ъй лшьэенёсф лючв ъбушд аипвшидък опс ришчь к 1984 ычпв. ъц зщртычдчтчч, акч рачс щд ыэккбфлаз кжршэтцжъюк фъзччкучъиюк н ушараюкэ чющьдкчоэ цфцал чшз бфф юъакчнйх иьщра лфбъж, бъ ёкч ччдсдч мй эфжпщвй шичвупьиь лвюнёыфгфушяфч нъчуъсъ ъедэъл. мжфоэр киншн фмэз дошсии ъаюиъичоэе ъърсъ фббз щьийснэх уисыбълииачгнйуъш очдчнэччеуъш, щч ъ 2000 лътя, йдиоэпииз ътщчв сурръкцъш язакфыъъкс н лчфбшючгнйуъш цщбшюэощшэфч, фмэж ятлфжъз ръшдчючюе ъ пфцще.";

var key = "Лишило";

var decryptedText = vigenereCipher.DecryptString(encryptedText, key);

Console.WriteLine($"Decrypted text: {decryptedText}");

#endregion
