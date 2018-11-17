using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitConversionNS.ExpressionParsing.Execution
{
    public class ConstantInfo
    {
        public ConstantInfo(string constantName, ExecutionResult value, bool isOverWritable)
        {
            this.ConstantName = constantName;
            this.Value = value;
            this.IsOverWritable = isOverWritable;
        }

        public string ConstantName { get; private set; }

        public ExecutionResult Value { get; private set; }

        public bool IsOverWritable { get; set; }
    }
}
