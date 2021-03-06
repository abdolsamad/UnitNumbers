﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace UnitConversionNS.ExpressionParsing.Execution
{
    public class FunctionRegistry : IFunctionRegistry
    {
        private const string DynamicFuncName = "Jace.DynamicFunc";

        private readonly bool caseSensitive;
        private readonly Dictionary<string, FunctionInfo> functions;

        public FunctionRegistry(bool caseSensitive)
        {
            this.caseSensitive = caseSensitive;
            this.functions = new Dictionary<string, FunctionInfo>();
        }

        public IEnumerator<FunctionInfo> GetEnumerator()
        {
            return functions.Select(p => p.Value).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public FunctionInfo GetFunctionInfo(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            FunctionInfo functionInfo = null;
            return functions.TryGetValue(ConvertFunctionName(functionName), out functionInfo) ? functionInfo : null;
        }

        public void RegisterFunction(string functionName, Delegate function)
        {
            RegisterFunction(functionName, function, true);
        }
        
        public void RegisterFunction(string functionName, Delegate function, bool isOverWritable)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            if (function == null)
                throw new ArgumentNullException("function");

            Type funcType = function.GetType();
            bool isDynamicFunc = false;
            int numberOfParameters = -1;
            
            if (funcType.FullName.StartsWith("System.Func"))
            {
                foreach (var genericArgument in funcType.GetMethod("Invoke").GetParameters())
                    if (genericArgument.ParameterType != typeof(ExecutionResult))
                        throw new ArgumentException($"Only {nameof(ExecutionResult)}s are supported as function arguments.", "function");

                numberOfParameters = funcType.GetMethod("Invoke").GetParameters().Length;
            }
            else if (funcType.FullName.StartsWith(DynamicFuncName))
            {
                isDynamicFunc = true;
            }
            else
                throw new ArgumentException("Only System.Func and " + DynamicFuncName + " delegates are permitted.", "function");

            functionName = ConvertFunctionName(functionName);

            if (functions.ContainsKey(functionName) && !functions[functionName].IsOverWritable)
            {
                string message = string.Format("The function \"{0}\" cannot be overwriten.", functionName);
                throw new Exception(message);
            }

            if (functions.ContainsKey(functionName) && functions[functionName].NumberOfParameters != numberOfParameters)
            {
                string message = string.Format("The number of parameters cannot be changed when overwriting a method.");
                throw new Exception(message);
            }

            if (functions.ContainsKey(functionName) && functions[functionName].IsDynamicFunc != isDynamicFunc)
            {
                string message = string.Format("A Func can only be overwritten by another Func and a DynamicFunc can only be overwritten by another DynamicFunc.");
                throw new Exception(message);
            }

            FunctionInfo functionInfo = new FunctionInfo(functionName, numberOfParameters, isOverWritable, isDynamicFunc, function);

            if (functions.ContainsKey(functionName))
                functions[functionName] = functionInfo;
            else
                functions.Add(functionName, functionInfo);
        }

            public bool IsFunctionName(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            return functions.ContainsKey(ConvertFunctionName(functionName));
        }

        private string ConvertFunctionName(string functionName)
        {
            return caseSensitive ? functionName : functionName.ToLowerInvariant();
        }
    }
}
