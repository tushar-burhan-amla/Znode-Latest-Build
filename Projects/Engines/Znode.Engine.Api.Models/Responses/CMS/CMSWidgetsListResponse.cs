using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CMSWidgetsListResponse : BaseListResponse
    {
        public List<CMSWidgetsModel> CMSWidgetsList { get; set; }
    }
}
