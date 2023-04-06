using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductDetailsViewModel : BaseViewModel
    {
        public int ProductId { get; set; }
        public string ImagePath { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string AttributeFamily { get; set; }
        public string SKU { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public int PublishProductId { get; set; }
        public string Status { get; set; }
        public string Assortment { get; set; }
        public int? PimAddOnProductDetailId { get; set; }
        public int? PimProductTypeAssociationId { get; set; }
        public int? PimLinkProductDetailId { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }

        public int? DisplayOrder { get; set; } = ZnodeConstant.WidgetItemDisplayOrder;
        public int CMSWidgetProductId { get; set; }
        public int? AddOnDisplayOrder { get; set; }
        public int? PimCategoryId { get; set; }
        public int? PimCatalogId { get; set; }
        public int? PimCategoryHierarchyId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public int? ProfileCatalogCategoryId { get; set; }
        public int? RelatedProductId { get; set; }
        public int? PimAddOnProductId { get; set; }
        public int? PimChildProductId { get; set; }
        public bool? IsDefault { get; set; }
        public List<SelectListItem> IsDefaultList { get; set; }
        public int AddonGroupProductId { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public string PublishStatus { get; set; }
        public List<SelectListItem> Locales { get; set; }
        public string CatalogName { get; set; }
        public string SEOCode { get; set; }
    }
}