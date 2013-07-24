namespace Server.Controllers
{
    using System.Web.Mvc;

    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Workspace");
            }
            return View("Index");
        }

        public ActionResult Legal()
        {
            return View("Legal");
        }
    }
}
