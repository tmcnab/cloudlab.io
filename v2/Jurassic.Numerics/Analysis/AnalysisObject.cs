

namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics.IntegralTransforms;

    [Serializable]
    public class AnalysisObject : ObjectInstance
    {
        public AnalysisObject(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        /// <summary>
        /// Performs the forward-fft of a real-valued data array
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [JSFunction]
        public static object fft(ArrayInstance x)
        {
            var _r = Utilities.ToComplex((ArrayInstance)x);
            Transform.FourierForward(_r, FourierOptions.Matlab);
            return Utilities.ToArrayInstance(((ArrayInstance)x).Engine, _r);
        }

        /// <summary>
        /// Performs the inverse fourier transform of an array
        /// </summary>
        /// <param name="x">The array to perform the inverse fft on</param>
        /// <returns>Array if all went well, Undefined if it didn't</returns>
        [JSFunction]
        public static object ifft(object x)
        {
            if (x is ArrayInstance)
            {
                var _a = Utilities.ToComplex((ArrayInstance)x);
                Transform.FourierInverse(_a, FourierOptions.Matlab);
                return Utilities.ToArrayInstance(((ArrayInstance)x).Engine, _a);
            }
            else
            {
                return Undefined.Value;
            }
        }
    }
}