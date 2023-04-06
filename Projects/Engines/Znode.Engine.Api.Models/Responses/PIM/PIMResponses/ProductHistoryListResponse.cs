using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProductHistoryListResponse : BaseListResponse
    {
        public List<ProductHistoryModel> ProductHistoryList { get; set; }
    }
}
