using System;

namespace Znode.Engine.Api.Models
{
    public class ExportPriceModel
    {
        public string SKU { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? SalesPrice { get; set; }
        public DateTime? SKUActivationDate { get; set; }
        public DateTime? SKUExpirationDate { get; set; }
        public decimal? TierStartQuantity { get; set; }
        public decimal? TierPrice { get; set; }
        public int PriceListId { get; set; }
        public string PriceListCode { get; set; }
    }
}
