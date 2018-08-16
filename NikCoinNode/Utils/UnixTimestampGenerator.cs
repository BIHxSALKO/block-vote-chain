using System;

namespace NikCoin.Utils
{
    public static class UnixTimestampGenerator
    {
        public static double GetUnixTimestamp()
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}