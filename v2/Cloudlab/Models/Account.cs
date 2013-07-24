using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Cloudlab.Helpers;

namespace Cloudlab.Models
{
    public static class Account
    {
        public static class API
        {
            static MongoDatabase Database = MongoDatabase.Create(System.Configuration.ConfigurationManager.AppSettings.Get("MONGOLAB_URI"));

            const long N_STARTING_CREDITS = 500;

            public static class APIQuota
            {
                public static bool Decrement(string apikey)
                {
                    try
                    {
                        var item = Database.GetCollection<APIRecord>("APIRecord")
                                           .FindAll()
                                           .SingleOrDefault(d => d.APIKey == apikey);

                        var _id = (item == null) ? new ObjectId() : item.Id;
                        Database.GetCollection<APIRecord>("APIRecord").Save(new APIRecord()
                        {
                            Id = _id,
                            APIKey = apikey,
                            Credits = (item == null) ? N_STARTING_CREDITS - 1 : item.Credits - 1
                        });
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ex.SendToACP();
                        return false;
                    }
                }

                public static bool Increment(string apikey, long credits)
                {
                    try
                    {
                        var item = Database.GetCollection<APIRecord>("APIRecord")
                                           .FindAll()
                                           .SingleOrDefault(d => d.APIKey == apikey);

                        var _id = (item == null) ? new ObjectId() : item.Id;
                        Database.GetCollection<APIRecord>("APIRecord").Save(new APIRecord()
                        {
                            Id = _id,
                            APIKey = apikey,
                            Credits = (item == null) ? N_STARTING_CREDITS : item.Credits + credits
                        });
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ex.SendToACP();
                        return false;
                    }
                }

                public static long Get(string apikey)
                {
                    var item = Database.GetCollection<APIRecord>("APIRecord")
                                    .FindAll()
                                    .SingleOrDefault(d => d.APIKey == apikey);
                    
                    if (item == null)
                    {
                        Increment(apikey, 0);
                        return N_STARTING_CREDITS;
                    }
                    else
                    {
                        return item.Credits;
                    }
                }
            }
        }
    }

    public class APIRecord
    {
        public ObjectId Id { get; set; }

        public string APIKey { get; set; }

        public long Credits { get; set; }
    }
}