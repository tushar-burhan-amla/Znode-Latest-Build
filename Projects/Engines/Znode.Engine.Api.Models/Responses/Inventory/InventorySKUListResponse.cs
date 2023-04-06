using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class InventorySKUListResponse : BaseListResponse
    {
        public List<InventorySKUModel> InventorySKUList { get; set; }
    }
}
