using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PromotionListResponse : BaseListResponse
    {
        public List<PromotionModel> PromotionList { get; set; }
    }
}
