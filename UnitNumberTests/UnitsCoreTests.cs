﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var cm = new Unit("cm",Dimensions.Length, 0.01);
            var ft = new Unit("ft",Dimensions.Length, 0.3048);
            var min = new Unit("min",Dimensions.Time, 60.0);
            var cfm = new Unit("CFM",ft.Pow(3)/min);
            uc.AddBasicUnit(cm);
            var u1 = uc.ParseUnit("cm");
            Assert.IsTrue(u1==cm);
            var u2 = uc.ParseUnit("cm*cm");
            Assert.IsTrue(u2 == cm*cm);
            var u3 = uc.ParseUnit("cm^2");
            Assert.IsTrue(u3 == cm*cm);
            var u4 = uc.ParseUnit("cm^2/cm");
            Assert.IsTrue(u4 == cm);
        }
    }
}