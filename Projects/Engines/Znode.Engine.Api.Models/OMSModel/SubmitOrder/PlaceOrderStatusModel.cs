
namespace Znode.Engine.Api.Models
{
    public class PlaceOrderStatusModel
    {
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public int OmsOrderId { get; set; }
    }
}
