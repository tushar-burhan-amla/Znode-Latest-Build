using System.Collections.Generic;
namespace Znode.Engine.Api.Models.Responses
{
    public class CMSWidgetConfigurationListResponse : BaseListResponse
    {
        public List<CMSWidgetConfigurationModel> WidgetConfigurationList { get; set; }
    }
}
