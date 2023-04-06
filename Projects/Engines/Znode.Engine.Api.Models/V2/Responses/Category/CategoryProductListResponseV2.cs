using System.Collections.Generic;
using Znode.Engine.Api.Models.V2;

namespace Znode.Engine.Api.Models.Responses.V2
{
    public class CategoryProductListResponseV2 : BaseListResponse
    {
        public List<CategoryProductModelV2> CategoryProducts { get; set; }
    }
}
