using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AddonGroupListResponse : BaseListResponse
    {
        public List<AddonGroupModel> AddonGroups { get; set; }

        public AddonGroupListResponse()
        {
            AddonGroups = new List<AddonGroupModel>();
        }
    }
}
