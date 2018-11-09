using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitConversionNS.Tests
{
    [TestClass()]
    public class UnitTests
    {
        [TestMethod()]
        public void UnitTest()
        {
            Unit ft = new Unit("ft", new Dimension() { Length = 1 }, 0.3048);
            Unit m = new Unit("m", new Dimension() { Length = 1 }, 1);
            Unit cm = new Unit("cm", new Dimension() { Length = 1 }, 0.01);
            Unit kg = new Unit("kg", new Dimension() { Mass = 1 }, 1);
            Assert.AreEqual(ft.IsBasic, true);
            Assert.AreEqual(m.IsBasic, true);
            Assert.AreEqual(cm.IsBasic, true);
            Assert.AreEqual(kg.IsBasic, true);
        }

        [TestMethod()]
        public void MatchableTest()
        {
            Unit ft = new Unit("ft", new Dimension() {Length = 1}, 0.3048);
            Unit m = new Unit("m", new Dimension() {Length = 1}, 1);
            Unit cm = new Unit("cm", new Dimension() {Length = 1}, 0.01);
            Unit kg = new Unit("kg", new Dimension() {Mass = 1}, 1);
            Assert.IsTrue(ft.Matchable(ft));
            Assert.IsTrue(ft.Matchable(m));
            Assert.IsTrue(ft.Matchable(cm));
            Assert.IsFalse(ft.Matchable(kg));
        }

        [TestMethod()]
        public void PowerTest()
        {
            Unit ft = new Unit("ft", new Dimension() { Length = 1 }, 0.3048);
            Unit c = new Unit("C", new Dimension() { Temperature = 1 }, 1, 273.15);

            var ft2 = ft.Pow(2);
            var c2 = c.Pow(2);
            Assert.AreEqual(ft2.IsBasic, false);
            Assert.AreEqual(ft2.Dimension.Length, 2);
            Assert.AreEqual(ft2.ToSI(1), 0.3048 * 0.3048, 1e-8);
            Assert.AreEqual(c2.Dimension.Temperature, 2);
            Assert.AreEqual(c2.ToSI(100), 100, 1e-8);
        }

        [TestMethod()]
        public void MultiplierTest()
        {
            Unit ft = new Unit("ft", new Dimension() {Length = 1}, 0.3048);
            Unit c = new Unit("C", new Dimension() {Temperature = 1}, 1,273.15);
            Unit f = new Unit("F", new Dimension() {Temperature = 1}, 5.0/9.0,273.15-32.0/1.8);

            var ft2 = ft * ft;
            
            Assert.AreEqual(ft2.IsBasic, false);
            Assert.AreEqual(ft2.Dimension.Length,2);
            Assert.AreEqual(ft2.ToSI(1),0.3048*0.3048,1e-8);
            Assert.AreEqual(f.ToSI(52.0), 284.261111111111, 1e-8);
            Assert.AreEqual(c.ToSI(20), 293.15, 1e-8);
        }

        [TestMethod()]
        public void DividerTest()
        {
            Unit ft = new Unit("ft", new Dimension() { Length = 1 }, 0.3048);
            var ft0 = ft / ft;
            Assert.AreEqual(ft0.IsBasic, false);
            Assert.AreEqual(ft0.Dimension.Length, 0);
            Assert.AreEqual(ft0.ToSI(1), 1, 1e-8);
        }
    }
}