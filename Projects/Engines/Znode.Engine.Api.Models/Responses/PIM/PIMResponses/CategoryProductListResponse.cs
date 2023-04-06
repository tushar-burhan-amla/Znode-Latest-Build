using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CategoryProductListResponse : BaseListResponse
    {
        public List<CategoryProductModel> CategoryProducts { get; set; }
    }
}
