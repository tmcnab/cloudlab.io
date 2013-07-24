
namespace Server.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Server.Models;
    using System.Linq;

    [HandleError]
    public class DocumentationController : Controller
    {
        #region Members & Support Methods

        [NonAction]
        protected List<SelectListItem> GetDocCategories()
        {
            var categories = new List<SelectListItem>();
            
            categories.Add(new SelectListItem() { Text = "Runtime", Value = "Runtime" });
            categories.Add(new SelectListItem() { Text = "Guides", Value = "Guides" });
            categories.Add(new SelectListItem() { Text = "Javascript", Value = "Javascript" });
            categories.Add(new SelectListItem() { Text = "Tutorials", Value = "Tutorials" });
            categories.Add(new SelectListItem() { Text = "API", Value = "API" });

            return categories;
        }

        #endregion

        #region Web

            #region /Documentation

            public ActionResult Index()
            {
                Dictionary<string, List<KeyValuePair<string, int>>> model = new Dictionary<string, List<KeyValuePair<string, int>>>();

                List<DocumentationItem> docItems;
                if (Repository.Documentation.List(out docItems) == RepositoryStatusCode.OK)
                {
                    docItems.Sort((a, b) => string.Compare(a.Name, b.Name));
                    
                    // Create Categories
                    foreach (var item in docItems)
                    {
                        if (!model.ContainsKey(item.Object))
                            model.Add(item.Object, new List<KeyValuePair<string,int>>());
                        
                        model[item.Object].Add(new KeyValuePair<string,int>(item.Name, item.Id));
                    }
                }

                return View(model);
            }

            #endregion

            #region /Documentation/Editor

            [HttpGet]
            [Authorize(Roles = "Administrators")]
            public ActionResult Editor(int id = 0)
            {
                ViewBag.Categories = this.GetDocCategories();

                // Guard
                if (id == 0) {
                    return View(new DocumentationItem() { 
                        Body = "This does something Reference[^1] of something.\n" +
                                "### Usage\n    X = Y()\n" +
                                "| Parameter | Required | Type(s) | Description |\n" +
                                "| - | - | - | - |\n" +
                                "[^1]: [Link Title (Source)](http://cloudlab.io 'External Link')"
                    });
                }

                // Fetch
                DocumentationItem item;
                if (Repository.Documentation.Read(id, out item) == RepositoryStatusCode.OK)
                    return View(item);
                else
                    return View(new DocumentationItem());
            }

            [Authorize(Roles = "Administrators")]
            [HttpPost]
            [ValidateInput(false)]
            public ActionResult Editor(DocumentationItem item)
            {
                if (ModelState.IsValid) {
                    var stat = Repository.Documentation.CreateOrUpdate(item);
                    return RedirectToAction("Index");
                }
                else {
                    ViewBag.Categories = this.GetDocCategories();
                    return View(item);
                }
            }

            #endregion

        #endregion

        #region AJAX

            #region /Documentation/_Details

            public ActionResult _Details(int id)
            {
                if (Request.IsAjaxRequest())
                {
                    DocumentationItem item;
                    if (Repository.Documentation.Read(id, out item) == RepositoryStatusCode.OK)
                        return Json(item, JsonRequestBehavior.AllowGet);
                    else
                        return Json(null, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Index");
            }

            #endregion

        #endregion

        #region API

        #endregion
    }
}
