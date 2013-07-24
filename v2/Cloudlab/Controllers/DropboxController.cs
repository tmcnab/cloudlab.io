namespace Cloudlab.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Cloudlab.Helpers;
    using Cloudlab.Models;
    using DropNet;
    using DropNet.Exceptions;
    using System.Text;
    using System.IO;

    [Authorize]
    public class DropboxController : Controller
    {
        public JsonResult Info()
        {
            var dbClient = Dropbox.GetSession(User);
            var info = dbClient.AccountInfo();
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        #region Authorization

        public JsonResult IsAuthorized()
        {
            return Json(Dropbox.IsAuthorized(User), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AuthorizeS1()
        {
            try
            {
                var dbClient = Dropbox.GetSession();
                dbClient.GetToken();
                var url = dbClient.BuildAuthorizeUrl("http://cloudlab.apphb.com/api/dropbox/authorizeS2/");
                Session["DropNetClient"] = dbClient;
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                e.SendToACP();
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AuthorizeS2(long uid, string oauth_token)
        {
            if (!string.IsNullOrWhiteSpace(oauth_token))
            {
                try
                {
                    var dbClient = (DropNetClient)Session["DropNetClient"];
                    Session.Remove("DropNetClient");
                    var accessToken = dbClient.GetAccessToken();
                    using (var entities = new DatabaseEntities())
                    {
                        var user = entities.Users.Single(u => u.Email == User.Identity.Name);
                        user.DropboxSecret = accessToken.Secret;
                        user.DropboxToken = accessToken.Token;
                        entities.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    e.SendToACP();
                }
            }
            return View("~/Views/Main/Misc/DBAuthorizedView.cshtml");
        }

        public JsonResult DeAuthorize()
        {
            try
            {
                using (var entities = new DatabaseEntities())
                {
                    var user = entities.Users.Single(u => u.Email == User.Identity.Name);
                    user.DropboxToken = null;
                    user.DropboxSecret = null;
                    entities.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex){
                ex.SendToACP();
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Files

        public ActionResult Download(string path, bool asBinary = false)
        {
            try
            {
                var dropbox = Dropbox.GetSession(User);
                if (asBinary)
                {
                    return new HttpStatusCodeResult(501);
                }
                else
                {
                    return Content(UTF8Encoding.UTF8.GetString(dropbox.GetFile(path)));
                }
            }
            catch (DropboxException ex)
            {
                ex.InnerException.SendToACP();
                return new HttpStatusCodeResult(404);
            }

        }

        [HttpPost]
        public JsonResult Upload(string path)
        {
            try
            {
                var dropbox = Dropbox.GetSession(User);
                var filename = Path.GetFileName(path);
                path = Path.GetDirectoryName(path);
                byte[] buffer = new byte[Request.InputStream.Length];
                Request.InputStream.Read(buffer, 0, (int)Request.InputStream.Length);
                var mdata = dropbox.UploadFile(path, filename, buffer);
                return Json(true);
            }
            catch (DropboxException ex)
            {
                return Json(ex);
            }
        }

        #endregion
    }
}
