using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrdersListViewModel : BaseViewModel
    {
        public OrdersListViewModel()
        {
            List = new List<OrderViewModel>();
            GridModel = new GridModel();
        }

        public List<OrderViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public int? AccountId { get; set; }
        public bool HasParentAccounts { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public int UserId { get; set; }
        public string UpdatePageType { get; set; }
        public string PortalName { get; set; }
        public int PortalId { get; set; }
    }
}