using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class UsersListViewModel : BaseViewModel
    {
        public UsersListViewModel()
        {
            List = new List<UsersViewModel>();
            GridModel = new GridModel();
        }
        public List<UsersViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public int UserId { get; set; }
        
    }
}