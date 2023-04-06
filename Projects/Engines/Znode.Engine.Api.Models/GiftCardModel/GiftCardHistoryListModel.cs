using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GiftCardHistoryListModel : BaseListModel
    {
        public GiftCardHistoryListModel()
        {
            GiftCardHistoryList = new List<GiftCardHistoryModel>();
            GiftCard = new GiftCardModel();
        }
        public List<GiftCardHistoryModel> GiftCardHistoryList { get; set; }
        public GiftCardModel GiftCard { get; set; }
    }
}
