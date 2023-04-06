using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CasePriorityListResponse : BaseListResponse
    {
        public List<CasePriorityModel> CasePriorities { get; set; }
    }
}
