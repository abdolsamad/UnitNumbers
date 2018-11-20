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
            var oneFoot = new UnitNumber(1, foot);
            var ce = new CalculationEngine(uc);
            var vars = new Dictionary<string, ExecutionResult>();
            vars.Add("A", new ExecutionResult(1));
            vars.Add("B", new ExecutionResult(oneFoot));
            ExecutionResult result;
            //A number
            result = ce.Calculate("1.515");
            Assert.AreEqual(result.DataType,DataType.Number);
            Assert.AreEqual((double)result.Value,1.515,1e-8);
            //A number with unit
            result = ce.Calculate("1.0[ft]");
            Assert.AreEqual(result.DataType, DataType.UnitNumber);
            Assert.IsTrue((UnitNumber)result.Value==oneFoot);
            result = ce.Calculate("-1.0[ft]");
            Assert.AreEqual(result.DataType, DataType.UnitNumber);
            Assert.IsTrue((UnitNumber)result.Value == -oneFoot);
            //Unitless calculations
            result = ce.Calculate("1+0.515");
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, 1.515, 1e-8);
            //Calculations with unit
            result = ce.Calculate("0.5[ft]+6[in]");
            Assert.AreEqual(result.DataType, DataType.UnitNumber);
            Assert.IsTrue((UnitNumber)result.Value == oneFoot);
            //Constant
            result = ce.Calculate("pi");
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, Math.PI, 1e-8);
            result = ce.Calculate("-pi");
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, -Math.PI, 1e-8);
            result = ce.Calculate("1-pi");
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, 1-Math.PI, 1e-8);
            //Variable
            result = ce.Calculate("B",vars);
            Assert.AreEqual(result.DataType, DataType.UnitNumber);
            Assert.IsTrue((UnitNumber)result.Value==oneFoot);
            result = ce.Calculate("-B", vars);
            Assert.AreEqual(result.DataType, DataType.UnitNumber);
            Assert.IsTrue((UnitNumber)result.Value == -oneFoot);
            result = ce.Calculate("1[ft]-B", vars);
            Assert.AreEqual(result.DataType, DataType.UnitNumber);
            Assert.IsTrue((UnitNumber)result.Value == oneFoot-oneFoot);
            //Function with number
            result = ce.Calculate("sin(3.1415926535897932384626433832795)", vars);
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, Math.Sin(Math.PI), 1e-8);
            result = ce.Calculate("sin(3.1415926535897932384626433832795/3)", vars);
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, Math.Sin(Math.PI/3), 1e-8);
            //Function with constant
            result = ce.Calculate("sin(pi)", vars);
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, Math.Sin(Math.PI), 1e-8);
            result = ce.Calculate("sin(pi/3)", vars);
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, Math.Sin(Math.PI / 3), 1e-8);
            //Function with variable
            result = ce.Calculate("sin(A)", vars);
            Assert.AreEqual(result.DataType, DataType.Number);
            Assert.AreEqual((double)result.Value, Math.Sin(1.0), 1e-8);
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