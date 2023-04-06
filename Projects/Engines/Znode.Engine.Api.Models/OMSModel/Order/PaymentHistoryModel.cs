namespace Znode.Engine.Api.Models
{
    public class PaymentHistoryModel : BaseModel
    {
        public int OrderPaymentId { get; set; }
        public int OmsOrderId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int? OmsNotesId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionStatus { get; set; }
        public string PaymentType { get; set; }
        public decimal RemainingOrderAmount { get; set; }
    }
}
