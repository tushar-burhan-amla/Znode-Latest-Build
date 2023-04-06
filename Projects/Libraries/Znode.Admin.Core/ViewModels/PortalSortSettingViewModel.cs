namespace Znode.Engine.Admin.ViewModels
{
    public class PortalSortSettingViewModel : BaseViewModel
    {
        public int PortalSortSettingId { get; set; }
        public int PortalId { get; set; }
        public int SortSettingId { get; set; }
        public string SortName { get; set; }
        public string SortDisplayName { get; set; }
        public string SortValue { get; set; }
        public int DisplayOrder { get; set; }        
    }
}