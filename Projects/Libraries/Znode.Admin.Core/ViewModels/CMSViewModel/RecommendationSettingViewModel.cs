using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class RecommendationSettingViewModel : BaseViewModel
    {
        public int PortalRecommendationSettingId { get; set; }

        public int PortalId { get; set; }

        public string PortalName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEnableRecommendationOnHome, ResourceType = typeof(Admin_Resources))]
        public bool IsHomeRecommendation { get; set; } = false;

        [Display(Name = ZnodeAdmin_Resources.LabelEnableRecommendationOnPDP, ResourceType = typeof(Admin_Resources))]
        public bool IsPDPRecommendation { get; set; } = false;

        [Display(Name = ZnodeAdmin_Resources.LabelEnableRecommendationOnCart, ResourceType = typeof(Admin_Resources))]
        public bool IsCartRecommendation { get; set; } = false;

        public string TouchPointName { get; set; }

        public int ERPTaskSchedulerId { get; set; }
    }
}
