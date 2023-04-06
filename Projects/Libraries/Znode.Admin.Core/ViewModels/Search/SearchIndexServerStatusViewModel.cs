namespace Znode.Engine.Admin.ViewModels
{
    public class SearchIndexServerStatusViewModel : BaseViewModel
    {
        public int SearchIndexServerStatusId { get; set; }
        public string ServerName { get; set; }
        public int SearchIndexMonitorId { get; set; }
        public string Status { get; set; }
    }
}