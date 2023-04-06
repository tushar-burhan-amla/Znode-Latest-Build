using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ShippingTypeListModel : BaseListModel
    {
        public ShippingTypeListModel()
        {
            ShippingTypeList = new List<ShippingTypeModel>();
        }

        public List<ShippingTypeModel> ShippingTypeList { get; set; }
    }
}
