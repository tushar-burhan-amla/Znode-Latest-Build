using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AccountQuoteLineItemListViewModel : BaseViewModel
    {
        public string GroupId { get; set; }
        public List<AccountQuoteLineItemViewModel> AccountQuoteLineItems { get; set; }
    }
}