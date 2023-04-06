using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Admin.ViewModels
{
    public class AssociatedPublishedBundleProductViewModel : BaseViewModel
    {
        public int PublishProductId { get; set; }
        public List<PublishAttributeViewModel> Attributes { get; set; }
        public string SKU { get; set; }
        public string InventoryMessage { get; set; }
        public bool ShowAddToCart { get; set; }
        public decimal? MinQuantity { get; set; }
        public decimal? MaxQuantity { get; set; }
        public bool? AllowBackOrder { get; set; }
        public bool? TrackInventory { get; set; }
        public string ProductType { get; set; }
        public string Name { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public decimal AssociatedQuantity { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public int ParentPublishProductId { get; set; }
        public string ParentBundleSKU { get; set; }
    }
}
