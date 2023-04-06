using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Api.Models.Responses
{
    public class PaymentGatewayTokenResponse: BaseResponse
    {
        public PaymentGatewayTokenModel PaymentTokenModel { get; set; }
    }
}
