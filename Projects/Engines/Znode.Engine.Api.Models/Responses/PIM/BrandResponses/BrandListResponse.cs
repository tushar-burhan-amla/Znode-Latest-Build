using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class BrandListResponse : BaseListResponse
    {
        public List<BrandModel> Brands { get; set; }
        public List<PIMAttributeDefaultValueModel> BrandCodes { get; set; }
    }
}
