
namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics;

    [Serializable]
    public class SpecialObject : ObjectInstance
    {
        public SpecialObject(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        #region Static Members

        // Documented
        [JSFunction]
        public static object beta(object z, object w)
        {
            if (z is ArrayInstance && w is ArrayInstance)
            {
                Utilities.GuardSameLength(z, w);
                var _z = Utilities.ToDoubleArray((ArrayInstance)z);
                var _w = Utilities.ToDoubleArray((ArrayInstance)w);
                var _r = new double[_z.Length];

                for (int i = 0; i < _r.Length; i++)
                {
                    _r[i] = SpecialFunctions.Beta(_z[i], _w[i]);
                }
                return Utilities.ToArrayInstance(((ArrayInstance)z).Engine, _r);
            }
            else if ((z is System.Double || z is System.Int32) && 
                     (w is System.Double || w is System.Int32))
            {
                return SpecialFunctions.Beta(Convert.ToDouble(z), Convert.ToDouble(w));
            }
            else
            {
                return Undefined.Value;
            }
        }

        // Documented
        [JSFunction]
        public static object betaln(object z, object w)
        {
            if (z is ArrayInstance && w is ArrayInstance)
            {
                Utilities.GuardSameLength(z, w);
                var _z = Utilities.ToDoubleArray((ArrayInstance)z);
                var _w = Utilities.ToDoubleArray((ArrayInstance)w);
                var _r = new double[_z.Length];

                for (int i = 0; i < _r.Length; i++)
                {
                    _r[i] = SpecialFunctions.BetaLn(_z[i], _w[i]);
                }
                return Utilities.ToArrayInstance(((ArrayInstance)z).Engine, _r);
            }
            else if ((z is System.Double || z is System.Int32) &&
                     (w is System.Double || w is System.Int32))
            {
                return SpecialFunctions.BetaLn(Convert.ToDouble(z), Convert.ToDouble(w));
            }
            else
            {
                return Undefined.Value;
            }
        }

        // Documented
        [JSFunction]
        public static object betainc(object z, object w, object x, bool f = true)
        {
            if (z is ArrayInstance && w is ArrayInstance && x is ArrayInstance)
            {
                Utilities.GuardSameLength(x, z);
                Utilities.GuardSameLength(z, w);

                var _x = Utilities.ToDoubleArray((ArrayInstance)x);
                var _z = Utilities.ToDoubleArray((ArrayInstance)z);
                var _w = Utilities.ToDoubleArray((ArrayInstance)w);
                var _r = new double[_z.Length];

                if (f)
                {
                    for (int i = 0; i < _r.Length; i++)
                    {
                        _r[i] = SpecialFunctions.BetaIncomplete(_z[i], _w[i], _x[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < _r.Length; i++)
                    {
                        _r[i] = SpecialFunctions.BetaRegularized(_z[i], _w[i], _x[i]);
                    }
                }
                return Utilities.ToArrayInstance(((ArrayInstance)z).Engine, _r);
            }
            else if ((z is System.Double || z is System.Int32) &&
                     (w is System.Double || w is System.Int32) &&
                     (x is System.Double || x is System.Int32))
            {
                if (f)
                {
                    return SpecialFunctions.BetaIncomplete(Convert.ToDouble(z), Convert.ToDouble(w), Convert.ToDouble(x));
                }
                else
                {
                    return SpecialFunctions.BetaRegularized(Convert.ToDouble(z), Convert.ToDouble(w), Convert.ToDouble(x));
                }
            }
            else
            {
                return Undefined.Value;
            }
        }

        // Documented
        [JSFunction]
        public static object erf(object x)
        {
            if (x is ArrayInstance)
            {
                var _x = Utilities.ToDoubleArray((ArrayInstance)x);
                var _r = new double[_x.Length];

                for (int i = 0; i < _r.Length; i++) {
                    _r[i] = SpecialFunctions.Erf(_x[i]);
                }

                return Utilities.ToArrayInstance(((ArrayInstance)x).Engine, _r);
            }
            else if ((x is System.Double || x is System.Int32))
            {
                return SpecialFunctions.Erf(Convert.ToDouble(x));
            }
            else
            {
                return Undefined.Value;
            }
        }

        // Documented
        [JSFunction]
        public static object erfinv(object x)
        {
            if (x is ArrayInstance)
            {
                var _x = Utilities.ToDoubleArray((ArrayInstance)x);
                var _r = new double[_x.Length];

                for (int i = 0; i < _r.Length; i++)
                {
                    _r[i] = SpecialFunctions.ErfInv(_x[i]);
                }

                return Utilities.ToArrayInstance(((ArrayInstance)x).Engine, _r);
            }
            else if ((x is System.Double || x is System.Int32))
            {
                return SpecialFunctions.ErfInv(Convert.ToDouble(x));
            }
            else
            {
                return Undefined.Value;
            }
        }

        // Documented
        [JSFunction]
        public static object erfc(object x)
        {
            if (x is ArrayInstance)
            {
                var _x = Utilities.ToDoubleArray((ArrayInstance)x);
                var _r = new double[_x.Length];

                for (int i = 0; i < _r.Length; i++)
                {
                    _r[i] = SpecialFunctions.Erfc(_x[i]);
                }

                return Utilities.ToArrayInstance(((ArrayInstance)x).Engine, _r);
            }
            else if ((x is System.Double || x is System.Int32))
            {
                return SpecialFunctions.Erfc(Convert.ToDouble(x));
            }
            else
            {
                return Undefined.Value;
            }
        }

        // Documented
        [JSFunction]
        public static object erfcinv(object x)
        {
            if (x is ArrayInstance)
            {
                var _x = Utilities.ToDoubleArray((ArrayInstance)x);
                var _r = new double[_x.Length];

                for (int i = 0; i < _r.Length; i++)
                {
                    _r[i] = SpecialFunctions.ErfcInv(_x[i]);
                }

                return Utilities.ToArrayInstance(((ArrayInstance)x).Engine, _r);
            }
            else if ((x is System.Double || x is System.Int32))
            {
                return SpecialFunctions.ErfcInv(Convert.ToDouble(x));
            }
            else
            {
                return Undefined.Value;
            }
        }

        #endregion
    }
}