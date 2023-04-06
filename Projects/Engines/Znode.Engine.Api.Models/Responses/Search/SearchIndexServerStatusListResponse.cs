using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchIndexServerStatusListResponse : BaseListResponse
    {
        public List<SearchIndexServerStatusModel> SearchIndexServerStatusList { get; set; }
    }
}
