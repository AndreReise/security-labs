using System.Collections;

namespace Caesar.Console.Alphabets;

public class RussianAlphabet : IReadOnlyList<char>
{
    // https://www.unicode.org/charts/PDF/U0400.pdf
    private static readonly IList<char> alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"
        .ToLower()
        .ToCharArray();

    public static RussianAlphabet Instance { get; } = new RussianAlphabet();
    public IEnumerator<char> GetEnumerator()
    {
        return alphabet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Count => alphabet.Count;

    public char this[int index] => alphabet[index];
}