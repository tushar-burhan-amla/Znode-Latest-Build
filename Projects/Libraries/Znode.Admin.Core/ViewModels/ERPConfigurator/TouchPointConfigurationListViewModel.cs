using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class TouchPointConfigurationListViewModel : BaseViewModel
    {
        public List<TouchPointConfigurationViewModel> TouchPointConfigurationList { get; set; }
        public List<TouchPointConfigurationViewModel> SchedulerLogList { get; set; }
        public GridModel GridModel { get; set; }
        public int TouchPointConfigurationId { get; set; }
        public TouchPointConfigurationListViewModel()
        {
            TouchPointConfigurationList = new List<TouchPointConfigurationViewModel>();
            SchedulerLogList = new List<TouchPointConfigurationViewModel>();
        }
        public string ActiveERPClassName { get; set; }
        public string ERPClassName { get; set; }
        public string SchedulerName { get; set; }
    }
}