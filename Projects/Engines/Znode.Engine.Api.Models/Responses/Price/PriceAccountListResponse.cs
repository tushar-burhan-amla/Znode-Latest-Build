using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PriceAccountListResponse : BaseListResponse
    {
        public List<PriceAccountModel> PriceAccountList { get; set; }
    }
}
