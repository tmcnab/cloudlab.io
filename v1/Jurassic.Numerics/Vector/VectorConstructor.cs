
namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class VectorConstructor : ClrFunction
    {
        public VectorConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Vector", new VectorInstance(engine.Object.InstancePrototype))
        { }

        [JSConstructorFunction]
        public VectorInstance Construct(params object[] parameters)
        {
            return new VectorInstance(this.InstancePrototype, parameters);
        }
    }
}
