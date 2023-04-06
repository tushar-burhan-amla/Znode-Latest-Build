using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalPageSettingViewModel : BaseViewModel
    {
        public int PortalPageSettingId { get; set; }
        public int PortalId { get; set; }
        public int PageSettingId { get; set; }
        public string PageName { get; set; }
        public string PageDisplayName { get; set; }
        public string PageValue { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
        public List<SelectListItem> IsDefaultList { get; set; }
    }
}