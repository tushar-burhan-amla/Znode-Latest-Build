using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class RequestStatusListViewModel : BaseViewModel
    {
        public List<RequestStatusViewModel> RequestStatusList { get; set; }
        public GridModel GridModel { get; set; }
    }
}