using System;

namespace Znode.Libraries.Hangfire
{
    public class PortalIndexModel
    {
        public int CatalogIndexId { get; set; }
        public int PublishCatalogId { get; set; }
        public string IndexName { get; set; }
        public int SearchCreateIndexMonitorId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class PortalIndexResponse
    {
        public PortalIndexModel PortalIndex { get; set; }
    }
}
