using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AddonGroupProductListResponse:BaseListResponse
    {
        public List<AddonGroupProductModel> AddonGroupProducts { get; set; }

        public AddonGroupProductListResponse()
        {
            AddonGroupProducts = new List<AddonGroupProductModel>();
        }
    }
}
