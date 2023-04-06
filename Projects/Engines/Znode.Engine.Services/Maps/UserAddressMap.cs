using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class UserAddressMap
    {
        // This method will map the list of ZnodeUserAddress Entities to AddressListModel.
        public static AddressListModel ToListModel(IList<ZnodeUserAddress> entities)
        {
            AddressListModel addressListModel = new AddressListModel();

            if (entities?.Count > 0)
            {
                addressListModel.AddressList = new List<AddressModel>();
                foreach (ZnodeUserAddress userAddress in entities)
                {
                    AddressModel address = ToModel(userAddress);
                    //Skip the null entries
                    if (HelperUtility.IsNotNull(address))
                        addressListModel.AddressList.Add(address);
                }
            }
            return addressListModel;
        }

        // This method will map the ZnodeUserAddress Entity to AddressModel.
        public static AddressModel ToModel(ZnodeUserAddress entity)
        {
            if (HelperUtility.IsNotNull(entity))
            {
                AddressModel address = entity.ZnodeAddress?.ToModel<AddressModel>();
                if (HelperUtility.IsNotNull(address))
                {
                    address.UserId = entity.UserId;
                    address.AspNetUserId = entity.ZnodeUser?.AspNetUserId;
                    address.UserAddressId = entity.UserAddressId;
                    address.EmailAddress = entity.ZnodeAddress?.EmailAddress;
                    address.StateCode = !string.IsNullOrEmpty(address.StateCode) ? address.StateCode : AddressHelper.GetStateCode(address.StateName, address.CountryName);
                    return address;
                }
            }
            return null;
        }
    }
}
