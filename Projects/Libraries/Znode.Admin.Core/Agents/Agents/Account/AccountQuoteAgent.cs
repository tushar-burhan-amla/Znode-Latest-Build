using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Admin.Core;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Admin.Agents
{
    public class AccountQuoteAgent : BaseAgent, IAccountQuoteAgent
    {
        #region Private Variables
        private readonly IAccountQuoteClient _accountQuoteClient;
        private readonly IOrderStateClient _orderStateClient;
        #endregion

        #region Constructor
        public AccountQuoteAgent(IAccountQuoteClient accountQuoteClient, IOrderStateClient orderStateClient)
        {
            _accountQuoteClient = GetClient<IAccountQuoteClient>(accountQuoteClient);
            _orderStateClient = GetClient<IOrderStateClient>(orderStateClient);
        }
        #endregion

        #region Public Methods
        // Get Account Quote List.
        public virtual AccountQuoteListViewModel GetAccountQuoteList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int accountId = 0, int userId = 0, bool isPendingPayment = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sorts:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, new { Filter = filters, Sort = sortCollection });
            //Set filters for UserId / AccountId.
            SetFilters(filters, accountId, userId, isPendingPayment);
            AccountQuoteListModel accountQuoteListModel = _accountQuoteClient.GetAccountQuoteList(null, filters, sortCollection, pageIndex, recordPerPage);
            AccountQuoteListViewModel accountQuoteListViewModel = new AccountQuoteListViewModel { AccountQuotes = accountQuoteListModel?.AccountQuotes?.ToViewModel<AccountQuoteViewModel>()?.ToList() };
            SetListPagingData(accountQuoteListViewModel, accountQuoteListModel);
            //if (!isPendingPayment)
            // SetQuotesToolMenus(accountQuoteListViewModel);

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //If Quote List View Model count is greater then BindDataToViewModel binds required data to quote list view model.
            return accountQuoteListViewModel.AccountQuotes?.Count > 0 ? BindDataToListViewModel(accountQuoteListModel, accountQuoteListViewModel, accountId, userId)
                : new AccountQuoteListViewModel { AccountQuotes = new List<AccountQuoteViewModel>(), AccountId = accountId, UserId = userId, CustomerName = accountQuoteListModel?.CustomerName, AccountName = accountQuoteListModel?.AccountName, HasParentAccounts = (accountQuoteListModel?.HasParentAccounts).GetValueOrDefault() };
        }

        // Gets an Account Quote by omsQuoteId.
        public virtual AccountQuoteViewModel GetAccountQuote(int omsQuoteId, string updatePageType)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                FilterCollection filters = new FilterCollection();
                filters.Add(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString());

                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeOmsQuoteEnum.ZnodeOmsQuoteLineItems.ToString());
                ZnodeLogging.LogMessage("filters and sorts:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { Filter = filters, Expand = expands });
                AccountQuoteModel accountQuoteModel = _accountQuoteClient.GetAccountQuote(expands, filters);
                AccountQuoteViewModel accountQuoteViewModel = accountQuoteModel?.ToViewModel<AccountQuoteViewModel>();
                accountQuoteViewModel.ShippingAddressModel = accountQuoteModel.ShippingAddressModel?.ToViewModel<AddressViewModel>();
                accountQuoteViewModel.BillingAddressModel = accountQuoteModel.BillingAddressModel?.ToViewModel<AddressViewModel>();
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(accountQuoteModel) ? BindDataToViewModel(accountQuoteViewModel, omsQuoteId, updatePageType)
                    : new AccountQuoteViewModel() { OrderStatusList = GetOrderStateList(omsQuoteId), UpdatePageType = updatePageType };
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Info);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorAccessMessage);
                }
            }
            return null;
        }

        //Convert quote to order.
        public virtual CreateOrderViewModel ConvertToOrder(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString());

            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeOmsQuoteEnum.ZnodeOmsQuoteLineItems.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString());
            ZnodeLogging.LogMessage("filters and sorts:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { Filter = filters, Expand = expands });
            //Get quote details by quote id.   
            AccountQuoteModel accountQuoteModel = _accountQuoteClient.GetAccountQuote(expands, filters);

            if (HelperUtility.IsNotNull(accountQuoteModel?.ShoppingCart?.ShoppingCartItems))
            {
                CreateOrderViewModel createOrderViewModel = new CreateOrderViewModel();

                if (string.Equals(accountQuoteModel.OrderStatus, ZnodeOrderStatusEnum.ORDERED.ToString(), System.StringComparison.InvariantCultureIgnoreCase))
                    return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, Admin_Resources.OrderedQuoteError);

                //Remove all cart items.
                var objCartAgent = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<OrderStateClient>(), GetClient<PortalClient>(), GetClient<IUserClient>());
                objCartAgent.RemoveAllCart(accountQuoteModel?.UserId);

                //Set Quote Information of user.
                new OrderAgent(GetClient<ShippingClient>(), GetClient<ShippingTypeClient>(), GetClient<StateClient>(),
           GetClient<CityClient>(), GetClient<ProductsClient>(), GetClient<BrandClient>(),
           GetClient<UserClient>(), GetClient<PortalClient>(), GetClient<AccountClient>(), GetClient<RoleClient>(),
           GetClient<DomainClient>(), GetClient<OrderClient>(), GetClient<EcommerceCatalogClient>(),
           GetClient<CustomerClient>(), GetClient<PublishProductClient>(), GetClient<MediaConfigurationClient>(),
           GetClient<PaymentClient>(), GetClient<ShoppingCartClient>(), GetClient<AccountQuoteClient>(),
           GetClient<OrderStateClient>(), GetClient<PIMAttributeClient>(), GetClient<CountryClient>(), GetClient<AddressClient>()).SetOrderedInformationOfUser(accountQuoteModel, createOrderViewModel);


                return createOrderViewModel;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return new CreateOrderViewModel() { HasError = true };
        }

        // Update the Account Quote Details.
        public virtual bool UpdateQuoteStatus(AccountQuoteViewModel accountQuoteViewModel)
           => _accountQuoteClient.UpdateQuoteStatus(new QuoteStatusModel() { OmsQuoteIds = accountQuoteViewModel.OmsQuoteId.ToString(), OmsOrderStateId = accountQuoteViewModel.OmsOrderStateId, Notes = accountQuoteViewModel.AdditionalNotes })?.IsUpdated ?? false;

        // Update the Account Quote Details.
        public virtual bool UpdateQuoteStatus(string quoteId, int status, bool isPendingPaymentStatus, string orderStatus, ref string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                if (!string.IsNullOrEmpty(quoteId) && (status > 0 || isPendingPaymentStatus))
                    return _accountQuoteClient.UpdateQuoteStatus(new QuoteStatusModel() { OmsQuoteIds = quoteId, OmsOrderStateId = status, IsPendingPaymentStatus = isPendingPaymentStatus, OrderStatus = orderStatus })?.IsUpdated ?? false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                message = Admin_Resources.UpdateErrorMessage;
                return false;
            }
            return false;
        }

        // Convert Quote to Order.
        public virtual OrderViewModel ConvertToOrder(AccountQuoteViewModel accountQuoteViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                if (HelperUtility.IsNotNull(accountQuoteViewModel))
                {
                    accountQuoteViewModel.CurrencyCode = DefaultSettingHelper.DefaultCurrency;
                    accountQuoteViewModel.CultureCode = DefaultSettingHelper.DefaultCulture;
                    accountQuoteViewModel.CountryCode = DefaultSettingHelper.DefaultCountry;
                    return _accountQuoteClient.ConvertToOrder(accountQuoteViewModel.ToModel<AccountQuoteModel>())?.ToViewModel<OrderViewModel>();
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return (OrderViewModel)GetViewModelWithErrorMessage(new OrderViewModel(), WebStore_Resources.ErrorFailedToCreate);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (OrderViewModel)GetViewModelWithErrorMessage(new OrderViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }
        #endregion

        #region Private Methods
        //Get Order State List.
        private List<SelectListItem> GetOrderStateList(int omsQuoteId)
          => AccountViewModelMap.ToOrderStateSelectListItemList(_orderStateClient.GetOrderStates(null, new FilterCollection() { new FilterTuple(ZnodeOmsOrderStateEnum.IsAccountStatus.ToString(), FilterOperators.Equals, "true") }, null, null, null)?.OrderStates, omsQuoteId);

        //Set filters for UserId / AccountId.
        private void SetFilters(FilterCollection filters, int accountId, int userId, bool isPendingPayment)
        {
            if (filters.Exists(x => x.Item1 == ZnodeAccountEnum.AccountId.ToString()))
                //If AccountId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeAccountEnum.AccountId.ToString());

            if (filters.Exists(x => x.Item1 == ZnodeUserEnum.UserId.ToString()))
                //If UserId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());

            if (userId > 0)
                SetUserIdFilter(filters, userId);

            if (accountId > 0)
                SetAccountIdFilter(filters, accountId);

            filters.Add(new FilterTuple(ZnodeConstant.PendingPayment.ToString(), FilterOperators.Equals, Convert.ToString(isPendingPayment)));
            filters.Add(new FilterTuple(ZnodeConstant.IsParentPendingOrder.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));
        }

        //Set filter for User Id.
        private static void SetUserIdFilter(FilterCollection filters, int userId)
        {
            //Add New UserId Into filters
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString())); return;
        }

        //Set filter for Account Id.
        private static void SetAccountIdFilter(FilterCollection filters, int accountId)
        {
            //Add New AccountId Into filters
            filters.Add(new FilterTuple(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString())); return;
        }

        //Set the Tool Menus for Sub Account List Grid View.
        private void SetQuotesToolMenus(AccountQuoteListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                List<SelectListItem> statusList = GetOrderStateList(0);
                if (statusList?.Count > 0)
                {
                    model.GridModel = new GridModel();
                    model.GridModel.FilterColumn = new FilterColumnListModel();
                    model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                    //Binds the function.
                    foreach (var item in statusList)
                        model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = $"{Admin_Resources.LabelUpdateStatus} to {item.Text}", JSFunctionName = $"Account.prototype.UpdateQuoteStatus(this,{item.Value})", ControllerName = "Account", ActionName = "UpdateQuoteStatus" });
                }
            }
        }

        //Bind required data to quote list view model.
        private AccountQuoteListViewModel BindDataToListViewModel(AccountQuoteListModel accountQuoteListModel, AccountQuoteListViewModel accountQuoteListViewModel, int accountId, int userId)
        {
            //Get Order State List.
            accountQuoteListViewModel.OrderStatusList = GetOrderStateList(0);

            //If account id and user id is greater, page will redirect to customer quote history list on click of back button.
            if (accountId > 0 && userId > 0)
                accountQuoteListViewModel.UpdatePageType = AdminConstants.QuoteHistory;

            //If account id is greater, page will redirect to account quote list on click of back button.
            else if (accountId > 0 && userId <= 0)
                accountQuoteListViewModel.UpdatePageType = AdminConstants.AccountQuotes;

            //If account id and userId is zero, page will redirect to mail quote list on click of back button.
            else if (accountId <= 0 && userId <= 0)
                accountQuoteListViewModel.UpdatePageType = AdminConstants.Quotes;

            accountQuoteListViewModel.AccountName = accountQuoteListModel.AccountName;
            accountQuoteListViewModel.CustomerName = accountQuoteListModel.CustomerName;
            accountQuoteListViewModel.HasParentAccounts = accountQuoteListModel.HasParentAccounts;
            accountQuoteListViewModel.AccountId = accountId;
            accountQuoteListViewModel.UserId = userId;
            return accountQuoteListViewModel;
        }

        //Bind details to account quote view model.
        private AccountQuoteViewModel BindDataToViewModel(AccountQuoteViewModel accountQuoteViewModel, int omsQuoteId, string updatePageType)
        {
            accountQuoteViewModel.OrderStatusList = GetOrderStateList(omsQuoteId);
            accountQuoteViewModel.UpdatePageType = updatePageType;
            accountQuoteViewModel.ShippingConstraints = GetService<IAdminHelper>().GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete).
                    Select(s => new ShippingConstraintsViewModel
                    {
                        IsSelected = string.IsNullOrWhiteSpace(accountQuoteViewModel.ShippingConstraintCode) ? false : accountQuoteViewModel.ShippingConstraintCode.Equals(s.ShippingConstraintCode),
                        ShippingConstraintCode = s.ShippingConstraintCode,
                        Description = s.Description
                    }).ToList();
            
            return accountQuoteViewModel;
        }
        #endregion
    }
}