
namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class MatrixConstructor : ClrFunction
    {
        public MatrixConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Matrix", new MatrixInstance(engine.Object.InstancePrototype))
        { }

        [JSConstructorFunction]
        public MatrixInstance Construct(params object[] parameters)
        {
            return new MatrixInstance(this.InstancePrototype, parameters);
        }
    }
}
