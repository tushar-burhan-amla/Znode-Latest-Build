
using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{

    public class VoucherListViewModel : BaseViewModel
    {
        public List<VoucherViewModel> List { get; set; }
        public VoucherListViewModel()
        {
            List = new List<VoucherViewModel>();
            GridModel = new GridModel();
        }

        public GridModel GridModel { get; set; }
    }
}