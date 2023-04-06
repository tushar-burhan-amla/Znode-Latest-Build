using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ShippingListModel : BaseListModel
    {
        public List<ShippingModel> ShippingList { get; set; }
        public string ProfileName { get; set; }
        public string PortalName { get; set; }
    }
}
