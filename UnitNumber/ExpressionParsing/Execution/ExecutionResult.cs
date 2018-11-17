using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitConversionNS.ExpressionParsing.Execution
{
    public class ExecutionResult
    {
        public object Value;
        public DataType DataType;
        public ExecutionResult(DataType dataType,object value)
        {
            Value = value;
            DataType = dataType;
        }
    }
}
