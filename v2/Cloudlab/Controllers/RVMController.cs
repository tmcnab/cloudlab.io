namespace Cloudlab.Controllers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web.Mvc;
    using Cloudlab.Logic;
    using Cloudlab.Models;

    public class RVMController : Controller
    {
        static ConcurrentBag<Guid> CurrentKeys = new ConcurrentBag<Guid>();

        DatabaseEntities entities = new DatabaseEntities();

        // Executes a remote JS connection
        [HttpPost]
        public ActionResult Index()
        {
            var key = GetPermissions();
            if (key == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "{ \"error\": \"provided api key was not good\" }");
            }

            if (CurrentKeys.Contains(key.Key))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "{ \"error\": \"provided api key is already in use\" }");
            }

            try
            {
                if (!Account.API.APIQuota.Decrement(key.User.MasterKey.ToString()))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.PaymentRequired, "RUN OUT OF CREDITS SON!");
                }
                CurrentKeys.Add(key.Key);
                return Content(new APIVM(key.User.Email, GetTimeout(), key.ReadFlag, key.WriteFlag).Interpret(GetBody()));
            }
            finally
            {
                Guid k;
                CurrentKeys.TryTake(out k);
            }
            
        }

        // Returns a list of all of the user's API Keys
        [HttpGet]
        public ActionResult Keys()
        {
            var user = GetUser();
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "{ \"error\": \"provided master api key was not good\" }");
            }
            else
            {
                var keys = user.APIKeys.Select((key,value) => new { key = key.Key, read = key.ReadFlag, write = key.WriteFlag }).ToList();

                return Json(keys, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult NextKey()
        {
            var user = GetUser();
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "{ \"error\": \"provided master api key was not good\" }");
            }
            else
            {
                var keys = from k in user.APIKeys
                           select k.Key;
                var difference = keys.Except(CurrentKeys);
                
                return Json(difference, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Quota()
        {
            var user = GetUser();
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "{ \"error\": \"provided master api key was not good\" }");
            }
            else
            {
                return Json(Account.API.APIQuota.Get(user.MasterKey.ToString()), JsonRequestBehavior.AllowGet);
            }
        }

        #region Backend Request Helpers

        [NonAction]
        private string GetBody()
        {
            byte[] buffer = new byte[Request.InputStream.Length];
            Request.InputStream.Read(buffer, 0, (int)Request.InputStream.Length);
            return UTF8Encoding.UTF8.GetString(buffer);
        }

        [NonAction]
        private TimeSpan GetTimeout()
        {
            if (Request.Headers.HasKeys() && !string.IsNullOrWhiteSpace(Request.Headers["timeout"]))
            {
                return new TimeSpan(0, 0, 0, 0, int.Parse(Request.Headers["timeout"]));
            }
            else
            {
                return new TimeSpan(0, 0, 30);
            }
        }

        [NonAction]
        private User GetUser()
        {
            Guid g;
            var masterKey = Request.Headers.HasKeys() ? Request.Headers["Authorization"] : string.Empty;
            if (string.IsNullOrWhiteSpace(masterKey) || !Guid.TryParse(masterKey, out g))
            {
                return null;
            }
            else
            {
                return this.entities.Users.SingleOrDefault(u => u.MasterKey == g);
            }
        }

        [NonAction]
        private User GetAccess()
        {
            Guid g;
            var accessKey = Request.Headers.HasKeys() ? Request.Headers["Authorization"] : string.Empty;
            if (string.IsNullOrWhiteSpace(accessKey) || !Guid.TryParse(accessKey, out g))
            {
                return null;
            }
            else
            {
                return entities.Users.SingleOrDefault(u => u.APIKeys.Where(k => k.Key == g).Count() > 0);
            }
        }

        [NonAction]
        private APIKey GetPermissions()
        {
            Guid g;
            var accessKey = Request.Headers.HasKeys() ? Request.Headers["Authorization"] : string.Empty;
            if (string.IsNullOrWhiteSpace(accessKey) || !Guid.TryParse(accessKey, out g))
            {
                return null;
            }
            else
            {
                return entities.APIKeys.SingleOrDefault(k => k.Key == g);
            }
        }

        #endregion
    }
}
