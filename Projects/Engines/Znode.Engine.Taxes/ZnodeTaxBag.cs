using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Taxes
{
    /// Property bag of settings used by the taxes. 
    public class ZnodeTaxBag : ZnodeBusinessBase
    {
        public int TaxClassId { get; set; }
        public decimal SalesTax { get; set; }
        public decimal GST { get; set; }
        public decimal HST { get; set; }
        public decimal PST { get; set; }
        public decimal VAT { get; set; }
        public string DestinationStateCode { get; set; }
        public string DestinationCountryCode { get; set; }
        public string CountyFIPS { get; set; }
        public bool ShippingTaxInd { get; set; }
        public bool InclusiveInd { get; set; }
        public bool? IsDefault { get; set; }
        public string Custom1 { get; set; }
        public string AssociatedTaxRuleIds { get; set; }
        public int TaxRuleId { get; set; }

        public decimal TaxRate
        {
            get { return SalesTax + GST + HST + PST + VAT; }
        }
    }
}
