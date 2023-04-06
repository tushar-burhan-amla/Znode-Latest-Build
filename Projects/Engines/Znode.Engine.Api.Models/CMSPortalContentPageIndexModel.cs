namespace Znode.Engine.Api.Models
{
    public class CMSPortalContentPageIndexModel : BaseModel
    {
        public int CMSSearchIndexId { get; set; }
        public int PortalId { get; set; }
        public string IndexName { get; set; }
        public string StoreName { get; set; }
        public int CMSSearchIndexMonitorId { get; set; }        
        public string RevisionType { get; set; }
        public bool IsFromStorePublish { get; set; }
        public bool IsDisabledCMSPageResults { get; set; }
    }
}
