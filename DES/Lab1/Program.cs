using Lab1;
using System.Text;

namespace Lab1
{

    public class Program
    {
        public static Encryptor Encryptor = new Encryptor();

        static void Main(string[] args)
        {
            Console.WriteLine("Encryption / decryption program \nChoose the action:\n");
            Console.WriteLine("\t 1 - encrypt some text (using basic string key)");
            Console.WriteLine("\t 2 - decrypt some text (using basic string key)");
            Console.WriteLine("\t 3 - encrypt some text (using hex key)");
            Console.WriteLine("\t 4 - decrypt some text (using hex key)");
            Console.WriteLine("\t 5 - show test cases for weak keys");
            Console.WriteLine("\t 0 - close the program\n");

            MainLoop();
        }

        static void MainLoop()
        {
            var cont = true;
            while (cont)
            {
                Console.Write("Input instruction: ");
                var input = Console.ReadKey();
                Console.WriteLine();
                try
                {
                    switch (input.Key)
                    {
                        case ConsoleKey.D1:
                            Encrypt();
                            continue;
                        case ConsoleKey.D2:
                            Decrypt();
                            continue;
                        case ConsoleKey.D3:
                            Encrypt(true);
                            continue;
                        case ConsoleKey.D4:
                            Decrypt(true);
                            continue;
                        case ConsoleKey.D5:
                            TestKeys();
                            continue;
                        case ConsoleKey.D0:
                            cont = false;
                            continue;
                        default:
                            Console.WriteLine("\nUnknown instruction");
                            continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void Encrypt(bool useHexKey = false)
        {

            Console.WriteLine("Write a text to encrypt:");
            string text = Console.ReadLine().Trim();

            Console.WriteLine("Write a key to use:");
            string key = Console.ReadLine().Trim();
            string res;

            res = Encryptor.Encrypt(text, key, useHexKey);

            Console.WriteLine("Result: " + res);
        }

        static void Decrypt(bool useHexKey = false)
        {

            Console.WriteLine("Write a text to decrypt:");
            string text = Console.ReadLine().Trim();

            Console.WriteLine("Write a key to use:");
            string key = Console.ReadLine().Trim();
            string res;

            res = Encryptor.Decrypt(text, key, useHexKey);

            Console.WriteLine("Result: " + res);
        }

        static void TestKeys()
        {
            Console.WriteLine("Is weak:" + Encryptor.IsWeak("0000000000000000", true));
            Console.WriteLine("Is weak:" + Encryptor.IsWeak("0030000300003000", true));
            Console.WriteLine("Is weak:" + Encryptor.IsWeak("0000010100001000", true));
            Console.WriteLine("Is weak:" + Encryptor.IsWeak("FFFFFFFFFFFFFFFF", true));
            Console.WriteLine("Is weak:" + Encryptor.IsWeak("FEE0FEE0FEF1FEF1", true));
            Console.WriteLine("Is weak:" + Encryptor.IsWeak("1F011F010E010E01", true));
            Console.WriteLine("Is weak:" + Encryptor.IsWeak("FbE0FaE0FEa1FEa1", true));
        }

    }
}