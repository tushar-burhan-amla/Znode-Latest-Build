using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AccountListViewModel:BaseViewModel
    {
        public List<AccountViewModel> AccountList { get; set; }

        public GridModel GridModel { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public bool HasParentAccounts { get; set; }
    }
}