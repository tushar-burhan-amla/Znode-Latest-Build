using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class InventoryListModel : BaseListModel
    {
        public List<InventoryModel> InventoryList { get; set; }

        public InventoryListModel()
        {
            InventoryList = new List<InventoryModel>();
        }
    }
}
