using System.Collections.Concurrent;

namespace Znode.Engine.Api.Models
{
    public class AssociatedProductsModel : BaseModel
    {
        public int PublishProductId { get; set; }
        public int ParentPublishProductId { get; set; }
        public int? PimProductId { get; set; }
        public string SKU { get; set; }
        public string OMSColorSwatchText { get; set; }
        public string OMSColorCode { get; set; }
        public string OMSColorValue { get; set; }
        public string OMSColorPath { get; set; }
        public int DisplayOrder { get; set; }

        #region Custom

        public decimal? RetailPrice { get; set; }
        public decimal? SalesPrice { get; set; }

        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }
        public ConcurrentDictionary<string, string> AdditionalAttributes { get; set; }

        #endregion
        
    }
}
