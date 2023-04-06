using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AccountDataViewModel : BaseViewModel
    {
        public AccountViewModel CompanyAccount { get; set; }

        public GridModel GridModel { get; set; }

        public int AccountId { get; set; }
        public bool HasParentAccounts { get; set; }
    }
}