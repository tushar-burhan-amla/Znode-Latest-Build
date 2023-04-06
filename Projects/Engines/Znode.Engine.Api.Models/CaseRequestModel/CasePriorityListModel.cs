using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CasePriorityListModel : BaseListModel
    {
        public List<CasePriorityModel> CasePriorities { get; set; }
    }
}
