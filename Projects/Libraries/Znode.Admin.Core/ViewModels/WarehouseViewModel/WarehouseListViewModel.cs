using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class WarehouseListViewModel : BaseViewModel
    {
        public List<WarehouseViewModel> WarehouseList { get; set; }
        public GridModel GridModel { get; set; }
        public int InventoryListId { get; set; }
        public int WarehouseId { get; set; }

        public WarehouseListViewModel()
        {
            WarehouseList = new List<WarehouseViewModel>();
        }
    }
}