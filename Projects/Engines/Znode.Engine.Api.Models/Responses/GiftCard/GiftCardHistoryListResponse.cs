using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class GiftCardHistoryListResponse : BaseListResponse
    {
        public List<GiftCardHistoryModel> GiftCardHistoryList { get; set; }
        public GiftCardModel GiftCard { get; set; }
    }
}
