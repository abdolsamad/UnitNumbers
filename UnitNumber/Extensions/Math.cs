using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace UnitConversionNS.Extensions
{
    public static class Math
    {
        public static UnitNumber Abs(UnitNumber number)
        {
            return new UnitNumber(System.Math.Abs(number.Number),number.Unit);
        }

        public static double Acos(UnitNumber number)
        {
            return System.Math.Acos(number.Number);
        }
        public static double Asin(UnitNumber number)
        {
            return System.Math.Asin(number.Number);
        }
        public static double Atan(UnitNumber number)
        {
            return System.Math.Atan(number.Number);
        }
        public static double Atan2(UnitNumber number1, UnitNumber number2)
        {
            return System.Math.Atan2(number1.GetValueSi(),number2.GetValueSi());
        }

        public static UnitNumber Ceiling(UnitNumber number)
        {
            return new UnitNumber(System.Math.Ceiling(number.Number),number.Unit); 
        }
        public static UnitNumber Floor(UnitNumber number)
        {
            return new UnitNumber(System.Math.Floor(number.Number), number.Unit);
        }
        public static UnitNumber Truncate(UnitNumber number)
        {
            return new UnitNumber(System.Math.Truncate(number.Number), number.Unit);
        }
        public static UnitNumber Round(UnitNumber number)
        {
            return new UnitNumber(System.Math.Round(number.Number), number.Unit);
        }
        public static double Sin(UnitNumber number)
        {
            return System.Math.Sin(number.Number);
        }
        public static double Cos(UnitNumber number)
        {
            return System.Math.Cos(number.Number);
        }
        public static double Tan(UnitNumber number)
        {
            return System.Math.Tan(number.Number);
        }
        public static double Sinh(UnitNumber number)
        {
            return System.Math.Sinh(number.Number);
        }
        public static double Cosh(UnitNumber number)
        {
            return System.Math.Cosh(number.Number);
        }
        public static double Tanh(UnitNumber number)
        {
            return System.Math.Tanh(number.Number);
        }
        public static UnitNumber Pow(UnitNumber number,double pow)
        {
            if(number.Unit.HasOffset)
                throw new Exception("This numbers unit has offset and can not be raised to a power.");
            return new UnitNumber(System.Math.Pow(number.Number,pow), number.Unit.Pow(pow));
        }
    }
}
