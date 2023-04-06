using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchGlobalProductBoostListResponse : BaseListResponse
    {
        public List<SearchGlobalProductBoostModel> SearchGlobalProductBoostList { get; set; }
    }
}