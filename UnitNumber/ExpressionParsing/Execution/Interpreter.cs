using System;
using System.Collections.Generic;
using System.Linq;
using UnitConversionNS.Exceptions;
using UnitConversionNS.ExpressionParsing.Operations;
using UnitConversionNS.ExpressionParsing.Util;


namespace UnitConversionNS.ExpressionParsing.Execution
{
    internal class Interpreter : IExecutor
    {
        public Func<IDictionary<string, ExecutionResult>, ExecutionResult> BuildFormula(Operation operation, 
            IFunctionRegistry functionRegistry,UnitsCore core)
        { 
            return variables =>
                {
                    variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
                    return Execute(operation, functionRegistry, variables, core);
                };
        }

        public ExecutionResult Execute(Operation operation, IFunctionRegistry functionRegistry,UnitsCore core)
        {
            return Execute(operation, functionRegistry, new Dictionary<string, ExecutionResult>(),core);
        }

        public ExecutionResult Execute(Operation operation, IFunctionRegistry functionRegistry, 
            IDictionary<string, ExecutionResult> variables,UnitsCore core)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (operation.GetType() == typeof(UnitNumberConstant))
            {
                UnitNumberConstant constant = (UnitNumberConstant)operation;
                return new ExecutionResult(constant.Value);
            }
            else if (operation.GetType() == typeof(FloatingPointConstant))
            {
                FloatingPointConstant constant = (FloatingPointConstant)operation;
                return new ExecutionResult(constant.Value);
            }
            else if (operation.GetType() == typeof(Variable))
            {
                Variable variable = (Variable)operation;

                ExecutionResult value;
                bool variableFound = variables.TryGetValue(variable.Name, out value);

                if (variableFound)
                    return value;
                else
                    throw new VariableNotDefinedException(string.Format("The variable \"{0}\" used is not defined.", variable.Name));
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;
                var executionResult1 = Execute(multiplication.Argument1, functionRegistry, variables,core);
                var executionResult2 = Execute(multiplication.Argument2, functionRegistry, variables,core);
                return ExecuteMultiplication(executionResult1, executionResult2);
                
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                var executionResult1 = Execute(addition.Argument1, functionRegistry, variables,core);
                var executionResult2 = Execute(addition.Argument2, functionRegistry, variables,core);
                return ExecuteAddition(executionResult1, executionResult2); ;
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction)operation;
                var executionResult1 = Execute(addition.Argument1, functionRegistry, variables,core);
                var executionResult2 = Execute(addition.Argument2, functionRegistry, variables,core);
                return ExecuteSubtraction(executionResult1, executionResult2);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                var executionResult1 = Execute(division.Dividend, functionRegistry, variables,core);
                var executionResult2 = Execute(division.Divisor, functionRegistry, variables, core);
                return ExecuteDivision(executionResult1, executionResult2);
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentiation = (Exponentiation)operation;
                var executionResult1 = Execute(exponentiation.Base, functionRegistry, variables, core);
                var executionResult2 = Execute(exponentiation.Exponent, functionRegistry, variables, core);
                return ExecuteExponentiation(executionResult1, executionResult2);
            }
            else if (operation.GetType() == typeof(ChangeUnit))
            {
                ChangeUnit exponentiation = (ChangeUnit)operation;
                var executionResult1 = Execute(exponentiation.Argument1, functionRegistry, variables, core);
                return ExecuteUnitChange(executionResult1, exponentiation.Unit,core);
            }
            else if (operation.GetType() == typeof(UnaryMinus))
            {
                UnaryMinus unaryMinus = (UnaryMinus)operation;
                var executionResult = Execute(unaryMinus.Argument, functionRegistry, variables, core);
                if(executionResult.DataType == DataType.Number)
                    return new ExecutionResult( -(double)executionResult.Value);
                else
                {
                        return new ExecutionResult( -(UnitNumber)executionResult.Value);
                }
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);

                ExecutionResult[] arguments = new ExecutionResult[functionInfo.IsDynamicFunc ? function.Arguments.Count : functionInfo.NumberOfParameters];
                for (int i = 0; i < arguments.Length; i++)
                    arguments[i] = Execute(function.Arguments[i], functionRegistry, variables, core);

                return Invoke(functionInfo.Function, arguments);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }

        private ExecutionResult ExecuteUnitChange(ExecutionResult executionResult1, string unit,UnitsCore core)
        {
            if (executionResult1.DataType == DataType.Number)
            {
                var un = new UnitNumber((double)executionResult1.Value,core.ParseUnit(unit));
                return new ExecutionResult( un);
            }
            else if (executionResult1.DataType == DataType.UnitNumber)
            {
                var un = (UnitNumber)executionResult1.Value;
                var newUnit = core.ParseUnit(unit);
                un.SetUnit(newUnit);
                return new ExecutionResult( un);
            }
            throw new Exception("Bad type");
        }


        private ExecutionResult ExecuteExponentiation(ExecutionResult executionResult1, ExecutionResult executionResult2)
        {
            if (executionResult1.DataType == DataType.Number)
            {
                var num1 = (double)executionResult1.Value;
                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double)executionResult2.Value;
                    return new ExecutionResult(Math.Pow(num1, num2));
                }
                else
                {
                    var num2 = (UnitNumber)executionResult2.Value;
                    if(!num2.Unit.Dimension.IsDimensionless)
                        throw new Exception("Raising to a power with unit is not defined.");
                    return new ExecutionResult( Math.Pow(num1 ,num2.Number));
                }
            }
            else
            {
                var num1 = (UnitNumber)executionResult1.Value;

                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double)executionResult2.Value;
                    return new ExecutionResult(Extensions.Math.Pow(num1,num2));
                }
                else
                {
                    var num2 = (UnitNumber)executionResult2.Value;
                    if (!num2.Unit.Dimension.IsDimensionless)
                        throw new Exception("Raising to a power with unit is not defined.");
                    return new ExecutionResult(Extensions.Math.Pow(num1, num2.Number));
                }
            }
        }

        private ExecutionResult ExecuteDivision(ExecutionResult executionResult1, ExecutionResult executionResult2)
        {
            if (executionResult1.DataType == DataType.Number)
            {
                var num1 = (double)executionResult1.Value;
                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double)executionResult2.Value;
                    return new ExecutionResult(num1 / num2);
                }
                else
                {
                    var num2 = (UnitNumber)executionResult2.Value;
                    return new ExecutionResult(num1 / num2);
                }
            }
            else
            {
                var num1 = (UnitNumber)executionResult1.Value;

                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double)executionResult2.Value;
                    return new ExecutionResult( num1 / num2);
                }
                else
                {
                    var num2 = (UnitNumber)executionResult2.Value;
                    return new ExecutionResult(num1 / num2);
                }
            }
        }

        private ExecutionResult ExecuteSubtraction(ExecutionResult executionResult1, ExecutionResult executionResult2)
        {
            if (executionResult1.DataType == DataType.Number)
            {
                var num1 = (double)executionResult1.Value;
                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double)executionResult2.Value;
                    return new ExecutionResult(num1 - num2);
                }
                else
                {
                    var num2 = (UnitNumber)executionResult2.Value;
                    return new ExecutionResult( num1 - num2);
                }
            }
            else
            {
                var num1 = (UnitNumber)executionResult1.Value;

                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double)executionResult2.Value;
                    return new ExecutionResult(num1 - num2);
                }
                else
                {
                    var num2 = (UnitNumber)executionResult2.Value;
                    return new ExecutionResult(num1 - num2);
                }
            }
        }

        private ExecutionResult ExecuteAddition(ExecutionResult executionResult1, ExecutionResult executionResult2)
        {
            if (executionResult1.DataType == DataType.Number)
            {
                var num1 = (double)executionResult1.Value;
                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double)executionResult2.Value;
                    return new ExecutionResult(num1 + num2);
                }
                else
                {
                    var num2 = (UnitNumber)executionResult2.Value;
                    return new ExecutionResult( num1 + num2);
                }
            }
            else
            {
                var num1 = (UnitNumber)executionResult1.Value;

                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double)executionResult2.Value;
                    return new ExecutionResult(num1 + num2);
                }
                else
                {
                    var num2 = (UnitNumber)executionResult2.Value;
                    return new ExecutionResult(num1 + num2);
                }
            }
        }

        private static ExecutionResult ExecuteMultiplication(ExecutionResult executionResult1, ExecutionResult executionResult2)
        {
            if (executionResult1.DataType == DataType.Number)
            {
                var num1 = (double) executionResult1.Value;
                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double) executionResult2.Value;
                    return new ExecutionResult( num1 * num2);
                }
                else
                {
                    var num2 = (UnitNumber) executionResult2.Value;
                    return new ExecutionResult( num1 * num2);
                }
            }
            else
            {
                var num1 = (UnitNumber) executionResult1.Value;

                if (executionResult2.DataType == DataType.Number)
                {
                    var num2 = (double) executionResult2.Value;
                    return new ExecutionResult(num1 * num2);
                }
                else
                {
                    var num2 = (UnitNumber) executionResult2.Value;
                    return new ExecutionResult(num1 * num2);
                }
            }
        }

        private ExecutionResult Invoke(Delegate function, ExecutionResult[] arguments)
        {
            // DynamicInvoke is slow, so we first try to convert it to a Func
            if (function is Func<ExecutionResult>)
            {
                return ((Func<ExecutionResult>)function).Invoke();
            }
            else if (function is Func<ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult>)function).Invoke(arguments[0]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13], arguments[14]);
            }
            else if (function is Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)
            {
                return ((Func<ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult, ExecutionResult>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13], arguments[14], arguments[15]);
            }
            else if (function is DynamicFunc<ExecutionResult, ExecutionResult>)
            {
                return ((DynamicFunc<ExecutionResult, ExecutionResult>)function).Invoke(arguments);
            }
            else
            {
                return (ExecutionResult)function.DynamicInvoke((from s in arguments select (object)s).ToArray());
            }
        }
    }
}
