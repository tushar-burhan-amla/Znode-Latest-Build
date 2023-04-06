using System;

namespace Znode.Engine.Api.Models
{
    public class GiftCardHistoryModel : BaseModel
    {
        public int GiftCardHistoryId { get; set; }
        public int GiftCardId { get; set; }
        public int OmsOrderId { get; set; }
        public int? OmsUserId { get; set; }        
        public DateTime TransactionDate { get; set; }
        public decimal TransactionAmount { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string UserName { get; set; }
        public string CultureCode { get; set; }
        public string Notes { get; set; }
    }
}
