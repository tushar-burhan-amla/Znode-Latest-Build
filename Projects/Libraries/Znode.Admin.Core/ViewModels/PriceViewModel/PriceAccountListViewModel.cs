using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceAccountListViewModel : BaseViewModel
    {
        public List<PriceAccountViewModel> PriceAccountList { get; set; }
        public GridModel GridModel { get; set; }
        public int PriceListId { get; set; }
        public string ListName { get; set; }
    }
}
   