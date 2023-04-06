using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ImpersonationActivityListResponse : BaseListResponse
    {
        public List<ImpersonationActivityLogModel> LogActivityList { get; set; }
    }
}
