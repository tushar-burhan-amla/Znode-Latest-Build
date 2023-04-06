using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class InventorySKUListViewModel : BaseViewModel
    {
        public List<InventorySKUViewModel> InventorySKUList { get; set; }
        public GridModel GridModel { get; set; }
        public int InventoryListId { get; set; }
        public string ListName { get; set; }
    }
}