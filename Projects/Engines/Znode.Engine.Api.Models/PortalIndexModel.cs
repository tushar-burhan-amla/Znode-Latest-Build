namespace Znode.Engine.Api.Models
{
    public class PortalIndexModel : BaseModel
    {
        public int PortalIndexId { get; set; }
        public int CatalogIndexId { get; set; }
        public int PortalId { get; set; }
        public string IndexName { get; set; }
        public string StoreName { get; set; }
        public int SearchCreateIndexMonitorId { get; set; }

        public int PublishCatalogId { get; set; }

        public string CatalogName { get; set; }

        public PortalModel ZnodePortal { get; set; }
        public string RevisionType { get; set; }
        public bool DirectCalling { get; set; }

        // A new name for index creation when the index is already present.
        public string NewIndexName { get; set; }

        //To check whether preview production is enabled or not.
        public bool IsPreviewProductionEnabled { get; set; }

        //To specify whether to publish only draft products or not.
        public bool IsPublishDraftProductsOnly { get; set; }
    }
}
