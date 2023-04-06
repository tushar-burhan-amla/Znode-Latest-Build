using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class TradeCentricCartViewModel : BaseViewModel
    {       
        public decimal Total { get; set; }
        public List<TradeCentricCartItemViewModel> Items { get; set; } = new List<TradeCentricCartItemViewModel>();
    }
}
