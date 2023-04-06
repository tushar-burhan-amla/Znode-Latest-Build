using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OmsDownloadableProductKeyModel : BaseModel
    {
        public int OmsDownloadableProductKeyId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public int PimDownloadableProductKeyId { get; set; }

        public List<OmsDownloadableProductKeyModel> OmsDownloadableProductKeyList { get; set; }
    }
}
