
namespace Jurassic.Cloudlab
{
    using System;
    using System.Net;
    using Jurassic.Library;
    using System.IO;
    using System.Diagnostics;

    [Serializable]
    public class HttpRequestInstance : ObjectInstance
    {
        #region Constructors

        public HttpRequestInstance(ObjectInstance prototype, params object[] settings) : base(prototype)
        {
            this.PopulateFunctions();
            this.PopulateProperties();
        }

        private void PopulateProperties()
        {
            // Immutable Properties
            this.DefineProperty("UNSENT", new PropertyDescriptor(0, PropertyAttributes.Sealed), false);
            this.DefineProperty("OPENED", new PropertyDescriptor(1, PropertyAttributes.Sealed), false);
            this.DefineProperty("HEADERS_RECEIVED", new PropertyDescriptor(2, PropertyAttributes.Sealed), false);
            this.DefineProperty("LOADING", new PropertyDescriptor(3, PropertyAttributes.Sealed), false);
            this.DefineProperty("DONE", new PropertyDescriptor(4, PropertyAttributes.Sealed), false);

            // Mutable Properties
            this.DefineProperty("timeout", new PropertyDescriptor(0, PropertyAttributes.Writable), false);
            this.DefineProperty("readyState", new PropertyDescriptor(0, PropertyAttributes.Sealed), false);
            
            //this.DefineProperty("responseText", new PropertyDescriptor(string.Empty, PropertyAttributes.), false);
        }

        #endregion

        #region Members

        public HttpWebRequest Request { get; private set; }

        #endregion

        #region Properties

        [JSProperty]
        public string responseText
        {
            get { return this._responseText; }
        }
        string _responseText = string.Empty;

        public FunctionInstance onreadystatechange
        {
            get { return this._onreadystatechange; }
            set { this._onreadystatechange = value; }
        }
        FunctionInstance _onreadystatechange;

        #endregion

        #region User-Visible Methods
        
        [JSFunction(Name="abort")]
        public void Abort()
        {
        }

        [JSFunction(Name = "open")]
        public void Open(string method, string url, [DefaultParameterValue(true)] bool async = true, [DefaultParameterValue(null)] string user = null, [DefaultParameterValue(null)] string password = null)
        {
            // Check that the verb is ok
            if (!this.IsValidHttpVerb(method))
            {
                throw new JavaScriptException(this.Engine, "SyntaxError", "Parameter 'method' does not contain a valid HTTP method.");
            }

            this.Request = (HttpWebRequest)WebRequest.Create(url);
        }

        [JSFunction(Name = "send")]
        public void Send([DefaultParameterValue(null)] ObjectInstance data,
                         [DefaultParameterValue(true)] bool complete)
        {
            var response = this.Request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                this._responseText = reader.ReadToEnd();
            }
        }

        [JSFunction(Name="toString")]
        public override string ToString()
        {
            return "[object HttpRequest]";
        }

        #endregion

        #region Helpers

        internal bool IsValidHttpVerb(string verb)
        {
            switch (verb.ToUpper())
            {
                case "CONNECT":
                case "DELETE":
                case "GET":
                case "HEAD":
                case "OPTIONS":
                case "POST":
                case "PUT":
                case "TRACE":
                case "TRACK":
                    return true;

                default:
                    return false;
            }
        }

        #endregion
    }
}