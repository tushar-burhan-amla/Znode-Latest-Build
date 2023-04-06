using System;

namespace Znode.Engine.Api.Models
{
    public class PriceSKUModel : BaseModel
    {
        public int? PriceId { get; set; }
        public int PriceListId { get; set; }
        public string SKU { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? TierPrice { get; set; }
        public decimal? TierQuantity { get; set; }
        public decimal? RetailPrice { get; set; }
        public int? UomId { get; set; }
        public decimal? UnitSize { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ListName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int PublishProductId { get; set; }
        public int PimProductId { get; set; }
        public string ExternalId { get; set; }
        public string CultureCode { get; set; }
        public int PortalId { get; set; }


    }
}
