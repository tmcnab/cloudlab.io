using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloudlab.Models.UI
{
    public class Notification
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public bool Static { get; set; }

        public TimeSpan Interval { get; set; }
    }
}