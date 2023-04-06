using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PaymentSettingListModel : BaseListModel
    {
        public List<PaymentSettingModel> PaymentSettings { get; set; }
    }
}
