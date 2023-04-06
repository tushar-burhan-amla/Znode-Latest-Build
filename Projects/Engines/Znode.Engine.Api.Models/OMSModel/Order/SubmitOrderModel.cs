namespace Znode.Engine.Api.Models
{
    public class SubmitOrderModel
    {
        #region Property
        public int? OrderId { get; set; }
        public int? OrderStateId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int? PaymentStateId { get; set; }
        public string TrackingNumber { get; set; }
        public string OrderNumber { get; set; }
        public bool IsLineItemReturned { get; set; }
        public decimal? LineItemReturnAmount { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public ReturnOrderLineItemListModel ReturnOrderLineItems { get; set; }
        public string RefundedSkus { get; set; }
        #endregion
    }
}
