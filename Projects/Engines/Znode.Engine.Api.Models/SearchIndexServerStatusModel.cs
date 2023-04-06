namespace Znode.Engine.Api.Models
{
    public class SearchIndexServerStatusModel : BaseModel
    {
        public int SearchIndexServerStatusId { get; set; }
        public string ServerName { get; set; }
        public int SearchIndexMonitorId { get; set; }
        public string Status { get; set; }
    }
}
