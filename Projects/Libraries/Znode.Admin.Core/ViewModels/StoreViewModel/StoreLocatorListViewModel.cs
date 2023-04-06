using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class StoreLocatorListViewModel:BaseViewModel
    {
        public List<StoreLocatorDataViewModel> StoreLocatorList { get; set; }
        public GridModel GridModel { get; set; }
    }
}