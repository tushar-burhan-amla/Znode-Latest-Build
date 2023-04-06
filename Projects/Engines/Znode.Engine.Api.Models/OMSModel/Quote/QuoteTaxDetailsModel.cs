namespace Znode.Engine.Api.Models
{
    public class QuoteTaxDetailsModel : BaseModel
    {
        public int OmsQuoteTaxOrderDetailsId { get; set; }
        public int OmsQuoteId { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? VAT { get; set; }
        public decimal? GST { get; set; }
        public decimal? PST { get; set; }
        public decimal? HST { get; set; }
        public decimal? ImportDuty { get; set; }
    }
}
