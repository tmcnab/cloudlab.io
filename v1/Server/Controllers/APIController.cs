
namespace Server.Controllers
{
    using System;
    using System.IO;
    using System.Web.Mvc;
    using Server.Models;
    using System.Web;
    using System.Linq;
    using Server.Controllers.Filters;

    public class APIController : Controller
    {
        #region Helper Methods

        protected string GetBody(HttpRequestBase request)
        {
            string responseBody = string.Empty;
            using (StreamReader reader = new StreamReader(request.InputStream))
                responseBody = reader.ReadToEnd();

            return responseBody;
        }

        protected Guid GetAuth(HttpRequestBase request)
        {
            if (request.Headers.HasKeys())
            {
                Guid g;
                if (Guid.TryParse(request.Headers["Authorization"], out g))
                {
                    return g;
                }
            }
            return Guid.Empty;
        }

        #endregion

        #region /API/

        [RateLimit(Seconds=5)]
        public ActionResult Index()
        {
            if (APIRepository.User.Exists(GetAuth(Request)))
                return APIRepository.API.Status();
            else
                return APIRepository.JsonResults.Unauthorized;
        }

        #endregion

        #region /API/Documentation/ (CEMENT)

        [RateLimit(Seconds=5)]
        [HttpGet]
        public ActionResult Documentation(string command)
        {
            if (APIRepository.User.Exists(GetAuth(Request)))
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    return APIRepository.Documentation.List();
                }
                int n = 0;
                if (int.TryParse(command, out n))
                {
                    return APIRepository.Documentation.Read(n);
                }
                return APIRepository.JsonResults.BadRequest;
            }
            else
            {
                return APIRepository.JsonResults.Unauthorized;
            }
        }

        #endregion

        #region /API/VM/

        [RateLimit(Seconds=1)]
        [HttpPost]
        public ActionResult VM(string command)
        {
            // Decode API Key and Check Authorization
            var key = GetAuth(Request);
            if (!APIRepository.User.Exists(key))
                return APIRepository.JsonResults.Unauthorized;

            // If the command string is empty, process the request body as VM commands
            if (string.IsNullOrWhiteSpace(command))
            {
                var body = GetBody(Request);
                if (APIRepository.User.Exists(key))
                    return Json(APIRepository.Session.Evaluate(key, body), JsonRequestBehavior.AllowGet);
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }

            // If the command string is not empty, process the command
            switch (command.Trim())
            {
                case "reset":
                    APIRepository.Session.Reset(key);
                    break;

                // The user did not provide a valid command
                default:
                    return APIRepository.JsonResults.BadRequest;
            }

            // Everything is A-OK
            return APIRepository.JsonResults.OK;
        }

        #endregion

        #region /API/Data/

        [RateLimit(Seconds=5)]
        public ActionResult Data(string id)
        {
            // Check Authorization
            var key = GetAuth(Request);
            if (key == Guid.Empty)
                return APIRepository.JsonResults.Unauthorized;

            switch (Request.HttpMethod)
            {
                #region GET
                case "GET":
                {
                    // Empty Parameter - List Data
                    if (string.IsNullOrEmpty(id))
                        return APIRepository.Data.List(key);

                    // Check Parameter
                    long _id;
                    if (long.TryParse(id, out _id))
                        return APIRepository.Data.Read(key, _id);

                    return APIRepository.JsonResults.BadRequest;
                }
                #endregion

                #region POST
                case "POST":
                {
                    return APIRepository.Data.Create(key, GetBody(Request));
                }
                #endregion

                #region PUT
                case "PUT":
                {
                    // Empty Parameter - List Data
                    if (string.IsNullOrEmpty(id))
                        return APIRepository.JsonResults.BadRequest;

                    // Check Parameter
                    long _id;
                    if (long.TryParse(id, out _id))
                        return APIRepository.Data.Update(key, _id, GetBody(Request));
                    else
                        return APIRepository.JsonResults.BadRequest;
                }
                #endregion

                #region DELETE
                case "DELETE":
                {
                    if (string.IsNullOrEmpty(id))
                        return APIRepository.JsonResults.BadRequest;

                    long _id;
                    if (long.TryParse(id, out _id))
                        return APIRepository.Data.Delete(key, _id);

                    return APIRepository.JsonResults.BadRequest;
                }
                #endregion

                default:
                return APIRepository.JsonResults.BadRequest;
            }
        }

        #endregion

        #region /API/Tool/

        [RateLimit(Seconds = 5)]
        public ActionResult Tool(string id)
        {
            // Check Authorization
            var key = GetAuth(Request);
            if (key == Guid.Empty)
                return APIRepository.JsonResults.Unauthorized;

            switch (Request.HttpMethod)
            {
                #region GET
                case "GET":
                    {
                        // Empty Parameter - List Data
                        if (string.IsNullOrEmpty(id))
                            return APIRepository.Tool.List(key);

                        // Check Parameter
                        long _id;
                        if (long.TryParse(id, out _id))
                            return APIRepository.Tool.Read(key, _id);

                        return APIRepository.JsonResults.BadRequest;
                    }
                #endregion

                #region POST
                case "POST":
                    {
                        return APIRepository.Tool.Create(key, GetBody(Request));
                    }
                #endregion

                #region PUT
                case "PUT":
                    {
                        // Empty Parameter - List Data
                        if (string.IsNullOrEmpty(id))
                            return APIRepository.JsonResults.BadRequest;

                        // Check Parameter
                        long _id;
                        if (long.TryParse(id, out _id))
                            return APIRepository.Tool.Update(key, _id, GetBody(Request));
                        else
                            return APIRepository.JsonResults.BadRequest;
                    }
                #endregion

                #region DELETE
                case "DELETE":
                    {
                        if (string.IsNullOrEmpty(id))
                            return APIRepository.JsonResults.BadRequest;

                        long _id;
                        if (long.TryParse(id, out _id))
                            return APIRepository.Tool.Delete(key, _id);

                        return APIRepository.JsonResults.BadRequest;
                    }
                #endregion

                default:
                    return APIRepository.JsonResults.BadRequest;
            }
        }

        #endregion
    }
}
