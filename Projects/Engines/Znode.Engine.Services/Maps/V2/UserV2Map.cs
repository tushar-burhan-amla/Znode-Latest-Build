using System;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services.Maps
{
    public static class UserV2Map
    {
        public static UserModel ToUserModel(CreateUserModelV2 model)
        {
            UserModel userModel = new UserModel();
            if (HelperUtility.IsNotNull(model))
            {
                userModel.Email = model.Email;
                userModel.UserId = model.UserId;
                userModel.UserName = model.UserName;
                userModel.FirstName = model.FirstName;
                userModel.LastName = model.LastName;
                userModel.PhoneNumber = model.PhoneNo;
                userModel.AccountId = model.AccountId.Equals(0) ? (int?)null : model.AccountId;
                userModel.ProfileId = model.ProfileId;
                userModel.EmailOptIn = model.IsSendPeriodicEmail;
                userModel.PortalId = model.PortalId;
                userModel.IsWebStoreUser = true;
                userModel.User = new LoginUserModel
                {
                    Username = model.UserName,
                    Password = model.Password,
                    Email = model.Email,
                    LastPasswordChangedDate = DateTime.Now
                };
            }
            return userModel;
        }

        public static CreateUserModelV2 ToCreateUserModel(UserModel model)
        {
            CreateUserModelV2 createUserModel = new CreateUserModelV2();
            if (HelperUtility.IsNotNull(model))
            {
                createUserModel.Email = model.Email;
                createUserModel.UserId = model.UserId;
                createUserModel.UserName = model.UserName;
                createUserModel.FirstName = model.FirstName;
                createUserModel.LastName = model.LastName;
                createUserModel.PhoneNo = model.PhoneNumber;
                createUserModel.AccountId = model.AccountId == null ? 0 : model.AccountId.Value;
                createUserModel.ProfileId = model.ProfileId;
                createUserModel.IsSendPeriodicEmail = model.EmailOptIn;
                createUserModel.PortalId = model.PortalId == null ? 0 : model.PortalId.Value;
                createUserModel.Password = model.User?.Password;
            }
            return createUserModel;
        }

        public static UserModel ToUserModelUpdate(UpdateUserModelV2 model)
        {
            UserModel userModel = new UserModel();
            if (HelperUtility.IsNotNull(model))
            {
                userModel.Email = model.Email;
                userModel.UserId = model.UserId;
                userModel.FirstName = model.FirstName;
                userModel.LastName = model.LastName;
                userModel.PhoneNumber = model.PhoneNo;
                userModel.AccountId = model.AccountId.Equals(0) ? (int?)null : model.AccountId;
                userModel.ProfileId = model.ProfileId;
                userModel.EmailOptIn = model.IsSendPeriodicEmail;
                userModel.PortalId = model.PortalId;
                userModel.IsWebStoreUser = true;
                userModel.User = new LoginUserModel
                {
                    Email = model.Email
                };
            }
            return userModel;
        }

        public static GuestUserModelV2 ToGuestUserModel(int billingAddressId, int shippingAddressId, ZnodeUser account)
        {
            if (billingAddressId > 0 && shippingAddressId > 0 && IsNotNull(account))
            {
                return new GuestUserModelV2
                {
                    BillingAddressId = billingAddressId,
                    ShippingAddressId = shippingAddressId,
                    UserId = account.UserId,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    Email = account.Email,
                    PhoneNumber = account.PhoneNumber,
                    PortalId = Convert.ToInt32(account.ZnodeUserPortals?.FirstOrDefault()?.PortalId),
                    ProfileId = Convert.ToInt32(account.ZnodeUserProfiles?.FirstOrDefault()?.ProfileId)
                };
            }
            else
                return null;
        }
    }
}
