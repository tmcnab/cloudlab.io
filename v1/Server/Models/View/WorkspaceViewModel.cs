using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.Models;

namespace Server.Models.View
{
    public class WorkspaceViewModel
    {
        public WorkspaceViewModel(IEnumerable<DataObject> datas, IEnumerable<ToolObject> tools)
        {
            this.DataObjects = datas.ToList();
            this.ToolObjects = tools.ToList();
        }

        public List<DataObject> DataObjects { get; set; }

        public List<ToolObject> ToolObjects { get; set; }

    }
}