using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AddressViewModel : BaseViewModel
    {
        public int AddressId { get; set; }
        public int OtherAddressId { get; set; }
        public int AccountId { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelAddress1, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredStreetAddress)]
        [StringLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.StreetAddressLengthErrorMessage)]
        public string Address1 { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelAddress2, ResourceType = typeof(WebStore_Resources))]
        [StringLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.StreetAddressLengthErrorMessage)]
        public string Address2 { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelAddressName, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredDisplayName)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeWebStore_Resources.DisplayNameLengthError, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string DisplayName { get; set; }


        [Display(Name = ZnodeWebStore_Resources.LabelFirstName, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredFirstName)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeWebStore_Resources.FirstNameLengthError, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string FirstName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelLastName, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredLastName)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeWebStore_Resources.LastNameLengthError, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string LastName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelAddress3, ResourceType = typeof(WebStore_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeWebStore_Resources.StreetCodeLengthErrorMessage, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string Address3 { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelCountry, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredCountryCode)]
        public string CountryName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelState, ResourceType = typeof(WebStore_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeWebStore_Resources.StateProvinceRegionCodeLengthErrorMessage, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string StateName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelCity, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredCityCode)]
        [StringLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.LengthErrorMessage)]
        public string CityName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelPostalZipCode, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredPostalCode)]
        [StringLength(10, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.LengthErrorMessage)]
        public string PostalCode { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelPhoneNumber, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredPhoneNumber)]
        [MaxLength(30, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.PhoneNumberMaxLengthErrorMessage)]
        //[RegularExpression(@"^\(?([0-9]{1,})\)?[-. ]?([0-9]{1,})[-. ]?([0-9]{1,})[-. ]?([0-9]{1,})[-. ]?([0-9]{1,})[-. ]?([0-9]{1,})[-. ]?([0-9]{1,})[-. ]?([0-9]{1,})$", ErrorMessageResourceName = ZnodeWebStore_Resources.MessageNumericValueAllowed, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ValidEmailAddress)]
        [MaxLength(50, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.EmailMaxLengthError)]
        public string EmailAddress { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelMobileNumber, ResourceType = typeof(WebStore_Resources))]
        public string Mobilenumber { get; set; }
        public string FaxNumber { get; set; }
        public string AlternateMobileNumber { get; set; }

        public bool IsActive { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public bool IsDefaultBilling { get; set; }
        public bool IsShippingBillingDifferent { get; set; }
        public bool IsDefaultShipping { get; set; }

        public bool IsShipping { get; set; }
        public bool IsBilling { get; set; }
        public bool IsBothBillingShipping { get; set; }
        public bool IsSameAsBillingAddress { get; set; }
        public bool UseSameAsShippingAddress { get; set; }
        public string AddressType { get; set; }
        public int UserId { get; set; }
        public string AspNetUserId { get; set; }
        public int AccountAddressId { get; set; }
        public string RoleName { get; set; }
        public int AddressCount { get; set; }
        public string ExternalId { get; set; }
        public List<SelectListItem> States { get; set; }
        public string StateCode { get; set; }
        [MaxLength(100, ErrorMessageResourceName = ZnodeWebStore_Resources.CompanyNameLengthError, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string CompanyName { get; set; }
        public bool DontAddUpdateAddress { get; set; }
        public AddressViewModel()
        {
            Countries = new List<SelectListItem>();
            States = new List<SelectListItem>();
        }
        public bool IsAddressBook { get; set; }
    }
}