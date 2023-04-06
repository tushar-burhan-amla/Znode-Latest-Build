using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PromotionTypeListResponse : BaseListResponse
    {
        public List<PromotionTypeModel>  PromotionTypeList { get; set; }
    }
}
