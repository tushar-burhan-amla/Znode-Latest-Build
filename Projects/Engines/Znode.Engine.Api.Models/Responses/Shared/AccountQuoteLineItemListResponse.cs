using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AccountQuoteLineItemListResponse : BaseListResponse
    {
        public List<AccountQuoteLineItemModel> AccountQuoteLineItems { get; set; }
    }
}
