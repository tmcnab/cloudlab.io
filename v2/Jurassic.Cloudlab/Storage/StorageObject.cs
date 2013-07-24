namespace Jurassic.Cloudlab
{
    using System;
    using System.Linq;
    using Jurassic;
    using Jurassic.Library;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    [Serializable]
    public class StorageObject : ObjectInstance
    {
        public StorageObject(ScriptEngine engine, MongoDatabase database, string username) : base(engine)
        {
            this.PopulateFunctions();
            this.Database = database;
        }

        #region Members

        private MongoDatabase Database { get; set; }

        private string Username { get; set; }

        #endregion

        #region User-Visible

        // Documented
        [JSProperty(Name = "length")]
        public int Length
        {
            get
            {
                return this.Database.GetCollection<KVStorageDocument>("StorageObject")
                                    .FindAll()
                                    .Where(d => d.Username == this.Username)
                                    .Count();
            }
        }

        // Documented
        [JSFunction(Name = "key")]
        public object Key(int index)
        {
            var value = this.MDB_KeyForIndex(index);
            if (value == null) return Null.Value;
            else return value;
        }

        // Documented
        [JSFunction(Name = "setItem")]
        public void SetItem(string key, object value)
        {
            this.MDB_CreateOrUpdate(key, JSONObject.Stringify(this.Engine, value));
        }

        // Documented
        [JSFunction(Name = "getItem")]
        public object GetItem(string key)
        {
            return this.MDB_Retrieve(key);
        }

        // Documented
        [JSFunction(Name = "removeItem")]
        public void RemoveItem(string key)
        {
            this.MDB_Delete(key);
        }

        // Documented
        [JSFunction(Name = "clear")]
        public void Clear()
        {
            this.MDB_Clear();
        }

        #endregion

        #region Utility Methods

        private void MDB_CreateOrUpdate(string key, string value)
        {
            var item = this.Database.GetCollection<KVStorageDocument>("StorageObject")
                                    .FindAll()
                                    .SingleOrDefault(d => d.Key == key && d.Username == this.Username);
            var _id = (item == null) ? new ObjectId() : item.Id;
            this.Database.GetCollection<KVStorageDocument>("StorageObject").Save(new KVStorageDocument()
            {
                Id = _id,
                Key = key,
                Value = value,
                Username = this.Username
            });
        }

        private object MDB_Retrieve(string key)
        {
            var item = this.Database.GetCollection<KVStorageDocument>("StorageObject")
                                    .FindAll()
                                    .SingleOrDefault(d => d.Key == key && d.Username == this.Username);
            if (item == null)
            {
                return Undefined.Value;
            }
            else
            {
                return JSONObject.Parse(this.Engine, item.Value);
            }
        }

        private void MDB_Delete(string key)
        {
            var item = this.Database.GetCollection<KVStorageDocument>("StorageObject")
                                    .FindAll()
                                    .SingleOrDefault(d => d.Key == key && d.Username == this.Username);
            this.Database.GetCollection<KVStorageDocument>("StorageObject").Remove(Query.EQ("_id", item.Id));
        }

        private void MDB_Clear()
        {
            this.Database.GetCollection<KVStorageDocument>("StorageObject")
                         .Remove(Query.EQ("Username", this.Username));
        }

        private string MDB_KeyForIndex(int i)
        {
            var item = this.Database.GetCollection<KVStorageDocument>("StorageObject")
                                    .Find(Query.EQ("Username", this.Username))
                                    .SetSortOrder(SortBy.Ascending("Key"))
                                    .SetSkip(i)
                                    .Take(1)
                                    .SingleOrDefault();

            if (item == null)
            {
                return null;
            }
            else
            {
                return item.Key;
            }
        }

        #endregion
    }
}