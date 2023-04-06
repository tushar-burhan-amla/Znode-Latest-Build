using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalApprovalTypeListModel : BaseListModel
    {
        public List<PortalApprovalTypeModel> PortalApprovalTypes { get; set; }

        public PortalApprovalTypeListModel()
        {
            PortalApprovalTypes = new List<PortalApprovalTypeModel>();
        }
    }
}
