using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class TaxRuleViewModel : BaseViewModel
    {
        public int TaxRuleId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TaxType, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.TaxRuleTypeRequiredMessage)]
        public int? TaxRuleTypeId { get; set; }
        public string TaxRuleTypeName { get; set; }
        public int PortalId { get; set; }
        public int? TaxClassId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.DestinationCountry, ResourceType = typeof(Admin_Resources))]
        public string DestinationCountryCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.DestinationState, ResourceType = typeof(Admin_Resources))]
        public string DestinationStateCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.DestinationCounty, ResourceType = typeof(Admin_Resources))]
        public string CountyFIPS { get; set; }

        [Display(Name = ZnodeAdmin_Resources.Precedence, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Range(1, 999999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidDisplayOrder)]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidDisplayOrder)]
        public int Precedence { get; set; }

        [Display(Name = ZnodeAdmin_Resources.SalesTax, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.SalesTaxErrorMessage)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeBetween0to100)]
        public decimal? SalesTax { get; set; }

        [Display(Name = ZnodeAdmin_Resources.VATTax, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeBetween0to100)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.VATTaxErrorMessage)]
        public decimal? VAT { get; set; }

        [Display(Name = ZnodeAdmin_Resources.GSTTax, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeBetween0to100)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.GSTTaxErrorMessage)]
        public decimal? GST { get; set; }

        [Display(Name = ZnodeAdmin_Resources.PSTTax, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeBetween0to100)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PSTTaxErrorMessage)]
        public decimal? PST { get; set; }

        [Display(Name = ZnodeAdmin_Resources.HSTTax, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeBetween0to100)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.HSTTaxErrorMessage)]
        public decimal? HST { get; set; }

        [MaxLength(5000, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ZipCodeLengthErrorMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelZipCode, ResourceType = typeof(Admin_Resources))]
        public string ZipCode { get; set; }
        public bool TaxShipping { get; set; }
       
        public string ExternalId { get; set; }

        public List<SelectListItem> TaxRuleTypeList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> CountyList { get; set; }
        public string SelectedCountryCodes { get; set; } = string.Empty;
    }
}