
namespace Server.Models
{
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Configuration;
    using System.Configuration;

    public static class Mail
    {
        static Mail()
        {
            using (StreamReader reader = new StreamReader(HostingEnvironment.VirtualPathProvider.GetFile("~/Content/mail/Recovery.html").Open())) {
                Mail.RecoveryTemplate = reader.ReadToEnd();
            }
            using (StreamReader reader = new StreamReader(HostingEnvironment.VirtualPathProvider.GetFile("~/Content/mail/Registration.html").Open()))
            {
                Mail.RegistrationTemplate = reader.ReadToEnd();
            }
        }

        private static string RecoveryTemplate = string.Empty;
        private static string RegistrationTemplate = string.Empty;

        private static readonly string host = "smtp.sendgrid.net";
        private static readonly int port = 587;
        private static readonly string accountName = "cloudlab.io@seditious-tech.com";

        
        public static MailStatusCode SendRecovery(string email, string username, string password)
        {
            var mailMessage = new MailMessage(accountName, email);
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = "cloudlab.io - Account Recovery";
            mailMessage.Body = Mail.RecoveryTemplate.Replace("###username###", username).Replace("###password###", password);

            return Mail.Send(mailMessage);
        }

        public static MailStatusCode SendRegistration(string email, string username, string password)
        {
            var mailMessage = new MailMessage(accountName, email);
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = "cloudlab.io - Account Registration";
            mailMessage.Body = Mail.RegistrationTemplate.Replace("###username###", username).Replace("###password###", password);

            return Mail.Send(mailMessage);
        }

        private static MailStatusCode Send(MailMessage message)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = Mail.host;
            client.Port = Mail.port;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(WebConfigurationManager.AppSettings["SENDGRID_USERNAME"],
                                                       WebConfigurationManager.AppSettings["SENDGRID_PASSWORD"]);
            
            client.SendAsync(message, new object());

            return MailStatusCode.OK;
        }

    }

    public enum MailStatusCode
    {
        OK,
        Error
    }
}