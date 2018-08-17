using System;
using System.Text;

namespace Pericles.Crypto
{
    public static class CryptoExtensions
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string GetBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}