using System.Text;

namespace Caesar.Console;

public class VigenereCipher
{
    private readonly IList<char> alphabet;

    private readonly int alphabetLength;
    private readonly string specialChars = " ,.;!?1234567890";

    internal VigenereCipher(IReadOnlyList<char> alphabet)
    {
        this.alphabetLength = alphabet.Count;
        this.alphabet = alphabet.ToList();
    }

    public static VigenereCipher Create(IReadOnlyList<char> alphabet)
    {
        return new VigenereCipher(alphabet);
    }


    public string EncryptString(string originalString, string key)
    {
        var resultBuilder = new StringBuilder();

        var processedCharCount = 0;

        for (var i = 0; i < originalString.Length; i++)
        {
            var textChar = originalString[i];
            var keyChar = key[processedCharCount % (key.Length)];

            if (this.specialChars.Contains(textChar))
            {
                resultBuilder.Append(textChar);

                continue;
            }

            var textCaseAddition = !this.IsLower(textChar) ? 0 : this.alphabetLength - 1;
            var keyCaseAddition = !this.IsLower(keyChar) ? 0 : this.alphabetLength;


            var textCharIndex = this.alphabet.IndexOf((char)(textChar + textCaseAddition));
            var keyCharIndex = this.alphabet.IndexOf((char)(keyChar + keyCaseAddition));

            var encryptedIndex = (textCharIndex + keyCharIndex) % this.alphabet.Count;
            var encryptedChar = this.alphabet[encryptedIndex];

            resultBuilder.Append(encryptedChar);

            processedCharCount++;
        }

        return resultBuilder.ToString();
    }

    public string DecryptString(string encryptedString, string key)
    {
        var resultBuilder = new StringBuilder();

        var processedCharCount = 0;

        for (var i = 0; i < encryptedString.Length; i++)
        {
            var textChar = encryptedString[i];
            var keyChar = key[processedCharCount % (key.Length)];

            if (this.specialChars.Contains(textChar))
            {
                resultBuilder.Append(textChar);

                continue;
            }

            var textCaseAddition = !this.IsLower(textChar) ? 0 : this.alphabetLength - 1;
            var keyCaseAddition = !this.IsLower(keyChar) ? 0 : this.alphabetLength - 1;


            var textCharIndex = this.alphabet.IndexOf((char)(textChar + textCaseAddition));
            var keyCharIndex = this.alphabet.IndexOf((char)(keyChar + keyCaseAddition));

            var encryptedIndex = (textCharIndex - keyCharIndex + this.alphabetLength) % this.alphabet.Count;
            var encryptedChar = this.alphabet[encryptedIndex];

            resultBuilder.Append(encryptedChar);

            processedCharCount++;
        }

        return resultBuilder.ToString();
    }

    public bool IsLower(char @char) => @char < this.alphabet[0];
}