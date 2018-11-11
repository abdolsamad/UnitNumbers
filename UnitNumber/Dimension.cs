using System;
using System.Text;

namespace UnitConversionNS
{
    public class Dimension
    {
        public int Mass = 0;
        public int Length = 0;
        public int Time = 0;
        public int Temperature = 0;
        public int Current = 0;
        public int Mole = 0;
        public int Luminosity = 0;

        public bool IsSingle
        {
            get
            {
                bool mass = Mass == 1 && Length == 0 && Time == 0 && Temperature == 0 && Current == 0 && Mole == 0 &&
                            Luminosity == 0;
                bool length = Mass == 0 && Length == 1 && Time == 0 && Temperature == 0 && Current == 0 && Mole == 0 &&
                              Luminosity == 0;
                bool time = Mass == 0 && Length == 0 && Time == 1 && Temperature == 0 && Current == 0 && Mole == 0 &&
                            Luminosity == 0;
                bool temp = Mass == 0 && Length == 0 && Time == 0 && Temperature == 1 && Current == 0 && Mole == 0 &&
                            Luminosity == 0;
                bool curr = Mass == 0 && Length == 0 && Time == 0 && Temperature == 0 && Current == 1 && Mole == 0 &&
                            Luminosity == 0;
                bool mole = Mass == 0 && Length == 0 && Time == 0 && Temperature == 0 && Current == 0 && Mole == 1 &&
                            Luminosity == 0;
                bool lum = Mass == 0 && Length == 0 && Time == 0 && Temperature == 0 && Current == 0 && Mole == 0 &&
                           Luminosity == 1;


                return mass || length || time || temp || curr || mole || lum;
            }
        }

        public bool IsDimensionless => Mass == 0 &&
                                       Length == 0 &&
                                       Time == 0 &&
                                       Temperature == 0 &&
                                       Current == 0 &&
                                       Mole == 0 &&
                                       Luminosity == 0;

        public static bool operator ==(Dimension unit1, Dimension unit2)
        {
            return unit1.Mass == unit2.Mass && unit1.Length == unit2.Length && unit1.Time == unit2.Time &&
                   unit1.Temperature == unit2.Temperature && unit1.Current == unit2.Current &&
                   unit1.Mole == unit2.Mole && unit1.Luminosity == unit2.Luminosity;
        }

        public static bool operator !=(Dimension unit1, Dimension unit2)
        {
            return !(unit1 == unit2);
        }

        public static Dimension operator +(Dimension unit1, Dimension unit2)
        {
            return new Dimension
            {
                Length = unit1.Length + unit2.Length,
                Temperature = unit1.Temperature + unit2.Temperature,
                Current = unit1.Current + unit2.Current,
                Time = unit1.Time + unit2.Time,
                Luminosity = unit1.Luminosity + unit2.Luminosity,
                Mass = unit1.Mass + unit2.Mass,
                Mole = unit1.Mole + unit2.Mole,
            };
        }

        public static Dimension operator -(Dimension unit1, Dimension unit2)
        {
            return new Dimension
            {
                Length = unit1.Length - unit2.Length,
                Temperature = unit1.Temperature - unit2.Temperature,
                Current = unit1.Current - unit2.Current,
                Time = unit1.Time - unit2.Time,
                Luminosity = unit1.Luminosity - unit2.Luminosity,
                Mass = unit1.Mass - unit2.Mass,
                Mole = unit1.Mole - unit2.Mole,
            };
        }

        public Dimension Pow(int p)
        {
            return new Dimension
            {
                Length = Length * p,
                Temperature = Temperature * p,
                Current = Current * p,
                Time = Time * p,
                Luminosity = Luminosity * p,
                Mass = Mass * p,
                Mole = Mole * p
            };
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(PowerString("M", Mass));
            result.Append(PowerString("L", Length));
            result.Append(PowerString("T", Time));
            result.Append(PowerString("Θ", Temperature));
            result.Append(PowerString("I", Current));
            result.Append(PowerString("J", Luminosity));
            result.Append(PowerString("mol", Mole));

            return result.ToString();
        }

        private static string PowerString(string str, int i)
        {
            if (i == 0)
                return String.Empty;
            if (i < 0)
                return $"{str}^({i})";
            if (i == 1)
                return str;
            return $"{str}^{i}";
        }

        public Dimension Clone()
        {
            return new Dimension
            {
                Current = Current,
                Length = Length,
                Luminosity = Luminosity,
                Mass = Mass,
                Mole = Mole,
                Temperature = Temperature,
                Time = Time,
            };
        }
    }
}