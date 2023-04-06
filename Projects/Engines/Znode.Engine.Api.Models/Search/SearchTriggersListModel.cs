using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public  class SearchTriggersListModel : BaseListModel
    {
        public List<SearchTriggersModel> SearchTriggersList { get; set; }
    }
}
