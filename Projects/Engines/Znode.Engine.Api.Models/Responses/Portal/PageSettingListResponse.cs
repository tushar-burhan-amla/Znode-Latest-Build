using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PageSettingListResponse : BaseListResponse
    {
        public List<PortalPageSettingModel> PageSettings { get; set; }
    }
}
