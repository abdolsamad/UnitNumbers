using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitConversionNS.ExpressionParsing.Operations
{
    public class UnaryMinus : Operation
    {
        public UnaryMinus(DataType dataType, Operation argument)
            : base(dataType, argument.DependsOnVariables)
        {
            this.Argument = argument;
        }

        public Operation Argument { get; internal set; }
    }
}
