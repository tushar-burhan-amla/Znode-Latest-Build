using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class ReferralCommissionModel : BaseModel
    {
        public int OmsReferralCommissionId { get; set; }
        public int PortalId { get; set; }
        [Required]
        public int UserId { get; set; }
        public int? ReferralCommissionTypeId { get; set; }

        public decimal? ReferralCommission { get; set; }
        public decimal? TotalCommission { get; set; }
        public decimal OrderCommission { get; set; }
        public decimal? OwedAmount { get; set; }

        public string TransactionId { get; set; }
        public string Description { get; set; }
        public string ReferralStatus { get; set; }

        public string[] DomainIds { get; set; }

        public List<PortalModel> Portals { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public string Name { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public string ReferralCommissionType { get; set; }
        public string Url { get; set; }
    }
}
