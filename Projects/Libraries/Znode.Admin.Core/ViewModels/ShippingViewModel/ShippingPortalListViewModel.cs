using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ShippingPortalListViewModel : BaseViewModel
    {
        public List<ShippingPortalViewModel> ShippingPortalList { get; set; }
        public GridModel GridModel { get; set; }
        public int ShippingId { get; set; }
        public int ShippingTypeId { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
    }
}