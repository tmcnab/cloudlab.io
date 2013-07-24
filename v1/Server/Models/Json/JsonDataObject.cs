
namespace Server.Models.Json
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System;

    [Serializable]
    public class JsonDataObject
    {
        
        // Converts a DataObject entity to a JsonDataObject for use with the /Data API's
        public static JsonDataObject ToJsonDataObject(DataObject dataObject)
        {
            return new JsonDataObject()
            {
                data = dataObject.Data,
                description = dataObject.Description,
                id = dataObject.Id,
                parentId = dataObject.ParentId ?? 0,
                tags = new List<string>(),
                title = dataObject.Title,
                userProfileId = dataObject.UserProfileId,
                isPublic = dataObject.IsPublic,
                created = dataObject.Created
            };
        }

        public static JsonDataObject ToJsonDataObject(string json)
        {
            var jsonDataObject = JsonConvert.DeserializeObject<JsonDataObject>(json, new JsonSerializerSettings() {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include  
            });
            return jsonDataObject;
        }

        public DateTime created { get; set; }
        
        public string title { get; set; }

        public bool isPublic { get; set; }

        public string description { get; set; }

        public long parentId { get; set; }

        public int userProfileId { get; set; }

        public long id { get; set; }

        public string data { get; set; }

        public List<string> tags { get; set; }
    }
}