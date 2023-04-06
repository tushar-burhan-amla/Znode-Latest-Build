using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class DownloadableProductKeyListViewModel : BaseViewModel
    {
        public List<DownloadableProductKeyViewModel> DownloadableProductKeys { get; set; }
        public GridModel GridModel { get; set; }
        public int ProductId { get; set; }
        public int InventoryId { get; set; }
        public int PimProductId { get; set; }
        public string SKU { get; set; }
        public bool IsDownloadable { get; set; }
    }
}
