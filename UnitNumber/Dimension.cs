using System;
using System.Text;

namespace UnitConversionNS
{
    public class Dimension
    {
        public double Mass = 0;
        public double Length = 0;
        public double Time = 0;
        public double Temperature = 0;
        public double Current = 0;
        public double Mole = 0;
        public double Luminosity = 0;

        public bool IsSingle
        {
            get
            {
                bool mass = Utils.DEqual(Mass, 1) && Utils.IsZero(Length) && Utils.IsZero(Time) &&
                            Utils.IsZero(Temperature) && Utils.IsZero(Current) && Utils.IsZero(Mole) &&
                            Utils.IsZero(Luminosity);
                bool length = Utils.IsZero(Mass) && Utils.DEqual(Length, 1) && Utils.IsZero(Time) &&
                              Utils.IsZero(Temperature) && Utils.IsZero(Current) && Utils.IsZero(Mole) &&
                              Utils.IsZero(Luminosity);
                bool time = Utils.IsZero(Mass) && Utils.IsZero(Length) && Utils.DEqual(Time, 1) &&
                            Utils.IsZero(Temperature) && Utils.IsZero(Current) && Utils.IsZero(Mole) &&
                            Utils.IsZero(Luminosity);
                bool temp = Utils.IsZero(Mass) && Utils.IsZero(Length) && Utils.IsZero(Time) &&
                            Utils.DEqual(Temperature, 1) && Utils.IsZero(Current) && Utils.IsZero(Mole) &&
                            Utils.IsZero(Luminosity);
                bool curr = Utils.IsZero(Mass) && Utils.IsZero(Length) && Utils.IsZero(Time) &&
                            Utils.IsZero(Temperature) && Utils.DEqual(Current, 1) && Utils.IsZero(Mole) &&
                            Utils.IsZero(Luminosity);
                bool mole = Utils.IsZero(Mass) && Utils.IsZero(Length) && Utils.IsZero(Time) &&
                            Utils.IsZero(Temperature) && Utils.IsZero(Current) && Utils.DEqual(Mole, 1) &&
                            Utils.IsZero(Luminosity);
                bool lum = Utils.IsZero(Mass) && Utils.IsZero(Length) && Utils.IsZero(Time) &&
                           Utils.IsZero(Temperature) && Utils.IsZero(Current) && Utils.IsZero(Mole) &&
                           Utils.DEqual(Luminosity, 1);


                return mass || length || time || temp || curr || mole || lum;
            }
        }

        public bool IsDimensionless => Utils.IsZero(Mass) &&
                                       Utils.IsZero(Length) &&
                                       Utils.IsZero(Time) &&
                                       Utils.IsZero(Temperature) &&
                                       Utils.IsZero(Current) &&
                                       Utils.IsZero(Mole) &&
                                       Utils.IsZero(Luminosity);

        public static bool operator ==(Dimension unit1, Dimension unit2)
        {
            if (ReferenceEquals(unit1, unit2)) return true;
            return Utils.DEqual(unit1.Mass, unit2.Mass) && Utils.DEqual(unit1.Length , unit2.Length)&& Utils.DEqual(unit1.Time , unit2.Time) && Utils.DEqual(unit1.Temperature , unit2.Temperature) && Utils.DEqual(unit1.Current , unit2.Current)&& Utils.DEqual(unit1.Mole , unit2.Mole) && Utils.DEqual(unit1.Luminosity , unit2.Luminosity);
        }

        public static bool operator !=(Dimension unit1, Dimension unit2)
        {
            return !(unit1 == unit2);
        }

        public static Dimension operator *(Dimension unit1, Dimension unit2)
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

        public static Dimension operator /(Dimension unit1, Dimension unit2)
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

        public Dimension Pow(double p)
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

        private static string PowerString(string str, double pow)
        {
            if (Utils.IsZero(pow))
                return String.Empty;
            if (pow < 0)
                return $"{str}^({pow:G4})";
            if (Utils.DEqual(pow, 1))
                return str;
            return $"{str}^{pow:G4}";
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