
using System;
using Znode.Engine.Api.Models.Enum;

namespace Znode.Engine.Api.Models
{
    public class GatewayResponseModel : BaseModel
    {
        public string CardAuthCode { get; set; }
        public string CustomerPaymentProfileId { get; set; }
        public string CustomerProfileId { get; set; }
        public string CustomerGUID { get; set; }
        public string PaymentToken { get; set; }
        public string GatewayResponseData { get; set; }
        public string ReferenceNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string Token { get; set; }
        public string TransactionId { get; set; }
        public string HostUrl { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsGatewayPreAuthorize { get; set; }
        public ZnodePaymentStatus PaymentStatus { get; set; }
        public DateTime? TransactionDate { get; set; }
        public System.Guid PaymentGUID { get; set; }
        public string CardNumber { get; set; }
    }
}
