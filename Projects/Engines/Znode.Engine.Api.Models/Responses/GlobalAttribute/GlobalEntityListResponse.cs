using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class GlobalEntityListResponse : BaseListResponse
    {
        public List<GlobalEntityModel> GlobalEntityList { get; set; }
    }
}
