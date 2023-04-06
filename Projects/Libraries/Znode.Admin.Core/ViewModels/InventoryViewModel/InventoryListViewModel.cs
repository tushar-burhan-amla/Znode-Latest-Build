using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class InventoryListViewModel : BaseViewModel
    {
        public int InventoryListId { get; set; }
        public List<InventoryViewModel> InventoryList { get; set; }
        public GridModel GridModel { get; set; }
        public int WarehouseId { get; set; }
        public string ListCode { get; set; }
        public string ListName { get; set; }

        public InventoryListViewModel()
        {
            InventoryList = new List<InventoryViewModel>();
        }

    }
}
