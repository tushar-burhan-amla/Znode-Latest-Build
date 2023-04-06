using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductPriceSKUDataViewModel : PriceSKUDataViewModel
    {
        public PriceSKUDataViewModel PriceSKUData { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelPriceList, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> PriceList { get; set; }
        public string RequiredAction { get; set; }
    }
}
