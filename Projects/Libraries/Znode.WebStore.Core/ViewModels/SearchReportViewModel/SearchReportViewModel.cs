using Znode.Engine.WebStore;

namespace Znode.WebStore.Core.ViewModels
{
    public class SearchReportViewModel : BaseViewModel
    {
        public int SearchActivityId { get; set; }
        public int PortalId { get; set; }
        public int? UserId { get; set; }
        public int? UserProfileId { get; set; }
        public int? SearchProfileId { get; set; }
        public int NumberOfSearches { get; set; }
        public int ResultCount { get; set; }

        public string PortalName { get; set; }
        public string SearchKeyword { get; set; }
        public string TransformationKeyword { get; set; }
    }
}
