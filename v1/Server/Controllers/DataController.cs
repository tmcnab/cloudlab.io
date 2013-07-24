namespace Server.Controllers
{
    using System.Web.Mvc;
    using NLog;
    using Server.Models;
    using Server.Models.Ajax;
    using Server.Models.View;

    [HandleError]
    public class DataController : Controller
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        #region Web

            #region /Data/Details
            
            [Authorize]
            public ActionResult Details(long id = 0)
            {
                DataObject dataObject;

                // fetch data object and handle any errors
                switch (Repository.Data.Read(User, id, out dataObject))
                {
                    case RepositoryStatusCode.Unauthorized:
                        TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "You are not authorized to view this data.");
                        return RedirectToAction("Index", "Workspace");

                    case RepositoryStatusCode.NotFound:
                        TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "The data you were looking for does not exist or was deleted.");
                        return RedirectToAction("Index", "Workspace");
                }

                // return view with model
                ViewBag.ModelType = "Data";
                return View("ObjectDetails", dataObject);
            }

            #endregion

            #region /Data/Create (Working, V&V Required)

            [Authorize]
            public ActionResult Create()
            {
                ViewBag.ActionVerb = "Create";
                return View("DataObjectForm", new DataFormModel());
            }

            [Authorize]
            [HttpPost]
            public ActionResult Create(DataFormModel model)
            {
                var alert = new AlertModel();

                if (ModelState.IsValid)
                {
                    switch (Repository.Data.Create(User, model.ToDataObject()))
                    {
                        case RepositoryStatusCode.OK:
                            alert.Message = "Your data was successfully added.";
                            alert.Style = AlertModel.Level.Success;
                            break;

                        case RepositoryStatusCode.UpgradeRequired:
                            alert.Message = "Data was not added because you have exceeded your quota. Please upgrade your account.";
                            alert.Style = AlertModel.Level.Error;
                            break;

                        case RepositoryStatusCode.BadInput:
                            alert.Message = "There was an error with your submission. You did not submit valid JSON.";
                            alert.Style = AlertModel.Level.Error;
                            ViewBag.Notification = alert;
                            ViewBag.ActionVerb = "Create";
                            return View("DataObjectForm", model);

                        default:
                            alert.Message = "There was an error adding your data. Please retry or lodge a support ticket.";
                            alert.Style = AlertModel.Level.Error;
                            break;
                    }

                    TempData["Notification"] = alert;
                    return RedirectToAction("Index", "Workspace");
                }
               
                // Validation failed, so let the user know
                alert.Message = "There was an error with your submission. Please correct the errors and resubmit.";
                alert.Style = AlertModel.Level.Error;
                ViewBag.Notification = alert;
                ViewBag.ActionVerb = "Create";
                
                // Return the model to the form
                return View("DataObjectForm", model);
            }

            #endregion

            #region /Data/Edit

            [Authorize]
            public ActionResult Edit(long id)
            {
                DataObject dataObject;
                var code = Repository.Data.Read(User, id, out dataObject);

                if (code == RepositoryStatusCode.NotFound)
                {
                        TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "The data you were trying to edit does not exist or has since been deleted.");
                        return RedirectToAction("Index", "Workspace");
                }

                if (dataObject.UserProfile.User != User.Identity.Name)
                {
                    TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "The data you were trying to edit does not belong to you.");
                    return RedirectToAction("Index", "Workspace");
                }

                ViewBag.ActionVerb = "Edit";
                return View("DataObjectForm", new DataFormModel(dataObject));
            }

            [Authorize]
            [HttpPost]
            public ActionResult Edit(long id, DataFormModel model)
            {
                if (ModelState.IsValid)
                {
                    var alert = new AlertModel();
                    switch (Repository.Data.Update(User, model.ToDataObject()))
                    {
                        case RepositoryStatusCode.BadInput:
                            alert.Message = "There was an error with your submission. You did not submit valid JSON.";
                            alert.Style = AlertModel.Level.Error;
                            ViewBag.Notification = alert;
                            ViewBag.ActionVerb = "Edit";
                            return View("DataObjectForm", model);

                        case RepositoryStatusCode.OK:
                            alert.Style = AlertModel.Level.Success;
                            alert.Message = "Your data was successfully edited.";
                            break;

                        case RepositoryStatusCode.Unauthorized:
                            alert.Style = AlertModel.Level.Error;
                            alert.Message = "You are not authorized to edit this data";
                            break;

                        case RepositoryStatusCode.NotFound:
                            alert.Style = AlertModel.Level.Error;
                            alert.Message = "The data you were trying to edit does not exist or has since been deleted.";
                            break;

                        default:
                            alert.Style = AlertModel.Level.Error;
                            alert.Message = "There was an error updating your data. Please lodge a <a href='/Account'>Support Request</a>.";
                            break;
                    }

                    TempData["Notification"] = alert;
                    return RedirectToAction("Index", "Workspace");
                }

                ViewBag.ActionVerb = "Edit";
                ViewBag.Notification = new AlertModel(AlertModel.Level.Warning, "There were some errors in your submission. Please ensure you complete all required fields.");
                return View("DataObjectForm", model);
            }

            #endregion

            #region /Data/Fork (Working, V&V Required)

            [Authorize]
            public ActionResult Fork(long id)
            {
                var alert = new AlertModel();
                switch (Repository.Data.Fork(User, id))
                {
                    case RepositoryStatusCode.OK:
                        alert.Style = AlertModel.Level.Success;
                        alert.Message = "Data successfully forked";
                        break;

                    case RepositoryStatusCode.UpgradeRequired:
                        alert.Style = AlertModel.Level.Warning;
                        alert.Message = "Your data fork was not processed as you would be over account privelege.";
                        break;

                    case RepositoryStatusCode.Unauthorized:
                        alert.Style = AlertModel.Level.Error;
                        alert.Message = "Your are not authorized to fork this data.";
                        break;

                    case RepositoryStatusCode.NotFound:
                        alert.Style = AlertModel.Level.Error;
                        alert.Message = "The data you were trying to fork does not exist or was deleted.";
                        break;

                    default:
                        alert.Style = AlertModel.Level.Error;
                        alert.Message = "There was an error forking this data. Please lodge a <a href=/Account/Support/'>support ticket</a>.";
                        break;
                }

                TempData["Notification"] = alert;
                return RedirectToAction("Index", "Workspace");
            }

            #endregion

            #region /Data/Delete (Working, V&V Required)

            [Authorize]
            public ActionResult Delete(long id)
            {
                var alert = new AlertModel();

                switch (Repository.Data.Delete(User, id))
                {
                    case RepositoryStatusCode.OK:
                        alert.Style = AlertModel.Level.Success;
                        alert.Message = "Data was successfully deleted.";
                        break;

                    case RepositoryStatusCode.Unauthorized:
                        alert.Style = AlertModel.Level.Error;
                        alert.Message = "You are not authorized to delete this data.";
                        break;

                    case RepositoryStatusCode.NotFound:
                        alert.Style = AlertModel.Level.Error;
                        alert.Message = "The data you specified for deletion was not found.";
                        break;

                    default:
                        alert.Style = AlertModel.Level.Error;
                        alert.Message = "There was an error processing your deletion request. Please lodge a <a href='/Account/Support/'>Support Ticket</a>";
                        break;
                }

                TempData["Notification"] = alert;
                return RedirectToAction("Index", "Workspace");
            }

            #endregion

        #endregion

        #region AJAX

            #region /Data/_Details

            public ActionResult _Details(long id)
            {
                DataObject modelSource;
                switch (Repository.Data.Read(User, id, out modelSource))
                {
                    case RepositoryStatusCode.OK:
                        return Json(new AjaxDataItem(modelSource), JsonRequestBehavior.AllowGet);

                    case RepositoryStatusCode.Unauthorized:
                        return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                    case RepositoryStatusCode.NotFound:
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                    default:
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            #endregion

        #endregion
    }
}