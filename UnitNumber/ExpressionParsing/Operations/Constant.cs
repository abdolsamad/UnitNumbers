using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitConversionNS.ExpressionParsing.Operations
{
    public abstract class Constant<T> : Operation
    {
        public Constant(DataType dataType, T value)
            : base(dataType, false)
        {
            this.Value = value;
        }

        public T Value { get; private set; }

        public override bool Equals(object obj)
        {
            Constant<T> other = obj as Constant<T>;
            if (other != null)
                return this.Value.Equals(other.Value);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }

    public class FloatingPointConstant : Constant<double>
    {
        public FloatingPointConstant(double value)
            : base(DataType.Number, value)
        {
        }
    }

    public class UnitNumberConstant : Constant<UnitNumber>
    {
        public UnitNumberConstant(UnitNumber value)
            : base(DataType.UnitNumber, value)
        {
        }
    }
}
