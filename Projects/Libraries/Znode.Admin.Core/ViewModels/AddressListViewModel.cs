using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddressListViewModel : BaseViewModel
    {
        public List<AddressViewModel> AddressList { get; set; }
        public GridModel GridModel { get; set; }
        public string AccountName { get; set; }
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsRoleAdministrator { get; set; }
        public int UserId { get; set; }
        public bool HasParentAccounts { get; set; }
        public string CustomerName { get; set; }
        public AddressViewModel BillingAddress { get; set; }

        public AddressViewModel ShippingAddress { get; set; }

    }
}