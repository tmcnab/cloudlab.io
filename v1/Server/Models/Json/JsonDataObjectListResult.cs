using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models.Json
{
    public class JsonDataObjectListResult
    {
        public JsonDataObjectListResult(DataObject dataObject)
        {
            this.id = dataObject.Id;
            this.title = dataObject.Title;
            this.created = dataObject.Created;
            this.parentId = dataObject.ParentId ?? 0;
            this.tags = new List<string>();
            if (dataObject.DataObjectTags.Count > 0)
            {
                foreach (var tag in dataObject.DataObjectTags) {
                    this.tags.Add(tag.Value);
                }
            }
        }

        public long id { get; set; }

        public long parentId { get; set; }

        public string title { get; set; }

        public DateTime created { get; set; }

        public List<string> tags { get; set; }
    }
}