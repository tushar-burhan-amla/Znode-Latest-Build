using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalProfileShippingListModel : BaseListModel
    {
        public List<PortalProfileShippingModel> Shippings { get; set; }
    }
}
