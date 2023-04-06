using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class HighlightProductListResponse : BaseListResponse
    {
        public List<HighlightProductModel> HighlightProductList { get; set; }
    }
}
