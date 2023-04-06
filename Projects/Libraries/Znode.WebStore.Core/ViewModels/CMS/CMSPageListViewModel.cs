using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CMSPageListViewModel : BaseViewModel
    {
        public List<CMSPageViewModel> CMSPages { get; set; }

        public string SearchResultCountText { get; set; }

        public int TotalCMSPageCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string SearchKeyword { get; set; }
    }
}

