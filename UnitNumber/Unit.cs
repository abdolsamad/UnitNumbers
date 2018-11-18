using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UnitConversionNS
{
    public class Unit
    {
        private static int _counter = 0;
        private class IdPowers
        {
            public string Identifier;
            public double Power;
        }

        private List<IdPowers> _idPowers;


        private Dimension _dimension;
        private double _multiplier;
        private double _sum;
        private bool _isBasic;
        public bool IsBasic => _isBasic;
        public bool HasOffset => !Utils.IsZero(_sum);
        public Dimension Dimension => _dimension;

        public Unit(string identifier, Dimension dimension, double multiplier, double sum = 0.0)
        {
            _idPowers = new List<IdPowers>();
            _idPowers.Add(new IdPowers {Identifier = identifier, Power = 1});
            _multiplier = multiplier;
            _sum = sum;
            _dimension = dimension.Clone();
            _isBasic = dimension.IsDimensionless || dimension.IsSingle;
        }
        public Unit(string identifier, Unit unit)
        {
            _idPowers = new List<IdPowers> {new IdPowers() {Identifier = identifier, Power = 1.0}};
            _dimension = unit.Dimension.Clone();
            _multiplier = unit._multiplier;
            _sum = unit._sum;
            _isBasic = unit._isBasic;
        }

        private Unit()
        {
        }

        private Unit(List<IdPowers> idPowers, Dimension dimension, double multiplier, double sum)
        {
            _idPowers = new List<IdPowers>(idPowers);
            _dimension = dimension.Clone();
            _multiplier = multiplier;
            _sum = sum;
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
            List<IdPowers> idp = JoinPowerLists(u1._idPowers, u2._idPowers, +1);

            return new Unit
            {
                _idPowers = idp,
                _isBasic = false,
                _multiplier = u1._multiplier * u2._multiplier,
                _sum = 0,
                _dimension = u1._dimension * u2._dimension
            };
        }

        private static List<IdPowers> JoinPowerLists(List<IdPowers> powerList1, List<IdPowers> powerList2,
            short multiplier)
        {
            List<IdPowers> result = new List<IdPowers>();
            foreach (var idPower in powerList1)
            {
                result.Add(new IdPowers {Identifier = idPower.Identifier,Power = idPower.Power});
            }
            for (int i = 0; i < powerList2.Count; i++)
            {
                var item = powerList2[i];
                var idx = result.FindIndex(x =>
                    x.Identifier.Equals(item.Identifier, StringComparison.OrdinalIgnoreCase));
                if (idx < 0)
                    result.Add(new IdPowers {Power = item.Power*multiplier, Identifier = item.Identifier});
                else
                {
                    result[idx].Power += multiplier * item.Power;
                    if (Utils.IsZero(result[idx].Power))
                        result.RemoveAt(idx);
                }
            }

            return result;
        }

        public static Unit operator /(Unit u1, Unit u2)
        {
            var idp = JoinPowerLists(u1._idPowers, u2._idPowers, -1);

            return new Unit
            {
                _idPowers = idp,
                _isBasic = false,
                _multiplier = u1._multiplier / u2._multiplier,
                _sum = 0,
                _dimension = u1._dimension / u2._dimension
            };
        }

        public Unit Pow(double p)
        {
            return new Unit
            {
                _idPowers = MultiplyPowerLists(_idPowers, p),
                _isBasic = false,
                _multiplier = Math.Pow(_multiplier, p),
                _sum = Utils.DEqual(p, 1) ? _sum : 0,
                _dimension = _dimension.Pow(p)
            };
        }

        private List<IdPowers> MultiplyPowerLists(List<IdPowers> idPowers, double p)
        {
            var result = new List<IdPowers>();

            if (Utils.IsZero(p))
                return result;

            foreach (var idPower in _idPowers)
            {
                result.Add(new IdPowers {Identifier = idPower.Identifier,Power = idPower.Power*p});
            }

            return result;
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
            return new Unit(_idPowers, _dimension, _multiplier, _sum) {_isBasic = _isBasic};
        }

        public override string ToString()
        {
            string[] resList = new string[_idPowers.Count];
            for (var i = 0; i < _idPowers.Count; i++)
            {
                String part = _idPowers[i].Identifier;
                if (!Utils.DEqual(_idPowers[i].Power, 1))
                    part += $"^{_idPowers[i].Power:G4}";
                resList[i] = part;
            }

            return String.Join("*", resList);
        }

        public static bool operator ==(Unit u1, Unit u2)
        {
            if (u1 is null) return u2 is null;
            if (u2 is null) return u1 is null;

            return (u1.IsBasic == u2.IsBasic || (Utils.IsZero(u1._sum) && Utils.IsZero(u2._sum))) &&
                   u1.Matchable(u2) && Utils.DEqual(u1._multiplier, u2._multiplier) &&
                   Utils.DEqual(u1._sum, u2._sum);
        }

        public static bool operator !=(Unit u1, Unit u2)
        {
            return !(u1 == u2);
        }

        public static Unit operator *(double mult,Unit u)
        {
            return new Unit($"unit_{_counter++}",u.Dimension,u._multiplier*mult,u._sum)
            {
                _isBasic = u.IsBasic
            };
        }
    }
}