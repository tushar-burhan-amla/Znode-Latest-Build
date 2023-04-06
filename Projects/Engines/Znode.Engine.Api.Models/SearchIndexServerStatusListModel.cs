using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchIndexServerStatusListModel : BaseListModel
    {
        public List<SearchIndexServerStatusModel> SearchIndexServerStatusList { get; set; }
    }
}
