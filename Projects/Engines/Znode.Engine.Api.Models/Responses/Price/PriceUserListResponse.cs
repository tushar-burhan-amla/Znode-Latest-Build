using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PriceUserListResponse : BaseListResponse
    {
        public List<PriceUserModel> PriceUserList { get; set; }
    }
}
