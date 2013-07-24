
namespace Jurassic.Cloudlab
{
    using System;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class ByteArrayConstructor : ClrFunction
    {
        public ByteArrayConstructor(ScriptEngine engine) 
            : base(engine.Function.InstancePrototype, "ByteArray", new ByteArrayInstance(engine.Object.InstancePrototype))
        { 
        }

        [JSConstructorFunction]
        public ByteArrayInstance Construct(params object[] args)
        {
            return new ByteArrayInstance(this.InstancePrototype, args);
        }
    }
}
