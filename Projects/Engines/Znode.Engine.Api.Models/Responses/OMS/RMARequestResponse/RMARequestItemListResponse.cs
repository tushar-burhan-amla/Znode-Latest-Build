using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class RMARequestItemListResponse : BaseListResponse
    {
        public List<RMARequestItemModel> RMARequestItems { get; set; }
    }
}
