using System.Collections.Generic;

namespace UnitConversionNS.ExpressionParsing.Execution
{
    public interface IConstantRegistry : IEnumerable<ConstantInfo>
    {
        ConstantInfo GetConstantInfo(string constantName);
        bool IsConstantName(string constantName);
        void RegisterConstant(string constantName, ExecutionResult value);
        void RegisterConstant(string constantName, ExecutionResult value, bool isOverWritable);
        void RegisterConstant(string constantName, double value, bool isOverWritable);
        void RegisterConstant(string constantName, UnitNumber value, bool isOverWritable);
        void UnregisterConstant(string constantName, bool force = false);
    }
}
