using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.WebStore.ViewModels
{
    public class QuickOrderViewModel : BaseViewModel
    {
        public QuickOrderViewModel()
        {
            ProductDetail = new List<QuickOrderProductViewModel>();
        }
        public string ProductSKUText { get; set; }
        public int ValidSKUCount { get; set; }
        public int InvalidSKUCount { get; set; }
        public bool IsSuccess { get; set; }
        public string NotificationMessage { get; set; }
        public List<QuickOrderProductViewModel> ProductDetail { get; set; }
        public string Name { get; set; }        
        public int Id { get; set; }
    }
}
