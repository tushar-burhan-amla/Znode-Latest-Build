namespace Znode.Engine.Admin.ViewModels
{
    public class TaxSummaryViewModel : BaseViewModel
    {
        public int OmsOrderTaxSummaryId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int OmsQuoteTaxSummaryId { get; set; }
        public int? OmsQuoteId { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Rate { get; set; }
        public string TaxName { get; set; }
        public string TaxTypeName { get; set; }
    }
}
