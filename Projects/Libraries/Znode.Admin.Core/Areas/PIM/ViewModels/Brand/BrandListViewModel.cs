using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class BrandListViewModel : BaseViewModel
    {
        public List<BrandViewModel> Brands { get; set; }
        public GridModel GridModel { get; set; }
        public List<ProductDetailsViewModel> ProductDetailList { get; set; }
        public List<StoreDetailsViewModel> StoreDetailList { get; set; }
        public int LocaleId { get; set; }
        public int BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string BrandIds { get; set; }
        public int PromotionId { get; set; }
        public int? DisplayOrder { get; set; } 
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public string DisplayName { get; set; }
        public string WidgetName { get; set; }
        public string FileName { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
    }
}