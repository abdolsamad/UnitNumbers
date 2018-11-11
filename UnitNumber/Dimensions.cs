namespace UnitConversionNS
{
    public static class Dimensions
    {
        public static Dimension Empty => new Dimension();
        public static Dimension Mass => new Dimension(){Mass = 1};
        public static Dimension Length => new Dimension(){Length = 1};
        public static Dimension Area => new Dimension(){Length = 2};
        public static Dimension Volume => new Dimension(){Length = 3};
        public static Dimension Velocity => new Dimension() { Length = 1,Time = -1 };
        public static Dimension Acceleration => new Dimension() { Length = 1,Time = -2 };
        public static Dimension Time => new Dimension() { Time = 1 };
        public static Dimension Temperature => new Dimension() { Temperature = 1 };
        public static Dimension Current => new Dimension() { Current = 1 };
        public static Dimension Mole => new Dimension() { Mole = 1 };
        public static Dimension Luminosity => new Dimension() { Luminosity = 1 };
    }
}