namespace Server.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using Server.Models;
    using Server.Models.Ajax;
    using Server.Models.View;

    [HandleError]
    public class AccountController : Controller
    {
        #region Web
            
            #region /Account/

            [Authorize]
            public ActionResult Index()
            {
                ViewBag.Notification = TempData["Notification"];
                UserProfile profile;
                Repository.User.Read(User, out profile);
                return View(profile);
            }

            #endregion

            #region /Account/SignIn/

            public ActionResult SignIn()
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index");
                }
                return View();
            }


            [HttpPost]
            public ActionResult SignIn(LogOnModel model, string returnUrl)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index");
                }
                if (ModelState.IsValid)
                {
                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Workspace");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }

            #endregion

            #region /Account/SignOut/

            public ActionResult SignOut()
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Home");
            }

            #endregion

            #region /Account/ACP

            [Authorize(Roles = "Administrators")]
            public ActionResult ACP()
            {
                return View();
            }

            #endregion

            #region /Account/Recover

            public ActionResult Recover()
            {
                if (User.Identity.IsAuthenticated) {
                    return RedirectToAction("Index");
                }
                return View();
            }

            [HttpPost]
            public ActionResult Recover(RecoveryModel model)
            {
                if (User.Identity.IsAuthenticated) {
                    return RedirectToAction("Index");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var username = Membership.GetUserNameByEmail(model.Email);
                        var user = Membership.GetUser(username);
                        if (model.Username != username)
                        {
                            return View("RecoverComplete");
                        }
                        var newPassword = user.ResetPassword();
                        Mail.SendRecovery(user.Email, username, newPassword);
                    }
                    catch {
                        return View(model);
                    }
                    return View("RecoverComplete");
                }
                else {
                    return View(model);
                }
            }

            #endregion

            #region /Account/Register

            public ActionResult Register()
            {
                return View();
            }

            [HttpPost]
            public ActionResult Register(NewRegisterModel model)
            {
                if (ModelState.IsValid)
                {
                    // Attempt to register the user
                    MembershipCreateStatus createStatus;

                    if (Membership.FindUsersByEmail(model.REmail).Count != 0)
                    {
                        ModelState.AddModelError("", ErrorCodeToString(MembershipCreateStatus.DuplicateEmail));
                        return View(model);
                    }

                    var pwd = Membership.GeneratePassword(14, 1);
                    Membership.CreateUser(model.RUserName, pwd, model.REmail, null, null, true, out createStatus);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        Mail.SendRegistration(model.REmail, model.RUserName, pwd);
                        FormsAuthentication.SetAuthCookie(model.RUserName, false /* createPersistentCookie */);

                        // Add new user profile
                        var profile = new UserProfile()
                        {
                            UseGravatar = false,
                            AllowContact = false,
                            Coverage = (byte)CoverageLevel.Free,
                            CoverageUntil = DateTime.MaxValue,
                            APIKey = Guid.NewGuid(),
                            User = Membership.GetUser(model.RUserName).UserName,
                            FirstName = string.Empty,
                            LastName = string.Empty
                        };

                        Repository.User.Create(profile);

                        TempData["Notification"] = new AlertModel(AlertModel.Level.Info, "<strong>Thanks for registering!</strong> We've sent you an email with your account details. If you haven't recieved an email, please check your spam folder.");
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }

            #endregion

            #region /Account/Report

            [Authorize]
            public ActionResult Report(long dataId = 0, long toolId = 0)
            {
                if (dataId == 0 && toolId == 0) {
                    TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "There was an error submitting your report. Please submit a valid report.");
                    return RedirectToAction("Index", "Workspace");
                }

                if(Repository.Violations.Create(User, dataId, toolId) == RepositoryStatusCode.OK)
                    TempData["Notification"] = new AlertModel(AlertModel.Level.Info, "Thank you for your report.");
                else 
                    TempData["Notification"] = new AlertModel(AlertModel.Level.Error, "There was an error submitting your report. Please submit a support ticket.");

                return RedirectToAction("Index", "Workspace");
            }

            #endregion

            public ActionResult ThankYou()
            {
                TempData["Notification"] = new AlertModel(AlertModel.Level.Success, "<strong>Thank you!</strong> Your support helps cloudlab in more ways than you know!");
                return RedirectToAction("Index");
            }

        #endregion

        #region AJAX

            #region /Account/_Email

            [Authorize]
            public ActionResult _Email()
            {
                return Json(Membership.GetUser(User.Identity.Name).Email.ToLowerInvariant(), JsonRequestBehavior.AllowGet);
            }

            #endregion

            #region /Account/_Quota

            [Authorize]
            public ActionResult _Quota()
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        tools = Repository.Quotas.Tools(User),
                        data = Repository.Quotas.Data(User)
                    }, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Index");
            }

            #endregion

            #region /Account/_UpdateProfile

            [Authorize]
            [HttpPost]
            public ActionResult _UpdateProfile(AjaxCredentialsModel model)
            {
                // Make sure it's an ajax request
                if (!Request.IsAjaxRequest()) {
                    return RedirectToAction("Index");
                }

                if (ModelState.IsValid) {
                    return Json(Repository.Optima.AjaxProfileUpdate(User, model), JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            #endregion

            #region /Account/_SubmitTicket (Works)

            [Authorize]
            [HttpPost]
            public ActionResult _SubmitTicket(AjaxSupportTicketFormModel model)
            {
                if(Request.IsAjaxRequest()) 
                {
                    if (ModelState.IsValid) {
                        return Json(Repository.Optima.AjaxAddSupportTicket(User, model), JsonRequestBehavior.AllowGet);
                    }
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Index");
            }

            #endregion

            #region /Account/_SupportTicket

            [Authorize]
            public ActionResult _SupportTicketGet(int id = 0)
            {
                if (Request.IsAjaxRequest())
                {
                    if (id == 0) {
                        return Json(Repository.Optima.SupportTicketGet(User), JsonRequestBehavior.AllowGet);
                    }
                    else {
                        return Json(Repository.Optima.SupportTicketGet(User, id), JsonRequestBehavior.AllowGet);
                    }
                }
                return RedirectToAction("Index");
            }

            #endregion

            #region /Account/_RevokeAPIKey (Works)

            [Authorize]
            public ActionResult _RevokeAPIKey()
            {
                if (Request.IsAjaxRequest())
                {
                    Guid newKey;
                    if(Repository.User.RevokeAPIKey(User, out newKey) == RepositoryStatusCode.OK)
                        return Json(new { status = true, key = newKey.ToString() }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Index");
            }
            
            #endregion

        #endregion

        #region Status Codes

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}