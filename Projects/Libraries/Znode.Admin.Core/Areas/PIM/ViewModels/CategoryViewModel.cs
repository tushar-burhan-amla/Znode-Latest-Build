using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public int PimCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Status { get; set; }
        public string CategoryImage { get; set; }
        public int CMSOfferPageCategoryId { get; set; }
        public int CMSContentPagesId { get; set; }
        public int PublishCategoryId { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public string AttributeFamilyName { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int CMSWidgetCategoryId { get; set; }
        public int? PimCatalogId { get; set; }
        public int? PimParentCategoryId { get; set; }
        public int PromotionId { get; set; }
        public string CatalogName { get; set; }
        public string CategoryTitle { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }
        public int LocaleId { get; set; }
        public List<SelectListItem> Locales { get; set; }
        public int PortalId { get; set; }
        public string PublishStatus { get; set; }
        public string SEOCode { get; set; }
        public int DisplayOrder { get; set; } = 999;
        public string CategoryCode { get; set; }
    }
}