using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CacheViewModel : BaseViewModel
    {
        public int ApplicationCacheId { get; set; }
        public string ApplicationType { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLastRefreshDate, ResourceType = typeof(Admin_Resources))]
        public string StartDate { get; set; }
        public string DomainIds { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CacheDurationRequiredError)]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRangeBetween)]
        public int Duration { get; set; }

        public List<CloudflareErrorViewModel> CloudflareErrorList { get; set; }
    }
}