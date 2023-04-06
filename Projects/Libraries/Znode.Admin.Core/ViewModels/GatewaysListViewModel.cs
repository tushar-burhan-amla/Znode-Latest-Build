using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class GatewaysListViewModel : BaseViewModel
    {
        public GatewaysListViewModel()
        {
            List = new List<GatewaysViewModel>();
        }
        public List<GatewaysViewModel> List { get; set; }
    }
}