namespace Znode.Engine.Api.Models
{
    public class OrderLineItemDataModel : BaseModel
    {
        public int OmsOrderLineItemsId { get; set; }
        public int? OrderLineItemStateId { get; set; }
        public string TrackingNumber { get; set; }
        public string OrderLineItemState { get; set; }
        public int? ParentOmsOrderLineItemsId { get; set; }
        public string Sku { get; set; }
        public decimal? Quantity { get; set; }
        public int OmsOrderId { get; set; }
    }
}