using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CustomerAffiliateViewModel : BaseViewModel
    {
        public int OmsReferralCommissionId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }
        public int UserId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCommissionType, ResourceType = typeof(Admin_Resources))]
        public int? ReferralCommissionTypeId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCommission, ResourceType = typeof(Admin_Resources))]
        public decimal? ReferralCommission { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelAmountOwed, ResourceType = typeof(Admin_Resources))]
        public decimal OwedAmount { get; set; }
        public decimal OrderCommission { get; set; }

        public string TransactionId { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = ZnodeAdmin_Resources.LabelPartnerApprovalStatus, ResourceType = typeof(Admin_Resources))]
        public string ReferralStatus { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelTrackingLink, ResourceType = typeof(Admin_Resources))]
        public string[] DomainIds { get; set; }

        public string CustomerName { get; set; }

        public List<SelectListItem> AvailablePortals { get; set; }
        public List<SelectListItem> ApprovalStatusList { get; set; }
        public List<SelectListItem> Domains { get; set; }
        public List<SelectListItem> ReferralCommissionTypes { get; set; }
    }
}