using System.Collections.Generic;

namespace Znode.Engine.Api.Models.V2
{
    public class CategoryProductListModelV2 : BaseListModel
    {
        public List<CategoryProductModelV2> CategoryProducts { get; set; }
    }
}
