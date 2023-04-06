using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ERPTaskSchedulerListViewModel : BaseViewModel
    {
        public List<ERPTaskSchedulerViewModel> ERPTaskSchedulerList { get; set; }
        public GridModel GridModel { get; set; }
        public ERPTaskSchedulerListViewModel()
        {
            ERPTaskSchedulerList = new List<ERPTaskSchedulerViewModel>();
        }
        public string ActiveERPClassName { get; set; }
    }
}