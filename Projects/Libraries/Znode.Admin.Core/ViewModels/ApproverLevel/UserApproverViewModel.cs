using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class UserApproverViewModel : BaseViewModel
    {
        public int UserApproverId { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int ApproverLevelId { get; set; }
        public int ApproverUserId { get; set; }
        public int ApproverOrder { get; set; }
        public bool IsNotifyEmail { get; set; }
        public bool IsMandatory { get; set; }
        [RegularExpression(@"^\d{0,}(\.\d{1,6})?$", ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidToAmount, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelToAmount, ResourceType = typeof(Admin_Resources))]
        public Nullable<decimal> ToBudgetAmount { get; set; }
        [RegularExpression(@"^\d{0,}(\.\d{1,6})?$", ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidFromAmount, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelFromAmount, ResourceType = typeof(Admin_Resources))]
        public Nullable<decimal> FromBudgetAmount { get; set; }
        public Nullable<bool> IsNoLimit { get; set; }
        public int OmsOrderStateId { get; set; }
        public int OmsQuoteId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredApproverName")]
        public string ApproverName { get; set; }
        public string OmsOrderState { get; set; }
        public string ApproverLevelName { get; set; }
        public bool IsAddMode { get; set; }
        public string ApproverUser { get; set; }
        public string LevelCode { get; set; }
        public string LevelName { get; set; }
        public string Description { get; set; }
        public List<SelectListItem> ApproverOrders { get; set; }
        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> Levels { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
