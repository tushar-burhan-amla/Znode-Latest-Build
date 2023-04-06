namespace Znode.Engine.Api.Models
{
    public class TaxOrderLineDetailsModel : BaseModel
    {
        public int OmsTaxOrderLineDetailsId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public decimal SalesTax { get; set; }
        public decimal VAT { get; set; }
        public decimal GST { get; set; }
        public decimal PST { get; set; }
        public decimal HST { get; set; }
        public string TaxTransactionNumber { get; set; }
        public int TaxRuleId { get; set; }
        public decimal ImportDuty { get; set; }
    }
}
