using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class DashboardItemsListViewModel : BaseViewModel
    {
        public DashboardItemsListViewModel()
        {
            DashboardViewModelList = new List<DashboardItemsViewModel>();
        }
        public List<DashboardItemsViewModel> DashboardViewModelList { get; set; }
    }
}
