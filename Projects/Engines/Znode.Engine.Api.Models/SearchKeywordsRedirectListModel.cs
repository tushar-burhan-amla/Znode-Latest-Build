using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchKeywordsRedirectListModel : BaseListModel
    {
        public List<SearchKeywordsRedirectModel> KeywordsList { get; set; }
    }
}
