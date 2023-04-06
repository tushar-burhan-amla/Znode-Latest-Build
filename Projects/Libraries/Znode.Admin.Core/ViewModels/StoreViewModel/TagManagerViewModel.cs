using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class TagManagerViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public int GoogleTagManagerId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelContainerId, ResourceType = typeof(Admin_Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        public string ContainerId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAnalyticsId, ResourceType = typeof(Admin_Resources))]
        public string AnalyticsUId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAnalyticsIdForAddToCart, ResourceType = typeof(Admin_Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        public string AnalyticsIdForAddToCart { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAnalyticsIdForRemoveFromCart, ResourceType = typeof(Admin_Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        public string AnalyticsIdForRemoveFromCart { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEnabled, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }

        public bool AnalyticsIsActive { get; set; }

        public bool EnableEnhancedEcommerce { get; set; }
    }
}
