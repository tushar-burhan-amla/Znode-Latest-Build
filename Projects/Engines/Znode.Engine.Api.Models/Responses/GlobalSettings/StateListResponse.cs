using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class StateListResponse : BaseListResponse
    {
        public List<StateModel> States { get; set; }
    }
}
