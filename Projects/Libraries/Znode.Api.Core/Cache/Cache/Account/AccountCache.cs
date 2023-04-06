using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class AccountCache : BaseCache, IAccountCache
    {
        private readonly IAccountService _service;
        private readonly IAccountQuoteService _accountQuoteService;


        public AccountCache(IAccountService accountService, IAccountQuoteService accountQuoteService)
        {
            _service = accountService;
            _accountQuoteService = accountQuoteService;
        }

        //Get user account list
        public virtual string GetAccountList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Account list
                AccountListModel Accounts = _service.GetAccountList(Expands, Filters, Sorts, Page);
                if (Accounts?.Accounts?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AccountListResponse response = new AccountListResponse { Accounts = Accounts.Accounts };
                    response.MapPagingDataFromModel(Accounts);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #region Account Notes 
        public virtual string GetAccountNote(int noteId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                NoteModel note = _service.GetAccountNote(noteId);
                if (IsNotNull(note))
                {
                    NoteResponse response = new NoteResponse { Note = note };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAccountNotes(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                NoteListModel list = _service.GetAccountNotes(Expands, Filters, Sorts, Page);
                if (IsNotNull(list?.Notes))
                {
                    NoteListResponse response = new NoteListResponse { Notes = list.Notes, CustomerName = list.CustomerName };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region  Account Department
        public virtual string GetAccountDepartment(int departmentId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountDepartmentModel department = _service.GetAccountDepartment(departmentId);
                if (IsNotNull(department))
                {
                    AccountDepartmentResponse response = new AccountDepartmentResponse { Department = department };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAccountDepartments(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountDepartmentListModel list = _service.GetAccountDepartments(Expands, Filters, Sorts, Page);
                if (list?.Departments?.Count > 0)
                {
                    AccountDepartmentListResponse response = new AccountDepartmentListResponse { Departments = list?.Departments };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        //Get account by account Id.
        public virtual string GetAccount(int accountId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get profile by profile id.
                AccountModel accountDetails = _service.GetAccount(accountId);
                if (!Equals(accountDetails, null))
                {
                    AccountResponse response = new AccountResponse { Account = accountDetails };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }        

        //Get user approver list.
        public virtual string GetUserApproverList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                UserApproverListModel userApproverListModel = _accountQuoteService.GetUserApproverList(Expands, Filters, Sorts, Page);

                if (IsNotNull(userApproverListModel))
                {
                    UserApproverListResponse response = new UserApproverListResponse { UserApproverList = userApproverListModel };
                    response.MapPagingDataFromModel(userApproverListModel);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get level list.
        public virtual string Getlevelslist(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get level list.
                ApproverLevelListModel approverLevelList = _service.Getlevelslist();

                if (IsNotNull(approverLevelList))
                {
                    ApproverLevelListResponse response = new ApproverLevelListResponse { ApproverLevelList = approverLevelList.ApproverLevelList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get account on the basis of provided name
        public virtual string GetAccountByName(string accountName, int portalId, string routeUri, string routeTemplate)
        {
            //Get data from cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountModel accountDetails = _service.GetAccountByName(accountName, Expands, portalId);
                if (!Equals(accountDetails, null))
                {
                    AccountResponse response = new AccountResponse { Account = accountDetails };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                } 
            }
            return data;
        }

        #region Address
        public virtual string GetAddressList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Account list
                AddressListModel Accounts = _service.GetAddressList(Expands, Filters, Sorts, Page);
                if (Accounts?.AddressList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AccountListResponse response = new AccountListResponse { AddressList = Accounts.AddressList };
                    response.MapPagingDataFromModel(Accounts);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAccountAddress(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AddressModel address = _service.GetAccountAddress(Filters, Expands);
                if (IsNotNull(address))
                {
                    //Get response and insert it into cache.
                    AccountResponse response = new AccountResponse { AccountAddress = address };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Account Order
        public virtual string GetAccountUserOrderList(int accountId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrdersListModel orderList = _service.GetAccountUserOrderList(accountId, Expands, Filters, Sorts, Page);
                if (IsNotNull(orderList))
                {
                    OrderListResponse response = new OrderListResponse { OrderList = orderList };

                    response.MapPagingDataFromModel(orderList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get account by account Code.
        public virtual string GetAccountByCode(string accountCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get account details by account Code.
                AccountModel accountDetails = _service.GetAccountByCode(accountCode);
                if (!Equals(accountDetails, null))
                {
                    AccountResponse response = new AccountResponse { Account = accountDetails };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Account profile

        //Get a list associated/unassociated profiles for account.
        public virtual string GetAssociatedUnAssociatedProfile(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get associated/unassociated profiles for account.
                ProfileListModel list = _service.GetAssociatedUnAssociatedProfile(Expands, Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    //Create response.
                    ProfileListResponse response = new ProfileListResponse { Profiles = list?.Profiles, HasParentAccounts = list.HasParentAccounts };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion

        //Get parent account list
        public virtual string GetParentAccountList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                //Parent account list
                ParentAccountListModel parentAccountList = _service.GetParentAccountList(Expands, Filters, Sorts, Page);
                if (parentAccountList?.ParentAccount?.Count > 0)
                {
                    //Get response and insert it into cache.
                    ParentAccountListResponse response = new ParentAccountListResponse { ParentAccount = parentAccountList.ParentAccount };
                    response.MapPagingDataFromModel(parentAccountList);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}