using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class UserApproverListViewModel : BaseViewModel
    {
        public List<UserApproverViewModel> UserApprover { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
    }
}
