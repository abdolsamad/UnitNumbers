using System;
using System.Collections.Generic;
using UnitConversionNS.ExpressionParsing.Operations;


namespace UnitConversionNS.ExpressionParsing.Execution
{
    public interface IExecutor
    {
        ExecutionResult Execute(Operation operation, IFunctionRegistry functionRegistry);
        ExecutionResult Execute(Operation operation, IFunctionRegistry functionRegistry, IDictionary<string, ExecutionResult> variables);

        Func<IDictionary<string, ExecutionResult>, ExecutionResult> BuildFormula(Operation operation, IFunctionRegistry functionRegistry);
    }
}
