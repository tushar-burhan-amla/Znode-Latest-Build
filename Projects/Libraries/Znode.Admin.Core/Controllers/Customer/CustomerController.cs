using MvcSiteMapProvider;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
namespace Znode.Engine.Admin.Controllers
{
    public class CustomerController : BaseController
    {
        #region Private Readonly members
        private readonly ICustomerAgent _customerAgent;
        private readonly IGiftCardAgent _giftCardAgent;
        private readonly IUserAgent _userAgent;
        private readonly ICartAgent _cartAgent;
        private const string CustomerOrderHistoryView = "_OrderList";
        private const string CreateEditNoteView = "_CreateEditCustomerNote";
        private const string CustomerNoteListView = "_CustomerNoteList";
        private const string CustomerAddressListView = "_CustomerAddressList";
        private const string CreateEditCustomerAddressView = "_CreateEditCustomerAddress";
        private const string GuestUserListView = "CustomersList";
        private const string AddCustomerAddressPanel = "_AddCustomerAddressPanel";
        private const string AssociatedPriceList = "_AssociatedPriceListForCustomer";
        private const string UnAssociatedPriceList = "_UnAssociatedPriceListForCustomer";
        private const string CreatePaymentView = "_CreatePayment";
        private const string AccountListView = "_AccountListAsidePanel";
        private readonly string customerCreateEditView = "CustomerCreateEdit";
        private readonly string UserCartDetailsView = "UserCartDetails";
        private readonly string UserImpersonationView = "UserImpersonation";
        private readonly string SalesRepAssociated = "AssociatedSalesRep";
        private readonly string AssociatedSalesRepForAccount = "AssociatedSalesRepForAccount";
        #endregion

        #region Public Constructor
        public CustomerController(ICustomerAgent customerAgent, IGiftCardAgent giftCardAgent, IOrderAgent orderAgent, IUserAgent userAgent, ICartAgent cartAgent)
        {
            _customerAgent = customerAgent;
            _giftCardAgent = giftCardAgent;
            _userAgent = userAgent;
            _cartAgent = cartAgent;
        }
        #endregion

        #region Public Methods
        #region Profile association
        //Get associated profiles based on customers.
        public virtual ActionResult GetAssociatedProfileList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
                => ActionView("_AssociatedProfileList", _customerAgent.GetAssociatedProfileList(model, userId));

        //Get unassociated profiles.
        public virtual ActionResult GetUnAssociatedProfileList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserUnAssociatedProfiles.ToString(), model);
            ProfileListViewModel list = _customerAgent.GetUnAssociatedProfileList(model, userId);

            //Get the grid model.
            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.List, GridListType.ZnodeUserUnAssociatedProfiles.ToString(), string.Empty, null, true);

            //Set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("_UnAssociatedProfileList", list);
        }

        //Unassociate profiles from associated profiles list.
        public virtual JsonResult UnAssociateProfiles(string profileId, int userId)
        {
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(profileId))
            {
                bool status = _customerAgent.UnAssociateProfiles(profileId, userId, out errorMessage);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UnassignError }, JsonRequestBehavior.AllowGet);
        }

        //Associate profiles.
        public virtual JsonResult AssociateProfiles(string profileIds, int userId)
        {
            SetNotificationMessage(_customerAgent.AssociateProfiles(profileIds, userId)
               ? GetSuccessNotificationMessage(Admin_Resources.AssociatedSuccessMessage)
               : GetErrorNotificationMessage(Admin_Resources.AssociatedErrorMessage));
            return Json(JsonRequestBehavior.AllowGet);
        }

        //Set default profile for customer.
        public virtual ActionResult SetDefaultProfile(int userId, string profileId)
        {
            string errorMessage = string.Empty;
            bool isUpdated = _customerAgent.SetDefaultProfile(userId, profileId, out errorMessage);

            SetNotificationMessage(isUpdated ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
               : GetErrorNotificationMessage(errorMessage));
            return RedirectToAction<CustomerController>(x => x.GetAssociatedProfileList(null, userId));
        }
        #endregion

        #region Customer Notes
        //Get view to create new customer notes.
        [HttpGet]
        public virtual ActionResult CreateCustomerNote(int userId)
           => ActionView(CreateEditNoteView, new NoteViewModel { UserId = userId });

        //Create new customer notes.
        [HttpPost]
        public virtual ActionResult CreateCustomerNote(NoteViewModel noteViewModel)
        {
            if (ModelState.IsValid)
            {
                noteViewModel = _customerAgent.CreateCustomerNote(noteViewModel);
                if (!noteViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<CustomerController>(x => x.EditCustomerNote(noteViewModel.NoteId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(noteViewModel.ErrorMessage));
            return RedirectToAction<CustomerController>(x => x.CreateCustomerNote(noteViewModel.UserId));
        }

        //Get customer note list.
        public virtual ActionResult CustomerNoteList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.CustomerNotes.ToString(), model);
            NoteListViewModel noteList = _customerAgent.GetCustomerNotes(userId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            noteList.UserId = userId;
            noteList.GridModel = FilterHelpers.GetDynamicGridModel(model, noteList?.Notes, GridListType.CustomerNotes.ToString(), string.Empty, null, true, true, noteList?.GridModel?.FilterColumn?.ToolMenuList);
            noteList.GridModel.TotalRecordCount = noteList.TotalResults;
            return ActionView(CustomerNoteListView, noteList);
        }

        //Get view to update existing customer note.
        [HttpGet]
        public virtual ActionResult EditCustomerNote(int noteId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            return ActionView(CreateEditNoteView, _customerAgent.GetCustomerNote(noteId));
        }

        //Update existing customer note.
        [HttpPost]
        public virtual ActionResult EditCustomerNote(NoteViewModel noteViewModel)
        {
            noteViewModel = _customerAgent.UpdateCustomerNote(noteViewModel);
            if (!noteViewModel.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<CustomerController>(x => x.EditCustomerNote(noteViewModel.NoteId));
            }
            SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<CustomerController>(x => x.EditCustomerNote(noteViewModel.NoteId));
        }

        //Delete customer notes.
        public virtual JsonResult DeleteCustomerNote(string noteId)
        {
            if (!string.IsNullOrEmpty(noteId))
            {
                bool status = _customerAgent.DeleteCustomerNote(noteId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Address
        //Get Address list of customer.
        public virtual ActionResult GetAddressList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
            => ActionView(CustomerAddressListView, _customerAgent.GetAddressList(model, userId));

        //Get view to create new customer address.
        [HttpGet]
        public virtual ActionResult CreateAddress(int userId, int listCount = 0, int accountId = 0)
        {
            CustomerViewModel customerAccountDetails = _userAgent.GetCustomerAccountById(userId);
            return ActionView(AddCustomerAddressPanel, new AddressViewModel
            {
                UserId = userId,
                Countries = HelperMethods.GetPortalAssociatedCountries(customerAccountDetails.PortalId.GetValueOrDefault()),
                IsGuest = string.IsNullOrEmpty(customerAccountDetails.AspNetUserId) ? true : false,
                ListCount = listCount,
                AccountId = accountId
            });
        }

        //Create new customer address.
        [HttpPost]
        public virtual ActionResult CreateAddress(AddressViewModel addressViewModel)
        {
            if (ModelState.IsValid)
            {
                addressViewModel = _customerAgent.CreateCustomerAddress(addressViewModel);
                if (!addressViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<CustomerController>(x => x.GetAddressList(null, addressViewModel.UserId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
            addressViewModel.Countries = HelperMethods.GetCountries();
            return RedirectToAction<CustomerController>(x => x.GetAddressList(null, addressViewModel.UserId));
        }

        //Get view to update existing customer address.
        [HttpGet]
        public virtual ActionResult EditAddress(int userAddressId, int accountAddressId = 0, int userId = 0)
        {
            AddressViewModel address = _customerAgent.GetCustomerAddress(userAddressId, accountAddressId, userId);

            //Get customer details by user id.
            CustomerViewModel customerAccountDetails = _userAgent.GetCustomerAccountById(userId);
            //Get country list to bind in dropdown.
            address.Countries = HelperMethods.GetPortalAssociatedCountries(customerAccountDetails.PortalId.GetValueOrDefault());
            return ActionView(CreateEditCustomerAddressView, address);
        }

        //Update existing customer address.
        [HttpPost]
        public virtual ActionResult EditAddress(AddressViewModel addressViewModel)
        {
            addressViewModel = _customerAgent.UpdateCustomerAddress(addressViewModel);
            if (!addressViewModel.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<CustomerController>(x => x.GetAddressList(null, addressViewModel.UserId));
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<CustomerController>(x => x.EditAddress(addressViewModel.UserAddressId, addressViewModel.AccountAddressId, addressViewModel.UserId));
        }

        //Delete customer address.
        public virtual JsonResult DeleteAddress(string userAddressId, string accountAddressId)
        {
            if (!string.IsNullOrEmpty(userAddressId) | !string.IsNullOrEmpty(accountAddressId))
            {
                string errorMessage = string.Empty;
                bool status = _customerAgent.DeleteCustomerAddress(userAddressId, accountAddressId, out errorMessage);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Affiliate
        //Gets the Customer affiliate view.
        public virtual ActionResult GetAffiliate(int userId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            CustomerAffiliateViewModel model = _customerAgent.GetCustomerAffiliateData(userId);
            model.Domains = _customerAgent.GetDomains(model.PortalId, userId);

            return ActionView("_CustomerAffiliate", model);
        }

        //Save the affiliate data.
        [HttpPost]
        public virtual ActionResult SaveAffiliateData(CustomerAffiliateViewModel model)
        {
            if (ModelState.IsValid)
                SetNotificationMessage(_customerAgent.SaveAffiliateData(model).HasError ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage) : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            return RedirectToAction<CustomerController>(x => x.GetAffiliate(model.UserId));
        }

        //Get the domain list associated with customer.
        public virtual ActionResult GetDomains(int portalId, string domainIds, int userId)
            => ActionView("_CustomerDomains", new CustomerAffiliateViewModel() { DomainIds = domainIds.Split(','), Domains = _customerAgent.GetDomains(portalId, userId) });

        //Get tab structure.
        public virtual ActionResult GetTabStructure(int userId)
           => PartialView(AdminConstants.TabStructurePath, _customerAgent.CreateTabStructure(userId));

        //Get Referral Commission List.
        public virtual ActionResult GetReferralCommissionList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_CustomerReferralCommissionDetail.ToString(), model);
            ReferralCommissionListViewModel _referralCommissionList = _customerAgent.GetReferralCommissionList(model, userId);

            //Get the grid model.
            _referralCommissionList.GridModel = FilterHelpers.GetDynamicGridModel(model, _referralCommissionList?.List, GridListType.View_CustomerReferralCommissionDetail.ToString(), string.Empty, null, true, true, null);

            //Set the total record count
            _referralCommissionList.GridModel.TotalRecordCount = _referralCommissionList.TotalResults;

            return ActionView("_GetReferralCommissionList", _referralCommissionList);
        }

        #region Payment List
        //Get Referral Commission Payment List.
        public virtual ActionResult GetPaymentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCustomerPaymentList.ToString(), model);
            GiftCardListViewModel giftCardList = _customerAgent.GetPaymentList(model, userId);

            //Get the grid model.
            giftCardList.GridModel = FilterHelpers.GetDynamicGridModel(model, giftCardList?.GiftCardList, GridListType.ZnodeCustomerPaymentList.ToString(), string.Empty, null, true, true, null);
            giftCardList.GridModel.TotalRecordCount = giftCardList.TotalResults;

            //Returns the GiftCard list.
            return ActionView(giftCardList);
        }

        //Get: CreateReferralCommissionPayment.
        public virtual ActionResult CreateReferralCommissionPayment(int userId)
        {
            GiftCardViewModel giftCardViewModel = _customerAgent.GetDataToCreatePayment(userId);
            if (giftCardViewModel.HasError)
            {
                SetNotificationMessage(GetErrorNotificationMessage(giftCardViewModel.ErrorMessage));
                return RedirectToAction<CustomerController>(x => x.GetAffiliate(giftCardViewModel.UserId.GetValueOrDefault()));
            }
            else
                return View(CreatePaymentView, giftCardViewModel);
        }

        //Post: CreateReferralCommissionPayment.
        [HttpPost]
        public virtual ActionResult CreateReferralCommissionPayment(GiftCardViewModel giftCardViewModel)
        {
            if (ModelState.IsValid)
            {
                giftCardViewModel.IsReferralCommission = true;
                giftCardViewModel = _giftCardAgent.Create(giftCardViewModel);

                if (giftCardViewModel.HasError)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(giftCardViewModel.ErrorMessage));
                    return View(CreatePaymentView, giftCardViewModel);
                }
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
            return RedirectToAction<CustomerController>(x => x.GetAffiliate(giftCardViewModel.UserId.GetValueOrDefault()));
        }

        // Get User voucher list.
        public virtual ActionResult GetUserVoucherList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId, bool isExcludeExpired = true)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeUserVoucherList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserVoucherList.ToString(), model);
            //Get Voucher list.
            GiftCardListViewModel giftCardList = _giftCardAgent.GetGiftCardList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, isExcludeExpired, userId);

            //Get the grid model.
            giftCardList.GridModel = FilterHelpers.GetDynamicGridModel(model, giftCardList.GiftCardList, GridListType.ZnodeUserVoucherList.ToString(), string.Empty, null, true, true, giftCardList?.GridModel?.FilterColumn?.ToolMenuList);
            giftCardList.GridModel.TotalRecordCount = giftCardList.TotalResults;
            giftCardList.UserId = userId;
            //Returns the Voucher list.
            return ActionView("_UserVoucherList", giftCardList);
        }
        #endregion
        #endregion

        #region Associate Price
        // Get Associated Price list for Customer.
        public virtual ActionResult GetAssociatedPriceListForCustomer([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedPriceListToCustomer.ToString(), model);
            //Get Price list.
            PriceListViewModel priceList = _customerAgent.GetPriceList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, userId);

            //Get the grid model.
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.AssociatedPriceListToCustomer.ToString(), string.Empty, null, true, true, priceList?.GridModel?.FilterColumn?.ToolMenuList);

            priceList.GridModel.TotalRecordCount = priceList.TotalResults;

            return ActionView(AssociatedPriceList, priceList);
        }

        // Get UnAssociated Price list for Customer.
        public virtual ActionResult GetUnAssociatedPriceListForCustomer([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedPriceList.ToString(), model);
            //Get Price list.
            PriceListViewModel priceList = _customerAgent.GetUnAssociatedPriceList(userId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            priceList.UserId = userId;
            priceList.GridModel = FilterHelpers.GetDynamicGridModel(model, priceList?.PriceList, GridListType.UnAssociatedPriceList.ToString(), string.Empty, null, true);
            priceList.GridModel.TotalRecordCount = priceList.TotalResults;

            return ActionView(UnAssociatedPriceList, priceList);
        }

        //Associate UnAssociated Price List to Customer.
        public virtual JsonResult AssociatePriceListToCustomer(int userId, string priceListId)
        {
            SetNotificationMessage(_customerAgent.AssociatePriceList(userId, priceListId)
                ? GetSuccessNotificationMessage(Admin_Resources.CustomerPriceSuccessMessage) : GetErrorNotificationMessage(Admin_Resources.ErrorAssociatePriceListToCustomer));
            return Json(JsonRequestBehavior.AllowGet);
        }

        //UnAssociate Price List from Customer.
        public virtual JsonResult UnAssociatePriceListToCustomer(string priceListId, int userId)
        {
            string message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(priceListId))
            {
                bool status = _customerAgent.UnAssociatePriceList(priceListId, userId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }


        //Update associated price list precedence value for Customer.
        public virtual JsonResult EditAssociatedPriceListPrecedence(string data)
        {
            PriceUserViewModel model = JsonConvert.DeserializeObject<PriceUserViewModel[]>(data)[0];

            bool status = _customerAgent.UpdateAssociatedPriceListPrecedence(model);

            string message = status ? PIM_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage;

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Order
        //Get Orders list for customer.
        public virtual ActionResult GetOrderList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.CustomerOrderHistory.ToString(), model);
            //Get the list of Orders     
            OrdersListViewModel orders = _customerAgent.GetCustomerOrderList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, userId);

            //Get the grid model
            orders.GridModel = FilterHelpers.GetDynamicGridModel(model, orders.List, GridListType.CustomerOrderHistory.ToString(), string.Empty, null, true, true, orders?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            orders.GridModel.TotalRecordCount = orders.TotalResults;

            //Returns the attribute list view
            return ActionView(CustomerOrderHistoryView, orders);
        }
        #endregion

        #region Customer

        // This method will fetch the list of all the customer account details.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "Customer", Area = "", ParentKey = "Admin")]
        public virtual ActionResult CustomersList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0, string portalName = null)
        {
            string currentUserName = HttpContext.User.Identity.Name;
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCustomerAccount.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCustomerAccount.ToString(), model);
            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            //Get the list of attributes            
            CustomerListViewModel customerViewModel = _userAgent.GetCustomerAccountList(currentUserName, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId, portalName);

            //Get the grid model
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, customerViewModel.List, GridListType.ZnodeCustomerAccount.ToString(), string.Empty, null, true, true, customerViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            gridModel.TotalRecordCount = customerViewModel.TotalResults;

            //Returns the attribute list view
            return ActionView(new CustomerListViewModel() { List = customerViewModel.List, GridModel = gridModel, PortalId = customerViewModel.PortalId, PortalName = customerViewModel?.PortalName });
        }

        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "Customer", Area = "", ParentKey = "Admin")]
        public virtual ActionResult GuestUserList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            bool guestUser = true;
            string currentUserName = HttpContext.User.Identity.Name;
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeGuestAccount.ToString(), model);
            model.Filters.Add(new FilterTuple(FilterKeys.IsGuestUser, FilterOperators.Is, Convert.ToString(guestUser)));

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeGuestAccount.ToString(), model);
            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            //Get the list of attributes            
            CustomerListViewModel customerViewModel = _userAgent.GetCustomerAccountList(currentUserName, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, customerViewModel.List, GridListType.ZnodeGuestAccount.ToString(), string.Empty, null, true, true, customerViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            gridModel.TotalRecordCount = customerViewModel.TotalResults;

            //Returns the attribute list view
            return ActionView(GuestUserListView, new CustomerListViewModel() { List = customerViewModel.List, GridModel = gridModel });
        }

        // Create Customer User.
        public virtual ActionResult CustomerCreate()
        {
            MenuListViewModel menuList = null;
            menuList = SessionHelper.GetDataFromSession<MenuListViewModel>(AdminConstants.UserMenuListSessionKey);
            ViewBag.isAccountAccess = menuList?.Menus.Any(x => x.MenuName == AdminConstants.Accounts);
            return ActionView(customerCreateEditView, new CustomerViewModel { Portals = _userAgent.GetPortals(), Accounts = _userAgent.GetAccounts() });
        }

        //Get the List of Sales Rep
        public virtual ActionResult GetSalesRepList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAssociatedSalesRep.ToString(), model);

            model.Filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Is, Convert.ToString(userId)));

            //Get the list of Customers.
            UsersListViewModel salesRepList = _userAgent.GetSalesRepListForAssociation(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            salesRepList.GridModel = FilterHelpers.GetDynamicGridModel(model, salesRepList.List, GridListType.ZnodeAssociatedSalesRep.ToString(), string.Empty, null, true, true, salesRepList?.GridModel?.FilterColumn?.ToolMenuList);

            salesRepList.UserId = userId;

            //Set the total record count.
            salesRepList.GridModel.TotalRecordCount = salesRepList.TotalResults;

            return ActionView(SalesRepAssociated, salesRepList);
        }

        // This method will add Customer User.
        [HttpPost]
        public virtual ActionResult CustomerCreate(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                CustomerViewModel model = _userAgent.CreateCustomerAccount(customerViewModel);

                if (!model?.HasError ?? false)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<CustomerController>(x => x.CustomerEdit(model.UserId));
                }
                SetNotificationMessage(GetErrorNotificationMessage(string.IsNullOrEmpty(model.ErrorMessage) ? Admin_Resources.ErrorFailedToCreate : model?.ErrorMessage));
            }
            _userAgent.SetCustomerViewModel(customerViewModel);
            return ActionView(customerCreateEditView, customerViewModel);
        }

        /// Edit Customer User.
        [HttpGet]
        public virtual ActionResult CustomerEdit(int userId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            if (userId > 0)
            {
                CustomerViewModel customerAccountDetails = _userAgent.GetCustomerAccountDetails(userId);
                customerAccountDetails.IsGuestUser = string.IsNullOrEmpty(customerAccountDetails.AspNetUserId);
                TempData["IsGuestUser" + userId] = customerAccountDetails.IsGuestUser;
                TempData["CustomerDisplayName" + userId] = string.IsNullOrEmpty(customerAccountDetails.FullName?.Trim()) ? customerAccountDetails.Email : customerAccountDetails.FullName;

                if (customerAccountDetails.UserId > 0)
                    _userAgent.SetCustomerViewModel(customerAccountDetails);
                return ActionView(customerCreateEditView, customerAccountDetails);
            }
            return RedirectToAction<CustomerController>(x => x.CustomersList(null, 0, null));
        }

        // This method update the users account details.
        [HttpPost]
        public virtual ActionResult CustomerEdit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                string errorMessage = string.Empty;

                if (_userAgent.UpdateCustomerAccount(model, out errorMessage))
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UserUpdateMessage));
                    return RedirectToAction<CustomerController>(x => x.CustomerEdit(model.UserId));
                }
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(errorMessage));
                    _userAgent.SetCustomerViewModel(model);
                    return ActionView(customerCreateEditView, model);
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<CustomerController>(x => x.CustomerEdit(model.UserId));
        }

    public virtual ActionResult GetAccountList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0, string accountCode = null)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserAccountList.ToString(), model);
            //Add portalId filter.
            HelperMethods.SetPortalIdFilters(model.Filters, portalId);

            //Get account list.            
            AccountListViewModel accountViewListModel = _userAgent.GetAccountList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, accountCode);

            //Get the grid model
            accountViewListModel.GridModel = FilterHelpers.GetDynamicGridModel(model, accountViewListModel.AccountList, GridListType.ZnodeUserAccountList.ToString(), string.Empty, null, true, true, accountViewListModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            accountViewListModel.GridModel.TotalRecordCount = accountViewListModel.TotalResults;

            //Returns the account list view.
            return ActionView(AccountListView, accountViewListModel);
        }

        // Get states by country code.
        public virtual ActionResult GetStates(string countryCode) => Json(new
        {
            states = _userAgent.GetStates(countryCode)
        }, JsonRequestBehavior.AllowGet);


        // Get User Cart details by user ID.
        public virtual ActionResult GetUserCartByUserId(int userId)
        {
            if (userId > 0)
                return ActionView(UserCartDetailsView, _userAgent.GetUserCartByUserId(userId));

            return RedirectToAction<CustomerController>(x => x.CustomersList(null, 0, null));
        }

        //Method to remove all cart items from user cart
        public virtual ActionResult RemoveAllCartItem()
        {
            CartViewModel cartViewModel = _cartAgent.RemoveAllCart();
            CreateOrderViewModel createOrderViewModel= new CreateOrderViewModel
            {
                CartViewModel = cartViewModel,
                UserId = cartViewModel.UserId,
                CustomerName = cartViewModel.CustomerName
            };
            return ActionView(UserCartDetailsView, createOrderViewModel);
        }

        //Get the List of Sales Rep
        public virtual ActionResult GetSalesRepListForAccount([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAssociatedSalesRep.ToString(), model);

            HelperMethods.SetPortalIdFilters(model.Filters, portalId);

            //Get the list of Customers.
            SalesRepUsersListViewModel salesRepList = _userAgent.GetSalesRepListForAccount(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            salesRepList.GridModel = FilterHelpers.GetDynamicGridModel(model, salesRepList.List, GridListType.ZnodeAssociatedSalesRep.ToString(), string.Empty, null, true, true, salesRepList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count.
            salesRepList.GridModel.TotalRecordCount = salesRepList.TotalResults;

            return ActionView(AssociatedSalesRepForAccount, salesRepList);
        }

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
        #endregion

        #region Impersonation

        // Get User Impersonation by user ID.
        public virtual ActionResult GetImpersonationByUserId(int userId)
        {
            if (userId > 0)
            {
                ImpersonationViewModel model = _customerAgent.GetImpersonationByUserId(userId);
                if (HelperUtility.IsNotNull(model))
                {
                    model.IsLock = _userAgent.GetCustomerAccountById(userId).IsLock;
                    if (model.IsLock)
                    {
                        SetNotificationMessage(GetInfoNotificationMessage(Admin_Resources.Impersonation_UserLockMessage));
                    }
                }
                return ActionView(UserImpersonationView, model);
            }
            SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.Impersonation_Invalid));
            return RedirectToAction<CustomerController>(x => x.CustomersList(null, 0, null));
        }


        //Get User Impersonation URL.
        public virtual JsonResult GetImpersonationUrl(int portalId, int userId)
        {
            if (userId > 0 && portalId > 0)
            {
                var launchUrl = _userAgent.GetImpersonationUrl(userId, portalId);
                return Json(new { status = true, url = launchUrl }, JsonRequestBehavior.AllowGet);
            }
            SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.Impersonation_Invalid));
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        // This method is implemented to allow the admin users to edit the username of any user (customer/admin) account
        [HttpPut]
        public virtual JsonResult UpdateUsernameForRegisteredUser(UserDetailsViewModel userDetailsViewModel)
        {
            if (ModelState.IsValid)
            {
                bool status = _userAgent.UpdateUsernameForRegisteredUser(userDetailsViewModel);
                return Json(new { success = status }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, statusMessage = Admin_Resources.DataNotFound }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
    }
}