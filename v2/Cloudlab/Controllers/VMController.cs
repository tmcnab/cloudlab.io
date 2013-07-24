namespace Cloudlab.Controllers
{
    using System;
    using System.Web.Mvc;
    using Cloudlab.Helpers;
    using Cloudlab.Logic;

    public class VMController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.InputStream))
                {
                    try { return Json(VMManager.Execute(User.Identity.Name, reader.ReadToEnd())); }
                    catch (Exception e)
                    {
                        e.SendToACP();
                        return Json(null);
                    }
                }
            }
            else { return RedirectToAction("Index", "Main"); }
        }

        [HttpPost]
        public ActionResult Reset()
        {
            if (Request.IsAjaxRequest()) {
                try
                {
                    VMManager.AddOrReset(User.Identity.Name);
                    return Json(true);
                }
                catch { return Json(false); }
            }
            else
            {
                return RedirectToAction("Index", "Main");
            }
        }
    }
}
