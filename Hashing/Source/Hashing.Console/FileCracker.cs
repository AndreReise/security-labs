using System.ComponentModel.DataAnnotations;

namespace Hashing.Console;

public class FileCracker
{
    /// <summary>
    /// Метод, що вносить зміни в файл таким чином, щоб дайджест нового файлу співпав зі старим.
    /// Метод викликається 3 рази для тестових файлів.
    /// Метод використовує brute-force атаку: намагається змінити 1 байт змісту.
    /// </summary>
    /// <param name="sourcePath">Шлях до файлу.</param>
    /// <param name="resultPath">Шлях до файлу для збереження результату.</param>
    /// <param name="hashSizeBit">Розмір хєш значення.</param>
    /// <param name="maxSteps">Максимальна кількість ітерацій для зміни файлу.</param>
    public static void CrackFile(string sourcePath, string resultPath, int hashSizeBit, int maxSteps)
    {
        var contentBuffer = new byte[1024 * 1024];

        using var fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);

        var contentLength = fs.Read(contentBuffer);
        var content = contentBuffer.AsSpan(0, contentLength).ToArray();

        var originalHash = MyHasher.Hash(content, Demo.Seed, hashSizeBit);

        var fileName = Path.GetFileName(sourcePath);

        System.Console.WriteLine("File hash = {0}", originalHash);

        var step = 0;

        // Проходимось по файлу з кінця
        for (var i = content.Length - 1; i >= 0 && step < maxSteps; i-- )
        {
            var initialByte = content[i];

            // Намагаємося змінити байт на усі 256 можливих значення.
            for (var j = 0; j < 256; j++, step++)
            {
                if (step >= maxSteps)
                {
                    System.Console.WriteLine("The program has reached max step count. Exiting.");
                }

                if (initialByte == (byte) j)
                {
                    continue;
                }

                content[i] = (byte)j;

                var updatedHash = MyHasher.Hash(content, Demo.Seed, hashSizeBit);

                // Колізія! Хєш зміненого файлу співпадає з оригінальним.
                if (updatedHash == originalHash)
                {
                    var resultFilePath = Path.Combine(resultPath, fileName);

                    System.Console.WriteLine("The hash is cracked on step: {0} ", step);
                    System.Console.WriteLine("Writing {0} bytes in {1} file", content.Length, resultFilePath);

                    
                    using var updateFs = File.Open(resultFilePath, FileMode.Create, FileAccess.Write);
                    updateFs.Write(content);

                    return;
                }
            }

            // Відновлюємо початковий вміст
            content[i] = initialByte;
        }
    }

}