using MvcSiteMapProvider;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class AccountController : BaseController
    {
        #region Private ReadOnly members
        private readonly IUserAgent _userAgent;
        private readonly IAccountAgent _accountAgent;
        private readonly IAccountQuoteAgent _accountQuoteAgent;
        private readonly IAccessPermissionAgent _accessPermissionAgent;
        private readonly ICartAgent _cartAgent;
        private readonly IOrderAgent _orderAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly string AddSubAccountAsidePanel = "_AddSubAccountAsidePanel";
        private readonly string AddAccountCustomerAddressAsidePanel = "_AddAccountCustomerAddressAsidePanel";
        private readonly string ModelStatePortalIds = "PortalIds";
        private readonly string storeListAsidePanelPopup = "_asideStorelistPanelPopup";
        private readonly string EditAccountView = "_EditAccount";
        private readonly string PartialApprovalLevel = "_PartialApprovalLevel";
        private readonly string EditApproverView = "_EditApproverView";
        private readonly string createEditView = "_createEditView";
        private readonly string _manageApproverView = "~/Views/Account/_manageApproverView.cshtml";
        private readonly IHelperAgent _helperAgent;

        protected readonly string parentAccountListAsidePanel= "_asideParentAccountListPanel";
        #endregion

        #region Public Constructor
        public AccountController(IUserAgent userAgent, IAccountAgent accountAgent, IAccountQuoteAgent accountQuoteAgent, IAccessPermissionAgent accessPermissionAgent, ICartAgent cartAgent, IOrderAgent orderAgent, IStoreAgent storeAgent, IHelperAgent helperAgent)
        {
            _userAgent = userAgent;
            _accountAgent = accountAgent;
            _accountQuoteAgent = accountQuoteAgent;
            _accessPermissionAgent = accessPermissionAgent;
            _cartAgent = cartAgent;
            _orderAgent = orderAgent;
            _storeAgent = storeAgent;
            _helperAgent = helperAgent;
        }
        #endregion

        #region Public Methods

        #region Account
        //Get account list.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleCMS", Key = "Account", Area = "", ParentKey = "OMS")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeAccount.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAccount.ToString(), model);
            //Get Account list.
            AccountListViewModel accountList = _accountAgent.GetAccountList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            accountList.GridModel = FilterHelpers.GetDynamicGridModel(model, accountList?.AccountList, GridListType.ZnodeAccount.ToString(), string.Empty, null, true, true, accountList?.GridModel?.FilterColumn?.ToolMenuList);
            accountList.GridModel.TotalRecordCount = accountList.TotalResults;

            //Returns the Accounts list.
            return ActionView(accountList);
        }

        //Get sub account list.
        public virtual ActionResult SubAccountList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int parentAccountId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_GetAccountListWithAddress.ToString(), model);
            //Get Account list.
            AccountListViewModel accountList = _accountAgent.GetSubAccountList(parentAccountId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model.
            accountList.GridModel = FilterHelpers.GetDynamicGridModel(model, accountList?.AccountList, GridListType.View_GetAccountListWithAddress.ToString(), string.Empty, null, true, true, accountList?.GridModel?.FilterColumn?.ToolMenuList);
            accountList.GridModel.TotalRecordCount = accountList.TotalResults;
            accountList.AccountId = parentAccountId;
            //Returns the Accounts list.
            return ActionView("_SubAccountList", accountList);
        }

        //Create account.
        public virtual ActionResult Create()
        {
            AccountDataViewModel companyDataViewModel = new AccountDataViewModel();

            //Get available countries to bind on Drop Down.
            companyDataViewModel.CompanyAccount = new AccountViewModel();
            companyDataViewModel.CompanyAccount.Address = new AddressViewModel() { Countries = HelperMethods.GetCountries(), IsDefaultBilling = true, IsDefaultShipping = true };
            companyDataViewModel.CompanyAccount.IsDefault = true;
            return ActionView(companyDataViewModel);
        }

        //Create account.
        [HttpPost]
        public virtual ActionResult Create(AccountDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Create the Company Account.
                model = _accountAgent.Create(model);
                if (!model.HasError && model.CompanyAccount.AccountId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<AccountController>(x => x.EditAccount(model.CompanyAccount.AccountId));
                }
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }
            if (IsNotNull(model.CompanyAccount))
            {
                model.CompanyAccount.Address.Countries = HelperMethods.GetCountries();
            }
            else
            {
                //Bind Parent Account List.
                model.CompanyAccount = new AccountViewModel() {Address = new AddressViewModel() { Countries = HelperMethods.GetCountries(), IsDefaultBilling = true, IsDefaultShipping = true } };
            }
            return ActionView(model);
        }

        //Create sub account
        public virtual ActionResult CreateSubAccount(int parentAccountId = 0)
        {
            //If the parent id is lost then redirect to list page.
            if (parentAccountId == 0)
                return RedirectToAction<AccountController>(x => x.List(null));

            //Get the Account Details by id.
            AccountDataViewModel model = _accountAgent.GetAccountById(parentAccountId);
            _accountAgent.SetAccountDataModel(parentAccountId, model);
            model.CompanyAccount.AccountId = 0;
            return ActionView(AddSubAccountAsidePanel, model);
        }

        //Create sub account
        [HttpPost]
        public virtual ActionResult CreateSubAccount(AccountDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Create the Company Sub Account.
                model = _accountAgent.Create(model);

                //Set notification message.
                SetNotificationMessage(!model.HasError && model.CompanyAccount.AccountId > 0 ?
                    GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage) : GetErrorNotificationMessage(model.ErrorMessage));
            }
            else
                // Bind the Available Countries.
                SetAddressViewModel(model.CompanyAccount.Address);

            return RedirectToAction<AccountController>(x => x.SubAccountList(null, model.CompanyAccount.ParentAccountId));
        }

        public virtual ActionResult Manage(int accountId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AccountAddressList.ToString(), model);
            //Get the Account Details by id.
            AccountDataViewModel accountDataViewModel = _accountAgent.GetAccountById(accountId);
            HelperMethods.SetAccountIdFilters(model.Filters, accountId);
            AddressListViewModel addressList = _accountAgent.GetAddressList(accountId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            accountDataViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, addressList?.AddressList, GridListType.AccountAddressList.ToString(), string.Empty, null, true, true, addressList?.GridModel?.FilterColumn?.ToolMenuList);
            accountDataViewModel.GridModel.TotalRecordCount = addressList.TotalResults;
            return (Request.IsAjaxRequest()) ? PartialView("_ManagePartial", accountDataViewModel) : ActionView(accountDataViewModel);
        }

        //Get account on the basis of account id.
        public virtual ActionResult EditAccount(int accountId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            //Get the Account Details by id.
            AccountDataViewModel model = _accountAgent.GetAccountById(accountId);
            if (Equals(model?.CompanyAccount?.Address, null))
                model.CompanyAccount.Address = new AddressViewModel() { Countries = HelperMethods.GetPortalAssociatedCountries(model.CompanyAccount.PortalId.GetValueOrDefault()) };
            else
                model.CompanyAccount.Address.Countries = HelperMethods.GetPortalAssociatedCountries(model.CompanyAccount.PortalId.GetValueOrDefault());
            return ActionView(EditAccountView, model);
        }

        //Update account.
        [HttpPost]
        public virtual ActionResult EditAccount(AccountDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_accountAgent.UpdateAccount(model).HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<AccountController>(x => x.EditAccount(model.CompanyAccount.AccountId));
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }

            //Bind the Countries List.
            model.CompanyAccount.Address.Countries = HelperMethods.GetCountries();

            return ActionView(EditAccountView, model);
        }

        //Edit sub account.
        public virtual ActionResult EditSubAccount(int accountId = 0)
        {
            //If the account Id is lost then redirect to list page.
            if (accountId == 0)
                return RedirectToAction<AccountController>(x => x.List(null));

            //Get the Account Details by id.
            AccountDataViewModel model = _accountAgent.GetAccountById(accountId);
            if (model?.CompanyAccount?.AccountId > 0)
            {
                if (Equals(model.CompanyAccount.Address, null))
                    model.CompanyAccount.Address = new AddressViewModel() { Countries = HelperMethods.GetCountries() };
                else
                    model.CompanyAccount.Address.Countries = HelperMethods.GetCountries();
            }
            return ActionView(AddSubAccountAsidePanel, model);
        }

        //Edit sub account.
        [HttpPost]
        public virtual ActionResult EditSubAccount(AccountDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_accountAgent.UpdateAccount(model).HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<AccountController>(x => x.SubAccountList(null, model.CompanyAccount.ParentAccountId));
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }

            //Bind the Countries List.
            model.CompanyAccount.Address.Countries = HelperMethods.GetCountries();

            return ActionView(AddSubAccountAsidePanel, model);
        }

        //Delete account.
        public virtual JsonResult Delete(string accountId)
        {
            if (!string.IsNullOrEmpty(accountId))
            {
                string message = string.Empty;
                //Delete the Account based on the id.
                bool status = _accountAgent.DeleteAccount(accountId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : string.IsNullOrEmpty(message) ? Admin_Resources.ErrorFailToDeleteParentAccount : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get the account's portal Id.
        public virtual JsonResult GetAccountsPortal(int accountId)
        {
            if (accountId > 0)
            {
                AccountDataViewModel account = _accountAgent.GetAccountById(accountId);
                if (account?.CompanyAccount?.AccountId > 0)
                    return Json(new { portalId = account.CompanyAccount.PortalId, storeName = account.CompanyAccount.StoreName }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { portalId = 0 }, JsonRequestBehavior.AllowGet);
        }

        //Get Portal List
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserPortalList.ToString(), model);

            StoreListViewModel storeList = _storeAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView(storeListAsidePanelPopup, storeList);
        }

        //Get Catalog List
        public virtual ActionResult GetCatalogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeStoreCatalog.ToString(), model);
            PortalCatalogListViewModel catalogList = _storeAgent.GetPublishCatalogList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            catalogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catalogList.PortalCatalogs, GridListType.ZnodeStoreCatalog.ToString(), string.Empty, null, true);

            catalogList.GridModel.TotalRecordCount = catalogList.TotalResults;
            return ActionView("_asideCatalogListPanel", catalogList);
        }

        public virtual JsonResult GetCatalog(string catalogName)
          => Json(_storeAgent.GetCatalogList(catalogName), JsonRequestBehavior.AllowGet);

        //Gets parent account list based on portalId.
        [HttpGet]
        public virtual JsonResult GetParentAccountList(int portalId = 0)
            => Json(_accountAgent.GetParentAccountList(portalId), JsonRequestBehavior.AllowGet);

        //Gets Countries list based on portalId.
        [HttpGet]
        public virtual JsonResult GetCountryBasedOnPortalId(int portalId)
        {
            if (portalId > 0)
            {
                AddressViewModel objAddressViewModel = new AddressViewModel() { Countries = HelperMethods.GetPortalAssociatedCountries(portalId) };

                return Json(objAddressViewModel.Countries, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        //Check account name already exists or not.
        public virtual JsonResult IsAccountNameExists(string accountName, int accountId, int portalId)
          => Json(!_accountAgent.CheckAccountNameExist(accountName, accountId, portalId), JsonRequestBehavior.AllowGet);
        #endregion

        #region Account Notes
        //Create account note.
        [HttpGet]
        public virtual ActionResult CreateAccountNote(int accountId)
         => ActionView(new NoteViewModel { AccountId = accountId });

        //Create account note.
        [HttpPost]
        public virtual ActionResult CreateAccountNote(NoteViewModel noteViewModel)
        {
            if (ModelState.IsValid)
            {
                noteViewModel = _accountAgent.CreateAccountNote(noteViewModel);
                if (!noteViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<AccountController>(x => x.EditAccountNote(noteViewModel.NoteId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(noteViewModel.ErrorMessage));
            return RedirectToAction<AccountController>(x => x.CreateAccountNote(noteViewModel.AccountId.GetValueOrDefault()));
        }

        //Get account note list.
        public virtual ActionResult AccountNoteList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int accountId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeNote.ToString(), model);
            _accountAgent.SetFiltersForAccountId(model?.Filters, accountId);
            NoteListViewModel noteList = _accountAgent.GetAccountNotes(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            noteList.AccountId = accountId;
            noteList.GridModel = FilterHelpers.GetDynamicGridModel(model, noteList?.Notes, GridListType.ZnodeNote.ToString(), string.Empty, null, true, true, noteList?.GridModel?.FilterColumn?.ToolMenuList);
            noteList.GridModel.TotalRecordCount = noteList.TotalResults;

            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = _accountAgent.GetAccountById(accountId);
            noteList.HasParentAccounts = accountDetails.HasParentAccounts;
            noteList.AccountName = accountDetails?.CompanyAccount?.Name;
            return ActionView(noteList);
        }

        //Get account note on the basis of note id.
        [HttpGet]
        public virtual ActionResult EditAccountNote(int noteId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView("CreateAccountNote", _accountAgent.GetAccountNote(noteId));
        }
        //Edit account note.
        [HttpPost]
        public virtual ActionResult EditAccountNote(NoteViewModel noteViewModel)
        {
            noteViewModel = _accountAgent.UpdateAccountNote(noteViewModel);
            if (!noteViewModel.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<AccountController>(x => x.EditAccountNote(noteViewModel.NoteId));
            }
            SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<AccountController>(x => x.EditAccountNote(noteViewModel.NoteId));
        }

        //Delete account note.
        public virtual JsonResult DeleteAccountNote(string noteId)
        {
            if (!string.IsNullOrEmpty(noteId))
            {
                string message = string.Empty;
                bool status = _accountAgent.DeleteAccountNote(noteId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : string.IsNullOrEmpty(message) ? Admin_Resources.ErrorFailedToDelete : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Permissions
        //Gets the permission list.
        public virtual ActionResult PermissionList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int accountId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAccountPermission.ToString(), model);
            //Gets the list of account permissions.
            AccountPermissionListViewModel accountPermissionList = _accessPermissionAgent.AccountPermissionList(accountId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Set the grid model
            accountPermissionList.GridModel = FilterHelpers.GetDynamicGridModel(model, accountPermissionList?.AccountPermissionList, GridListType.ZnodeAccountPermission.ToString(), string.Empty, null, true);
            accountPermissionList.GridModel.TotalRecordCount = accountPermissionList.TotalResults;

            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = _accountAgent.GetAccountById(accountId);
            accountPermissionList.HasParentAccounts = accountDetails.HasParentAccounts;
            accountPermissionList.AccountName = accountDetails?.CompanyAccount?.Name;
            //return view
            return ActionView("_AccountPermissionList", accountPermissionList);
        }

        //Gets the view of create account permission.
        public virtual ActionResult CreateAccountPermission(int accountId)
            => View("CreateEditAccountPermission", new AccountPermissionViewModel() { AccountId = accountId, AccessPermissions = _accessPermissionAgent.AccessPermissionList() });

        //Submits the data of the created form of account permission.
        [HttpPost]
        public virtual ActionResult CreateAccountPermission(AccountPermissionViewModel model)
        {
            if (ModelState.IsValid)
                SetNotificationMessage(_accessPermissionAgent.CreateAccountPermission(model) ? GetSuccessNotificationMessage(Admin_Resources.RecordAddedSuccessMessage) : GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
            return RedirectToAction<AccountController>(x => x.PermissionList(null, model.AccountId));
        }

        //Returns update status of edit account permission.
        public virtual ActionResult EditAccountPermission(string data)
        {
            string message = string.Empty;
            if (ModelState.IsValid && IsNotNull(data))
            {
                AccountPermissionViewModel accountPermission = _accessPermissionAgent.UpdateAccountPermission(data);
                if (!accountPermission.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = accountPermission.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete account permission.
        public virtual JsonResult DeleteAccountPermission(string accountPermissionId)
        {
            string message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(accountPermissionId))
            {
                bool status = _accessPermissionAgent.DeleteAccountPermission(accountPermissionId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeletePermissionErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Check Permission Already exists or not.
        [HttpPost]
        public virtual JsonResult IsAccountPermissionExist(string AccountPermissionName, int AccountId, int AccountPermissionId = 0)
          => Json(!_accessPermissionAgent.CheckAccountPermissionExist(AccountPermissionName, AccountId, AccountPermissionId), JsonRequestBehavior.AllowGet);
        #endregion

        #region Account Department

        //Create account department.
        [HttpGet]
        public virtual ActionResult CreateAccountDepartment(int accountId)
             => ActionView("_CreateEditAccountDepartment", new AccountDepartmentViewModel { AccountId = accountId });

        //Create account department.
        [HttpPost]
        public virtual ActionResult CreateAccountDepartment(AccountDepartmentViewModel departmentViewModel)
        {
            if (ModelState.IsValid)
            {
                departmentViewModel = _accountAgent.CreateAccountDepartment(departmentViewModel);
                if (!departmentViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<AccountController>(x => x.AccountDepartmentList(null, departmentViewModel.AccountId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(departmentViewModel.ErrorMessage));
            return RedirectToAction<AccountController>(x => x.AccountDepartmentList(null, departmentViewModel.AccountId));
        }

        //Get account department list.
        public virtual ActionResult AccountDepartmentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int accountId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeDepartment.ToString(), model);

            _accountAgent.SetFiltersForAccountId(model?.Filters, accountId);
            AccountDepartmentListViewModel departmentList = _accountAgent.GetAccountDepartments(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            departmentList.AccountId = accountId;
            departmentList.GridModel = FilterHelpers.GetDynamicGridModel(model, departmentList?.Departments, GridListType.ZnodeDepartment.ToString(), string.Empty, null, true, true, departmentList?.GridModel?.FilterColumn?.ToolMenuList);
            departmentList.GridModel.TotalRecordCount = departmentList.TotalResults;
            return ActionView(departmentList);
        }

        //Update account department.
        public virtual JsonResult EditAccountDepartment(int departmentId, int accountIdToRedirect, int accountId, string data)
        {
            if (IsNotNull(data))
            {
                AccountDepartmentViewModel departmentViewModel = JsonConvert.DeserializeObject<AccountDepartmentViewModel[]>(data)[0];
                departmentViewModel = new AccountDepartmentViewModel { DepartmentId = departmentId, AccountIdToRedirect = accountIdToRedirect, AccountId = accountId, DepartmentName = departmentViewModel.DepartmentName };
                bool status = _accountAgent.UpdateAccountDepartment(departmentViewModel);

                string message = status ? PIM_Resources.UpdateMessage : string.IsNullOrEmpty(departmentViewModel.ErrorMessage) ? Admin_Resources.UpdateErrorMessage : departmentViewModel.ErrorMessage;

                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete account department.
        public virtual JsonResult DeleteAccountDepartment(string departmentId)
        {
            if (!string.IsNullOrEmpty(departmentId))
            {
                string message = string.Empty;
                bool status = _accountAgent.DeleteAccountDepartment(departmentId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : string.IsNullOrEmpty(message) ? Admin_Resources.ErrorFailedToDelete : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Customers

        /// <summary>
        /// This method will fetch the list of all the customer account details.
        /// </summary>
        /// <param name="model">Filtercollection model</param>
        /// <returns>Returns the list of admin account details.</returns>
        public virtual ActionResult CustomersList(int accountId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            string currentUserName = HttpContext.User.Identity.Name;
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAccountsCustomer.ToString(), model);
            //Set filters for Account id and IsAccountCustomer
            HelperMethods.SetAccountIdFilters(model.Filters, accountId);
            HelperMethods.SetIsAccountCustomerFilter(model.Filters, 1);

            //Get the list of customers            
            CustomerListViewModel customerViewModel = _accountAgent.GetCustomerAccountList(currentUserName, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model      
            customerViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, customerViewModel.List, GridListType.ZnodeAccountsCustomer.ToString(), string.Empty, null, true, true, customerViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            customerViewModel.GridModel.TotalRecordCount = customerViewModel.TotalResults;
            customerViewModel.AccountId = accountId;
            //Returns the customer list view
            return ActionView(customerViewModel);
        }

        /// <summary>
        /// Create Customer User.
        /// </summary>
        /// <returns>Create view for Customer User.</returns>
        public virtual ActionResult CustomerCreate(int accountId = 0)
        {
            //If the account Id is lost then redirect to list page.
            if (accountId == 0)
                return RedirectToAction<AccountController>(x => x.List(null));

            CustomerAccountViewModel customerViewModel = new CustomerAccountViewModel();
            _accountAgent.SetCustomerAccountViewModel(customerViewModel, accountId);
            return ActionView("_AddCustomerAsidePanel", customerViewModel);
        }

        /// <summary>
        /// This method will add Customer User.
        /// </summary>
        /// <returns>Returns created customer account.</returns>
        [HttpPost]
        public virtual ActionResult CustomerCreate(CustomerAccountViewModel customerViewModel)
        {
            ModelState.Remove(ModelStatePortalIds);
            if (ModelState.IsValid)
            {
                CustomerAccountViewModel model = _accountAgent.CreateCustomerAccount(customerViewModel);

                if (IsNotNull(model) && !model.HasError)
                {
                    SetNotificationMessage(model.IsEmailSentFailed ? GetErrorNotificationMessage(Admin_Resources.CustomerCreationSucccessWithEmailFailedMessage) : GetSuccessNotificationMessage(Admin_Resources.CustomerCreationSuccessMessage));
                    return RedirectToAction<AccountController>(x => x.CustomersList(Convert.ToInt32(model.AccountId), null));
                }
                else
                {
                    _accountAgent.SetCustomerAccountViewModel(customerViewModel, customerViewModel.AccountId.Value);
                    SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                }
            }
            return RedirectToAction<AccountController>(x => x.CustomersList(Convert.ToInt32(customerViewModel.AccountId), null));
        }

        /// <summary>
        /// Edit Customer User.
        /// </summary>
        /// <param name="id">User Id whose account has to be updated.</param>
        /// <returns>Returns updated customer account.</returns>
        [HttpGet]
        public virtual ActionResult CustomerEdit(int userId, int accountId)
        {
            if (userId > 0)
            {
                CustomerAccountViewModel customerAccountDetails = _accountAgent.GetAccountsCustomer(userId);
                _accountAgent.SetCustomerAccountViewModel(customerAccountDetails, accountId);
                return ActionView("_AddCustomerAsidePanel", customerAccountDetails);
            }
            return RedirectToAction<AccountController>(x => x.CustomersList(accountId, null));
        }

        /// <summary>
        /// This method update the users account details.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Returns updated account data.</returns>
        [HttpPost]
        public virtual ActionResult CustomerEdit(CustomerAccountViewModel model)
        {
            ModelState.Remove(ModelStatePortalIds);
            if (ModelState.IsValid)
            {
                string errorMessage = string.Empty;
                SetNotificationMessage(_accountAgent.UpdateCustomerAccount(model, out errorMessage) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                    : GetErrorNotificationMessage(errorMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<AccountController>(x => x.CustomersList(Convert.ToInt32(model.AccountId), null));
        }

        /// <summary>
        /// Delete customer account.
        /// </summary>
        /// <param name="accountId">User Ids of customer.</param>
        /// <returns>Returns true if account deleted successfully, else return false.</returns>
        public virtual JsonResult CustomerDelete(string userId)
        {
            string message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(userId))
            {
                string currentUserName = HttpContext.User.Identity.Name;
                bool status = _userAgent.DeleteCustomerAccount(userId, currentUserName, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method will enable or disable the customer account.
        /// </summary>
        /// <param name="id">User Ids whose accounts have to be enabled or disabled.</param>
        /// <param name="isLock">To check if customer account is locked or not.</param>
        /// <param name="accountId">accountId</param>
        /// <returns>Returns if the customer account is enabled or disabled.</returns>
        public virtual ActionResult CustomerEnableDisableAccount(int accountId, string userId, bool isLock, bool isRedirect = true)
        {
            string message = Admin_Resources.LockMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(userId))
                status = _userAgent.EnableDisableUser(userId, !isLock, out message);

            if (!isRedirect)
                return Json(new { status = status, message = (status && isLock) ? Admin_Resources.UnlockMessage : (status && !isLock) ? Admin_Resources.LockMessage : message }, JsonRequestBehavior.AllowGet);
            else
            {
                if (status && isLock)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UnlockMessage));
                else if (status && !isLock)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.LockMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
                return RedirectToAction<AccountController>(x => x.CustomersList(accountId, null));
            }
        }

        // Reset the password for the user in bulk.       
        public virtual ActionResult BulkResetPassword(int accountId, string userId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(userId))
                if (!_userAgent.BulkResetPassword(userId, out message))
                    return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);

            return Json(new { status = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get UnAssociated Customer(s).
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="accountId">Account Id</param>
        /// <returns>Create view for unassociated Customer.</returns>
        public virtual ActionResult GetUnAssociatedCustomerList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0, int accountId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAccountUnAssociatedCustomer.ToString(), model);

            //Get the list of customers            
            CustomerListViewModel customerViewModel = _accountAgent.GetUnAssociatedCustomerList(portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model      
            customerViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, customerViewModel.List, GridListType.ZnodeAccountUnAssociatedCustomer.ToString(), string.Empty, null, true, true, customerViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            customerViewModel.GridModel.TotalRecordCount = customerViewModel.TotalResults;
            customerViewModel.AccountId = accountId;
            customerViewModel.PortalId = portalId;
            //Returns the customer list view
            return ActionView("_UnAssociatedCustomerAccount", customerViewModel);
        }

        /// <summary>
        /// This method associate users with account.
        /// </summary>
        /// <param name="userIds">User Ids whose accounts have to be updated </param>
        /// <param name="accountId">Account Id</param>
        /// <returns>Returns status with message.</returns>
        [HttpPost]
        public virtual JsonResult AssociateUsersWithAccount(string userIds, int accountId)
        {
            if (IsNotNull(userIds) && accountId > 0)
            {
                UserAccountViewModel userModel = new UserAccountViewModel() { UserIds = userIds, AccountId = accountId };
                string message = string.Empty;
                bool status = _accountAgent.UpdateUserAccountMapping(userModel, out message);
                return Json(new { status = status, message = status ? Admin_Resources.SuccessAccountUsersAssociate : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Address
        //Create address.
        [HttpGet]
        public virtual ActionResult CreateAddress(int accountId, int portalId)
            => ActionView(AddAccountCustomerAddressAsidePanel, new AddressViewModel { AccountId = accountId, Countries = HelperMethods.GetPortalAssociatedCountries(portalId) });

        //Create address.
        [HttpPost]
        public virtual ActionResult CreateAddress(AddressViewModel addressViewModel)
        {
            if (ModelState.IsValid)
            {
                addressViewModel = _accountAgent.CreateAccountAddress(addressViewModel);
                if (!addressViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<AccountController>(x => x.Manage(addressViewModel.AccountId, null));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(addressViewModel.ErrorMessage));
            return RedirectToAction<AccountController>(x => x.Manage(addressViewModel.AccountId, null));
        }

        //Get address on the basis of account address id.
        [HttpGet]
        public virtual ActionResult EditAddress(int accountAddressId, int portalId)
        {
            AddressViewModel address = _accountAgent.GetAccountAddress(accountAddressId);
            address.Countries = HelperMethods.GetPortalAssociatedCountries(portalId);
            return ActionView(AddAccountCustomerAddressAsidePanel, address);
        }

        //Edit address.
        [HttpPost]
        public virtual ActionResult EditAddress(AddressViewModel addressViewModel)
        {
            addressViewModel = _accountAgent.UpdateAccountAddress(addressViewModel);
            if (!addressViewModel.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<AccountController>(x => x.Manage(addressViewModel.AccountId, null));
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<AccountController>(x => x.EditAddress(addressViewModel.AccountAddressId, addressViewModel.PortalId));
        }

        //Delete address.
        public virtual JsonResult DeleteAddress(string accountAddressId)
        {
            if (!string.IsNullOrEmpty(accountAddressId))
            {
                string message = string.Empty;
                bool status = _accountAgent.DeleteAccountAddress(accountAddressId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : string.IsNullOrEmpty(message) ? Admin_Resources.ErrorFailedToDelete : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Associate Price
        // Get Associated Price list for Account.
        public virtual ActionResult GetAssociatedPriceListForAccount([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int accountId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedPriceListToAccount.ToString(), model);
            //Get Price list.
            PriceListViewModel priceList = _accountAgent.GetPriceList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, accountId);

            //Get the grid model.
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.AssociatedPriceListToAccount.ToString(), string.Empty, null, true, true, priceList?.GridModel?.FilterColumn?.ToolMenuList);
            priceList.GridModel.TotalRecordCount = priceList.TotalResults;

            return ActionView("_AssociatedPriceListForAccount", priceList);
        }

        // Get UnAssociated Price list for Account.
        public virtual ActionResult GetUnAssociatedPriceListForAccount([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int accountId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedPriceList.ToString(), model);
            //Get Price list.
            PriceListViewModel priceList = _accountAgent.GetUnAssociatedPriceList(accountId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            priceList.AccountId = accountId;
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.UnAssociatedPriceList.ToString(), string.Empty, null, true);
            priceList.GridModel.TotalRecordCount = priceList.TotalResults;

            return ActionView("_UnAssociatedPriceListForAccount", priceList);
        }

        //Associate UnAssociated Price List to Account.
        public virtual JsonResult AssociatePriceListToAccount(int accountId, string priceListId)
        {
            SetNotificationMessage(_accountAgent.AssociatePriceList(accountId, priceListId)
                ? GetSuccessNotificationMessage(Admin_Resources.AccountPriceSuccessMessage) : GetErrorNotificationMessage(Admin_Resources.ErrorAssociatePriceListToAccount));
            return Json(JsonRequestBehavior.AllowGet);
        }

        //UnAssociate Price List from Account.
        public virtual JsonResult UnAssociatePriceListToAccount(string priceListId, int accountId)
        {
            if (!string.IsNullOrEmpty(priceListId))
            {
                bool status = _accountAgent.UnAssociatedPriceList(priceListId, accountId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }


        //Update associated price list precedence value for Account.
        public virtual JsonResult EditAssociatedPriceListPrecedence(string data)
        {
            PriceAccountViewModel model = JsonConvert.DeserializeObject<PriceAccountViewModel[]>(data)[0];
            bool status = false;
            string message = string.Empty;

            if (ModelState.IsValid)
                status = _accountAgent.UpdateAssociatedPriceListPrecedence(model);
            message = status ? PIM_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage;

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Account Quote
        //Get account note list.
        public virtual ActionResult AccountQuoteList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId = 0, int accountId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeOmsQuote.ToString(), model);
            AccountQuoteListViewModel quoteList = _accountQuoteAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, accountId, userId);
            quoteList.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteList?.AccountQuotes, GridListType.ZnodeOmsQuote.ToString(), string.Empty, null, true, true, quoteList?.GridModel?.FilterColumn?.ToolMenuList);
            quoteList.GridModel.TotalRecordCount = quoteList.TotalResults;
            return ActionView(quoteList);
        }

        //Get account quote on the basis of oms quote id.
        [HttpGet]
        public virtual ActionResult UpdateAccountQuote(int omsQuoteId, string orderStatus = null, string updatePageType = null)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

           AccountQuoteViewModel accountViewModel = _accountQuoteAgent.GetAccountQuote(omsQuoteId, updatePageType);
           if(accountViewModel.ShoppingCart.ShoppingCartItems.Count < 1)
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorMessageForDisableProduct));
                return RedirectToAction<QuoteController>(x => x.AccountQuoteList(null));
            }
           else
            {
                return ActionView("_UpdateAccountQuote", accountViewModel);
           }
        }

        //Update account quote.
        [HttpPost]
        public virtual ActionResult UpdateAccountQuote(AccountQuoteViewModel accountQuoteViewModel)
        {
            SetNotificationMessage(_accountQuoteAgent.UpdateQuoteStatus(accountQuoteViewModel) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));

            return RedirectToAction<AccountController>(x => x.UpdateAccountQuote(accountQuoteViewModel.OmsQuoteId, accountQuoteViewModel.OrderStatus, accountQuoteViewModel.UpdatePageType));
        }

        //Update multiple quote status.
        public virtual JsonResult UpdateQuoteStatus(string quoteId, int status, bool isPendingPaymentStatus = false, string orderStatus = null)
        {
            string message = Admin_Resources.ErrorDraftOrderedStatus;
            //Check for null or empty.
            if (!string.IsNullOrEmpty(quoteId) && (status > 0 || isPendingPaymentStatus))
            {
                bool isUpdated = _accountQuoteAgent.UpdateQuoteStatus(quoteId, status, isPendingPaymentStatus, orderStatus, ref message);
                return Json(new { status = isUpdated, message = isUpdated ? Admin_Resources.UpdateMessage : message, paymentStatus = isUpdated ? AdminConstants.Rejected : AdminConstants.PendingApproval }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message, paymentStatus = AdminConstants.PendingApproval }, JsonRequestBehavior.AllowGet);
        }

        // The index page for create quote.
        [HttpGet]
        public virtual ActionResult CreateAccountQuote(int accountId)
        {
            AccountDataViewModel model = _accountAgent.GetAccountById(accountId);

            CreateOrderViewModel createQuote = _orderAgent.GetCreateOrderDetails(model?.CompanyAccount?.PortalId ?? 0);
            createQuote.IsQuote = true;
            createQuote.StoreName = createQuote.PortalList.Find(x => x.Value == model.CompanyAccount.PortalId.ToString())?.Text;
            createQuote.PortalId = model.CompanyAccount?.PortalId ?? 0;
            createQuote.AccountId = accountId;
            return ActionView("~/Views/Quote/CreateQuote.cshtml", createQuote);
        }

        //Get Pending Payment Quotes
        [HttpGet]
        public virtual ActionResult UpdatePendingPaymentQuote(int omsQuoteId, string orderStatus = null, string updatePageType = null)
          => ActionView("_UpdatePendingPayment", _accountQuoteAgent.GetAccountQuote(omsQuoteId, updatePageType));

        //Update Pending Payment Quote
        [HttpPost]
        public virtual ActionResult UpdatePendingPaymentQuote(AccountQuoteViewModel accountQuoteViewModel)
        {
            SetNotificationMessage(_accountQuoteAgent.UpdateQuoteStatus(accountQuoteViewModel) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));

            return (accountQuoteViewModel.UpdatePageType.Equals(AdminConstants.Quotes)) ? RedirectToAction<QuoteController>(x => x.PendingPaymentList(null))
                : RedirectToAction<AccountController>(x => x.UpdatePendingPaymentQuote(accountQuoteViewModel.OmsQuoteId, accountQuoteViewModel.OrderStatus, accountQuoteViewModel.UpdatePageType));
        }
        #endregion

        #region Account Order
        //Get user order list of account.
        public virtual ActionResult AccountUserOrderList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId = 0, int accountId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AccountUserOrderList.ToString(), model);
            //Get the list of order of account     
            OrdersListViewModel orders = _accountAgent.GetAccountUserOrderList(accountId, userId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            orders.GridModel = FilterHelpers.GetDynamicGridModel(model, orders.List, GridListType.AccountUserOrderList.ToString(), string.Empty, null, true, true, orders?.GridModel?.FilterColumn?.ToolMenuList);
            orders.AccountId = accountId;
            orders.UserId = userId;
            //Set the total record count
            orders.GridModel.TotalRecordCount = orders.TotalResults;

            return ActionView(orders);
        }
        #endregion

        #region Account Profile
        // Get Associated profile for Account.
        public virtual ActionResult GetAssociatedProfileForAccount([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int accountId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_AccountProfileList.ToString(), model);
            //Get profile list.
            ProfileListViewModel profileList = _accountAgent.GetAssociatedProfile(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, accountId);

            //Get the grid model.
            profileList.GridModel = FilterHelpers.GetDynamicGridModel(model, profileList?.List, GridListType.View_AccountProfileList.ToString(), string.Empty, null, true, true, profileList?.GridModel?.FilterColumn?.ToolMenuList);
            profileList.GridModel.TotalRecordCount = profileList.TotalResults;

            return ActionView("_AssociatedProfileListForAccount", profileList);
        }

        // Get UnAssociated profile for Account.
        public virtual ActionResult GetUnAssociatedProfileForAccount([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int accountId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedProfilesForAccount.ToString(), model);
            //Get unassociated profile.
            ProfileListViewModel profileList = _accountAgent.GetUnAssociatedProfile(accountId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            profileList.AccountId = accountId;
            profileList.GridModel = FilterHelpers.GetDynamicGridModel(model, profileList?.List, GridListType.UnAssociatedProfilesForAccount.ToString(), string.Empty, null, true);
            profileList.GridModel.TotalRecordCount = profileList.TotalResults;

            return ActionView("_UnAssociatedProfileForAccount", profileList);
        }

        //Associate UnAssociated Profile to Account.
        public virtual JsonResult AssociateProfileToAccount(int accountId, string profileIds)
        {
            SetNotificationMessage(_accountAgent.AssociateProfile(accountId, profileIds)
                ? GetSuccessNotificationMessage(Admin_Resources.AccountProfileSuccessMessage) : GetErrorNotificationMessage(Admin_Resources.ErrorAssociateProfileToAccount));
            return Json(JsonRequestBehavior.AllowGet);
        }

        //UnAssociate Profiles from Account.
        public virtual JsonResult UnAssociateProfileToAccount(string accountProfileId, int accountId)
        {
            if (!string.IsNullOrEmpty(accountProfileId))
            {
                string message = Admin_Resources.UnassignError;
                bool status = _accountAgent.UnAssociateProfile(accountProfileId, accountId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
        }

        //Set default profile for account.
        public virtual ActionResult SetDefaultProfile(int accountId, int accountProfileId, int profileId)
        {
            string errorMessage = string.Empty;

            SetNotificationMessage(_accountAgent.SetDefaultProfile(accountId, accountProfileId, profileId, out errorMessage) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
               : GetErrorNotificationMessage(errorMessage));
            return RedirectToAction<CustomerController>(x => x.GetAssociatedProfileList(null, accountId));
        }
        #endregion       

        // Get approver level list.
        public virtual ActionResult GetApproverLevelList(int userId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            UserApproverListViewModel _userApproverList = _accountAgent.GetApproverLevelList(userId);
            return (Request.IsAjaxRequest()) ? PartialView(createEditView, _userApproverList) :
               ActionView(_manageApproverView, _userApproverList);
        }

        // Get level list
        [HttpGet]
        public virtual ActionResult GetLevelList()
        {
            //Get List of Available Template Area. 
            UserApproverViewModel approverLevelViewModel = _accountAgent.GetLevelList();

            //Convert View Result into the String.
            string partialView = RenderRazorViewToString(PartialApprovalLevel, approverLevelViewModel);
            bool isAreaAvailable = HelperUtility.IsNotNull(approverLevelViewModel?.Levels) && approverLevelViewModel?.Levels.Count > 0;
            return Json(new { html = partialView, status = isAreaAvailable, message = Admin_Resources.AreaNotFound }, JsonRequestBehavior.AllowGet);
        }

        // This method will add permission code to that user.
        [HttpPost]
        public virtual ActionResult SavePermissionSetting(PermissionCodeViewModel permissionViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!_accountAgent.SavePermissionSetting(permissionViewModel).HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.ApprovalTypeRecordUpdated));
                    return RedirectToAction<AccountController>(x => x.GetApproverLevelList(permissionViewModel.UserId));
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(permissionViewModel.ErrorMessage));
                    var backURLcookies = Request.Cookies["_backURL"];
                    backURLcookies.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(backURLcookies);
            }
            return RedirectToAction<AccountController>(x => x.GetApproverLevelList(permissionViewModel.UserId));
        }

        //This method will create and update the approver level for the user.
        [HttpPost]
        public virtual JsonResult CreateApproverLevel(UserApproverViewModel userApproverViewModel)
        {
            string errorMessage;
            string message = string.Empty;
            int userId = Convert.ToInt32(HttpContext.Request.UrlReferrer.Query.Split('=')[1]);

            userApproverViewModel.UserId = userId;
            bool status = _accountAgent.CreateApproverLevel(userApproverViewModel, out errorMessage);

            return Json(new
            {
                status = status,
                message = status ? (userApproverViewModel.UserApproverId.Equals(0) ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage) : (message = errorMessage),
                userApprovalId = userApproverViewModel.UserApproverId
            }, JsonRequestBehavior.AllowGet);

        }


        // Delete approval level by id
        public virtual JsonResult DeleteApproverLevel(string userApproverId)
        {
            if (!string.IsNullOrEmpty(userApproverId))
            {
                string message = string.Empty;
                bool status = _accountAgent.DeleteApproverLevelById(userApproverId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : string.IsNullOrEmpty(message) ? Admin_Resources.ErrorFailToDeleteParentAccount : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get User Approver list based on search term.
        /// </summary>
        /// <param name="searchTerm">searchTerm</param>
        /// <param name="portalId">portalId</param>
        /// <param name="accountId">accountId</param>
        /// <returns>returns approver users name</returns>
        public virtual JsonResult GetApproverUsersByName(string searchTerm, int portalId, int? accountId, string approvalUserIds = null)
        {
            int? userId = Convert.ToInt32(HttpContext.Request.UrlReferrer.Query.Split('=')[1]);
            return Json(_accountAgent.GetApproverUsersByName(searchTerm, portalId, accountId, userId, approvalUserIds), JsonRequestBehavior.AllowGet);
        }

        //Check Account Code already present in DB.
        [HttpGet]
        public virtual JsonResult IsAccountCodeExists(string codeField)
        {
            bool isExist = true;
            if (!string.IsNullOrEmpty(codeField))
            {
                isExist =_helperAgent.IsCodeExists(codeField, CodeFieldService.AccountService.ToString(), CodeFieldService.IsCodeExists.ToString());
            }
            return Json(new { isExist = !isExist, message = Admin_Resources.ErrorAccountCodeExist }, JsonRequestBehavior.AllowGet);
        }

        //Get parent account list
        public virtual ActionResult GetParentAccountsList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserAccountList.ToString(), model);

            //Add portalId filter.
            _accountAgent.SetPortalIdFilter(model.Filters, portalId);

            //Get account list.            
            ParentAccountListViewModel parentAccountViewListModel = _accountAgent.GetParentAccountList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            parentAccountViewListModel.GridModel = FilterHelpers.GetDynamicGridModel(model, parentAccountViewListModel.ParentAccountList, GridListType.ZnodeParentAccountList.ToString(), string.Empty, null, true, true, parentAccountViewListModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            parentAccountViewListModel.GridModel.TotalRecordCount = parentAccountViewListModel.TotalResults;

            //Returns the account list view.
            return ActionView(parentAccountListAsidePanel, parentAccountViewListModel);
        }
        #endregion

        #region Private Methods       

        //Sets the Address view model to its default value.
        private void SetAddressViewModel(AddressViewModel address)
        {
            if (IsNull(address))
                address = new AddressViewModel();
            address.Countries = HelperMethods.GetCountries();
            address.IsDefaultBilling = true;
            address.IsDefaultShipping = true;
        }
        #endregion
    }
}