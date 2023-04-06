using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class QuoteResponseViewModel : BaseViewModel
    {
        public int OmsQuoteId { get; set; }
        public string QuoteNumber { get; set; }
        public int PortalId { get; set; }
        public int OmsQuoteStateId { get; set; }
        public string QuoteStatus { get; set; }
        public bool IsConvertedToOrder { get; set; }
        public int LocaleId { get; set; }

        public int PublishCatalogId { get; set; }
        public int UserId { get; set; }
        public string CultureCode { get; set; }

        public string CustomerServiceEmail { get; set; }
        public string CustomerServicePhoneNumber { get; set; }

        public CustomerInfoViewModel CustomerInformation { get; set; }
        public QuoteInfoViewModel QuoteInformation { get; set; }
        public QuoteCartViewModel CartInformation { get; set; }

        public List<OrderHistoryViewModel> QuoteHistoryList { get; set; }
        public string AdditionalNotes { get; set; }
        public bool IsOldQuote { get; set; }
        public List<TaxSummaryViewModel> TaxSummaryList { get; set; }

    }
}
