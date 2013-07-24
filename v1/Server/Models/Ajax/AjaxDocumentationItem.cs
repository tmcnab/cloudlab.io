
namespace Server.Models.Ajax
{
    using Server.Models;

    public class AjaxDocumentationItem
    {
        public AjaxDocumentationItem(DocumentationItem item)
        {
            this.Id = item.Id;
            this.Title = item.Title;
            this.Body = item.Body;
            this.Category = item.Category;
        }

        public int Id { get; set; }

        public string Category { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}