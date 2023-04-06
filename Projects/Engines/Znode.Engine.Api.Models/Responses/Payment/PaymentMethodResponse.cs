using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PaymentMethodResponse : BaseResponse
    {
        public PaymentMethodResponse()
        {
            PaymentMethodCCDetails = new List<PaymentMethodCCDetailsModel>();
        }
        public List<PaymentMethodCCDetailsModel> PaymentMethodCCDetails { get; set; }
    }
}
