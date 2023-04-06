using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PricePortalListViewModel : BaseViewModel
    {
        public List<PricePortalViewModel> PricePortals { get; set; }
        public GridModel GridModel { get; set; }
        public int PriceListId { get; set; }
        public string ListName { get; set; }
    }
}