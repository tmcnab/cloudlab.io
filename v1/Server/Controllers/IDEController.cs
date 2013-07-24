
namespace Server.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Server.Models;
    using Server.Models.Ajax;
    using Server.Controllers.Helpers;

    [Authorize]
    [HandleError]
    public class IDEController : Controller
    {
        #region Members

        protected void ProcessorGuard()
        {
            if (Session["processor"] == null)
            {
                Session.Add("processor", new DataProcessor(User));
            }
        }

        #endregion

        #region Web

        public ActionResult Index()
        {
            this.ProcessorGuard();
            return View();
        }

        #endregion

        #region AJAX

            #region Interpretation

            public ActionResult _vm(string cmd)
            {
                this.ProcessorGuard();
                var processor = (DataProcessor)Session["processor"];
                var result = processor.Process(cmd);
                Session["processor"] = processor;
                return Json(result, JsonRequestBehavior.AllowGet);
            } 

            #endregion

            public ActionResult _locals()
            {
                this.ProcessorGuard();
                var processor = (DataProcessor)Session["processor"];
                var results = processor.GetGlobals();
                return Json(results, JsonRequestBehavior.AllowGet);
            }

            #region '@ls' Command

            /// <summary>
            /// Lists data or tools according to the option provided by the user
            /// </summary>
            /// <param name="opts">The option to ls</param>
            /// <returns>Null if incorrect flag format, or a list of results</returns>
            public ActionResult _ls(string opts)
            {
                // Make sure it's an ajax request
                if (!Request.IsAjaxRequest()) {
                    return RedirectToAction("Index");
                }

                List<AjaxListResponseItem> items = new List<AjaxListResponseItem>();

                switch (opts)
                {
                    case "--data":
                    case "-d":
                        List<DataObject> dataObjects;
                        Repository.Data.List(User, out dataObjects);
                        //var dataObjects = this.repository.DataObjectGet(User);
                        foreach (var item in dataObjects)
                        {
                            items.Add(new AjaxListResponseItem(item.Id, item.Title));
                        }
                        break;

                    case "--tools":
                    case "-t":
                        List<ToolObject> toolObjects;
                        Repository.Tool.List(User, out toolObjects);
                        foreach (var item in toolObjects)
                        {
                            items.Add(new AjaxListResponseItem(item.Id, item.Title));
                        }
                        break;

                    default:
                        return Json(null, JsonRequestBehavior.AllowGet);
                }

                return Json(items, JsonRequestBehavior.AllowGet);
            }

            #endregion

            #region '@rm' Command

            [Authorize]
            public ActionResult _rm(string opt, int id = 0)
            {
                if(Request.IsAjaxRequest())
                {
                    switch (opt)
	                {   
		                case "-d":
                        case "--data":
                            return Json(Repository.Data.Delete(User, id) == RepositoryStatusCode.OK, JsonRequestBehavior.AllowGet);

                        case "-t":
                        case "--tool":
                            return Json(Repository.Optima.AjaxDeleteTool(User, id), JsonRequestBehavior.AllowGet);

                        default:
                            return Json(false, JsonRequestBehavior.AllowGet);
	                }
                }
                return RedirectToAction("Index");
            }

            #endregion

            #region '@reset' Command

            public ActionResult _reset()
            {
                // Make sure it's an ajax request
                if (!Request.IsAjaxRequest()) {
                    return RedirectToAction("Index");
                }

                Session["processor"] = new DataProcessor(User);
                return Json("", JsonRequestBehavior.AllowGet);
            }

            #endregion

            #region '@doc' Command

            public ActionResult _doc(string name, string opt)
            {
                // Make sure it's an ajax request
                if (Request.IsAjaxRequest())
                {
                    // Guard 
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return Json(null, JsonRequestBehavior.AllowGet);
                    }

                    if (string.IsNullOrWhiteSpace(opt))
                    {
                        return Json(Repository.Optima.AjaxDocumentationGet(name), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        try
                        {
                            switch (opt)
                            {
                                case "-t":
                                case "--tool":
                                    ToolObject toolObject;
                                    Repository.Tool.Read(User, long.Parse(name), out toolObject);
                                    return Json(new { title = toolObject.Title, body = toolObject.Description }, JsonRequestBehavior.AllowGet);

                                case "-d":
                                case "--data":
                                    DataObject dataObject;
                                    Repository.Data.Read(User, long.Parse(name), out dataObject);
                                    return Json(new { title = dataObject.Title, body = dataObject.Description }, JsonRequestBehavior.AllowGet);

                                default:
                                    return Json(null, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch
                        {
                            return Json(null, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            #endregion

        #endregion
    }
}