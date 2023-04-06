using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMAReturnHistoryViewModel:BaseViewModel
    {
        public DateTime UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string Notes { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string ReturnDateWithTime { get; set; }
        public string ReturnAmountWithCurrency { get; set; }
    }
}
