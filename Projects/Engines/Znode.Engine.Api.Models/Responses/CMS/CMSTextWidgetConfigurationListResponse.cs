using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CMSTextWidgetConfigurationListResponse:BaseListResponse
    {
        public List<CMSTextWidgetConfigurationModel> TextWidgetConfigurationList { get; set; }
    }
}
