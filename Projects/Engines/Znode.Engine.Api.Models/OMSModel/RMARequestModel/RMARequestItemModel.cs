namespace Znode.Engine.Api.Models
{
    public class RMARequestItemModel : BaseModel
    {
        public int? OmsOrderLineItemsId { get; set; }
        public int? OMSOrderId { get; set; }
        public string ProductNum { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? Price { get; set; }
        public string SKU { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingCost { get; set; }
        public string PromoDescription { get; set; }
        public string OrderNumber { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? RMAMaxQuantity { get; set; }
        public decimal? RMAQuantity { get; set; }
        public bool IsReturnable { get; set; }
        public bool IsReceived { get; set; }
        public int? RmaReasonForReturnId { get; set; }
        public decimal? TaxCost { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public string ReasonforReturn { get; set; }
        public int? RMARequestId { get; set; }
        public int RmaRequestItemId { get; set; }
        public int UserId { get; set; }
        public int? GCExpirationPeriod { get; set; }
        public decimal? Quantity { get; set; }
        public int? GiftCardId { get; set; }
        public string TransactionId { get; set; }
        public decimal UnitPrice { get; set; }
        public string CurrencyCode { get; set; }       
    }
}
