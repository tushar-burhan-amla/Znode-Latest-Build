using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.WebStore.ViewModels
{
    public class QuickOrderProductViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public int Id { get; set; }
        public decimal CartQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public string ProductType { get; set; }
        public string AddOnProductSkus { get; set; }
        public decimal? RetailPrice { get; set; }
        public string GroupProductSKUs { get; set; }
        public int GroupProductsQuantity { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public string AutoAddonSKUs { get; set; }
        public string InventoryCode { get; set; }
        public string IsObsolete { get; set; }
        public bool? IsCallForPricing { get; set; }
        public string TrackInventory { get; set; }
        public string OutOfStockMessage { get; set; }
        public decimal? MaxQuantity { get; set; }
        public decimal? MinQuantity { get; set; }
        public bool IsActive { get; set; }      
        public bool IsPersonalizable { get; set; }
        public int UniqueId { get; set; }
        public string InventoryMessage { get; set; }
        public decimal? QuantityOnHand { get; set; }
        public string ChildTrackInventory { get; set; }
        public List<AssociatedPublishBundleProductViewModel> PublishBundleProducts { get; set; }
    }
}
