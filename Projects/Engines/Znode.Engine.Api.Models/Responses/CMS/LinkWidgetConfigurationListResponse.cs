using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class LinkWidgetConfigurationListResponse : BaseListResponse
    {
        public List<LinkWidgetConfigurationModel> LinkWidgetConfigurationList { get; set; }
    }
}
          