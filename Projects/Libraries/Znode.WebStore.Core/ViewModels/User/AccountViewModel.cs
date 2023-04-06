using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.LabelAccountId, ResourceType = typeof(Admin_Resources))]
        public int AccountId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccountName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredAccountName)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AccountNameCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Name { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelPhoneNumber, ResourceType = typeof(Admin_Resources))]
        public string PhoneNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelExternalId, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.ExternalIdCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string ExternalId { get; set; }
        public string UserExternalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelParentAccount, ResourceType = typeof(Admin_Resources))]
        public int ParentAccountId { get; set; }
        public string ParentAccountName { get; set; }

        public List<SelectListItem> ParentAccountList { get; set; }
        public List<SelectListItem> CountryList { get; set; }

        public string AccountAddress { get; set; }
        public AddressViewModel Address { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> Portals { get; set; }
        public int? PortalId { get; set; }
        public string RoleName { get; set; }
    }
}