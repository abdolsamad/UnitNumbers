using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitConversionNS.ExpressionParsing.Execution;

namespace UnitConversionNS.ExpressionParsing.Util
{
    /// <summary>
    /// Utility methods of Jace.NET that can be used throughout the engine.
    /// </summary>
    internal  static class EngineUtil
    {
        static internal IDictionary<string, ExecutionResult> ConvertVariableNamesToLowerCase(IDictionary<string, ExecutionResult> variables)
        {
            var temp = new Dictionary<string, ExecutionResult>();
            foreach (var keyValuePair in variables)
            {
                temp.Add(keyValuePair.Key.ToLowerInvariant(), keyValuePair.Value);
            }

            return temp;
        }
    }
}
