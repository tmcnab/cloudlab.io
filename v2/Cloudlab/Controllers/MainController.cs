namespace Cloudlab.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using Cloudlab.Logic;
    using Cloudlab.Models;
    using NBrowserID;

    public class MainController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return View("Splash");
            }
        }

        [HttpPost]
        public ActionResult Authenticate(string assertion)
        {
            var authentication = new BrowserIDAuthentication();
            var verificationResult = authentication.Verify(assertion);
            if (verificationResult.IsVerified)
            {
                string email = verificationResult.Email;
                FormsAuthentication.SetAuthCookie(email, false);

                using (var entities = new Cloudlab.Models.DatabaseEntities())
                {
                    var user = entities.Users.SingleOrDefault(u => u.Email == email);
                    if (user == null)
                    {
                        entities.Users.AddObject(new Models.User()
                        {
                            Email = email,
                            IsContactable = false,
                            ServiceLevel = 0,
                            ServiceExpiration = DateTime.MaxValue,
                            Created = DateTime.UtcNow,
                            LastLogin = DateTime.UtcNow,
                            MasterKey = Guid.NewGuid()
                        });
                    }
                    else {
                        user.LastLogin = DateTime.UtcNow;
                    }
                    entities.SaveChanges();

                    /*user.APIKeys.Add(new APIKey() { Key = Guid.NewGuid(), ReadFlag = false, WriteFlag = false });
                    user.APIKeys.Add(new APIKey() { Key = Guid.NewGuid(), ReadFlag = true,  WriteFlag = false });
                    user.APIKeys.Add(new APIKey() { Key = Guid.NewGuid(), ReadFlag = false, WriteFlag = true  });
                    user.APIKeys.Add(new APIKey() { Key = Guid.NewGuid(), ReadFlag = true,  WriteFlag = true  });*/
                }

                //VMManager.AddUser(email);
                VMManager.AddOrReset(User.Identity.Name);
                return Json(new { email });
            }

            return Json(null);
        }

        public ActionResult Legal()
        {
            return View();
        }

        public ActionResult Exit()
        {
            Cloudlab.Logic.VMManager.Evict(User.Identity.Name);
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        [HttpPost]
        public ActionResult PaymentReturned(string transactionID, string responseCode, string paymentAmount,
                                            string currency, string paymentDate, string @ref, string buyerEmail,
                                            string billingAddress1, string billingAddress2, string billingCity,
                                            string billingState, string billingCountry, string billingPostcode, 
                                            string billingFirstName, string billingSurname)
        {
            
            //Response Code	Definition	Recommended Action
            //PP	Payment processing	Await email notification from Paymate prior to organising delivery of purchased items or service
            //PA	Payment approved	Proceed with organising delivery of items or provision of service immediately
            //PD	Payment declined	Contact buyer to organise another means of payment or discontinue order
            return View("PaymentReturned");
        }
    }
}
