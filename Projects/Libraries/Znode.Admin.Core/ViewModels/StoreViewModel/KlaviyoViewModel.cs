using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class KlaviyoViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public int PortalKlaviyoSettingId { get; set; }
        public string KlaviyoCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorKlaviyoApiKeyRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelPublicApiKey, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.ApiKeyMaxLengthMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string PublicApiKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredUserName)]
        [Display(Name = ZnodeAdmin_Resources.LabelKlaviyoUserName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.KlaviyoUserNamePasswordError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string KlaviyoUserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredPassword)]
        [DataType(DataType.Password)]
        [Display(Name = ZnodeAdmin_Resources.LabelKlaviyoPassword, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.KlaviyoUserNamePasswordError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string KlaviyoPassword { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActiveForKlaviyo, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }
        public int EmailProviderId { get; set; }
        public List<BaseDropDownOptions> EmailProviderList { get; set; }
    }

}
