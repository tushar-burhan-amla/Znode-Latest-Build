using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class UserMap
    {
        // This method will map the ZnodeUser Entity to User Model.
        public static UserModel ToModel(ZnodeUser entity, string email)
        {
            if (HelperUtility.IsNotNull(entity))
            {
                UserModel model = entity.ToModel<UserModel>();
                model.Email = !Equals(email, string.Empty) ? email : entity.Email;
                model.FullName = $"{entity.FirstName } { entity.LastName }";
                model.User = new LoginUserModel() { UserId = entity.AspNetUserId, Email = !Equals(email, string.Empty) ? email : entity.Email, };
                model.WishList = entity.ZnodeUserWishLists?.ToModel<WishListModel>().ToList();
                model.Addresses = entity.ZnodeUserAddresses?.ToModel<AddressModel>().ToList();
                model.Profiles = entity.ZnodeUserProfiles?.ToModel<ProfileModel>().ToList();
                return model;
            }
            else
                return new UserModel();
        }

        // This method will map the ZnodeUser Entity to User Model.
        public static UserModel ToModel(ZnodeUser entity, string email, string userName)
        {
            UserModel userModel = ToModel(entity, email);
            if (HelperUtility.IsNotNull(userModel))
                userModel.UserName = userName;
            return userModel;
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
                    address.UserAddressId = entity.UserAddressId;
                    if (!Equals(entity.ZnodeUser, null))
                        address.IsGuest = Equals(entity.ZnodeUser.AspNetUserId, null);
                    return address;
                }
            }
            return null;
        }

        // This method will map the list of ZnodeUserAddress Entities to AddressListModel.
        public static AddressListModel ToAddressListModel(IList<ZnodeUserAddress> entities, PageListModel pageListModel)
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
            addressListModel.BindPageListModel(pageListModel);
            return addressListModel;
        }
    }
}
