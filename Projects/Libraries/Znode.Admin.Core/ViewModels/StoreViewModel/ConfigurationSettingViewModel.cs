
namespace Znode.Engine.Admin.ViewModels
{
    public class ConfigurationSettingViewModel : BaseViewModel
    {
        public bool IsAllowWithOtherPromotionsAndCoupons { get; set; }
        public bool IsCalculateTaxAfterDiscount { get; set; }
        public string AllowedPromotions { get; set; }
    }
}
