using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PIMAttributeFamilyListResponse : BaseListResponse
    {
        public List<PIMAttributeFamilyModel> PIMAttributeFamilies { get; set; }
    }
}
