using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class InventoryWarehouseMapperListViewModel : BaseViewModel
    {
        public List<InventoryWarehouseMapperViewModel> InventoryWarehouseMapperList { get; set; }
        public GridModel GridModel { get; set; }
        public int InventoryListId { get; set; }
        public int WarehouseId { get; set; }
        public string ListName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string SKU { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public int InventoryId { get; set; }
        public string ProductName { get; set; }
    }
}