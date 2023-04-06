namespace Znode.Engine.Admin.ViewModels
{
    public class ImportPriceViewModel 
    {
        public string SKU { get; set; }
        public string RetailPrice { get; set; }
        public string SalesPrice { get; set; }
        public string SKUActivationDate { get; set; }
        public string SKUExpirationDate { get; set; }
        public string TierStartQuantity { get; set; }
        public string TierPrice { get; set; }
        public string UOM { get; set; }
        public string UnitSize { get; set; }
        public int SequenceNumber { get; set; }
        public string PriceListCode { get; set; }
        public string PriceListName { get; set; }
        public string CurrencyId { get; set; }
        public string ActivationDate { get; set; }
        public string ExpirationDate { get; set; }
        public string ErrorDescription { get; set; }
    }
}