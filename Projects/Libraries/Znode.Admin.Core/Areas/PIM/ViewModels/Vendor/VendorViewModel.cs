using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class VendorViewModel : BaseViewModel
    {
        public int PimVendorId { get; set; }

        [Display(Name = ZnodePIM_Resources.LabelVendorName, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorVendorRequired)]
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public int? PimAttributeId { get; set; }
        public int? AddressId { get; set; }
        public string ExternalVendorNo { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEmail, ResourceType = typeof(Admin_Resources))]
        [EmailAddress(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "ValidEmailAddress", ErrorMessage = "")]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredEmail")]
        public string Email { get; set; }
        public string NotificationEmailID { get; set; }
        public string EmailNotificationTemplate { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCompany, ResourceType = typeof(Admin_Resources))]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.CompanyNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string CompanyName { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public List<SelectListItem> VendorCodeList { get; set; }
        public WarehouseAddressViewModel Address { get; set; }
    }
}