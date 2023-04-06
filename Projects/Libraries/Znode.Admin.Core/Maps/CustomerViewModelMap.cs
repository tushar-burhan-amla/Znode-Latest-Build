using System;
using System.Web;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Maps
{
    public class CustomerViewModelMap
    {
        /// <summary>
        /// To convert CustomerViewModel to AccountModel.
        /// </summary>
        /// <param name="customerViewModel">CustomerViewModel</param>
        /// <returns>Returns AccountModel</returns>
        public static UserModel ToAccountFromCustomerViewModel(CustomerViewModel customerViewModel)
        {
            if (HelperUtility.IsNotNull(customerViewModel))
            {
                return new UserModel()
                {
                    UserId = customerViewModel.UserId,
                    Email = string.IsNullOrEmpty(customerViewModel.Email) ? customerViewModel.UserName.Trim() : customerViewModel.Email,
                    User = new LoginUserModel()
                    {
                        Email = string.IsNullOrEmpty(customerViewModel.Email) ? customerViewModel.UserName.Trim() : customerViewModel.Email,
                        Username = customerViewModel.UserName.Trim(),
                    },
                    UserName = customerViewModel.UserName.Trim(),
                    AspNetUserId = customerViewModel.AspNetUserId,
                    FirstName = customerViewModel.FirstName,
                    LastName = customerViewModel.LastName,
                    PhoneNumber = customerViewModel.PhoneNumber,
                    ProfileId = customerViewModel.ProfileId,
                    CompanyName = HttpUtility.HtmlEncode(customerViewModel.CompanyName),
                    EmailOptIn = customerViewModel.EmailOptIn,
                    SMSOptIn = customerViewModel.SMSOptIn,
                    ExternalId = string.IsNullOrEmpty(customerViewModel.ExternalId) ? null : HttpUtility.HtmlEncode(customerViewModel.ExternalId.Trim()),
                    Description = HttpUtility.HtmlEncode(customerViewModel.Description),
                    Website = HttpUtility.HtmlEncode(customerViewModel.Website),
                    Source = HttpUtility.HtmlEncode(customerViewModel.Source),
                    PortalIds = customerViewModel.IsSelectAllPortal ? new string[] { "0" } : customerViewModel.PortalIds,
                    Custom1 = (!Equals(customerViewModel.Custom1, null) ? HttpUtility.HtmlEncode(customerViewModel.Custom1.ToString().Trim()) : string.Empty),
                    Custom2 = (!Equals(customerViewModel.Custom2, null) ? HttpUtility.HtmlEncode(customerViewModel.Custom2.ToString().Trim()) : string.Empty),
                    Custom3 = (!Equals(customerViewModel.Custom3, null) ? HttpUtility.HtmlEncode(customerViewModel.Custom3.ToString().Trim()) : string.Empty),
                    RoleId = customerViewModel.RoleId,
                    ApprovalUserId = Convert.ToInt32(customerViewModel.ApprovalUserId),
                    PermissionCode = customerViewModel.PermissionCode,
                    BudgetAmount = customerViewModel.BudgetAmount,
                    RoleName = customerViewModel.RoleName,
                    AccountId = customerViewModel.AccountId,
                    PortalId = customerViewModel.PortalId,
                    AccountPermissionAccessId = customerViewModel.AccountPermissionAccessId.GetValueOrDefault(),
                    Accountname = customerViewModel.AccountName,
                    PermissionsName = customerViewModel.PermissionsName,
                    DepartmentName = customerViewModel.DepartmentName,
                    DepartmentId = customerViewModel.DepartmentId,
                    AccountUserOrderApprovalId = customerViewModel.AccountUserOrderApprovalId,
                    SalesRepId = customerViewModel.SalesRepId,
                };
            }
            return new UserModel();
        }

        /// <summary>
        /// To convert CustomerAccountViewModel to UserModel.
        /// </summary>
        /// <param name="customerAccountViewModel">CustomerAccountViewModel</param>
        /// <returns>Returns UserModel</returns>
        public static UserModel ToUserModel(CustomerAccountViewModel customerAccountViewModel)
        {
            UserModel userModel = ToAccountFromCustomerViewModel(customerAccountViewModel);
            userModel.Accountname = customerAccountViewModel.AccountName;
            userModel.PermissionsName = customerAccountViewModel.PermissionsName;
            userModel.DepartmentName = customerAccountViewModel.DepartmentName;
            userModel.RoleId = customerAccountViewModel.RoleId;
            userModel.DepartmentId = customerAccountViewModel.DepartmentId;
            userModel.AccountPermissionAccessId = Convert.ToInt32(customerAccountViewModel.AccountPermissionAccessId);
            userModel.AccountId = customerAccountViewModel.AccountId;
            userModel.RoleName = customerAccountViewModel.RoleName;
            userModel.PhoneNumber = customerAccountViewModel.PhoneNumber;
            userModel.PermissionCode = customerAccountViewModel.PermissionCode;
            userModel.ApprovalUserId = Convert.ToInt32(customerAccountViewModel.ApprovalUserId);
            userModel.AccountUserOrderApprovalId = customerAccountViewModel.AccountUserOrderApprovalId;
            return userModel;
        }

        /// <summary>
        /// This method converts CustomerViewModel to AccountModel
        /// </summary>
        /// <param name="model">CustomerViewModel model</param>
        /// <returns>AccountModel</returns>
        public static UserModel ToAdminUpdateAccountModel(CustomerViewModel model)
        {
            if (!Equals(model, null))
            {
                return new UserModel()
                {
                    UserName = model.UserName,
                    UserId = model.UserId,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CompanyName = model.CompanyName,
                    EmailOptIn = model.EmailOptIn,
                    SMSOptIn = model.SMSOptIn,
                    Custom1 = model.Custom1,
                    Custom2 = model.Custom2,
                    Custom3 = model.Custom3,
                    Description = model.Description,
                    Source = model.Source,
                    Website = model.Website,
                    ExternalId = model.ExternalId,
                    ProfileId = model.ProfileId,
                    AspNetUserId = model.AspNetUserId,
                    PermissionCode = model.PermissionCode,
                    RoleName = model.RoleName,
                    RoleId = model.RoleId,
                    PortalIds = model.IsSelectAllPortal ? new string[] { "0" } : model.PortalIds,
                    PortalId = model.PortalId,
                    AccountId = model.AccountId,
                    Accountname = model.AccountName,
                    PermissionsName = model.PermissionsName,
                    DepartmentName = model.DepartmentName,
                    DepartmentId = model.DepartmentId,
                    AccountPermissionAccessId = Convert.ToInt32(model.AccountPermissionAccessId),
                    ApprovalUserId = Convert.ToInt32(model.ApprovalUserId),
                    BudgetAmount = model.BudgetAmount,
                    AccountUserOrderApprovalId = model.AccountUserOrderApprovalId,
                    CustomerPaymentGUID = model.CustomerPaymentGUID,
                    SalesRepId = model.SalesRepId,
                    User = new LoginUserModel()
                    {
                        Email = model.Email,
                        UserId = model.AspNetUserId,
                        Username = model.UserName
                    }
                };
            }
            return new UserModel();
        }

        /// <summary>
        /// To convert AccountListModel to CustomerListViewModel
        /// </summary>
        /// <param name="models">AccountListModel.</param>
        /// <returns>Returns CustomerListViewModel.</returns>
        public static CustomerListViewModel ToListViewModel(UserListModel models)
        {
            //users list model
            CustomerListViewModel listViewModel = new CustomerListViewModel();

            //checks for model not null and User list not null
            if (!Equals(models, null) && !Equals(models.Users, null))
            {
                //Iterate through api User Model
                foreach (UserModel model in models.Users)
                {
                    listViewModel.List.Add(ToViewModel(model));
                }
                //set for pagination
                listViewModel.Page = Convert.ToInt32(models.PageIndex);
                listViewModel.RecordPerPage = Convert.ToInt32(models.PageSize);
                listViewModel.TotalPages = Convert.ToInt32(models.TotalPages);
                listViewModel.TotalResults = Convert.ToInt32(models.TotalResults);
            }
            return listViewModel;
        }

        /// <summary>
        /// This method will convert User Model to CustomerViewModel model.
        /// </summary>
        /// <param name="userModel">userModel</param>
        /// <returns>Returns CustomerViewModel.</returns>
        public static CustomerViewModel ToViewModel(UserModel userModel)
        {
            if (!Equals(userModel, null))
            {
                return new CustomerViewModel
                {
                    AspNetUserId = userModel.AspNetUserId,
                    UserName = userModel.UserName,
                    AccountId = userModel.AccountId,
                    Email = userModel.Email,
                    FirstName = $"{userModel.FirstName } { userModel.LastName }",
                    PhoneNumber = userModel.PhoneNumber,
                    CreatedDate = userModel.CreatedDate.ToDateTimeFormat(),
                    IsLock = Convert.ToBoolean(userModel.IsLock),
                    Custom1 = userModel.Custom1,
                    Custom2 = userModel.Custom2,
                    Custom3 = userModel.Custom3,
                    Description = userModel.Description,
                    Source = userModel.Source,
                    Website = userModel.Website,
                    EmailOptIn = userModel.EmailOptIn,
                    CompanyName = userModel.CompanyName,
                    ExternalId = userModel.ExternalId,
                    FullName = userModel.FullName,
                    UserId = userModel.UserId
                };
            }
            return new CustomerViewModel();
        }

        //
        public static UserModel ToUserAddressModel(UserAddressDataViewModel userAddressDataViewModel, string emailAddress)
        {
            if (HelperUtility.IsNotNull(userAddressDataViewModel.LoginViewModel))
            {
                LoginViewModel loginViewModel = userAddressDataViewModel.LoginViewModel;
                //
                UserModel userModel = new UserModel()
                {
                    User = new LoginUserModel()
                    {
                        Username = loginViewModel.Username,
                        Email = emailAddress.Trim(),
                        IsApproved = true,
                    },
                    ProfileId = 1,

                };
                //
                userModel.PortalIds = new string[100];
                userModel.PortalIds.SetValue(userAddressDataViewModel.PortalId.ToString(), 0);
                //
                userModel.User.Password = string.IsNullOrEmpty(loginViewModel.Password) ? loginViewModel.Password : loginViewModel.Password;
                return userModel;
            }
            return new UserModel();
        }
    }
}