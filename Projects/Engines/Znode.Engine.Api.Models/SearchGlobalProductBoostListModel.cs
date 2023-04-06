using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchGlobalProductBoostListModel : BaseListModel
    {
        public List<SearchGlobalProductBoostModel> SearchGlobalProductBoostList { get; set; }

        public SearchGlobalProductBoostListModel()
        {
            SearchGlobalProductBoostList = new List<SearchGlobalProductBoostModel>();
        }
    }
}
