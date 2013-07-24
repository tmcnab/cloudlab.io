
namespace Server.Controllers
{
    using System.Web.Mvc;
    using Server.Models;
    using Server.Models.View;

    [HandleError]
    public class ToolsController : Controller
    {
        #region Web

            #region /Tools/Details

            /// <summary>
            /// Displays a particular Tool
            /// </summary>
            /// <param name="id">The id of the tool to get</param>
            /// <returns>A view containing the models</returns>
            [Authorize]
            public ActionResult Details(long id)
            {
                ToolObject toolObject;
                switch (Repository.Tool.Read(User, id, out toolObject))
                {
                    case RepositoryStatusCode.Unauthorized:
                        TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "You are not authorized to view this tool.");
                        return RedirectToAction("Index", "Workspace");

                    case RepositoryStatusCode.NotFound:
                        TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "Could not find the specifed record.");
                        return RedirectToAction("Index", "Workspace");
                }

                // return view with model
                ViewBag.ModelType = "Tools";
                return View("ObjectDetails", toolObject);
            }

            #endregion

            #region /Tools/Create

            /// <summary>
            /// Returns a view containing a form
            /// </summary>
            /// <returns>A view containing the form</returns>
            [Authorize]
            public ActionResult Create()
            {
                ViewBag.ActionVerb = "Create";
                return View("ToolObjectForm", new ToolFormModel());
            }

            /// <summary>
            /// Submits a Tool to the system for inserting into the database
            /// </summary>
            /// <param name="model">The model to insert into the database</param>
            /// <returns>If okay or other, a redirect back to the workspace. If the model contained errors, it will return the model and any validation errors</returns>
            [Authorize]
            [HttpPost]
            public ActionResult Create(ToolFormModel model)
            {
                var alert = new AlertModel();

                if (ModelState.IsValid)
                {
                    switch (Repository.Tool.Create(User, model.ToToolObject()))
                    {
                        case RepositoryStatusCode.OK:
                            alert.Message = "Your tool was successfully added.";
                            alert.Style = AlertModel.Level.Success;
                            break;

                        case RepositoryStatusCode.UpgradeRequired:
                            alert.Message = "Tool was not added because you have exceeded your quota. Please upgrade your account.";
                            alert.Style = AlertModel.Level.Error;
                            break;

                        default:
                            alert.Message = "There was an error adding your tool. Please retry or lodge a support ticket.";
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
                return View("ToolObjectForm", model);
            }

            #endregion

            #region /Tools/Edit

            /// <summary>
            /// Retrieves a Tool for the User to Edit
            /// </summary>
            /// <param name="id">The ID number of the tool to edit</param>
            /// <returns>A form containing the tool (if authorized), or a redirect back to the workspace if not found or unauthorized</returns>
            [Authorize]
            public ActionResult Edit(long id)
            {
                ToolObject toolObject;
                switch (Repository.Tool.Read(User, id, out toolObject))
                {
                    case RepositoryStatusCode.Unauthorized:
                        TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "The tool you were trying to edit does not belong to you.");
                        return RedirectToAction("Index", "Workspace");

                    case RepositoryStatusCode.NotFound:
                        TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "The tool you were trying to edit does not exist or has since been deleted.");
                        return RedirectToAction("Index", "Workspace");
                }

                ViewBag.ActionVerb = "Edit";
                return View("ToolObjectForm", new ToolFormModel(toolObject));
            }

            /// <summary>
            /// Submits the Tool model to the database to edit
            /// </summary>
            /// <param name="id">Id number of the tool to update</param>
            /// <param name="model">The model data</param>
            /// <returns>A redirect to the workspace if successful or other, or the view with model data if validation failed</returns>
            [Authorize]
            [HttpPost]
            public ActionResult Edit(long id, ToolFormModel model)
            {
                if (ModelState.IsValid)
                {
                    var alert = new AlertModel();
                    switch (Repository.Tool.Update(User, model.ToToolObject()))
                    {
                        case RepositoryStatusCode.OK:
                            alert.Style = AlertModel.Level.Success;
                            alert.Message = "Your tool was successfully edited.";
                            break;

                        case RepositoryStatusCode.Unauthorized:
                            alert.Style = AlertModel.Level.Error;
                            alert.Message = "You are not authorized to edit this tool";
                            break;

                        case RepositoryStatusCode.NotFound:
                            alert.Style = AlertModel.Level.Error;
                            alert.Message = "The tool you were trying to edit does not exist or has since been deleted.";
                            break;

                        default:
                            alert.Style = AlertModel.Level.Error;
                            alert.Message = "There was an error updating your tool. Please lodge a <a href='/Account'>Support Request</a>.";
                            break;
                    }

                    TempData["Notification"] = alert;
                    return RedirectToAction("Index", "Workspace");
                }

                ViewBag.ActionVerb = "Edit";
                ViewBag.Notification = new AlertModel(AlertModel.Level.Warning, "There were some errors in your submission. Please ensure you complete all required fields.");
                return View("ToolObjectForm", model);
            }

            #endregion

            #region /Tools/Fork

            /// <summary>
            /// Forks a tool
            /// </summary>
            /// <param name="id">Id of the tool to fork</param>
            /// <returns>A redirection to the Workspace with any alerts stored in TempData</returns>
            [Authorize]
            public ActionResult Fork(long id)
            {
                // Perform the fork and determine response based on how the operation went
                AlertModel alert;
                switch (Repository.Tool.Fork(User, id))
                {
                    case RepositoryStatusCode.OK:
                        alert = new AlertModel(AlertModel.Level.Success, "Tool was successfully forked.");
                        break;

                    case RepositoryStatusCode.Unauthorized:
                        alert = new AlertModel(AlertModel.Level.Error, "You are not authorized to fork this tool.");
                        break;

                    case RepositoryStatusCode.NotFound:
                        alert = new AlertModel(AlertModel.Level.Error, "The tool you specified was not found.");
                        break;

                    default:
                        alert = new AlertModel(AlertModel.Level.Error, "An unknown error occurred. Please lodge a support ticket.");
                        break;
                }

                // Redirect back to the workspace
                TempData["Notification"] = alert;
                return RedirectToAction("Index", "Workspace");
            }

            #endregion

            #region /Tools/Delete

            /// <summary>
            /// Deletes a tool from the database
            /// </summary>
            /// <param name="id">The ID of the tool to delete</param>
            /// <returns>A redirect back to the workspace with the status of the operation</returns>
            [Authorize]
            public ActionResult Delete(long id)
            {
                AlertModel alert;
                switch (Repository.Tool.Delete(User, id))
                {
                    case RepositoryStatusCode.OK:
                        alert = new AlertModel(AlertModel.Level.Success, "Tool was successfully deleted.");
                        break;

                    case RepositoryStatusCode.Unauthorized:
                        alert = new AlertModel(AlertModel.Level.Error, "You are not authorized to delete this tool.");
                        break;

                    case RepositoryStatusCode.NotFound:
                        alert = new AlertModel(AlertModel.Level.Error, "The tool you specified was not found.");
                        break;

                    default:
                        alert = new AlertModel(AlertModel.Level.Error, "An unknown error occurred. Please lodge a support ticket.");
                        break;   
                }

                TempData["Notification"] = alert;
                return RedirectToAction("Index", "Workspace");
            }

            #endregion

        #endregion

        #region AJAX

            #region /Tools/_Details

            /// <summary>
            /// Gets the details (body) of a Tool and returns the result as JSON
            /// </summary>
            /// <param name="id">The ID of the tool to fetch information for</param>
            /// <returns>JSON object if successful, null if not (as JSON)</returns>
            public ActionResult _Details(long id)
            {
                ToolObject toolObject;
                switch (Repository.Tool.Read(User, id, out toolObject))
                {
                    case RepositoryStatusCode.OK:
                        return Json(new { body = toolObject.Description }, JsonRequestBehavior.AllowGet);

                    default:
                        return Json(null, JsonRequestBehavior.AllowGet);
                }
            }

            #endregion

        #endregion
    }
}