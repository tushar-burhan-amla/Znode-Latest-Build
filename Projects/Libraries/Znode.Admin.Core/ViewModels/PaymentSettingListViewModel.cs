using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PaymentSettingListViewModel : BaseViewModel
    {
        public PaymentSettingListViewModel()
        {
            PaymentSettings = new List<PaymentSettingViewModel>();
        }
        public List<PaymentSettingViewModel> PaymentSettings { get; set; }
        public GridModel GridModel { get; set; }
        public string PortalName { get; set; }
        public string ProfileName { get; set; }
        public int PortalId { get; set; }
        public int ProfileId { get; set; }
        public bool IsUsedForOfflinePayment { get; set; }

    }
}