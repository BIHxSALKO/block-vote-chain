using System.Numerics;

namespace NikCoin.Mining
{
    public static class TargetFactory
    {
        private const uint CoefficientMask = 0x00FFFFFF;
        private const uint ExponentMask = 0xFF000000;
        private const int ExponentRightShift = 24;

        public static BigInteger Build(uint difficultyBits)
        {
            var coefficient = difficultyBits & CoefficientMask;
            var exponent = (difficultyBits & ExponentMask) >> ExponentRightShift;
            return ComputeTarget(coefficient, exponent);
        }

        private static BigInteger ComputeTarget(uint coefficient, uint exponent)
        {
            var power = (int)(0x8 * (exponent - 0x3));
            var target = coefficient * BigInteger.Pow(2, power);
            return target;
        }
    }
}