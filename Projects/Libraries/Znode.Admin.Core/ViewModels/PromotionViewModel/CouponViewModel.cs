using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CouponViewModel : BaseViewModel
    {
        public int PromotionCouponId { get; set; }

        public int PromotionId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCouponCode, ResourceType = typeof(Admin_Resources))]
        public string Code { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelInitialQuantity, ResourceType = typeof(Admin_Resources))]
        public int? InitialQuantity { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAvailableQuantity, ResourceType = typeof(Admin_Resources))]
        public int? AvailableQuantity { get; set; }

        public bool IsActive { get; set; }
        public string PromotionMessage { get; set; }
        public bool CouponApplied { get; set; }
        public bool CouponValid { get; set; }
        public string CustomCouponCode { get; set; }
        public bool IsCustomCoupon { get; set; }
        public bool IsExistInOrder { get; set; }
    }
}