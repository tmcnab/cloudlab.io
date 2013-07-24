namespace Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Server.Models.Processors;

    public static class APIRepository
    {
        static TauEntities Entities = new TauEntities();

        public static class JsonResults
        {
            public static JsonResult BadRequest = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    status = HttpStatusCode.BadRequest,
                    message = "The request you made was improper",
                    error = true
                }
            };

            public static JsonResult NotImplemented = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    status = HttpStatusCode.NotImplemented,
                    message = "This action has not been implemented",
                    error = true
                }
            };

            public static JsonResult NotFound = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    status = 404,
                    message = "The item specified was not found",
                    error = true
                }
            };

            public static JsonResult ServerError = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    status = 500,
                    message = "There was an error servicing your request",
                    error = true
                }
            };

            public static JsonResult Unauthorized = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    status = 401,
                    message = "You are unauthorized to perform this request",
                    error = true
                }
            };

            public static JsonResult UpgradeRequired = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    status = 402,
                    message = "This request requires an account upgrade to proceed",
                    error = true
                }
            };

            public static JsonResult OK = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    status = "200",
                    error = false
                }
            };


        }

        const int PageSize = 25;

        public static class Documentation
        {
            public static JsonResult List()
            {
                List<object> results = new List<object>();
                foreach (var item in Entities.DocumentationItems.OrderBy(p => p.Object).ThenBy(q => q.Name))
                {
                    results.Add(new {
                        id = item.Id,
                        category = item.Category,
                        @object = item.Object,
                        name = item.Name
                    });
                }

                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = results
                };
            }

            public static JsonResult Read(int n)
            {
                DocumentationItem item;
                if (Repository.Documentation.Read(n, out item) == RepositoryStatusCode.OK)
                {
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = item.Body
                    };
                }
                else
                {
                    return JsonResults.NotFound;
                }
            }
        }

        public static class Data
        {
            public static JsonResult Create(Guid key, string json)
            {
                APIDataObject apiDataObject;
                try {
                    apiDataObject = JsonConvert.DeserializeObject<APIDataObject>(json, new JsonSerializerSettings()
                    {
                    });
                }
                catch {
                    return JsonResults.BadRequest;
                }

                try
                {
                    JsonConvert.DeserializeObject(apiDataObject.data);
                }
                catch
                {
                    return JsonResults.BadRequest;
                }

                DataObject dataObject = new DataObject() {
                    Created = DateTime.UtcNow,
                    Data = Regex.Replace(apiDataObject.data, @"\s+", string.Empty),
                    IsPublic = apiDataObject.isPublic,
                    Description = apiDataObject.description,
                    Title = apiDataObject.title
                };
                if (apiDataObject.tags != null)
                {
                    foreach (var tag in apiDataObject.tags)
                    {
                        dataObject.DataObjectTags.Add(new DataObjectTag()
                        {
                            Value = tag
                        });
                    }
                }

                switch (Repository.Data.Create(key, dataObject))
	            {
		            case RepositoryStatusCode.OK:
                        return JsonResults.OK;
                        
                    case RepositoryStatusCode.Unauthorized:
                        return JsonResults.Unauthorized;
                        
                    case RepositoryStatusCode.UpgradeRequired:
                        return JsonResults.UpgradeRequired;
                    
                    default:
                        return JsonResults.ServerError;
	            }
            }

            public static JsonResult Delete(Guid key, long id)
            {
                try
                {
                    switch (Repository.Data.Delete(key, id))
                    {
                        case RepositoryStatusCode.OK:
                            return JsonResults.OK;
                            
                        case RepositoryStatusCode.Unauthorized:
                            return JsonResults.Unauthorized;
                            
                        case RepositoryStatusCode.NotFound:
                            return JsonResults.NotFound;

                        default:
                            return JsonResults.ServerError;
                    }
                }
                catch
                {
                    return JsonResults.ServerError;
                }
            }

            public static JsonResult List(Guid key)
            {
                try
                {
                    List<object> results = new List<object>();

                    // Grab all the user's data results
                    foreach (var dataObject in Entities.DataObjects.Where(p => p.UserProfile.APIKey == key))
                    {
                        results.Add(new
                        {
                            id = dataObject.Id,
                            created = dataObject.Created,
                            modified = dataObject.Modified,
                            isPublic = dataObject.IsPublic,
                            title = dataObject.Title,
                            parentId = dataObject.ParentId,
                            description = dataObject.Description,
                            tags = dataObject.DataObjectTags.ToList().Select(p => p.Value)
                        });
                    }

                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = results
                    };
                }
                catch
                {
                    return JsonResults.ServerError;
                }
            }

            public static JsonResult Update(Guid key, long id, string json)
            {
                APIDataObject apiDataObject;
                try {
                    apiDataObject = JsonConvert.DeserializeObject<APIDataObject>(json, new JsonSerializerSettings());
                }
                catch  {
                    return JsonResults.BadRequest;
                }

                try {
                    JsonConvert.DeserializeObject(apiDataObject.data);
                }
                catch {
                    return JsonResults.BadRequest;
                }

                DataObject originalObject;
                switch (Repository.Data.Read(key, id, out originalObject))
                {
                    case RepositoryStatusCode.Failed:
                        return JsonResults.ServerError;

                    case RepositoryStatusCode.Unauthorized:
                        return JsonResults.Unauthorized;

                    case RepositoryStatusCode.NotFound:
                        return JsonResults.NotFound;
                }

                originalObject.Modified = DateTime.UtcNow;
                originalObject.Data = apiDataObject.data;
                originalObject.Description = apiDataObject.description;
                originalObject.Title = apiDataObject.title;
                foreach (var tag in originalObject.DataObjectTags.ToList())
                    originalObject.DataObjectTags.Remove(tag);
                
                if (apiDataObject.tags != null)
                {
                    foreach (var tag in apiDataObject.tags)
                    {
                        originalObject.DataObjectTags.Add(new DataObjectTag()
                        {
                            Value = tag
                        });
                    }
                }

                switch (Repository.Data.Update(key, originalObject))
                {
                    case RepositoryStatusCode.OK:
                        return JsonResults.OK;
                        
                    case RepositoryStatusCode.UpgradeRequired:
                        return JsonResults.UpgradeRequired;
                    
                    default:
                        return JsonResults.ServerError;
                }
            }

            public static ActionResult Read(Guid key, long id)
            {
                DataObject dataObject;
                switch (Repository.Data.Read(key, id, out dataObject))
                {
                    case RepositoryStatusCode.OK:
                        return new ContentResult()
                        {
                            Content = dataObject.Data,
                            ContentType = "application/json"
                        };

                    case RepositoryStatusCode.Unauthorized:
                        return JsonResults.Unauthorized;

                    case RepositoryStatusCode.NotFound:
                        return JsonResults.NotFound;
                }

                return JsonResults.ServerError;
            }
        }

        public static class Tool
        {
            public static JsonResult Create(Guid key, string json)
            {
                APIToolObject apiToolObject;
                try {
                    apiToolObject = JsonConvert.DeserializeObject<APIToolObject>(json);
                }
                catch {
                    return JsonResults.BadRequest;
                }

                ToolObject toolObject = new ToolObject()
                {
                    Created = DateTime.UtcNow,
                    Tool = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(apiToolObject.tool, false, false, true, true, 0),
                    IsPublic = apiToolObject.isPublic,
                    Description = apiToolObject.description,
                    Title = apiToolObject.title,
                };
                if (apiToolObject.tags != null)
                {
                    foreach (var tag in apiToolObject.tags)
                    {
                        toolObject.ToolObjectTags.Add(new ToolObjectTag()
                        {
                            Value = tag
                        });
                    }
                }

                switch (Repository.Tool.Create(key, toolObject))
                {
                    case RepositoryStatusCode.OK:
                        return JsonResults.OK;

                    case RepositoryStatusCode.Unauthorized:
                        return JsonResults.Unauthorized;

                    case RepositoryStatusCode.UpgradeRequired:
                        return JsonResults.UpgradeRequired;

                    default:
                        return JsonResults.ServerError;
                }
            }

            public static JsonResult Delete(Guid key, long id)
            {
                try
                {
                    switch (Repository.Tool.Delete(key, id))
                    {
                        case RepositoryStatusCode.OK:
                            return JsonResults.OK;

                        case RepositoryStatusCode.Unauthorized:
                            return JsonResults.Unauthorized;

                        case RepositoryStatusCode.NotFound:
                            return JsonResults.NotFound;

                        default:
                            return JsonResults.ServerError;
                    }
                }
                catch
                {
                    return JsonResults.ServerError;
                }
            }

            public static JsonResult List(Guid key)
            {
                try
                {
                    List<object> results = new List<object>();

                    // Grab all the user's data results
                    foreach (var toolObject in Entities.ToolObjects.Where(p => p.UserProfile.APIKey == key))
                    {
                        results.Add(new
                        {
                            id = toolObject.Id,
                            created = toolObject.Created,
                            modified = toolObject.Modified,
                            isPublic = toolObject.IsPublic,
                            title = toolObject.Title,
                            parentId = toolObject.ParentId,
                            description = toolObject.Description,
                            tags = toolObject.ToolObjectTags.ToList().Select(p => p.Value)
                        });
                    }

                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = results
                    };
                }
                catch
                {
                    return JsonResults.ServerError;
                }
            }

            public static JsonResult Update(Guid key, long id, string json)
            {
                APIToolObject apiToolObject;
                try {
                    apiToolObject = JsonConvert.DeserializeObject<APIToolObject>(json);
                }
                catch {
                    return JsonResults.BadRequest;
                }

                ToolObject originalObject;
                switch (Repository.Tool.Read(key, id, out originalObject))
                {
                    case RepositoryStatusCode.Failed:
                        return JsonResults.ServerError;

                    case RepositoryStatusCode.Unauthorized:
                        return JsonResults.Unauthorized;

                    case RepositoryStatusCode.NotFound:
                        return JsonResults.NotFound;
                }

                originalObject.Modified = DateTime.UtcNow;
                originalObject.Tool = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(apiToolObject.tool, false, false, true, true, 0);
                originalObject.Description = apiToolObject.description;
                originalObject.Title = apiToolObject.title;
                foreach (var tag in originalObject.ToolObjectTags.ToList())
                    originalObject.ToolObjectTags.Remove(tag);

                if (apiToolObject.tags != null)
                {
                    foreach (var tag in apiToolObject.tags)
                    {
                        originalObject.ToolObjectTags.Add(new ToolObjectTag()
                        {
                            Value = tag
                        });
                    }
                }

                switch (Repository.Tool.Update(key, originalObject))
                {
                    case RepositoryStatusCode.OK:
                        return JsonResults.OK;

                    case RepositoryStatusCode.UpgradeRequired:
                        return JsonResults.UpgradeRequired;

                    default:
                        return JsonResults.ServerError;
                }
            }

            public static ActionResult Read(Guid key, long id)
            {
                ToolObject toolObject;
                switch (Repository.Tool.Read(key, id, out toolObject))
                {
                    case RepositoryStatusCode.OK:
                        return new ContentResult()
                        {
                            Content = toolObject.Tool,
                            ContentType = "text/javascript"
                        };

                    case RepositoryStatusCode.Unauthorized:
                        return JsonResults.Unauthorized;

                    case RepositoryStatusCode.NotFound:
                        return JsonResults.NotFound;
                }

                return JsonResults.ServerError;
            }
        }

        public static class API
        {
            public static JsonResult Status()
            {
                List<object> results = new List<object>();

                // Documentation
                results.Add(new { resource = "/API/Documentation/",   verb = "GET", status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Documentation/id", verb = "GET", status = HttpStatusCode.OK });

                // Data
                results.Add(new { resource = "/API/Data/",   verb = "GET",    status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Data/",   verb = "POST",   status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Data/id", verb = "GET",    status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Data/id", verb = "PUT",    status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Data/id", verb = "DELETE", status = HttpStatusCode.OK });

                // Tool
                results.Add(new { resource = "/API/Tool/",   verb = "GET",    status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Tool/",   verb = "POST",   status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Tool/id", verb = "GET",    status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Tool/id", verb = "PUT",    status = HttpStatusCode.OK });
                results.Add(new { resource = "/API/Tool/id", verb = "DELETE", status = HttpStatusCode.OK });

                // VM
                results.Add(new { resource = "/API/VM", verb = "POST", status = HttpStatusCode.OK });

                return new JsonResult() {
                    Data = results,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        public static class User
        {
            public static bool Exists(Guid key)
            {
                UserProfile profile;
                if (Repository.User.Read(key, out profile) == RepositoryStatusCode.OK)
                    return true;
                else
                    return false;
            }
        }

        public static class Session
        {
            static Timer Recycler;
            static Dictionary<Guid, APIProcessor> APIProcessors = new Dictionary<Guid, APIProcessor>();
            static Dictionary<Guid, DateTime> APIProcessorTime = new Dictionary<Guid, DateTime>();
            
            static Session()
            {
                Recycler = new Timer(new TimerCallback(Recycle), null, 5 * 60 * 1000, 5 * 60 * 1000);
            }

            public static Models.Processors.CommandResponse Evaluate(Guid key, string command)
            {
                Guard(key);
                var processor = APIProcessors[key];
                var result = processor.Process(command);
                APIProcessors[key] = processor;
                APIProcessorTime[key] = DateTime.UtcNow;
                return result;
            }

            public static void Reset(Guid key)
            {
                Guard(key);
                APIProcessors[key] = new APIProcessor(key);
            }

            static void Guard(Guid key)
            {
                if (!APIProcessors.ContainsKey(key))
                {
                    APIProcessors.Add(key, new APIProcessor(key));
                    APIProcessorTime.Add(key, DateTime.UtcNow);
                }
            }

            static void Recycle(object obj)
            {
            }
        }
    }

    #region API Models

    public class APIDataObject
    {
        [JsonProperty(Required=Required.Always)]
        public string title { get; set; }

        [JsonProperty]
        public string description { get; set; }

        [JsonProperty]
        public List<string> tags { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string data { get; set; }

        [JsonProperty]
        public bool isPublic { get; set; }
    }

    public class APIToolObject
    {
        [JsonProperty(Required = Required.Always)]
        public string title { get; set; }

        [JsonProperty]
        public string description { get; set; }

        [JsonProperty]
        public List<string> tags { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string tool { get; set; }

        [JsonProperty]
        public bool isPublic { get; set; }
    }

    #endregion
}