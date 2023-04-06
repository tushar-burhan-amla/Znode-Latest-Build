namespace Znode.Engine.Api.Models
{
    public class OrderPaymentModel : BaseModel
    {
        public decimal OverDueAmount { get; set; }
        public string OrderState { get; set; }
        public decimal Total { get; set; }
    }
}
