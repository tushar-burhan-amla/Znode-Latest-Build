using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ERPTaskSchedulerListResponse : BaseListResponse
    {
        public List<ERPTaskSchedulerModel> ERPTaskScheduler { get; set; }
    }
}
