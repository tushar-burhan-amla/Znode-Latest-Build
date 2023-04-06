
namespace Znode.Engine.Api.Models
{
    public class PlaceOrderDiscountModel
    {
        public string DiscountCode { get; set; }
        public int? OmsDiscountTypeId { get; set; }
        public decimal? PerQuantityDiscount { get; set; }
        public decimal? DiscountMultiplier { get; set; }
        public int? DiscountLevelTypeId { get; set; }
        public string Sku { get; set; }

        //This property is used for product customization
        public string GroupId { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string PromotionName { get; set; }
        public int? PromotionTypeId { get; set; }
        public string PromotionMessage { get; set; }
        public string Description { get; set; }
        public int? DiscountAppliedSequence { get; set; }
    }
}
