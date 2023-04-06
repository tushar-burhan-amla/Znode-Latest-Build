using System.Collections.Generic;

namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentSettingListModel : BaseListModel
    {
        public PaymentSettingListModel()
        {
            PaymentSettings = new List<PaymentSettingsModel>();
        }
        public List<PaymentSettingsModel> PaymentSettings { get; set; }
    }
}
