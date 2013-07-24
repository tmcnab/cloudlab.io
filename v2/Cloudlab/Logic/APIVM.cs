namespace Cloudlab.Logic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudlab.Helpers;
    using Cloudlab.Models;
    using DropNet;
    using Jurassic;
    using Jurassic.Library;
    using Jurassic.Numerics;
    using MongoDB.Driver;

    [Serializable]
    public class APIVM
    {
        #region Data Members

        public string User { get; set; }

        ScriptEngine Engine { get; set; }

        public DropNetClient Storage { get; protected set; }

        private TimeSpan Timeout { get; set; }

        #endregion

        public APIVM(string username, TimeSpan maxTime, bool readFiles, bool writeFiles)
        {
            this.User = username;
            this.Timeout = maxTime;
            GetStorage();

            var kvConnStr = System.Configuration.ConfigurationManager.AppSettings.Get("MONGOLAB_URI");

            this.Engine = Jurassic.Numerics.Configurator.New();
            this.Engine = Jurassic.Cloudlab.Configurator.Configure(this.Engine, username, this.Storage, kvConnStr);
        }

        private void GetStorage()
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

        public string Interpret(string javascript)
        {
            if (string.IsNullOrWhiteSpace(javascript))
            {
                return string.Empty;
            }

            try
            {
                var cts = new CancellationTokenSource();
                var task = Task.Factory.StartNew(() => this.Engine.Execute(javascript), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                if (!task.Wait((int)this.Timeout.TotalMilliseconds, cts.Token))
                {
                    cts.Cancel();
                }

                this.Engine.Execute(javascript);
                return JSONObject.Stringify(this.Engine, this.Engine.GetGlobalValue("APIResponse"));
            }
            catch (Exception e)
            {
                e.SendToACP();
                return JSONObject.Stringify(this.Engine, e.Message);
            }
        }
    }
}