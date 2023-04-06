namespace Znode.Engine.Api.Models
{
    public class TransactionDetailsModel
    {
        public string TransactionId { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionType { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public bool IsRefundable { get; set; }
        public bool IsVoidable { get; set; }
        public string GatewayCode { get; set; }
    }
}
