using System.Collections.Generic;

namespace Znode.Engine.SMS
{
    public class ZnodeSMSContext
    {
        public int PortalId { get; set; }
        public string SmsTemplateName { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public Dictionary<string, string> MacroValues { get; set; }
        public bool SMSOptIn { get; set; }
    }
}
