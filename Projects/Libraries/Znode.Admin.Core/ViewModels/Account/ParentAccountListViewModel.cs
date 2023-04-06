using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ParentAccountListViewModel : BaseViewModel
    {
        public List<ParentAccountViewModel> ParentAccountList { get; set; }

        public GridModel GridModel { get; set; }

        public ParentAccountListViewModel()
        {
            ParentAccountList = new List<ParentAccountViewModel>();
        }
    }
}
