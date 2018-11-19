using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitConversionNS.ExpressionParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitConversionNS.ExpressionParsing.Execution;

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
            var inch = new Unit("in", Dimensions.Length, 0.0254);
            var foot = new Unit("ft", 12 * inch);
            var sec = new Unit("s", Dimensions.Time, 1.0);
            var min = new Unit("min", Dimensions.Time, 60);
            var cfm = new Unit("CFM", foot.Pow(3) / min);
            uc.RegisterUnit(cfm);
            uc.RegisterUnit(inch);
            uc.RegisterUnit(foot);
            uc.RegisterUnit(sec);
            var ce = new CalculationEngine(uc);
            var res = ce.Calculate("((40[cfm]/4)/3[in^2])[ft/s]");
            var vars = new Dictionary<string, ExecutionResult>();
            vars.Add("A", new ExecutionResult(1));
            vars.Add("B", new ExecutionResult(new UnitNumber(1, foot)));
            res = ce.Calculate("A",vars);
            res = ce.Calculate("pi",vars);
            res = ce.Calculate("sin(3.141592653)",vars);
            res = ce.Calculate("sin(pi)",vars);
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