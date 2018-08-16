using System;
using System.Linq;
using System.Text;

namespace NikCoin.Hashing
{
    public static class HexStringConverter
    {
        public static string BytesToHexString(byte[] bytes)
        {
            var backwardsHexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            return ReverseHexStringEndianness(backwardsHexString);
        }

        public static byte[] HexStringToBytes(string hexString)
        {
            var endianReversedHexString = ReverseHexStringEndianness(hexString);
            return Enumerable.Range(0, endianReversedHexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(endianReversedHexString.Substring(x, 2), 16))
                .ToArray();
        }

        private static string ReverseHexStringEndianness(string forwardString)
        {
            var reverseStringBuilder = new StringBuilder(forwardString);

            var length = forwardString.Length;
            for (var i = 0; i < length / 2; i += 2)
            {
                char tmp = reverseStringBuilder[length - i - 2];
                reverseStringBuilder[length - i - 2] = reverseStringBuilder[i];
                reverseStringBuilder[i] = tmp;

                tmp = reverseStringBuilder[length - i - 1];
                reverseStringBuilder[length - i - 1] = reverseStringBuilder[i + 1];
                reverseStringBuilder[i + 1] = tmp;
            }

            return reverseStringBuilder.ToString();
        }
    }
}