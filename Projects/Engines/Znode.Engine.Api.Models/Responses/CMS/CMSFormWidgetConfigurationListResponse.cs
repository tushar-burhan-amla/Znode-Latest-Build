using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CMSFormWidgetConfigurationListResponse : BaseListResponse
    {
        public List<CMSFormWidgetConfigrationModel> FormWidgetConfigurationList { get; set; }
    }
}
