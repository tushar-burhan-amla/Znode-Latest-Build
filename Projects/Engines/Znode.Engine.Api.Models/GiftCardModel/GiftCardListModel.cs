using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GiftCardListModel : BaseListModel
    {
        public GiftCardListModel()
        {
            GiftCardList = new List<GiftCardModel>();
        }
        public List<GiftCardModel> GiftCardList { get; set; }
        public bool ReferralCommissionCount { get; set; }
    }
}
