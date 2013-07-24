using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models.Json
{
    public class JsonToolObjectListResult
    {
        public JsonToolObjectListResult(ToolObject toolObject)
        {
            this.id = toolObject.Id;
            this.title = toolObject.Title;
            this.created = toolObject.Created;
            this.parentId = toolObject.ParentId ?? 0;
            this.tags = new List<string>();
            if (toolObject.ToolObjectTags.Count > 0)
            {
                foreach (var tag in toolObject.ToolObjectTags) {
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