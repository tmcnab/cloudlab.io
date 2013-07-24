namespace Cloudlab.Logic
{
    using System;
    using System.Linq;
    using System.Threading;
    using Cloudlab.Helpers;
    using Cloudlab.Models;
    using DropNet;
    using Jurassic;
    using Jurassic.Library;
    using Jurassic.Numerics;

    [Serializable]
    public class JSVM : IDisposable
    {
        public string User { get; set; }

        ScriptEngine Engine { get; set; }

        public DropNetClient Storage { get; protected set; }

        public Thread Executor { get; set; }

        public JSVM(string username)
        {
            this.User = username;
            GetStorage();
            var kvConnStr = System.Configuration.ConfigurationManager.AppSettings.Get("MONGOLAB_URI");

            this.Engine = Jurassic.Numerics.Configurator.New();
            this.Engine = Jurassic.Cloudlab.Configurator.Configure(this.Engine, username, this.Storage, VMManager.PostMessage, kvConnStr);
        }

        public void Dispose()
        {
            try { this.Executor.Abort(); }
            catch { }
        }

        internal void GetStorage()
        {
            try
            {
                this.Storage = Dropbox.GetSession(this.User);
            }
            catch (Exception ex)
            {
                this.Storage = null;
                ex.SendToACP();
            }
        }

        public VMResponse Interpret(string javascript)
        {
            if (string.IsNullOrWhiteSpace(javascript)) {
                return new VMResponse() {
                    Text = string.Empty,
                    IsError = false,
                    IsJS = false
                };
            }

            var response = new VMResponse()
            {
                IsError = false,
                IsJS = false
            };

            try
            {
                this.Executor = Thread.CurrentThread;
                var evalResult = this.Engine.Evaluate(javascript);

                if (evalResult is ArrayInstance || evalResult is MatrixInstance ||
                    evalResult is VectorInstance || evalResult is ObjectInstance)
                {
                    object callResult;
                    if (((ObjectInstance)evalResult).TryCallMemberFunction(out callResult, "toString")) 
                    {
                        response.Text = callResult.ToString();
                    }
                    else 
                    {
                        response.Text = JSONObject.Stringify(this.Engine, evalResult);
                    }
                }
                else 
                {
                    response.Text = evalResult.ToString();
                }

                if (response.Text.StartsWith("###") && response.Text.EndsWith("###"))
                {
                    response.Text = response.Text.Replace("###", string.Empty);
                    response.IsJS = true;
                }
            }
            catch (Exception e)
            {
                e.SendToACP();
                response.Text = e.Message;
                response.IsError = true;
            }

            return response;
        }
    }
}