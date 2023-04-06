using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DownloadableProductKeyListResponse : BaseListResponse
    {
        public List<DownloadableProductKeyModel> DownloadableProductKeys { get; set; }
    }
}
