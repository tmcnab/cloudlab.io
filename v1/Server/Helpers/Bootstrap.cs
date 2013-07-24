
namespace Server.Helpers
{
    using System.Web.Mvc;
    using Server.Models.View;

    public static class Bootstrap
    {
        public static MvcHtmlString NotificationAlert(this HtmlHelper helper, AlertModel model)
        {
            var html = "<div  class='alert fade in ";
            switch(model.Style) {
                case AlertModel.Level.Success:
                    html += "alert-success";
                    break;
                case AlertModel.Level.Warning:
                    html += "alert-warning";
                    break;
                case AlertModel.Level.Error:
                    html += "alert-error";
                    break;
                case AlertModel.Level.Info:
                    html += "alert-info";
                    break;
            }
            html += "\'><a class='close' data-dismiss='alert'>&times;</a>" + model.Message + "</div>";
            return MvcHtmlString.Create(html);
        }
    }
}