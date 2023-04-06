using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class TaxPortalViewModel : BaseViewModel
    {
        public int TaxPortalId { get; set; }
        public int PortalId { get; set; }
        
        [MaxLength(50, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        [Display(Name = ZnodeAdmin_Resources.TextUserName, ResourceType = typeof(Admin_Resources))]
        public string AvataxUserName { get; set; }
        [MaxLength(50, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        [Display(Name = ZnodeAdmin_Resources.Password, ResourceType = typeof(Admin_Resources))]
        public string AvataxPassword { get; set; }
        [MaxLength(50, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        [Display(Name = ZnodeAdmin_Resources.LabelSecretKey, ResourceType = typeof(Admin_Resources))]
        public string SecretKey { get; set; }
        
        public string PortalName { get; set; }
        
        [Display(Name = ZnodeAdmin_Resources.LabelAvataxUrl, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        public string AvataxUrl { get; set; }
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        [Display(Name = ZnodeAdmin_Resources.TextAvataxAccountNumber, ResourceType = typeof(Admin_Resources))]
        public string AvalaraAccount { get; set; }
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        [Display(Name = ZnodeAdmin_Resources.TextAvataxLicenseKey, ResourceType = typeof(Admin_Resources))]
        public string AvalaraLicense { get; set; }
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        [Display(Name = ZnodeAdmin_Resources.TextCompanyCode, ResourceType = typeof(Admin_Resources))]
        public string AvalaraCompanyCode { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TextAvalaraFreightIdentifier, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        public string AvalaraFreightIdentifier { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAvalaraIsTaxIncluded, ResourceType = typeof(Admin_Resources))]
        public bool AvataxIsTaxIncluded { get; set; }        
        [Display(Name = ZnodeAdmin_Resources.Username, ResourceType = typeof(Admin_Resources))]
        public new string Custom1 { get; set; }
        [Display(Name = ZnodeAdmin_Resources.Password, ResourceType = typeof(Admin_Resources))]
        public new string Custom2 { get; set; }
      
    }
}
