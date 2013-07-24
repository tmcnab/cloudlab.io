
namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics.Distributions;
    using MathNet.Numerics.LinearAlgebra.Double;

    [Serializable]
    public class VectorConstructor : ClrFunction
    {
        public VectorConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Vector", new VectorInstance(engine.Object.InstancePrototype))
        {
            this.PopulateFunctions();
        }

        [JSConstructorFunction]
        public VectorInstance Construct(params object[] parameters)
        {
            return new VectorInstance(this.InstancePrototype, parameters);
        }

        // Documented
        [JSFunction(Name="add")]
        public static VectorInstance Add(VectorInstance A, object B)
        {
            Utilities.Guard(A.Engine, B, new[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(double), typeof(int) });
            return VectorInstance.Add(A, B);
        }

        // Documented
        [JSFunction(Name = "clone")]
        public static VectorInstance Clone(VectorInstance A)
        {
            return A.Clone();
        }

        // Documented
        [JSFunction(Name = "div")]
        public static VectorInstance Divide(VectorInstance A, object B)
        {
            Utilities.Guard(A.Engine, B, new[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(double), typeof(int) });
            return VectorInstance.Divide(A, B);
        }

        // Documented
        [JSFunction(Name = "dot")]
        public double DotProduct(object A, object B)
        {
            if ((A is VectorInstance || A is ArrayInstance) &&
                (B is VectorInstance || B is ArrayInstance))
            {
                return VectorInstance.DotProduct(A, B);
            }
            else
            {
                throw new JavaScriptException(this.Engine, "TypeError", "Input must be Array or Vector", null);
            }
        }

        // Documented
        [JSFunction(Name="mul")]
        public static VectorInstance Multiply(VectorInstance A, object B)
        {
            Utilities.Guard(A.Engine, B, new[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(double), typeof(int) });
            return VectorInstance.Multiply(A, B);
        }

        // Documented
        [JSFunction(Name = "neg")]
        public VectorInstance Negate(VectorInstance A)
        {
            return new VectorInstance(A.Engine.Object.InstancePrototype, A.V.Negate());
        }

        // Documented
        [JSFunction(Name = "sub")]
        public static VectorInstance Subtract(VectorInstance A, object B)
        {
            Utilities.Guard(A.Engine, B, new[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(double), typeof(int) });
            return VectorInstance.Subtract(A, B);
        }

        // Documented
        [JSFunction(Name = "Random", Flags = JSFunctionFlags.HasEngineParameter)]
        public static VectorInstance Random(ScriptEngine engine, int n, 
                                           [DefaultParameterValue("")]  string type = "",
                                           [DefaultParameterValue(double.NaN)] double mean = double.NaN, 
                                           [DefaultParameterValue(double.NaN)] double sttdev = double.NaN)
        {
            Vector v;
            switch (type)
            {
                case "normal":
                default:
                    v = (Vector)new DenseVector(n).Random(n, new Normal(mean == double.NaN ? 0.0 : mean, sttdev == double.NaN ? 1.0 : sttdev));
                    break;
            }
            return new VectorInstance(engine.Object.InstancePrototype, v);
        }
    }
}
