
namespace Znode.Multifront.PaymentApplication.Models
{
    /// <summary>
    /// This is the model used for the Subscription
    /// </summary>
    public class SubscriptionModel : BaseModel
    {
        public decimal Amount { get; set; }
        public int TotalCycles { get; set; }
        public string Frequency { get; set; }
        public string GUID { get; set; }
        public string InitialAmount { get; set; }
        public string InstallmentInd { get; set; }
        public string InvoiceNo { get; set; }
        public string Period { get; set; }
        public string ProfileName { get; set; }
    }
}
