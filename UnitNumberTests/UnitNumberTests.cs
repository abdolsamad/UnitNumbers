using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace UnitConversionNS.Tests
{
    [TestClass()]
    public class UnitNumberTests
    {
        private Unit C => new Unit("C", Dimensions.Temperature, 1, 273.15);
        private Unit F => new Unit("F", Dimensions.Temperature, 5.0 / 9.0, 273.15 - 32.0 / 1.8);
        private Unit cm => new Unit("cm", Dimensions.Length, 0.01);
        private Unit m => new Unit("m", Dimensions.Length, 1.0);

        UnitNumber NumC => new UnitNumber(1.3, C);
        UnitNumber NumF => new UnitNumber(35.33, F);
        UnitNumber NumF2 => new UnitNumber(5.0, F);
        UnitNumber NumCm => new UnitNumber(1.0, cm);
        UnitNumber NumM => new UnitNumber(1.0, m);

        [TestMethod()]
        public void ToStringTest()
        {
            Regex reg = new Regex(@"(?<number>.+?) \[(?<unit>.+?)\]");
            var uncs = reg.Match(NumC.ToString());
            var unfs = reg.Match(NumF.ToString());
            Assert.IsTrue(uncs.Success);
            Assert.IsTrue(unfs.Success);
            Assert.AreEqual(Double.Parse(uncs.Groups["number"].Value), 1.3, 1e-8);
            Assert.AreEqual(Double.Parse(unfs.Groups["number"].Value), 35.33, 1e-8);
            Assert.AreEqual(uncs.Groups["unit"].Value, "C");
            Assert.AreEqual(unfs.Groups["unit"].Value, "F");
        }

        [TestMethod()]
        public void ToStringTest1()
        {
            Assert.AreEqual(NumC.ToString("F2"), "1.30 [C]");
            Assert.AreEqual(NumF.ToString("E4"), "3.5330E+001 [F]");
        }

        [TestMethod()]
        public void CompareToTest()
        {
            Assert.AreEqual(NumF.CompareTo(NumF2), 1);
            Assert.AreEqual(NumF.CompareTo(NumC), 1);
            Assert.AreEqual(NumC.CompareTo(NumF), -1);
            Assert.AreEqual(NumC.CompareTo(22), -1);
            Assert.AreEqual(NumC.CompareTo(0), 1);
        }

        [TestMethod()]
        public void SetUnitTest()
        {
            var c = NumC;
            var f = NumF;
            c.SetUnit(F);
            f.SetUnit(C);
            Assert.AreEqual(c.Number, 34.3400, 1e-8);
            Assert.AreEqual(f.Number, 1.85, 1e-8);
        }

        [TestMethod()]
        public void SetValueTest()
        {
            var c = NumC;
            c.SetValue(55);
            Assert.AreEqual(c.Number, 55, 1e-8);
        }

        [TestMethod()]
        public void SetValueTest1()
        {
            var c = NumC;
            c.SetValue(55, F);
            Assert.AreEqual(c.Number, 55, 1e-8);
            Assert.IsTrue(c.Unit == F);
        }

        [TestMethod()]
        public void GetValueSiTest()
        {
            Assert.AreEqual(NumC.GetValueSi(), 1.3 + 273.15, 1e-8);
            Assert.AreEqual(NumF.GetValueSi(), 275, 1e-8);
        }

        [TestMethod()]
        public void GetValueTest()
        {
            Assert.AreEqual(NumC.GetValue(F), 34.3400, 1e-8);
            Assert.AreEqual(NumF.GetValue(C), 1.85, 1e-8);
        }

        [TestMethod]
        public void SumTest()
        {
            //40.33
            var un1 = NumF + NumF2;
            Assert.AreEqual(un1.Number, 40.33, 1e-3);
            Assert.IsTrue(un1.Unit == F);
            var un2 = NumCm + NumM;
            Assert.AreEqual(un2.Number, 101, 1e-3);
            Assert.IsTrue(un2.Unit == cm);
        }

        [TestMethod]
        public void MultiplicationTest()
        {
            //40.33
            var unF = new UnitNumber(10, (F * F) / F);
            Assert.AreEqual(unF.Number, 10, 1e-3);
            Assert.AreEqual(unF.GetValue(C * C / C), 5.555555555555555555, 1e-8);
            var un2 = NumCm * NumM;
            Assert.AreEqual(un2.GetValue(cm * cm), 100, 1e-3);
            Assert.IsTrue(un2.Unit == cm * m);
        }

        [TestMethod]
        public void CompareUnitTest()
        {
            var f1 = NumF; //35.33
            var f2 = NumF2; //5.0
            Assert.IsTrue(f1 > f2);
            Assert.IsTrue(f1 >= f2);
            Assert.IsTrue(f2 <= f1);
            Assert.IsTrue(f2 < f1);
            Assert.IsTrue(f1 > 30.0);
            Assert.IsTrue(f1 >= 30.0);
            Assert.IsTrue(30.0 <= f1);
            Assert.IsTrue(30.0 < f1);
        }
    }
}