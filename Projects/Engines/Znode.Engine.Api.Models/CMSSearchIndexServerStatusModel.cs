namespace Znode.Engine.Api.Models
{
    public class CMSSearchIndexServerStatusModel : BaseModel
    {
        public int CMSSearchIndexServerStatusId { get; set; }
        public string ServerName { get; set; }
        public int CMSSearchIndexMonitorId { get; set; }
        public string Status { get; set; }
    }
}
