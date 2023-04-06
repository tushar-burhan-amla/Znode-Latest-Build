using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishCatalogLogListModel : BaseListModel
    {
        public List<PublishCatalogLogModel> PublishCatalogLogList { get; set; }
    }
}
