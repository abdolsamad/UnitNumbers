using UnitConversionNS.ExpressionParsing.Execution;
using UnitConversionNS.ExpressionParsing.Operations;

namespace UnitConversionNS.ExpressionParsing
{
    public class Optimizer
    {
        private readonly IExecutor executor;

        public Optimizer(IExecutor executor)
        {
            this.executor = executor;
        }

        public Operation Optimize(Operation operation, IFunctionRegistry functionRegistry,UnitsCore core)
        {
            if (!operation.DependsOnVariables && operation.GetType() != typeof(UnitNumberConstant)
                && operation.GetType() != typeof(FloatingPointConstant))
            {
                var result = executor.Execute(operation, functionRegistry,core);
                if(result.DataType == DataType.Number)
                    return new FloatingPointConstant((double)result.Value);
                else
                {
                    return new UnitNumberConstant((UnitNumber)result.Value);
                }
            }
            else
            {
                if (operation.GetType() == typeof(Addition))
                {
                    Addition addition = (Addition)operation;
                    addition.Argument1 = Optimize(addition.Argument1, functionRegistry, core);
                    addition.Argument2 = Optimize(addition.Argument2, functionRegistry, core);
                }
                else if (operation.GetType() == typeof(Subtraction))
                {
                    Subtraction substraction = (Subtraction)operation;
                    substraction.Argument1 = Optimize(substraction.Argument1, functionRegistry, core);
                    substraction.Argument2 = Optimize(substraction.Argument2, functionRegistry, core);
                }
                else if (operation.GetType() == typeof(Multiplication))
                {
                    Multiplication multiplication = (Multiplication)operation;
                    multiplication.Argument1 = Optimize(multiplication.Argument1, functionRegistry, core);
                    multiplication.Argument2 = Optimize(multiplication.Argument2, functionRegistry, core);
                }
                else if (operation.GetType() == typeof(Division))
                {
                    Division division = (Division)operation;
                    division.Dividend = Optimize(division.Dividend, functionRegistry, core);
                    division.Divisor = Optimize(division.Divisor, functionRegistry, core);
                }
                else if (operation.GetType() == typeof(Exponentiation))
                {
                    Exponentiation division = (Exponentiation)operation;
                    division.Base = Optimize(division.Base, functionRegistry, core);
                    division.Exponent = Optimize(division.Exponent, functionRegistry, core);
                }

                return operation;
            }
        }
    }
}
