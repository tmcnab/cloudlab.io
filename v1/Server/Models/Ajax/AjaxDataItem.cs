
namespace Server.Models.Ajax
{
    public class AjaxDataItem
    {
        public AjaxDataItem(DataObject dataObject)
        {
            this.Description = dataObject.Description;
            this.Data = dataObject.Data;
        }

        public string Description { get; set; }

        public string Data { get; set; }
    }
}