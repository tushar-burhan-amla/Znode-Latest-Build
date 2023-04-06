using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class FailedOrderTransactionListViewModel : BaseViewModel
    {
        public FailedOrderTransactionListViewModel()
        {
            FailedOrderTransactionListVM = new List<FailedOrderTransactionViewModel>();
            GridModel = new GridModel();
        }

        public List<FailedOrderTransactionViewModel> FailedOrderTransactionListVM { get; set; }
        public GridModel GridModel { get; set; }
    }
}