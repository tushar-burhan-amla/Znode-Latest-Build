using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.Resources;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Admin.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.LabelAccount, ResourceType = typeof(Admin_Resources))]
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

        [Display(Name = ZnodeAdmin_Resources.LabelParentAccount, ResourceType = typeof(Admin_Resources))]
        public int ParentAccountId { get; set; }
        public string ParentAccountName { get; set; }

        public List<SelectListItem> ParentAccountList { get; set; }

        public string AccountAddress { get; set; }
        public AddressViewModel Address { get; set; }
        public int? PortalId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public string StoreName { get; set; }
        public List<SelectListItem> Portals { get; set; }
        public string ShippingPostalCode { get; set; }
        public string BillingPostalCode { get; set; }

        public int? PublishCatalogId { get; set; }
        public bool IsDefault { get; set; }
        public string CatalogName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorAccountCodeRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelAccountCode, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AccountCodeMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string AccountCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.SalesRep, ResourceType = typeof(Admin_Resources))]
        public string SalesRepUserName { get; set; }
        public int SalesRepId { get; set; }
        public string SalesRepFullName { get; set; }
    }
}