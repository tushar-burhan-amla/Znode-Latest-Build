namespace Znode.Engine.Api.Models
{
    public class TradeCentricCartItemModel 
    {
        public decimal Quantity { get; set; }
        public string Supplier_id { get; set; }
        public string Supplier_aux_id { get; set; }
        public string Description { get; set; }
        public string Classification { get; set; }
        public string Uom { get; set; }
        public decimal Unitprice { get; set; }
        public string CurrencyCode { get; set; }
        public TradeCentricAdditionalOptionsModel Options { get; set; }

    }
    public class TradeCentricAdditionalOptionsModel
    {
        public string ParentSupplierId { get; set; }
        public string Message { get; set; }
        public string Gift { get; set; }
    }
}
