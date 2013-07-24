namespace Server.Models.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;
    using Jurassic;
    using Jurassic.Library;
    using Jurassic.Numerics;

    public class CommandResponse
    {
        public string Text { get; set; }
        public string Error { get; set; }
        public string JSAction { get; set; }
    }

    sealed public class APIProcessor
    {
        #region Members

        private ScriptEngine JSEngine;

        private Guid UserKey;

        #endregion

        /// <summary>
        /// Initializes a new instance of the DataProcessor class.
        /// </summary>
        /// <param name="user">The username of the person this processor is attached to</param>
        public APIProcessor(Guid key)
        {
            this.UserKey = key;
            this.JSEngine = Jurassic.Numerics.Configurator.New();
            this.JSEngine.SetGlobalValue("System", new SystemObject(this.JSEngine, key));
        }

        public CommandResponse Process(string command)
        {
            // If we get a bad input, return nothing
            if (string.IsNullOrWhiteSpace(command))
            {
                return new CommandResponse();
            }

            try
            {
                var val = JSEngine.Evaluate(command);
                var retval = new CommandResponse();

                if (val is ArrayInstance || val is ObjectInstance || val is MatrixInstance)
                {
                    object result = null;
                    if (((ObjectInstance)val).TryCallMemberFunction(out result, "toString"))
                    {
                        retval.Text = result.ToString();
                    }
                    else
                    {
                        retval.Text = JSONObject.Stringify(this.JSEngine, val);
                    }
                }
                else
                {
                    retval.Text = val.ToString();
                }

                if (retval.Text.StartsWith("###") && retval.Text.EndsWith("###"))
                {
                    retval.JSAction = retval.Text.Replace("###", string.Empty);
                    retval.Text = string.Empty;
                }

                return retval;
            }
            catch (JavaScriptException e)
            {
                return new CommandResponse()
                {
                    Error = e.Message
                };
            }
            catch (Exception e)
            {
                return new CommandResponse()
                {
                    // just during pre-release
                    Error = e.Message
                };
            }
        }

        public string GetVar(string varname)
        {
            return JSONObject.Stringify(this.JSEngine, this.JSEngine.Evaluate(varname));
        }

        public Dictionary<string, string> GetGlobals()
        {
            var variables = new Dictionary<string, string>();

            // Copy & Sort
            var globalVars = new List<PropertyNameAndValue>(this.JSEngine.Global.Properties);
            globalVars.Sort((a, b) => a.Name.CompareTo(b.Name));

            // Enumerate
            foreach (var property in globalVars)
            {
                switch (property.Value.GetType().ToString())
                {
                    case "Jurassic.Null":
                        variables.Add(property.Name, "null");
                        break;

                    case "Jurassic.Undefined":
                        variables.Add(property.Name, "undefined");
                        break;

                    case "Jurassic.Library.ArrayInstance":
                        variables.Add(property.Name, "array");
                        break;

                    case "System.Int32":
                    case "System.Double":
                        variables.Add(property.Name, "number");
                        break;

                    case "System.String":
                        variables.Add(property.Name, "string");
                        break;

                    case "System.Boolean":
                        variables.Add(property.Name, "boolean");
                        break;

                    default:
                        variables.Add(property.Name, "object");
                        break;
                }
            }

            return variables;
        }
    }
}