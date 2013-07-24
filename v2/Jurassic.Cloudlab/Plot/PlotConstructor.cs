namespace Jurassic.Cloudlab
{
    using System;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class PlotConstructor : ClrFunction
    {
        private Action<string, string> PostMessage;
        private string Username;

        public PlotConstructor(ScriptEngine engine, Action<string, string> postMessage, string username)
            : base(engine.Function.InstancePrototype, "Plot", new HttpRequestInstance(engine.Object.InstancePrototype))
        {
            this.PopulateFunctions();
            this.PostMessage = postMessage;
            this.Username = username;
        }

        [JSConstructorFunction]
        public PlotInstance Construct()
        {
            return new PlotInstance(this.InstancePrototype, this.PostMessage, this.Username);
        }
    }
}