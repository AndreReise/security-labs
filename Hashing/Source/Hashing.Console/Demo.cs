
using System.Collections;

namespace Hashing.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class Demo
    {
        public const int Seed = 1024;

        private static string[] TestFilePaths { get;  } = new string[] {".\\Properties\\document.docx", ".\\Properties\\picture.jpg", ".\\Properties\\source.js"};

        /// <summary>
        /// Метод, що викликає <see cref="FileCracker.CrackFile"/> для 3х тестових файлі: .docx, .js and .png.
        /// Викликається один раз із Main(...).
        /// </summary>
        /// <param name="resultFolderPath"></param>
        public static void TryHackTestFiles(string resultFolderPath)
        {
            const int bitMask = 8;
            const int maxAttempts = 1024 * 1024;

            Console.WriteLine("Try hack test files with {0} bit size hash and {1} max attempts\n\n", bitMask, maxAttempts);

            foreach (var path in TestFilePaths)
            {
                FileCracker.CrackFile(path, resultFolderPath, bitMask, maxAttempts);
            }
        }
        /// <summary>
        /// Метод для тестування хєш-алгоритму. У тестовій строці послідовно змінється і-та літера на випадкову.
        /// Потім обчислюєтьяс новий хєш і підраховується кількість зміненних літер.
        /// Викликається один раз із Main(...).
        /// </summary>
        public static void TestHashAlgorithm()
        {
            var rnd = new Random(Seed);

            const int mask = 32;
            var  maxIterations = mask;
            

            const string message = "It is reasonable to make  p a prime number roughly equal to the number of characters in the input alphabet.";
            var messageBytes = Encoding.ASCII.GetBytes(message);

            // Початковий хєш строки, використовується для порівняння.
            var initialHash = MyHasher.Hash(messageBytes, mask);

            var usedCombinations = new HashSet<KeyValuePair<int, byte>>();

            var iterations = 0;

            Console.WriteLine("Running {0} hash algorithm tests with {1} bit mask", maxIterations, mask);

            while (iterations < maxIterations)
            {
                var changedMessageBytes = messageBytes.ToArray();

                var index = rnd.Next(0, messageBytes.Length);

                // випадкова літера англ. алфавіту
                var _byte = (byte)rnd.Next(64, 128);

                var combination = new KeyValuePair<int, byte>(index, _byte);

                if (usedCombinations.Add(combination) == false)
                {
                    continue;
                }

                // зміна літери на випадкову
                changedMessageBytes[index] = _byte;

                // нове значення хєшу
                var newHash = MyHasher.Hash(changedMessageBytes, mask);

                var similarBits = GetBitsSimilarityCoefficient(initialHash, newHash, mask);

                var coef = (double)similarBits / mask;

                Console.WriteLine("Similar bits: {0}({1})", similarBits, coef);

                iterations++;
            }
        }

        /// <summary>
        /// Метод для порівняння хєш-значень на схожість. Хєш значення транслюються у бінарний вигляд і порівнюються побітно.
        /// </summary>
        /// <param name="a">Хєш А</param>
        /// <param name="b">Хєщ б</param>
        /// <param name="bitSize"></param>
        /// <returns></returns>
        private static int GetBitsSimilarityCoefficient(ulong a, ulong b, int bitSize)
        {
            var a1 = (int)(a & ulong.MaxValue);
            var a2 = (int)(a >> 32);

            var b1 = (int)(b & ulong.MaxValue);
            var b2 = (int)(b >> 32);

            var bits1 = new BitArray(new int[] {a1, a2});
            var bits2 = new BitArray(new int[] { b1, b2 });

            var similarBits = 0;

            for (var i = 0; i < bitSize; i++)
            {
                if (bits1[i] == bits2[i])
                {
                    similarBits++;
                }
            }

            return similarBits;
        }
    }


}
