using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AddonProductListResponse:BaseListResponse
    {
        public List<AddOnProductModel> AddonProducts { get; set; }
    }
}
