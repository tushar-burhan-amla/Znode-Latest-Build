using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderHistoryViewModel:BaseViewModel
    {
        public DateTime UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string Notes { get; set; }
        public decimal? OrderAmount { get; set; }
        public string OrderDateWithTime { get; set; }
        public string OrderAmountWithCurrency { get; set; }
    }
}
