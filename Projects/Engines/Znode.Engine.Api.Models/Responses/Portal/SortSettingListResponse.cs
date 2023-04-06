using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SortSettingListResponse : BaseListResponse
    {
        public List<PortalSortSettingModel> SortSettings { get; set; }
    }
}
