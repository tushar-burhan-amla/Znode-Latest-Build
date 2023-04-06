using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class AddressMap
    {
        public static AddressListModel ToListModel(IList<ZnodeAddress> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new AddressListModel();
                model.AddressList = new List<AddressModel>();
                foreach (var item in entity)
                {
                    model.AddressList.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        public static AddressModel ToModel(ZnodeAddress entity)
        {
            if (!Equals(entity, null))
            {
                return entity.ToModel<AddressModel>();
            }
            else
                return null;
        }

    }
}
