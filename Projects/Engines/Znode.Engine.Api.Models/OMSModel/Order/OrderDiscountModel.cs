namespace Znode.Engine.Api.Models
{
    public class OrderDiscountModel : BaseModel
    {
        public int OmsOrderDiscountId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int? OmsOrderLineItemId { get; set; }
        public int? OmsDiscountTypeId { get; set; }
        public string DiscountCode { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? OriginalDiscount { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; }
        public decimal? PerQuantityDiscount { get; set; }
        public int? ParentOmsOrderLineItemsId { get; set; }
        public decimal? DiscountMultiplier { get; set; }
        //DiscountLevelTypeId is used to saved the type of discount e.g.(OrderLevel, ShippingLevel, LineItemLevel CSRLevel etc)
        public int? DiscountLevelTypeId { get; set; }
        public string PromotionName { get; set; }
        public int? PromotionTypeId { get; set; }
        public int? DiscountAppliedSequence { get; set; }
        public string PromotionMessage { get; set; }

        //This property is used as a product identifier as an alternative for OmsOrderLineItemId
        public string Sku { get; set; }

        //This property is used as an product identifier and is used in product customization
        public string GroupId { get; set; }
    }
}
