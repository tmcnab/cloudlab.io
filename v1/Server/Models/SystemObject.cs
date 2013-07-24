using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jurassic;
using Jurassic.Library;
using MathNet.Numerics.IntegralTransforms;
using Server.Models;

namespace Server.Models
{
    [Serializable]
    public class SystemObject : ObjectInstance
    {
        #region Members

        private string Username;

        private Guid UserKey;

        #endregion

        #region Constructors

        public SystemObject(ScriptEngine engine, string username) : base(engine)
        {
            this.Username = username;
            this.PopulateFunctions();
        }

        public SystemObject(ScriptEngine engine, Guid key)
            : base(engine)
        {
            this.UserKey = key;
            this.PopulateFunctions();
        }

        #endregion

        #region Import

        [JSFunction]
        public bool importd(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            DataObject dataObject;
            if (Repository.Data.Read(this.Username, id, out dataObject) == RepositoryStatusCode.OK)
            {
                this.Engine.Evaluate(string.Format("var {0} = eval({1})", name, dataObject.Data));
                return true;
            }
            else
            {
                return false;
            }
        }

        [JSFunction]
        public bool importt(int id)
        {
            ToolObject toolObject;
            if (Repository.Tool.Read(this.Username, id, out toolObject) == RepositoryStatusCode.OK)
            {
                try
                {
                    this.Engine.Execute(toolObject.Tool);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Export

        [JSFunction]
        public bool exportd(ObjectInstance obj, string title, string description, bool isPublic = false)
        {
            if (obj == null || string.IsNullOrWhiteSpace(title)) {
                return false;
            }

            return Repository.Data.Create(this.Username, new DataObject() { 
                Data = JSONObject.Stringify(this.Engine, obj),
                Description = description,
                IsPublic = isPublic,
                Title = title,
                Created = DateTime.UtcNow,
            }) == RepositoryStatusCode.OK;
        }

        #endregion

        #region Forking

        public bool forkd(int id)
        {
            return false;
        }

        public bool forkt(int id)
        {
            return false;
        }

        #endregion
    }
}