using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderInvoiceViewModel : BaseViewModel
    {
        public List<OrderViewModel> Orders { get; set; }
        public OrderInvoiceViewModel()
        {
            Orders = new List<OrderViewModel>();
        }
    }
}