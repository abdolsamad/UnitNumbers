using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitConversionNS.ExpressionParsing.Tokenizer;

namespace UnitConversionNS.ExpressionParsing.Tests
{
    [TestClass()]
    public class AstBuilderTests
    {
        [TestMethod()]
        public void BuildTest()
        {
            var tokenizer = new TokenReader();
            var tokens = tokenizer.Read("-1+2-3[Pa]^2*33.21+1e-8");
            var ast = new AstBuilder(null).Build(tokens);
        }
    }
}