using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Models
{
    public class ProductDetailsModel : BaseModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public string ProductType { get; set; }
        public string AttributeFamily { get; set; }
        public string SKU { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Status { get; set; }
        public string Assortment { get; set; }
        public int? PimProductTypeAssociationId { get; set; }
        public int? PimLinkProductDetailId { get; set; }
        public int PublishProductId { get; set; }
        //Addon related properties
        public int? PimAddOnProductDetailId { get; set; }
        public int? RelatedProductId { get; set; }
        public int? localeid { get; set; }
        public int? PimAddonGroupId { get; set; }

        public int? PimCategoryId { get; set; }
        public int? PimCatalogId { get; set; }
        public int? PimCategoryHierarchyId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public int? ProfileCatalogCategoryId { get; set; }
        public int AddonGroupProductId { get; set; }

        public int? DisplayOrder { get; set; } = ZnodeConstant.WidgetItemDisplayOrder;
        public int? AddOnDisplayOrder { get; set; }
        public bool? IsDefault { get; set; }
        public int? PimAddOnProductId { get; set; }
        public int? PimChildProductId { get; set; }
        public string AttributeValue { get; set; }
        public int CMSWidgetProductId { get;set;}
        public string CatalogName { get; set; }
    }
}