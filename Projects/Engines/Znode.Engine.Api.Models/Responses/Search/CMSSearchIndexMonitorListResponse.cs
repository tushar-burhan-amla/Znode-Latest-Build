using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CMSSearchIndexMonitorListResponse : BaseListResponse
    {
        public List<CMSSearchIndexMonitorModel> CMSSearchIndexMonitorList { get; set; }
        public int PortalId { get; set; }
        public int CMSSearchIndexId { get; set; }
    }
}
