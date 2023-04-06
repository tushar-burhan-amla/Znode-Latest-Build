using Znode.Multifront.PaymentApplication.Models.Models;

namespace Znode.Multifront.PaymentApplication.Models
{
    public class RefundPaymentModel : BaseModel
    {
        public string Token { get; set; }
        public decimal RefundAmount { get; set; }
        public bool IsCompleteOrderRefund { get; set; }

        public AddressModel BillingAddress { get; set; }
        public AddressModel ShippingAddress { get; set; }
    }
}
