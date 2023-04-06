using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PriceProfileListResponse : BaseListResponse
    {
        public List<PriceProfileModel> PriceProfileList { get; set; }
    }
}
