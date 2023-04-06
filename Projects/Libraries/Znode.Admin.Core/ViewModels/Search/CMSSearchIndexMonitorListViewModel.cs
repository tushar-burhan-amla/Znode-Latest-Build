using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSSearchIndexMonitorListViewModel : BaseViewModel
    {
        public GridModel GridModel { get; set; }

        public List<CMSSearchIndexMonitorViewModel> CMSSearchIndexMonitorList { get; set; }

        public int CMSSearchIndexId { get; set; }

        public int PortalId { get; set; }
    }
}
