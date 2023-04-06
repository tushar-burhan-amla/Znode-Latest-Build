using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class InventorySKUListModel : BaseListModel
    {
        public List<InventorySKUModel> InventorySKUList { get; set; }
    }
}
