using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitConversionNS.ExpressionParsing.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConversionNS.ExpressionParsing.Tokenizer.Tests
{
    [TestClass()]
    public class TokenReaderTests
    {
        [TestMethod()]
        public void ReadTest()
        {
            var tokenizer = new TokenReader();
            var tokens = tokenizer.Read("-1+2-3[Pa]^2*33.21+1e-8");
            Assert.AreEqual(tokens.Count,13);
            Assert.AreEqual(tokens[0].TokenType,TokenType.Operation);
            Assert.AreEqual(tokens[1].TokenType,TokenType.Number);
            Assert.AreEqual(tokens[2].TokenType,TokenType.Operation);
            Assert.AreEqual(tokens[3].TokenType,TokenType.Number);
            Assert.AreEqual(tokens[4].TokenType,TokenType.Operation);
            Assert.AreEqual(tokens[5].TokenType,TokenType.Number);
            Assert.AreEqual(tokens[6].TokenType,TokenType.Unit);
            Assert.AreEqual(tokens[7].TokenType,TokenType.Operation);
            Assert.AreEqual(tokens[8].TokenType,TokenType.Number);
            Assert.AreEqual(tokens[9].TokenType,TokenType.Operation);
            Assert.AreEqual(tokens[10].TokenType,TokenType.Number);
            Assert.AreEqual(tokens[11].TokenType,TokenType.Operation);
            Assert.AreEqual(tokens[12].TokenType,TokenType.Number);

            Assert.AreEqual(tokens[0].Value, '_');
            Assert.AreEqual((double)tokens[1].Value, 1.0, 1e-8);
            Assert.AreEqual(tokens[2].Value, '+');
            Assert.AreEqual((double)tokens[3].Value, 2, 1e-8);
            Assert.AreEqual(tokens[4].Value, '-');
            Assert.AreEqual((double)tokens[5].Value, 3, 1e-8);
            Assert.AreEqual(tokens[6].Value, "Pa");
            Assert.AreEqual(tokens[7].Value, '^');
            Assert.AreEqual((double)tokens[8].Value, 2,1e-8);
            Assert.AreEqual(tokens[9].Value, '*');
            Assert.AreEqual((double)tokens[10].Value, 33.21,1e-8);
            Assert.AreEqual(tokens[11].Value, '+');
            Assert.AreEqual((double)tokens[12].Value, 1e-8,1e-16);
        }
    }
}