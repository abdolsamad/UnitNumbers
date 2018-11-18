using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitConversionNS.Tests
{
    [TestClass()]
    public class UnitsCoreTests
    {
        [TestMethod()]
        public void ParseUnitTest()
        {
            UnitsCore uc = new UnitsCore();
            var cm = new Unit("cm",Dimensions.Length, 0.01);
            var ft = new Unit("ft",Dimensions.Length, 0.3048);
            var min = new Unit("min",Dimensions.Time, 60.0);
            var s = new Unit("s",Dimensions.Time, 1.0);
            var cfm = new Unit("CFM",ft.Pow(3)/min);

            uc.RegisterUnit(cm);
            uc.RegisterUnit(s);
            var u1 = uc.ParseUnit("cm");
            Assert.IsTrue(u1 == cm);
            var u2 = uc.ParseUnit("cm*cm");
            Assert.IsTrue(u2 == cm * cm);
            var u3 = uc.ParseUnit("cm^2");
            Assert.IsTrue(u3 == cm * cm);
            var u4 = uc.ParseUnit("cm^2.5/cm^1.5");
            Assert.IsTrue(u4 == cm);
            //flow over area -> speed
            var u5 = uc.ParseUnit("cm^3*s^-1/cm^2");
            Assert.IsTrue(u5 == cm / s);
        }

        [TestMethod()]
        public void ParseNumberTest()
        {
            UnitsCore uc = new UnitsCore();
            var kPa = new Unit("kPa", Dimensions.Pressure, 1000);
            uc.RegisterUnit(kPa);
            var un = uc.ParseNumber("101.325[kPa]");
            Assert.AreEqual(un.Number,101.325,1e-8);
            Assert.IsTrue(un.Unit==kPa);
        }
    }
}