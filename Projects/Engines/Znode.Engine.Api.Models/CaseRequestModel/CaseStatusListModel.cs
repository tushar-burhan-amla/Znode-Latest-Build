using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CaseStatusListModel : BaseListModel
    {
        public List<CaseStatusModel> CaseStatuses { get; set; }
    }
}
