using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreCaseRequestListResponse : BaseListResponse
    {
        public List<WebStoreCaseRequestModel> CaseRequests { get; set; }
    }
}
