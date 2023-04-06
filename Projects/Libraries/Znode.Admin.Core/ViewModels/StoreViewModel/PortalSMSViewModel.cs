using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalSMSViewModel : BaseViewModel
    {
        public int PortalSmsSettingId { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public string SmsPortalAccountId { get; set; }
        public string AuthToken { get; set; }
        public string FromMobileNumber { get; set; }
        public int SMSProviderId { get; set; }

        public bool IsSMSSettingEnabled { get; set; }
        public List<BaseDropDownOptions> SmsProviderList { get; set; }

    }
}
