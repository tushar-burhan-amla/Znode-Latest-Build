using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceProfileListViewModel : BaseViewModel
    {
        public int PriceListId { get; set; }
        public List<PriceProfileViewModel> PriceProfiles { get; set; }
        public GridModel GridModel { get; set; }
        public string ListName { get; set; }
    }
}