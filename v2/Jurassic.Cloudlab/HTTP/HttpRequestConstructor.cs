
namespace Jurassic.Cloudlab
{
    using System;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class HttpRequestConstructor : ClrFunction
    {
        public HttpRequestConstructor(ScriptEngine engine) 
            : base(engine.Function.InstancePrototype, "HttpRequest", new HttpRequestInstance(engine.Object.InstancePrototype))
        { 
        }

        [JSConstructorFunction]
        public HttpRequestInstance Construct()
        {
            return new HttpRequestInstance(this.InstancePrototype);
        }
    }
}
