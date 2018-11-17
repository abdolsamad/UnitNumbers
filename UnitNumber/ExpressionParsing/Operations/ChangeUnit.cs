namespace UnitConversionNS.ExpressionParsing.Operations
{
    public class ChangeUnit : Operation
    {
        public ChangeUnit(Operation argument1,string unit)
            : base(DataType.UnitNumber, argument1.DependsOnVariables)
        {
            this.Argument1 = argument1;
            this.Unit = unit;
        }

        public Operation Argument1 { get; internal set; }
        public string Unit { get; internal set; }
    }
}
