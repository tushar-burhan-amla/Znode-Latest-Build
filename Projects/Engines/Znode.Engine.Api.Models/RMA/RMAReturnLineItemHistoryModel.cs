namespace Znode.Engine.Api.Models
{
    public class RMAReturnLineItemHistoryModel
    {
        public string ReturnedQuantity { get; set; }
        public string ReturnUpdatedStatus { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string IsShippingReturn { get; set; }
        public int RmaReturnLineItemsId { get; set; }
        public string PartialRefundAmount { get; set; }
        public decimal ReturnShippingAmount { get; set; }
        public decimal TaxCost { get; set; }
        public decimal Total { get; set; }
    }
}