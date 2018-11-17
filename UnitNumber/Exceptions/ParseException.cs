using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitConversionNS.Exceptions
{
    class ParseException : Exception
    {
        public ParseException(string message) : base(message)
        {
        }
    }
}
