using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchTriggersListResponse : BaseListResponse
    {
        public List<SearchTriggersModel> SearchTriggersList { get; set; }
    }
}
