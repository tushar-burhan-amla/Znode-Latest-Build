namespace Znode.Engine.Api.Models
{
    public class OrderPromotionModel : BaseModel
    {
        public int OmsOrderDetailsId { get; set; }
        public int? OmsOrderLineItemId { get; set; }
        public int? ParentOmsOrderLineItemsId { get; set; }
        public string DiscountCode { get; set; }
        public decimal? PerQuantityDiscount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public int OmsDiscountTypeId { get; set; }
        public decimal? DiscountMultiplier { get; set; }
        //DiscountLevelTypeId is used to saved the type of discount e.g.(OrderLevel, ShippingLevel, LineItemLevel CSRLevel etc)
        public int? DiscountLevelTypeId { get; set; }
        public string PromotionName { get; set; }
        public int? PromotionTypeId { get; set; }
        public string Sku { get; set; }
        public int? DiscountAppliedSequence { get; set; }
        public string PromotionMessage { get; set; }
    }
}
