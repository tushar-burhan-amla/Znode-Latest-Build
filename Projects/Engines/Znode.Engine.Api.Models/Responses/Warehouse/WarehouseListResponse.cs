using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WarehouseListResponse : BaseListResponse
    {
        public List<WarehouseModel> WarehouseList { get; set; }
    }
}
