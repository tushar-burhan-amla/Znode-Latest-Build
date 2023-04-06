using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AccountQuoteListModel : BaseListModel
    {
        public List<AccountQuoteModel> AccountQuotes { get; set; }
        public string CustomerName { get; set; }
        public string AccountName { get; set; }
        public bool HasParentAccounts { get; set; }
    }
}
