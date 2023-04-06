using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class DashboardListViewModel : BaseViewModel
    {
        public DashboardListViewModel()
        {
            DashboardViewModelList = new List<DashboardTopItemsViewModel>();
        }
        public List<DashboardTopItemsViewModel> DashboardViewModelList { get; set; }
    }
}
