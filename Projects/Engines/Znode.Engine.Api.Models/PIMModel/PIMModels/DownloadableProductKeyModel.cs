using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DownloadableProductKeyModel : BaseModel
    {
        public int? ProductId { get; set; }
        public int PimDownloadableProductKeyId { get; set; }
        public string SKU { get; set; }
        public string DownloadableProductKey { get; set; }
        public string DownloadableProductURL { get; set; }
        public string Quantity { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDuplicate { get; set; }
        public int rowIndexId { get; set; }
        public string ProductName { get; set; }
        public List<DownloadableProductKeyModel> DownloadableProductKeyList { get; set; }
    }
}
