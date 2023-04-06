using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class PromotionExportViewModel
    {
        public int PromotionId { get; set; }
        public string PromoCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PromotionTypeId { get; set; }
        public string PromotionTypeName { get; set; }
        public decimal? Discount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? OrderMinimum { get; set; }
        public decimal? QuantityMinimum { get; set; }
        public bool IsCouponRequired { get; set; }
        public int DisplayOrder { get; set; } = 99;
        public bool IsUnique { get; set; }
        public decimal? PromotionProductQuantity { get; set; }
        public string StoreName { get; set; }
    }
}