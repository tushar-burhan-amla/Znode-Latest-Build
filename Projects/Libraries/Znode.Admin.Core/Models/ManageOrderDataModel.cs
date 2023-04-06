namespace Znode.Engine.Admin.Models
{
    public class ManageOrderDataModel
    {
        public int OrderId { get; set; }
        public string Guid { get; set; }
        public decimal Quantity { get; set; }
        public decimal CustomQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal ShippingCost { get; set; }
        public string TrackingNumber { get; set; }
        public int OrderLineItemStatusId { get; set; }
        public string OrderLineItemStatus { get; set; }
        public int ReasonForReturnId { get; set; }
        public string ReasonForReturn { get; set; }
        public int ProductId { get; set; }
        public bool IsShippingReturn { get; set; }
        public decimal? PartialRefundAmount { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public int OmsQuoteId { get; set; }
        public bool IsOrderLineItemShipping { get; set; }
        public decimal OrderLineItemShippingCost { get; set; }
        public decimal OriginalOrderLineItemShippingCost { get; set; }
        public bool IsShippingEdit { get; set; }     
    }
}
