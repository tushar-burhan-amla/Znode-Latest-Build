using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Admin.Core.ViewModels
{
    public class SearchTriggerRuleViewModel : BaseViewModel
    {
        public int SearchCatalogRuleId { get; set; }
        public int SearchTriggerRuleId { get; set; }
        public string SearchTriggerKeyword { get; set; }
        public string SearchTriggerCondition { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredTriggerValueField)]
        [MaxLength(255, ErrorMessageResourceName = ZnodeAdmin_Resources.TriggerValueMaxlengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression("[^,]+", ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMultipleTriggerValue, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SearchTriggerValue { get; set; }

    }
}
