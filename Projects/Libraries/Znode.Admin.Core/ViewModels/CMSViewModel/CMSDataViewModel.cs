using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSDataViewModel : BaseViewModel
    {
        public CMSAreaWidgetsDataViewModel CMSAreaWidgetsData { get; set; }
        public List<SelectListItem> CMSAreas { get; set; }
        public int? CMSAreaId { get; set; }
        public int CMSThemeId { get; set; }

    }
}