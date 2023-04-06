using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class StoreListViewModel : BaseViewModel
    {
        public int PriceListId { get; set; }
        public StoreListViewModel()
        {
            StoreList = new List<StoreViewModel>();
        }

        public List<StoreViewModel> StoreList { get; set; }
        public GridModel GridModel { get; set; }
        public int TaxClassId { get; set; }
        public int ShippingId { get; set; }
        public int SearchProfileId { get; set; }
    }
}