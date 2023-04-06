using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class RequestStatusListResponse : BaseListResponse
    {
        public List<RequestStatusModel> RequestStatusList { get; set; }
    }
}
