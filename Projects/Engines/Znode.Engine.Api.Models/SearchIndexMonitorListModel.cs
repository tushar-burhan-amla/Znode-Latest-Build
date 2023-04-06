using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchIndexMonitorListModel : BaseListModel
    {
        public List<SearchIndexMonitorModel> SearchIndexMonitorList { get; set; }
    }
}
