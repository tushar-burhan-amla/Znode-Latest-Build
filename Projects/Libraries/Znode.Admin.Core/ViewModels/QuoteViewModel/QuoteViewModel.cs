using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class QuoteViewModel : BaseViewModel
    {
        public int OmsQuoteId { get; set; }
        public string QuoteNumber { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public int OmsQuoteStateId { get; set; }
        public string QuoteStatus { get; set; }
        public DateTime QuoteDate { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }
        public string StoreName { get; set; }
        public string CustomerName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string TotalAmount { get; set; }
        public string CultureCode { get; set; }
        
    }
}
