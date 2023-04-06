using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class PublishProductViewModel : BaseViewModel
    {
        public int PublishProductId { get; set; }
        public int ParentProductId { get; set; }
        public int ZnodeCategoryIds { get; set; }
        public int ZnodeCatalogId { get; set; }
        public int LocaleId { get; set; }

        public string SKU { get; set; }
        public string ConfigurableProductSKU { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }

        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? PromotionalPrice { get; set; }
        public decimal? AllLocationQuantity { get; set; }

        public List<InventorySKUViewModel> Inventory { get; set; }
        public List<AttributesViewModel> Attributes { get; set; }
        public List<ProductReviewViewModel> ProductReviews { get; set; }

        public List<int> ZnodeProductCategoryIds { get; set; }

    }
}