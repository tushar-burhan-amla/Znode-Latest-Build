namespace Znode.Multifront.PaymentApplication.Models
{
    public class TrueFalseResponse : BaseResponse
    {
        public BooleanModel booleanModel { get; set; }
        public bool IsSuccess { get; set; }
    }
}
