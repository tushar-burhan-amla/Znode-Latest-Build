using System;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Entities
{
    [Serializable()]
    public class ZnodeOrderShipment : ZnodeBusinessBase
    {
        public string SlNo { get; set; } = System.Guid.NewGuid().ToString();
        public int ShippingID { get; set; }
        public int AddressID { get; set; }
        public decimal Quantity { get; set; }
        public string ItemGUID { get; set; }
        public string ShippingName { get; set; }


        /// Generates the SlNo for the New ZnodeOrdershipment and assigned the values for AddressID,Quantity and item GUID
        public ZnodeOrderShipment(int addressId, decimal quantity, string itemGuid)
        {
            AddressID = addressId;
            Quantity = quantity;
            ItemGUID = itemGuid;
        }

        public ZnodeOrderShipment(int addressId, decimal quantity, string itemGuid, int shippingId, string shippingName)
        {
            AddressID = addressId;
            Quantity = quantity;
            ItemGUID = itemGuid;
            ShippingID = shippingId;
            ShippingName = shippingName;
        }
    }
}
