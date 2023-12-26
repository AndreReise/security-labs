using System.Globalization;
using System.Text;

namespace Lab1;

public class Encryptor
{
    // Масив для зберігання 64 біт об'єднанної лівої та правої частин перед [фінальною перестановкою] в дешифруванні.
    private readonly int[] attachedct = new int[64];

    // Масив для зберігання 64 біт об'єднанної лівої та правої частин перед [фінальною перестановкою] в шифруванні.
    private readonly int[] attachedpt = new int[64];

    // Масив для зберігання 56 біт ключа. Ключ отримують з початкового ключа розміром 64 біт наступним чином:
    // Вісім бітів, що знаходять в позиціях 8, 16, 24, 32, 40, 48, 56, 64 додаються в ключ k таким чином щоб кожен байт містив непарне число одиниць.
    private readonly int[] changedkey = new int[56];

    // буфер для зберігання шифротексту
    private readonly int[] ciphertextbin = new int[5000];

    // Число звсувів на і-й раунд (лівий зсув)
    private readonly int[] clst = {1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1};

    // Compression Permutation Table  
    private readonly int[] cpt =
    {
        14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10,
        23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2,
        41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48,
        44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
    };

    // Число звсувів на і-й раунд (правий зсув)
    private readonly int[] crst = {1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1, 1};

    // буфер для зберігання шифротексту при дешифруванні
    private char[] ctca;

    // буфер для 48 бітів результату розширення блоку
    private readonly int[] ctExpandedLPT = new int[48];

    // буфер для 64 бітів результату початкової перестановки
    private readonly int[] ctextbitslice = new int[64];

    // буфер для запису результатів маніпуляцій з 32 бітами лівої части
    private readonly int[] ctLPT = new int[32];

    // буфер для запису результатів ПЕРЕстановки 32 бітів лівої части
    private readonly int[] ctPBoxLPT = new int[32];

    // буфер для запису результатів маніпуляцій з 32 бітами правої части
    private readonly int[] ctRPT = new int[32];

    // буфер для запису результатів ПІДстановки 32 бітів правої части
    private readonly int[] ctSBoxLPT = new int[32];

    // буфер для перестановки 28 біт ключа для обчислення лівого циклічного зрушення
    private readonly int[] CKey = new int[28];

    // буфер для перестановки 28 біт ключа для обчислення правого циклічного зрушення
    private readonly int[] DKey = new int[28];

    // Expansion Permutation Table  
    private readonly int[] ept =
    {
        32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9,
        8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17,
        16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25,
        24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1
    };

    // 64 бітна таблиця з індексами елементів фінальної перестановки
    private readonly int[] fp =
    {
        40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31,
        38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,
        36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27,
        34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25
    };

    // буфер для збереження 64 бітів результат кінцевої перестановки (дешифрування)
    private readonly int[] fpct = new int[64];

    // буфер для збереження 64 бітів результат кінцевої перестановки (шифрування)
    private readonly int[] fppt = new int[64];

    // 64 бітна таблиця з індексами елементів початкової перестановки
    private readonly int[] ip =
    {
        58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
        62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
        57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
        61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7
    };

    // буфер для збереження 64 бітів результат початкової перестановки (дешифрування)
    private readonly int[] ipct = new int[64];

    // буфер для збереження 64 бітів результат початкової перестановки (шифрування)
    private readonly int[] ippt = new int[64];

    // буфер для збереження результату шифрування або дешифрування у текстовому виді (64 біт)
    private char[] kca;

    // буфер для збереження 64 біт результату шифрування або дешифрування у текстовому виді
    private readonly int[] keybin = new int[64];

    // Таблиця перестановки P-box
    private readonly int[] pbox =
    {
        16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10,
        2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25
    };

    // буфер для збегінання бітів тексту на вході 
    private readonly int[] plaintextbin = new int[5000];

    // 64 бітний буфер для тексту на вход
    private char[] ptca;

    // 48 бітний буфер для зберігання результату перестановки розширення
    private readonly int[] ptExpandedRPT = new int[48];

    private readonly int[] ptextbitslice = new int[64];

    // буфер для збереження 32біт лівої частини блоку. Буфер використовується для зберігання проміжного результату між етапами раунда алгоритму.
    private readonly int[] ptLPT = new int[32];

    // буфер для збереження 32біт резульату перестановки правої частини блоку. 
    private readonly int[] ptPBoxRPT = new int[32];

    // буфер для збереження 32біт правої частини блоку. Буфер використовується для зберігання проміжного результату між етапами раунда алгоритму.
    private readonly int[] ptRPT = new int[32];

    private readonly int[] ptSBoxRPT = new int[32];

    // Таблиці підстановки (S-box)
    private readonly int[,] sbox = new int[8, 64]
    {
        {
            14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
            0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
            4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
            15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13
        },
        {
            15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
            3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
            0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
            13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9
        },
        {
            10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
            13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
            13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
            1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12
        },
        {
            7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
            13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
            10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
            3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14
        },
        {
            2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
            14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
            4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
            11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3
        },
        {
            12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
            10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
            9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
            4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13
        },
        {
            4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
            13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
            1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
            6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12
        },
        {
            13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
            1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
            7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
            2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11
        }
    };

    // буфер для збереження 48 біт раундового ключа
    private readonly int[] compressedkey = new int[48];

    // буфер для збереження 56 біт резульату зсуву ключа
    private readonly int[] shiftedkey = new int[56];

    // буфер для збереження тимчасовиї 32біт лівої частини блоку. 
    private readonly int[] tempLPT = new int[32];

    // буфер для збереження тимчасових 32біт правої частини блоку. 
    private readonly int[] tempRPT = new int[32];

    // буфер для збереженні поточно обраної таблиці підстановки
    private readonly int[] tempsboxarray = new int[4];

    // буфер для збереження результата Xor лівої частини блоку
    private readonly int[] XoredLPT = new int[48];

    // буфер для збереження результата Xor правої частини блоку
    private readonly int[] XoredRPT = new int[48];

    private int GetASCII(char ch)
    {
        int n = ch;
        return n;
    }

    /// <summary>
    /// Метод-помічник для конверсії тексту у бітове представлення.
    /// Викликається при кожному виклкику <see cref="Encrypt"/> та <see cref="Decrypt"/>.
    /// </summary>
    /// <param name="charArray">Масив символів</param>
    /// <param name="savedArray">Масив бітів</param>
    private int ConvertTextToBits(char[] charArray, int[] savedArray)
    {
        var j = 0;
        var bitsPerChar = 16;

        for (var i = 0; i < charArray.Length; ++i)
        {
            var bitArray = BitArray.ToBits(this.GetASCII(charArray[i]), bitsPerChar);
            j = i * bitsPerChar;
            this.AssignArray1ToArray2b(bitArray, savedArray, j);
        }

        return j + bitsPerChar;
    }

    /// <summary>
    /// Метод-помічник для конверсії тексту hex значень у бітове представлення.
    /// Викликається при кожному виклкику <see cref="Encrypt"/> та <see cref="Decrypt"/>.
    /// </summary>
    /// <param name="charArray">Масив символів</param>
    /// <param name="savedArray">Масив бітів</param>
    private int ConvertHexToBits(char[] charArray, int[] savedArray)
    {
        var j = 0;
        var bitsPerChar = 4;

        for (var i = 0; i < charArray.Length; ++i)
        {
            var bitArray = BitArray.ToBits(int.Parse(charArray[i].ToString(), NumberStyles.HexNumber), bitsPerChar);
            j = i * bitsPerChar;
            this.AssignArray1ToArray2b(bitArray, savedArray, j);
        }

        return j + bitsPerChar;
    }

    /// <summary>
    /// Метод для виконання Value-copy двох масивів.
    /// </summary>
    /// <param name="array1">Масив А</param>
    /// <param name="array2">Масив Б</param>
    /// <param name="fromIndex">Індекс з якого проводити копіювання.</param>
    private void AssignArray1ToArray2b(int[] array1, int[] array2, int fromIndex)
    {
        int x, y;
        for (x = 0, y = fromIndex; x < array1.Length; ++x, ++y)
            array2[y] = array1[x];
    }

    /// <summary>
    /// Додає нульові біти, якщо довжина block-payload менша за 64 біт.
    /// Викликається довільну кількість раз, якщо виконується вищезазначена умова.
    /// </summary>
    /// <param name="appendedArray">Масив</param>
    /// <param name="length">Довжина масива</param>
    /// <returns></returns>
    private int AppendZeroes(int[] appendedArray, int length)
    {
        int zeroes;
        const int blockSize = 64;

        if (length % blockSize != 0)
        {
            zeroes = blockSize - length % blockSize;

            for (var i = 0; i < zeroes; ++i) appendedArray[length++] = 0;
        }

        return length;
    }

    /// <summary>
    /// Метод для генерації раундового ключа.
    /// Викликається кожен раз при визові <see cref="Encrypt"/> або <see cref="Decrypt"/>.
    /// </summary>
    private void Discard8thBitsFromKey()
    {
        const int bitsPerByte = 8;
        const int keySize = 64;

        for (int i = 0, j = 0; i < keySize; i++)
        {
            if ((i + 1) % bitsPerByte == 0) continue;
            this.changedkey[j++] = this.keybin[i];
        }
    }

    /// <summary>
    /// Метод для присвоєння перестановки біт ключа.
    /// Викликається кожен раз при визові <see cref="Encrypt"/> або <see cref="Decrypt"/>.
    /// </summary>
    private void AssignChangedKeyToShiftedKey()
    {
        for (var i = 0; i < 56; i++) this.shiftedkey[i] = this.changedkey[i];
    }

    /// <summary>
    /// Метод для обчислення початкової перестановки.
    /// Викликається кожен раз при визові <see cref="Encrypt"/> або <see cref="Decrypt"/>.
    /// </summary>
    private void InitialPermutation(int[] sentarray, int[] savedarray)
    {
        int tmp;
        for (var i = 0; i < 64; i++)
        {
            tmp = this.ip[i];
            savedarray[i] = sentarray[tmp - 1];
        }
    }

    /// <summary>
    /// Метод для розбиття 64 бітного блоку на 32-бітних лівого і правого блоків.
    /// Викликається кожен раз при визові <see cref="Encrypt"/> або <see cref="Decrypt"/>.
    /// </summary>
    private void DivideIntoLPTAndRPT(int[] sentarray, int[] savedLPT, int[] savedRPT)
    {
        for (int i = 0, k = 0; i < 32; i++, ++k) savedLPT[k] = sentarray[i];

        for (int i = 32, k = 0; i < 64; i++, ++k) savedRPT[k] = sentarray[i];
    }

    /// <summary>
    /// Метод для збереження результату раунду.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void SaveTemporaryHPT(int[] fromHPT, int[] toHPT)
    {
        for (var i = 0; i < 32; i++) toHPT[i] = fromHPT[i];
    }

    /// <summary>
    /// Метод для розбиття 56 бітного ключа на 28-бітні блоки.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void DivideIntoCKeyAndDKey()
    {
        for (int i = 0, j = 0; i < 28; i++, ++j) this.CKey[j] = this.shiftedkey[i];

        for (int i = 28, j = 0; i < 56; i++, ++j) this.DKey[j] = this.shiftedkey[i];
    }

    /// <summary>
    /// Метод для обчислення лівого циклічного зсуву. Викликається на кожен раунд DES.
    /// Викликається кожен раз на 16 раундів шифрування.
    /// </summary>
    private void CircularLeftShift(int[] HKey)
    {
        int i, FirstBit = HKey[0];
        for (i = 0; i < 27; i++) HKey[i] = HKey[i + 1];
        HKey[i] = FirstBit;
    }

    /// <summary>
    /// Метод для об'єднання лівої і правої половини ключа, створюючи підключ для даного раунду.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void AttachCKeyAndDKey()
    {
        var j = 0;
        for (var i = 0; i < 28; i++) this.shiftedkey[j++] = this.CKey[i];

        for (var i = 0; i < 28; i++) this.shiftedkey[j++] = this.DKey[i];
    }

    /// <summary>
    /// Метод для розширення правої половини тексту на 48 бітів за допомого таблиці розширення.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void CompressionPermutation()
    {
        int temp;
        for (var i = 0; i < 48; i++)
        {
            temp = this.cpt[i];
            this.compressedkey[i] = this.shiftedkey[temp - 1];
        }
    }

    /// <summary>
    /// Метод дял перестановки 48 бітного розширення.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void ExpansionPermutation(int[] HPT, int[] ExpandedHPT)
    {
        int temp;
        for (var i = 0; i < 48; i++)
        {
            temp = this.ept[i];
            ExpandedHPT[i] = HPT[temp - 1];
        }
    }

    /// <summary>
    /// Метод для виконання операції XOR між стиснутим ключем і розширеною правою половиною тексту.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void XOROperation(int[] array1, int[] array2, int[] array3, int SizeOfTheArray)
    {
        for (var i = 0; i < SizeOfTheArray; i++) array3[i] = array1[i] ^ array2[i];
    }

    /// <summary>
    /// Метод-хєлпер для <see cref="SBoxSubstitution"/>.
    /// </summary>
    private void AssignSBoxHPT(int[] temparray, int[] SBoxHPTArray, int fromIndex)
    {
        var j = fromIndex;
        for (var i = 0; i < 4; i++) SBoxHPTArray[j++] = this.tempsboxarray[i];
    }

    /// <summary>
    /// Метод для Використання таблиці S-Box для заміни результатів XOR.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void SBoxSubstitution(int[] xoredHPT, int[] sBoxHPT)
    {
        int j = 0, q = 0;

        for (var i = 0; i < 48; i += 6)
        {
            int[] rowBits = {xoredHPT[i], xoredHPT[i + 5]};
            var rowIndex = BitArray.ToDecimal(rowBits);

            int[] columnBits = {xoredHPT[i + 1], xoredHPT[i + 2], xoredHPT[i + 3], xoredHPT[i + 4]};
            var columnIndex = BitArray.ToDecimal(columnBits);

            var t = 16 * rowIndex + columnIndex;

            var sBoxValue = this.sbox[j++, t];
            var tempSBoxArray = BitArray.ToBits(sBoxValue, 4);

            var r = q * 4;

            this.AssignSBoxHPT(tempSBoxArray, sBoxHPT, r);

            ++q;
        }
    }


    /// <summary>
    /// Метод для Виконання перестановки P-Box для отримання нової правої половини тексту.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void PBoxPermutation(int[] SBoxHPT, int[] PBoxHPT)
    {
        int temp;
        for (var i = 0; i < 32; i++)
        {
            temp = this.pbox[i];
            PBoxHPT[i] = SBoxHPT[temp - 1];
        }
    }

    /// <summary>
    /// Метод для заміни лівої половини тексту на тимчасову праву половину.
    /// Викликається кожен раз на 16 раундів шифрування/дешифрування.
    /// </summary>
    private void Swap(int[] tempHPT, int[] HPT)
    {
        for (var i = 0; i < 32; i++) HPT[i] = tempHPT[i];
    }

    /// <summary>
    /// Метод для обічислення результату проведення 16 раундів алгоритму DES.
    /// Викликаєтьяс 1 раз на enc/dec.
    /// </summary>
    private void SixteenRounds()
    {
        int n;

        Console.WriteLine($"Entropy before: {this.CalcEntropy()}");

        for (var i = 0; i < 16; i++)
        {
            // 1. Зберігання правої половини тексту ptRPT в тимчасовому масиві tempRPT
            this.SaveTemporaryHPT(this.ptRPT, this.tempRPT);

            // 2.Визначення кількості циклічних зсувів(n) для даного раунду згідно з таблицею циклічних зсувів clst.
            n = this.clst[i];

            // 3. Розділення ключа на ліву половину CKey і праву половину DKey.
            this.DivideIntoCKeyAndDKey();

            // 4. Виконання циклічних зсувів лівої і правої половини ключа на n позицій.
            for (var j = 0; j < n; j++)
            {
                this.CircularLeftShift(this.CKey);
                this.CircularLeftShift(this.DKey);
            }

            // 5. Об'єднання лівої і правої половини ключа, створюючи підключ для даного раунду.
            this.AttachCKeyAndDKey();

            // 6. Розширення правої половини тексту на 48 бітів за допомогою таблиці розширення.
            this.CompressionPermutation();

            // 7. Виконання операції XOR між стиснутим ключем і розширеною правою половиною тексту.
            this.ExpansionPermutation(this.ptRPT, this.ptExpandedRPT);
            this.XOROperation(this.compressedkey, this.ptExpandedRPT, this.XoredRPT, 48);

            // 8. Використання таблиці S-Box для заміни результатів XOR.
            this.SBoxSubstitution(this.XoredRPT, this.ptSBoxRPT);

            // 9. Виконання перестановки P-Box для отримання нової правої половини тексту.
            this.PBoxPermutation(this.ptSBoxRPT, this.ptPBoxRPT);

            // 10.Виконання операції XOR між результатом P-Box перестановки і лівою половиною тексту.
            this.XOROperation(this.ptPBoxRPT, this.ptLPT, this.ptRPT, 32);

            // 11.Заміна лівої половини тексту на тимчасову праву половину.
            this.Swap(this.tempRPT, this.ptLPT);

            // 12.Виведення значення ентропії для оцінки якості шифрування після кожного раунду.
                Console.WriteLine($"Entropy on round {i + 1}: {this.CalcEntropy()}");
        }
    }

    /// <summary>
    /// Метод для поєднання лівої та правох частини. Викликається один раз на enc/dec при отриманні результату обробки блоку.
    /// </summary>
    private void AttachLPTAndRPT(int[] savedLPT, int[] savedRPT, int[] AttachedPT)
    {
        var j = 0;
        for (var i = 0; i < 32; i++) AttachedPT[j++] = savedLPT[i];

        for (var i = 0; i < 32; i++) AttachedPT[j++] = savedRPT[i];
    }

    /// <summary>
    /// Метод для проведення фінальної перестановки.Викликається один раз на enc/dec при отриманні результату обробки блоку.
    /// </summary>
    private void FinalPermutation(int[] fromPT, int[] toPT)
    {
        int temp;
        for (var i = 0; i < 64; i++)
        {
            temp = this.fp[i];
            toPT[i] = fromPT[temp - 1];
        }
    }

    /// <summary>
    /// Метод-помічник для старту процесу шифрування.
    /// </summary>
    private void StartEncryption()
    {
        this.InitialPermutation(this.ptextbitslice, this.ippt);

        this.DivideIntoLPTAndRPT(this.ippt, this.ptLPT, this.ptRPT);

        this.AssignChangedKeyToShiftedKey();

        this.SixteenRounds();

        this.AttachLPTAndRPT(this.ptLPT, this.ptRPT, this.attachedpt);

        this.FinalPermutation(this.attachedpt, this.fppt);
    }

    /// <summary>
    /// Метод для конверсії бітового масиву на текстове представлення.
    /// </summary>
    private string ConvertBitsToText(int[] sentArray, int len)
    {
        var finalText = new StringBuilder();
        var tempBitArray = new int[16];

        for (var i = 0; i < len; i += 16)
        {
            Array.Copy(sentArray, i, tempBitArray, 0, 16);

            var decimalValue = BitArray.ToDecimal(tempBitArray);

            if (decimalValue == 0)
                break;

            finalText.Append((char) decimalValue);
        }

        return finalText.ToString();
    }

    /// <summary>
    /// Конверсія бітового масиву на тестове представлення.
    /// </summary>
    private string ConvertBitsToIntSequenceStr(int[] sentArray, int len)
    {
        var finalText = new StringBuilder();
        var tempBitArray = new int[16];

        for (var i = 0; i < len; i += 16)
        {
            Array.Copy(sentArray, i, tempBitArray, 0, 16);

            var decimalValue = BitArray.ToDecimal(tempBitArray);

            if (decimalValue == 0)
                break;

            finalText.Append($"{decimalValue} ");
        }

        return finalText.ToString();
    }


    public string Encrypt(string plaintext, string key, bool useHexKey = false)
    {
        string ciphertext = null;

        if (this.IsWeak(key, useHexKey))
            throw new ArgumentException("the key is too weak.");

        this.ptca = plaintext.ToCharArray();
        this.kca = key.ToCharArray();
        int j, k;

        var st = this.ConvertTextToBits(this.ptca, this.plaintextbin);

        var fst = this.AppendZeroes(this.plaintextbin, st);

        if (useHexKey)
        {
            this.ConvertHexToBits(this.kca, this.keybin);
        }
        else
        {
            var sk = this.ConvertTextToBits(this.kca, this.keybin);

            var fsk = this.AppendZeroes(this.keybin, sk);
        }

        this.Discard8thBitsFromKey();

        var blockCount = 0;

        for (var i = 0; i < fst; i += 64)
        {
            Console.WriteLine($" ---- Block {++blockCount} started ---- ");

            for (k = 0, j = i; j < i + 64; ++j, ++k) this.ptextbitslice[k] = this.plaintextbin[j];

            this.StartEncryption();

            for (k = 0, j = i; j < i + 64; ++j, ++k) this.ciphertextbin[j] = this.fppt[k];

            Console.WriteLine($" ---- Block {blockCount} ended ---- ");
        }

        ciphertext = this.ConvertBitsToIntSequenceStr(this.ciphertextbin, fst);

        return ciphertext;
    }

    /// <summary>
    /// Метод для обчислення лівого циклічного зсуву.
    /// Викликається кожен раз на 16 раундів дешифрування.
    /// </summary>
    private void CircularRightShift(int[] HKey)
    {
        int i, LastBit = HKey[27];
        for (i = 27; i >= 1; --i) HKey[i] = HKey[i - 1];
        HKey[i] = LastBit;
    }

    /// <summary>
    /// Реферс див. <see cref="SixteenRounds"/> для дешифрування. 
    /// </summary>
    private void ReversedSixteenRounds()
    {
        int n;

        for (var i = 0; i < 16; i++)
        {
            this.SaveTemporaryHPT(this.ctLPT, this.tempLPT);

            this.CompressionPermutation();

            this.ExpansionPermutation(this.ctLPT, this.ctExpandedLPT);

            this.XOROperation(this.compressedkey, this.ctExpandedLPT, this.XoredLPT, 48);

            this.SBoxSubstitution(this.XoredLPT, this.ctSBoxLPT);

            this.PBoxPermutation(this.ctSBoxLPT, this.ctPBoxLPT);

            this.XOROperation(this.ctPBoxLPT, this.ctRPT, this.ctLPT, 32);

            this.Swap(this.tempLPT, this.ctRPT);

            n = this.crst[i];

            this.DivideIntoCKeyAndDKey();

            for (var j = 0; j < n; j++)
            {
                this.CircularRightShift(this.CKey);
                this.CircularRightShift(this.DKey);
            }

            this.AttachCKeyAndDKey();
        }
    }

    /// <summary>
    /// Метод-помічник для старту процесу дешифрування.
    /// </summary>
    private void StartDecryption()
    {
        this.InitialPermutation(this.ctextbitslice, this.ipct);

        this.DivideIntoLPTAndRPT(this.ipct, this.ctLPT, this.ctRPT);

        this.AssignChangedKeyToShiftedKey();

        this.ReversedSixteenRounds();

        this.AttachLPTAndRPT(this.ctLPT, this.ctRPT, this.attachedct);

        this.FinalPermutation(this.attachedct, this.fpct);
    }

    public string Decrypt(string ciphertext, string key, bool useHexKey = false)
    {
        string plaintext = null;

        this.ctca = ciphertext.Split(" ").Select(i => (char) int.Parse(i)).ToArray();
        this.kca = key.ToCharArray();
        int j, k;

        var st = this.ConvertTextToBits(this.ctca, this.ciphertextbin);

        var fst = this.AppendZeroes(this.ciphertextbin, st);

        if (useHexKey)
        {
            this.ConvertHexToBits(this.kca, this.keybin);
        }
        else
        {
            var sk = this.ConvertTextToBits(this.kca, this.keybin);

            var fsk = this.AppendZeroes(this.keybin, sk);
        }

        this.Discard8thBitsFromKey();

        for (var i = 0; i < fst; i += 64)
        {
            for (k = 0, j = i; j < i + 64; ++j, ++k) this.ctextbitslice[k] = this.ciphertextbin[j];

            this.StartDecryption();

            for (k = 0, j = i; j < i + 64; ++j, ++k) this.plaintextbin[j] = this.fpct[k];
        }

        plaintext = this.ConvertBitsToText(this.plaintextbin, fst);

        return plaintext;
    }

    /// <summary>
    /// Метод для перевіки ключа на слабкість. Не використовується беспосередньо у алгоритмі.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="useHexKey"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsWeak(string key, bool useHexKey = false)
    {
        List<string> hex = new();

        if (useHexKey)
        {
            if (key.Length != 16)
                throw new ArgumentException("Key is not 8 bytes long");
            for (var i = 0; i < 16; i += 4)
                hex.Add(key[i].ToString() + key[i + 1] + key[i + 2] + key[i + 3]);
        }
        else
        {
            hex = key.Select(c =>
                ((int) c).ToString("X")).ToList();

            if (hex.Count > 4)
                throw new ArgumentException("Key is not 8 bytes long");

            for (var i = 0; i < 4; i++)
                if (i >= hex.Count)
                    hex.Add("0000");
                else if (hex[i].Length == 1)
                    hex[i] = "000" + hex[i];
                else if (hex[i].Length == 2)
                    hex[i] = "00" + hex[i];
                else if (hex[i].Length == 3)
                    hex[i] = "0" + hex[i];
        }

        Console.WriteLine("Key used (hex):" + hex.Aggregate((a, s) => a + " " + s));

        var x = new List<string> {"11", "00", "FF", "FE", "01", "F1", "E1", "F0", "E0", "1F", "1E"};

        foreach (var first in x)
        foreach (var second in x)
        {
            var comb = first + second;

            if (hex.Count(s => s == comb) >= 2)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Метод для обчислення ентропії після кожного раунду DES.
    /// </summary>
    /// <returns></returns>
    public double CalcEntropy()
    {
        var calcOnes = 0;
        var calcZeros = 0;

        foreach (var i in this.ptLPT)
            if (i == 0) calcZeros++;
            else calcOnes++;

        foreach (var i in this.ptRPT)
            if (i == 0) calcZeros++;
            else calcOnes++;


        var class0 = calcOnes / 64d;
        var class1 = calcZeros / 64d;
        var entropy = -(class0 * Math.Log2(class0) + class1 * Math.Log2(class1));

        return entropy;
    }
}