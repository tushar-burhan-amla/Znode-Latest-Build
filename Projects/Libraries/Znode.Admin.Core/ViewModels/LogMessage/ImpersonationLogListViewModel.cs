using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ImpersonationLogListViewModel : BaseViewModel
    {
        public List<ImpersonationLogViewModel> ImpersonationActivityList { get; set; }
        public GridModel GridModel { get; set; }
        public string Component { get; set; }

        public ImpersonationLogListViewModel()
        {
            ImpersonationActivityList = new List<ImpersonationLogViewModel>();
        }
    }
}
