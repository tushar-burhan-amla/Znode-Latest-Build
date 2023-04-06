using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalApproverListViewModel : BaseViewModel
    {
        public List<PortalApprovalViewModel> PortalApproverSettings { get; set; }
        public GridModel GridModel { get; set; }

    }
}
