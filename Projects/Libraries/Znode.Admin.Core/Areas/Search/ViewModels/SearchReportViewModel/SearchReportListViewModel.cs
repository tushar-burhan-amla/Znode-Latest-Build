using System.Collections.Generic;

using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchReportListViewModel : BaseViewModel
    {
        public List<SearchReportViewModel> SearchReportList { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public GridModel GridModel { get; set; }
    }
}
