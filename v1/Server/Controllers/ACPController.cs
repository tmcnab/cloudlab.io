namespace Server.Controllers
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Server.Models;

    [Authorize(Roles="Administrators")]
    public class ACPController : Controller
    {
        #region Web 

        public ActionResult Index()
        {
            ViewBag.Violations = Repository.Administration.Violations();
            ViewBag.SupportRequests = Repository.Administration.SupportRequests(false);
            ViewBag.Users = Repository.Administration.Users();
            ViewBag.UsersOnline = Membership.GetNumberOfUsersOnline();

            ViewBag.TotalMemory = (System.GC.GetTotalMemory(true)/1024)/1024;
            ViewBag.CacheMemory = HttpRuntime.Cache.EffectivePercentagePhysicalMemoryLimit;
            
            return View();
        }

        #endregion
    }
}
