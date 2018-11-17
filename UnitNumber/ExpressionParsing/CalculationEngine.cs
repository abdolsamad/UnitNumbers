﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnitConversionNS.ExpressionParsing.Execution;
using UnitConversionNS.ExpressionParsing.Operations;
using UnitConversionNS.ExpressionParsing.Tokenizer;
using UnitConversionNS.ExpressionParsing.Util;

namespace UnitConversionNS.ExpressionParsing
{
    public delegate TResult DynamicFunc<T, TResult>(params T[] values);

    /// <summary>
    /// The CalculationEngine class is the main class of Jace.NET to convert strings containing
    /// mathematical formulas into .NET Delegates and to calculate the result.
    /// It can be configured to run in a number of modes based on the constructor parameters choosen.
    /// </summary>
    public class CalculationEngine
    {
        private readonly IExecutor executor;
        private readonly Optimizer optimizer;
        private readonly CultureInfo cultureInfo;
        private readonly MemoryCache<string, Func<IDictionary<string, ExecutionResult>, ExecutionResult>> executionFormulaCache;
        private readonly bool cacheEnabled;
        private readonly bool optimizerEnabled;
        private UnitsCore core;
        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class with
        /// default parameters.
        /// </summary>
        public CalculationEngine(UnitsCore core)
            : this(core,CultureInfo.CurrentCulture, ExecutionMode.Compiled)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class. The dynamic compiler
        /// is used for formula execution and the optimizer and cache are enabled.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </param>
        public CalculationEngine(UnitsCore core, CultureInfo cultureInfo)
            : this(core,cultureInfo, ExecutionMode.Compiled)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class. The optimizer and 
        /// cache are enabled.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        public CalculationEngine(UnitsCore core, CultureInfo cultureInfo, ExecutionMode executionMode)
            : this(core,cultureInfo, executionMode, true, true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        /// <param name="cacheEnabled">Enable or disable caching of mathematical formulas.</param>
        /// <param name="optimizerEnabled">Enable or disable optimizing of formulas.</param>
        public CalculationEngine(UnitsCore core, CultureInfo cultureInfo, ExecutionMode executionMode, bool cacheEnabled, bool optimizerEnabled)
            : this(core,cultureInfo, executionMode, cacheEnabled, optimizerEnabled, true, true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        /// <param name="cacheEnabled">Enable or disable caching of mathematical formulas.</param>
        /// <param name="optimizerEnabled">Enable or disable optimizing of formulas.</param>
        /// <param name="defaultFunctions">Enable or disable the default functions.</param>
        /// <param name="defaultConstants">Enable or disable the default constants.</param>
        public CalculationEngine(UnitsCore core,CultureInfo cultureInfo, ExecutionMode executionMode, bool cacheEnabled,
            bool optimizerEnabled, bool defaultFunctions, bool defaultConstants)
        {
            this.executionFormulaCache = new MemoryCache<string, Func<IDictionary<string, ExecutionResult>, ExecutionResult>>();
            this.FunctionRegistry = new FunctionRegistry(false);
            this.ConstantRegistry = new ConstantRegistry(false);
            this.cultureInfo = cultureInfo;
            this.cacheEnabled = cacheEnabled;
            this.optimizerEnabled = optimizerEnabled;
            this.core = core;
            if (executionMode == ExecutionMode.Interpreted)
                executor = new Interpreter();
            else if (executionMode == ExecutionMode.Compiled)
                executor = new DynamicCompiler();
            else
                throw new ArgumentException(string.Format("Unsupported execution mode \"{0}\".", executionMode),
                    "executionMode");

            optimizer = new Optimizer(new Interpreter()); // We run the optimizer with the interpreter 

            // Register the default constants of Jace.NET into the constant registry
            if (defaultConstants)
                RegisterDefaultConstants();

            // Register the default functions of Jace.NET into the function registry
            if (defaultFunctions)
                RegisterDefaultFunctions();
        }

        internal IFunctionRegistry FunctionRegistry { get; private set; }

        internal IConstantRegistry ConstantRegistry { get; private set; }

        public IEnumerable<FunctionInfo> Functions { get { return FunctionRegistry; } }

        public IEnumerable<ConstantInfo> Constants { get { return ConstantRegistry; } }

        public ExecutionResult Calculate(string formulaText)
        {
            return Calculate(formulaText, new Dictionary<string, ExecutionResult>());
        }

        public ExecutionResult Calculate(string formulaText, IDictionary<string, ExecutionResult> variables)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (variables == null)
                throw new ArgumentNullException("variables");


            variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
            VerifyVariableNames(variables);

            // Add the reserved variables to the dictionary
            foreach (ConstantInfo constant in ConstantRegistry)
                variables.Add(constant.ConstantName, constant.Value);

            if (IsInFormulaCache(formulaText, out var function))
            {
                return function(variables);
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText);
                function = BuildFormula(formulaText, operation);
                return function(variables);
            }
        }

        public FormulaBuilder Formula(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            return new FormulaBuilder(formulaText, this);
        }

        /// <summary>
        /// Build a .NET func for the provided formula.
        /// </summary>
        /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
        /// <returns>A .NET func for the provided formula.</returns>
        public Func<IDictionary<string, ExecutionResult>, ExecutionResult> Build(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (IsInFormulaCache(formulaText, out var result))
            {
                return result;
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText);
                return BuildFormula(formulaText, operation);
            }
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        public void AddFunction(string functionName, Func<double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        public void AddFunction(string functionName, Func<double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        public void AddFunction(string functionName, Func<double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        public void AddFunction(string functionName, Func<double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        public void AddFunction(string functionName, Func<double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        public void AddFunction(string functionName, Func<double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> function)
        {
            FunctionRegistry.RegisterFunction(functionName, function);
        }

        public void AddFunction(string functionName, DynamicFunc<double, double> functionDelegate)
        {
            FunctionRegistry.RegisterFunction(functionName, functionDelegate);
        }

        /// <summary>
        /// Add a constant to the calculation engine.
        /// </summary>
        /// <param name="constantName">The name of the constant. This name can be used in mathematical formulas.</param>
        /// <param name="value">The value of the constant.</param>
        public void AddConstant(string constantName, ExecutionResult value)
        {
            ConstantRegistry.RegisterConstant(constantName, value);
        }
        /// <summary>
        /// Removes a constant in the calculation engine.
        /// </summary>
        /// <param name="constantName">The name of the constant.</param>
        /// <param name="force">Can remove by force?(remove even nonWritable items)</param>
        public void RemoveConstant(string constantName, bool force = false)
        {
            ConstantRegistry.UnregisterConstant(constantName, force);
        }
        private void RegisterDefaultFunctions()
        {
            FunctionRegistry.RegisterFunction("sin", (Func<double, double>)((a) => Math.Sin(a)), false);
            FunctionRegistry.RegisterFunction("cos", (Func<double, double>)((a) => Math.Cos(a)), false);
            FunctionRegistry.RegisterFunction("csc", (Func<double, double>)((a) => MathUtil.Csc(a)), false);
            FunctionRegistry.RegisterFunction("sec", (Func<double, double>)((a) => MathUtil.Sec(a)), false);
            FunctionRegistry.RegisterFunction("asin", (Func<double, double>)((a) => Math.Asin(a)), false);
            FunctionRegistry.RegisterFunction("acos", (Func<double, double>)((a) => Math.Acos(a)), false);
            FunctionRegistry.RegisterFunction("tan", (Func<double, double>)((a) => Math.Tan(a)), false);
            FunctionRegistry.RegisterFunction("cot", (Func<double, double>)((a) => MathUtil.Cot(a)), false);
            FunctionRegistry.RegisterFunction("atan", (Func<double, double>)((a) => Math.Atan(a)), false);
            FunctionRegistry.RegisterFunction("acot", (Func<double, double>)((a) => MathUtil.Acot(a)), false);
            FunctionRegistry.RegisterFunction("exp", (Func<double, double>)((a) => Math.Exp(a)), false);
            FunctionRegistry.RegisterFunction("log", (Func<double, double>)((a) => Math.Log(a)), false);
            FunctionRegistry.RegisterFunction("loge", (Func<double, double>)((a) => Math.Log(a)), false);
            FunctionRegistry.RegisterFunction("log10", (Func<double, double>)((a) => Math.Log10(a)), false);
            FunctionRegistry.RegisterFunction("logn", (Func<double, double, double>)((a, b) => Math.Log(a, b)), false);
            FunctionRegistry.RegisterFunction("sqrt", (Func<double, double>)((a) => Math.Sqrt(a)), false);
            FunctionRegistry.RegisterFunction("abs", (Func<double, double>)((a) => Math.Abs(a)), false);
            FunctionRegistry.RegisterFunction("max", (Func<double, double, double>)((a, b) => Math.Max(a, b)), false);
            FunctionRegistry.RegisterFunction("min", (Func<double, double, double>)((a, b) => Math.Min(a, b)), false);
            FunctionRegistry.RegisterFunction("ceiling", (Func<double, double>)((a) => Math.Ceiling(a)), false);
            FunctionRegistry.RegisterFunction("floor", (Func<double, double>)((a) => Math.Floor(a)), false);
            FunctionRegistry.RegisterFunction("truncate", (Func<double, double>)((a) => Math.Truncate(a)), false);
        }

        private void RegisterDefaultConstants()
        {
            ConstantRegistry.RegisterConstant("e", Math.E, false);
            ConstantRegistry.RegisterConstant("pi", Math.PI, false);
        }

        /// <summary>
        /// Build the abstract syntax tree for a given formula. The formula string will
        /// be first tokenized.
        /// </summary>
        /// <param name="formulaText">A string containing the mathematical formula that must be converted 
        /// into an abstract syntax tree.</param>
        /// <returns>The abstract syntax tree of the formula.</returns>
        private Operation BuildAbstractSyntaxTree(string formulaText)
        {
            TokenReader tokenReader = new TokenReader(cultureInfo);
            List<Token> tokens = tokenReader.Read(formulaText);

            AstBuilder astBuilder = new AstBuilder(FunctionRegistry);
            Operation operation = astBuilder.Build(tokens);

            if (optimizerEnabled)
                return optimizer.Optimize(operation, this.FunctionRegistry, this.core);
            else
                return operation;
        }

        private Func<IDictionary<string, ExecutionResult>, ExecutionResult> BuildFormula(string formulaText, Operation operation)
        {
            return executionFormulaCache.GetOrAdd(formulaText, v => executor.BuildFormula(operation, this.FunctionRegistry,core));
        }

        private bool IsInFormulaCache(string formulaText, out Func<IDictionary<string, ExecutionResult>, ExecutionResult> function)
        {
            function = null;
            return cacheEnabled && executionFormulaCache.TryGetValue(formulaText, out function);
        }

        /// <summary>
        /// Verify a collection of variables to ensure that all the variable names are valid.
        /// Users are not allowed to overwrite reserved variables or use function names as variables.
        /// If an invalid variable is detected an exception is thrown.
        /// </summary>
        /// <param name="variables">The colletion of variables that must be verified.</param>
        internal void VerifyVariableNames(IDictionary<string, ExecutionResult> variables)
        {
            foreach (string variableName in variables.Keys)
            {
                if (ConstantRegistry.IsConstantName(variableName) && !ConstantRegistry.GetConstantInfo(variableName).IsOverWritable)
                    throw new ArgumentException(string.Format("The name \"{0}\" is a reservered variable name that cannot be overwritten.", variableName), "variables");

                if (FunctionRegistry.IsFunctionName(variableName))
                    throw new ArgumentException(string.Format("The name \"{0}\" is a function name. Parameters cannot have this name.", variableName), "variables");
            }
        }
    }
}
