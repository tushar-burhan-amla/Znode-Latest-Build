using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceUserListViewModel : BaseViewModel
    {
        public List<PriceUserViewModel> PriceUserList { get; set; } 
        public GridModel GridModel { get; set; }
        public int PriceListId { get; set; }
        public string ListName { get; set; }
    }
}