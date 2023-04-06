using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ExportPriceListResponse : BaseListResponse
    {
        public List<ExportPriceModel> ExportPriceList { get; set; }
        public List<InventorySKUModel> ExportInventoryList { get; set; }
    }
}
