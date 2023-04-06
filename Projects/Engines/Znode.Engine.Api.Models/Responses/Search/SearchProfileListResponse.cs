using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchProfileListResponse : BaseListResponse
    {
        public List<SearchProfileModel> SearchProfileList { get; set; }
        public int PublishCatalogId { get; set; }
        public string CatalogName { get; set; }
    }
}
