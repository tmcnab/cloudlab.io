namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics.Distributions;
    using MathNet.Numerics.LinearAlgebra.Double;

    [Serializable]
    public class MatrixConstructor : ClrFunction
    {
        public MatrixConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Matrix", new MatrixInstance(engine.Object.InstancePrototype))
        {
            this.PopulateFunctions();
        }

        [JSConstructorFunction]
        public MatrixInstance Construct(params object[] parameters)
        {
            return new MatrixInstance(this.InstancePrototype, parameters);
        }

        // Documented
        [JSFunction(Name = "Random", Flags = JSFunctionFlags.HasEngineParameter)]
        public static MatrixInstance Random(ScriptEngine engine, int n, int m)
        {
            var matrix = (Matrix) new DenseMatrix (n,m).Random(n, m, new Normal());
            return new MatrixInstance(engine.Object.InstancePrototype , matrix);
        }

        // Documented
        [JSFunction(Name = "add")]
        public static MatrixInstance Add(MatrixInstance A, object B)
        {
            Utilities.Guard(A.Engine, B, new[] { typeof(MatrixInstance), typeof(double), typeof(int) });
            return MatrixInstance.Add(A, B);
        }

        // Documented
        [JSFunction(Name = "div")]
        public static MatrixInstance Divide(MatrixInstance A, object B)
        {
            Utilities.Guard(A.Engine, B, new[] { typeof(MatrixInstance), typeof(double), typeof(int) });
            return MatrixInstance.Divide(A, B);
        }

        // Documented
        [JSFunction(Name = "mul")]
        public static MatrixInstance Multiply(MatrixInstance A, object B, 
                                             [DefaultParameterValue(false)] bool pointwise = false)
        {
            Utilities.Guard(A.Engine, B, new[] { typeof(MatrixInstance), typeof(double), typeof(int) });
            return MatrixInstance.Multiply(A, B, pointwise);
        }

        // Documented
        [JSFunction(Name = "sub")]
        public static MatrixInstance Subtract(MatrixInstance A, object B)
        {
            Utilities.Guard(A.Engine, B, new[] { typeof(MatrixInstance), typeof(double), typeof(int) });
            return MatrixInstance.Sub(A, B);
        }


    }
}