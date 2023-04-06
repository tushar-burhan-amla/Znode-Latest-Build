namespace Znode.Engine.Api.Models
{
    public class RMAReturnCalculateLineItemModel : BaseModel
    {
        public int OrderId { get; set; }
        public string Guid { get; set; }
        public decimal ShippedQuantity { get; set; }
        public decimal ExpectedReturnQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalLineItemPrice { get; set; }
        public int ReasonForReturnId { get; set; }
        public string ReasonForReturn { get; set; }
        public int ProductId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public decimal TaxCost { get; set; }
        public string CultureCode { get; set; }
        public bool IsShippingReturn { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? ReturnedQuantity { get; set; }
        public int RmaReturnLineItemsId { get; set; }
        public decimal PerQuantityLineItemDiscount { get; set; }
        public decimal PerQuantityCSRDiscount { get; set; }
        public decimal PerQuantityShippingDiscount { get; set; }
        public decimal PerQuantityShippingCost { get; set; }
        public decimal PerQuantityOrderLevelDiscountOnLineItem { get; set; }
        public int? PaymentStatusId { get; set; }
        public decimal PerQuantityVoucherAmount { get; set; }
        public int? ParentOmsOrderLineItemsId { get; set; }
        public decimal ImportDuty { get; set; }
    }
}
        