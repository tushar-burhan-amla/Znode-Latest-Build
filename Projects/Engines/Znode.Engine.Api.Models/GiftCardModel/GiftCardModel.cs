using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GiftCardModel : BaseModel
    {
        public int GiftCardId { get; set; }
        public int? UserId { get; set; }
        public int PortalId { get; set; }

        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string DisplayAmount { get; set; }
      
        public decimal? Amount { get; set; }
        public decimal? OwedAmount { get; set; }
        public decimal? LeftAmount { get; set; }
        public decimal? TransactionAmount { get; set; }
        public bool IsReferralCommission { get; set; }
        public decimal? RemainingAmount { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public bool RestrictToCustomerAccount { get; set; }

        public List<PortalModel> PortalList { get; set; }
        public bool SendMail { get; set; }
        public int OMSOrderDetailsId { get; set; }
        public string NotificationSentToCustomer { get; set; }

        public string StoreName { get; set; }
        public string CustomerName { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
    }
}
