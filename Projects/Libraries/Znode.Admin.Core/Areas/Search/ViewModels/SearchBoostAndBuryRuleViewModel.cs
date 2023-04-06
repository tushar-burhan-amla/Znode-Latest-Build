using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Admin.Core.ViewModels;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchBoostAndBuryRuleViewModel : BaseViewModel
    {
        public int PublishCatalogId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCatalogName, ResourceType = typeof(Admin_Resources))]
        public string CatalogName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RuleNameRequiredMessage)]
        [RegularExpression(AdminConstants.RuleNameValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRuleName)]
        [Display(Name = ZnodeAdmin_Resources.LabelRuleName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(600, ErrorMessageResourceName = ZnodeAdmin_Resources.RuleNameMaxlengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string RuleName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.StartDateRequiredMessage)]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SearchCatalogRuleId { get; set; }
        public bool IsPaused { get; set; }
        public bool IsTriggerForAll { get; set; }
        public bool IsItemForAll { get; set; }
        public bool IsGlobalRule { get; set; }
        public int SearchItemRuleId { get; set; }
        public int SearchTriggerRuleId { get; set; }
        public bool IsPause { get; set; }
        public bool IsSearchIndexExists { get; set; }
        public string SearchTriggerCondition { get; set; }
        public bool IsActive { get; set; }
        public List<SearchTriggerRuleViewModel> SearchTriggerRuleList { get; set; }
        public List<SearchItemRuleViewModel> SearchItemRuleList { get; set; }
        public List<SelectListItem> TriggerOperatorList { get; set; }

        public List<SelectListItem> SearchItemTextOperatorList { get; set; }

        public List<FieldValueViewModel> SearchConditionList { get; set; }

        public List<SelectListItem> SearchItmeNumericOperatorList { get; set; }

        public string Paused { get; set; }

        public string UserName { get; set; }

    }
}
