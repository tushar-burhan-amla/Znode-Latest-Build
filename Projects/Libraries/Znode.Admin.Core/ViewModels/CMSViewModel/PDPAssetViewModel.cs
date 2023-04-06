using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class PDPAssetViewModel : BaseViewModel
    {
        public int ProductTypeId { get; set; }
        public int CMSAssetId { get; set; }
        public List<SelectListItem> AssetList { get; set; }
    }
}