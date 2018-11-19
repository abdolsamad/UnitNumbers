using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnitConversionNS.Exceptions;
using UnitConversionNS.ExpressionParsing.Operations;
using UnitConversionNS.ExpressionParsing.Util;

namespace UnitConversionNS.ExpressionParsing.Execution
{
    internal class DynamicCompiler : IExecutor
    {
        private string FuncAssemblyQualifiedName;

        public DynamicCompiler()
        {
            // The lower func reside in mscorelib, the higher ones in another assembly.
            // This is  an easy cross platform way to to have this AssemblyQualifiedName.
            FuncAssemblyQualifiedName =
                typeof(Func<double, double, double, double, double, double, double, double, double, double>).GetType()
                    .Assembly.FullName;
        }

        public ExecutionResult Execute(Operation operation, IFunctionRegistry functionRegistry, UnitsCore core)
        {
            return Execute(operation, functionRegistry, new Dictionary<string, ExecutionResult>(), core);
        }

        public ExecutionResult Execute(Operation operation, IFunctionRegistry functionRegistry,
            IDictionary<string, ExecutionResult> variables, UnitsCore core)
        {
            return BuildFormula(operation, functionRegistry, core)(variables);
        }

        public Func<IDictionary<string, ExecutionResult>, ExecutionResult> BuildFormula(Operation operation,
            IFunctionRegistry functionRegistry, UnitsCore core)
        {
            Func<FormulaContext, ExecutionResult> func = BuildFormulaInternal(operation, functionRegistry);
            return variables =>
            {
                variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
                FormulaContext context = new FormulaContext(variables, functionRegistry);
                return func(context);
            };
        }

        private Func<FormulaContext, ExecutionResult> BuildFormulaInternal(Operation operation,
            IFunctionRegistry functionRegistry)
        {
            ParameterExpression contextParameter = Expression.Parameter(typeof(FormulaContext), "context");

            LabelTarget returnLabel = Expression.Label(typeof(ExecutionResult));

            return Expression.Lambda<Func<FormulaContext, ExecutionResult>>(
                Expression.Block( //block as a body
                    Expression.Return(returnLabel, GenerateMethodBody(operation, contextParameter, functionRegistry)),
                    Expression.Label(returnLabel, Expression.Constant(new ExecutionResult( 0)))
                ),
                contextParameter //parameter
            ).Compile();
        }

        private Expression GenerateMethodBody(Operation operation, ParameterExpression contextParameter,
            IFunctionRegistry functionRegistry)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (operation.GetType() == typeof(UnitNumberConstant))
            {
                var constant = (UnitNumberConstant) operation;
                var ret = new ExecutionResult(constant.Value);
                return Expression.Constant(ret, typeof(ExecutionResult));
            }
            else if (operation.GetType() == typeof(FloatingPointConstant))
            {
                FloatingPointConstant constant = (FloatingPointConstant) operation;
                var ret = new ExecutionResult(constant.Value);
                return Expression.Constant(ret, typeof(ExecutionResult));
            }
            else if (operation.GetType() == typeof(Variable))
            {
                Type contextType = typeof(FormulaContext);
                Type dictionaryType = typeof(IDictionary<string, ExecutionResult>);

                Variable variable = (Variable) operation;

                Expression getVariables = Expression.Property(contextParameter, "Variables");
                ParameterExpression value = Expression.Variable(typeof(ExecutionResult), "value");

                Expression variableFound = Expression.Call(getVariables,
                    dictionaryType.GetMethod("TryGetValue",
                        new Type[] {typeof(string), typeof(ExecutionResult).MakeByRefType()}),
                    Expression.Constant(variable.Name),
                    value);

                Expression throwException = Expression.Throw(
                    Expression.New(typeof(VariableNotDefinedException).GetConstructor(new Type[] {typeof(string)}),
                        Expression.Constant(string.Format("The variable \"{0}\" used is not defined.",
                            variable.Name))));

                LabelTarget returnLabel = Expression.Label(typeof(ExecutionResult));

                return Expression.Block(
                    new[] {value},
                    Expression.IfThenElse(
                        variableFound,
                        Expression.Return(returnLabel, value),
                        throwException
                    ),
                    Expression.Label(returnLabel, Expression.Constant(new ExecutionResult(0)))
                );
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication) operation;
                Expression argument1 = GenerateMethodBody(multiplication.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(multiplication.Argument2, contextParameter, functionRegistry);
                return Expression.Multiply(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition) operation;
                Expression argument1 = GenerateMethodBody(addition.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(addition.Argument2, contextParameter, functionRegistry);

                return Expression.Add(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction) operation;
                Expression argument1 = GenerateMethodBody(addition.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(addition.Argument2, contextParameter, functionRegistry);

                return Expression.Subtract(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division) operation;
                Expression dividend = GenerateMethodBody(division.Dividend, contextParameter, functionRegistry);
                Expression divisor = GenerateMethodBody(division.Divisor, contextParameter, functionRegistry);

                return Expression.Divide(dividend, divisor);
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentation = (Exponentiation) operation;
                Expression @base = GenerateMethodBody(exponentation.Base, contextParameter, functionRegistry);
                Expression exponent = GenerateMethodBody(exponentation.Exponent, contextParameter, functionRegistry);

                return Expression.Call(@base, typeof(ExecutionResult).GetMethod("Pow", new Type[] {typeof(ExecutionResult)}), exponent);
            }
            else if (operation.GetType() == typeof(UnaryMinus))
            {
                UnaryMinus unaryMinus = (UnaryMinus) operation;
                Expression argument = GenerateMethodBody(unaryMinus.Argument, contextParameter, functionRegistry);
                return Expression.Negate(argument);
            }

            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function) operation;

                FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);
                Type funcType;
                Type[] parameterTypes;
                Expression[] arguments;

                if (functionInfo.IsDynamicFunc)
                {
                    funcType = typeof(DynamicFunc<ExecutionResult, ExecutionResult>);
                    parameterTypes = new Type[] {typeof(ExecutionResult[])};


                    Expression[] arrayArguments = new Expression[function.Arguments.Count];
                    for (int i = 0; i < function.Arguments.Count; i++)
                        arrayArguments[i] =
                            GenerateMethodBody(function.Arguments[i], contextParameter, functionRegistry);

                    arguments = new Expression[1];
                    arguments[0] = Expression.NewArrayInit(typeof(ExecutionResult), arrayArguments);
                }
                else
                {
                    funcType = GetFuncType(functionInfo.NumberOfParameters);
                    parameterTypes = (from i in Enumerable.Range(0, functionInfo.NumberOfParameters)
                        select typeof(ExecutionResult)).ToArray();

                    arguments = new Expression[functionInfo.NumberOfParameters];
                    for (int i = 0; i < functionInfo.NumberOfParameters; i++)
                        arguments[i] = GenerateMethodBody(function.Arguments[i], contextParameter, functionRegistry);
                }

                Expression getFunctionRegistry = Expression.Property(contextParameter, "FunctionRegistry");

                ParameterExpression functionInfoVariable = Expression.Variable(typeof(FunctionInfo));

                return Expression.Block(
                    new[] {functionInfoVariable},
                    Expression.Assign(
                        functionInfoVariable,
                        Expression.Call(getFunctionRegistry,
                            typeof(IFunctionRegistry).GetMethod("GetFunctionInfo", new Type[] {typeof(string)}),
                            Expression.Constant(function.FunctionName))
                    ),
                    Expression.Call(
                        Expression.Convert(Expression.Property(functionInfoVariable, "Function"), funcType),
                        funcType.GetMethod("Invoke", parameterTypes),
                        arguments));
            }
            else
            {
                throw new ArgumentException(
                    string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }

        private Type GetFuncType(int numberOfParameters)
        {
            string funcTypeName;
            if (numberOfParameters < 9)
                funcTypeName = string.Format("System.Func`{0}", numberOfParameters + 1);
            else
                funcTypeName = string.Format("System.Func`{0}, {1}", numberOfParameters + 1, FuncAssemblyQualifiedName);
            Type funcType = Type.GetType(funcTypeName);

            Type[] typeArguments = new Type[numberOfParameters + 1];
            for (int i = 0; i < typeArguments.Length; i++)
                typeArguments[i] = typeof(ExecutionResult);

            return funcType.MakeGenericType(typeArguments);
        }
    }
}