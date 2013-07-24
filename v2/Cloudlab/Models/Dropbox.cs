namespace Cloudlab.Models
{
    using System.Linq;
    using System.Security.Principal;
    using DropNet;

    public static class Dropbox
    {
        private const string APIUsername = "45b9iptimsftpmg";
        private const string APISecret   = "2cynyrd5bdewakk";

        public static DropNetClient GetSession(IPrincipal user)
        {
            return Dropbox.GetSession(user.Identity.Name);
        }

        public static DropNetClient GetSession(string user)
        {
            using (var entities = new DatabaseEntities())
            {
                var _user = entities.Users.Single(u => u.Email == user);
                return new DropNetClient(APIUsername, APISecret, _user.DropboxToken, _user.DropboxSecret) { UseSandbox = true };
            }
        }

        public static DropNetClient GetSession()
        {
            return new DropNetClient(APIUsername, APISecret);
        }

        public static bool IsAuthorized(IPrincipal User)
        {
            using(var entities = new DatabaseEntities()) 
            {
                var _user = entities.Users.Single(u => u.Email == User.Identity.Name);
                try
                {
                    var db = new DropNetClient(APIUsername, APISecret, _user.DropboxToken, _user.DropboxSecret) { UseSandbox = true };
                    db.AccountInfo();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}