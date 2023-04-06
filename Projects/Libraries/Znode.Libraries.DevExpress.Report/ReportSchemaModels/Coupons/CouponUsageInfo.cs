using System;

namespace Znode.Libraries.DevExpress.Report
{
    public class CouponUsageInfo : BaseInfo
    {
        public string CouponCode { get; set; }
        public string DiscountType { get; set; }
        public string StoreName { get; set; }
        public int NumberOfUses { get; set; }
        public decimal SalesTotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public string Description { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Symbol { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
    }
}
