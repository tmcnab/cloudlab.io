namespace Jurassic.Cloudlab
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class ConsoleObject : ObjectInstance
    {
        
        private enum Level
        {
            Log,
            Warning,
            Debug,
            Error,
            Info
        }

        public ConsoleObject(ScriptEngine engine, Action<string,string> addMessage, string username) : base(engine)
        {
            this.PopulateFunctions();
            this.Logs = new Queue<KeyValuePair<Level, string>>();
            this.AddMessage = addMessage;
            this.Username = username;
        }

        #region Members

        private Queue<KeyValuePair<Level, string>> Logs;
        private Action<string, string> AddMessage;
        private string Username;

        #endregion

        #region User-Visible Functions

        #region Logging

        [JSFunction(Name = "debug")]
        public void Debug(params object[] args)
        {
            this.AddMessage(this.Username, "Terminal.Console.Debug(JSON.parse(JSON.stringify('" + args[0].ToString() + "')))");
        }

        [JSFunction(Name = "error")]
        public void Error(params object[] args)
        {
            this.AddMessage(this.Username, "Terminal.Console.Error(JSON.parse(JSON.stringify('" + args[0].ToString() + "')))");
        }

        [JSFunction(Name = "info")]
        public void Info(params object[] args)
        {
            this.AddMessage(this.Username, "Terminal.Console.Info(JSON.parse(JSON.stringify('" + args[0].ToString() + "')))");
        }

        [JSFunction(Name="log")]
        public void Log(params object[] args)
        {
            this.AddMessage(this.Username, "Terminal.Console.Log(JSON.parse(JSON.stringify('" + args[0].ToString() + "')))");
        }

        [JSFunction(Name = "warn")]
        public void Warn(params object[] args)
        {
            this.AddMessage(this.Username, "Terminal.Console.Warn(JSON.parse(JSON.stringify('" + args[0].ToString() + "')))");
        }

        #endregion

        [JSFunction(Name = "assert")]
        public void Assert(bool expression, [DefaultParameterValue(null)] string message)
        {
            if (!expression)
            {
                throw new JavaScriptException(this.Engine, "Error", string.IsNullOrWhiteSpace(message) ? string.Empty : message);
            }
        }

        [JSFunction(Name="dir")]
        public void Dir(object obj)
        {
            // TBA - probably in UI somehow
        }

        [JSFunction(Name = "trace")]
        public void Trace()
        {
            // Not Implemented
        }

        [JSFunction(Name = "group")]
        public void Group(params object[] args)
        {
            // Not Implemented
        }

        [JSFunction(Name = "groupEnd")]
        public void GroupEnd()
        {
            // Not Implemented
        }

        [JSFunction(Name = "time")]
        public void Time(string name)
        {
            // Not Implemented
        }

        [JSFunction(Name = "timeEnd")]
        public void TimeEnd(string name)
        {
            // Not Implemented
        }

        [JSFunction(Name = "profile")]
        public void Profile(string title)
        {
            // Not Implemented
        }

        [JSFunction(Name = "profileEnd")]
        public void ProfileEnd()
        {
            // Not Implemented
        }

        [JSFunction(Name = "count")]
        public void Count(string title)
        {
            // Not Implemented
        }

        #endregion

        #region Helper Methods

        private void AddToQueue(Level level, params object[] args)
        {
            /* string substitution characters:
             *  %s      string
             *  %d,%i   Integer
             *  %f      Floats
             *  %o      Object
            */
            // insert here later
            this.Logs.Enqueue(new KeyValuePair<Level, string>(level, args.ToString()));
        }

        #endregion
    }
}