using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Admin.Core.ViewModels
{
    public class SearchItemRuleViewModel : BaseViewModel
    {
        public int SearchItemRuleId { get; set; }
        public int SearchCatalogRuleId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredItemKeywordField)]
 
        public string SearchItemKeyword { get; set; }
        public string SearchItemCondition { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredItemValueField)]
        [MaxLength(600, ErrorMessageResourceName = ZnodeAdmin_Resources.TriggerItemValueMaxlengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SearchItemValue { get; set; }
       
        [Range(0, 999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.BoostValueRangeValidator)]
        [RegularExpression(@"^(?!(?:1|(?:1+([.][0-0]*))|1.00)$)\d+(\.\d{1,2})?", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.BoostValueRangeValidator)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredBoostValueField)]
        public decimal? SearchItemBoostValue { get; set; }
}
}
