using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class QuickOrderProductListResponse : BaseListResponse
    {
        public List<QuickOrderProductModel> Products { get; set; }
    }
}
