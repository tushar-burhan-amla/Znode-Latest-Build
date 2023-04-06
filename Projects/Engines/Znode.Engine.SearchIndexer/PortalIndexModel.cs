using System;

namespace Znode.Engine.SearchIndexer
{
    public class PortalIndexResponse
    {
        public PortalIndexModel PortalIndex { get; set; }
    }

    public class PortalIndexModel
    {
        public int PortalIndexId { get; set; }
        public int PortalId { get; set; }
        public string IndexName { get; set; }
        public int SearchCreateIndexMonitorId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
