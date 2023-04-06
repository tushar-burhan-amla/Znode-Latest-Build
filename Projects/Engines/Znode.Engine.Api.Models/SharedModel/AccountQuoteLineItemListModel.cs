using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AccountQuoteLineItemListModel : BaseListModel
    {
        public List<AccountQuoteLineItemModel> AccountQuoteLineItems { get; set; }
    }
}
