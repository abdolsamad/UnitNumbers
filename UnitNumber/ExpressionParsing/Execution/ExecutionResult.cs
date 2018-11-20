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

        public ExecutionResult(double value)
        {
            Value = value;
            DataType = DataType.Number;
        }

        public ExecutionResult(UnitNumber value)
        {
            Value = value;
            DataType = DataType.UnitNumber;
        }

        public ExecutionResult(object value)
        {
            if (value is int || value is double || value is float || value is decimal || value is short)
            {
                Value = Convert.ToDouble(value);
                DataType = DataType.Number;
            }
            else if (value is UnitNumber)
            {
                Value = value;
                DataType = DataType.UnitNumber;
            }
        }

        public ExecutionResult ChangeUnit(Unit unit)
        {
            if (DataType == DataType.Number)
                return new ExecutionResult(new UnitNumber((double) Value, unit));
            else if (DataType == DataType.UnitNumber)
            {
                var unitNumber = (UnitNumber) Value;
                return new ExecutionResult(new UnitNumber(unitNumber.GetValue(unit), unit));
            }

            throw new Exception("Error in unit conversion.");
        }

        public static ExecutionResult operator *(ExecutionResult r1, ExecutionResult r2)
        {
            object value = null;
            switch (r1.DataType)
            {
                case DataType.Number:
                    switch (r2.DataType)
                    {
                        case DataType.Number:
                            value = (double) r1.Value * (double) r2.Value;
                            break;
                        case DataType.UnitNumber:
                            value = (double) r1.Value * (UnitNumber) r2.Value;
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                case DataType.UnitNumber:
                    switch (r2.DataType)
                    {
                        case DataType.Number:
                            value = (UnitNumber) r1.Value * (double) r2.Value;
                            break;
                        case DataType.UnitNumber:
                            value = (UnitNumber) r1.Value * (UnitNumber) r2.Value;
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                default:
                    throw new Exception("Bad execution result type.");
            }

            return new ExecutionResult(value);
        }

        public static ExecutionResult operator /(ExecutionResult r1, ExecutionResult r2)
        {
            object value = null;
            switch (r1.DataType)
            {
                case DataType.Number:
                    switch (r2.DataType)
                    {
                        case DataType.Number:
                            value = (double) r1.Value / (double) r2.Value;
                            break;
                        case DataType.UnitNumber:
                            value = (double) r1.Value / (UnitNumber) r2.Value;
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                case DataType.UnitNumber:
                    switch (r2.DataType)
                    {
                        case DataType.Number:
                            value = (UnitNumber) r1.Value / (double) r2.Value;
                            break;
                        case DataType.UnitNumber:
                            value = (UnitNumber) r1.Value / (UnitNumber) r2.Value;
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                default:
                    throw new Exception("Bad execution result type.");
            }

            return new ExecutionResult(value);
        }

        public static ExecutionResult operator +(ExecutionResult r1, ExecutionResult r2)
        {
            object value = null;
            switch (r1.DataType)
            {
                case DataType.Number:
                    switch (r2.DataType)
                    {
                        case DataType.Number:
                            value = (double) r1.Value + (double) r2.Value;
                            break;
                        case DataType.UnitNumber:
                            value = (double) r1.Value + (UnitNumber) r2.Value;
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                case DataType.UnitNumber:
                    switch (r2.DataType)
                    {
                        case DataType.Number:
                            value = (UnitNumber) r1.Value + (double) r2.Value;
                            break;
                        case DataType.UnitNumber:
                            value = (UnitNumber) r1.Value + (UnitNumber) r2.Value;
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                default:
                    throw new Exception("Bad execution result type.");
            }

            return new ExecutionResult(value);
        }

        public static ExecutionResult operator -(ExecutionResult r1, ExecutionResult r2)
        {
            object value = null;
            switch (r1.DataType)
            {
                case DataType.Number:
                    switch (r2.DataType)
                    {
                        case DataType.Number:
                            value = (double) r1.Value - (double) r2.Value;
                            break;
                        case DataType.UnitNumber:
                            value = (double) r1.Value - (UnitNumber) r2.Value;
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                case DataType.UnitNumber:
                    switch (r2.DataType)
                    {
                        case DataType.Number:
                            value = (UnitNumber) r1.Value - (double) r2.Value;
                            break;
                        case DataType.UnitNumber:
                            value = (UnitNumber) r1.Value - (UnitNumber) r2.Value;
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                default:
                    throw new Exception("Bad execution result type.");
            }

            return new ExecutionResult(value);
        }

        public static ExecutionResult operator -(ExecutionResult r1)
        {
            object value = null;
            switch (r1.DataType)
            {
                case DataType.Number:
                    return new ExecutionResult(-(double) r1.Value);
                    break;
                case DataType.UnitNumber:
                    return new ExecutionResult(-(UnitNumber) r1.Value);
                    break;
                default:
                    throw new Exception("Bad execution result type.");
            }

            return new ExecutionResult(value);
        }

        public ExecutionResult Pow(ExecutionResult p)
        {
            object value = null;
            switch (DataType)
            {
                case DataType.Number:
                    switch (p.DataType)
                    {
                        case DataType.Number:
                            value = Math.Pow((double) Value, (double) p.Value);
                            break;
                        case DataType.UnitNumber:
                            throw new Exception("Can not to a power with unit");
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                case DataType.UnitNumber:
                    switch (p.DataType)
                    {
                        case DataType.Number:
                            value = Extensions.Math.Pow((UnitNumber) Value, (double) p.Value);
                            break;
                        case DataType.UnitNumber:
                            throw new Exception("Can not to a power with unit");
                            break;
                        default:
                            throw new Exception("Bad execution result type.");
                    }

                    break;
                default:
                    throw new Exception("Bad execution result type.");
            }

            return new ExecutionResult(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}