using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMARequestListViewModel : BaseViewModel
    {
        public RMARequestListViewModel()
        {
            RMARequestList = new List<RMARequestViewModel>();
        }
        public List<RMARequestViewModel> RMARequestList { get; set; }
        public List<RequestStatusViewModel> RequestStatusList { get; set; }
        public List<PortalViewModel> PortalList { get; set; }
        public GridModel GridModel { get; set; }
    }
}