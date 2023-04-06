using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchSynonymsListResponse : BaseListResponse
    {
        public List<SearchSynonymsModel> SynonymsList { get; set; }
    }
}
