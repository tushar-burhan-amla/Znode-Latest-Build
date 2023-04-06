namespace Znode.Engine.Admin.ViewModels
{
    public class CMSSearchIndexMonitorViewModel : BaseViewModel
    {
        public int CMSSearchIndexMonitorId { get; set; }
        public int SourceId { get; set; }
        public string SourceType { get; set; }
        public string SourceTransactionType { get; set; }
        public string AffectedType { get; set; }
        public string UserName { get; set; }
        public string ServerName { get; set; }
        public string Status { get; set; }
        public string CreatedDateWithTime { get; set; }
        public string ModifiedDateWithTime { get; set; }
    }
}