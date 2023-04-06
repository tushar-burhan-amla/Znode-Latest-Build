namespace Znode.Engine.Api.Models
{
    public class OrderHistoryModel : BaseModel
    {
        public int OmsHistoryId { get; set; }
        public int OmsOrderId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int? OmsNotesId { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string Notes { get; set; }
        public string UpdatedBy { get; set; }
        public decimal? OrderAmount { get; set; }
        public int OMSQuoteId { get; set; }
    }
}
