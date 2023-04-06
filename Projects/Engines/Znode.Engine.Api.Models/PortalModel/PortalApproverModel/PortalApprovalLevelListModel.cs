using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalApprovalLevelListModel : BaseListModel
    {
        public List<PortalApprovalLevelModel> PortalApprovalLevels { get; set; }

        public PortalApprovalLevelListModel()
        {
            PortalApprovalLevels = new List<PortalApprovalLevelModel>();
        }
    }
}
