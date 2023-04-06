using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class WarehouseAddressViewModel : BaseViewModel
    {
        public int AddressId { get; set; }
        public int AccountId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAddressStreet1, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AddressStreet1CodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Address1 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAddressStreet2, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AddressStreet2CodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Address2 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFirstName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredFirstName)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.FirstNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string FirstName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLastName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredLastName)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.LastNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string LastName { get; set; }

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

        [Display(Name = ZnodeAdmin_Resources.LabelContactName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(600, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PhoneNumberMaxLengthErrorMessage)]
        public string DisplayName { get; set; }

        public bool IsActive { get; set; }
        public string ExternalId { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> States { get; set; }
    }
}