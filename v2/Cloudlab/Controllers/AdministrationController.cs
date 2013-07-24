namespace Cloudlab.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Cloudlab.Models;
    using Cloudlab.ViewModels;

    [Authorize(Users="tristan@seditious-tech.com,tristan.j.mcnab@gmail.com")]
    public class AdministrationController : Controller
    {
        public static HashSet<Exception> Exceptions = new HashSet<Exception>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Errors()
        {
            return View(Exceptions);
        }

        public ActionResult Documentation(int id = 0)
        {
            if (id == 0)
            {
                return View(new JSEntityViewModel());
            }
            else
            {
                using (var entities = new DatabaseEntities())
                {
                    var document = entities.JSEntities.Single(d => d.Id == id);
                    if (document == null)
                    {
                        return View(new JSEntityViewModel());
                    }
                    else
                    {
                        return View(new JSEntityViewModel(document));
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult Documentation(JSEntityViewModel model)
        {
            using (var entities = new DatabaseEntities())
            {
                if (model.Id == 0)
                {
                    entities.JSEntities.AddObject(new JSEntity()
                    {
                        Body = model.Body,
                        Category = model.Category,
                        Entity = model.Entity,
                        Name = model.Name,
                        Type = model.Type
                    });
                }
                else
                {
                    var oldEntity = entities.JSEntities.Single(e => e.Id == model.Id);
                    oldEntity.Name = model.Name;
                    oldEntity.Type = model.Type;
                    oldEntity.Entity = model.Entity;
                    oldEntity.Category = model.Category;
                    oldEntity.Body = model.Body;
                }
                entities.SaveChanges();
                return RedirectToAction("Documentation");
            }
        }
    }
}
