using System;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Maps
{
    public class UserViewModelMap
    {
        //Convert Login View Model to User Model. Return model of type UserModel.
        public static UserModel ToLoginModel(LoginViewModel model)
        {
            UserModel userModel = model.ToModel<UserModel>();
            userModel.BaseUrl = (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority))) ? HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) : string.Empty;
            userModel.UserVerificationTypeCode = PortalAgent.CurrentPortal.UserVerificationTypeCode;
            return userModel;
        }

        // Convert User Model to Login View Model. Return model of type LoginViewModel.
        public static LoginViewModel ToLoginViewModel(UserModel model)
        {
            if (HelperUtility.IsNotNull(model?.User))
            {
                return new LoginViewModel()
                {
                    Username = string.IsNullOrEmpty(model.User.Username) ? model.UserName : model.User.Username,
                    Password = model.User.Password,
                    IsResetPassword = model.User.IsLockedOut,
                    PasswordResetToken = model.User.PasswordToken,
                    PublishCatalogId = model.PublishCatalogId
                };
            }
            return new LoginViewModel { HasError = true };
        }

        // Convert UserViewModel to User Model. Return model of type UserModel.
        public static UserModel ToUserModel(UserViewModel model)
        {
            return new UserModel()
            {
                UserId = model.UserId,
                Email = model.Email,
                User = new LoginUserModel()
                {
                    Username = model.UserName,
                    Email = model.Email,
                    UserId = model.AspNetUserId,
                },
                BaseUrl = model.BaseUrl,
                UserName = model.UserName,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                LocaleId = PortalAgent.LocaleId,
                UserVerificationTypeCode = PortalAgent.CurrentPortal.UserVerificationTypeCode,
            };
        }

        public static UserModel ToChangePasswordModel(ChangePasswordViewModel model)
        {
            var changePasswordModel = new UserModel()
            {
                User = new LoginUserModel()
                {
                    Username = model.UserName,
                    Password = model.OldPassword,
                    NewPassword = model.NewPassword,
                    PasswordToken = model.PasswordToken,
                },
                PortalId = PortalAgent.CurrentPortal.PortalId,
            };

            return changePasswordModel;
        }

        public static UserModel ToSignUpModel(RegisterViewModel model)
        {
            var signUpModel = new UserModel()
            {
                User = new LoginUserModel()
                {
                    Username = model.UserName,
                    Password = model.Password,
                    Email = model.UserName,
                },
                EmailOptIn = model.EmailOptIn,
                IsWebStoreUser = model.IsWebStoreUser,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                LocaleId = PortalAgent.LocaleId,
                Email = model.Email,
                ExternalId = model.ExternalId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Custom1 = model.Custom1,
                Custom2 = model.Custom2,
                Custom3 = model.Custom3,
                Custom4 = model.Custom4,
                Custom5 = model.Custom5,
                UserVerificationTypeCode = PortalAgent.CurrentPortal.UserVerificationTypeCode,
                BaseUrl = model.BaseUrl,
                IsTradeCentricUser = model.IsTradeCentricUser
            };

            return signUpModel;
        }

    }
}