using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddOnViewModel : BaseViewModel
    {
        public string GroupName { get; set; }
        public string DisplayType { get; set; }
        public int AddOnId { get; set; }
        public bool IsRequired { get; set; }
        public bool IsAutoAddon { get; set; }
        public string Description { get; set; }
        public string AutoAddonSKUs { get; set; }
        public int?[] SelectedAddOnValue { get; set; }
        public bool TrackInventory { get; set; }
        public string Name { get; set; }
        public string InventoryMessage { get; set; }
        public bool IsOutOfStock { get; set; }
        public List<AddOnValuesViewModel> AddOnValues { get; set; }
    }
}