using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AccountDataViewModel : BaseViewModel
    {
        public AccountViewModel CompanyAccount { get; set; }

        public GridModel GridModel { get; set; }

        public int AccountId { get; set; }
        public bool HasParentAccounts { get; set; }
    }
}