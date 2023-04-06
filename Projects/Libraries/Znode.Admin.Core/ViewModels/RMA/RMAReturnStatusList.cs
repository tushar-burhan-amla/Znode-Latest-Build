using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMAReturnStatusList : BaseViewModel
    {
        public int SelectedItemId { get; set; }
        public string SelectedItemValue { get; set; }
        public List<SelectListItem> ReturnStatusList { get; set; }
    }
}
