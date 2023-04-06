using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Models
{
    public class ViewModel
    {
        public int ViewId { get; set; }
        public string ViewName { get; set; }
        public string XmlSetting { get; set; }
        public string XmlSettingName { get; set; }

        public bool IsSelected { get; set; }
        public int ApplicationSettingId { get; set; }
        public FilterCollection Filters { get; set; }
        public string SortColumn { get; set; }
        public string SortType { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDefault { get; set; }
        public int CreatedBy { get; set; }
        public int UserId { get; set; }
        public bool HasError { get; set; }
    }
}