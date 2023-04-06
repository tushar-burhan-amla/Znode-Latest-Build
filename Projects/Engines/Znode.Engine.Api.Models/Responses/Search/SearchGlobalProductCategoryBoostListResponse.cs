using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchGlobalProductCategoryBoostListResponse : BaseListResponse
    {
        public List<SearchGlobalProductCategoryBoostModel> SearchGlobalProductCategoryBoostList { get; set; }
    }
}