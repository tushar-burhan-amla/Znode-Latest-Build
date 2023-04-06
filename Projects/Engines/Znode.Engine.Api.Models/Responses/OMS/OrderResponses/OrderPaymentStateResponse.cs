using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class OrderPaymentStateResponse : BaseListResponse
    {
        public List<OrderPaymentStateModel> PaymentStateList { get; set; }
    }
}
