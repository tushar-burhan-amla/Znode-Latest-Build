using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProfileListViewModel : BaseViewModel
    {
        public int PriceListId { get; set; }
        public List<ProfileViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public int AccountProfileListCount { get; set; }
        public bool HasParentAccounts { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
    }
}