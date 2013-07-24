namespace Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Web.Security;
    using Newtonsoft.Json;

    public static class Repository
    {
        private static TauEntities Entities = new TauEntities();

        public static class Data
        {
            public static RepositoryStatusCode Read(string username, long id, out DataObject dataObject)
            {
                dataObject = Entities.DataObjects.SingleOrDefault(d => d.Id == id);
                if (dataObject == null)
                {
                    return RepositoryStatusCode.NotFound;
                }

                if (dataObject.IsPublic || dataObject.UserProfile.User == username)
                {
                    return RepositoryStatusCode.OK;
                }
                else
                {
                    dataObject = null;
                    return RepositoryStatusCode.Unauthorized;
                }
            }

            public static RepositoryStatusCode Read(IPrincipal user, long id, out DataObject dataObject)
            {
                return Data.Read(user.Identity.Name, id, out dataObject);
            }

            public static RepositoryStatusCode Read(Guid key, long id, out DataObject dataObject)
            {
                UserProfile profile;
                if (User.Read(key, out profile) == RepositoryStatusCode.NotFound)
                {
                    dataObject = null;
                    return RepositoryStatusCode.Unauthorized;
                }

                return Data.Read(profile.User, id, out dataObject);
            }

            public static RepositoryStatusCode List(string username, out List<DataObject> dataObjects)
            {
                dataObjects = Entities.UserProfiles.SingleOrDefault(u => u.User == username)
                                      .DataObjects.ToList();
                return RepositoryStatusCode.OK;
            }

            public static RepositoryStatusCode List(IPrincipal user, out List<DataObject> dataObjects)
            {
                return Data.List(user.Identity.Name, out dataObjects);
            }

            public static RepositoryStatusCode List(Guid key, out List<DataObject> dataObjects)
            {
                UserProfile profile;
                if (User.Read(key, out profile) == RepositoryStatusCode.NotFound)
                {
                    dataObjects = null;
                    return RepositoryStatusCode.Unauthorized;
                }

                return Data.List(profile.User, out dataObjects);
            }

            public static RepositoryStatusCode Create(string username, DataObject dataObject)
            {
                if (!QuotaCheck(username, dataObject))
                    return RepositoryStatusCode.UpgradeRequired;

                // Insert the user profile id
                UserProfile profile;
                if (User.Read(username, out profile) == RepositoryStatusCode.Failed)
                    return RepositoryStatusCode.Failed;
                else
                    dataObject.UserProfileId = profile.Id;

                if (!Data.IsValidJSON(dataObject.Data))
                    return RepositoryStatusCode.BadInput;

                try
                {
                    Entities.DataObjects.AddObject(dataObject);
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode Create(IPrincipal user, DataObject dataObject)
            {
                return Data.Create(user.Identity.Name, dataObject);
            }

            public static RepositoryStatusCode Create(Guid key, DataObject dataObject)
            {
                UserProfile profile;
                if (User.Read(key, out profile) == RepositoryStatusCode.NotFound)
                    return RepositoryStatusCode.Unauthorized;

                return Data.Create(profile.User, dataObject);
            }

            public static RepositoryStatusCode Fork(string username, long id)
            {
                // Fetch & Check
                DataObject ancestorObject;
                switch (Data.Read(username, id, out ancestorObject))
                {
                    case RepositoryStatusCode.NotFound:
                        return RepositoryStatusCode.NotFound;
                    case RepositoryStatusCode.Unauthorized:
                        return RepositoryStatusCode.Unauthorized;
                }

                if (!Data.QuotaCheck(username, ancestorObject))
                    return RepositoryStatusCode.UpgradeRequired;

                var childObject = new DataObject()
                {
                    Created = DateTime.UtcNow,
                    Title = ancestorObject.Title,
                    IsPublic = ancestorObject.IsPublic,
                    ParentId = ancestorObject.Id,
                    UserProfileId = Entities.UserProfiles.Single(u => u.User == username).Id,
                    Description = ancestorObject.Description,
                    Data = ancestorObject.Data
                };

                foreach (var item in ancestorObject.DataObjectTags.ToList())
                    childObject.DataObjectTags.Add(new DataObjectTag() {
                        Value = item.Value
                    });

                try
                {
                    Entities.DataObjects.AddObject(childObject);
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode Fork(IPrincipal user, long id)
            {
                return Data.Fork(user.Identity.Name, id);
            }

            public static RepositoryStatusCode Update(string username, DataObject dataObject)
            {
                if (!Data.IsValidJSON(dataObject.Data))
                    return RepositoryStatusCode.BadInput;

                DataObject originalObject = Entities.UserProfiles.SingleOrDefault(u => u.User == username)
                                                    .DataObjects.SingleOrDefault(d => d.Id == dataObject.Id);
                if (originalObject == null) 
                    return RepositoryStatusCode.NotFound;

                originalObject.IsPublic = dataObject.IsPublic;
                originalObject.Description = dataObject.Description;
                originalObject.Title = dataObject.Title;
                originalObject.Data = dataObject.Data;
                originalObject.Modified = DateTime.UtcNow;

                try
                {
                    foreach (var tag in originalObject.DataObjectTags.ToList())
                        originalObject.DataObjectTags.Remove(tag);

                    foreach (var tag in dataObject.DataObjectTags.ToList())
                        originalObject.DataObjectTags.Add(tag);

                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode Update(IPrincipal user, DataObject dataObject)
            {
                return Data.Update(user.Identity.Name, dataObject);
            }

            public static RepositoryStatusCode Update(Guid key, DataObject dataObject)
            {
                UserProfile profile;
                if (User.Read(key, out profile) == RepositoryStatusCode.NotFound)
                    return RepositoryStatusCode.Unauthorized;

                return Data.Update(profile.User, dataObject);
            }

            public static RepositoryStatusCode Delete(string username, long id)
            {
                DataObject dataObject = Entities.UserProfiles.SingleOrDefault(u => u.User == username)
                                                .DataObjects.SingleOrDefault(d => d.Id == id);
                if (dataObject == null)
                {
                    return RepositoryStatusCode.NotFound;
                }
                else
                {
                    try
                    {
                        foreach (var tag in dataObject.DataObjectTags.ToList())
                            Entities.DataObjectTags.DeleteObject(tag);

                        Entities.DataObjects.DeleteObject(dataObject);
                        Entities.SaveChanges();
                        return RepositoryStatusCode.OK;
                    }
                    catch
                    {
                        return RepositoryStatusCode.Failed;
                    }
                }
            }

            public static RepositoryStatusCode Delete(IPrincipal user, long id)
            {
                return Data.Delete(user.Identity.Name, id);
            }

            public static RepositoryStatusCode Delete(Guid key, long id)
            {
                UserProfile profile;
                if (User.Read(key, out profile) == RepositoryStatusCode.NotFound)
                    return RepositoryStatusCode.Unauthorized;

                return Data.Delete(profile.User, id);
            }

            // Stubbed
            private static bool QuotaCheck(string username, DataObject dataObject)
            {
                return true;
            }

            private static bool IsValidJSON(string json)
            {
                try {
                    var x = JsonConvert.DeserializeObject(json);
                    return true;
                }
                catch {
                    return false;
                }
            }
        }

        public static class Documentation
        {
            public static RepositoryStatusCode CreateOrUpdate(DocumentationItem item)
            {
                if (Entities.DocumentationItems.SingleOrDefault(d => d.Id == item.Id) == null)
                    return Create(item);
                else
                    return Update(item);
            }

            public static RepositoryStatusCode Create(DocumentationItem item)
            {
                try
                {
                    Entities.DocumentationItems.AddObject(item);
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }
            
            public static RepositoryStatusCode List(out List<DocumentationItem> items)
            {
                items = Entities.DocumentationItems.OrderBy(d => d.Category).ToList();
                return items == null ? RepositoryStatusCode.NotFound : RepositoryStatusCode.OK;
            }

            public static RepositoryStatusCode Read(int id, out DocumentationItem item)
            {
                item = Entities.DocumentationItems.SingleOrDefault(d => d.Id == id);
                return item == null ? RepositoryStatusCode.NotFound : RepositoryStatusCode.OK;
            }

            public static RepositoryStatusCode Read(string title, out DocumentationItem item)
            {
                item = Entities.DocumentationItems.SingleOrDefault(d => d.Title == title);
                return item == null ? RepositoryStatusCode.NotFound : RepositoryStatusCode.OK;
            }

            public static RepositoryStatusCode Update(DocumentationItem item)
            {
                var docItem = Entities.DocumentationItems.SingleOrDefault(d => d.Id == item.Id);
                if (docItem != null)
                {
                    docItem.Title = item.Title;
                    docItem.Body = item.Body;
                    docItem.Category = item.Category;
                    try
                    {
                        Entities.SaveChanges();
                        return RepositoryStatusCode.OK;
                    }
                    catch
                    {
                        return RepositoryStatusCode.Failed;
                    }
                }
                else
                {
                    return RepositoryStatusCode.Failed;
                }
            }
        }

        public static class Search
        {
            public static RepositoryStatusCode Tools(string username, string queryString, out List<ToolObject> results)
            {
                // Split the query string into components and buckets
                var tokens = queryString.ToLowerInvariant().Split(' ');
                List<string> titleWords = new List<string>();
                List<string> tagWords = new List<string>();
                foreach (var token in tokens)
                {
                    if (token.StartsWith("[") && token.EndsWith("]"))
                        tagWords.Add(token.Replace("[", "").Replace("]", ""));
                    else
                        titleWords.Add(token);
                }

                // Start with all publicly-available data (including user's private data) and filter with criterion
                UserProfile userProfile;
                if (Repository.User.Read(username, out userProfile) != RepositoryStatusCode.OK)
                    throw new NotImplementedException();

                var toolObjects = from t in Entities.ToolObjects
                                  where (t.IsPublic || t.UserProfileId == userProfile.Id)
                                  orderby t.Created descending
                                  select t;

                results = toolObjects.ToList();
                return RepositoryStatusCode.OK;
            }

            public static RepositoryStatusCode Data(string username, string queryString, out List<DataObject> results)
            {
                // Split the query string into components and buckets
                var tokens = queryString.ToLowerInvariant().Split(' ');
                List<string> titleWords = new List<string>();
                List<string> tagWords = new List<string>();
                foreach (var token in tokens)
                {
                    if (token.StartsWith("[") && token.EndsWith("]"))
                        tagWords.Add(token.Replace("[", "").Replace("]", ""));
                    else
                        titleWords.Add(token);
                }

                // Start with all publicly-available data (including user's private data) and filter with criterion
                UserProfile userProfile;
                if (Repository.User.Read(username, out userProfile) != RepositoryStatusCode.OK)
                    throw new NotImplementedException();

                var dataObjects = from d in Entities.DataObjects
                                  where (d.IsPublic || d.UserProfileId == userProfile.Id)
                                  orderby d.Created descending
                                  select d;

                results = dataObjects.ToList();
                return RepositoryStatusCode.OK;
            }

            public static RepositoryStatusCode Tools(IPrincipal user, string queryString, out List<ToolObject> results)
            {
                return Tools(user.Identity.Name, queryString, out results);
            }

            public static RepositoryStatusCode Data(IPrincipal user, string queryString, out List<DataObject> results)
            {
                return Data(user.Identity.Name, queryString, out results);
            }

            public static RepositoryStatusCode All(string username, string queryString, out List<Ajax.AjaxSearchResult> results)
            {
                // Split the query string into components and buckets
                var tokens = queryString.ToLowerInvariant().Split(' ');
                List<string> titleWords = new List<string>();
                List<string> tagWords = new List<string>();
                foreach (var token in tokens)
                {
                    if (token.StartsWith("[") && token.EndsWith("]"))
                    {
                        tagWords.Add(token.Replace("[", "").Replace("]", ""));
                    }
                    else
                    {
                        titleWords.Add(token);
                    }
                }

                // Start with all publicly-available data (including user's private data) and filter with criterion
                UserProfile userProfile;
                if (Repository.User.Read(username, out userProfile) != RepositoryStatusCode.OK)
                    throw new NotImplementedException();

                var dataObjects = from d in Entities.DataObjects
                                  where (d.IsPublic || d.UserProfileId == userProfile.Id)
                                  orderby d.Created descending
                                  select d;


                var toolObjects = from t in Entities.ToolObjects
                                  where (t.IsPublic || t.UserProfileId == userProfile.Id)
                                  orderby t.Created descending
                                  select t;

                results = new List<Ajax.AjaxSearchResult>();
                foreach (var dataResult in dataObjects)
                {
                    if (ApplyCriterion(dataResult, titleWords, tagWords))
                    {
                        results.Add(new Ajax.AjaxSearchResult(dataResult));
                    }
                }
                foreach (var toolResult in toolObjects)
                {
                    if (ApplyCriterion(toolResult, titleWords, tagWords))
                    {
                        results.Add(new Ajax.AjaxSearchResult(toolResult));
                    }
                }

                return RepositoryStatusCode.OK;
            }

            private static bool ApplyCriterion(DataObject dataObject, List<string> titleWords, List<string> tagWords)
            {
                // Tag-first search:
                //  create a list of tag values from the data object
                List<string> dataTagWords = new List<string>();
                foreach (var tag in dataObject.DataObjectTags)
                {
                    dataTagWords.Add(tag.Value);
                }

                // if the intersection is false, it does not have all of the tag words as tags
                if (dataTagWords.Intersect(tagWords).Count() != tagWords.Count())
                {
                    return false;
                }


                // Lowercase, Split and convert the title of the data object to a list
                List<string> dataTitleWords = new List<string>();
                foreach (var tag in dataObject.Title.ToLowerInvariant().Split(' '))
                {
                    dataTitleWords.Add(tag);
                }

                // if the intersection is true, all of the search words are in the title of the data object
                return dataTitleWords.Intersect(titleWords).Count() == titleWords.Count();
            }

            private static bool ApplyCriterion(ToolObject toolObject, List<string> titleWords, List<string> tagWords)
            {
                // Tag-first search:
                //  create a list of tag values from the data object
                List<string> toolTagWords = new List<string>();
                foreach (var tag in toolObject.ToolObjectTags)
                {
                    toolTagWords.Add(tag.Value);
                }

                // if the intersection is false, it does not have all of the tag words as tags
                if (toolTagWords.Intersect(tagWords).Count() != tagWords.Count())
                {
                    return false;
                }


                // Lowercase, Split and convert the title of the data object to a list
                List<string> toolTitleWords = new List<string>();
                foreach (var tag in toolObject.Title.ToLowerInvariant().Split(' '))
                {
                    toolTitleWords.Add(tag);
                }

                // if the intersection is true, all of the search words are in the title of the data object
                return toolTitleWords.Intersect(titleWords).Count() == titleWords.Count();
            }
        }

        public static class Support
        {
            public static RepositoryStatusCode Create(string username, string title, string problem)
            {
                UserProfile userProfile;
                if (Repository.User.Read(username, out userProfile) != RepositoryStatusCode.OK)
                    return RepositoryStatusCode.NotFound;

                SupportRequest request = new SupportRequest()
                {
                    Problem = problem,
                    Solution = string.Empty,
                    Created = DateTime.UtcNow,
                    Title = title,
                    UserProfileId = userProfile.Id,
                    WeKnowAboutIt = false,
                    WeSolvedIt = false
                };
                try
                {
                    Entities.SupportRequests.AddObject(request);
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode List(string username, out List<SupportRequest> requestItems)
            {
                requestItems = Entities.UserProfiles.SingleOrDefault(u => u.User == username)
                                       .SupportRequests.ToList();
                return requestItems == null ? RepositoryStatusCode.Failed : RepositoryStatusCode.OK;
            }
        }

        public static class Tool
        {
            public static RepositoryStatusCode Read(string username, long id, out ToolObject toolObject)
            {
                toolObject = Entities.ToolObjects.SingleOrDefault(d => d.Id == id);
                if (toolObject == null)
                {
                    return RepositoryStatusCode.NotFound;
                }

                if (toolObject.IsPublic || toolObject.UserProfile.User == username)
                {
                    return RepositoryStatusCode.OK;
                }
                else
                {
                    toolObject = null;
                    return RepositoryStatusCode.Unauthorized;
                }
            }

            public static RepositoryStatusCode Read(IPrincipal user, long id, out ToolObject toolObject)
            {
                return Read(user.Identity.Name, id, out toolObject);
            }

            public static RepositoryStatusCode Read(Guid key, long id, out ToolObject toolObject)
            {
                UserProfile profile;
                if (User.Read(key, out profile) == RepositoryStatusCode.NotFound)
                {
                    toolObject = null;
                    return RepositoryStatusCode.Unauthorized;
                }

                return Read(profile.User, id, out toolObject);
            }

            public static RepositoryStatusCode List(string username, out List<ToolObject> toolObjects)
            {
                toolObjects = Entities.UserProfiles.SingleOrDefault(u => u.User == username)
                                      .ToolObjects.ToList();
                return RepositoryStatusCode.OK;
            }

            public static RepositoryStatusCode List(IPrincipal user, out List<ToolObject> toolObjects)
            {
                return List(user.Identity.Name, out toolObjects);
            }

            public static RepositoryStatusCode List(Guid key, out List<ToolObject> toolObjects)
            {
                UserProfile profile;
                if (User.Read(key, out profile) == RepositoryStatusCode.NotFound)
                {
                    toolObjects = null;
                    return RepositoryStatusCode.Unauthorized;
                }

                return List(profile.User, out toolObjects);
            }

            public static RepositoryStatusCode Create(string username, ToolObject toolObject)
            {
                if (!QuotaCheck(username, toolObject))
                    return RepositoryStatusCode.UpgradeRequired;

                // Insert the user profile id
                UserProfile profile;
                if (User.Read(username, out profile) == RepositoryStatusCode.Failed)
                    return RepositoryStatusCode.Failed;
                else
                    toolObject.UserProfileId = profile.Id;

                // !!! NEED TO INSERT A JSON VALIDATION THINGY HERE

                try
                {
                    Entities.ToolObjects.AddObject(toolObject);
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode Create(IPrincipal user, ToolObject toolObject)
            {
                return Create(user.Identity.Name, toolObject);
            }

            public static RepositoryStatusCode Create(Guid key, ToolObject toolObject)
            {
                return Create(Entities.UserProfiles.SingleOrDefault(u => u.APIKey == key).User, toolObject);
            }

            public static RepositoryStatusCode Fork(string username, long id)
            {
                // Fetch & Check
                ToolObject ancestorObject;
                switch (Read(username, id, out ancestorObject))
                {
                    case RepositoryStatusCode.NotFound:
                        return RepositoryStatusCode.NotFound;
                    case RepositoryStatusCode.Unauthorized:
                        return RepositoryStatusCode.Unauthorized;
                }

                if (!QuotaCheck(username, ancestorObject))
                    return RepositoryStatusCode.UpgradeRequired;

                var childObject = new ToolObject()
                {
                    Created = DateTime.UtcNow,
                    Title = ancestorObject.Title,
                    IsPublic = ancestorObject.IsPublic,
                    ParentId = ancestorObject.Id,
                    UserProfileId = Entities.UserProfiles.Single(u => u.User == username).Id,
                    Description = ancestorObject.Description,
                    Tool = ancestorObject.Tool
                };

                foreach (var item in ancestorObject.ToolObjectTags.ToList())
                    childObject.ToolObjectTags.Add(new ToolObjectTag()
                    {
                        Value = item.Value
                    });

                try
                {
                    Entities.ToolObjects.AddObject(childObject);
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode Fork(IPrincipal user, long id)
            {
                return Fork(user.Identity.Name, id);
            }

            public static RepositoryStatusCode Update(string username, ToolObject toolObject)
            {
                ToolObject originalObject = Entities.UserProfiles.SingleOrDefault(u => u.User == username)
                                                    .ToolObjects.SingleOrDefault(d => d.Id == toolObject.Id);
                if (originalObject == null)
                    return RepositoryStatusCode.NotFound;

                originalObject.IsPublic = toolObject.IsPublic;
                originalObject.Description = toolObject.Description;
                originalObject.Title = toolObject.Title;
                originalObject.Tool = toolObject.Tool;
                originalObject.Modified = DateTime.UtcNow;

                try
                {
                    foreach (var tag in originalObject.ToolObjectTags.ToList())
                        originalObject.ToolObjectTags.Remove(tag);

                    foreach (var tag in toolObject.ToolObjectTags.ToList())
                        originalObject.ToolObjectTags.Add(tag);

                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode Update(IPrincipal user, ToolObject toolObject)
            {
                return Update(user.Identity.Name, toolObject);
            }

            public static RepositoryStatusCode Update(Guid key, ToolObject toolObject)
            {
                return Tool.Update(Entities.UserProfiles.SingleOrDefault(u => u.APIKey == key).User, toolObject);
            }

            public static RepositoryStatusCode Delete(string username, long id)
            {
                ToolObject toolObject = Entities.UserProfiles.SingleOrDefault(u => u.User == username)
                                                .ToolObjects.SingleOrDefault(d => d.Id == id);
                if (toolObject == null)
                {
                    return RepositoryStatusCode.NotFound;
                }
                else
                {
                    try
                    {
                        foreach (var tag in toolObject.ToolObjectTags.ToList())
                            Entities.ToolObjectTags.DeleteObject(tag);

                        Entities.ToolObjects.DeleteObject(toolObject);
                        Entities.SaveChanges();
                        return RepositoryStatusCode.OK;
                    }
                    catch
                    {
                        return RepositoryStatusCode.Failed;
                    }
                }
            }

            public static RepositoryStatusCode Delete(IPrincipal user, long id)
            {
                return Delete(user.Identity.Name, id);
            }

            public static RepositoryStatusCode Delete(Guid key, long id)
            {
                return Delete(Entities.UserProfiles.SingleOrDefault(u => u.APIKey == key).User, id);
            }

            private static bool QuotaCheck(string username, ToolObject toolObject)
            {
                return true;
            }
        }

        public static class User
        {
            public static RepositoryStatusCode Create(UserProfile userProfile)
            {
                try
                {
                    Entities.UserProfiles.AddObject(userProfile);
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
                    
            }

            public static RepositoryStatusCode Read(string username, out UserProfile userProfile)
            {
                userProfile = Entities.UserProfiles.SingleOrDefault(u => u.User == username);
                if (userProfile == null)
                {
                    return RepositoryStatusCode.NotFound;
                }
                else
                {
                    return RepositoryStatusCode.OK;
                }
            }

            public static RepositoryStatusCode Read(IPrincipal user, out UserProfile userProfile)
            {
                return User.Read(user.Identity.Name, out userProfile);
            }

            public static RepositoryStatusCode Read(Guid key, out UserProfile userProfile)
            {
                userProfile = Entities.UserProfiles.SingleOrDefault(u => u.APIKey == key);
                if (userProfile == null)
                    return RepositoryStatusCode.NotFound;
                else
                    return RepositoryStatusCode.OK;
            }

            public static RepositoryStatusCode Update(string username, UserProfile userProfile)
            {
                var profile = Entities.UserProfiles.SingleOrDefault(u => u.User == username);
                if (profile == null)
                    return RepositoryStatusCode.NotFound;

                profile.AllowContact = userProfile.AllowContact;
                profile.FirstName = userProfile.FirstName;
                profile.LastName = userProfile.LastName;
                profile.UseGravatar = userProfile.UseGravatar;

                try
                {
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode Update(IPrincipal user, UserProfile userProfile)
            {
                return Update(user.Identity.Name, userProfile);
            }

            public static RepositoryStatusCode RevokeAPIKey(string username, out Guid newkey)
            {
                var userProfile = Entities.UserProfiles.SingleOrDefault(u => u.User == username);
                if (userProfile == null)
                {
                    newkey = Guid.Empty;
                    return RepositoryStatusCode.NotFound;
                }

                newkey = Guid.NewGuid();
                
                try
                {
                    userProfile.APIKey = newkey;
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    newkey = Guid.Empty;
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode RevokeAPIKey(IPrincipal user, out Guid newkey)
            {
                return RevokeAPIKey(user.Identity.Name, out newkey);
            }
        }

        public static class Violations
        {
            public static RepositoryStatusCode Create(string username, long dataId = 0, long toolId = 0)
            {
                if(dataId == 0 && toolId == 0)
                    return RepositoryStatusCode.Failed;

                UserProfile userProfile;
                if(User.Read(username, out userProfile) != RepositoryStatusCode.OK)
                    return RepositoryStatusCode.Failed;

                try
                {
                    Entities.ViolationReportItems.AddObject(new ViolationReportItem()
                    {
                        ToolId = toolId,
                        DataId = dataId,
                        UserProfileId = userProfile.Id
                    });
                    Entities.SaveChanges();
                    return RepositoryStatusCode.OK;
                }
                catch
                {
                    return RepositoryStatusCode.Failed;
                }
            }

            public static RepositoryStatusCode Create(IPrincipal user, long dataId = 0, long toolId = 0)
            {
                return Create(user, dataId, toolId);
            }
        }

        public static class Quotas
        {
            public static double Data(string username)
            {
                UserProfile userProfile;
                if (Repository.User.Read(username, out userProfile) != RepositoryStatusCode.OK)
                    return double.NaN;

                List<DataObject> dataItems;
                Repository.Data.List(username, out dataItems);

                long count = 0;
                foreach (var item in dataItems)
                {
                    count += item.Data.LongCount();
                }

                switch (userProfile.Coverage)
                {
                    case 1: return (count / (0.25 * 1048576)) * 100;
                    case 2: return (count / (  10 * 1048576)) * 100;
                    case 3: return (count / ( 100 * 1048576)) * 100;
                    default: return 0;
                }
            }

            public static double Data(IPrincipal user)
            {
                return Data(user.Identity.Name);
            }

            public static double Tools(string username)
            {
                UserProfile userProfile;
                if (Repository.User.Read(username, out userProfile) != RepositoryStatusCode.OK)
                    return double.NaN;

                List<DataObject> toolItems;
                Repository.Data.List(username, out toolItems);

                long count = 0;
                foreach (var item in toolItems)
                {
                    count += item.Data.LongCount();
                }

                switch (userProfile.Coverage)
                {
                    case 1: return (count / (0.25 * 1048576)) * 100;
                    case 2: return (count / (10 * 1048576)) * 100;
                    case 3: return (count / (100 * 1048576)) * 100;
                    default: return 0;
                }
            }

            public static double Tools(IPrincipal user)
            {
                return Tools(user.Identity.Name);
            }
        }

        //---------------------------//

        public static class Administration
        {
            public static List<SupportRequest> SupportRequests(bool includeClosedItems = false)
            {
                if (includeClosedItems)
                    return Entities.SupportRequests.ToList();
                else
                    return Entities.SupportRequests.Where(t => t.WeSolvedIt == false).ToList();
            }

            public static List<UserProfile> Users()
            {
                return Entities.UserProfiles.OrderBy(u => u.User).ToList();
            }

            public static List<ViolationReportItem> Violations()
            {
                return Entities.ViolationReportItems.ToList();
            }
        }

        public static class Optima
        {
            public static object SupportTicketGet(IPrincipal user)
            {
                return from t in Entities.UserProfiles.SingleOrDefault(u => u.User == user.Identity.Name)
                                         .SupportRequests
                       select new { t.Title, t.Id, t.WeSolvedIt };
            }

            public static object SupportTicketGet(IPrincipal user, int id)
            {

                return (from t in Entities.UserProfiles.SingleOrDefault(u => u.User == user.Identity.Name)
                                          .SupportRequests
                        where t.Id == id
                        select new { title = t.Title, problem = t.Problem, solution = t.Solution, solved = t.WeSolvedIt }).FirstOrDefault();
            }

            public static bool AjaxAddSupportTicket(IPrincipal user, Ajax.AjaxSupportTicketFormModel model)
            {
                try
                {
                    UserProfile userProfile;
                    if (Repository.User.Read(user, out userProfile) != RepositoryStatusCode.OK)
                        throw new NotImplementedException();

                    Entities.SupportRequests.AddObject(new SupportRequest()
                    {
                        Created = DateTime.UtcNow,
                        Title = model.Title,
                        Problem = model.Description,
                        UserProfileId = userProfile.Id,
                        Solution = string.Empty,
                        WeKnowAboutIt = false,
                        WeSolvedIt = false
                    });
                    Entities.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool AjaxProfileUpdate(IPrincipal user, Ajax.AjaxCredentialsModel model)
            {
                // Get Profile and Guard
                UserProfile userProfile;
                if (Repository.User.Read(user, out userProfile) != RepositoryStatusCode.OK)
                    throw new NotImplementedException();

                if (userProfile == null) return false;

                try
                {
                    userProfile.FirstName = model.FirstName;
                    userProfile.LastName = model.LastName;
                    userProfile.UseGravatar = model.UseGravatar;
                    userProfile.AllowContact = model.AllowContact;
                    Entities.SaveChanges();

                    var mUser = Membership.GetUser(user.Identity.Name, true);
                    mUser.Email = model.Email;
                    Membership.UpdateUser(mUser);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool AjaxRevokeAPIKey(IPrincipal user, out string key)
            {
                UserProfile userProfile;
                if (Repository.User.Read(user, out userProfile) != RepositoryStatusCode.OK)
                    throw new NotImplementedException();

                try
                {
                    userProfile.APIKey = Guid.NewGuid();
                    Entities.SaveChanges();
                    key = userProfile.APIKey.ToString();
                    return true;
                }
                catch
                {
                    key = null;
                    return false;
                }
            }

            public static object AjaxDocumentationGet(string command)
            {
                var toks = command.Split('.');
                if (toks.Length != 2) return null;

                var obj = toks[0];
                var name = toks[1];

                var p = (from t in Entities.DocumentationItems
                         where t.Object.Contains(obj) &&
                               t.Name.Contains(name)
                         select t).ToList().FirstOrDefault();
                
                if (p != null)
                    return new { title = string.Format("{0}.{1}",p.Object, p.Name) , body = p.Body };
                else
                    return null;
            }

            public static bool AjaxDeleteTool(IPrincipal user, long id)
            {
                try
                {
                    return Repository.Tool.Delete(user, id) == RepositoryStatusCode.OK;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}