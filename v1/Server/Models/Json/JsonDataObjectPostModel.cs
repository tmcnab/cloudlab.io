
namespace Server.Models.Json
{
    public class JsonDataObjectPostModel
    {
        public JsonDataObjectPostModel(string title, string description, string data, bool ispublic)
        {
            this.Title = title;
            this.Description = description;
            this.Data = data;
            this.IsPublic = ispublic;
        }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public string Data { get; set; }

        public bool IsPublic { get; set; }
    }
}