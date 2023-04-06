using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CustomerListViewModel : BaseViewModel
    {
        public CustomerListViewModel()
        {
            List = new List<CustomerViewModel>();
            GridModel = new GridModel();
        }
        public List<CustomerViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public int PriceListId { get; set; }
        public int PortalId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public bool HasParentAccounts { get; set; }
        public bool IsAccountCustomer { get; set; }
    }
}