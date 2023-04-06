using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ShippingRuleViewModel : BaseViewModel
    {
        public int ShippingRuleId { get; set; }
        public int ShippingId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ShippingRuleTypeRequiredMessage)]
        public int ShippingRuleTypeId { get; set; }
        public string ClassName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LowerLimit, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorShippingAmount)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.LowerLimitRequiredMessage)]
        [Range(0, 999999.99, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeValidatorForShipping)]
        [GenericCompare(CompareToPropertyName = "UpperLimit", OperatorName = GenericCompareOperator.LessThanOrEqual, ErrorMessageResourceName = ZnodeAdmin_Resources.LowerLimitLessThanOrEqualsToUpperLimit,
            ErrorMessageResourceType = typeof(Admin_Resources))]
        public decimal? LowerLimit { get; set; }

        [Display(Name = ZnodeAdmin_Resources.UpperLimit, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorShippingAmount)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.UpperLimitRequiredMessage)]
        [Range(0, 999999.99, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeValidatorForShipping)]
        [GenericCompare(CompareToPropertyName = "LowerLimit", OperatorName = GenericCompareOperator.GreaterThanOrEqual, ErrorMessageResourceName = ZnodeAdmin_Resources.UpperLimitGreaterThanOrEqualsToLowerLimit,
            ErrorMessageResourceType = typeof(Admin_Resources))]
        public decimal? UpperLimit { get; set; }

        [Display(Name = ZnodeAdmin_Resources.BaseCost, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorShippingAmount)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.BaseCostRequiredMessage)]
        [Range(0, 999999.99, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeValidatorForShipping)]
        public decimal? BaseCost { get; set; }

        [Display(Name = ZnodeAdmin_Resources.PerUnitCost, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorShippingAmount)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PerItemCostRequiredMessage)]
        [Range(0, 999999.99, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeValidatorForShipping)]
        public decimal? PerItemCost { get; set; }
      
        public string ExternalId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ShippingRuleTypeRequiredMessage)]
        public string ShippingRuleTypeCode { get; set; }
        public int ShippingTypeId { get; set; }
        public List<SelectListItem> ShippingRuleTypeList { get; set; }
        public string ShippingRuleTypeCodeLocale { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }
        public int? CurrencyId { get; set; }
    }
}