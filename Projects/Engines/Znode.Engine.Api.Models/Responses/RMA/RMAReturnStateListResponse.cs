using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class RMAReturnStateListResponse : BaseListResponse
    {
        public List<RMAReturnStateModel> ReturnStates { get; set; }
    }
}
