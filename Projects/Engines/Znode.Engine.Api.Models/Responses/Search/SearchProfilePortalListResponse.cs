using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses.Search
{
    public class SearchProfilePortalListResponse : BaseListResponse
    {
        public List<SearchProfilePortalModel> SearchProfilePortalList { get; set; }
    }
}
