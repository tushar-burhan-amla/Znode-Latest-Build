using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ShippingServiceCodeListModel : BaseListModel
    {
        public ShippingServiceCodeListModel()
        {
            ShippingServiceCodes = new List<ShippingServiceCodeModel>();
        }
        public List<ShippingServiceCodeModel> ShippingServiceCodes { get; set; }
    }
}
