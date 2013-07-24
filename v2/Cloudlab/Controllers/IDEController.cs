namespace Cloudlab.Controllers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Cloudlab.Helpers;
    using Cloudlab.Logic;
    using Cloudlab.Models;
    using Cloudlab.ViewModels;
    using DropNet;
    using DropNet.Exceptions;
    using System.Threading;

    [Authorize]
    public class IDEController : Controller
    {
        //public FileUploadJsonResult AjaxUpload(HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        using (var reader = new BinaryReader(file.InputStream))
        //        {
        //            entities.Resources.AddObject(new Resource()
        //            {
        //                Created = DateTime.UtcNow,
        //                IsPublic = false,
        //                MimeType = file.ContentType,
        //                User = entities.Users.Single(u => u.Email == User.Identity.Name),
        //                Content = reader.ReadBytes(file.ContentLength),
        //                Filename = file.FileName,
        //                VirtualPath = "/"
        //            });
        //            entities.SaveChanges();
        //        }
        //        return new FileUploadJsonResult()
        //        {
        //            Data = new
        //            {
        //                message = string.Format("{0} uploaded successfully", System.IO.Path.GetFileName(file.FileName))
        //            }
        //        };
        //    }
        //    catch
        //    {
        //        return new FileUploadJsonResult()
        //        {
        //            Data = new
        //            {
        //                message = string.Format("{0} uploaded terribly", System.IO.Path.GetFileName(file.FileName))
        //            }
        //        };
        //    }
        //}

        public ActionResult PushEvent()
        {
            string response;
            while (!VMManager.FetchMessage(User.Identity.Name, out response))
            {
                Thread.Sleep(10);
            }
            /*if(!VMManager.FetchMessage(User.Identity.Name, out response))
            {
                return new HttpStatusCodeResult(200);
            }*/
            return Content(response);
        }

        #region Dropbox

        public JsonResult DropboxIsAuthorized()
        {
            return Json(Dropbox.IsAuthorized(User), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DropboxDownload(string path, bool asBinary = false)
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

        

        public ActionResult DropboxListDir(string path = "/")
        {
            var dropbox = Dropbox.GetSession(User);
            var items = dropbox.GetMetaData(path);
            return Json(items.Contents, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DBAuth()
        {
            try
            {
                var client = Dropbox.GetSession();
                client.GetToken();
                var url = client.BuildAuthorizeUrl("http://cloudlab.apphb.com/IDE/DBAuthed/");
                Session["DropNetClient"] = client;
                return Json(url, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                e.SendToACP();
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DBAuthed(long uid, string oauth_token)
        {
            if (!string.IsNullOrWhiteSpace(oauth_token))
            {
                try
                {
                    var client = (DropNetClient)Session["DropNetClient"];
                    Session.Remove("DropNetClient");
                    var accessToken = client.GetAccessToken();
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
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }              
            return View("~/Views/Main/Misc/DBAuthorizedView.cshtml");
        }

        #endregion

        #region Documentation

        public ActionResult DocList()
        {
            using (var entities = new DatabaseEntities())
            {
                var model = entities.JSEntities.OrderBy(d => d.Category).ThenBy(e => e.Entity).ThenBy(f => f.Name).ToList();
                return Json(new JSEntityIndexViewModel(model).Model, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Doc(int id)
        {
            using (var entities = new DatabaseEntities())
            {
                return Content(entities.JSEntities.Single(d => d.Id == id).Body);
            }
        }

        public ActionResult DocSearch(string q = "")
        {
            if(string.IsNullOrWhiteSpace(q))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }


            using (var entities = new DatabaseEntities())
            {
                var terms = q.Split(new char[' '], StringSplitOptions.RemoveEmptyEntries).ToList();

                string firstTime = terms[0];
                var query = entities.JSEntities.Where(e => e.Body.Contains(firstTime))
                                               .ToList();

                for (int i = 1; i < terms.Count; i++)
                {
                    query = query.Where((a, b) => a.Body.Contains(terms[i])).ToList();
                }
                
                var jsonResults = query.Select(e => new
                {
                    id = e.Id,
                    category = e.Category,
                    entity = e.Entity,
                    name = e.Name,
                    type = e.Type
                });

                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Account Actions

        public JsonResult APIInfo()
        {
            using (var entities = new DatabaseEntities())
            {
                var user = entities.Users.Single(u => u.Email == User.Identity.Name); 
                var model = new
                {
                    key = user.MasterKey,
                    credits = Account.API.APIQuota.Get(user.MasterKey.ToString())
                };
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAPIPaymentLink(string sku)
        {
            var link  = "https://www.paymate.com/PayMate/ExpressPayment?";
                link += "mid=tristan@seditious-tech.com";
                link += "&currency=USD";
                link += "&pmt_sender_email=" + User.Identity.Name;
                link += "&return=http://cloudlab.apphb.com/main/PaymentReturned/";
                switch (sku) {
                    case "CLOUDLAB-API-1K":
                        link += "&amt=4.95&ref=CLOUDLAB-API-1K";
                        break;
                    case "CLOUDLAB-API-10K":
                        link += "amt=49.95&ref=CLOUDLAB-API-10K";
                        break;
                    case "CLOUDLAB-API-100K":
                        link += "&amt=149.95&ref=CLOUDLAB-API-100K";
                        break;
                }
                return Json(link, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AccountInfo()
        //{
        //    using (var entities = new DatabaseEntities())
        //    {
        //        var user = entities.Users.Single(u => u.Email == User.Identity.Name);
        //        var model = new {
        //            username = user.Email,
        //            created = user.Created,
        //            plan = user.ServiceLevel,
        //            expires = user.ServiceExpiration,
        //            contactable = user.IsContactable
        //        };
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion
    }
}
