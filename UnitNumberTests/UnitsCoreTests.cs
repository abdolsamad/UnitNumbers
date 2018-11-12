using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitConversionNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConversionNS.Tests
{
    [TestClass()]
    public class UnitsCoreTests
    {
        [TestMethod()]
        public void ParseUnitTest()
        {
            UnitsCore uc = new UnitsCore();
            var cm = new Unit("cm", Dimensions.Length, 0.01);
            var s = new Unit("s", Dimensions.Time, 1);
            uc.AddBasicUnit(cm);
            uc.AddBasicUnit(s);
            var u1 = uc.ParseUnit("cm");
            Assert.IsTrue(u1 == cm);
            var u2 = uc.ParseUnit("cm*cm");
            Assert.IsTrue(u2 == cm * cm);
            var u3 = uc.ParseUnit("cm^2");
            Assert.IsTrue(u3 == cm * cm);
            var u4 = uc.ParseUnit("cm^2/cm");
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
            uc.AddComplexUnit(kPa);
            var un = uc.ParseNumber("101.325[kPa]");
            Assert.AreEqual(un.Number,101.325,1e-8);
            Assert.IsTrue(un.Unit==kPa);
        }
    }
}