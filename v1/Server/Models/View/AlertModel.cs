
namespace Server.Models.View
{
    public class AlertModel
    {
        public AlertModel() { }

        public AlertModel(Level style, string message)
        {
            this.Style = style;
            this.Message = message;
        }

        public string Message { get; set; }

        public Level Style { get; set; }

        public enum Level
        {
            Success,
            Warning,
            Error,
            Info
        }
    }
}