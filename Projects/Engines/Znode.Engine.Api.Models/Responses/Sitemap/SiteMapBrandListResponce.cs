using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SiteMapBrandListResponce : BaseListResponse
    {
        public List<SiteMapBrandModel> BrandList { get; set; }
    }
}
