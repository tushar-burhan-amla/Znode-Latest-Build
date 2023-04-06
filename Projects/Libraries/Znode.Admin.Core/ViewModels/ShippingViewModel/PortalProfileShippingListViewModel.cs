using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalProfileShippingListViewModel : BaseViewModel
    {
        public List<PortalProfileShippingViewModel> Shippings { get; set; }
        public GridModel GridModel { get; set; }

        public int ProfileId { get; set; }
        public int PortalId { get; set; }
    }
}