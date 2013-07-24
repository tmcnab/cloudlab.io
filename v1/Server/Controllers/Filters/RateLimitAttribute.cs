namespace Server.Controllers.Filters
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitAttribute : ActionFilterAttribute
    {
        public int Seconds { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Using the IP Address here as part of the key but you could modify
            // and use the username if you are going to limit only authenticated users
            // filterContext.HttpContext.User.Identity.Name
            var key = string.Format("{0}-{1}-{2}",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName,
                filterContext.HttpContext.Request.UserHostAddress
            );
            var allowExecute = false;

            if (HttpRuntime.Cache[key] == null)
            {
                HttpRuntime.Cache.Add(key,
                    true,
                    null,
                    DateTime.Now.AddSeconds(Seconds),
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null);
                allowExecute = true;
            }

            if (!allowExecute)
            {
                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        status = 429,
                        message = string.Format("Too many requests in a {0} second time period", Seconds)
                    }
                };
                filterContext.HttpContext.Response.StatusCode = 200;
            }
        }
    }
}