using System;

namespace UnitConversionNS
{
    public class Unit
    {
        private string _identifier;
        private Dimension _dimension;
        private double _multiplier;
        private double _sum;
        private bool _isBasic;
        public bool IsBasic => _isBasic;
        public Dimension Dimension => _dimension;

        public Unit(string identifier, Dimension dimension, double multiplier, double sum = 0.0)
        {
            _identifier = identifier;
            _multiplier = multiplier;
            _sum = sum;
            _dimension = dimension;
            _isBasic = dimension.IsDimensionless || dimension.IsSingle;
        }

        private Unit()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u2"></param>
        /// <returns></returns>
        public bool Matchable(Unit u2)
        {
            return _dimension == u2._dimension;
        }

        public static Unit operator *(Unit u1, Unit u2)
        {
            return new Unit
            {
                _identifier = u1._identifier + "*" + u2._identifier,
                _isBasic = false,
                _multiplier = u1._multiplier * u2._multiplier,
                _sum = 0,
                _dimension = u1._dimension + u2._dimension
            };
        }

        public static Unit operator /(Unit u1, Unit u2)
        {
            return new Unit
            {
                _identifier = u1._identifier + "/" + u2._identifier,
                _isBasic = false,
                _multiplier = u1._multiplier / u2._multiplier,
                _sum = 0,
                _dimension = u1._dimension - u2._dimension
            };
        }

        public Unit Pow(int p)
        {
            return new Unit
            {
                _identifier = _identifier + "^" + p,
                _isBasic = false,
                _multiplier = Math.Pow(_multiplier, p),
                _sum = p == 1 ? _sum : 0,
                _dimension = _dimension.Pow(p)
            };
        }

        public double ToSI(double unitValue)
        {
            return _multiplier * unitValue + _sum;
        }

        public double FromSI(double siValue)
        {
            return (siValue - _sum) / _multiplier;
        }

        public Unit Clone()
        {
            return new Unit(_identifier, _dimension.Clone(), _multiplier, _sum) {_isBasic = _isBasic};
        }

        public override string ToString()
        {
            return _identifier;
        }

        public static bool operator ==(Unit u1, Unit u2)
        {
            if (u1 is null) return u2 is null;
            if (u2 is null) return u1 is null;

            return (u1.IsBasic == u2.IsBasic || (Math.Abs(u1._sum) < 1e-8 && Math.Abs(u2._sum) < 1e-8)) &&
                   u1.Matchable(u2) && Math.Abs(u1._multiplier - u2._multiplier) < 1.0e-8 &&
                   Math.Abs(u1._sum - u2._sum) < 1.0e-8;
        }

        public static bool operator !=(Unit u1, Unit u2)
        {
            return !(u1 == u2);
        }
    }
}