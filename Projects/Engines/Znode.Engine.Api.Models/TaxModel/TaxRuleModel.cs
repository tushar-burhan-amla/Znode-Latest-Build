using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class TaxRuleModel : BaseModel
    {
        public int TaxRuleId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public int? TaxRuleTypeId { get; set; }
        public string TaxRuleTypeName { get; set; }
        public int PortalId { get; set; }
        public int? TaxClassId { get; set; }
        public string DestinationCountryCode { get; set; }
        public string DestinationStateCode { get; set; }
        public string CountyFIPS { get; set; }
        [Range(1, 999999999, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.InvalidDisplayOrder)]
        public int Precedence { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.SalesTaxErrorMessage)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RangeBetween0to100)]
        public decimal? SalesTax { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RangeBetween0to100)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.VATTaxErrorMessage)]
        public decimal? VAT { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RangeBetween0to100)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.GSTTaxErrorMessage)]
        public decimal? GST { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RangeBetween0to100)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.PSTTaxErrorMessage)]
        public decimal? PST { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RangeBetween0to100)]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.HSTTaxErrorMessage)]
        public decimal? HST { get; set; }
        public bool TaxShipping { get; set; }
        public bool InclusiveInd { get; set; }
        public string ExternalId { get; set; }
        public decimal TaxRate { get; set; }

        [MaxLength(5000, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ZipCodeLengthErrorMessage)]
        public string ZipCode { get; set; }
        // This property is used instead of adding whole "classmodel". 
        public string ClassName { get; set; }
        public bool? IsDefault { get; set; }
    }
}
