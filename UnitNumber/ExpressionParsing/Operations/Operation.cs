﻿namespace UnitConversionNS.ExpressionParsing.Operations
{
    public abstract class Operation
    {
        public Operation(DataType dataType, bool dependsOnVariables)
        {
            this.DataType = dataType;
            this.DependsOnVariables = dependsOnVariables;
        }

        public DataType DataType { get; private set; }

        public bool DependsOnVariables { get; private set; }
    }
}
