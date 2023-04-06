namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentGatewayResponse : BaseResponse
    {
        public PaymentGatewayListModel PaymentGatewayList { get; set; }
        public PaymentGatewayModel PaymentGateway { get; set; }
        public GatewayResponseModel GatewayResponse { get; set; }
        public PaymentModel PaymentModel { get; set; }
    }
}
