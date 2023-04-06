using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentGatewayTokenResponse : BaseResponse
    {
        public PaymentGatewayTokenModel PaymentTokenModel { get; set; }
    }
}
