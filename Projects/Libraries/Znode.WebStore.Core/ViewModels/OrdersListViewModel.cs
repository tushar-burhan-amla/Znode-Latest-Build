using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    //ToDo
    public class OrdersListViewModel : BaseViewModel
    {
        public List<OrdersViewModel> List { get; set; }
        public OrdersListViewModel()
        {
            List = new List<OrdersViewModel>();
            GridModel = new GridModel();
        }

        public bool HasParentAccounts { get; set; }
        public string AccountName { get; set; }
        public GridModel GridModel { get; set; }
        public int? AccountId { get; set; }
        public int UserId { get; set; }
    }
}