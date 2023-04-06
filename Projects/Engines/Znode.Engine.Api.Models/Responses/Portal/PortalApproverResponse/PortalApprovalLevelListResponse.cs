using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PortalApprovalLevelListResponse : BaseListResponse
    {
        public List<PortalApprovalLevelModel> portalApprovalLevelList { get; set; }
    }
}
