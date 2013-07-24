
namespace Jurassic.Numerics
{
    using System;
    using System.Linq;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics;

    [Serializable]
    public class TrigObject : ObjectInstance
    {
        public TrigObject(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        #region Utility Methods

        public static object Execute(object x, Func<double, double> func)
        {
            if (x is ArrayInstance)
            {
                var _x = Utilities.ToDoubleArray((ArrayInstance)x);
                for (int i = 0; i < _x.Length; i++) {
                    _x[i] = func.Invoke(_x[i]);
                }
                return Utilities.ToArrayInstance(((ArrayInstance)x).Engine, _x);
            }
            else if (x is System.Double || x is System.Int32) {
                return func.Invoke(Convert.ToDouble(x));
            }
            else
            {
                return Undefined.Value;
            }
        }

        public static object Execute(object x, object y, Func<double, double, double> func)
        {
            if (x is ArrayInstance && y is ArrayInstance)
            {
                var _x = Utilities.ToDoubleArray((ArrayInstance)x);
                var _y = Utilities.ToDoubleArray((ArrayInstance)y);

                for (int i = 0; i < _x.Length; i++)
                {
                    _x[i] = func.Invoke(_x[i], _y[i]);
                }
                return Utilities.ToArrayInstance(((ArrayInstance)x).Engine, _x);
            }
            else if (x is System.Double || x is System.Int32)
            {
                return func.Invoke(Convert.ToDouble(x), Convert.ToDouble(y));
            }
            else
            {
                return Undefined.Value;
            }
        }

        #endregion

        #region acos- (Inverse Cosine, Complete)

        [JSFunction]
        public static object acos(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseCosine(a));
        }

        [JSFunction]
        public static object acosh(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseHyperbolicCosine(a));
        }

        #endregion

        #region acot- (Inverse Cotangent, Complete)

        [JSFunction]
        public static object acot(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseCotangent(a));
        }

        [JSFunction]
        public static object acoth(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseHyperbolicCotangent(a));
        }

        #endregion

        #region acsc- (Inverse Cosecant, Complete)

        [JSFunction]
        public static object acsc(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseCosecant(a));
        }

        [JSFunction]
        public static object acsch(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseHyperbolicCosecant(a));
        }

        #endregion

        #region asec- (Inverse Secant, Complete)

        [JSFunction]
        public static object asec(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseSecant(a));
        }

        [JSFunction]
        public static object asech(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseHyperbolicSecant(a));
        }

        #endregion

        #region asin- (Inverse Sine, Complete)

        [JSFunction]
        public static object asin(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseSine(a));
        }

        [JSFunction]
        public static object asinh(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseHyperbolicSine(a));
        }

        #endregion

        #region atan- (Inverse Tangent, Complete)

        [JSFunction]
        public static object atan(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseTangent(a));
        }

        [JSFunction]
        public static object atanh(object x)
        {
            return TrigObject.Execute(x, a => Trig.InverseHyperbolicTangent(a));
        }

        #endregion

        #region cos- (Cosine, Complete)

        [JSFunction]
        public static object cos(object x)
        {
            return TrigObject.Execute(x, a => Trig.Cosine(a));
        }

        [JSFunction]
        public static object cosh(object x)
        {
            return TrigObject.Execute(x, a => Trig.HyperbolicCosine(a));
        }

        #endregion

        #region cot- (Cotangent, Complete)

        [JSFunction]
        public static object cot(object x)
        {
            return TrigObject.Execute(x, a => Trig.Cotangent(a));
        }

        [JSFunction]
        public static object coth(object x)
        {
            return TrigObject.Execute(x, a => Trig.HyperbolicCotangent(a));
        }

        #endregion

        #region csc- (Cosecant, Complete)

        [JSFunction]
        public static object csc(object x)
        {
            return TrigObject.Execute(x, a => Trig.Cosecant(a));
        }

        [JSFunction]
        public static object csch(object x)
        {
            return TrigObject.Execute(x, a => Trig.HyperbolicCosecant(a));
        }

        #endregion

        #region sec- (Secant, Complete)

        [JSFunction]
        public static object sec(object x)
        {
            return TrigObject.Execute(x, a => Trig.Secant(a));
        }

        [JSFunction]
        public static object sech(object x)
        {
            return TrigObject.Execute(x, a => Trig.HyperbolicSecant(a));
        }

        #endregion

        #region sin- (Sine, Complete)

        [JSFunction]
        public static object sin(object x)
        {
            return TrigObject.Execute(x, a => Trig.Sine(a));
        }

        [JSFunction]
        public static object sinh(object x)
        {
            return TrigObject.Execute(x, a => Trig.HyperbolicSine(a));
        }

        #endregion

        #region tan- (Tangent, Complete)

        [JSFunction]
        public static object tan(object x)
        {
            return TrigObject.Execute(x, a => Trig.Tangent(a)); 
        }

        [JSFunction]
        public static object tanh(object x)
        {
            return TrigObject.Execute(x, a => Trig.HyperbolicTangent(a));
        }

        #endregion

        #region x2y (Conversion, Complete)

        [JSFunction]
        public static object deg2rad(object x)
        {
            return TrigObject.Execute(x, a => Trig.DegreeToRadian(a));
        }

        [JSFunction]
        public static object deg2grad(object x)
        {
            return TrigObject.Execute(x, a => Trig.DegreeToGrad(a));
        }

        [JSFunction]
        public static object grad2deg(object x)
        {
            return TrigObject.Execute(x, a => Trig.GradToDegree(a));
        }

        [JSFunction]
        public static object grad2rad(object x)
        {
            return TrigObject.Execute(x, a => Trig.GradToRadian(a));
        }

        [JSFunction]
        public static object rad2deg(object x)
        {
            return TrigObject.Execute(x, a => Trig.RadianToDegree(a));
        }

        [JSFunction]
        public static object rad2grad(object x)
        {
            return TrigObject.Execute(x, a => Trig.RadianToGrad(a));
        }

        #endregion

        // Complete
        [JSFunction]
        public static object hypot(object x, object y)
        {
            return TrigObject.Execute(x, y, (a,b) => SpecialFunctions.Hypotenuse(a,b));
        }
    }
}
