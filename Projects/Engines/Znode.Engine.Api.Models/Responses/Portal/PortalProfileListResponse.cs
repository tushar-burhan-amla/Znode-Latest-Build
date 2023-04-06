using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PortalProfileListResponse : BaseListResponse
    {
        public List<PortalProfileModel> PortalProfiles { get; set; }
    }
}
