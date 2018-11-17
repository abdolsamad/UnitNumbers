using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitConversionNS.ExpressionParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConversionNS.ExpressionParsing.Tests
{
    [TestClass()]
    public class CalculationEngineTests
    {
        [TestMethod()]
        public void CalculationEngineTest()
        {
        }

        [TestMethod()]
        public void CalculationEngineTest1()
        {
        }

        [TestMethod()]
        public void CalculationEngineTest2()
        {
        }

        [TestMethod()]
        public void CalculationEngineTest3()
        {
        }

        [TestMethod()]
        public void CalculationEngineTest4()
        {
        }

        [TestMethod()]
        public void CalculateTest()
        {
            UnitsCore uc = new UnitsCore();
            uc.AddComplexUnit(new Unit("Pa",Dimensions.Pressure,1.0));
            var ce=new CalculationEngine(uc);
            var res = ce.Calculate("-1+2-3[Pa]^2*33.21+1e-8");
        }

        [TestMethod()]
        public void CalculateTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FormulaTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BuildTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest4()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest5()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest6()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest7()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest8()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest9()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest10()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest11()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest12()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest13()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest14()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest15()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest16()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFunctionTest17()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddConstantTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveConstantTest()
        {
            Assert.Fail();
        }
    }
}