using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CaseTypeListResponse : BaseListResponse
    {
        public List<CaseTypeModel> CaseTypes { get; set; }
    }
}
