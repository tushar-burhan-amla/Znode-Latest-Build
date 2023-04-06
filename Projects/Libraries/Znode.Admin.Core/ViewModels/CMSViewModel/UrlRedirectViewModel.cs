using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Helpers;

namespace Znode.Engine.Admin.ViewModels
{
    public class UrlRedirectViewModel : BaseViewModel
    {
        public int CMSUrlRedirectId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelRedirectFrom, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [RegularExpression(AdminConstants.URLRedirectValidation, ErrorMessageResourceName = ZnodeAdmin_Resources.UrlRedirectValidationMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string RedirectFrom { get; set; }
        
        [Display(Name = ZnodeAdmin_Resources.LabelRedirectTo, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [RegularExpression(AdminConstants.URLRedirectValidation, ErrorMessageResourceName = ZnodeAdmin_Resources.UrlRedirectValidationMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string RedirectTo { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEnableRedirection, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }  
            
        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        public string StoreName { get; set; }
    }
}