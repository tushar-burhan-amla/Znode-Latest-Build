using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreGroupProductModel : ProductInventoryPriceModel
    {
        public int PublishProductId { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public int LocaleId { get; set; }
        public string CurrencySuffix { get; set; }

        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }

        public List<PublishAttributeModel> Attributes { get; set; }

        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public int AssociatedProductDisplayOrder { get; set; }

        public List<WebStoreAddOnModel> Addons { get; set; }
    }
}
