namespace Znode.Engine.WebStore.ViewModels
{
    public class PaymentHistoryViewModel 
    {
        public int OrderPaymentId { get; set; }
        public int OmsOrderId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionStatus { get; set; }
        public string PaymentType { get; set; }
        public decimal RemainingOrderAmount { get; set; }
        public string OrderAmountWithCurrency { get; set; }
        public string RemainingAmountWithCurrency { get; set; }
        public string OrderDateWithTime { get; set; }
        public string CultureCode { get; set; }
    }
}
