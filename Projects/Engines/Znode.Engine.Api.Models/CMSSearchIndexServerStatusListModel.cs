using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSSearchIndexServerStatusListModel : BaseListModel
    {
        public List<CMSSearchIndexServerStatusModel> CmsSearchIndexServerStatusList { get; set; }
    }
}