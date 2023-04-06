using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalApprovalListModel : BaseListModel
    {
        public List<PortalApprovalModel> PortalApprovals { get; set; }

        public PortalApprovalListModel()
        {
            PortalApprovals = new List<PortalApprovalModel>();
        }
    }
}
