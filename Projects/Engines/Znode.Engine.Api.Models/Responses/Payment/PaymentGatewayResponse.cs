namespace Znode.Engine.Api.Models.Responses
{
    public class PaymentGatewayResponse : BaseResponse
    {
        public PaymentGatewayListModel PaymentGatewayList { get; set; }
        public PaymentGatewayModel PaymentGateway { get; set; }
        public GatewayResponseModel GatewayResponse { get; set; }
        public SubmitPaymentModel PaymentModel { get; set; }
    }
}
