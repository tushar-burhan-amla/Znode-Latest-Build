namespace Znode.Engine.Api.Models
{
    public class ConfigurationSettingModel : BaseModel
    {
        public bool IsAllowWithOtherPromotionsAndCoupons { get; set; }
        public bool IsCalculateTaxAfterDiscount { get; set; } 
        public string AllowedPromotions { get; set; }
    }
}
