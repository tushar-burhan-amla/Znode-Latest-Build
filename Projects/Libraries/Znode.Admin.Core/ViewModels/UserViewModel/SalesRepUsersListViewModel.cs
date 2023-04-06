using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class SalesRepUsersListViewModel : BaseViewModel
    {
        public SalesRepUsersListViewModel()
        {
            List = new List<SalesRepUsersViewModel>();
            GridModel = new GridModel();
        }
        public List<SalesRepUsersViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public int? UserId { get; set; }
        
    }
}