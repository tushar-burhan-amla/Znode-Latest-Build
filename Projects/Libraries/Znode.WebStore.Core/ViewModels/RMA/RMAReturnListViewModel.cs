using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class RMAReturnListViewModel : BaseViewModel
    {
        public List<RMAReturnViewModel> ReturnList { get; set; }
        public RMAReturnListViewModel()
        {
            ReturnList = new List<RMAReturnViewModel>();
            GridModel = new GridModel();
        }
        public GridModel GridModel { get; set; }
    }
}