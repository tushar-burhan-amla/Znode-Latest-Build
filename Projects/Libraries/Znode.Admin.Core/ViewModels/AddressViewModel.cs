using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddressViewModel : BaseViewModel
    {        
        public int AddressId { get; set; }
        public int AccountId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelAddress1, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredAddress2)]
        [MaxLength(200, ErrorMessageResourceName = ZnodeAdmin_Resources.StreetCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Address1 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAddress2, ResourceType = typeof(Admin_Resources))]
        [MaxLength(200, ErrorMessageResourceName = ZnodeAdmin_Resources.StreetCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Address2 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFirstName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredFirstName)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.FirstNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string FirstName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLastName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredLastName)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.LastNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string LastName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAddress3, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.StreetCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Address3 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCountry, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredCountryCode)]
        public string CountryName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelState, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredStateCode)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.StateProvinceRegionCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string StateName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCity, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredCityCode)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.CityCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string CityName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPostalCode, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredPostalCode)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.PostalCodeCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string PostalCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPhoneNumber, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredPhoneNumber)]
        [MaxLength(30, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PhoneNumberMaxLengthErrorMessage)]
        public string PhoneNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAddressName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.DisplayNameMaxLengthErrorMessage)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredAddress1)]
        public string DisplayName { get; set; }

        public bool IsDefaultBilling { get; set; }
        public bool IsDefaultShipping { get; set; }
        public bool IsActive { get; set; }
        public List<SelectListItem> Countries { get; set; }

        public bool IsShipping { get; set; }
        public bool IsBilling { get; set; }
        public bool UseSameAsShippingAddress { get; set; }

        public bool IsGuest { get; set; }
        public int PortalId { get; set; }
        public int AccountAddressId { get; set; }
        public int UserAddressId { get; set; }
        public int UserId { get; set; }
        public int ListCount { get; set; }
        public string DisplayAddress { get; set; }
        public string CheckboxPrefix { get; set; }
        [Display(Name = ZnodeAdmin_Resources.HeaderCompanyName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(300, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CompanyNameLengthError)]
        public string CompanyName { get; set; }
        public string FaxNumber { get; set; }
        public bool IsShippingAddressChange { get; set; }
        public int SelectedShippingId { get; set; }
        public int SelectedBillingId { get; set; }
        public string ExternalId { get; set; }
        public List<SelectListItem> UsersAddressNameList { get; set; }
        public string FromBillingShipping { get; set; }
        public string StateCode { get; set; }
        public List<SelectListItem> States { get; set; }
        public bool IsAddUpdateBillingAddress { get; set; }
        public bool IsAddUpdateShippingAddress { get; set; }
        public bool DontAddUpdateAddress { get; set; }        
        public int omsOrderId { get; set; }
        public int omsOrderShipmentId { get; set; }
        public bool IsShippingBillingDifferent { get; set; }
        public int PublishStateId { get; set; }
        public int OmsQuoteId { get; set; }
        public bool IsQuote { get; set; }
        public bool IsAddEditAddressAllow { get; set; }
        public string EmailAddress { get; set; }
    }
}