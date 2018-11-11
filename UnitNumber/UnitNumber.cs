using System;

namespace UnitConversionNS
{
    public class UnitNumber : IComparable, IFormattable
    {
        private double _siValue;

        public double Number => Unit.FromSI(_siValue);

        public Unit Unit { get; protected set; }

        public UnitNumber(double value, Unit unit)
        {
            SetValue(value, unit);
        }

        public override string ToString()
        {
            return ToString("g", null);
        }

        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            return $"{Number.ToString(format, formatProvider)} [{Unit}]";
        }

        public int CompareTo(object obj)
        {
            if (obj is double || obj is int || obj is decimal)
            {
                double d = Convert.ToDouble(obj);
                if (this > d) return +1;
                if (Math.Abs(Number - d) < Math.Max(Number , d)/1e8) return 0;
                if (this < d) return -1;
            }else if (obj.GetType() == typeof(UnitNumber))
            {
                var un = (UnitNumber) obj;
                if (this > un) return +1;
                if (this == un) return 0;
                if (this < un) return -1;
            }
            throw new Exception("Can't compare!");
        }

        public void SetUnit(Unit unit)
        {
            ConfirmUnitMatch(Unit, unit);
            Unit = unit.Clone();
        }

        public void SetValue(double value)
        {
            _siValue = Unit.ToSI(value);
        }

        public void SetValue(double value, Unit unit)
        {
            Unit = unit.Clone();
            _siValue = Unit.ToSI(value);
        }

        public double GetValueSi()
        {
            return _siValue;
        }

        public double GetValue(Unit unit)
        {
            ConfirmUnitMatch(Unit, unit);
            return unit.FromSI(Unit.ToSI(Number));
        }

        #region operators

        public static UnitNumber operator +(UnitNumber un1, UnitNumber un2)
        {
            ConfirmUnitMatch(un1.Unit, un2.Unit);
            return new UnitNumber(un1.Number + un2.GetValue(un1.Unit), un1.Unit);
        }

        public static UnitNumber operator -(UnitNumber un1, UnitNumber un2)
        {
            ConfirmUnitMatch(un1.Unit, un2.Unit);
            return new UnitNumber(un1.Number - un2.GetValue(un1.Unit), un1.Unit);
        }

        public static UnitNumber operator *(UnitNumber un1, UnitNumber un2)
        {
            return new UnitNumber(un1.Number * un2.Number, un1.Unit * un2.Unit);
        }

        public static UnitNumber operator /(UnitNumber un1, UnitNumber un2)
        {
            return new UnitNumber(un1.Number / un2.Number, un1.Unit / un2.Unit);
        }

        public static bool operator >(UnitNumber un1, UnitNumber un2)
        {
            ConfirmUnitMatch(un1.Unit, un2.Unit);
            return un1.GetValueSi() > un2.GetValueSi();
        }

        public static bool operator <(UnitNumber un1, UnitNumber un2)
        {
            ConfirmUnitMatch(un1.Unit, un2.Unit);
            return un1.GetValueSi() < un2.GetValueSi();
        }

        public static bool operator >=(UnitNumber un1, UnitNumber un2)
        {
            ConfirmUnitMatch(un1.Unit, un2.Unit);
            return un1.GetValueSi() >= un2.GetValueSi();
        }

        public static bool operator <=(UnitNumber un1, UnitNumber un2)
        {
            ConfirmUnitMatch(un1.Unit, un2.Unit);
            return un1.GetValueSi() <= un2.GetValueSi();
        }

        public static bool operator >(UnitNumber un1, double n2)
        {
            return un1.Number > n2;
        }

        public static bool operator <(UnitNumber un1, double n2)
        {
            return un1.Number < n2;
        }

        public static bool operator >=(UnitNumber un1, double n2)
        {
            return un1.Number >= n2;
        }

        public static bool operator <=(UnitNumber un1, double n2)
        {
            return un1.Number <= n2;
        }

        public static bool operator >(double n1, UnitNumber un2)
        {
            return n1 > un2.Number;
        }

        public static bool operator <(double n1, UnitNumber un2)
        {
            return n1 < un2.Number;
        }

        public static bool operator >=(double n1, UnitNumber un2)
        {
            return n1 >= un2.Number;
        }

        public static bool operator <=(double n1, UnitNumber un2)
        {
            return n1 <= un2.Number;
        }

        #endregion

        private static void ConfirmUnitMatch(Unit un1, Unit un2)
        {
            if (!un1.Matchable(un2))
                throw new Exception($"Units {un1}[{un1.Dimension}] and {un2}[{un2.Dimension}] won't match");
        }
    }
}