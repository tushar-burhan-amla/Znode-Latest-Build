using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PortalApprovalTypeListResponse : BaseListResponse
    {
        public List<PortalApprovalTypeModel> portalApprovalTypeListResponse { get; set; }
    }
}
