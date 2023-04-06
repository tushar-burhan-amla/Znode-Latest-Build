using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class UomListResponse : BaseListResponse
    {
        public List<UomModel> UomList { get; set; }
    }
}
