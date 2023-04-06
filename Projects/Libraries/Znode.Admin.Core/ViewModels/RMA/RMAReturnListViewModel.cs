using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMAReturnListViewModel : BaseViewModel
    {
        public List<RMAReturnViewModel> ReturnList { get; set; }
        public GridModel GridModel { get; set; }
        public RMAReturnListViewModel()
        {
            ReturnList = new List<RMAReturnViewModel>();
            GridModel = new GridModel();
        }
        public string PortalName { get; set; }
        public int PortalId { get; set; }
    }
}
