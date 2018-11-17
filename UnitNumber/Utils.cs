using System;

namespace UnitConversionNS
{
    internal static class Utils
    {
        public static bool DEqual(double number1, double number2)
        {
            return Math.Abs(number1 - number2) < 1e-8;
        }
        public static bool IsZero(double number1)
        {
            return Utils.DEqual(number1, 0);
        }
    }
}

