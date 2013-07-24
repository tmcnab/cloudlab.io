
namespace Server.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Server.Models;
    using Server.Models.Json;
    using Server.Models.View;
    using Server.Models.Ajax;

    [HandleError]
    public class WorkspaceController : Controller
    {
        #region Web

        [Authorize]
        public ActionResult Index()
        {
            if (TempData["Notification"] != null) {
                ViewBag.Notification = TempData["Notification"];
            }

            List<DataObject> dataObjects;
            List<ToolObject> toolObjects;
            Repository.Data.List(User, out dataObjects);
            Repository.Tool.List(User, out toolObjects);

            var model = new WorkspaceViewModel(dataObjects.OrderByDescending(d => d.Created), toolObjects.OrderByDescending(d => d.Created));

            return View(model);
        }

        #endregion

        #region AJAX

        public ActionResult _Search(string q)
        {
            if (Request.IsAjaxRequest())
            {
                List<AjaxSearchResult> results;
                Repository.Search.All(User.Identity.Name, q, out results);
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region API

        #region GET:/Workspace/Data (Works, V&V Required)

        public ActionResult Data(string k, int p = 0)
            {
                // Ensure this is an AJAX request
                if(!Request.IsAjaxRequest()) {
                    return RedirectToAction("Index", "Workspace");
                }

                // Validate the parameters
                Guid key;
                if (!Guid.TryParse(k, out key) || p < 0) {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                List<DataObject> results;
                if (Repository.Data.List(key, out results) == RepositoryStatusCode.OK)
                {
                    List<JsonDataObjectListResult> data = new List<JsonDataObjectListResult>();
                    foreach (var item in results.OrderByDescending(t => t.Created).Skip(25*p).Take(25)) {
                        data.Add(new JsonDataObjectListResult(item));
                    }
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }

            #endregion

            #region GET:/Workspace/Tools (Works, V&V Required)

            public ActionResult Tools(string k, int p = 0)
            {
                // Ensure this is an AJAX request
                if (!Request.IsAjaxRequest()) {
                    return RedirectToAction("Index", "Workspace");
                }

                // Validate the parameters
                Guid key;
                if (!Guid.TryParse(k, out key) || p < 0) {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Grab the results
                List<ToolObject> results;
                if (Repository.Tool.List(key, out results) == RepositoryStatusCode.OK)
                {
                    List<object> data = new List<object>();
                    foreach (var item in results.OrderByDescending(t => t.Created).Skip(25 * p).Take(25))
                    {
                        data.Add(new { 
                            id = item.Id,
                            title = item.Title,
                            created = item.Created,
                            modified = item.Modified,
                            parentId = item.ParentId ?? 0,
                            isPublic = item.IsPublic,
                            description = item.Description,
                            userId = item.UserProfileId
                        });
                    }
                    
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }

            #endregion

        #endregion
    }
}
