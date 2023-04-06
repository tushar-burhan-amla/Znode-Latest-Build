using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PortalListResponse : BaseListResponse
    {
        public List<PortalModel> PortalList { get; set; }
        public List<PortalFeatureModel> PortalFeatureList { get; set; }
    }
}
