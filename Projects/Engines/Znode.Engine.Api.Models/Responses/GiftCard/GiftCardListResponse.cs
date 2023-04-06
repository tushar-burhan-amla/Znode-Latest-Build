using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class GiftCardListResponse : BaseListResponse
    {
        public List<GiftCardModel> GiftCardList { get; set; }
        public bool ReferralCommissionCount { get; set; }
    }
}
