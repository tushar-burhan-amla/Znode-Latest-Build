using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CaseStatusListResponse: BaseListResponse
    {
        public List<CaseStatusModel> CaseStatus { get; set; }
    }
}
