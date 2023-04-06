namespace Znode.Engine.Api.Models
{
    public class OrderLineItemHistoryModel
    {
        public string OrderTrackingNumber { get; set; }
        public string OrderLineQuantity { get; set; }
        public string OrderLineUnitPrice { get; set; }
        public string OrderUpdatedStatus { get; set; }
        public string ProductName { get; set; }
        public string OrderLineDelete { get; set; }
        public string OrderLineAdd { get; set; }
        public string Quantity { get; set; }
        public string SKU { get; set; }
        public bool IsShippingReturn { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public string PartialRefundAmount { get; set; }
        public string ReturnShippingAmount { get; set; }
        public decimal TaxCost { get; set; }
        public decimal SubTotal { get; set; }
        public decimal NewOrderLineItemShippingCost { get; set; }
        public bool IsOrderLineItemShippingUpdate { get; set; }
        public decimal OriginalOrderLineItemShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ImportDuty { get; set; }
    }
}