using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Server.Models.WePay;
namespace Server.Models
{
    public class Payments
    {
        sealed class WePay
        {
            public const string AccessToken = "8ff070c9e919a4a4727cc0dc09b28cfb7946ab10cc89e7811bdfe36dca43db40";
            public const string ClientName = "cloudlab";
            public const string ClientId = "18376";
            public const string ClientSecret = "6a82f6d2c5";
            public const string BaseUrl = "https://stage.wepayapi.com/v2/";
            public const string AccountId = "36073";
        }

        public bool PreApprove()
        {
            
            return false;
        }


    }
}