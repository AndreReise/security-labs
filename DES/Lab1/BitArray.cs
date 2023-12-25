using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class BitArray
    {
        /// <summary>
        /// DEC TO BIN
        /// </summary>
        public static int[] ToBits(int decimalNumber, int numberOfBits)
        {
            int[] bitArray = new int[numberOfBits];
            char[] binaryDigits = Convert.ToString(decimalNumber, 2).ToCharArray();

            for (int i = binaryDigits.Length - 1, k = numberOfBits - 1; i >= 0 && k >= 0; --i, --k)
            {
                bitArray[k] = (binaryDigits[i] == '1') ? 1 : 0;
            }

            return bitArray;
        }

        /// <summary>
        /// BIN TO DEC
        /// </summary>
        public static int ToDecimal(int[] bitsArray)
        {
            string binaryString = string.Join("", bitsArray);
            int decimalValue = Convert.ToInt32(binaryString, 2);

            return decimalValue;
        }
    }
}
