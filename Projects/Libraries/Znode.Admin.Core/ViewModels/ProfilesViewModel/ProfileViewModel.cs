using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.ProfileNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterProfileName)]
        public string ProfileName { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }

        public string DefaultExternalAccountNo { get; set; }
        public bool ShowOnPartnerSignup { get; set; }
        public bool TaxExempt { get; set; }
        public bool IsParentAccount { get; set; }
        public bool IsDefault { get; set; }
        public int? AccountId { get; set; }
        public int? AccountProfileId { get; set; }
        public string Name { get; set; }
        public string ProfileIds { get; set; }
        public List<SelectListItem> TaxExemptList { get; set; }
        public int? ParentProfileId { get; set; }
    }
}