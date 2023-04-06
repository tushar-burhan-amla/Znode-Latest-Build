namespace Znode.Engine.Api.Models
{
    public class CMSSearchIndexMonitorModel : BaseModel
    {
        public int CMSSearchIndexMonitorId { get; set; }
        public int CMSSearchIndexId { get; set; }
        public int SourceId { get; set; }
        public string SourceType { get; set; }
        public string SourceTransactionType { get; set; }
        public string AffectedType { get; set; }
        public string UserName { get; set; }
        public string ServerName { get; set; }
        public string Status { get; set; }
    }
}