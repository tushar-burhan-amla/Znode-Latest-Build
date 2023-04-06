using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Api.Models
{
   public class PaymentGatewayTokenModel : BaseModel
    {
        public string PaymentGatewayToken { get; set; }
        public string PaymentGatewayTokenUrl { get; set; }
        public decimal Total { get; set; }
        public string PaymentCode { get; set; }
        public int PaymentSettingId { get; set; }
        public string CustomerProfileId { get; set; }

        //This will be admin or webstore AuthorizeNet iframe URL
        public string IFrameUrl { get; set; }
        public string GatewayLoginName { get; set; }
        public string GatewayTransactionKey { get; set; }
        public string CustomerGUID { get; set; }
        public string PaymentGatewayId { get; set; }

        public string GatewayCode { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; }
        public bool IsAdminRequestUrl { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public AddressModel BillingAddress { get; set; }
        public string GatewayPassword { get; set; }
        public bool GatewayTestMode { get; set; }
    }
}
