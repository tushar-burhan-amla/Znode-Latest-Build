using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class InventoryWarehouseMapperListResponse : BaseListResponse
    {
        public List<InventoryWarehouseMapperModel> InventoryWarehouseMapperList { get; set; }
    }
}
