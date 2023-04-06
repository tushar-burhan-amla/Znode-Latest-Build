using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CategoryTreeResponse : BaseResponse
    {
        public List<CategoryTreeModel> CategoryTree { get; set; }

        public ContentPageTreeModel Tree { get; set; }
    }
    public class CatalogTreeResponse : BaseResponse
    {
        public List<CatalogTreeModel> Tree { get; set; }
    }
}
