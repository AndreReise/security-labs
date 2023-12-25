using System.Text;

namespace Caesar.Console;

public class CaesarCipher
{
    private string specialChars = " ,.;!?";

    private readonly int alphabetLength;
    private readonly IList<char> alphabet;

    internal CaesarCipher(IReadOnlyList<char> alphabet)
    {
        this.alphabetLength = alphabet.Count;
        this.alphabet = alphabet.ToList();
    }

    public static CaesarCipher Create(IReadOnlyList<char> alphabet)
    {
        return new CaesarCipher(alphabet);
    }

    public string DecryptString(string encryptedString, int key)
    {
        var shift = key % this.alphabetLength;

        var sb = new StringBuilder();

        foreach (var encryptedChar in encryptedString)
        {
            // special case
            if (this.specialChars.Contains(encryptedChar))
            {
                sb.Append(encryptedChar);

                continue;
            }

            var caseAddition = !this.IsLower(encryptedChar) ? 0 : this.alphabetLength;


            var encryptedIndex = this.alphabet.IndexOf((char)(encryptedChar + caseAddition));

            var decryptedIndex = (encryptedIndex + this.alphabetLength - shift) % this.alphabetLength;
            var decryptedChar = this.alphabet[decryptedIndex];


            sb.Append((char)(decryptedChar  -caseAddition));
        }

        return sb.ToString();
    }

    public bool IsLower(char @char) => @char < this.alphabet[0];

    public string EncryptString(string originalString, int key)
    {
        var sb = new StringBuilder();


        foreach (var @char in originalString)
        {
            // special case
            if (this.specialChars.Contains(@char))
            {
                sb.Append(@char);

                continue;
            }

            var charIndex = this.alphabet.IndexOf(@char);

            var encryptedIndex = (charIndex + key) % this.alphabetLength;
            var encryptedChar = this.alphabet[encryptedIndex];

            sb.Append((char)encryptedChar);
        }

        return sb.ToString();
    }
}