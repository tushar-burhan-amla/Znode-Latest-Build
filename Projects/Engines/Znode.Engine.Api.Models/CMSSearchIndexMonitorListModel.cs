using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSSearchIndexMonitorListModel : BaseListModel
    {
        public List<CMSSearchIndexMonitorModel> CMSSearchIndexMonitorList { get; set; }
        public int PortalId { get; set; }
        public int CMSSearchIndexId { get; set; }
    }
}
