using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TouchPointConfigurationListResponse : BaseListResponse
    {
        public List<TouchPointConfigurationModel> TouchPointConfiguration { get; set; }
        public List<TouchPointConfigurationModel> SchedulerLog { get; set; }
    }
}
