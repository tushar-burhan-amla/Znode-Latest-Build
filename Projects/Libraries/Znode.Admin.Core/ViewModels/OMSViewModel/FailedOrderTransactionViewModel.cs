namespace Znode.Engine.Admin.ViewModels
{
    public class FailedOrderTransactionViewModel : BaseViewModel
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

        // To display dollar sign in FailedOrderTransactionList.
        public string FailedOrderTotalWithCurrency { get; set; }
    }
}