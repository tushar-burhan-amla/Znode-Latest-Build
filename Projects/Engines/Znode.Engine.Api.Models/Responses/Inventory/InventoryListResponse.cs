using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class InventoryListResponse : BaseListResponse
    {
        public List<InventoryModel> InventoryList { get; set; }
    }
}
