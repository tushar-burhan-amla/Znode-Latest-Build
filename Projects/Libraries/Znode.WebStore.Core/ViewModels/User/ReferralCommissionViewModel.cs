namespace Znode.Engine.WebStore.ViewModels
{
    public class ReferralCommissionViewModel : BaseViewModel
    {
        public int OmsReferralCommissionId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public int? ReferralCommissionTypeId { get; set; }

        public string ReferralCommission { get; set; }
        public decimal? OwedAmount { get; set; }

        public string ReferralStatus { get; set; }

        public string[] DomainIds { get; set; }

        public int OmsOrderDetailsId { get; set; }

        public string ReferralCommissionType { get; set; }
        public string Url { get; set; }
    }
}
