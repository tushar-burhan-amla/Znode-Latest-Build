using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class RMARequestListResponse : BaseListResponse
    {
        public List<RMARequestModel> RMARequestList { get; set; }
    }
}
