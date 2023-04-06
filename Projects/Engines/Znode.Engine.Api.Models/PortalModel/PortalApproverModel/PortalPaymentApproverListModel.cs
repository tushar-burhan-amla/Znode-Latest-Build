using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalPaymentApproverListModel : BaseListModel
    {
        public List<PortalPaymentApproverModel> PortalPaymentApproverList { get; set; }

        public PortalPaymentApproverListModel()
        {
            PortalPaymentApproverList = new List<PortalPaymentApproverModel>();
        }
    }
}
