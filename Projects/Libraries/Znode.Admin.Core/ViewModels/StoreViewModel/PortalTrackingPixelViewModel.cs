using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalTrackingPixelViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public int PortalPixelTrackingId { get; set; }
        public string StoreName { get; set; }

        [MaxLength(1000, ErrorMessageResourceName = ZnodeAdmin_Resources.TrackingPixelScriptMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.TrackingPixelScriptMaxLength, ResourceType = typeof(Admin_Resources))]
        public string TrackingPixelScriptCode { get; set; }
    }
}
