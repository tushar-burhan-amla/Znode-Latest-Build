namespace Znode.Engine.Api.Models
{
    public class QuoteStatusModel : BaseModel
    {
        public string OmsQuoteIds { get; set; }
        public string OrderStatus { get; set; }
        public string Notes { get; set; }
        public int OmsOrderStateId { get; set; }
        public int LocaleId { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsPendingPaymentStatus { get; set; }
        public string Comments { get; set; }
    }
}
