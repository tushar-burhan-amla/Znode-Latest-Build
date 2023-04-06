using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PortalApproverListResponse : BaseListResponse
    {
        public List<PortalApprovalModel> PortalApprovalResponse { get; set; }
    }
}
