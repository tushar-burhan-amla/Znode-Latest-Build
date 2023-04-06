using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.WebStore;

namespace Znode.Engine.WebStore.ViewModels
{
    public class PaymentHistoryListViewModel : BaseViewModel
    {

        public PaymentHistoryListViewModel()
        {
            PaymentHistoryList = new List<PaymentHistoryViewModel>();
        }

        public List<PaymentHistoryViewModel> PaymentHistoryList { get; set; }
    }
}
