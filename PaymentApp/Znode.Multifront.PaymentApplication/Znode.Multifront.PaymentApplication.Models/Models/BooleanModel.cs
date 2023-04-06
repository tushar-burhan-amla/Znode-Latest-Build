namespace Znode.Multifront.PaymentApplication.Models
{
    public class BooleanModel : BaseModel
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
