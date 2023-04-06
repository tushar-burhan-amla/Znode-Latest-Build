using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class AccountService : BaseService, IAccountService
    {
        #region Private Variables
        protected readonly IZnodeRepository<ZnodeAccount> _accountRepository;
        protected readonly IZnodeRepository<View_GetNotes> _viewGetNotesRepository;
        protected readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        protected readonly IZnodeRepository<ZnodeNote> _noteRepository;
        protected readonly IZnodeRepository<ZnodeDepartment> _departmentRepository;
        protected readonly IZnodeRepository<ZnodeAccountAddress> _accountAddressRepository;
        protected readonly IZnodeRepository<ZnodeUserAddress> _userAddressRepository;
        protected readonly IZnodeRepository<ZnodePortalAccount> _portalAccountRepository;
        protected readonly IZnodeRepository<ZnodePriceListAccount> _priceListAccountRepository;
        protected readonly IZnodeRepository<View_AccountProfileList> _viewAccountProfileList;
        protected readonly IZnodeRepository<ZnodeAccountProfile> _accountProfileRepository;
        protected readonly IZnodeRepository<ZnodePortal> _portalRepository;
        protected readonly IZnodeRepository<ZnodeApproverLevel> _approverLevelRepository;
        protected readonly IZnodeRepository<ZnodeUserApprover> _userapproverRepository;
        protected readonly IZnodeRepository<ZnodeUser> _userRepository;
        protected readonly IZnodeRepository<ZnodeAccountUserPermission> _accountUserPermissionRepository;
        protected readonly IZnodeRepository<ZnodeAccountUserOrderApproval> _accountUserOrderApprovalRepository;
        protected readonly IZnodeRepository<AspNetZnodeUser> _aspNetZnodeUserRepository;
        protected readonly IZnodeRepository<AspNetUser> _aspNetUserRepository;
        #endregion

        #region Constructor
        public AccountService()
        {
            _accountUserPermissionRepository = new ZnodeRepository<ZnodeAccountUserPermission>();
            _accountUserOrderApprovalRepository = new ZnodeRepository<ZnodeAccountUserOrderApproval>();
            _accountRepository = new ZnodeRepository<ZnodeAccount>();
            _viewGetNotesRepository = new ZnodeRepository<View_GetNotes>();
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _noteRepository = new ZnodeRepository<ZnodeNote>();
            _departmentRepository = new ZnodeRepository<ZnodeDepartment>();
            _accountAddressRepository = new ZnodeRepository<ZnodeAccountAddress>();
            _userAddressRepository = new ZnodeRepository<ZnodeUserAddress>();
            _priceListAccountRepository = new ZnodeRepository<ZnodePriceListAccount>();
            _portalAccountRepository = new ZnodeRepository<ZnodePortalAccount>();
            _viewAccountProfileList = new ZnodeRepository<View_AccountProfileList>();
            _accountProfileRepository = new ZnodeRepository<ZnodeAccountProfile>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _approverLevelRepository = new ZnodeRepository<ZnodeApproverLevel>();
            _userapproverRepository = new ZnodeRepository<ZnodeUserApprover>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _aspNetZnodeUserRepository = new ZnodeRepository<AspNetZnodeUser>();
            _aspNetUserRepository = new ZnodeRepository<AspNetUser>();
        }
        #endregion

        #region Public Methods
        #region Account
        public virtual AccountListModel GetAccountList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(),TraceLevel.Info);
            //if the global level user creation is set to true then set portal id to null.
            if (!(filters?.Any(x => string.Equals(x.FilterName, ZnodePortalEnum.PortalId.ToString(), StringComparison.CurrentCultureIgnoreCase))).GetValueOrDefault() &&
                !DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation)
                //Bind the Filter conditions for the authorized portal access.
                BindUserPortalFilter(ref filters);

            //Bind the Filter, sorts & Paging details.
             PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameter values: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<AccountModel> objStoredProc = new ZnodeViewRepository<AccountModel>();
            
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            var list = objStoredProc.ExecuteStoredProcedureList("Znode_GetAccountListWithAddress @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Account list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());
            AccountListModel listModel = new AccountListModel { Accounts = list?.ToList() };

            //Set for pagination.
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return listModel;
        }

        public virtual AccountModel GetAccount(int accountId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (accountId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.AccountIdNotLessThanOne);

            //Get the Account Details based on id.
            ZnodeAccount account = _accountRepository.Table.FirstOrDefault(x => x.AccountId == accountId);
            ZnodeLogging.LogMessage("ZnodeAccount account properties: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { account?.AccountId, account?.ParentAccountId, account?.PublishCatalogId });
            if (IsNotNull(account))
            {
                AccountModel model = account.ToModel<AccountModel>();
                if (account.AccountId > 0)
                    //Gets account address by accountId
                    model.Address = GetAddressByAccountId(account);

                if (account.ParentAccountId > 0)
                    //Set the Account model with parent accounts data.
                    SetParentAccountData(account, model);

                //Binds portalId and storeName to account details
                BindPortalDetailsToAccount(account, model);

                if (account.PublishCatalogId > 0 || IsNotNull(account.PublishCatalogId))                    
                    model.CatalogName = GetService<IPublishedCatalogDataService>().GetPublishCatalogById(account.PublishCatalogId.GetValueOrDefault())?.CatalogName; ;

                SetSalesRepDetails(account, model);
                ZnodeLogging.LogMessage("AccountId and CatalogName of AccountModel to be returned: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { AccountId = model?.AccountId, CatalogName = model?.CatalogName });
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return model;
            }

            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PleaseProvideValidAccountId);
        }

        protected virtual void SetSalesRepDetails(ZnodeAccount account, AccountModel model)
        {
            if (account.SalesRepId > 0)
            {
                var user = (from users in _userRepository.Table
                            join aspNetUser in _aspNetUserRepository.Table on users.AspNetUserId equals aspNetUser.Id
                            join aspnetZnodeUser in _aspNetZnodeUserRepository.Table on aspNetUser.UserName equals aspnetZnodeUser.AspNetZnodeUserId
                            where users.UserId == account.SalesRepId
                            select new
                            {
                                users.FirstName,
                                users.MiddleName,
                                users.LastName,
                                aspnetZnodeUser.UserName
                            }).FirstOrDefault();

                if (IsNotNull(user))
                {
                    model.SalesRepFullName = (!string.IsNullOrEmpty(user.FirstName) ? user.FirstName : "") + (!string.IsNullOrEmpty(user.MiddleName) ? " " + user.MiddleName : "") + (!string.IsNullOrEmpty(user.LastName) ? " " + user.LastName : "");
                    model.SalesRepUserName = user.UserName;
                }
            }
        }

        //Get Account details by Account code.
        public virtual AccountModel GetAccountByCode(string accountCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(accountCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.AccountCodeCanNotBeEmpty);

            int? accountId = _accountRepository?.Table?.FirstOrDefault(x => x.AccountCode.ToLower().Equals(accountCode.ToLower()))?.AccountId;

            if (IsNull(accountId) || accountId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.AccountCodeIsInValidMessage);

            return GetAccount(accountId.GetValueOrDefault());
        }

        //Get level list.
        public virtual ApproverLevelListModel Getlevelslist() => new ApproverLevelListModel
        {
            ApproverLevelList = _approverLevelRepository.Table.ToModel<ApproverLevelModel>().ToList()
        };

        //Create approver level.
        public virtual bool CreateApproverLevel(UserApproverModel userApproverModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (IsNull(userApproverModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);
            ZnodeLogging.LogMessage("Input parameter UserApproverId of userApproverModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { UserApproverId = userApproverModel?.UserApproverId });

            bool status = false;

            return (userApproverModel.UserApproverId > 0) ? UpdateApproverLevel(userApproverModel, out status) : InsertApproverLevel(userApproverModel, status);
        }

        //Save permission setting.
        public bool SavePermissionSetting(PermissionCodeModel permissionCodeModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            bool status = false;
            if (string.IsNullOrEmpty(permissionCodeModel.PermissionCode))
                permissionCodeModel.PermissionCode = ZnodeConstant.SRA;

            FilterCollection filters = new FilterCollection
            {
                new FilterTuple(ZnodeDepartmentUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, permissionCodeModel.UserId.ToString()),
                new FilterTuple(ZnodeConstant.ShowAllApprovers.ToString().ToLower(), FilterOperators.Equals, true.ToString())
            };
            filters.Add(new FilterTuple(ZnodeConstant.User.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue.ToString()));
            IAccountQuoteService _accountQuoteService = GetService<IAccountQuoteService>();
            UserApproverListModel userApproverListModel = _accountQuoteService.GetUserApproverList(null, filters, null, null);
            filters.RemoveAll(x => x.FilterName == ZnodeConstant.User.ToString().ToLower());
            filters.RemoveAll(x => x.FilterName == ZnodeConstant.ShowAllApprovers.ToString().ToLower());

            int count = userApproverListModel.UserApprovers.Count;
            ZnodeLogging.LogMessage("UserApprovers list count of userApproverListModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, count);
            if(!string.Equals(permissionCodeModel.PermissionCode, ZnodePermissionCodeEnum.DNRA.ToString(), StringComparison.InvariantCultureIgnoreCase) && count.Equals(0))
                throw new ZnodeException(ErrorCodes.AtLeastSelectOne, Admin_Resources.AssociateAtLeastOneApprover);

            if (string.Equals(permissionCodeModel.PermissionCode, ZnodePermissionCodeEnum.DNRA.ToString(), StringComparison.InvariantCultureIgnoreCase) && count > 0)
            {
                ZnodeLogging.LogMessage("GenerateDynamicWhereClause method call with parameter filters.ToFilterDataCollection(): ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, filters.ToFilterDataCollection());
                _userapproverRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection()));
            }

            if ((_accountUserPermissionRepository.Table.Where(x => x.UserId == permissionCodeModel.UserId)?.ToList()?.Count > 0))
            {
                ZnodeLogging.LogMessage("AccountPermissionAccessId and AccountUserPermissionId values of permissionCodeModel:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { AccountPermissionAccessId = permissionCodeModel?.AccountPermissionAccessId });
                if (permissionCodeModel.AccountPermissionAccessId > 0)
                    status = _accountUserPermissionRepository.Update(new ZnodeAccountUserPermission { AccountUserPermissionId = Convert.ToInt32(permissionCodeModel.AccountUserPermissionId), UserId = permissionCodeModel.UserId, AccountPermissionAccessId = permissionCodeModel.AccountPermissionAccessId.GetValueOrDefault() });
            }
            else
            {
                if (permissionCodeModel.AccountPermissionAccessId > 0)
                {
                    ZnodeAccountUserPermission permissionDetails = _accountUserPermissionRepository.Insert(new ZnodeAccountUserPermission { UserId = permissionCodeModel.UserId, AccountPermissionAccessId = permissionCodeModel.AccountPermissionAccessId.GetValueOrDefault() });
                    ZnodeLogging.LogMessage("AccountUserPermissionId value of permissionDetails:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { AccountUserPermissionId = permissionDetails?.AccountUserPermissionId });
                    status = permissionDetails.AccountUserPermissionId > 0;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return status;
        }

        //Delete User Approver on the basis User Approver Id
        public virtual bool DeleteApproverLevel(ParameterModel userApproverId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("GenerateWhereClauseForDeleteApprover method call with parameter userApproverId: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, userApproverId);
            bool status = _userapproverRepository.Delete(GenerateWhereClauseForDeleteApprover(userApproverId).WhereClause);
            if (status)
            {
                ZnodeLogging.LogMessage(Admin_Resources.DeleteMessage, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorFailedToDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.RestrictSystemDefineDeletion, Admin_Resources.ErrorFailedToDelete);
            }
        }

        //Gets account by provided name and portalId
        public virtual AccountModel GetAccountByName(string accountName, NameValueCollection expands, int portalId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input paremeters: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { accountName = accountName, portalId = portalId });
            if (string.IsNullOrEmpty(accountName))
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.AccountNameNotEmpty);

            ZnodeAccount account;
            if (portalId > 0)
                //Gets the account based on accountName and portalId
                account = GetAccountBasedOnNameAndPortalId(accountName, portalId);
            else
                account = _accountRepository.Table.Where(x => x.Name.ToLower() == accountName.ToLower())?.FirstOrDefault();

            if (IsNotNull(account))
            {
                AccountModel accountModel = account.ToModel<AccountModel>();
                if (account.AccountId > 0)
                    //Gets account address by accountId
                    accountModel.Address = GetAddressByAccountId(account);

                if (accountModel.ParentAccountId > 0)
                    //Set the Account model with parent accounts data.
                    SetParentAccountData(account, accountModel);

                //Binds portalId and storeName to account details
                BindPortalDetailsToAccount(account, accountModel);
                ZnodeLogging.LogMessage("AccountId and ParentAccountId of accountModel to be returned: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { AccountId = accountModel?.AccountId, ParentAccountId = accountModel?.ParentAccountId });

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return accountModel;
            }

            throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.PleaseProvideValidAccountNameAndPortalId);
        }

        public virtual AccountDataModel CreateAccount(AccountDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (IsNull(model?.CompanyAccount))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ModelNotNull);
            ZnodeLogging.LogMessage("Input parameters Name, AccountId, PortalId properties of CompanyAccount model: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { Name = model?.CompanyAccount?.Name, AccountId = model?.CompanyAccount?.AccountId, PortalId = model?.CompanyAccount?.PortalId });

            //Create new Company Account.
            ZnodeAccount account = _accountRepository.Insert(model.CompanyAccount.ToEntity<ZnodeAccount>());
            ZnodeLogging.LogMessage("AccountId, ParentAccountId values: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { AccountId = account?.AccountId, ParentAccountId = account?.ParentAccountId});
            if (account?.AccountId > 0)
            {
                IGlobalAttributeGroupEntityService _service = ZnodeDependencyResolver.GetService<IGlobalAttributeGroupEntityService>();
                GlobalAttributeEntityDetailsModel attributeDetail= _service.GetEntityAttributeDetails(account.AccountId, EntityTypeEnum.Account.ToString());
                if (IsNotNull(attributeDetail))
                {
                    EntityAttributeModel attributesModel = attributeDetail.ToEntity<EntityAttributeModel>();
                    attributesModel.EntityValueId = account.AccountId;
                    if (IsNotNull(attributeDetail.Attributes))
                    {
                        attributesModel.Attributes = attributeDetail.Attributes.Where(x => IsNotNull(x) && IsNotNull(x.AttributeDefaultValue))?
                            .Distinct()?.ToList().ToEntity<EntityAttributeDetailsModel>().ToList();
                        if(IsNotNull(attributesModel.Attributes))
                        {
                            if (attributesModel.Attributes.Count() > 0)
                            {
                                attributesModel.Attributes.ForEach(a => a.LocaleId = model.CompanyAccount.LocaleId);
                                _service.SaveEntityAttributeDetails(attributesModel);
                            }
                        }
                        
                    }
                }
               
                    
                model.CompanyAccount.Address.AccountId = account.AccountId;
                model.CompanyAccount.AccountId = account.AccountId;

                if (account.ParentAccountId > 0)
                    model.CompanyAccount.PortalId = GetPortalId(account.ParentAccountId.GetValueOrDefault());

                //Save the portal for account.
                InsertIntoPortalAccount(model);

                //Save the Permissions for user.
                SavePermissions(account.AccountId);

                if (account.ParentAccountId > 0)
                    SaveProfileForSubAccount(model.CompanyAccount.ParentAccountId.Value, account.AccountId);
                else
                    //Save portal default profiles for account.
                    SaveDefaultProfile((model.CompanyAccount?.PortalId).GetValueOrDefault(), account.AccountId);

                //Create the Address for the Account.
                ZnodeAddress address = _addressRepository.Insert(model.CompanyAccount.Address.ToEntity<ZnodeAddress>());
                int? accountAddressId = 0;
                if (address?.AddressId > 0)
                {
                    accountAddressId = _accountAddressRepository.Insert(new ZnodeAccountAddress() { AccountId = account.AccountId, AddressId = address.AddressId })?.AccountAddressId;
                    ZnodeLogging.LogMessage("accountAddressId value: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, accountAddressId);
                    ZnodeLogging.LogMessage(accountAddressId > 0 ? Admin_Resources.SuccessAccountAddressMappingInsert : Admin_Resources.ErrorAccountAddressMappingInsert, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                }

                model.CompanyAccount = account.ToModel<AccountModel>();
                if (IsNotNull(address))
                {
                    //Binds the address.
                    model.CompanyAccount.Address = address.ToModel<AddressModel>();
                    model.CompanyAccount.Address.AccountAddressId = Convert.ToInt32(accountAddressId);
                    model.CompanyAccount.Address.AccountId = model.CompanyAccount.AccountId;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return model;
        }

        //Save all parent account profiles for child account.
        private void SaveProfileForSubAccount(int parentAccountId, int subAccountId)
        {
            if (parentAccountId > 0)
            {
                IQueryable<int?> profileIds = _accountProfileRepository.Table.Where(x => x.AccountId == parentAccountId).Select(x => x.ProfileId);
                ZnodeLogging.LogMessage("profileIds count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, profileIds?.Count());
                if (profileIds?.Count() > 0)
                    AssociateProfile(new ProfileModel() { ProfileIds = string.Join(",", profileIds), AccountId = subAccountId });
            }
        }

        //Save IsDefaultRegistered profile to account.
        private void SaveDefaultProfile(int portalId, int accountId)
        {
            //If allow global level user creation is true, get portal id from siteconfig.
            if (DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation)
                portalId = portalId > 0 ? portalId : ZnodeConfigManager.SiteConfig?.PortalId ?? 0;
            if (portalId > 0 && accountId > 0)
            {
                IZnodeRepository<ZnodePortalProfile> _portalProfileRepository = new ZnodeRepository<ZnodePortalProfile>();
                int profileId = _portalProfileRepository.Table.Where(x => x.PortalId == portalId && x.IsDefaultRegistedProfile).Select(x => x.ProfileId).FirstOrDefault();
                ZnodeLogging.LogMessage("profileId value: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, profileId);
                if (profileId > 0)
                    AssociateProfile(new ProfileModel() { ProfileIds = profileId.ToString(), AccountId = accountId });
            }
        }

        public virtual bool Update(AccountDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNull(model?.CompanyAccount))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            ZnodeLogging.LogMessage("Input parameters Name, AccountId, PortalId properties of CompanyAccount model: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { Name = model?.CompanyAccount?.Name, AccountId = model?.CompanyAccount?.AccountId, PortalId = model?.CompanyAccount?.PortalId });

            if (model?.CompanyAccount?.AccountId > 0)
            {
                status = _accountRepository.Update(model?.CompanyAccount.ToEntity<ZnodeAccount>());
                if (status)
                {
                    model.CompanyAccount.Address.AccountId = model.CompanyAccount.AccountId;

                    if (model.CompanyAccount.ParentAccountId > 0)
                        model.CompanyAccount.PortalId = _portalAccountRepository.Table.FirstOrDefault(x => x.AccountId == model.CompanyAccount.ParentAccountId)?.PortalId;

                    InsertIntoPortalAccount(model, true);

                    SetAddressFlagsToFalse(model.CompanyAccount.Address);

                    //If Address present then Update, Else Insert the Address.
                    if (model?.CompanyAccount?.Address?.AddressId > 0)
                    {
                        status = _addressRepository.Update(model?.CompanyAccount?.Address.ToEntity<ZnodeAddress>());
                        ZnodeLogging.LogMessage("Update address status: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, status);
                    }
                    else
                    {
                        //Create the Address for the Account.
                        ZnodeAddress address = _addressRepository.Insert(model.CompanyAccount.Address.ToEntity<ZnodeAddress>());
                        if (address?.AddressId > 0)
                            _accountAddressRepository.Insert(new ZnodeAccountAddress() { AccountId = model.CompanyAccount.Address.AccountId, AddressId = address.AddressId });
                    }
                }
            }
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessAccountUpdated : Admin_Resources.ErrorAccountUpdate, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return status;
        }

        public virtual bool Delete(ParameterModel accountIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(accountIds?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);
            ZnodeLogging.LogMessage("accountIds value to delete account: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, accountIds?.Ids);
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter(ZnodeAccountEnum.AccountId.ToString(), accountIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

                int status = 0;
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteAccount @AccountId, @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, deleteResult?.Count);
            if (deleteResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessAccountDelete, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, deleteResult?.Count());
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorAccountDelete, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                    return false;
                }
            }

        public virtual bool IsCodeExists(HelperParameterModel parameterModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return _accountRepository.Table.Any(a => a.AccountCode == parameterModel.CodeField);
        }


        //Delete Accounts by AccountCode.
        public virtual bool DeleteAccountByCode(ParameterModel accountCodes)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(accountCodes.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.AccountCodeCanNotBeEmpty);

            ParameterModel accountId = new ParameterModel();

            var accountIds = _accountRepository?.Table?.Where(x => accountCodes.Ids.ToLower().Contains(x.AccountCode.ToLower()))?.Select(x => x.AccountId.ToString())?.ToList();

            if (accountIds?.Count > 0)
                accountId.Ids = String.Join(",", accountIds);

            if (IsNull(accountId.Ids) || accountId.Ids?.Count() < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.AccountCodeIsInValidMessage);

            return Delete(accountId);
        }
        #endregion

        #region Account Note
        public virtual NoteListModel GetAccountNotes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to get note list: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Gets the entity list according to where clause, order by clause and pagination.
            NoteListModel noteList = new NoteListModel
            {
                Notes = _viewGetNotesRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.AsEnumerable()
                .ToModel<NoteModel>()?.ToList()
            };
            noteList.BindPageListModel(pageListModel);

            if (filters.Any(x => string.Equals(x.FilterName, ZnodeAccountEnum.AccountId.ToString(), StringComparison.CurrentCultureIgnoreCase)
                                    || string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase)))
            {
                //Get customer name by user id.
                noteList.CustomerName = GetCustomerName(filters);
                ZnodeLogging.LogMessage("CustomerName value of NoteListModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, noteList?.CustomerName);
            }
            return noteList;
        }

        public virtual NoteModel GetAccountNote(int noteId)
            => noteId > 0 ? _noteRepository.GetById(noteId)?.ToModel<NoteModel>() : new NoteModel();

        public virtual NoteModel CreateAccountNote(NoteModel noteModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (IsNull(noteModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            noteModel = _noteRepository.Insert(noteModel?.ToEntity<ZnodeNote>())?.ToModel<NoteModel>();
            ZnodeLogging.LogMessage(!Equals(noteModel, null) ? Admin_Resources.SuccessAccountNoteAdd : Admin_Resources.ErrorAccountNoteAdd, string.Empty, TraceLevel.Info);

            return noteModel;
        }

        public virtual bool UpdateAccountNote(NoteModel noteModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            bool status = false;
            if (IsNull(noteModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            ZnodeLogging.LogMessage("NoteId property of noteModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { noteModel?.NoteId });
            if (noteModel?.NoteId > 0)
                status = _noteRepository.Update(noteModel.ToEntity<ZnodeNote>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessAccountNoteUpdate : Admin_Resources.ErrorAccountNoteUpdate, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return status;
        }

        public virtual bool DeleteAccountNote(ParameterModel noteId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter NoteId to delete AccountNote: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { noteId = noteId?.Ids});

            if (string.IsNullOrEmpty(noteId?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeNoteEnum.NoteId.ToString(), ProcedureFilterOperators.In, noteId?.Ids?.ToString()));

            return _noteRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }
        #endregion

        #region Account Department
        public virtual AccountDepartmentListModel GetAccountDepartments(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            //Set the filter to get the departments of account.
            SetFilters(filters);
            
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to get departmentEntity: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Gets the entity list according to where clause, order by clause and pagination
            IList<ZnodeDepartment> departmentEntity = _departmentRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);

            AccountDepartmentListModel departmentListModel = new AccountDepartmentListModel { Departments = departmentEntity?.ToModel<AccountDepartmentModel>().ToList() };
            ZnodeLogging.LogMessage("Departments list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, departmentListModel?.Departments?.Count());
            //Binds the data for pagination.
            departmentListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return departmentListModel;
        }

        public virtual AccountDepartmentModel GetAccountDepartment(int departmentId)
          => departmentId > 0 ? _departmentRepository.GetById(departmentId)?.ToModel<AccountDepartmentModel>() : new AccountDepartmentModel();

        public virtual AccountDepartmentModel CreateAccountDepartment(AccountDepartmentModel accountDepartmentModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (Equals(accountDepartmentModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            ZnodeLogging.LogMessage("IsDepartmentExist method call with parameters DepartmentName and AccountId of accountDepartmentModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { DepartmentName = accountDepartmentModel?.DepartmentName, AccountId = accountDepartmentModel?.AccountId });            
            if (IsDepartmentExist(accountDepartmentModel.DepartmentName, accountDepartmentModel.AccountId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorDepartmentAlreadyExists);

            accountDepartmentModel = _departmentRepository.Insert(accountDepartmentModel?.ToEntity<ZnodeDepartment>())?.ToModel<AccountDepartmentModel>();
            ZnodeLogging.LogMessage(!Equals(accountDepartmentModel, null) ? Admin_Resources.SuccessAccountDepartmentAdd : Admin_Resources.ErrorAccountDepartmentAdd, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return accountDepartmentModel;
        }

        public virtual bool UpdateAccountDepartment(AccountDepartmentModel accountDepartmentModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (Equals(accountDepartmentModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            bool status = false;
            ZnodeLogging.LogMessage("DepartmentId property of accountDepartmentModel:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { DepartmentId = accountDepartmentModel?.DepartmentId });
            if (accountDepartmentModel?.DepartmentId > 0)
            {
                //Get the Department Details
                ZnodeDepartment department = _departmentRepository.Table.Where(x => x.DepartmentId == accountDepartmentModel.DepartmentId)?.FirstOrDefault();
                ZnodeLogging.LogMessage("IsDepartmentExist method call with parameters DepartmentName and AccountId of accountDepartmentModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { DepartmentName = accountDepartmentModel?.DepartmentName, AccountId = accountDepartmentModel?.AccountId });
                //Check for Existing department name, in case of update.
                if (!Equals(department?.DepartmentName, accountDepartmentModel.DepartmentName) && 
                    IsDepartmentExist(accountDepartmentModel.DepartmentName, accountDepartmentModel.AccountId))
                        throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorDepartmentAlreadyExists);

                status = _departmentRepository.Update(accountDepartmentModel.ToEntity<ZnodeDepartment>());
            }

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessAccountDepartmentUpdate : Admin_Resources.ErrorAccountDepartmentUpdate, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return status;
        }

        public virtual bool DeleteAccountDepartment(ParameterModel departmentId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter departmentId to be deleted: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, departmentId?.Ids);

            if (string.IsNullOrEmpty(departmentId?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);

            if (IsDepartmentAssociated(departmentId.Ids))
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.DepartmentAssociateToSomeUser);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeDepartmentEnum.DepartmentId.ToString(), ProcedureFilterOperators.In, departmentId?.Ids?.ToString()));

            return _departmentRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }
        #endregion

        #region Address
        public virtual AddressListModel GetAddressList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to get ZnodeAccountAddress list: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<ZnodeAccountAddress> list = _accountAddressRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Address list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());
            AddressListModel listModel = AccountMap.ToListModel(list);

            //Set for pagination.
            listModel.BindPageListModel(pageListModel);

            //Set one time address availability flag
            //listModel?.AddressList?.ForEach(o => o.DontAddUpdateAddress = true);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return listModel;
        }

        public virtual AddressModel GetAccountAddress(FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Check for filter present in filters.
            if (filters?.Count > 0)
            {
                EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("whereClause generated: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);

                return AccountMap.ToModel(_accountAddressRepository.GetEntity(whereClause.WhereClause, GetExpands(expands), whereClause.FilterValues));
            }
            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorFiltersEmpty);
        }

        public virtual AddressModel CreateAccountAddress(AddressModel addressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (Equals(addressModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            //Account id should be there for creating account address.
            if (addressModel.AccountId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.AccountIdNotLessThanOne);

            SetAddressFlagsToFalse(addressModel);

            ZnodeAddress address = _addressRepository.Insert(addressModel.ToEntity<ZnodeAddress>());
            ZnodeLogging.LogMessage("AddressId and AccountId properties of ZnodeAddress :", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new{ AddressId = address?.AddressId, AccountId = addressModel?.AccountId});
            //Check for address created or not.
            if (address?.AddressId > 0)
            {
                addressModel.AddressId = address.AddressId;
                //Insert into mapper table if do not add update address flag is false.
                if (!addressModel.DontAddUpdateAddress)
                    _accountAddressRepository.Insert(new ZnodeAccountAddress() { AccountId = addressModel.AccountId, AddressId = address.AddressId });
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return addressModel;
        }

        public virtual bool UpdateAccountAddress(AddressModel addressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (IsNull(addressModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeLogging.LogMessage("AddressId value of addressModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { AddressId = addressModel?.AddressId });
            if (addressModel.AddressId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.AccountIdNotLessThanOne);

            SetAddressFlagsToFalse(addressModel);

            bool status = _addressRepository.Update(addressModel.ToEntity<ZnodeAddress>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessUpdateAccountAddress : Admin_Resources.ErrorUpdateAccountAddress, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return status;
        }

        public virtual bool DeleteAccountAddress(ParameterModel accountAddressId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("accountAddressId value to delete account address: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, accountAddressId?.Ids);

            if (string.IsNullOrEmpty(accountAddressId?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdsCanNotEmpty);

            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodeAccountAddressEnum.ZnodeAddress.ToString(), ZnodeAccountAddressEnum.ZnodeAddress.ToString());

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountAddressEnum.AccountAddressId.ToString(), FilterOperators.In, accountAddressId?.Ids?.ToString()));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("whereClause generated to get ZnodeAccountAddress list: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { whereClause = whereClause });
            List<ZnodeAccountAddress> list = _accountAddressRepository.GetEntityList(whereClause, GetExpands(expands)).ToList();
            ZnodeLogging.LogMessage("Account address list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());

            //Exclude the IsDefaultBilling and IsDefaultShipping true from list.
            List<ZnodeAccountAddress> listToDelete = list.FindAll(x => !x.ZnodeAddress.IsDefaultBilling && !x.ZnodeAddress.IsDefaultShipping);
            ZnodeLogging.LogMessage("list excluding the IsDefaultBilling and IsDefaultShipping true count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, listToDelete?.Count());

            bool isDeleted = false;
            string idsToDelete = string.Join(",", listToDelete?.Select(x => x.AccountAddressId));
            ZnodeLogging.LogMessage("AccountAddressIds to delete:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, idsToDelete);

            if (!string.IsNullOrEmpty(idsToDelete))
            {
                filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeAccountAddressEnum.AccountAddressId.ToString(), FilterOperators.In, idsToDelete));
                whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
                ZnodeLogging.LogMessage("whereClause generated: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClause);
                isDeleted = _accountAddressRepository.Delete(whereClause);
                ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessAccountAddressDelete : Admin_Resources.ErrorAccountAddressDelete, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            }
            //Some of the address is IsDefaultBilling or IsDefaultShipping then throw exception.
            if (list.Count != listToDelete?.Count)
                throw new ZnodeException(ErrorCodes.NotPermitted,Admin_Resources.IsDefaultBillingOrShippingNotDelete);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return isDeleted;
        }
        #endregion

        #region Associate Price
        //Associate price list to account.
        public virtual bool AssociatePriceList(PriceAccountModel priceAccountModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (priceAccountModel?.AccountId < 1 || string.IsNullOrEmpty(priceAccountModel?.PriceListIds))
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.PriceListAndAccountIdNotLessThanOne);
            else if (_priceListAccountRepository?.Table?.Where(x => x.AccountId == priceAccountModel.AccountId && x.PriceListId == priceAccountModel.PriceListId)?.Count() > 0)
            {
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.PriceListAlreadyAssociatedToAccount);
            }
            ZnodeLogging.LogMessage("Input parameter priceAccountModel value: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, priceAccountModel);

            string[] priceListIds = priceAccountModel.PriceListIds.Split(',');

            List<ZnodePriceListAccount> entitiesToInsert = new List<ZnodePriceListAccount>();

            foreach (string item in priceListIds)
                entitiesToInsert.Add(new ZnodePriceListAccount() { AccountId = priceAccountModel.AccountId, PriceListId = Convert.ToInt32(item), Precedence = ZnodeConstant.DefaultPrecedence });

            return _priceListAccountRepository.Insert(entitiesToInsert)?.Count() > 0;
        }

        //UnAssociate associated price list from account.
        public virtual bool UnAssociatePriceList(PriceAccountModel priceAccountModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (IsNull(priceAccountModel) || string.IsNullOrEmpty(priceAccountModel.PriceListIds) || priceAccountModel.AccountId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelInvalid);
            ZnodeLogging.LogMessage("Input parameter priceAccountModel value: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, priceAccountModel);

            //Generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PriceListId.ToString(), ProcedureFilterOperators.In, priceAccountModel.PriceListIds.ToString()));

            if (priceAccountModel.AccountId > 0)
                filters.Add(new FilterTuple(ZnodePriceListAccountEnum.AccountId.ToString(), ProcedureFilterOperators.Equals, priceAccountModel.AccountId.ToString()));

            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, entityWhereClauseModel.WhereClause);

            //Delete mapping of account against price.
            bool isDeleted = _priceListAccountRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessPriceListFromAccountUnAssociate : Admin_Resources.ErrorPriceListFromAccountUnAssociate, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Get Associated Price List with Precedence data for Account.
        public virtual PriceAccountModel GetAssociatedPriceListPrecedence(PriceAccountModel priceAccountModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter priceAccountModel value: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, priceAccountModel);

            FilterCollection filters = new FilterCollection();
            if (HelperUtility.IsNotNull(priceAccountModel))
            {
                if (priceAccountModel.PriceListId < 1 || priceAccountModel.AccountId < 1)
                    throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ProfileIdAndAccountNotLessThanZero);
                
                filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PriceListId.ToString(), ProcedureFilterOperators.Equals, priceAccountModel.PriceListId.ToString()));
                filters.Add(new FilterTuple(ZnodePriceListAccountEnum.AccountId.ToString(), ProcedureFilterOperators.Equals, priceAccountModel.AccountId.ToString()));
            }
            return _priceListAccountRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause)?.ToModel<PriceAccountModel>();
        }

        //Update the precedence value for associated price list for Account.
        public virtual bool UpdateAssociatedPriceListPrecedence(PriceAccountModel priceAccountModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter priceAccountModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, priceAccountModel);

            if (IsNull(priceAccountModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            bool status = false;

            if (priceAccountModel?.PriceListAccountId > 0 || priceAccountModel?.AccountId > 0)
                status = _priceListAccountRepository.Update(priceAccountModel.ToEntity<ZnodePriceListAccount>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessPrecedenceValueForPriceListUpdate : Admin_Resources.ErrorPrecedenceValueForPriceListUpdate, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return status;
        }
        #endregion

        #region Account Order
        //Get user order list of account.      
        public virtual OrdersListModel GetAccountUserOrderList(int accountId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            //Replace sort key name.
            if (HelperUtility.IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to SP parameters: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<OrderModel> objStoredProc = new ZnodeViewRepository<OrderModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@AccountId", accountId, ParameterDirection.Input, DbType.String);
            IList<OrderModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetOmsAccountOrderList @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT,@AccountId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("AccountUserOrder list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());

            OrdersListModel orderListModel = new OrdersListModel() { Orders = list?.ToList() };
            if (accountId > 0)
                orderListModel.HasParentAccounts = IsNull(GetParentAccountId(accountId));

            orderListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return orderListModel;
        }
        #endregion

        #region Account Profile

        //Get associated/unassociated profiles to account.
        public virtual ProfileListModel GetAssociatedUnAssociatedProfile(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            int accountId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, ZnodeAccountEnum.AccountId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate profile list: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //If filter contains IsAssociated true get associated profiles to account else unassociated profiles to account.
            ProfileListModel profileList = new ProfileListModel
            {
                Profiles = _viewAccountProfileList.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.AsEnumerable()
                .ToModel<ProfileModel>()?.ToList()
            };
            profileList.BindPageListModel(pageListModel);

            if (accountId > 0)
                profileList.HasParentAccounts = IsNull(GetParentAccountId(accountId));
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return profileList;
        }

        //Associate profile to account.
        public virtual bool AssociateProfile(ProfileModel profileModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter AccountProfileId and ProfileId of profileModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { AccountProfileId = profileModel?.AccountProfileId, ProfileId = profileModel?.ProfileId });

            if (IsNull(profileModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (profileModel.AccountId < 1 && (profileModel.ProfileId < 1 && profileModel.AccountProfileId < 1 || IsNull(profileModel.ProfileIds)))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ProfileIdAndAccountNotLessThanZero);

            //If IsDefault is true, set default profile for account else associate profiles to account. 
            return profileModel.IsDefault.GetValueOrDefault() ? SetDefaultProfileForAccount(profileModel) : AssociateProfileToAccount(profileModel);
        }

        //UnAssociate associated profile list from account.
        public virtual bool UnAssociateProfile(AccountProfileModel profileModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (IsNull(profileModel) || string.IsNullOrEmpty(profileModel.AccountProfileIds) || profileModel.AccountId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelInvalid);
            ZnodeLogging.LogMessage("Input parameter AccountProfileId and ProfileId of profileModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { AccountProfileId = profileModel?.AccountProfileId, ProfileId = profileModel?.ProfileId });

            int defaultAccountProfileId = GetDefaultAccountProfileId(profileModel.AccountId);

            //Check if accountProfileIds that has to be deleted contains default id, other records will be deleted. 
            if (defaultAccountProfileId > 0)
            {
                ZnodeLogging.LogMessage("defaultAccountProfileId parameter of method DeleteNonDefaultAccountProfiles: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, defaultAccountProfileId);
                DeleteNonDefaultAccountProfiles(profileModel, defaultAccountProfileId);
            }

            //Delete account profiles.
            return DeleteAccountProfiles(profileModel.AccountProfileIds);
        }
        #endregion

        //Get parent account List
        public virtual ParentAccountListModel GetParentAccountList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            //If the global level user creation is set to true then set portal id to null.
            if (!(filters?.Any(x => string.Equals(x.FilterName, ZnodePortalEnum.PortalId.ToString(), StringComparison.CurrentCultureIgnoreCase))).GetValueOrDefault() &&
                !DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation)
                //Bind the filter conditions for the authorized portal access.
                BindUserPortalFilter(ref filters);

            //Bind the filter, sorts & paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("PageListModel to set SP parameter values: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<ParentAccountModel> objStoredProc = new ZnodeViewRepository<ParentAccountModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            var parentAccountList = objStoredProc.ExecuteStoredProcedureList("Znode_GetParentAccountList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);

            ZnodeLogging.LogMessage("Parent account list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, parentAccountList?.Count());

            ParentAccountListModel listModel = new ParentAccountListModel { ParentAccount = parentAccountList?.ToList() };
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return listModel;
        }
        #endregion

        #region Private Method   

        //Binds portalId and storeName to account details
        private void BindPortalDetailsToAccount(ZnodeAccount account, AccountModel accountModel)
        {
            var portalDetail = from portalAccount in _portalAccountRepository.Table
                               join portal in _portalRepository.Table on portalAccount.PortalId equals portal.PortalId
                               where portalAccount.AccountId == account.AccountId
                               select new { portalAccount.PortalId, portal.StoreName };
            ZnodeLogging.LogMessage("portalDetail: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, portalDetail);

            accountModel.PortalId = portalDetail?.FirstOrDefault()?.PortalId;
            accountModel.StoreName = portalDetail?.FirstOrDefault()?.StoreName;
            ZnodeLogging.LogMessage("PortalId and StoreName properties of accountModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { PortalId = accountModel?.PortalId, StoreName = accountModel?.StoreName });
        }

        //Gets the account based on accountName and portalId
        private ZnodeAccount GetAccountBasedOnNameAndPortalId(string accountName, int portalId)
        {
            ZnodeAccount account;
            List<ZnodeAccount> accounts = _accountRepository.Table.Where(x => x.Name.ToLower() == accountName.ToLower())?.ToList();
            FilterCollection filters = new FilterCollection { new FilterTuple(ZnodePortalAccountEnum.AccountId.ToString(), ProcedureFilterOperators.In, string.Join(",", accounts.Select(x => x.AccountId))) };
            filters.Add(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.In, portalId.ToString());

            int? selectedAccountId = _portalAccountRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause)?.AccountId;
            ZnodeLogging.LogMessage("selectedAccountId: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, selectedAccountId);

            account = accounts?.FirstOrDefault(x => x.AccountId == selectedAccountId);
            return account;
        }

        //Gets accountaddress by accountId
        private AddressModel GetAddressByAccountId(ZnodeAccount account)
        {
            //Get the Default Address for the account.
            ZnodeAddress defaultAddress = (from accountAddress in _accountAddressRepository.Table
                                           join address in _addressRepository.Table on accountAddress.AddressId equals address.AddressId
                                           where accountAddress.AccountId == account.AccountId
                                           select address).FirstOrDefault();        

            return defaultAddress?.ToModel<AddressModel>();
        }

        //Check if DepartmentName is already present or not.
        private bool IsDepartmentExist(string departmentName, int accountId)
        {
            //Get the parent account id.
            int? parentAccountId = GetParentAccountId(accountId);
            if (parentAccountId > 0)
                //Check if department already exists for account or parent account.
                return _departmentRepository.Table.Any(x => x.DepartmentName == departmentName && (x.AccountId == accountId || x.AccountId == parentAccountId));
            return _departmentRepository.Table.Any(x => x.DepartmentName == departmentName && x.AccountId == accountId);
        }

        //Check if Account Name is already present or not.
        private bool IsAccountNameExists(string name, int? accountId, int portalId)
        {
            //Removes the extra white space from name.
            name = name?.Trim();
            //If updating the record.
            if (accountId > 0)
            {
                if (portalId > 0)
                {
                    ZnodeAccount account = _accountRepository.Table.FirstOrDefault(x => x.AccountId == accountId && x.ZnodePortalAccounts.Select(y => y.PortalId == portalId).FirstOrDefault());
                    if (string.Equals(account.Name, name, StringComparison.InvariantCultureIgnoreCase))
                        return false;
                }
                else
                {
                    //If the accounts are created for all user i.e AllowGlobalLevelUserCreation is true.
                    ZnodeAccount account = _accountRepository.Table.FirstOrDefault(x => x.AccountId == accountId);
                    if (string.Equals(account.Name, name, StringComparison.InvariantCultureIgnoreCase))
                        return false;
                }
            }
            else
            {
                if (portalId > 0)
                    return _accountRepository.Table.Any(x => (x.Name.ToLower() == name.ToLower()) && x.ZnodePortalAccounts.Select(y => y.PortalId == portalId).FirstOrDefault());
                else
                    //If the accounts are created for all user i.e AllowGlobalLevelUserCreation is true.
                    return _accountRepository.Table.Any(x => x.Name.ToLower() == name.ToLower());
            }

            return false;
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (!Equals(expands, null) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    if (Equals(key.ToLower(), ZnodeAccountAddressEnum.ZnodeAddress.ToString().ToLower())) SetExpands(ZnodeAccountAddressEnum.ZnodeAddress.ToString(), navigationProperties);
                    if (Equals(key.ToLower(), ZnodeUserAddressEnum.ZnodeUser.ToString().ToLower())) SetExpands(ZnodeUserAddressEnum.ZnodeUser.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Account.
        private void SetAddressFlagsToFalse(AddressModel addressModel)
        {
            //Add filter for account id.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountAddressEnum.AccountId.ToString(), ProcedureFilterOperators.Equals, addressModel.AccountId.ToString()));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get assigned address list: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

            //Get customer address list on the basis of account id passed in filter.
            IList<ZnodeAccountAddress> assignedAddress = _accountAddressRepository.GetEntityList(whereClauseModel.WhereClause);
            ZnodeLogging.LogMessage("assignedAddress list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, assignedAddress?.Count());

            string commaSepratedAddressIds = string.Join(",", assignedAddress?.Select(x => x.AddressId));

            if (!string.IsNullOrEmpty(commaSepratedAddressIds))
            {
                filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeAccountAddressEnum.AddressId.ToString(), ProcedureFilterOperators.In, commaSepratedAddressIds));
                //Update IsDefaultBilling and IsDefaultShipping flag to false in database.
                AddressHelper.UpdateAddressFlagsToFalse(addressModel, filters);
            }
            else
                _addressRepository.Insert(addressModel.ToEntity<ZnodeAddress>());
        }

        //Saves the permission while creating the account.
        private void SavePermissions(int accountId)
        {
            List<ZnodeAccessPermission> accessPermissions = GetAccessPermissions();
            if (accessPermissions?.Count > 0)
            {
                IZnodeRepository<ZnodeAccountPermission> _accountPermissionRepository = new ZnodeRepository<ZnodeAccountPermission>();
                List<ZnodeAccountPermission> accountPermissionsToInsert = accessPermissions.Select(x => new ZnodeAccountPermission() { AccountId = accountId, AccountPermissionName = x.PermissionsName }).ToList();

                List<ZnodeAccountPermission> insertedaccountPermissions = _accountPermissionRepository.Insert(accountPermissionsToInsert)?.ToList();
                ZnodeLogging.LogMessage("accountPermissionsToInsert and insertedaccountPermissions list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { accountPermissionsToInsertListCount = accountPermissionsToInsert?.Count(), insertedaccountPermissionsListCount = insertedaccountPermissions?.Count() });

                if (insertedaccountPermissions?.Count > 0)
                {
                    IZnodeRepository<ZnodeAccountPermissionAccess> _accountPermissionAccessRepository = new ZnodeRepository<ZnodeAccountPermissionAccess>();

                    //Gets the account permission access list to insert.
                    List<ZnodeAccountPermissionAccess> accountPermissionAccess = (from access in accessPermissions
                                                                                  join accountPermission in insertedaccountPermissions on access.PermissionsName equals accountPermission.AccountPermissionName
                                                                                  where accountPermission.AccountPermissionName == access.PermissionsName
                                                                                  select new ZnodeAccountPermissionAccess { AccessPermissionId = access.AccessPermissionId, AccountPermissionId = accountPermission.AccountPermissionId }).ToList();
                    ZnodeLogging.LogMessage("accountPermissionAccess list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, accountPermissionAccess?.Count());

                    _accountPermissionAccessRepository.Insert(accountPermissionAccess);
                }
            }
        }

        //Get the Access permissions where type of permission is b2b.
        private List<ZnodeAccessPermission> GetAccessPermissions()
            => new ZnodeRepository<ZnodeAccessPermission>().Table.Where(x => x.TypeOfPermission == ZnodeRoleEnum.B2B.ToString())?.ToList();

        //Check if department is associated or not.
        private bool IsDepartmentAssociated(string departmentIds)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeDepartmentEnum.DepartmentId.ToString(), ProcedureFilterOperators.In, departmentIds.ToString()));

            IZnodeRepository<ZnodeDepartmentUser> _departmentUserRepository = new ZnodeRepository<ZnodeDepartmentUser>();
            List<int> associatedDepartments = _departmentUserRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause).Select(x => Convert.ToInt32(x.DepartmentId))?.Distinct()?.ToList();
            ZnodeLogging.LogMessage("associatedDepartments list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, associatedDepartments?.Count());

            if (associatedDepartments.Any())
            {
                filters.RemoveAll(x => x.FilterName == ZnodeDepartmentEnum.DepartmentId.ToString());

                //If some departments are associated, find unassociated departments from departmentIds to delete.
                string departmentsToDelete = string.Join(",", departmentIds.Split(',')?.Select(int.Parse).ToList()?.Except(associatedDepartments)?.ToList());
                if (!string.IsNullOrEmpty(departmentsToDelete))
                {
                    filters.Add(new FilterTuple(ZnodeDepartmentEnum.DepartmentId.ToString(), ProcedureFilterOperators.In, departmentsToDelete));
                    _departmentRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
                }
            }
            return associatedDepartments.Any();
        }

        //Save the portal for the account.
        private void InsertIntoPortalAccount(AccountDataModel model, bool isUpdate = false)
        {
            if (isUpdate)
            {
                //If record exists then return.
                if (_portalAccountRepository.Table.Any(x => x.AccountId == model.CompanyAccount.AccountId && x.PortalId == model.CompanyAccount.PortalId))
                    return;

                //Delete the existing entry.
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePortalAccountEnum.AccountId.ToString(), ProcedureFilterOperators.Equals, model.CompanyAccount.AccountId.ToString()));

                ZnodeLogging.LogMessage(_portalAccountRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection()))
                    ? Admin_Resources.SuccessAccountPortalInsert : Admin_Resources.ErrorAccountPortalInsert, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            }

            ZnodeLogging.LogMessage(_portalAccountRepository.Insert(new ZnodePortalAccount() { PortalId = model.CompanyAccount.PortalId > 0 ? model.CompanyAccount.PortalId : null, AccountId = model.CompanyAccount.AccountId })?.PortalAccountId > 0
                ? Admin_Resources.SuccessAccountPortalInsert : Admin_Resources.ErrorAccountPortalInsert, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
        }

        //Get portal id of account.
        private int? GetPortalId(int accountId)
            => _portalAccountRepository.Table.FirstOrDefault(x => x.AccountId == accountId)?.PortalId;

        //Set the Account model with parent accounts data.
        private void SetParentAccountData(ZnodeAccount account, AccountModel model)
        {
            ZnodeAccount parentAccount = _accountRepository.Table.FirstOrDefault(x => x.AccountId == account.ParentAccountId);
            model.ParentAccountName = (string.IsNullOrEmpty(parentAccount?.Name)) ? string.Empty : parentAccount.Name;
            //Get the assigned portal id.
            model.PortalId = GetPortalId(Convert.ToInt32(account.ParentAccountId));
        }

        //Set the filter to get the departments of account.
        private void SetFilters(FilterCollection filters)
        {
            //Check for accountId filter present in filters.
            if (filters.Exists(x => x.FilterName.Equals(ZnodeAccountEnum.AccountId.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                //Get the AccountId from filters.
                int accountId = Convert.ToInt32(filters.Where(x => x.Item1.Equals(ZnodeAccountEnum.AccountId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault()?.Item3);
                //Get the account.
                ZnodeAccount account = _accountRepository.Table.Where(x => x.AccountId == accountId)?.FirstOrDefault();
                if (IsNotNull(account))
                {
                    //If the account is child then set the filter for parent and child account ids only.
                    if (account.ParentAccountId > 0)
                    {
                        //Remove AccountId filter from filters.
                        filters.RemoveAll(x => x.Item1.Equals(ZnodeAccountEnum.AccountId.ToString(), StringComparison.InvariantCultureIgnoreCase));
                        filters.Add(new FilterTuple(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.In, string.Join(",", new List<int>() { account.AccountId, Convert.ToInt32(account.ParentAccountId) })));
                    }
                    else
                    {
                        //Remove AccountId filter from filters.
                        filters.RemoveAll(x => x.Item1.Equals(ZnodeAccountEnum.AccountId.ToString(), StringComparison.InvariantCultureIgnoreCase));
                        //Get all the child account ids.
                        List<int> allAccountIds = _accountRepository.Table.Where(x => x.ParentAccountId == accountId).Select(x => x.AccountId).ToList();
                        ZnodeLogging.LogMessage("allAccountIds list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, allAccountIds?.Count());
                        //Add the parent account Id.
                        allAccountIds.Add(accountId);
                        filters.Add(new FilterTuple(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.In, string.Join(",", allAccountIds)));
                    }
                }
            }
        }

        //Get the parent account id.
        private int? GetParentAccountId(int accountId)
            => _accountRepository.Table.FirstOrDefault(x => x.AccountId == accountId).ParentAccountId;

        //Get customer name by user id.
        private string GetCustomerName(FilterCollection filters)
        {
            int userId = Convert.ToInt32(filters.Find(x => string.Equals(x.Item1, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            if (userId > 0)
            {
                IZnodeRepository<ZnodeUser> _user = new ZnodeRepository<ZnodeUser>();
                return _user.Table.Where(x => x.UserId == userId)?.Select(x => x.FirstName + " " + x.LastName)?.FirstOrDefault();
            }
            return string.Empty;
        }

        //Set default profile for account.
        private bool SetDefaultProfileForAccount(ProfileModel profileModel)
        {
            UpdateAccountProfileList(profileModel.AccountId);
            return _accountProfileRepository.Update(profileModel.ToEntity<ZnodeAccountProfile>());
        }

        //Update account profile list of customer.
        private void UpdateAccountProfileList(int accountId)
        {
            List<ZnodeAccountProfile> listToUpdate = GetAccountProfileList(accountId);
            ZnodeLogging.LogMessage("Account profile list of customer count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, listToUpdate?.Count());

            if (listToUpdate?.Count > 0)
            {
                //Set the IsDefault flag to false. 
                listToUpdate.ForEach(x => { x.IsDefault = false; });

                listToUpdate.ForEach(x => _accountProfileRepository.Update(x));
            }
        }

        //Get account profile list to update.
        private List<ZnodeAccountProfile> GetAccountProfileList(int accountId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString());

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel to get account profile list to update: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

            //Gets the entity list.
            return _accountProfileRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }

        //Associate profile to account.
        private bool AssociateProfileToAccount(ProfileModel profileModel)
        {
            List<ZnodeAccountProfile> entitiesToInsert = new List<ZnodeAccountProfile>();
            string[] ProfileIds = profileModel.ProfileIds.Split(',');

            foreach (string item in ProfileIds)
                entitiesToInsert.Add(new ZnodeAccountProfile() { AccountId = profileModel.AccountId, ProfileId = Convert.ToInt32(item), IsDefault = false });

            //If non of the profile is associated to account, first profile will be saved as default profile.
            if (GetAccountProfileList(profileModel.AccountId)?.Count() <= 0)
                entitiesToInsert.FirstOrDefault(x => x.IsDefault = true);

            return _accountProfileRepository.Insert(entitiesToInsert)?.Count() > 0;
        }

        //Get default account profileId by account id.
        private int GetDefaultAccountProfileId(int accountId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountEnum.AccountId.ToString(), ProcedureFilterOperators.Equals, accountId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalCountryEnum.IsDefault.ToString(), ProcedureFilterOperators.Equals, ZnodeConstant.TrueValue));

            //Get account profile model which is default. 
            return (_accountProfileRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause)?.AccountProfileId).GetValueOrDefault();
        }

        //Delete non default account profiles and throws an exception default account profile can not be deleted.
        private void DeleteNonDefaultAccountProfiles(AccountProfileModel accountProfileModel, int defaultAccountProfileId)
        {
            //Check if accountProfileIds contains default defaultAccountProfileId.
            if (accountProfileModel.AccountProfileIds.Contains(Convert.ToString(defaultAccountProfileId)))
            {
                IList<int> defaultAccountProfile = new List<int>() { defaultAccountProfileId };

                //If accountProfileIds contains defaultAccountProfileId,it removes default account profile id so that is will not be deleted.
                accountProfileModel.AccountProfileIds = string.Join(",", accountProfileModel.AccountProfileIds.Split(',').Select(int.Parse).AsEnumerable().Except(defaultAccountProfile)?.ToList());

                //If defaultAccountProfileId is greater throws an exception that default account profile id can not be deleted.
                if (!string.IsNullOrEmpty(accountProfileModel.AccountProfileIds))
                    ZnodeLogging.LogMessage(DeleteAccountProfiles(accountProfileModel.AccountProfileIds) ? Admin_Resources.SuccessProfileFromAccountUnassociate : Admin_Resources.ErrorProfileFromAccountUnassociate, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

                throw new ZnodeException(ErrorCodes.DefaultDataDeletionError,Admin_Resources.DefaultAccountProfileDeleteError);
            }
        }

        //Delete account profiles. 
        private bool DeleteAccountProfiles(string accountProfileIds)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountProfileEnum.AccountProfileId.ToString(), ProcedureFilterOperators.In, accountProfileIds));

            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to delete account profiles: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, entityWhereClauseModel?.WhereClause);

            //Delete mapping of account against profile.
            bool isDeleted = _accountProfileRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
            ZnodeLogging.LogMessage(isDeleted ?Admin_Resources.SuccessProfileFromAccountUnassociate : Admin_Resources.ErrorProfileFromAccountUnassociate, string.Empty, TraceLevel.Info);
            return isDeleted;
        }
        //Replace sort key name
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, Constants.FilterKeys.OrderTotalWithCurrency, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Constants.FilterKeys.OrderTotalWithCurrency.ToLower(), Constants.FilterKeys.OrderTotal); }
            }
        }

        //Get where clause by User Approver Id
        private EntityWhereClauseModel GenerateWhereClauseForDeleteApprover(ParameterModel userApproverId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserApproverEnum.UserApproverId.ToString(), ProcedureFilterOperators.In, userApproverId.Ids.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        //Insert data for approver levels.
        private bool InsertApproverLevel(UserApproverModel userApproverModel, bool status)
        {
            FilterCollection filters = new FilterCollection
                {
                    { ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, userApproverModel.OmsQuoteId.ToString() },
                    { ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userApproverModel.UserId.ToString() },
                    new FilterTuple(ZnodeConstant.ShowAllApprovers.ToString().ToLower(), FilterOperators.Equals, true.ToString())
                };
            IAccountQuoteService _accountQuoteService = GetService<IAccountQuoteService>();
            UserApproverListModel model = _accountQuoteService?.GetUserApproverList(null, filters, null, null);
            if (IsNotNull(model) && model.UserApprovers.Any(x => x.ApproverUserId == userApproverModel.ApproverUserId))
            {
                ZnodeLogging.LogMessage(Admin_Resources.AlreadyAssignedApprover, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.AlreadyAssignedApprover);
            }
            ZnodeUserApprover userApproverdata = _userapproverRepository.Insert(userApproverModel?.ToEntity<ZnodeUserApprover>());
            if (userApproverdata?.UserApproverId > 0)
            {
                ZnodeLogging.LogMessage(userApproverdata.UserApproverId > 0 ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.ErrorFailedToCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                status = IsNotNull(userApproverdata);
            }
            return status;
        }

        //Update approver level data.
        private bool UpdateApproverLevel(UserApproverModel userApproverModel, out bool status)
        {
            status = _userapproverRepository.Update(userApproverModel.ToEntity<ZnodeUserApprover>());
            ZnodeLogging.LogMessage(status ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return status;
        }
        #endregion
    }
}