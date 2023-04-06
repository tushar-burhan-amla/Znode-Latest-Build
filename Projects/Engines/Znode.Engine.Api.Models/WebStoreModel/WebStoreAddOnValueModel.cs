using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreAddOnValueModel : BaseModel
    {
        public int PublishProductId { get; set; }
        public int LocaleId { get; set; }

        public string SKU { get; set; }
        public string Name { get; set; }

        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }
        public int DisplayOrder { get; set; }
        public string GroupName { get; set; }
        public bool IsDefault { get; set; }
        public List<PublishAttributeModel> Attributes { get; set; }
        public string CultureCode { get; set; }
    }
}
