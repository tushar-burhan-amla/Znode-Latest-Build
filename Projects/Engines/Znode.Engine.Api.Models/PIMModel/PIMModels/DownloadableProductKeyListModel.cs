using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DownloadableProductKeyListModel : BaseListModel
    {
        public List<DownloadableProductKeyModel> DownloadableProductKeys { get; set; }

        public DownloadableProductKeyListModel()
        {
            DownloadableProductKeys = new List<DownloadableProductKeyModel>();
        }
    }
}
