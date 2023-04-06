using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ShippingRuleModel : BaseModel
    {
        public int ShippingRuleId { get; set; }
        public int ShippingId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public int ShippingRuleTypeId { get; set; }
        public string ClassName { get; set; }
        public decimal? LowerLimit { get; set; }
        public decimal? UpperLimit { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public decimal BaseCost { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public decimal PerItemCost { get; set; }
        public string ExternalId { get; set;} 
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public string ShippingRuleTypeCode { get; set; }
        public string ShippingRuleTypeCodeLocale { get; set; }
        public int? CurrencyId { get; set; }
    }
}
