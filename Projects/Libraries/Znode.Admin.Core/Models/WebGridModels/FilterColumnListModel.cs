using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Models
{
    public class FilterColumnListModel
    {
        public List<FilterColumnModel> FilterColumnList { get; set; }

        public List<dynamic> GridColumnList { get; set; }

        public List<dynamic> DropdownList { get; set; }

        public string FilterDateFormat { get; } = Helpers.HelperMethods.DatePickDateFormat();

        public bool IsResourceRequired { get; set; }

        public FilterColumnListModel()
        {
            FilterColumnList = new List<FilterColumnModel>();
            DropdownList = new List<dynamic>();
            ToolMenuList = new List<ToolMenuModel>();
        }
        public List<ToolMenuModel> ToolMenuList { get; set; }
        public List<SelectListItem> ExportFileType { get; set; }
    }
}