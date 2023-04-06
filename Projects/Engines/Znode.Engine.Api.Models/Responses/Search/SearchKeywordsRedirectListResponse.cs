using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchKeywordsRedirectListResponse : BaseListResponse
    {
        public List<SearchKeywordsRedirectModel> KeywordsList { get; set; }
    }
}
