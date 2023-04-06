using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class TradeCentricCartModel
    {
        public decimal Total { get; set; }
        public List<TradeCentricCartItemModel> Items { get; set; } = new List<TradeCentricCartItemModel>();
    }
}
