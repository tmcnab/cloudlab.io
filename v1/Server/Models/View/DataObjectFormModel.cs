using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.View
{
    public class DataObjectFormModel
    {
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength=1)]
        public string Title { get; set; }

        [StringLength(50)]
        public string Description { get; set; }


        public string Data { get; set; }


        public bool IsPublic { get; set; }

    }
}