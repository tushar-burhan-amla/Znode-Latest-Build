using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WarehouseListModel : BaseListModel
    {
        public List<WarehouseModel> WarehouseList { get; set; }

        public WarehouseListModel()
        {
            WarehouseList = new List<WarehouseModel>();
        }
    }
}
