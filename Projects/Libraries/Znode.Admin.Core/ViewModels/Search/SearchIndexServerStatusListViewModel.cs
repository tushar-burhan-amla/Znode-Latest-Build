using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchIndexServerStatusListViewModel : BaseViewModel
    {
        public GridModel GridModel { get; set; }

        public List<SearchIndexServerStatusViewModel> SearchIndexServerStatusList { get; set; }

        public int SearchIndesMonitorId { get; set; }
    }
}