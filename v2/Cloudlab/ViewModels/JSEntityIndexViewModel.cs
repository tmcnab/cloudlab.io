using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cloudlab.Models;

namespace Cloudlab.ViewModels
{
    public class JSEntityIndexViewModel
    {
        public Dictionary<string, Dictionary<string,List<object>>> Model;

        public JSEntityIndexViewModel(List<JSEntity> index)
        {
            Model = new Dictionary<string, Dictionary<string, List<object>>>();

            foreach (var item in index)
            {
                // Guard for Category
                if (!Model.ContainsKey(item.Category)) {
                    Model.Add(item.Category, new Dictionary<string,List<object>>());
                }

                // Guard for Entity
                if (!Model[item.Category].ContainsKey(item.Entity)) {
                    Model[item.Category].Add(item.Entity, new List<object>());
                }

                Model[item.Category][item.Entity].Add(new {
                    id = item.Id,
                    name = item.Name,
                    type = item.Type
                });
            }
        }
    }
}