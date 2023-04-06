namespace Znode.Engine.Api.Models
{
    public class RefundPaymentModel : BaseModel
    {
        public int OmsPaymentRefundId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int? OmsOrderLineItemsId { get; set; }
        public int? OmsRefundTypeId { get; set; }
        public string RefundType { get; set; }
        public decimal RefundAmount { get; set; }
        public string Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public string ProductName { get; set; }
        public decimal RefundableAmountLeft { get; set; }
        public string Token { get; set; }
        public bool IsCompleteOrderRefund { get; set; }
        public AddressModel BillingAddress { get; set; }
        public AddressModel ShippingAddress { get; set; }

       
    }
}
