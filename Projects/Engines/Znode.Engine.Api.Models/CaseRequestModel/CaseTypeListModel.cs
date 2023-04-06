using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CaseTypeListModel : BaseListModel
    {
        public List<CaseTypeModel> CaseTypes { get; set; }
    }
}
