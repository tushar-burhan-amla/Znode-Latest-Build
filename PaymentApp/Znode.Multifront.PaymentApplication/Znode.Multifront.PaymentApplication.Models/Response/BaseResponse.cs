namespace Znode.Multifront.PaymentApplication.Models
{
    public class BaseResponse
    {
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
