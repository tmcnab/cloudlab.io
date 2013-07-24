using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cloudlab.Models;
using System.ComponentModel.DataAnnotations;

namespace Cloudlab.ViewModels
{
    public class JSEntityViewModel
    {
        public JSEntityViewModel() {
            this.Id = 0;
        }

        public JSEntityViewModel(JSEntity entity)
        {
            this.Id = entity.Id;
            this.Category = entity.Category;
            this.Entity = entity.Entity;
            this.Name = entity.Name;
            this.Body = entity.Body;
            this.Type = entity.Type;
        }

        public int Id { get; set; }

        public string Category { get; set; }

        public string Entity { get; set; }

        public string Name { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public string Type { get; set; }
    }
}