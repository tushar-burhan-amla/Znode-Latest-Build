using System;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;

namespace Znode.Engine.Admin.Maps
{
    public class UserViewModelMap
    {

        //Convert Login View Model to User Model. Return model of type AccountModel.
        public static UserModel ToLoginModel(LoginViewModel model)
        {
            UserModel accountModel = model.ToModel<UserModel>();
            accountModel.BaseUrl = (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority))) ? HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) : string.Empty;
            return accountModel;
        }

        // Convert User Model to Login View Model. Return model of type LoginViewModel.
        public static LoginViewModel ToLoginViewModel(UserModel model)
        {
            return new LoginViewModel()
            {
                Username = model.User.Username,
                Password = model.User.Password,               
                IsResetPassword = model.User.IsLockedOut,
                PasswordResetToken = model.User.PasswordToken,
            };
        }

        // Convert ChangePassword View Model to User Model. Return model of type UserModel.
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
                }
            };

            return changePasswordModel;
        }

        // Convert UserViewModel to User Model. Return model of type UserModel.
        public static UserModel ToAccountModel(UserViewModel model)
        {
            return new UserModel()
            {
                UserId = model.UserId,
                Email = model.EmailAddress,
                User = new LoginUserModel()
                {
                    Username = model.UserName,
                    Email = model.EmailAddress,
                    UserId = model.AspNetUserId,                 
                },
                BaseUrl = model.BaseUrl,
                UserName = model.UserName,
            };
        }

        // Convert User Model to User View Model. Return model of type UserViewModel.
        public static UserViewModel ToAccountViewModel(UserModel model)
        {
            var accountViewModel = new UserViewModel()
            {
                UserId = model.UserId,
                EmailAddress = Equals(model.User, null) ? model.Email : model.User.Email,
                UserName = Equals(model.User, null) ? string.Empty : model.User.Username,
                AspNetUserId = Equals(model.User, null) ? string.Empty : model.User.UserId,
                RoleId = model.User?.RoleId ?? string.Empty,
                RoleName = model.User?.RoleName ?? string.Empty,
            };

            return accountViewModel;
        }
    }
}