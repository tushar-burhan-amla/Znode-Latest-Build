using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchIndexMonitorListResponse : BaseListResponse
    {
        public List<SearchIndexMonitorModel> SearchIndexMonitorList { get; set; }
    }
}
