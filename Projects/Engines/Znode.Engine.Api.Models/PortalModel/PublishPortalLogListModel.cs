using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishPortalLogListModel : BaseListModel
    {
        public List<PublishPortalLogModel> PublishPortalLogList { get; set; }
        public string StoreName { get; set; }
    }
}
