using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductPromotionViewModel : BaseViewModel
    {
        public int ProductPromotionId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelProductToDiscount, ResourceType = typeof(Admin_Resources))]
        public int? PublishProductId { get; set; }
        public int PromotionId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelRequiredProduct, ResourceType = typeof(Admin_Resources))]
        public int? ReferralPublishProductId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPromotionProdQuantity, ResourceType = typeof(Admin_Resources))]
        public int? PromotionProductQuantity { get; set; }

        public string PromotionType { get; set; }

        public string PromotionMessage { get; set; }
    }
}