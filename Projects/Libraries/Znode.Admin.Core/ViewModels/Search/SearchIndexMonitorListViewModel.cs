using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchIndexMonitorListViewModel : BaseViewModel
    {
        public GridModel GridModel { get; set; }

        public List<SearchIndexMonitorViewModel> SearchIndexMonitorList { get; set; }

        public int CatalogIndexId { get; set; }

        public int PublishCatalogId { get; set; }
    }
}