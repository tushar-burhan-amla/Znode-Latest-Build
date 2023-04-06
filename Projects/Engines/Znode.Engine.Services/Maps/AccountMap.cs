using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class AccountMap
    {
        // This method will map the list of ZnodeAccountAddress Entities to AddressListModel.
        public static AddressListModel ToListModel(IList<ZnodeAccountAddress> entities)
        {
            AddressListModel addressListModel = new AddressListModel();

            if (entities?.Count > 0)
            {
                addressListModel.AddressList = new List<AddressModel>();
                foreach (ZnodeAccountAddress accountAddress in entities)
                {
                    AddressModel address = ToModel(accountAddress);
                    //Skip the null entries
                    if (HelperUtility.IsNotNull(address))
                        addressListModel.AddressList.Add(address);
                }
            }
            return addressListModel;
        }

        // This method will map the ZnodeAccountAddress Entity to AddressModel.
        public static AddressModel ToModel(ZnodeAccountAddress entity)
        {
            if (HelperUtility.IsNotNull(entity))
            {
                AddressModel address = entity.ZnodeAddress?.ToModel<AddressModel>();
                if (HelperUtility.IsNotNull(address))
                {
                    address.AccountId = entity.AccountId;
                    address.AccountAddressId = entity.AccountAddressId;
                    address.StateCode = AddressHelper.GetStateCode(address.StateName, address.CountryName);
                    return address;
                }
            }
            return null;
        }
    }
}
