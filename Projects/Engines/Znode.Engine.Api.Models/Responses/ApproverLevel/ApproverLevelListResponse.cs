using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ApproverLevelListResponse : BaseListResponse
    {
        public List<ApproverLevelModel> ApproverLevelList { get; set; }        
    }
}

