using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitConversionNS.ExpressionParsing.Execution;

namespace UnitConversionNS.ExpressionParsing
{
    public class FormulaContext
    {
        public FormulaContext(IDictionary<string, ExecutionResult> variables,
            IFunctionRegistry functionRegistry)
        {
            this.Variables = variables;
            this.FunctionRegistry = functionRegistry;
        }

        public IDictionary<string, ExecutionResult> Variables { get; private set; }

        public IFunctionRegistry FunctionRegistry { get; private set; }
    }
}
