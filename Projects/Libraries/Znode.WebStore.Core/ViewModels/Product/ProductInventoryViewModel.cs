using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ProductInventoryDetailViewModel : BaseViewModel
    {
        public string ProductName { get; set; }
        public List<InventorySKUViewModel> Inventory { get; set; }
    }
}
