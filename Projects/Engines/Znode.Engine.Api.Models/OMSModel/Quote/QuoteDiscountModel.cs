namespace Znode.Engine.Api.Models
{
    public class QuoteDiscountModel : BaseModel
    {
        public int OmsQuoteOrderDiscountId { get; set; }
        public int? OmsQuoteId { get; set; }
        public int? OmsQuoteLineItemId { get; set; }
        public int? OmsDiscountTypeId { get; set; }
        public string DiscountCode { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string Description { get; set; }
        public decimal? PerQuantityDiscount { get; set; }
        public decimal? DiscountMultiplier { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public int? DiscountLevelTypeId { get; set; }
        public string PromotionName { get; set; }
        public int? PromotionTypeId { get; set; }
        public int? DiscountAppliedSequence { get; set; }
        public string PromotionMessage { get; set; }
    }
}
