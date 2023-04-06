namespace Znode.Engine.Api.Models
{
    public class AddonGroupDetailModel:BaseModel
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string AttributeFamily { get; set; }
        public string SKU { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string IsActive { get; set; }
        public int? PimAddOnProductDetailId { get; set; }
        public int? RelatedProductId { get; set; }
        public string Assortment { get; set; }
        public int? localeid { get; set; }
        public int? PimAddonGroupId { get; set; }
        public string AddonGroupName { get; set; }
        public int? DisplayOrder { get; set; }
        public int? AddOnDisplayOrder { get; set; }
        public string DisplayType { get; set; }
        public bool? IsDefault { get; set; }
        public int? PimAddOnProductId { get; set; }
        public string RequiredType { get; set; }
    }
}
