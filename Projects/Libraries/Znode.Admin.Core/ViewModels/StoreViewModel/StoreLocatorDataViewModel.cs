using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class StoreLocatorDataViewModel : BaseViewModel
    {
        public int AddressId { get; set; }
        public int PortalId { get; set; }

        [Display(Name = ZnodeAttributes_Resources.LabelDisplayOrder, ResourceType = typeof(Attributes_Resources))]
        [Range(1, 999, ErrorMessageResourceName = ZnodeAttributes_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(Attributes_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredDisplayOrder)]
        public int? DisplayOrder { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStoreLocationName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string StoreName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorStoreLocationCodeRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelStoreLocatorCode, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.StoreLocationCodeMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]

        public string StoreLocationCode { get; set; }

        public string PortalName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> PortalList { get; set; }
        
        public int PortalAddressId { get; set; }
        public string MapQuestURL { get; set; }
        public string MediaPath { get; set; }
        public int? MediaId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredAddress2)]
        [Display(Name = ZnodeAdmin_Resources.LabelAddress1, ResourceType = typeof(Admin_Resources))]       
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AddressNameCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Address1 { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAddress2, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.StreetCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Address2 { get; set; }

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

        [Display(Name = ZnodeAdmin_Resources.LabelContactName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(600, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PhoneNumberMaxLengthErrorMessage)]
        public string DisplayName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFaxNumber, ResourceType = typeof(Admin_Resources))]
        [MaxLength(30, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.FaxNumberMaxLengthErrorMessage)]
        public string FaxNumber { get; set; }

        public bool IsActive { get; set; }

        [RegularExpression(AdminConstants.DecimalCoOrdinatesRegularExpression, ErrorMessageResourceName = ZnodeAdmin_Resources.MaxLimitError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public decimal? Longitude { get; set; }

        [RegularExpression(AdminConstants.DecimalCoOrdinatesRegularExpression, ErrorMessageResourceName = ZnodeAdmin_Resources.MaxLimitError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public decimal? Latitude { get; set; }

        public List<SelectListItem> CountryList { get; set; }

        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.CompanyNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string CompanyName { get; set; }

        public List<SelectListItem> States { get; set; }
    }
}