using System;
using System.Collections.Generic;

namespace UnitConversionNS
{
    public class UnitsCore
    {
        private List<Unit> _basicUnits;

        public UnitsCore()
        {
            _basicUnits = new List<Unit>();
        }

        public void AddBasicUnit(Unit unit)
        {
            if(!unit.IsBasic)
                throw new Exception("Unit is not basic!");
            _basicUnits.Add(unit);
        }
    }
}
