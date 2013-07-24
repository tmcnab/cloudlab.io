
namespace Jurassic.Cloudlab
{
    using System;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class ByteStringConstructor : ClrFunction
    {
        public ByteStringConstructor(ScriptEngine engine) 
            : base(engine.Function.InstancePrototype, "ByteString", new ByteStringInstance(engine.Object.InstancePrototype))
        { 
        }

        [JSConstructorFunction]
        public ByteStringInstance Construct(params object[] args)
        {
            return new ByteStringInstance(this.InstancePrototype, args);
        }

        [JSFunction(Name="join")]
        public static ByteStringInstance Join(object instance, int delimiter)
        {
            throw new NotImplementedException();
        }
    }
}
