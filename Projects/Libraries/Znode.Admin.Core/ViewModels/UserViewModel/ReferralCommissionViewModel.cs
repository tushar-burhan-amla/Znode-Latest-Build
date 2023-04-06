using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReferralCommissionViewModel : BaseViewModel
    {
        public int OmsReferralCommissionId { get; set; }
        public int PortalId { get; set; }
        [Required]
        public int UserId { get; set; }
        public int? ReferralCommissionTypeId { get; set; }
       
        public string ReferralCommission { get; set; }
        public decimal OwedAmount { get; set; }
        public string OrderCommission { get; set; }

        public string TransactionId { get; set; }
        public string Description { get; set; }
        public string ReferralStatus { get; set; }

        public string[] DomainIds { get; set; }

        public List<PortalViewModel> Portals { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public string Name { get; set; }
        public string OrderNumber { get; set; }
    }
}