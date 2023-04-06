using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class TradeCentricOrderViewModel
    {
        public TradeCentricOrderDetailsViewModel Details { get; set; }
        public List<TradeCentricCartItemViewModel> Items { get; set; } = new List<TradeCentricCartItemViewModel>();
        public HeaderViewModel Header { get; set; }
    }
}
