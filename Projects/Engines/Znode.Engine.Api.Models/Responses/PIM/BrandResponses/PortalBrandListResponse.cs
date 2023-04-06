using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
   public class PortalBrandListResponse:BaseListResponse
    {
        public List<PortalBrandModel> PortalBrand { get; set; }
    }
}
