namespace Znode.Engine.WebStore.ViewModels
{
    public class TradeCentricCartItemViewModel 
    {
        public string Primary_id { get; set; }
        public string Secondary_id { get; set; }
        public decimal Quantity { get; set; }
        public object Item_data { get; set; }
        public string Supplier_id { get; set; }
        public string Supplier_aux_id { get; set; }
        public string Description { get; set; }
        public string Classification { get; set; }
        public string Uom { get; set; }
        public decimal Unitprice { get; set; }
        public string CurrencyCode { get; set; }
        public TradeCentricAdditionalOptionsViewModel Options { get; set; }

        public class TradeCentricAdditionalOptionsViewModel
        {
            public string ParentSupplierId { get; set; }
            public string Message { get; set; }
            public string Gift { get; set; }
        }
    }
}
