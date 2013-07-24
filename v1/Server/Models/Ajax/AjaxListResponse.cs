
namespace Server.Models.Ajax
{
    public class AjaxListResponseItem
    {
        public AjaxListResponseItem() { }

        public AjaxListResponseItem(long id, string title)
        {
            this.Id = id;
            this.Title = title;
        }

        public long Id { get; set; }

        public string Title { get; set; }
    }
}