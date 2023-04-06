namespace Znode.Engine.Api.Models
{
    public class FailedOrderTransactionModel : BaseModel
    {
        public int OmsFailedOrderPaymentId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string PaymentDisplayName { get; set; }
        public int PaymentStatusId { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionToken { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
