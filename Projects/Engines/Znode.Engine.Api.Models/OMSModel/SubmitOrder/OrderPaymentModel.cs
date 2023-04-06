using System;

namespace Znode.Engine.Api.Models
{
    public class OrderPaymentDataModel : BaseModel
    {
        public int OrderPaymentId { get; set; }
        public int OmsOrderId { get; set; }
        public string TransactionReference { get; set; }
        public decimal Total { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime TransactionDate { get; set; }
        public int PaymentSettingId { get; set; }
        public decimal RemainingOrderAmount { get; set; }
    }
}
