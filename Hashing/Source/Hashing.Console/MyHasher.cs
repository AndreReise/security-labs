namespace Hashing.Console
{
    public class MyHasher
    {
        private const ulong C1 = 0x87c37b91114253d5;
        private const ulong C2 = 0x4cf5ad432745937f;

        /// <summary>
        /// Метод для обчислення хєш значення масиву байтів.
        /// Реалізовує MurmurHash3: https://commons.apache.org/proper/commons-codec/apidocs/org/apache/commons/codec/digest/MurmurHash3.html
        /// MurmurHash — це некриптографічна хеш-функція, яка підходить для загального пошуку на основі хешування.
        /// Назва походить від двох основних операцій, множення (MU) і обертання (R), які використовуються у його внутрішньому циклі.
        /// На відміну від криптографічних хеш-функцій, вона не розроблена спеціально для того, щоб її було важко відмінити зловмисником, що робить її непридатною для криптографічних цілей.
        /// </summary>
        /// <param name="data">Масив байт, на основі якого обчислюється хєш значення.</param>
        /// <param name="seed">Додаткове початкове число, яке можна надати, щоб вплинути на кінцеве хєш-значення.
        /// Мета початкового коду полягає в тому, щоб дозволити генерувати різні хєш-значення для тих самих даних, забезпечуючи спосіб варіювати результат хєш-функції.</param>
        /// <param name="bitShift"Розмір хєшу.></param>
        /// <returns></returns>
        public static ulong Hash(byte[] data, ulong seed = 0, int bitShift = 8)
        {
            ulong bitmask = (1UL << bitShift) - 1;
            int length = data.Length;
            int nblocks = length / 8;

            ulong h1 = seed;

            for (int i = 0; i < nblocks; i++)
            {
                ulong k1 = BitConverter.ToUInt64(data, i * 8);
                k1 *= C1;
                k1 = (k1 << 31) | (k1 >> 33);
                k1 *= C2;

                h1 ^= k1;
                h1 = (h1 << 27) | (h1 >> 37);
                h1 = h1 * 5 + 0x52dce729;
            }

            byte[] tail = new byte[8];
            Array.Copy(data, nblocks * 8, tail, 0, length % 8);

            ulong k2 = 0;

            if (length % 8 >= 7)
                k2 ^= (ulong)tail[6] << 48;
            if (length % 8 >= 6)
                k2 ^= (ulong)tail[5] << 40;
            if (length % 8 >= 5)
                k2 ^= (ulong)tail[4] << 32;
            if (length % 8 >= 4)
                k2 ^= (ulong)tail[3] << 24;
            if (length % 8 >= 3)
                k2 ^= (ulong)tail[2] << 16;
            if (length % 8 >= 2)
                k2 ^= (ulong)tail[1] << 8;
            if (length % 8 >= 1)
                k2 ^= tail[0];

            k2 *= C1;
            k2 = (k2 << 31) | (k2 >> 33);
            k2 *= C2;

            h1 ^= k2;

            h1 ^= (ulong)length;
            h1 ^= h1 >> 33;
            h1 *= 0xff51afd7ed558ccd;
            h1 ^= h1 >> 33;
            h1 *= 0xc4ceb9fe1a85ec53;
            h1 ^= h1 >> 33;

            return h1 & bitmask;
        }
    }
}
