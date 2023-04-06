using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PaymentHistoryListViewModel: BaseViewModel
    {

        public PaymentHistoryListViewModel()
        {
            List = new List<PaymentHistoryViewModel>();
        }

        public List<PaymentHistoryViewModel> List { get; set; }
    }
}
