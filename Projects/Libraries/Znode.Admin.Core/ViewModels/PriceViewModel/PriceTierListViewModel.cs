using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceTierListViewModel : BaseViewModel
    {
        public List<PriceTierViewModel> TierPriceList { get; set; }
        public GridModel GridModel { get; set; }
        public int PriceListId { get; set; }
    }
}