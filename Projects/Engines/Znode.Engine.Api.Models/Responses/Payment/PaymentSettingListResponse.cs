using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PaymentSettingListResponse : BaseListResponse
    {
        public List<PaymentSettingModel> PaymentSettings { get; set; }
    }
}
