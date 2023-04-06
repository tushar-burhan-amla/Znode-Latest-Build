using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin.Agents
{
    public class OrderAgent : BaseAgent, IOrderAgent
    {
        #region Private Variable

        protected readonly IOrderClient _orderClient;
        protected readonly IUserClient _userClient;
        protected readonly ICustomerClient _customerClient;
        protected readonly IAccountClient _accountClient;
        protected readonly IPortalClient _portalClient;
        protected readonly IEcommerceCatalogClient _eCommerceCatalogClient;
        protected readonly IPublishProductClient _publishProductClient;
        protected readonly IMediaConfigurationClient _mediaConfigurationClient;
        protected readonly IShippingAgent _shippingAgent;
        protected readonly IEcommerceCatalogClient _portalCatalogClient;
        protected readonly IUserAgent _userAgent;
        protected readonly IPaymentClient _paymentClient;
        protected readonly IShoppingCartClient _shoppingCartClient;
        protected readonly ICartAgent _cartAgent;
        protected readonly IPaymentAgent _paymentAgent;
        protected readonly IShippingClient _shippingClient;
        protected readonly IAccountQuoteClient _quoteClient;
        protected readonly IOrderStateClient _orderStateClient;
        protected readonly IPIMAttributeClient _attributeClient;
        protected readonly IAddressClient _addressClient;
        private const string paymentStatusSettledSuccessfully = "SettledSuccessfully";
        private const string paymentStatusPartiallyRefund = "Partially_Refunded";

        #endregion Private Variable

        #region Constructor

        public OrderAgent(IShippingClient shippingClient, IShippingTypeClient shippingTypeClient, IStateClient stateClient,
             ICityClient cityClient, IProductsClient productClient, IBrandClient brandClient,
             IUserClient userClient, IPortalClient portalClient, IAccountClient accountClient, IRoleClient roleClient,
             IDomainClient domainClient, IOrderClient orderClient, IEcommerceCatalogClient ecomCatalogClient,
             ICustomerClient customerClient, IPublishProductClient publishProductClient, IMediaConfigurationClient mediaConfigClient,
             IPaymentClient paymentClient, IShoppingCartClient shoppingCartClient, IAccountQuoteClient accountQuoteClient,
             IOrderStateClient orderStateClient, IPIMAttributeClient pimAttributeClient, ICountryClient countryClient, IAddressClient addressClient)

        {
            _orderClient = GetClient<IOrderClient>(orderClient);
            _userClient = GetClient<IUserClient>(userClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _eCommerceCatalogClient = GetClient<IEcommerceCatalogClient>(ecomCatalogClient);
            _customerClient = GetClient<ICustomerClient>(customerClient);
            _accountClient = GetClient<IAccountClient>(accountClient);
            _publishProductClient = GetClient<IPublishProductClient>(publishProductClient);
            _mediaConfigurationClient = GetClient<IMediaConfigurationClient>(mediaConfigClient);
            _shippingAgent = new ShippingAgent(shippingClient, shippingTypeClient, stateClient, cityClient, productClient, brandClient, userClient, portalClient, accountClient, roleClient, domainClient, GetClient<CurrencyClient>(), countryClient);
            _portalCatalogClient = GetClient<IEcommerceCatalogClient>(ecomCatalogClient);
            _userAgent = new UserAgent(userClient, portalClient, accountClient, roleClient, domainClient, stateClient, GetClient<GlobalAttributeEntityClient>(), GetClient<ShoppingCartClient>());
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
            _shoppingCartClient = GetClient<IShoppingCartClient>(shoppingCartClient);
            _paymentAgent = new PaymentAgent(GetClient<PaymentClient>(), GetClient<ProfileClient>());
            _cartAgent = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<OrderStateClient>(), GetClient<PortalClient>(), GetClient<UserClient>());
            _shippingClient = GetClient<IShippingClient>(shippingClient);
            _quoteClient = GetClient<IAccountQuoteClient>(accountQuoteClient);
            _orderStateClient = GetClient<IOrderStateClient>(orderStateClient);
            _attributeClient = GetClient<IPIMAttributeClient>(pimAttributeClient);
            _addressClient = GetClient<IAddressClient>(addressClient);
        }

        #endregion Constructor

        #region Public Methods

        #region Order

        public virtual OrdersListViewModel GetOrderList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int userId = 0, int accountId = 0, int portalId = 0, string portalName = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //If userId is greater than zero set filter for user id.
            if (userId > 0)
                SetUserIdFilter(filters, userId);
            filters.Add(new FilterTuple(FilterKeys.IsFromAdmin, FilterOperators.Equals, FilterKeys.ActiveTrue));

            RemoveLocalIdFromFilters(filters);

            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            //Add portal id in filter collection.
            AddPortalIdInFilters(_filters, portalId);

            DateRangePickerHelper.FormatFilterForDateTimeRange(_filters, DateTimeRange.Last_30_Days.ToString(), DateTimeRange.All_Orders.ToString());

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = _filters, Sorts = sortCollection });
            OrdersListModel orderList = _orderClient.GetOrderList(null, _filters, sortCollection, pageIndex, recordPerPage);
            OrdersListViewModel ordersListViewModel = new OrdersListViewModel { List = orderList?.Orders?.ToViewModel<OrderViewModel>()?.ToList() };
            SetListPagingData(ordersListViewModel, orderList);
            SetInvoiceToolMenu(ordersListViewModel);

            if (ordersListViewModel?.List?.Count > 0)
            {
                foreach (OrderViewModel order in ordersListViewModel.List)
                    SetOrderListData(order);
            }
            BindStoreFilterValues(ordersListViewModel, portalId, portalName);
            //Remove all the isFromAdmin filter.
            filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.IsFromAdmin.ToString());
            //If Order List View Model count is greater then BindDataToViewModel binds required data to order list view model.
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return ordersListViewModel?.List?.Count > 0 ? BindDataToViewModel(orderList, ordersListViewModel, accountId, userId) : new OrdersListViewModel() { AccountId = accountId, UserId = userId, CustomerName = orderList.CustomerName, PortalName = ordersListViewModel.PortalName, PortalId = ordersListViewModel.PortalId };
        }

        //Get list of Portals, Catalogs and Accounts on the basis of which create new order.
        public virtual CreateOrderViewModel GetCreateOrderDetails(int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            CreateOrderViewModel createOrderViewModel = new CreateOrderViewModel();

            //Get All Portal list.
            createOrderViewModel.PortalList = OrderViewModelMap.ToListItems(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);

            createOrderViewModel.UserAddressDataViewModel = new UserAddressDataViewModel()
            {
                ShippingAddress = new AddressViewModel(),
                BillingAddress = new AddressViewModel(),
            };

            //Get Portal id from list.
            portalId = portalId.Equals(0) ? createOrderViewModel.PortalList?.Count > 0 ? Convert.ToInt32(createOrderViewModel.PortalList.First().Value) : 0 : portalId;

            // Get Publish catalog id by First portal id.
            GetPortalCatalogByPortalId(portalId, createOrderViewModel.UserAddressDataViewModel);

            IAdminHelper _adminHelper = GetService<IAdminHelper>();

            createOrderViewModel.ShippingListViewModel = new ShippingListViewModel()
            {
                ShippingList = new List<ShippingViewModel>(),
                InHandDate = _adminHelper.GetInHandDate(),
                ShippingConstraints = _adminHelper.GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete)
            };

            createOrderViewModel.PaymentSettingViewModel = new PaymentSettingViewModel() { PaymentTypeList = new List<BaseDropDownOptions>() };
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return createOrderViewModel;
        }

        //Get list of Portals, Catalogs and Accounts on the basis of which create new order.
        public virtual List<SelectListItem> GetCatalogListByPortalId(int portalId)
            => OrderViewModelMap.ToListItems(_eCommerceCatalogClient.GetAssociatedPortalCatalogByPortalId(portalId, null, null, null, null, null)?.PortalCatalogs);

        //Get customer list
        public virtual CustomerListViewModel GetCustomerList(int portalId, int accountId, bool isAccountCustomer, FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Set the sort collection for user id desc.
            HelperMethods.SortUserIdDesc(ref sortCollection);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = portalId, AccountId = accountId, IsAccountCustomer = isAccountCustomer, Filters = filters, SortCollection = sortCollection });
            //Check if filter collection is null.
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            //Set filters to get customerList.
            SetCustomerListFilters(filters, portalId, accountId, isAccountCustomer);

            //Set filters to get GuestUser.
            SetGuestUserListFilters(filters);

            //Get the sort collection for lastName asc.
            sortCollection = HelperMethods.SortAsc(ZnodeUserEnum.LastName.ToString(), sortCollection);

            //Get selected column data.
            string columnList = FilterHelpers.GetVisibleColumn();

            //Get Customer List.
            ZnodeLogging.LogMessage("Filters and sorts to get customer account list: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sortCollection });
            UserListModel userList = _userClient.GetCustomerAccountList(HttpContext.Current.User.Identity.Name, filters, sortCollection, pageIndex, recordPerPage, columnList);

            //Bind UserListModel to CustomerListViewModel.
            CustomerListViewModel customerListViewModel = new CustomerListViewModel { List = userList?.Users?.ToViewModel<CustomerViewModel>().ToList() };
            customerListViewModel.List?.ForEach(x => x.PortalId = portalId);
            SetListPagingData(customerListViewModel, userList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return customerListViewModel?.List?.Count > 0 ? customerListViewModel
                : new CustomerListViewModel();
        }

        //Get all details of customer.
        public virtual UserAddressDataViewModel GetCustomerDetails(int portalId, int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            UserAddressDataViewModel userAddressDataViewModel = new UserAddressDataViewModel();

            //Get Portal Catalog By Portal Id.
            GetPortalCatalogByPortalId(portalId, userAddressDataViewModel);

            //Get parent Account Details of user.
            GetAccountdetails(userId, userAddressDataViewModel);

            //Set shipping and billing address of user.
            GetUserAddress(userAddressDataViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddressDataViewModel;
        }

        //Get customer details for update order.
        public virtual UserAddressDataViewModel GetCustomerDetailsForUpdateOrder(OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            UserAddressDataViewModel userAddressDataViewModel = new UserAddressDataViewModel();

            if (IsNotNull(orderModel))
            {
                //Get Portal Catalog By Portal Id.
                GetPortalCatalogByPortalId(orderModel.PortalId, userAddressDataViewModel);

                //Get parent Account Details of user.
                GetAccountdetails(orderModel.UserId, userAddressDataViewModel);

                //Set ordered Billing Shipping Addresses.
                SetOrderedBillingShippingAddresses(orderModel, userAddressDataViewModel);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddressDataViewModel;
        }

        //Get customer details for update order.
        public virtual UserAddressDataViewModel GetCustomerDetailsForUpdateOrder(AccountQuoteModel orderModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            UserAddressDataViewModel userAddressDataViewModel = new UserAddressDataViewModel();

            if (IsNotNull(orderModel))
            {
                //Get Portal Catalog By Portal Id.
                GetPortalCatalogByPortalId(orderModel.PortalId, userAddressDataViewModel);

                //Get parent Account Details of user.
                GetAccountdetails(orderModel.UserId, userAddressDataViewModel);

                //Set ordered Billing Shipping Addresses.
                SetOrderedBillingShippingAddresses(orderModel, userAddressDataViewModel);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddressDataViewModel;
        }

        //Get all details of customer.
        public virtual AddressViewModel GetUserAddressById(int addressId, int userId, int portalId)
        => GetAddressDefaultData(addressId, userId, GetCustomerAccountId(userId), portalId);

        //Get Address Default Data.
        public virtual AddressViewModel GetAddressDefaultData(int addressId, int userId, int accountId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get B2B account or user address list.
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = addressId, PortalId = portalId });
            AddressListModel addressList = GetAddressListOfUserAndAccount(userId, accountId);
            AddressViewModel addressViewModel = GetAddressData(addressList, addressId, portalId);

            UserModel userAccountData = _userClient.GetUserAccountData(userId);
            if (HelperUtility.IsNotNull(userAccountData))
                addressViewModel.IsGuest = string.IsNullOrEmpty(userAccountData.AspNetUserId) ? true : false;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return addressViewModel;
        }

        //Get Address Default Data.
        public virtual AddressViewModel GetAddressDefaultDataWithShipment(int addressId, int userId, int portalId, int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get B2B account or user address list.
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = portalId });
            AddressListModel addressList = _orderClient.GetAddressListWithShipment(orderId, userId);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return GetAddressData(addressList, addressId, portalId);
        }

        public virtual AddressViewModel GetAddressData(AddressListModel addressList, int addressId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Get address from address list by selected address id.
            AddressViewModel addressViewModel = (addressList?.AddressList?.Where(w => w.AddressId == addressId).FirstOrDefault())?.ToViewModel<AddressViewModel>();


            if (IsNull(addressViewModel))
            {
                addressViewModel = new AddressViewModel();
                addressList = GetAddressListByAddressId(addressId);
                addressViewModel = (addressList?.AddressList?.Where(w => w.AddressId == addressId).FirstOrDefault())?.ToViewModel<AddressViewModel>();
            }
            else
            {
                addressViewModel.DontAddUpdateAddress = false;
            }

            //Get User address name list to bind in dropdown.
            addressViewModel.UsersAddressNameList = OrderViewModelMap.ToListItems(addressList.AddressList);

            //Get portal associated country dropdown.
            addressViewModel.Countries = HelperMethods.GetPortalAssociatedCountries(portalId);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return addressViewModel;
        }

        //Add additional notes into order session.
        public virtual bool ManageOrderNotes(OrderViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + model.OmsOrderId);
            orderModel.AdditionalInstructions = model.AdditionalNotes;
            SaveInSession(AdminConstants.OMSOrderSessionKey + model.OmsOrderId, orderModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return true;
        }

        //Update Customer Address And Calculate.
        public virtual CreateOrderViewModel UpdateCustomerAddressAndCalculate(AddressViewModel addressViewModel, bool IsManage = false, int omsOrderId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            CreateOrderViewModel createOrderViewModel = new CreateOrderViewModel()
            {
                UserAddressDataViewModel = new UserAddressDataViewModel(),
                CartViewModel = new CartViewModel()
            };
            string fromBillingShipping = addressViewModel?.FromBillingShipping;
            createOrderViewModel.PortalId = addressViewModel.PortalId;
            createOrderViewModel.UserAddressDataViewModel.PortalId = addressViewModel.PortalId;
            createOrderViewModel.OrderId = omsOrderId;

            try
            {
                //Get Portal Catalog By Portal Id.
                GetPortalCatalogByPortalId(createOrderViewModel.PortalId, createOrderViewModel.UserAddressDataViewModel);

                BooleanModel booleanModel = IsValidAddressForCheckout(createOrderViewModel.UserAddressDataViewModel.EnableAddressValidation, addressViewModel.ToModel<AddressModel>());

                //validate address
                if (!booleanModel.IsSuccess)
                    return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, booleanModel.ErrorMessage ?? Admin_Resources.AddressValidationFailed);
                AddressViewModel updatedAddress = addressViewModel.AccountId > 0 ? addressViewModel.AddressId.Equals(0)
                        ? _accountClient.CreateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>()
                        : _accountClient.UpdateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>()
                        : addressViewModel.AddressId.Equals(0)
                        ? _customerClient.CreateCustomerAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>()
                        : _customerClient.UpdateCustomerAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();

                updatedAddress.IsShippingBillingDifferent = addressViewModel.IsShippingBillingDifferent;
                addressViewModel.AddressId = updatedAddress.AddressId;
                addressViewModel.omsOrderId = omsOrderId;
                _orderClient.UpdateOrderAddress(addressViewModel?.ToModel<AddressModel>());
                //Get B2B account or user address list.
                AddressListModel addressList = GetAddressListOfUserAndAccount(addressViewModel.UserId, addressViewModel.AccountId);
                if (HelperUtility.IsNotNull(addressList))
                {
                    if (addressViewModel.SelectedShippingId == 0 && addressList.AddressList.FirstOrDefault().IsShipping)
                        addressViewModel.SelectedShippingId = addressList.AddressList.First(x => x.IsShipping).AddressId;

                    if (addressViewModel.SelectedBillingId == 0 && addressList.AddressList.FirstOrDefault().IsBilling)
                        addressViewModel.SelectedBillingId = addressList.AddressList.First(x => x.IsBilling).AddressId;
                }
                //Shipping billing Address To CreateOrderViewModel.
                SetAddressToCreateOrderViewModel(addressViewModel, createOrderViewModel, updatedAddress, addressList);

                //Get parent Account Details of user.
                GetAccountdetails(addressViewModel.UserId, createOrderViewModel.UserAddressDataViewModel);

                //Set shipping address, ShippingCountryCode, and calculate.
                if (IsManage)
                    SetShoppingCartModelAndCalculateForManage(addressViewModel, createOrderViewModel, fromBillingShipping, omsOrderId);
                else
                    SetShoppingCartModelAndCalculate(addressViewModel, createOrderViewModel);

                //Shipping methods
                createOrderViewModel.ShippingListViewModel = BindShippingList(addressViewModel.UserId, addressViewModel.IsQuote);

                //Set address list in address session.
                SaveAddressListToSession(addressViewModel.UserId, addressList?.AddressList?.ToViewModel<AddressViewModel>().ToList());

                return createOrderViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        protected virtual CreateOrderViewModel SetUserShippingBillingAddress(AddressViewModel addressViewModel, CreateOrderViewModel createOrderViewModel)
        {
            if (addressViewModel.SelectedShippingId > 0 && createOrderViewModel.UserAddressDataViewModel.ShippingAddress.AddressId != addressViewModel.SelectedShippingId)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressViewModel.SelectedShippingId.ToString()));
                AddressListModel addresslistmodel = _addressClient.GetAddressList(filters, null, null, null);
                createOrderViewModel.UserAddressDataViewModel.ShippingAddress = addresslistmodel.AddressList.FirstOrDefault().ToViewModel<AddressViewModel>();
            }

            if (addressViewModel.SelectedBillingId > 0 && createOrderViewModel.UserAddressDataViewModel.BillingAddress.AddressId != addressViewModel.SelectedBillingId)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressViewModel.SelectedBillingId.ToString()));
                AddressListModel addresslistmodel = _addressClient.GetAddressList(filters, null, null, null);
                createOrderViewModel.UserAddressDataViewModel.BillingAddress = addresslistmodel.AddressList.FirstOrDefault().ToViewModel<AddressViewModel>();
            }

            return createOrderViewModel;
        }

        //Map Updated Customer Address.
        public virtual CustomerInfoViewModel MapAndUpdateCustomerAddress(CreateOrderViewModel createOrderViewModel)
            => new CustomerInfoViewModel()
            {
                OmsOrderId = createOrderViewModel.OrderId,
                CustomerName = createOrderViewModel?.CustomerName,
                UserName = createOrderViewModel?.UserName,
                CustomerId = createOrderViewModel.UserId,
                BillingAddress = createOrderViewModel?.UserAddressDataViewModel?.BillingAddress,
                ShippingAddress = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress,
                orderTotal = MapOrderTotalToCart(createOrderViewModel?.CartViewModel)
            };

        //Get User's full details like Shopping cart, shipping methods, payment option and review order.
        public virtual CreateOrderViewModel GetUserFullDetails(CartParameterModel cartParameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            SetCreatedByUser(cartParameter.UserId);

            CreateOrderViewModel createOrderViewModel = new CreateOrderViewModel();
            createOrderViewModel.PortalList = OrderViewModelMap.ToListItems(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);

            //If portal id is greater than 0 then pass it otherwise get it from user.
            createOrderViewModel.UserAddressDataViewModel = (cartParameter.PortalId > 0)
                ? GetCustomerDetails(cartParameter.PortalId, cartParameter.UserId.GetValueOrDefault())
                : createOrderViewModel.UserAddressDataViewModel = GetCustomerDetailsForQuote(cartParameter, createOrderViewModel.PortalList);

            createOrderViewModel.PortalId = Convert.ToInt32(createOrderViewModel.UserAddressDataViewModel?.PortalId);
            createOrderViewModel.AccountId = createOrderViewModel.UserAddressDataViewModel.AccountId.GetValueOrDefault();

            createOrderViewModel.CartViewModel = SetAddressDetailsToShoppingCart(cartParameter, createOrderViewModel).ToViewModel<CartViewModel>();

            IAdminHelper _adminHelper = GetService<IAdminHelper>();

            createOrderViewModel.ShippingListViewModel = new ShippingListViewModel()
            {
                ShippingList = new List<ShippingViewModel>(),
                InHandDate = _adminHelper.GetInHandDate(),
                ShippingConstraints = _adminHelper.GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete)
            };
            ZnodeLogging.LogMessage("CreateOrderViewModel with PortalId, CatalogId, AccountId and UserId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = createOrderViewModel?.PortalId, CatalogId = createOrderViewModel?.CatalogId, AccountId = createOrderViewModel?.AccountId, UserId = createOrderViewModel?.UserId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return createOrderViewModel;
        }

        //Get User's full details like Shopping cart, shipping methods, payment option and review quote.
        public virtual QuoteCreateViewModel GetUserFullDetailsForQuote(CartParameterModel cartParameter)
       {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            SetCreatedByUser(cartParameter.UserId);

            QuoteCreateViewModel quoteCreateViewModel = new QuoteCreateViewModel();
            quoteCreateViewModel.PortalList = OrderViewModelMap.ToListItems(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);

            //If portal id is greater than 0 then pass it otherwise get it from user.
            quoteCreateViewModel.UserAddressDataViewModel = (cartParameter.PortalId > 0)
                ? GetCustomerDetails(cartParameter.PortalId, cartParameter.UserId.GetValueOrDefault())
                : quoteCreateViewModel.UserAddressDataViewModel = GetCustomerDetailsForQuote(cartParameter, quoteCreateViewModel.PortalList);

            quoteCreateViewModel.PortalId = Convert.ToInt32(quoteCreateViewModel.UserAddressDataViewModel?.PortalId);
            quoteCreateViewModel.AccountId = quoteCreateViewModel.UserAddressDataViewModel.AccountId.GetValueOrDefault();

            quoteCreateViewModel.CartViewModel = SetAddressDetailsToShoppingCart(cartParameter, quoteCreateViewModel.ToModel<CreateOrderViewModel>(), cartParameter.IsQuote).ToViewModel<CartViewModel>();

            ZnodeLogging.LogMessage("QuoteCreateViewModel with PortalId, CatalogId, AccountId and UserId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = quoteCreateViewModel?.PortalId, CatalogId = quoteCreateViewModel?.CatalogId, AccountId = quoteCreateViewModel?.AccountId, UserId = quoteCreateViewModel?.UserId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return quoteCreateViewModel;
        }


        protected virtual void GetUserDetails(UserAddressDataViewModel UserAddressDataViewModel, int accountId)
        {
            AccountModel accountModel = _accountClient.GetAccount(accountId);
            UserAddressDataViewModel.PortalCatalogId = accountModel.PublishCatalogId.GetValueOrDefault();
        }

        //Bind shipping list.
        public virtual ShippingListViewModel BindShippingList(int userId, bool isQuote = false)
        {
            return isQuote ? GetShippingListWithRates(userId, false, isQuote) : GetShippingListWithRates(userId);
        }

        //Bind shipping list.
        [Obsolete]
        public virtual ShippingListViewModel BindShippingList()
          => GetShippingListWithRates();

        //Create new Customer.
        public virtual UserAddressDataViewModel CreateUpdateCustomerAddress(UserAddressDataViewModel userAddressDataViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                //Null check for Model.
                if (IsNotNull(userAddressDataViewModel))
                {
                    //Check if Shipping and Billing address is same or not, if so then store ony one address with IsDefaultShipping and IsDefaultBilling
                    if (userAddressDataViewModel.UseSameAsBillingAddress && IsNotNull(userAddressDataViewModel.BillingAddress))
                    {
                        userAddressDataViewModel.BillingAddress = userAddressDataViewModel.ShippingAddress;
                        //Create update customer address.
                        CreateEditCustomerAddress(userAddressDataViewModel);
                    }
                    else
                        CreateEditCustomerAddress(userAddressDataViewModel);

                    // Save updated address in session.
                    SaveAddressInSession(userAddressDataViewModel);

                    //Get user address details.
                    GetUserAddress(userAddressDataViewModel);
                    SaveInSession(AdminConstants.OMSUserAccountSessionKey, userAddressDataViewModel);
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.AlreadyExist))
                    userAddressDataViewModel = (UserAddressDataViewModel)GetViewModelWithErrorMessage(userAddressDataViewModel, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (UserAddressDataViewModel)GetViewModelWithErrorMessage(userAddressDataViewModel, Admin_Resources.ErrorFailedToCreate);
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddressDataViewModel;
        }

        //Get Country list to create new customer.
        public virtual UserAddressDataViewModel GetCountryList(int portalId, UserAddressDataViewModel userAddressDataViewModel = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Check if model is null.
            if (HelperUtility.IsNull(userAddressDataViewModel))
            {
                //Assign values to all property of user address data model.
                userAddressDataViewModel = new UserAddressDataViewModel();
                userAddressDataViewModel = GetShippingBillingAddresCountries(userAddressDataViewModel);
                userAddressDataViewModel.PortalId = portalId;
                return userAddressDataViewModel;
            }
            return GetShippingBillingAddresCountries(userAddressDataViewModel);
        }

        //Get published product list.
        public virtual PublishProductsListViewModel GetPublishProducts(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ExpandCollection expands = new ExpandCollection();

            //Set Filters and Expands
            SetFiltersAndExpands(ref filters, expands);
            _publishProductClient.UserId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            //Get published product list.
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts });
            PublishProductListModel productListModel = _publishProductClient.GetPublishProductList(expands, filters, sorts, pageIndex, pageSize);

            PublishProductsListViewModel productListViewModel = new PublishProductsListViewModel { PublishProductsList = productListModel?.PublishProducts?.DistinctBy(m => m.SKU).ToViewModel<PublishProductsViewModel>().ToList() };

            if (productListViewModel?.PublishProductsList?.Count > 0)
            {
                ShoppingCartModel cart = GetCartModelFromSession(_publishProductClient?.UserId);
                foreach (PublishProductsViewModel products in productListViewModel.PublishProductsList)
                {
                    //Get Product Price With Currency.
                    GetProductPriceWithCurrency(products, IsNotNull(cart) ? cart.CultureCode : DefaultSettingHelper.DefaultCulture);

                    //Check inventory.
                    if (products.PublishBundleProducts.Count > 0)
                        CheckBundleProductsInventory(products, 0, _publishProductClient?.UserId);
                    else
                        CheckInventory(products, _publishProductClient?.UserId);
                }
            }

            //Set paging
            SetListPagingData(productListViewModel, productListModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return productListViewModel?.PublishProductsList?.Count > 0 ? productListViewModel
                : new PublishProductsListViewModel();
        }

        /// <summary>
        /// Get ordered items quantity by the given sku.
        /// </summary>
        /// <param name="sku">Published product sku</param>
        /// <param name="omsOrderId">OMS order Id</param>
        /// <returns>cart Quantity</returns>
        public virtual decimal GetOrderedItemQuantity(string sku, int omsOrderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SKU = sku, OmsOrderId = omsOrderId });
            CartViewModel cart = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<OrderStateClient>(), GetClient<PortalClient>(), GetClient<UserClient>()).GetCart(omsOrderId, userId);
            decimal? cartQuantity = 0.00M;
            //
            if (cart?.ShoppingCartItems?.Count > 0)
            {
                cartQuantity = (
                   from CartItemViewModel item in cart.ShoppingCartItems
                   where !Equals(item, null) && !Equals(item.AddOnProductSKUs, null)
                   where string.Equals(sku, !string.IsNullOrEmpty(item.ConfigurableProductSKUs) ? item.ConfigurableProductSKUs : item.SKU, StringComparison.OrdinalIgnoreCase) || item.AddOnProductSKUs.Split(',').Contains(sku) || item.BundleProductSKUs.Split(',').Contains(sku)
                   select item.Quantity
                   ).Sum();
            }
            ZnodeLogging.LogMessage("OrderedItemQuantity: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderedItemQuantity = cartQuantity.GetValueOrDefault() });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartQuantity.GetValueOrDefault();
        }

        //Get review order details.
        public virtual ReviewOrderViewModel GetCheckoutReview(ShoppingCartModel cart, AddressViewModel shippingAddress)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(cart))
            {
                cart.ShippingAddress = shippingAddress?.ToModel<AddressModel>();
                cart.ShoppingCartItems.ForEach(item => item.InsufficientQuantity = false);
                ZnodeLogging.LogMessage("ShippingAddress with Id and ShoppingCartItems count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingAddressId = cart?.ShippingAddress?.AddressId, ShoppingCartItemsCount = cart?.ShoppingCartItems?.Count });

                //Save Cart details in session.
                SaveCartModelInSession(shippingAddress?.UserId, cart);

                //Return Shopping cart, shipping address, selected shipping details and error message.
                return new ReviewOrderViewModel()
                {
                    ShippingAddress = shippingAddress,
                    ShoppingCart = cart.ToViewModel<CartViewModel>(),
                    ShippingOption = new ShippingViewModel
                    {
                        ShippingId = IsNotNull(cart.Shipping) ? cart.Shipping.ShippingId : 0,
                        Description = IsNotNull(cart.Shipping) ? cart.Shipping.ShippingDiscountDescription : string.Empty
                    },
                    ErrorMessage = string.Empty,
                    HasError = false
                };
            }
            return new ReviewOrderViewModel()
            {
                ShippingAddress = new AddressViewModel(),
                ShoppingCart = new CartViewModel()
                {
                    ShoppingCartItems = new List<CartItemViewModel>()
                }
            };
        }

        //Save user details in session.
        public virtual void SaveUserDetailsInSession(int portalId, int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = portalId, UserId = userId });
            if (IsNull(_userAgent.GetUserAccountViewModel()))
            {
                UserAddressDataViewModel userAddressDataViewModel = GetCustomerDetails(portalId, userId);
                SaveInSession(AdminConstants.OMSUserAccountSessionKey, userAddressDataViewModel);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }
        }

        public virtual CartViewModel GetShippingChargesForManage(int UserId, int? ShippingId, int? omsOrderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = UserId, ShippingId = ShippingId, OmsOrderId = omsOrderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId);
            ShoppingCartModel cart = orderModel?.ShoppingCartModel;
            string countryCode = cart?.ShippingAddress?.CountryName;
            if (IsNotNull(cart))
            {
                SetCreatedByUser(UserId);
                //Get Shipping details by shipping id.

                ShippingListModel shippingListModel = _shippingClient.GetShippingList(null, null, null, null, null);
                RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderShippingType);
                OrderHistory(orderModel, ZnodeConstant.OrderShippingType, shippingListModel.ShippingList.Where(w => w.ShippingId == orderModel.ShippingId).Select(s => s.Description).FirstOrDefault(), shippingListModel.ShippingList.Where(w => w.ShippingId == ShippingId).Select(s => s.Description).FirstOrDefault());

                ShippingViewModel selectedShippingOption = _shippingAgent.GetShippingById(ShippingId.GetValueOrDefault());

                //Assign shipping details to cart.
                cart.Shipping = new OrderShippingModel
                {
                    ShippingId = IsNull(selectedShippingOption) ? 0 : selectedShippingOption.ShippingId,
                    ShippingDiscountDescription = selectedShippingOption?.Description,
                    ShippingCountryCode = string.IsNullOrEmpty(countryCode) ? string.Empty : countryCode,
                    ShippingName = selectedShippingOption?.Description
                };

                if (cart.GiftCardAmount > 0)
                    cart.IsCalculateVoucher = cart.IsQuoteOrder ? false : true;

                cart.ShippingAddress = cart?.ShippingAddress;
                cart.BillingAddress = cart?.BillingAddress;
                ZnodeLogging.LogMessage("ShippingAddressId and BillingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingAddressId = cart?.ShippingAddress?.AddressId, BillingAddressId = cart?.BillingAddress?.AddressId });
                cart.IsShippingRecalculate = true;
                cart.IsShippingDiscountRecalculate = true;
                cart.InvalidOrderLevelShippingPromotion = new List<PromotionModel>();
                cart = GetCalculatedShoppingCartForEditOrder(cart);

                if (string.IsNullOrEmpty(cart.Shipping.ResponseMessage))
                {
                    orderModel.ShoppingCartModel = cart;
                    orderModel.ShippingId = selectedShippingOption.ShippingId;
                }
                cart.IsShippingRecalculate = false;
                //Save Cart details in session.
                SaveInSession(AdminConstants.OMSOrderSessionKey + omsOrderId, orderModel);

                CartViewModel cartViewModel = GetCartOrderStatusList(cart, orderModel.TrackingUrl);
                cartViewModel.ShippingName = selectedShippingOption?.Description;

                if (!string.IsNullOrEmpty(cart.Shipping.ResponseMessage))
                {
                    cartViewModel.HasError = true;
                    cartViewModel.ShippingErrorMessage = cart.Shipping.ResponseMessage;
                }
                return cartViewModel;
            }
            return null;
        }

        public virtual CartViewModel GetShippingChargesById(CreateOrderViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cart = GetCartModelFromSession(model?.UserId, model.IsQuote);
            cart.IsQuoteOrder = model.IsQuote;
            string countryCode = cart?.ShippingAddress?.CountryName;

            if (IsNotNull(cart))
            {
                if (IsNotNull(cart.OrderLevelShipping))
                {
                    cart.CustomShippingCost = null;
                }
                SetCreatedByUser(model.UserId);
                //Get Shipping details by shipping id.
                ShippingViewModel selectedShippingOption = _shippingAgent.GetShippingById(model.ShippingId.GetValueOrDefault());

                //Assign shipping details to cart.
                cart.Shipping = new OrderShippingModel
                {
                    ShippingId = IsNull(selectedShippingOption) ? 0 : selectedShippingOption.ShippingId,
                    ShippingDiscountDescription = selectedShippingOption?.Description,
                    ShippingCountryCode = string.IsNullOrEmpty(countryCode) ? string.Empty : countryCode,
                    ShippingName = selectedShippingOption?.Description
                };
                ZnodeLogging.LogMessage("ShippingId and ShippingName: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingId = cart?.Shipping?.ShippingId, ShippingName = cart?.Shipping?.ShippingName });

                //Get shipping details from session.
                AddressViewModel shippingAddress = cart?.ShippingAddress?.ToViewModel<AddressViewModel>();
                AddressViewModel billingAddress = cart?.BillingAddress?.ToViewModel<AddressViewModel>();

                ZnodeLogging.LogMessage("ShippingAddressId and BillingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingAddressId = cart?.ShippingAddress?.AddressId, BillingAddressId = cart?.BillingAddress?.AddressId });

                cart.IsCalculatePromotionAndCoupon = model.IsQuote ? false : true;
                cart = _shoppingCartClient.Calculate(cart);

                //Save Cart details in session.
                SaveCartModelInSession(model?.UserId, cart, model.IsQuote);

                CartViewModel cartViewModel = cart.ToViewModel<CartViewModel>();
                cartViewModel.IsQuote = model.IsQuote;
                if (!string.IsNullOrEmpty(cart.Shipping.ResponseMessage))
                {
                    cartViewModel.HasError = true;
                    cartViewModel.ShippingErrorMessage = cart.Shipping.ResponseMessage;
                }
                return cartViewModel;
            }
            return null;
        }

        //Bind payment option list.
        public virtual List<BaseDropDownOptions> BindPaymentList(int userId, int portalId = 0, string paymentType = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId, PortalId = portalId, PaymentType = paymentType });
            SortCollection sorts = new SortCollection();
            sorts.Add(ZnodePaymentSettingEnum.DisplayOrder.ToString(), DynamicGridConstants.ASCKey);

            //Set Filters For Payment List.
            FilterCollection filters = SetFiltersForPaymentList(userId, portalId);
            //Get shipping option list.
            ZnodeLogging.LogMessage("Filters and sorts to get payment settings: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            PaymentSettingListModel paymentOptionListModel = _paymentClient.GetPaymentSettingByUserDetails(new UserPaymentSettingModel { UserId = userId, PortalId = portalId });
            
            // Get Profile based options and merge with All Profile options.
            if (IsNotNull(paymentOptionListModel?.PaymentSettings))
            {
                //Bind all payment option to Select List Item type.
                List<BaseDropDownOptions> paymentTypeItems = BindPaymentOptionToSelectListItem(paymentOptionListModel.PaymentSettings.ToViewModel<PaymentSettingViewModel>().ToList());

                if (!string.IsNullOrEmpty(paymentType))
                    paymentTypeItems.RemoveAll(x => x.Type.ToLower() != paymentType.ToLower());

                return paymentTypeItems.ToList();
            }
            return new List<BaseDropDownOptions>();
        }

        // Get customer details by id.
        public virtual UserViewModel GetUserDetailsByUserId(int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId });

            //Get customer details by user id.
            UserModel userModel = _userClient.GetUserAccountData(userId);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return (IsNotNull(userModel)) ? userModel.ToViewModel<UserViewModel>() : new UserViewModel() { HasError = false, ErrorMessage = string.Empty };
        }

        //Get Publish Product
        public virtual PublishProductsViewModel GetPublishProduct(int publishProductId, int localeId, int portalId, int userId, int catalogId, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PublishProductId = publishProductId, LocaleId = localeId, PortalId = portalId, UserId = userId, CatalogId = catalogId });
            //Set user id in client header.
            SetUserId(userId);
            //Get product by product id.
            PublishProductModel model = _publishProductClient.GetPublishProduct(publishProductId, GetProductFilters(portalId, localeId, catalogId), GetProductExpands());
            ZnodeLogging.LogMessage("PublishProductModel with Id: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PublishProductId = model?.PublishProductId });

            if (IsNotNull(model))
            {
                PublishProductsViewModel viewModel = model.ToViewModel<PublishProductsViewModel>();
                bool callForPricing = Convert.ToBoolean(viewModel.Attributes?.Value(ZnodeConstant.CallForPricing)) || (model.Promotions?.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing)).GetValueOrDefault();
                string minQuantity = viewModel?.Attributes?.Value(ZnodeConstant.MinimumQuantity);
                decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);

                //Check the inventory of product.
                if (viewModel.PublishBundleProducts.Count > 0)
                    CheckBundleProductsInventory(viewModel, 0, userId, quantity);
                else
                    CheckInventory(viewModel, quantity, 0, userId);

                //if addon is required then assign selected addon value.
                viewModel?.AddOns?.Where(x => x.IsRequired).ToList().ForEach(cc => cc.SelectedAddOnValue = new[] { cc.AddOnValues?.FirstOrDefault()?.PublishProductId });

                //Get Addon SKu from required addons.
                string addonSKu = string.Join(",", viewModel.AddOns?.Where(x => x.IsRequired)?.Select(y => y.AddOnValues?.First().SKU));

                if (!string.IsNullOrEmpty(addonSKu) && (IsNotNull(viewModel.Quantity) && viewModel.Quantity > 0))
                    //Check Associated addon inventory.
                    CheckAddOnInventory(viewModel, addonSKu, quantity);

                GetProductFinalPrice(viewModel, viewModel.AddOns, quantity, addonSKu);

                if (IsNull(viewModel.ProductPrice))
                {
                    viewModel.ShowAddToCart = false;
                    viewModel.InventoryMessage = Admin_Resources.ErrorPriceNotAssociate;
                }

                viewModel.ParentProductId = publishProductId;
                viewModel.LocaleId = localeId;
                viewModel.PortalId = portalId;
                viewModel.UserId = userId;
                viewModel.IsConfigurable = IsNotNull(viewModel.Attributes?.Find(x => x.ConfigurableAttribute?.Count > 0));
                viewModel.IsQuote = isQuote;
                if (viewModel.IsConfigurable)
                    GetConfigurableValues(model, viewModel);
                return viewModel;
            }
            return new PublishProductsViewModel();
        }

        // Swap the SKUs of Configurable product with its Child product
        public virtual void SwapSkuOfConfigurableProduct(PublishProductsViewModel viewModel)
        {
            if (IsNotNull(viewModel.ConfigurableProductSKU))
            {
                string sku = viewModel.ConfigurableProductSKU;
                viewModel.ConfigurableProductSKU = viewModel.SKU;
                viewModel.SKU = sku;
            }
        }

        //Get Bundle Products
        public virtual List<BundleProductViewModel> GetBundleProduct(int publishProductId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, publishProductId.ToString());
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, DefaultSettingHelper.DefaultLocale);
            ZnodeLogging.LogMessage("Filters to get bundle products: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            List<BundleProductViewModel> bundleProducts = _publishProductClient.GetBundleProducts(filters).BundleProducts?.ToViewModel<BundleProductViewModel>()?.ToList();
            if (bundleProducts?.Count > 0)
            {
                foreach (BundleProductViewModel bundleProduct in bundleProducts)
                    bundleProduct.SKU = bundleProduct.Attributes.Where(x => x.AttributeCode == ZnodeConstant.ProductSKU)?.FirstOrDefault()?.AttributeValues;
                ZnodeLogging.LogMessage("BundleProducts count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BundleProductsCount = bundleProducts.Count });
                return bundleProducts;
            }
            return new List<BundleProductViewModel>();
        }

        //Submit user's order.
        public virtual CreateOrderViewModel SubmitOrder(CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(createOrderViewModel))
            {
                ShoppingCartModel model = GetCartModelFromSession(createOrderViewModel?.UserId);

                SetCreatedByUser(model.UserId);

                model.OrderNumber = GenerateOrderNumber(model.PortalId);
                if (IsNotNull(model))
                {
                    createOrderViewModel.UserAddressDataViewModel = new UserAddressDataViewModel() { ShippingAddress = model.ShippingAddress.ToViewModel<AddressViewModel>() };
                    BooleanModel booleanModel = IsValidAddressForCheckout(createOrderViewModel);

                    if (!booleanModel.IsSuccess)
                        return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, booleanModel.ErrorMessage ?? Admin_Resources.AddressValidationFailed);

                    //Map all data required to submit order to shopping cart model
                    SetShoppingCartModel(model, createOrderViewModel);

                    //Remove invalid discount code.
                    RemoveInvalidDiscountCode(model);

                    try
                    {
                        OrderModel orderModel = _orderClient.CheckInventoryAndMinMaxQuantity(model);
                        createOrderViewModel.HasError = false;
                        createOrderViewModel.ErrorMessage = string.Empty;
                        //Place order.
                        OrderModel order = _orderClient.CreateOrder(model);

                        //This is used to skip pre-submit order validation, since all the validations have already been validated at the checkout page
                        model.SkipPreprocessing = true;

                        createOrderViewModel.OrderId = IsNotNull(order) ? order.OmsOrderId : 0;
                        if (IsNotNull(order))
                        {
                            createOrderViewModel.ReceiptHtml = order.ReceiptHtml;
                            createOrderViewModel.IsEmailSend = order.IsEmailSend;
                            RemoveUserDetailsFromSessions(orderModel.UserId);
                        }
                    }
                    catch (ZnodeException exception)
                    {
                        ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                        switch (exception.ErrorCode)
                        {
                            case ErrorCodes.ProcessingFailed:
                                return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, Admin_Resources.ProcessingFailedError);

                            case ErrorCodes.MinAndMaxSelectedQuantityError:
                                return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, exception.ErrorMessage);

                            case ErrorCodes.OutOfStockException:
                                return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, Admin_Resources.OutOfStockException);

                            default:
                                return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, Admin_Resources.ErrorFailedToCreate);
                        }
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        return (CreateOrderViewModel)GetViewModelWithErrorMessage(new CreateOrderViewModel(), Admin_Resources.ProcessingFailedError);
                    }
                }
                return createOrderViewModel;
            }
            return new CreateOrderViewModel();
        }

        //Submit user's order.
        public virtual CreateOrderViewModel SubmitQuote(CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(createOrderViewModel))
            {
                //Sets the user id for quote.
                SetCreatedByUser(createOrderViewModel.UserId);

                //Get the cart from session.
                ShoppingCartModel model = GetCartModelFromSession(createOrderViewModel?.UserId) ??
                           _cartAgent.GetCartFromCookie();

                if (IsNotNull(model))
                {
                    BooleanModel booleanModel = IsValidAddressForCheckout(createOrderViewModel);
                    if (!booleanModel.IsSuccess)
                        return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, booleanModel.ErrorMessage ?? Admin_Resources.AddressValidationFailed);

                    //Map all data required to submit quote to shopping cart model
                    SetShoppingCartModelForQuote(model, createOrderViewModel);

                    try
                    {
                        //Create Quote.
                        AccountQuoteModel createdQuote = _quoteClient.Create(model);

                        if (IsNull(createdQuote))
                            return (CreateOrderViewModel)GetViewModelWithErrorMessage(createOrderViewModel, Admin_Resources.ErrorSubmitQuote);

                        createOrderViewModel.AccountId = createdQuote.AccountId;
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        return (CreateOrderViewModel)GetViewModelWithErrorMessage(new CreateOrderViewModel(), Admin_Resources.ErrorFailedToCreate);
                    }
                }
                return createOrderViewModel;
            }
            return new CreateOrderViewModel();
        }

        //Set portalCatalog id and portal Id as filter.
        public virtual void SetProductListFilter(FilterCollectionDataModel model, int portalCatalogId, int portalId, int userId)
        {
            //Remove all filters from FilterCollectionDataModel
            RemoveFiltersForProductList(model);
            model.Filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, FilterKeys.ActiveTrueValue));
            model.Filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, userId.ToString()));

            //Add portalCatalogId and portalId in FilterCollectionDataModel.
            if (portalCatalogId > 0)
                model.Filters.Add(new FilterTuple(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, Convert.ToString(portalCatalogId)));
            if (portalId > 0)
                model.Filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(portalId)));
            ZnodeLogging.LogMessage("ProductListFilter: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = model?.Filters });
        }

        //Remove and add filter for customer name for search.
        public virtual void AddCustomerNameToFilterCollection(FilterCollectionDataModel model, bool isAccountCustomer)
        {
            //Remove all the isAccountCustomer filter.
            model.Filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.IsAccountCustomer.ToString());
            //If true then insert it in filter.
            if (isAccountCustomer)
            {
                model.Filters.Add(new FilterTuple(FilterKeys.IsAccountCustomer, FilterOperators.Equals, "1"));
                ZnodeLogging.LogMessage("Filters with customer name: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = model?.Filters });
            }
        }

        public virtual PublishProductsViewModel GetProductPriceAndInventory(string productSKU, string parentProductSKU, string quantity, string addOnIds, int portalId, int parentProductId = 0, int omsOrderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { productSKU = productSKU, parentProductSKU = parentProductSKU, quantity = quantity, addOnIds = addOnIds, portalId = portalId, parentProductId = parentProductId, omsOrderId = omsOrderId });
            //Get localeId and catalogId by portal id to get publish product.
            int localeId, catalogId;

            GetPortalDetailsById(portalId, out localeId, out catalogId);

            FilterCollection filters = GetProductFilters(portalId, localeId, catalogId);

            _publishProductClient.UserId = Convert.ToInt32(GetCartModelFromSession(userId)?.UserId);

            PublishProductsViewModel productModel = _publishProductClient.GetPublishProductBySKU(new ParameterProductModel { SKU = productSKU, ParentProductId = parentProductId, ParentProductSKU = parentProductSKU }, GetProductExpands(), filters)?.ToViewModel<PublishProductsViewModel>();

            if (!string.Equals(productSKU, parentProductSKU, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(parentProductSKU) && IsNotNull(productModel))
            {
                string sku = productModel.SKU;
                productModel.SKU = parentProductSKU;
                productModel.ConfigurableProductSKU = sku;
            }

            if (IsNotNull(productModel))
            {
                decimal selectedQuantity = 0;
                decimal.TryParse(quantity, out selectedQuantity);

                //Check product inventory.
                if (productModel.PublishBundleProducts.Count > 0)
                    CheckBundleProductsInventory(productModel, 0, userId, selectedQuantity);
                else
                    CheckInventory(productModel, selectedQuantity, omsOrderId);

                //Check Add on inventory only if parent product is in stock.
                if (productModel.ShowAddToCart)
                    CheckAddOnInventory(productModel, addOnIds, selectedQuantity);

                GetProductFinalPrice(productModel, productModel.AddOns, selectedQuantity, addOnIds);
                return productModel;
            }
            else
                return new PublishProductsViewModel();
        }

        //Get order details by order id.
        public virtual CreateOrderViewModel EditOrder(int orderId, int userId = 0, int accountId = 0, string updatePageType = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Set expands for order details.
            ExpandCollection expands = SetExpandsForOrderDetails();

            //Get order details by order id.
            OrderModel orderDetails = _orderClient.GetOrderById(orderId, expands);

            CreateOrderViewModel createOrderViewModel = new CreateOrderViewModel();
            createOrderViewModel.UpdatePageType = updatePageType;
            createOrderViewModel.SendUserId = userId;
            createOrderViewModel.AccountId = accountId;

            if (IsNotNull(orderDetails))
                //Set Ordered Information of user.
                SetOrderedInformationOfUser(orderDetails, createOrderViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return createOrderViewModel;
        }

        //Get order details by order id.
        public virtual CreateOrderViewModel ReOrder(int orderId, int userId = 0, int accountId = 0, string updatePageType = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId, UserId = userId, AccountId = accountId, UpdatePageType = updatePageType });
            //Set expands for order details.
            ExpandCollection expands = SetExpandsForOrderDetails();

            //Get order details by order id.
            OrderModel orderDetails = _orderClient.GetOrderById(orderId, expands);

            CreateOrderViewModel createOrderViewModel = new CreateOrderViewModel();
            createOrderViewModel.UpdatePageType = updatePageType;
            createOrderViewModel.SendUserId = userId;
            createOrderViewModel.AccountId = accountId;

            if (IsNotNull(orderDetails))
                //Set Ordered Information of user.
                SetOrderedInformationOfUserInReorder(orderDetails, createOrderViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return createOrderViewModel;
        }

        //Get Configurable product.
        public virtual PublishProductsViewModel GetConfigurableProduct(ParameterProductModel parameters)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (parameters.ParentProductId > 0)
            {
                Dictionary<string, string> SelectedAttributes = GetAttributeValues(parameters.Codes, parameters.Values);
                parameters.SelectedAttributes = SelectedAttributes;
                //Set user id in client header.
                SetUserId(parameters.UserId);

                PublishProductModel publishProductModel = _publishProductClient.GetConfigurableProduct(parameters, GetProductExpands());

                // If it doesn't have personalize attribute then Add parent product's personalize attribute
                AddPersonalizeAttributeToChildProduct(parameters, publishProductModel);
                if (IsNotNull(publishProductModel))
                {
                    PublishProductsViewModel viewModel = publishProductModel.ToViewModel<PublishProductsViewModel>();
                    ParameterProductModel productAttribute = null;
                    ConfigurableAttributeViewModel configurableData = null;
                    //If Product is default configurable product.
                    if (publishProductModel.IsDefaultConfigurableProduct)
                    {
                        //Default product attribute.
                        PublishAttributeViewModel defaultAttribute = viewModel.Attributes?.FirstOrDefault(x => x.IsConfigurable);
                        //Get parameter model.
                        productAttribute = GetConfigurableParameterModel(parameters.ParentProductId, parameters.LocaleId, parameters.PortalId, parameters.SelectedCode, parameters.SelectedValue, SelectedAttributes, parameters.PublishCatalogId);
                        //Get product attribute values.
                        configurableData = GetProductAttribute(parameters.ParentProductId, productAttribute,
                                              viewModel.Attributes.Where(x => x.IsConfigurable && x.ConfigurableAttribute.Count > 0).ToList(), publishProductModel.IsDefaultConfigurableProduct);
                        //Set message id combination does not exist.
                        configurableData.CombinationErrorMessage = WebStore_Resources.ProductCombinationErrorMessage;
                        viewModel.IsDefaultConfigurableProduct = publishProductModel.IsDefaultConfigurableProduct;
                    }
                    else
                    {
                        //Get parameter model.
                        productAttribute = GetConfigurableParameterModel(parameters.ParentProductId, parameters.LocaleId, parameters.PortalId, parameters.SelectedCode, parameters.SelectedValue, SelectedAttributes, parameters.PublishCatalogId);

                        //Get product attribute values.
                        configurableData = GetProductAttribute(parameters.ParentProductId, productAttribute,
                        viewModel.Attributes.Where(x => x.IsConfigurable && x.ConfigurableAttribute.Count > 0).ToList(), publishProductModel.IsDefaultConfigurableProduct);
                    }

                    //Map Product configurable product data.
                    MapConfigurableProductData(parameters.ParentProductId, viewModel, configurableData);

                    return viewModel;
                }
            }
            return new PublishProductsViewModel { Attributes = new List<PublishAttributeViewModel>() };
        }

        //Get list of bundle product.
        public virtual List<GroupProductViewModel> GetGroupProductList(int productId, int localeId, int portalId, int userId, int? catalogId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<GroupProductViewModel> groupProductList = GetGroupProducts(productId, localeId, portalId, userId, catalogId);

            if (groupProductList?.Count > 0)
            {
                //Assign parent product of all group products.
                groupProductList.ForEach(x => x.ParentPublishProductId = productId);
                groupProductList.ForEach(x => x.IsCallForPricing = Convert.ToBoolean(x.Attributes?.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.CallForPricing)?.AttributeValues));

                //Check inventory of all group products.
                foreach (GroupProductViewModel groupProduct in groupProductList)
                {
                    CheckGroupInventory(groupProduct, Convert.ToDecimal(groupProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.AttributeValues));
                    if (Equals(groupProduct.RetailPrice, null))
                    {
                        groupProduct.ShowAddToCart = false;
                        groupProduct.InventoryMessage = WebStore_Resources.ErrorPriceNotAssociate;
                    }
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return groupProductList;
        }

        //This method will use to call the payment and process the order
        public virtual SubmitOrderViewModel ProcessCreditCardPayment(SubmitPaymentViewModel submitPaymentViewModel, bool isPaypalExpressCheckout = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get shopping Cart from Session or cookie           
            ShoppingCartModel shoppingCart = GetCartModelFromSession(submitPaymentViewModel?.UserId) ??
                           _cartAgent.GetCartFromCookie();
            try
            {
                if (IsNull(shoppingCart))
                {
                    RemoveCartModelInSession(submitPaymentViewModel?.UserId);
                    if (isPaypalExpressCheckout)
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), string.Empty);

                    return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), Admin_Resources.ErrorNoItemsShoppingCart);
                }

                RemoveInvalidDiscountCode(shoppingCart);

                MapShoppingCartModel(shoppingCart, submitPaymentViewModel);

                //Set User Details
                shoppingCart.UserDetails = _userClient.GetUserAccountData(submitPaymentViewModel.UserId);
                if (IsNull(shoppingCart.UserDetails))
                {
                    RemoveCartModelInSession(submitPaymentViewModel?.UserId);
                    return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), Admin_Resources.ErrorCustomerNotExistForOrder);
                }

                //Set shipping Address
                shoppingCart.ShippingAddress = shoppingCart.ShippingAddress?.AddressId > 0 ? shoppingCart.ShippingAddress : shoppingCart?.UserDetails?.AccountId.GetValueOrDefault() > 0 ? GetAccountAddress(submitPaymentViewModel.ShippingAddressId) : GetCustomerAddress(submitPaymentViewModel.ShippingAddressId);
                if (IsNull(shoppingCart.ShippingAddress))
                {
                    RemoveCartModelInSession(submitPaymentViewModel?.UserId);
                    return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), Admin_Resources.ErrorCustomerShippingAddress);
                }

                BooleanModel booleanModel = IsValidAddressForCheckout(submitPaymentViewModel.EnableAddressValidation, shoppingCart.ShippingAddress);
                //validate address
                if (!booleanModel.IsSuccess)
                {
                    //Uncomment once GetCartFromCookie start working
                    //RemoveInSession(AdminConstants.CartModelSessionKey);
                    return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), booleanModel.ErrorMessage ?? Admin_Resources.AddressValidationFailed);
                }

                //Set Billing Address
                shoppingCart.BillingAddress = shoppingCart.BillingAddress?.AddressId > 0 ? shoppingCart.BillingAddress : shoppingCart?.UserDetails?.AccountId.GetValueOrDefault() > 0 ? GetAccountAddress(submitPaymentViewModel.BillingAddressId) : GetCustomerAddress(submitPaymentViewModel.BillingAddressId);

                //Set shipping
                shoppingCart.Shipping = shoppingCart.Shipping?.ShippingId > 0 ? shoppingCart.Shipping : _shippingAgent.GetShippingById(submitPaymentViewModel.ShippingOptionId).ToModel<OrderShippingModel>();

                shoppingCart.OrderNumber = !string.IsNullOrEmpty(submitPaymentViewModel.OrderNumber) ? submitPaymentViewModel.OrderNumber
                                        : GenerateOrderNumber(shoppingCart.PortalId);

                //Will be changed with Address change in Create order
                //if (IsNull(shoppingCart?.Payment?.PaymentSetting))
                SetUsersPaymentDetails(submitPaymentViewModel.PaymentSettingId, shoppingCart, false);
                OrderModel orderModel = _orderClient.CheckInventoryAndMinMaxQuantity(shoppingCart);
                GatewayResponseModel gatewayResponse = null;
                if (!isPaypalExpressCheckout)
                {
                    //Map shopping Cart model and submit Payment view model to Submit payment model
                    SubmitPaymentModel model = PaymentViewModelMap.ToModel(shoppingCart, submitPaymentViewModel);

                    //Save Customer Payment Guid for Save Credit Card
                    SaveCustomerPaymentGuid(submitPaymentViewModel.UserId, submitPaymentViewModel.CustomerGuid, shoppingCart.UserDetails.CustomerPaymentGUID);

                    gatewayResponse = _paymentAgent.ProcessPayNow(model);
                    if (gatewayResponse?.HasError ?? true || string.IsNullOrEmpty(gatewayResponse?.Token))
                    {
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? (string.Equals(gatewayResponse.ErrorMessage , Admin_Resources.ErrorCardConnectGatewayResponse, StringComparison.InvariantCultureIgnoreCase) ? Admin_Resources.ErrorProcessPayment : gatewayResponse.ErrorMessage) : Admin_Resources.ErrorProcessPayment);
                    }
                    //Map payment token
                    shoppingCart.Token = gatewayResponse.Token;
                    shoppingCart.TransactionDate = gatewayResponse.TransactionDate;
                    if (IsNotNull(shoppingCart?.Payment?.PaymentSetting?.GatewayCode) && (string.Equals(shoppingCart.Payment.PaymentSetting.GatewayCode, ZnodeConstant.CyberSource, StringComparison.InvariantCultureIgnoreCase) || string.Equals(shoppingCart.Payment.PaymentSetting.GatewayCode, ZnodeConstant.AuthorizeNet, StringComparison.InvariantCultureIgnoreCase)))
                        shoppingCart.CreditCardNumber = gatewayResponse.CardNumber;
                }
                else
                {
                    shoppingCart.Token = submitPaymentViewModel.Token;
                    shoppingCart.CardType = "paypal";
                }

                //Map additional information
                if (!string.IsNullOrEmpty(submitPaymentViewModel.AdditionalInfo))
                    shoppingCart.AdditionalInstructions = submitPaymentViewModel.AdditionalInfo;

                SetCreatedByUser(shoppingCart.UserId);

                if (!isPaypalExpressCheckout && HelperUtility.IsNull(submitPaymentViewModel.CustomerGuid) && HelperUtility.IsNull(shoppingCart.UserDetails.CustomerPaymentGUID))
                {
                    SaveCustomerPaymentGuid(submitPaymentViewModel.UserId, gatewayResponse.CustomerGUID, shoppingCart.UserDetails.CustomerPaymentGUID);
                }
                BooleanModel boolModel = new BooleanModel();

                if (!isPaypalExpressCheckout && (!gatewayResponse?.IsGatewayPreAuthorize ?? true))
                {
                    boolModel = CapturePayment(gatewayResponse.Token);
                    if (IsNull(booleanModel) || booleanModel.HasError)
                        throw new ZnodeException(ErrorCodes.PaymentCaptureError, Admin_Resources.PaymentCaptureErrorMessage);
                }

                OrderModel order = _orderClient.CreateOrder(shoppingCart);

                RemoveCookie(AdminConstants.CartCookieKey);
                RemoveCartModelInSession(submitPaymentViewModel?.UserId);

                string errorMessage = string.Empty;
                if (!isPaypalExpressCheckout && (!gatewayResponse?.IsGatewayPreAuthorize ?? true))
                    UpdateCapturedPaymentDetails(order.OmsOrderId, gatewayResponse.Token, false, boolModel, out errorMessage);

                return new SubmitOrderViewModel { OrderId = order.OmsOrderId, ReceiptHtml = WebUtility.HtmlEncode(order.ReceiptHtml), IsEmailSend = order.IsEmailSend };
            }
            catch (ZnodeException exception)
            {
                ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                switch (exception.ErrorCode)
                {
                    case ErrorCodes.MinAndMaxSelectedQuantityError:
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), exception.ErrorMessage);

                    case ErrorCodes.OutOfStockException:
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel() { }, Admin_Resources.OutOfStockException);

                    case ErrorCodes.PaymentCaptureError:
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel() { }, exception.ErrorMessage);

                    default:
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), Admin_Resources.ErrorUnablePlaceOrder);
                }
            }
            catch (Exception exception)
            {
                ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), Admin_Resources.ErrorUnablePlaceOrder);
            }
        }

        //To generate unique order number on basis of current date.
        public virtual string GenerateOrderNumber(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string portalName = _portalClient.GetPortal(portalId, null)?.StoreName;
            // var _erpInc = new ERPInitializer<SubmitOrderModel>(submitOrderModel, "GetOrderNumber");
            string orderNumber = string.Empty;

            if (!string.IsNullOrEmpty(portalName))
                orderNumber = portalName.Trim().Length > 2 ? portalName.Substring(0, 2) : portalName.Substring(0, 1);

            string randomSuffix = GetRandomCharacters();

            DateTime date = DateTime.Now;
            // we have removed '-fff' from the date string as order number field length not exceeds the limit.
            // This change in made for the ticket ZPD-13806
            String strDate = date.ToString("yyMMdd-HHmmss");
            orderNumber += $"-{strDate}-{randomSuffix}";

            ZnodeLogging.LogMessage("Order number: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderNumber = orderNumber.ToUpper() });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderNumber.ToUpper();
        }

        //to process paypal checkout
        public virtual List<string> ProcessPayPalCheckout(SubmitPaymentViewModel submitPaymentViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<string> response = new List<string>();
            //Get shopping Cart from Session or cookie

            ShoppingCartModel shoppingCart = GetCartModelFromSession(submitPaymentViewModel?.UserId) ??
                           _cartAgent.GetCartFromCookie();
            try
            {
                if (IsNull(shoppingCart))
                {
                    response.Add(Admin_Resources.ErrorCustomerNotExistForOrder);
                    response.Add(ZnodeConstant.Error);
                    return response;
                }

                MapShoppingCartModel(shoppingCart, submitPaymentViewModel);
                RemoveInvalidDiscountCode(shoppingCart);
                //Set User Details
                shoppingCart.UserDetails = shoppingCart.UserDetails?.UserId > 0 ? shoppingCart.UserDetails : _userClient.GetUserAccountData(submitPaymentViewModel.UserId);
                if (IsNull(shoppingCart.UserDetails))
                {
                    response.Add(Admin_Resources.ErrorCustomerNotExistForOrder);
                    response.Add(ZnodeConstant.Error);
                    return response;
                }

                //Set shipping Address
                shoppingCart.ShippingAddress = shoppingCart.ShippingAddress?.AddressId > 0 ? shoppingCart.ShippingAddress : GetCustomerAddress(submitPaymentViewModel.ShippingAddressId);
                if (IsNull(shoppingCart.ShippingAddress))
                {
                    response.Add(Admin_Resources.ErrorCustomerShippingAddress);
                    response.Add(ZnodeConstant.Error);
                    return response;
                }

                BooleanModel booleanModel = IsValidAddressForCheckout(submitPaymentViewModel.EnableAddressValidation, shoppingCart.ShippingAddress);
                //validate address
                if (!booleanModel.IsSuccess)
                {
                    response.Add(booleanModel.ErrorMessage ?? Admin_Resources.AddressValidationFailed);
                    response.Add(ZnodeConstant.Error);
                    return response;
                }

                //Set Billing Address
                shoppingCart.BillingAddress = shoppingCart.BillingAddress?.AddressId > 0 ? shoppingCart.BillingAddress : GetCustomerAddress(submitPaymentViewModel.BillingAddressId);

                //Set shipping
                shoppingCart.Shipping = shoppingCart.Shipping?.ShippingId > 0 ? shoppingCart.Shipping : _shippingAgent.GetShippingById(submitPaymentViewModel.ShippingOptionId).ToModel<OrderShippingModel>();

                //Will be changed with Address change in Create order
                //if (IsNull(shoppingCart?.Payment?.PaymentSetting))
                SetUsersPaymentDetails(submitPaymentViewModel.PaymentSettingId, shoppingCart, false);

                //Map shopping Cart model and submit Payment view model to Submit payment model
                SubmitPaymentModel model = PaymentViewModelMap.ToModel(shoppingCart, submitPaymentViewModel);

                //to set total amount in case if previous order payment type equals to current payment type that is credit card
                //in that case we need to make credit card transaction only for the difference amount
                if (!Equals(shoppingCart.OmsOrderId, null) && shoppingCart.OmsOrderId > 0)
                    model.Total = GetOrderTotal(null, submitPaymentViewModel?.UserId);

                if (!Equals(model, null))
                {
                    model.ReturnUrl = submitPaymentViewModel.PayPalReturnUrl;
                    model.CancelUrl = submitPaymentViewModel.PayPalCancelUrl;
                    model.PaymentSettingId = submitPaymentViewModel.PaymentSettingId;
                    model.PaymentApplicationSettingId = submitPaymentViewModel.PaymentApplicationSettingId;
                }

                OrderModel orderModel = _orderClient.CheckInventoryAndMinMaxQuantity(shoppingCart);

                GatewayResponseModel gatewayResponse = _paymentAgent.ProcessPayPal(model);

                if (gatewayResponse?.HasError ?? true || string.IsNullOrEmpty(gatewayResponse?.Token))
                {
                    response.Add(Convert.ToString(false));
                    response.Add(ZnodeConstant.Error);
                    return response;
                }
                else
                {
                    SaveCartModelInSession(submitPaymentViewModel?.UserId, shoppingCart);
                    response.Add(gatewayResponse.ResponseText);
                    response.Add(gatewayResponse.PaymentToken);
                    return response;
                }
            }
            catch (ZnodeException exception)
            {
                ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                if (exception.ErrorCode.Equals(ErrorCodes.MinAndMaxSelectedQuantityError) || exception.ErrorCode.Equals(ErrorCodes.OutOfStockException))
                {
                    response.Add(exception.ErrorMessage);
                    response.Add(ZnodeConstant.Error);
                    return response;
                }
                else
                {
                    response.Add(Admin_Resources.ErrorUnablePlaceOrder);
                    response.Add(ZnodeConstant.Error);
                    return response;
                }
            }
            catch (Exception exception)
            {
                ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response.Add(Admin_Resources.ErrorUnablePlaceOrder);
                response.Add(ZnodeConstant.Error);
                return response;
            }
        }

        //Check Group Product Inventory
        public virtual GroupProductViewModel CheckGroupProductInventory(ProductParameterModel parameters, string productSKU, string quantity)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ProductSKU = productSKU, Quantity = quantity });
            string message = string.Empty;

            if (parameters?.PublishProductId > 0 && !string.IsNullOrEmpty(productSKU) && !string.IsNullOrEmpty(quantity))
            {
                List<GroupProductViewModel> groupProductList = GetGroupProducts(parameters.PublishProductId, parameters.LocaleId, parameters.PortalId, parameters.UserId);

                string[] groupQuantity = quantity.Split('_');
                if (groupProductList?.Count > 0 && groupQuantity?.Length > 0)
                {
                    string[] groupSKU = productSKU.Split(',');

                    //Check Inventory and get piece of each group product.
                    for (int index = 0; index < groupQuantity.Length; index++)
                    {
                        GroupProductViewModel groupProduct = groupProductList.FirstOrDefault(x => x.SKU == groupSKU[index]);
                        //Check if selected quantity of group product is null.
                        if (!string.IsNullOrEmpty(groupQuantity[index]) && IsNotNull(groupProduct))
                        {
                            //Check the inventory of group product.
                            CheckGroupInventory(groupProduct, Convert.ToDecimal(groupQuantity[index]), parameters.OMSOrderId);

                            if (!groupProduct.ShowAddToCart)
                                message = groupProduct.InventoryMessage;

                        }
                    }
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return new GroupProductViewModel() { ErrorMessage = message };
        }

        // 
        public virtual bool AddCustomShippingAmount(decimal? amount, decimal? estimateShippingCost, int? userId = 0, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cartModel = GetCartModelFromSession(userId, isQuote);

            bool status = false;
            cartModel.IsCalculatePromotionAndCoupon = isQuote ? false : true;
            if (IsNotNull(amount) && amount >= 0)
            {

                cartModel.CustomShippingCost = amount;
                cartModel.EstimateShippingCost = estimateShippingCost;
                cartModel = GetCalculatedShoppingCartForEditOrder(cartModel, false);
                status = true;
            }
            else
            {
                cartModel.CustomShippingCost = null;
                cartModel.EstimateShippingCost = null;
                cartModel = _shoppingCartClient.Calculate(cartModel);
                status = true;
            }

            SaveCartModelInSession(userId, cartModel, isQuote);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }

        public virtual void AddPersonalizeAttributeToChildProduct(ParameterProductModel parameters, PublishProductModel publishProductModel)
        {
            // Get parent product     
            PublishProductModel parentPublishProductModel = _publishProductClient.GetParentProduct(parameters.ParentProductId, GetProductFilters(parameters.PortalId, parameters.LocaleId, parameters.PublishCatalogId), GetProductExpands());

            List<PublishAttributeModel> parentPersonalizableAttributes = parentPublishProductModel?.Attributes?.Where(x => x.IsPersonalizable)?.ToList();

            if (Convert.ToBoolean(!publishProductModel?.Attributes?.Contains(publishProductModel?.Attributes?.FirstOrDefault(x => x.IsPersonalizable))))
            {
                if (!IsNull(parentPersonalizableAttributes))
                {
                    foreach (PublishAttributeModel parentPersonalizableAttribute in parentPersonalizableAttributes)
                    {
                        publishProductModel.Attributes.Add(parentPersonalizableAttribute);
                    }
                }
            }
        }

        #region Customer list for Autocomplete feature

        public virtual List<CustomerViewModel> GetCustomerListByName(int portalId, string customerName, bool isAccountCustomer, int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();

            //Set filters to get customerList.
            SetCustomerListFilters(filters, portalId);
            filters.Add(new FilterTuple(View_CustomerUserDetailEnum.FullName.ToString(), FilterOperators.Contains, customerName));

            //Insert into filter if it is account's customer.
            if (isAccountCustomer)
                filters.Add(new FilterTuple(FilterKeys.IsAccountCustomer, FilterOperators.Equals, "1"));

            //Insert into filter if it is account's customer.
            if (accountId > 0)
                filters.Add(new FilterTuple(View_CustomerUserDetailEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString()));

            ZnodeLogging.LogMessage("Filters to get customer account list: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            UserListModel userList = _userClient.GetCustomerAccountList(HttpContext.Current.User.Identity.Name, filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userList?.Users?.Count > 0 ? userList.Users.ToViewModel<CustomerViewModel>().ToList() : new List<CustomerViewModel>();
        }

        //Add Customer new address
        public virtual UserAddressDataViewModel CreateEditCustomerAddress(UserAddressDataViewModel userAddressDataViewModel, int userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            SetCreatedByUser(userId);

            userAddressDataViewModel = GetUsersShippingBillingAddress(userAddressDataViewModel);
            userAddressDataViewModel = _orderClient.CreateNewCustomer(userAddressDataViewModel?.ToModel<UserAddressModel>())?.ToViewModel<UserAddressDataViewModel>();
            ZnodeLogging.LogMessage("ShippingAddressId and BillingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingAddressId = userAddressDataViewModel?.ShippingAddress?.AddressId, BillingAddressId = userAddressDataViewModel?.BillingAddress?.AddressId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddressDataViewModel;
        }

        #endregion Customer list for Autocomplete feature

        //Get review order details.
        public virtual ReviewOrderViewModel GetReviewOrder(CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(createOrderViewModel))
            {
                //Get Cart from session.
                ShoppingCartModel cart = GetCartModelFromSession(createOrderViewModel?.UserId, createOrderViewModel.IsQuote);
                CartViewModel shoppingCart = IsNotNull(cart) ? cart.ToViewModel<CartViewModel>() : new CartViewModel();
                shoppingCart.IsQuote = createOrderViewModel.IsQuote;

                //Return Shopping cart, shipping address, selected shipping details and error message.
                return new ReviewOrderViewModel()
                {
                    ShippingAddress = cart?.ShippingAddress?.ToViewModel<AddressViewModel>(),
                    BillingAddress = cart?.BillingAddress?.ToViewModel<AddressViewModel>(),
                    ShoppingCart = shoppingCart,
                    ShippingOption = new ShippingViewModel
                    {
                        ShippingId = cart.ShippingId,
                        Description = IsNotNull(cart.Shipping) ?
                        cart.Shipping.ShippingDiscountDescription
                        : string.Empty
                    },
                    ErrorMessage = string.Empty,
                    HasError = false
                };
            }
            return new ReviewOrderViewModel()
            {
                ShippingAddress = new AddressViewModel(),
                ShoppingCart = new CartViewModel()
                {
                    ShoppingCartItems = new List<CartItemViewModel>(),
                    IsQuote = createOrderViewModel.IsQuote
                }
            };
        }

        //Get order invoice details.
        public virtual OrderInvoiceViewModel GetOrderInvoiceDetails(string orderIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ExpandCollection expands = SetOrderInvoiceFilters();

            ZnodeLogging.LogMessage("OrderIds and Expands to get order details for invoice: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderIds = orderIds, Expands = expands });
            OrdersListModel ordersListModel = _orderClient.GetOrderDetailsForInvoice(new ParameterModel { Ids = orderIds.ToString() }, expands);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return new OrderInvoiceViewModel()
            {
                Orders = ordersListModel?.Orders?.ToViewModel<OrderViewModel>().ToList()
            };
        }

        [Obsolete("To be discontinued in one of the upcoming versions.")]
        public virtual bool CapturePayment(int OmsOrderId, string paymentTransactionToken, bool isUpdateOrderHistory, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = OmsOrderId, isUpdateOrderHistory = isUpdateOrderHistory });
            bool isSuccess = false;
            try
            {
                string message = string.Empty;
                BooleanModel booleanModel = _paymentClient.CapturePayment(paymentTransactionToken);
                if (!booleanModel.HasError)
                {
                    isSuccess = true;
                    _paymentClient.GetCapturedPaymentDetails(OmsOrderId);
                }
                message = (booleanModel?.HasError ?? true) ? Admin_Resources.ErrorCapturedFailed : Admin_Resources.SuccessCapture;
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + OmsOrderId);
                if (IsNotNull(orderModel) && isUpdateOrderHistory)
                    _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = paymentTransactionToken, OrderAmount = orderModel.Total });

                errorMessage = booleanModel.ErrorMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                isSuccess = false;
                errorMessage = ex.Message;
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isSuccess ? _orderClient.UpdateOrderPaymentStatus(OmsOrderId, ZnodeConstant.CAPTURED.ToString()) : false;
        }

        //Capture Payment
        public virtual BooleanModel CapturePayment(string paymentTransactionToken)
        => _paymentClient.CapturePayment(paymentTransactionToken);

        public virtual bool UpdateCapturedPaymentDetails(int OmsOrderId, string paymentTransactionToken, bool isUpdateOrderHistory, BooleanModel booleanModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = OmsOrderId, isUpdateOrderHistory = isUpdateOrderHistory });
            bool isSuccess = false;
            try
            {
                string message = string.Empty;
                if (!booleanModel.HasError)
                {
                    isSuccess = true;
                    _paymentClient.GetCapturedPaymentDetails(OmsOrderId);
                }
                message = (booleanModel?.HasError ?? true) ? Admin_Resources.ErrorCapturedFailed : Admin_Resources.SuccessCapture;
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + OmsOrderId);
                if (IsNotNull(orderModel) && isUpdateOrderHistory)
                    _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = paymentTransactionToken, OrderAmount = orderModel.Total });

                errorMessage = booleanModel.ErrorMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                isSuccess = false;
                errorMessage = ex.Message;
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isSuccess ? _orderClient.UpdateOrderPaymentStatus(OmsOrderId, ZnodeConstant.CAPTURED.ToString()) : false;
        }

        //Void Payment.
        public virtual bool VoidPayment(int OmsOrderId, string paymentTransactionToken, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = OmsOrderId });
            string message = string.Empty;
            BooleanModel booleanModel = _paymentClient.VoidPayment(paymentTransactionToken);

            message = (booleanModel?.HasError ?? true) ? Admin_Resources.ErrorVoidFailed : Admin_Resources.SuccessVoid;
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + OmsOrderId);
            _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = orderModel.PaymentTransactionToken });

            errorMessage = booleanModel?.ErrorMessage;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return (booleanModel?.HasError ?? true) ? false : _orderClient.UpdateOrderPaymentStatus(OmsOrderId, ZnodeConstant.VOIDED.ToString());
        }

        [Obsolete]
        //Get Customer Address by Address Id
        public virtual AddressViewModel GetUserAccountAddressByAddressId(int addressId, bool isB2BCustomer, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = addressId, IsB2BCustomer = isB2BCustomer, PortalId = portalId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressId.ToString()));

            //expand for address.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString());

            ZnodeLogging.LogMessage("Filters and Expands to get account address: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Expands = expands });
            AddressViewModel addressViewModel = isB2BCustomer ? _accountClient.GetAccountAddress(expands, filters).ToViewModel<AddressViewModel>() : _customerClient.GetCustomerAddress(expands, filters).ToViewModel<AddressViewModel>();

            if (IsNotNull(addressViewModel))
            {
                addressViewModel.DontAddUpdateAddress = false;
            }
            else
            {
                addressViewModel = new AddressViewModel();
                addressViewModel.DontAddUpdateAddress = true;
            }


            //Get portal associated country dropdown.
            addressViewModel.Countries = HelperMethods.GetPortalAssociatedCountries(portalId);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return addressViewModel;
        }

        //Get Customer Address by Address Id or User Id
        public virtual AddressViewModel GetUserAccountAddressByAddressId(int addressId, bool isB2BCustomer, int portalId, int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = addressId, IsB2BCustomer = isB2BCustomer, PortalId = portalId });

            AddressViewModel addressViewModel = GetAddressListFromSession(userId)?.FirstOrDefault(x => x.AddressId == addressId);

            if (IsNull(addressViewModel))
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeUserAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressId.ToString()));

                //expand for address.
                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString());
                ZnodeLogging.LogMessage("Filters and Expands to get account address: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Expands = expands });
                addressViewModel = isB2BCustomer ? _accountClient.GetAccountAddress(expands, filters).ToViewModel<AddressViewModel>() : _customerClient.GetCustomerAddress(expands, filters).ToViewModel<AddressViewModel>();
            }
            if (IsNotNull(addressViewModel))
            {
                addressViewModel.DontAddUpdateAddress = false;
            }
            else
            {
                addressViewModel = new AddressViewModel();
                addressViewModel.DontAddUpdateAddress = true;
            }

            //Get portal associated country dropdown.
            addressViewModel.Countries = HelperMethods.GetPortalAssociatedCountries(portalId);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return addressViewModel;
        }

        #region Manage Order

        //Get order details by order id.
        public virtual OrderViewModel Manage(int orderId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                OrderModel orderModel = _orderClient.GetOrderById(orderId, SetExpandForOrderDetails(true));
                if (IsNotNull(orderModel))
                {
                    return FillOrderDetails(orderId, orderModel);
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorAccessMessage);
                }
            }
            return null;
        }

        public virtual AddressViewModel GetorderdetailsById(int orderId, int shippingAddressId, int billingAddressId, string control, int portalId)
        {
            AddressViewModel addressViewModel = new AddressViewModel();
            OrderModel orderModel = _orderClient.GetOrderById(orderId, SetExpandForOrderDetails(true));

            if (IsNotNull(orderModel))
            {
                FillOrderDetails(orderId, orderModel);
                if (control == "shipping")
                {
                    addressViewModel.FromBillingShipping = "Shipping";
                    addressViewModel.UserId = orderModel.UserId;
                    addressViewModel.AddressId = orderModel.ShippingAddress.AddressId;
                    addressViewModel.DisplayName = orderModel.ShippingAddress.DisplayName;
                    addressViewModel.FirstName = orderModel.ShippingAddress.FirstName;
                    addressViewModel.LastName = orderModel.ShippingAddress.LastName;
                    addressViewModel.CompanyName = orderModel.ShippingAddress.CompanyName;
                    addressViewModel.Address1 = orderModel.ShippingAddress.Address1;
                    addressViewModel.Address2 = orderModel.ShippingAddress.Address2;
                    addressViewModel.CityName = orderModel.ShippingAddress.CityName;
                    addressViewModel.StateCode = orderModel.ShippingAddress.StateCode;
                    addressViewModel.PostalCode = orderModel.ShippingAddress.PostalCode;
                    addressViewModel.StateName = orderModel.ShippingAddress.StateName;
                    addressViewModel.CountryName = orderModel.ShippingAddress.CountryName;
                    addressViewModel.PhoneNumber = orderModel.ShippingAddress.PhoneNumber;
                    addressViewModel.omsOrderShipmentId = orderModel.ShippingAddress.omsOrderShipmentId;
                }
                else
                {
                    addressViewModel.FromBillingShipping = "Billing";
                    addressViewModel.UserId = orderModel.UserId;
                    addressViewModel.AddressId = orderModel.BillingAddress.AddressId;
                    addressViewModel.DisplayName = orderModel.BillingAddress.DisplayName;
                    addressViewModel.FirstName = orderModel.BillingAddress.FirstName;
                    addressViewModel.LastName = orderModel.BillingAddress.LastName;
                    addressViewModel.CompanyName = orderModel.BillingAddress.CompanyName;
                    addressViewModel.Address1 = orderModel.BillingAddress.Address1;
                    addressViewModel.Address2 = orderModel.BillingAddress.Address2;
                    addressViewModel.CityName = orderModel.BillingAddress.CityName;
                    addressViewModel.StateCode = orderModel.BillingAddress.StateCode;
                    addressViewModel.PostalCode = orderModel.BillingAddress.PostalCode;
                    addressViewModel.StateName = orderModel.BillingAddress.StateName;
                    addressViewModel.CountryName = orderModel.BillingAddress.CountryName;
                    addressViewModel.PhoneNumber = orderModel.BillingAddress.PhoneNumber;
                    addressViewModel.omsOrderShipmentId = orderModel.ShippingAddress.omsOrderShipmentId;
                }
                addressViewModel.Countries = HelperMethods.GetPortalAssociatedCountries(portalId);
                addressViewModel.omsOrderId = orderId;
                return addressViewModel;
            }
            return null;
        }

        //Fill the order details and return the view model
        protected virtual OrderViewModel FillOrderDetails(int orderId, OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId });
            SetUserDetails(new CreateOrderViewModel()
            {
                UserId = orderModel.UserId,
                UserAddressDataViewModel = new UserAddressDataViewModel() { Email = orderModel.BillingAddress.EmailAddress },
                AdditionalInstructions = orderModel.AdditionalInstructions,
                PurchaseOrderNumber = orderModel.PurchaseOrderNumber
            }, orderModel.ShoppingCartModel);

            if (orderModel.OmsOrderId > 0)
            {
                orderModel.OrderOldValue.OrderLineItems = orderModel.OrderLineItems?.Where(x => x.OrderLineItemState.ToUpper() != ZnodeOrderStatusEnum.RETURNED.ToString())?.ToList();
                orderModel.OrderOldValue.OrderLineItems.ForEach(s => s.Quantity = Convert.ToDecimal(s.Quantity.ToInventoryRoundOff()));
                orderModel.OrderOldValue.ShippingAmount = HelperMethods.FormatPriceWithCurrency(orderModel.ShippingCost, orderModel.CultureCode);
                orderModel.OrderOldValue.OrderState = orderModel.OrderState;
                orderModel.OrderOldValue.ShippingId = orderModel.ShippingId;
                orderModel.ReturnItemList.ReturnItemList.ForEach(x => x.PersonaliseValuesDetail = orderModel.OrderLineItems?.FirstOrDefault(order => order.OmsOrderLineItemsId == x.OmsOrderLineItemsId)?.PersonaliseValuesDetail);

                SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
            }
            orderModel.ShoppingCartModel.TotalAdditionalCost = orderModel.TotalAdditionalCost;
            OrderViewModel orderViewModel = orderModel.ToViewModel<OrderViewModel>();
            orderViewModel.OrderInformation = GetOrderInformation(orderId);
            orderViewModel.CustomerInformation = GetCustomerInformation(orderId, orderModel.ShoppingCartModel.UserDetails);
            orderViewModel.CartInformation = GetOrderLineItems(orderId);
            orderViewModel.ReturnOrderLineItems = GetReturnLineItemList(orderId);
            orderViewModel.AccountId = Convert.ToInt32(orderModel.ShoppingCartModel?.UserDetails?.AccountId);
            if (IsNotNull(orderModel?.PaymentSettingId) &&
                string.Equals(orderModel.PaymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase))
                orderViewModel.IsCaptureDisable = CaptureDisable(orderModel.PaymentSettingId.GetValueOrDefault());
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            orderViewModel.CustomerInformation.OmsOrderId = orderViewModel.OmsOrderId;
            return orderViewModel;
        }

        //Get Information of order (Payment status, order status, tracking number etc.)
        public virtual OrderInfoViewModel GetOrderInformation(int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);
            OrderShippingModel model = null;

            if (IsNull(orderModel?.ShoppingCartModel?.Shipping?.ShippingCode) || IsNull(orderModel?.ShoppingCartModel?.Shipping?.ShippingDiscountDescription))
                model = _shippingAgent.GetShippingById(orderModel.ShippingId).ToModel<OrderShippingModel>();

            if (IsNotNull(orderModel))
            {
                return new OrderInfoViewModel
                {
                    OmsOrderDetailId = orderModel.OmsOrderDetailsId,
                    OmsOrderId = orderId,
                    CreatedByName = orderModel.CreatedByName,
                    PaymentStatus = orderModel.PaymentStatus,
                    PaymentType = orderModel.PaymentType,
                    PaymentDisplayName = orderModel.PaymentDisplayName,
                    StoreName = orderModel.StoreName,
                    OrderNumber = orderModel.OrderNumber,
                    OrderStatus = orderModel.OrderState,
                    OrderDate = Convert.ToDateTime(orderModel.OrderDate.ToTimeZoneDateTimeFormat()),
                    TransactionId = IsNotNull(orderModel.TransactionId) ? orderModel.TransactionId : orderModel.PaymentTransactionToken,
                    TrackingNumber = orderModel.TrackingNumber,
                    ShippingType = orderModel.ShippingTypeName,
                    PurchaseOrderNumber = orderModel.PurchaseOrderNumber,
                    userId = orderModel.UserId,
                    PortalId = orderModel.PortalId,
                    CreditCardNumber = orderModel.CreditCardNumber,
                    ShippingTypeDescription = IsNotNull(model) ? model.ShippingDiscountDescription : orderModel?.ShoppingCartModel?.Shipping?.ShippingDiscountDescription,
                    PODocumentName = orderModel.PoDocument,
                    OrderDateWithTime = orderModel.OrderDate.ToTimeZoneDateTimeFormat(),
                    ShippingTrackingUrl = orderModel.TrackingUrl,
                    ShippingCode = IsNotNull(model) ?  model.ShippingCode : orderModel?.ShoppingCartModel?.Shipping?.ShippingCode,
                    TaxTransactionNumber = orderModel.OrderLineItems?.Where(x => !string.IsNullOrEmpty(x.TaxTransactionNumber))?.FirstOrDefault()?.TaxTransactionNumber,
                    AccountNumber = orderModel?.AccountNumber,
                    ShippingMethod = orderModel?.ShippingMethod,
                    ShippingTypeClassName = orderModel.ShippingTypeClassName,
                    ExternalId = orderModel?.ExternalId,
                    InHandDate = Convert.ToDateTime(Convert.ToDateTime(orderModel?.InHandDate).ToTimeZoneDateFormat()),
                    JobName = orderModel.JobName,
                    ShippingConstraintCode = orderModel.ShippingConstraintCode,
                    ShippingConstraints = GetService<IAdminHelper>().GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete).
                    Select(s => new ShippingConstraintsViewModel
                    {
                        IsSelected = string.IsNullOrWhiteSpace(orderModel.ShippingConstraintCode) ? false : orderModel.ShippingConstraintCode.Equals(s.ShippingConstraintCode),
                        ShippingConstraintCode = s.ShippingConstraintCode,
                        Description = s.Description
                    }).ToList(),
                    ShippingId = orderModel.ShippingId
                };
            }
            return new OrderInfoViewModel();
        }

        //Get Information of Customer (Customer Name Billing/Shipping Address etc.)
        public virtual CustomerInfoViewModel GetCustomerInformation(int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

            UserModel userAccountData = _userClient.GetUserAccountData(orderModel.UserId);
            return SetCutomerInfoModel(userAccountData, orderModel);
        }

        public virtual CustomerInfoViewModel GetCustomerInformation(int orderId, UserModel userModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);
            UserModel userAccountData = null;
            if (IsNull(userModel))
                userAccountData = _userClient.GetUserAccountData(orderModel.UserId);
            else
                userAccountData = userModel;

            return SetCutomerInfoModel(userAccountData, orderModel);
        }

        protected virtual CustomerInfoViewModel SetCutomerInfoModel(UserModel userAccountData, OrderModel orderModel)
        {

            if (HelperUtility.IsNotNull(userAccountData))
                orderModel.BillingAddress.IsGuest = string.IsNullOrEmpty(userAccountData.AspNetUserId) ? true : false;

            if (IsNotNull(orderModel))
            {
                return new CustomerInfoViewModel
                {
                    CustomerName = SetUserName(orderModel, userAccountData),
                    UserName = orderModel.BillingAddress.IsGuest ? orderModel.UserName ?? userAccountData.Email : userAccountData.Email,
                    CustomerId = orderModel.UserId,
                    OrderStatus = orderModel.OrderState,
                    PhoneNumber = orderModel.PhoneNumber,
                    ShippingAddress = orderModel.ShippingAddress?.ToViewModel<AddressViewModel>(),
                    BillingAddress = orderModel.BillingAddress?.ToViewModel<AddressViewModel>(),
                };
            }
            return new CustomerInfoViewModel();
        }

        //Get Information of All Order line items (Unit price, quantity, shipping status, etc.)
        public virtual CartViewModel GetOrderLineItems(int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);
            SetPersonalizeForShoppingCart(orderModel.ShoppingCartModel, orderModel.OrderLineItems);
            if (IsNotNull(orderModel?.ShoppingCartModel))
            {
                CartViewModel cartViewModel = GetCartOrderStatusList(orderModel?.ShoppingCartModel, orderModel.TrackingUrl);
                cartViewModel.ShippingName = orderModel.ShippingTypeName;
                cartViewModel.OmsOrderId = orderId;
                cartViewModel.UserId = cartViewModel.UserId > 0 ? cartViewModel.UserId : orderModel.UserId;
                cartViewModel.LocaleId = cartViewModel.LocaleId > 0 ? cartViewModel.LocaleId : orderModel.ShoppingCartModel.LocaleId;
                cartViewModel.ShoppingCartItems?.ForEach(x => x.TrackingUrl = orderModel.TrackingUrl);
                cartViewModel.OrderState = orderModel?.OrderState;
                cartViewModel.Discount = orderModel.DiscountAmount;
                cartViewModel.ReturnCharges = orderModel.ReturnCharges.GetValueOrDefault();
                cartViewModel.ShippingHandlingCharges = orderModel.ShippingHandlingCharges.GetValueOrDefault();
                cartViewModel.TaxSummaryList = orderModel?.TaxSummaryList?.ToViewModel<TaxSummaryViewModel>()?.ToList();
                SetAdditionalCostForShoppingCart(cartViewModel, orderModel.OrderLineItems);
                SaveLineItemHistorySession(orderModel?.ShoppingCartModel.ShoppingCartItems);
                return cartViewModel;
            }
            return new CartViewModel();
        }

        //Get Information of All return Order line items (Unit price, quantity, shipping status, etc.)
        public virtual ReturnOrderLineItemListViewModel GetReturnLineItems(int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);
            if (orderModel?.ReturnItemList?.ReturnItemList?.Count > 0)
                return new ReturnOrderLineItemListViewModel
                {
                    ReturnItemList = orderModel.ReturnItemList.ReturnItemList.ToViewModel<ReturnOrderLineItemViewModel>()?.ToList(),
                    CurrencyCode = orderModel.ReturnItemList.ReturnItemList.FirstOrDefault()?.CurrencyCode,
                    SubTotal = orderModel.ReturnItemList.SubTotal,
                    TaxCost = orderModel.ReturnItemList.TaxCost,
                    ShippingCost = orderModel.ReturnItemList.ShippingCost,
                    DiscountAmount = orderModel.ReturnItemList.DiscountAmount,
                    CSRDiscount = orderModel.ReturnItemList.CSRDiscount,
                    ShippingDiscount = orderModel.ReturnItemList.ShippingDiscount,
                    ReturnCharges = orderModel.ReturnItemList.ReturnCharges,
                    ImportDuty = orderModel.ReturnItemList.ImportDuty,
                    Total = orderModel.ReturnItemList.Total,
                    ReturnVoucherAmount = orderModel.ReturnVoucherAmount
                };

            return new ReturnOrderLineItemListViewModel();
        }

        //Get order history for manage.
        public virtual OrderHistoryListViewModel GetOrderHistory(int orderId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sortCollection, OrderId = orderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

            OrderHistoryListViewModel ordersListViewModel = new OrderHistoryListViewModel { List = orderModel.OrderHistoryList?.OrderHistoryList?.ToViewModel<OrderHistoryViewModel>().ToList() };
            SetListPagingData(ordersListViewModel, orderModel.OrderHistoryList);
            SetOrderAmountWithCurrency(orderModel.CultureCode, ordersListViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return ordersListViewModel;
        }

        protected virtual void SetOrderAmountWithCurrency(string cultureCode, OrderHistoryListViewModel ordersListViewModel)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CultureCode = cultureCode });
            if (IsNotNull(ordersListViewModel))
            {
                foreach (OrderHistoryViewModel orderHistory in ordersListViewModel.List)
                {
                    orderHistory.OrderAmountWithCurrency = IsNull(orderHistory.OrderAmount) ? string.Empty : HelperMethods.FormatPriceWithCurrency(orderHistory.OrderAmount, cultureCode);
                    orderHistory.OrderDateWithTime = Convert.ToDateTime(orderHistory.CreatedDate).ToString(HelperMethods.GetStringDateTimeFormat());
                }
            }
        }

        public virtual OrderStatusList GetOrderStatus(int omsOrderId, string orderStatus, string pageName = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = omsOrderId, OrderStatus = orderStatus, PageName = pageName });
            List<SelectListItem> statusList;
            if (pageName == AdminConstants.OrderStatus)
            {
                statusList = BindManageOrderStatus(new FilterTuple(ZnodeOmsOrderStateEnum.IsOrderState.ToString().ToLower(), FilterOperators.Equals, "true"));
                return new OrderStatusList()
                {
                    listItem = statusList,
                    pageName = pageName,
                    SelectedItemValue = orderStatus,
                    SelectedItemId = Convert.ToInt32(statusList.FirstOrDefault(x => x.Text == orderStatus).Value)
                };
            }
            else
            {
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId);
                statusList = GetPaymentStateList(orderModel.PaymentType);
                return new OrderStatusList()
                {
                    listItem = statusList,
                    pageName = pageName,
                    SelectedItemValue = orderStatus,
                    SelectedItemId = Convert.ToInt32(statusList.FirstOrDefault(x => x.Text == orderStatus)?.Value)

                };
            }
        }

        public virtual CartViewModel UpdateAmounts(int orderId, string amount, string pagetype)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId, Amount = amount, Pagetype = pagetype });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);
            orderModel.ShoppingCartModel.ReturnItemList = orderModel?.ReturnItemList.ReturnItemList;
            amount = string.IsNullOrEmpty(amount) ? "0.0" : amount;
            if (orderModel?.OmsOrderId > 0 && IsNotNull(orderModel.ShoppingCartModel))
            {
                switch (pagetype)
                {
                    case AdminConstants.ShippingView:
                        RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderShippingCost);
                        OrderHistory(orderModel, ZnodeConstant.OrderShippingCost, Convert.ToString(orderModel.ShoppingCartModel.CustomShippingCost), HelperMethods.FormatPriceWithCurrency(Convert.ToDecimal(amount), orderModel.CultureCode));

                        orderModel.ShoppingCartModel.CustomShippingCost = Convert.ToDecimal(amount);
                        break;

                    case AdminConstants.CSRDiscountAmountView:
                        RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderCSRDiscount);
                        OrderHistory(orderModel, ZnodeConstant.OrderCSRDiscount, Convert.ToString(orderModel.ShoppingCartModel.CSRDiscountAmount), HelperMethods.FormatPriceWithCurrency(Convert.ToDecimal(amount), orderModel.CultureCode));
                        orderModel.ShoppingCartModel.CSRDiscountEdited = orderModel.ShoppingCartModel.CSRDiscountAmount != Convert.ToDecimal(amount) ? true : false;

                        orderModel.ShoppingCartModel.CSRDiscountAmount = Convert.ToDecimal(amount);
                        orderModel.CSRDiscountAmount = Convert.ToDecimal(amount);
                        break;

                    case AdminConstants.TaxView:
                        RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderTax);
                        OrderHistory(orderModel, ZnodeConstant.OrderTax, Convert.ToString(amount), HelperMethods.FormatPriceWithCurrency(Convert.ToDecimal(orderModel.ShoppingCartModel.TaxCost), orderModel.CultureCode));
                        orderModel.IsTaxCostEdited = true;
                        orderModel.ShoppingCartModel.CustomTaxCost = Convert.ToDecimal(amount);
                        break;
                }
                if (orderModel.ShoppingCartModel.GiftCardAmount > 0)
                    orderModel.ShoppingCartModel.IsCalculateVoucher = orderModel.IsQuoteOrder ? false : true;
                orderModel.ShoppingCartModel = GetCalculatedShoppingCartForEditOrder(orderModel.ShoppingCartModel);

                if (!orderModel.ShoppingCartModel.CSRDiscountApplied)
                    RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderCSRDiscount);

                SetCartValueInOrderModel(orderModel);
                SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
                CartViewModel cartViewModel = GetCartOrderStatusList(orderModel.ShoppingCartModel);
                cartViewModel.OrderState = orderModel.OrderState;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return cartViewModel;
            }
            return new CartViewModel();
        }

        public virtual CartViewModel UpdateCartTaxAmounts(int userId, string amount, string pagetype, bool isTaxExempt, bool isQuote = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId, Amount = amount, Pagetype = pagetype });
            ShoppingCartModel shoppingCartModel = GetCartModelFromSession(userId, isQuote);
            amount = string.IsNullOrEmpty(amount) ? "0.0" : amount;
            shoppingCartModel.IsTaxExempt = isTaxExempt;
            if (IsNotNull(shoppingCartModel))
            {
                shoppingCartModel.IsQuoteOrder = isQuote;
                //if it is quote then exclude promotion calculation
                shoppingCartModel.IsCalculatePromotionAndCoupon = isQuote;

                switch (pagetype)
                {
                    case AdminConstants.TaxView:
                        if (isTaxExempt)
                        {
                            shoppingCartModel.CustomTaxCost = Convert.ToDecimal(amount);
                            shoppingCartModel.IsTaxCostEdited = true;
                        }
                        else
                        {
                            shoppingCartModel.CustomTaxCost = null;
                            shoppingCartModel.IsTaxCostEdited = false;
                        }
                        break;
                }

                shoppingCartModel = GetCalculatedShoppingCartForEditOrder(shoppingCartModel, false);
                CartViewModel cartViewModel = GetCartOrderStatusList(shoppingCartModel);
                cartViewModel.IsQuote = isQuote;
                SetCartSummaryDefaultValues(cartViewModel);
                SaveCartModelInSession(userId, shoppingCartModel, isQuote);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return cartViewModel;
            }
            return new CartViewModel();
        }

        /// <summary>
        /// For updating order session.
        /// </summary>
        /// <param name="OrderStatusList">depending on selection SelectedItemId</param>
        /// <returns>OrderStatusList </returns>
        public virtual OrderStatusList UpdateOrderAndPaymentStatus(OrderStatusList orderStatus)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(orderStatus))
            {
                ZnodeLogging.LogMessage("OrderStatusList model with SelectedItemId and OmsOrderId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SelectedItemId = orderStatus?.SelectedItemId, OmsOrderId = orderStatus?.OmsOrderId });
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderStatus.OmsOrderId);
                if (orderModel?.OmsOrderId > 0 && IsNotNull(orderModel.ShoppingCartModel))
                {
                    switch (orderStatus.pageName)
                    {
                        case AdminConstants.OrderStatus:

                            OrderStateListModel orderStateListModel = _orderStateClient.GetOrderStates(null, null, null, null, null);
                            RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderUpdatedStatus);
                            OrderHistory(orderModel, ZnodeConstant.OrderUpdatedStatus,
                                orderStateListModel.OrderStates.Where(w => w.OrderStateId == orderModel.OmsOrderStateId).Select(s => s.OrderStateName).FirstOrDefault(),
                                orderStateListModel.OrderStates.Where(w => w.OrderStateId == orderStatus.SelectedItemId).Select(s => s.OrderStateName).FirstOrDefault());

                            orderModel.OmsOrderStateId = orderStatus.SelectedItemId;
                            orderStatus.SuccessMessage = Admin_Resources.OrderStatusUpdated;

                            break;

                        case AdminConstants.PaymentStatus:
                            PaymentStateListModel paymentStateListModel = _orderClient.GetPaymentStateList();
                            RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderPaymentState);
                            OrderHistory(orderModel, ZnodeConstant.OrderPaymentState,
                                paymentStateListModel.PaymentStateList.Where(w => w.OmsPaymentStateId == orderModel.OmsPaymentStateId).Select(s => s.Name).FirstOrDefault(),
                                paymentStateListModel.PaymentStateList.Where(w => w.OmsPaymentStateId == orderStatus.SelectedItemId).Select(s => s.Name).FirstOrDefault());

                            orderModel.OmsPaymentStateId = orderStatus.SelectedItemId;
                            orderStatus.SuccessMessage = Admin_Resources.PaymentStatusUpdated;
                            break;
                    }
                    SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderStatus.OmsOrderId, orderModel);
                }
            }
            else
            {
                orderStatus.HasError = true;
                orderStatus.ErrorMessage = Admin_Resources.ErrorMessageFailedStatus;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderStatus;
        }

        //It will bind return shipping cost of return line item to it
        protected virtual void BindShippingCostOfReturnItem(ManageOrderDataModel orderDataModel, OrderModel orderModel, ShoppingCartModel cart, decimal originalShippingCost, string guid, ShoppingCartItemModel cartItem)
        {
            decimal returnItemShipping = originalShippingCost - cart.ShippingCost;

            if (returnItemShipping > 0)
            {
                orderModel.ReturnItemList.ReturnItemList.Where(x => x.ExternalId == guid).FirstOrDefault().ShippingCost = returnItemShipping;

                string sku = orderDataModel?.ProductId > 0
                    ? cartItem.ProductName + "-" + Convert.ToString(cartItem.GroupProducts?.Where(y => y.ProductId == orderDataModel.ProductId).Select(s => s.Sku).FirstOrDefault())
                    : sku = cartItem.SKU;

                orderModel.OrderLineItemHistory.Where(x => x.Key.Equals(sku) && string.IsNullOrEmpty(x.Value.ReturnShippingAmount)).FirstOrDefault().Value.ReturnShippingAmount = Convert.ToString(returnItemShipping);
            }
        }

        //Create return line items list
        protected virtual void GetReturnLineItemList(ManageOrderDataModel orderDataModel, OrderModel orderModel, ShoppingCartModel cart, ShoppingCartItemModel cartItemModel)
        {
            cart.ShoppingCartItems?.Remove(cart.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == orderDataModel.Guid));

            if (orderModel.ReturnItemList?.ReturnItemList?.Count > 0)
                orderModel.ReturnItemList.ReturnItemList.Add(GetReturnLineItem(cartItemModel, orderDataModel));
            else
            {
                orderModel.ReturnItemList = new ReturnOrderLineItemListModel { ReturnItemList = new List<ReturnOrderLineItemModel>() };
                orderModel.ReturnItemList.ReturnItemList.Add(GetReturnLineItem(cartItemModel, orderDataModel));
            }
            orderModel.ReturnItemList.SubTotal = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.ExtendedPrice);

            if (!orderModel.IsTaxCostEdited)
            {
                orderModel.ReturnItemList.TaxCost = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.TaxCost);
                orderModel.ReturnItemList.ImportDuty = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.ImportDuty);
            }
            cart.IsLineItemReturned = true;
            cart.ReturnItemList = orderModel.ReturnItemList.ReturnItemList;

            if (orderModel?.ShoppingCartModel.ShoppingCartItems?.Count < 1 && orderModel.ReturnItemList?.ReturnItemList?.Count > 0)
                orderModel.OrderHistory.Add(ZnodeConstant.OrderReturnAllAndCancelStatus, ZnodeOrderStatusEnum.CANCELED.ToString());
        }

        // Update manage order.
        public virtual OrderViewModel UpdateOrder(int orderId, string additionalNote)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId, AdditionalNote = additionalNote });
            try
            {
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);
                if (IsNotNull(orderModel))
                {
                    SetCreatedByUser(orderModel.UserId);

                    orderModel.AdditionalInstructions = additionalNote;
                    if (ValidateOrderOnManage(orderModel))
                    {
                        orderModel.ShoppingCartModel.BillingAddress = orderModel.BillingAddress;
                        orderModel.ShoppingCartModel.ShippingAddress = orderModel.ShippingAddress;
                        orderModel.ShoppingCartModel.UserId = orderModel.UserId;

                        SetUserInformation(orderModel.ShoppingCartModel);

                        SetUsersPaymentDetails(Convert.ToInt32(orderModel.PaymentSettingId), orderModel.ShoppingCartModel, (String.Equals(orderModel.PaymentType, ZnodeConstant.COD, StringComparison.OrdinalIgnoreCase)));

                        //Remove invalid discount code.
                        RemoveInvalidDiscountCode(orderModel.ShoppingCartModel);

                        orderModel.ShoppingCartModel.OrderDate = DateTime.Now;

                        OrderPaymentCreateModel orderPaymentCreateModel = GetOrderPaymentRequestModel(orderModel);

                        OrderPaymentModel orderPaymentModel = _orderClient.GetOrderDetailsForPayment(orderPaymentCreateModel);

                        if (IsNull(orderPaymentModel))
                            throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.AbortSaveInitiateRefundErrorMessage);

                        string errorMessage = "";
                        string successMessage = "";
                        bool isVoidRefundSuccessful = true;

                        if ((IsNotNull(orderModel) && (String.Equals(orderModel.PaymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase) ||
                            String.Equals(orderModel.PaymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase)) &&
                            String.Equals(orderPaymentModel.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.OrdinalIgnoreCase)))
                        {
                            //Void or Refund transactions
                            orderModel.Total = orderPaymentModel.Total;
                            VoidRefundPayment(orderModel,false,out errorMessage, orderId);
                        }

                        if ((IsNotNull(orderModel) && String.Equals(orderModel.PaymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase) ||
                            String.Equals(orderModel.PaymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase)) &&
                            orderPaymentModel.OverDueAmount < 0)
                        {
                            //refund order line item transactions
                            isVoidRefundSuccessful = RefundPaymentByAmount(orderModel.OmsOrderDetailsId, orderModel.PaymentTransactionToken, orderPaymentModel.OverDueAmount, out errorMessage,orderModel);
                        }
                        //Paypal Express Refund for cancel order
                        if ((IsNotNull(orderModel) && String.Equals(orderModel.PaymentType, ZnodeConstant.PAYPAL_EXPRESS, StringComparison.OrdinalIgnoreCase)) &&
                            string.Equals(orderPaymentModel.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            orderModel.Total = orderPaymentModel.Total;
                            VoidRefundPayment(orderModel, false, out errorMessage, orderId);
                            //RefundPaymentByGiftCard(orderModel.OmsOrderDetailsId, orderModel.PaymentTransactionToken, orderModel.LineItemReturnAmount.GetValueOrDefault());
                            //successMessage = Admin_Resources.PaymentRefundByGiftCard;
                        }
                        //Paypal Express Refund
                        if ((IsNotNull(orderModel) && String.Equals(orderModel.PaymentType, ZnodeConstant.PAYPAL_EXPRESS, StringComparison.OrdinalIgnoreCase) ||
                          String.Equals(orderModel.PaymentType, ZnodeConstant.PAYPAL_EXPRESS, StringComparison.OrdinalIgnoreCase)) &&
                          orderPaymentModel.OverDueAmount < 0)
                        {
                            //refund order line item transactions
                            isVoidRefundSuccessful = RefundPaymentByAmount(orderModel.OmsOrderDetailsId, orderModel.PaymentTransactionToken, orderPaymentModel.OverDueAmount, out errorMessage, orderModel);
                        }

                        //Amazon refund.
                        if ((IsNotNull(orderModel) && String.Equals(orderModel.PaymentType, ZnodeConstant.Amazon_Pay, StringComparison.OrdinalIgnoreCase) ||
                          String.Equals(orderModel.PaymentType, ZnodeConstant.Amazon_Pay, StringComparison.OrdinalIgnoreCase)) &&
                          orderPaymentModel.OverDueAmount < 0)
                        {
                            //refund order line item transactions
                            isVoidRefundSuccessful = RefundPaymentByAmount(orderModel.OmsOrderDetailsId, orderModel.PaymentTransactionToken, orderPaymentModel.OverDueAmount, out errorMessage,orderModel);
                        }

                        //Amazon refund for cancel order.
                        if ((IsNotNull(orderModel) && (String.Equals(orderModel.PaymentType, ZnodeConstant.Amazon_Pay, StringComparison.OrdinalIgnoreCase) ||
                           String.Equals(orderModel.PaymentType, ZnodeConstant.Amazon_Pay, StringComparison.OrdinalIgnoreCase)) &&
                           String.Equals(orderPaymentModel.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.OrdinalIgnoreCase)))
                        {
                            //Void or Refund transactions
                            orderModel.Total = orderPaymentModel.Total;
                            VoidRefundPayment(orderModel, false, out errorMessage, orderId);
                        }

                        if(isVoidRefundSuccessful == false)
                            throw new ZnodeException(ErrorCodes.PaymentRefundError, Admin_Resources.PaymentRefundErrorMessage);

                        OrderModel updatedOrderModel = _orderClient.UpdateOrder(orderModel);

                        OrderViewModel updatedOrderViewModel = UpdateMessageInOrderModel(updatedOrderModel, successMessage, errorMessage);

                        RemoveInSession(AdminConstants.OMSOrderSessionKey + orderId);
                        return updatedOrderViewModel;
                    }
                    else
                    {
                        OrderViewModel orderViewModel = orderModel.ToViewModel<OrderViewModel>();
                        orderViewModel.SuccessMessage = Admin_Resources.UpdateMessage;
                        return orderViewModel;
                    }
                }
                return new OrderViewModel();
            }
            catch (Exception exception)
            {
                ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (OrderViewModel)GetViewModelWithErrorMessage(new OrderViewModel(), exception.Message);
            }
        }

        protected virtual OrderViewModel UpdateMessageInOrderModel(OrderModel updatedOrderModel, string successMessage, string errorMessage)
        {
            string shippingErrorMesage = updatedOrderModel?.ShoppingCartModel?.Shipping?.ResponseMessage;

            OrderViewModel updatedOrderViewModel = updatedOrderModel.ToViewModel<OrderViewModel>();
            if (!string.IsNullOrEmpty(shippingErrorMesage))
            {
                updatedOrderViewModel.HasError = true;
                updatedOrderViewModel.ErrorMessage = shippingErrorMesage;
                return updatedOrderViewModel;
            }
            else
            {
                updatedOrderViewModel.SuccessMessage = Admin_Resources.UpdateMessage;
            }

            if (!string.IsNullOrEmpty(errorMessage))
                updatedOrderViewModel.SuccessMessage = $"{updatedOrderViewModel.SuccessMessage} {errorMessage}";

            if (!string.IsNullOrEmpty(successMessage))
                updatedOrderViewModel.SuccessMessage = successMessage;

            return updatedOrderViewModel;
        }


        //To get OrderPaymentCreateModel instance.
        protected virtual OrderPaymentCreateModel GetOrderPaymentRequestModel(OrderModel orderModel)
        {
            OrderPaymentCreateModel orderPaymentCreateModel = orderModel.ToModel<OrderPaymentCreateModel, OrderModel>();

            if(IsNotNull(orderPaymentCreateModel))
            {
                orderPaymentCreateModel.BillingAddressId = orderModel.BillingAddress.AddressId;
                orderPaymentCreateModel.ShippingAddressId = orderModel.BillingAddress.AddressId;
                orderPaymentCreateModel.ShoppingCartOverDueAmount = orderModel.ShoppingCartModel.OverDueAmount.GetValueOrDefault();
                orderPaymentCreateModel.ShoppingCartItemsCount = orderModel.ShoppingCartModel.ShoppingCartItems?.Count;
                orderPaymentCreateModel.IsAllowedTerritories = orderModel.ShoppingCartModel.ShoppingCartItems.Where(w => w.IsAllowedTerritories == false).ToList().Count > 0;
            }
            return orderPaymentCreateModel;
        }

        //TO DO Need to change list of parameters into model
        public virtual CartViewModel UpdateOrderLineItemDetails(ManageOrderDataModel orderDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderDataModel.OrderId);
            ShoppingCartModel cart = orderModel?.ShoppingCartModel;
            decimal originalShippingCost = 0;
            string guid = string.Empty;

            if (IsNull(cart) || cart.ShoppingCartItems?.Count < 1)
                return GetCartOrderStatusList(cart);

            // Check if item exists.
            ShoppingCartItemModel cartItem = cart.ShoppingCartItems.FirstOrDefault(x => x.ExternalId == orderDataModel.Guid);
            if (IsNull(cartItem))
                return GetCartOrderStatusList(cart);

            SetCreatedByUser(cart.UserId);

            if (!IsNull(orderDataModel) && orderDataModel.IsShippingReturn)
            {
                originalShippingCost = cart.ShippingCost;
            }

            decimal orderlineItemQuantity = GetOrderLineItemQuantity(orderModel, cartItem);
            ZnodeLogging.LogMessage("OrderlineItemQuantity: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderlineItemQuantity = orderlineItemQuantity });
            if (orderDataModel.Quantity > orderlineItemQuantity)
            {
                if (!string.IsNullOrEmpty(orderDataModel.OrderLineItemStatus) && Equals(orderDataModel.OrderLineItemStatus.ToLower(), ZnodeOrderStatusEnum.RETURNED.ToString().ToLower()))
                {
                    CartViewModel cartViewModel = GetCartOrderStatusList(cart);
                    cartViewModel.HasError = true;
                    cartViewModel.ErrorMessage = Admin_Resources.ErrorReturned;
                    return cartViewModel;
                }

                if (!cartItem.IsActive)
                {
                    CartViewModel cartViewModel = GetCartOrderStatusList(cart);
                    cartViewModel.HasError = true;
                    cartViewModel.ErrorMessage = Admin_Resources.ErrorProductDisabled;
                    return cartViewModel;
                }

                //Get inventory of sku and Check Inventory
                decimal quantityOnHand = GetQuantityOnHandBySku(cartItem, cart.PortalId, orderDataModel.ProductId);
                decimal orderQuantity = orderDataModel.Quantity - orderlineItemQuantity;
                if (quantityOnHand < orderQuantity && cartItem.TrackInventory && !cartItem.AllowBackOrder)
                {
                    CartViewModel cartViewModel = GetCartOrderStatusList(cart);
                    cartViewModel.HasError = true;
                    cartViewModel.ErrorMessage = Admin_Resources.TextOutofStock;
                    return cartViewModel;
                }
            }
            else
            {
                if (cartItem.IsActive == false && Equals(orderDataModel.OrderLineItemStatus, cartItem.OrderLineItemStatus))
                {
                    CartViewModel cartViewModel = GetCartOrderStatusList(cart);
                    cartViewModel.HasError = true;
                    cartViewModel.ErrorMessage = Admin_Resources.ErrorProductDisabled;
                    return cartViewModel;
                }
            }

            if (IsNotNull(cart))
            {
                //If order line item shipping update
                if (!IsNull(orderDataModel) && orderDataModel.IsOrderLineItemShipping)
                {
                    UpdateOrderLineItemShippingAmount(cart, cartItem, orderDataModel);
                    SetEditShippingOrderHistory(cart, cartItem, orderDataModel, orderModel);
                }

                GetUpdatedOrderLineItem(cart, cartItem, orderDataModel, orderModel);

                cart.ShippingAddress = cart.ShippingAddress;
                cart.Payment = new PaymentModel() { ShippingAddress = cart.ShippingAddress, PaymentSetting = new PaymentSettingModel() };

                //Set Gift Card Number and CSR Discount Amount Data For Calculation
                SetCartDataForCalculation(cart, cart.GiftCardNumber, cart.CSRDiscountAmount);

                if (cart.GiftCardAmount > 0)
                    cart.IsCalculateVoucher = true;

                if (!string.IsNullOrEmpty(orderDataModel.OrderLineItemStatus) && Equals(orderDataModel.OrderLineItemStatus.ToUpper(), ZnodeOrderStatusEnum.RETURNED.ToString()))
                {
                    ShoppingCartItemModel cartItemModel = GetCustomCartItem(cart.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == orderDataModel.Guid));
                    guid = cartItemModel?.ExternalId;

                    GetReturnLineItemList(orderDataModel, orderModel, cart, cartItemModel);
                }

                cart = GetCalculatedShoppingCartForEditOrder(cart);

                //This code will Execute, if ship separately line item return with shipping cost
                if (!IsNull(orderDataModel) && orderDataModel.IsShippingReturn)
                {
                    BindShippingCostOfReturnItem(orderDataModel, orderModel, cart, originalShippingCost, guid, cartItem);
                }

                GetOrderLineItemStatus(cart, orderDataModel);
            }

            orderModel.ShoppingCartModel = cart;
            //To avoid email sending when specific line item will get updated.
            orderModel.IsEmailSend = true;
            SaveInSession(AdminConstants.OMSOrderSessionKey + orderDataModel.OrderId, orderModel);

            return GetCartOrderStatusList(cart);
        }

        //Get list of return line items.
        public virtual ReturnOrderLineItemListViewModel GetReturnLineItemList(int orderId)
            => GetReturnLineItems(orderId);

        #endregion Manage Order

        //Get OrderLine Items With Refund payment left
        public virtual OrderItemsRefundViewModel GetOrderLineItemsWithRefund(int orderDetailsId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderItemsRefundModel refundPayments = _orderClient.GetOrderLineItemsWithRefund(orderDetailsId);

            if (refundPayments?.RefundOrderLineitems?.Count > 0)
            {
                ZnodeLogging.LogMessage("RefundOrderLineitems count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { RefundOrderLineitemsCount = refundPayments?.RefundOrderLineitems?.Count });
                return refundPayments?.ToViewModel<OrderItemsRefundViewModel>();
            }

            return null;
        }

        //Save Refund PAyment Details
        public virtual bool AddRefundPaymentDetails(OrderItemsRefundViewModel refundPaymentListViewModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.PaymentRefundErrorMessage;
            refundPaymentListViewModel.RefundOrderLineitems = IsNull(refundPaymentListViewModel.RefundOrderLineitems) ? new List<RefundPaymentViewModel>() : refundPaymentListViewModel.RefundOrderLineitems;

            //Add shipping Refund Details to RefundOrderLineitems list
            if (IsNotNull(refundPaymentListViewModel.ShippingRefundDetails))
                refundPaymentListViewModel.RefundOrderLineitems.Add(refundPaymentListViewModel.ShippingRefundDetails);

            //Add shipping Total Refund Details to RefundOrderLineitems list
            if (IsNotNull(refundPaymentListViewModel.TotalRefundDetails))
                refundPaymentListViewModel.RefundOrderLineitems.Add(refundPaymentListViewModel.TotalRefundDetails);
            ZnodeLogging.LogMessage("ShippingRefundDetails and TotalRefundDetails with OmsPaymentRefundId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingRefundDetailsWithId = refundPaymentListViewModel?.ShippingRefundDetails?.OmsPaymentRefundId, TotalRefundDetailsWithId = refundPaymentListViewModel?.TotalRefundDetails?.OmsPaymentRefundId });

            //Remove Null and RefundAmount less than zero entries
            refundPaymentListViewModel.RefundOrderLineitems.RemoveAll(x => IsNull(x) || x.RefundAmount <= 0 || IsNull(x.RefundAmount));

            if (refundPaymentListViewModel.RefundOrderLineitems?.Count > 0)
            {
                ZnodeLogging.LogMessage("RefundOrderLineitems list count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { RefundOrderLineitemsCount = refundPaymentListViewModel?.RefundOrderLineitems?.Count });
                decimal totalRefundAmount = refundPaymentListViewModel.RefundOrderLineitems.Sum(x => x.RefundAmount ?? 0.00m);

                if (totalRefundAmount > refundPaymentListViewModel.TotalRefundDetails.RefundableAmountLeft)
                {
                    errorMessage = Admin_Resources.ErrorRefundAmount;
                    return false;
                }
                if (totalRefundAmount > 0)
                {
                    BooleanModel booleanModel = _paymentClient.RefundPayment(new RefundPaymentModel { Token = refundPaymentListViewModel.TransactionId, RefundAmount = totalRefundAmount });
                    if (!booleanModel?.HasError ?? true)
                        return _orderClient.AddRefundPaymentDetails(refundPaymentListViewModel.ToModel<OrderItemsRefundModel>());
                }
            }
            return false;
        }

        //Resend order confirmation email
        public virtual string ResendOrderConfirmationEmail(int orderId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                _orderClient.RefreshCache = true;
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(FilterKeys.OmsOrderStateName, FilterOperators.Equals, Convert.ToString(ZnodeOrderStatusEnum.RETURNED)));

                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

                List<OrderLineItemModel> lineItemModel = orderModel?.OrderLineItems?.Where(x => x.OrderLineItemState != ZnodeOrderStatusEnum.RETURNED.ToString()).ToList();

                if (lineItemModel?.Count <= 0 && orderModel?.ReturnItemList?.ReturnItemList?.Count > 0)
                    return SendReturnedOrderEmail(orderId) ? Admin_Resources.ResendEmailSuccessMessage : Admin_Resources.ResendEmailFailedMessage;

                ZnodeLogging.LogMessage("Filters to resend order confirmation email: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
                string message = _orderClient.ResendOrderConfirmationEmail(orderId, filters, SetExpandForOrderDetails()) ? Admin_Resources.ResendEmailSuccessMessage : Admin_Resources.ResendEmailFailedMessage;

                _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = string.Empty });

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return message;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return ex.ErrorMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return Admin_Resources.Error;
            }
        }

        //Resend order confirmation email for single cart.
        public virtual string ResendOrderConfirmationEmailForCart(int orderId, int omsOrderLineItemId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                _orderClient.RefreshCache = true;

                string message = _orderClient.ResendOrderEmailForCartLineItem(orderId, omsOrderLineItemId, SetExpandForOrderDetails()) ? Admin_Resources.ResendCartMailSuccessMessage : Admin_Resources.ResendEmailFailedMessage;

                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);
                _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = string.Empty });

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return message;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return ex.ErrorMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return Admin_Resources.ResendOrderConfirmationEmail;
            }
        }

        //Bind Default order status and line item status on the basis of order state and orderlineitemstate
        public virtual List<SelectListItem> BindManageOrderStatus(FilterTuple filter = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            if (filter != null) { filters.Add(filter); }
            ZnodeLogging.LogMessage("Filters to get order states: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            return StoreViewModelMap.ToOrderStateList(_orderStateClient.GetOrderStates(null, filters, null, null, null)?.OrderStates, true);
        }

        public virtual bool SendReturnedOrderEmail(int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            _orderClient.RefreshCache = true;
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.OmsOrderId, FilterOperators.Equals, orderId.ToString()));
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, Convert.ToString(true)));
            filters.Add(new FilterTuple(FilterKeys.OmsOrderStateName, FilterOperators.Equals, Convert.ToString(ZnodeOrderStatusEnum.RETURNED)));
            string message = string.Empty;
            bool emailStatus = false;
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

            OrderStateListModel orderStateListModel = _orderStateClient.GetOrderStates(null, null, null, null, null);
            int returnedOrderStateId = Convert.ToInt32(orderStateListModel?.OrderStates?.Where(w => w.OrderStateName == Convert.ToString(ZnodeOrderStatusEnum.RETURNED)).Select(s => s.OrderStateId)?.FirstOrDefault());
            int returnedItemCount = Convert.ToInt32(orderModel?.OrderLineItems?.Where(w => w.OrderLineItemStateId == returnedOrderStateId)?.Count());
            ZnodeLogging.LogMessage("returnedOrderStateId and returnedItemCount: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ReturnedOrderStateId = returnedOrderStateId, ReturnedItemCount = returnedItemCount });

            if (returnedItemCount > 0)
            {
                ZnodeLogging.LogMessage("Filters to send returned order email: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
                emailStatus = _orderClient.SendReturnedOrderEmail(orderId, SetExpandForOrderDetails(), filters);
                message = emailStatus ? Admin_Resources.OrderReturnedEmailSuccess : Admin_Resources.OrderReturnedEmailFailed;
            }
            else
                message = Admin_Resources.OrderReturnedEmailError;

            _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = string.Empty });

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return emailStatus;
        }

        //Get Order Status Details.
        public virtual OrderStateParameterViewModel GetOrderStatusDetails(int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Set Expand For Order Status
            ExpandCollection expands = SetExpandForOrderStatus();

            //Get order details by order id.
            ZnodeLogging.LogMessage("Expands to get order details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands });
            OrderModel orderDetails = _orderClient.GetOrderById(orderId, expands);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderDetails.ToViewModel<OrderStateParameterViewModel>();
        }

        //Bind Order Status dropdown.
        public virtual List<SelectListItem> BindOrderStatus(FilterTuple filter = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeOmsOrderStateEnum.IsAccountStatus.ToString().ToLower(), FilterOperators.Equals, "false"));
            if (filter != null) { filters.Add(filter); }
            ZnodeLogging.LogMessage("Filters to get order states: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });

            List<OrderStateModel> orderStatesList = null;
            orderStatesList = GetFromSession<List<OrderStateModel>>(AdminConstants.OrderStatesList);

            if (IsNull(orderStatesList))
            {
                //Get order state option list.
                orderStatesList= _orderStateClient.GetOrderStates(null, filters, null, null, null)?.OrderStates;
                SaveInSession(AdminConstants.OrderStatesList, orderStatesList);

            }

            return StoreViewModelMap.ToOrderStateList(orderStatesList);
        }

        //Update order status.
        public virtual bool UpdateOrderStatus(OrderStateParameterViewModel model)
                => _orderClient.UpdateOrderStatus(model.ToModel<OrderStateParameterModel>());

        //Add new order note.
        public virtual List<OrderNotesViewModel> AddOrderNote(string additionalNotes, int? omsOrderDetailsId, int? omsQuoteId, int omsOrderId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AdditionalNotes = additionalNotes, OmsOrderDetailsId = omsOrderDetailsId, OmsQuoteId = omsQuoteId, OmsOrderId = omsOrderId });
            if (_orderClient.AddOrderNote(new OrderNotesModel() { OmsOrderDetailsId = omsOrderDetailsId, OmsQuoteId = omsQuoteId, Notes = additionalNotes }))
            {
                OrderNotesListModel noteList = _orderClient.GetOrderNotesList(omsOrderId, omsQuoteId.GetValueOrDefault());
                return noteList?.OrderNotes?.ToViewModel<OrderNotesViewModel>().ToList();
            }
            return new List<OrderNotesViewModel>();
        }

        public virtual OrderViewModel GetDataForReceipt(int omsOrderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = omsOrderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId);
            OrderViewModel orderViewModel = orderModel?.ToViewModel<OrderViewModel>();
            List<OrderLineItemViewModel> orderLineItems = new List<OrderLineItemViewModel>();
            CreateSingleOrderLineItem(orderViewModel, orderLineItems);
            orderViewModel.OrderLineItems = orderLineItems;
            PortalModel portalModel = _portalClient.GetPortal(orderViewModel.PortalId, null);
            orderViewModel.CustomerServiceEmail = portalModel.CustomerServiceEmail;
            orderViewModel.CustomerServicePhoneNumber = portalModel.CustomerServicePhoneNumber;
            orderViewModel.Email = orderModel.ShippingAddress.EmailAddress;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderViewModel;
        }

        public virtual PaymentDetailsViewModel GetOrderPaymentDetails(int paymentSettingId, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            PaymentSettingModel paymentSetting = GetPaymentSetting(paymentSettingId);
            bool isCreditCardEnabled = CreditCardEnabled(paymentSetting.PaymentTypeId, userId);
            PaymentDetailsViewModel model = new PaymentDetailsViewModel();
            string totalAmount = GetOrderTotal(paymentSetting.PaymentTypeId, userId);
            if (Equals(paymentSetting.PaymentTypeName, ZnodeConstant.PAYPAL_EXPRESS))
            {
                model.Total = totalAmount;

            }
            else if (!string.IsNullOrEmpty(paymentSetting?.GatewayCode))
            {
                totalAmount = Encryption.EncryptPaymentToken(totalAmount);
            }
            else
            {
                model.HasError = string.IsNullOrEmpty(paymentSetting?.GatewayCode);
            }
            model.GatewayCode = paymentSetting.GatewayCode;
            model.PaymentCode = paymentSetting.PaymentCode;
            model.PaymentProfileId = paymentSetting.ProfileId;
            model.Total = totalAmount;
            model.IsCreditCardEnabled = Convert.ToInt16(isCreditCardEnabled);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Get attribute validation by attribute code.
        public virtual List<PIMProductAttributeValuesViewModel> GetAttributeValidationByCodes(Dictionary<string, string> personliseValues, int productId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            PIMFamilyDetailsModel data = _attributeClient.GetAttributeValidationByCodes(new ParameterProductModel
            {
                HighLightsCodes = string.Join(",", personliseValues.Select(x => x.Key)),
                LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale)
            });
            List<PIMProductAttributeValuesViewModel> attributeValidationList = data?.Attributes?.ToViewModel<PIMProductAttributeValuesViewModel>()?.ToList();
            List<string> distinctAttributeCodes = attributeValidationList?.Where(x => x != null)?.Select(e => e.AttributeCode + e.PimAttributeFamilyId)?.Distinct().ToList();
            List<PIMProductAttributeValuesViewModel> finalAttributeList = GetAttributeControls(attributeValidationList, distinctAttributeCodes, personliseValues);
            ZnodeLogging.LogMessage("Count of: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { attributeValidationListCount = attributeValidationList, distinctAttributeCodesCount = distinctAttributeCodes, finalAttributeListCount = finalAttributeList });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return finalAttributeList;
        }

        //Update order payment status.
        public virtual OrderViewModel UpdateOrderPaymentStatus(int OmsOrderId, string paymentstatus)
         => (_orderClient.UpdateOrderPaymentStatus(OmsOrderId, paymentstatus)) ? _orderClient.GetOrderById(OmsOrderId, SetExpandsForUpdatePaymentStatus()).ToViewModel<OrderViewModel>() : null;

        //Update order payment status.
        public virtual string UpdateTrackingNumber(int orderId, string TrackingNumber)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

            RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderTrackingNumber);
            OrderHistory(orderModel, ZnodeConstant.OrderTrackingNumber, string.Empty, TrackingNumber);

            orderModel.TrackingNumber = TrackingNumber;
            SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return SetTrackingUrl(TrackingNumber, orderModel.TrackingUrl);
        }

        //Update the date in session against the orderID.
        public virtual string UpdateInHandDate(int orderId, string inHandDate)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId });
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

                RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderInHandsDate);
                OrderHistory(orderModel, ZnodeConstant.OrderInHandsDate, string.Empty, inHandDate);

                DateTime date;
                DateTime.TryParse(inHandDate, out date);
                orderModel.InHandDate = date;

                SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return inHandDate;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return Admin_Resources.UnableToUpdateDate;
            }
        }

        //Update the Job Name in session against the orderId
        public virtual string UpdateJobName(int orderId, string jobName)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent UpdateJobName method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId, JobName = jobName });
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

                RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderJobName);
                OrderHistory(orderModel, ZnodeConstant.OrderJobName, string.Empty, jobName);

                orderModel.JobName = jobName;
                SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
                ZnodeLogging.LogMessage("Agent UpdateJobName method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return jobName;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return Admin_Resources.UnableToUpdateJobName;
            }
        }

        //Update the Shipping Constraint Code in session against the orderId
        public virtual string UpdateShippingConstraintCode(int orderId, string shippingConstraintCode)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent UpdateShippingConstraintCode method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId, ShippingConstraintCode = shippingConstraintCode });
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

                RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderShippingConstraintCode);
                OrderHistory(orderModel, ZnodeConstant.OrderShippingConstraintCode, string.Empty,
                    GetEnumDescriptionValue((ShippingConstraintsEnum)Enum.Parse(typeof(ShippingConstraintsEnum), shippingConstraintCode)));

                orderModel.ShippingConstraintCode = shippingConstraintCode;
                SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
                ZnodeLogging.LogMessage("Agent UpdateShippingConstraintCode method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return shippingConstraintCode;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return Admin_Resources.UnableToUpdateShippingConstraintCode;
            }
        }

        //Update order Shipping Account Number.
        public virtual string UpdateShippingAccountNumber(int orderId, string ShippingAccountNumber)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

            RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderShippingAccountNumber);
            OrderHistory(orderModel, ZnodeConstant.OrderShippingAccountNumber, string.Empty, ShippingAccountNumber);

            orderModel.AccountNumber = ShippingAccountNumber;
            SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return ShippingAccountNumber;
        }

        //Update order Shipping Method.
        public virtual string UpdateShippingMethod(int orderId, string ShippingMethod)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId, ShippingMethod = ShippingMethod });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

            RemoveKeyFromDictionary(orderModel, ZnodeConstant.OrderShippingMethod);
            OrderHistory(orderModel, ZnodeConstant.OrderShippingMethod, string.Empty, ShippingMethod);

            orderModel.ShippingMethod = ShippingMethod;
            SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId, orderModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return ShippingMethod;
        }

        //Remove user details and cart from session.
        public virtual void RemoveUserDetailsFromSessions(int userId = 0)
        {
            RemoveInSession(AdminConstants.OMSUserAccountSessionKey);
            RemoveCartModelInSession(userId);
            RemoveAddressListSessionFrom(userId);
        }

        //get the list of Shipping list with rates
        public virtual ShippingListViewModel GetShippingListWithRates(int? userId = 0, bool excludeCustomShippingFromCreateOrder = false, bool isQuote = false)
        {
            ShoppingCartModel cartModel = GetCartModelFromSession(userId, isQuote);
            if(IsNotNull(cartModel))
                cartModel.IsCalculatePromotionAndCoupon = isQuote ? false : true;
            return GetShippingListAndRates(cartModel, excludeCustomShippingFromCreateOrder);
        }

        //get the list of Shipping list with rates for manage
        public virtual ShippingListViewModel GetShippingListForManage(int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);
            return GetShippingListAndRates(orderModel?.ShoppingCartModel);
        }

        //This method will use to call the payment and process the order
        public virtual SubmitOrderViewModel SubmitEditOrderpayment(SubmitPaymentViewModel submitPaymentViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + submitPaymentViewModel.OmsOrderId);

                if (IsNull(orderModel))
                    return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), "unable to process order");

                RemoveInvalidDiscountCode(orderModel.ShoppingCartModel);
                SetUsersPaymentDetails(submitPaymentViewModel.PaymentSettingId, orderModel.ShoppingCartModel, false);
                orderModel.ShoppingCartModel.Payment.BillingAddress = orderModel.BillingAddress;
                orderModel.ShoppingCartModel.Payment.ShippingAddress = orderModel.ShippingAddress;
                orderModel.ShoppingCartModel.OrderNumber = orderModel.OrderNumber;
                //Map shopping Cart model and submit Payment view model to Submit payment model
                SubmitPaymentModel model = PaymentViewModelMap.ToModel(orderModel.ShoppingCartModel, submitPaymentViewModel);

                //Save Customer Payment Guid for Save Credit Card
                SaveCustomerPaymentGuid(orderModel.UserId, submitPaymentViewModel.CustomerGuid, orderModel.CustomerPaymentGUID);

                GatewayResponseModel gatewayResponse = _paymentAgent.ProcessPayNow(model);

                if (HelperUtility.IsNull(submitPaymentViewModel.CustomerGuid) && HelperUtility.IsNull(orderModel.CustomerPaymentGUID))
                {
                    SaveCustomerPaymentGuid(submitPaymentViewModel.UserId, gatewayResponse.CustomerGUID, orderModel.CustomerPaymentGUID);
                }

                if (gatewayResponse?.HasError ?? true || string.IsNullOrEmpty(gatewayResponse?.Token))
                    return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? gatewayResponse.ErrorMessage : Admin_Resources.ErrorProcessPayment);

                //Map payment token
                orderModel.ShoppingCartModel.Token = gatewayResponse.Token;

                //Map additional information
                if (!string.IsNullOrEmpty(submitPaymentViewModel.AdditionalInfo))
                    orderModel.AdditionalInstructions = submitPaymentViewModel.AdditionalInfo;

                orderModel.ShoppingCartModel.CreditCardNumber = submitPaymentViewModel?.CreditCardNumber;
                orderModel.ShoppingCartModel.CardType = submitPaymentViewModel?.CardType;
                orderModel.ShoppingCartModel.CreditCardExpMonth = submitPaymentViewModel?.CreditCardExpMonth;
                orderModel.ShoppingCartModel.CreditCardExpYear = submitPaymentViewModel?.CreditCardExpYear;
                orderModel.ShoppingCartModel.UserId = orderModel.UserId;
                orderModel.ShoppingCartModel.OrderDate = DateTime.Now;
                orderModel.ShoppingCartModel.TransactionId = gatewayResponse.Token;
                orderModel.ShoppingCartModel.TransactionDate = gatewayResponse.TransactionDate;
                orderModel.ShoppingCartModel.Payment.PaymentStatusId = (int)gatewayResponse.PaymentStatus;
                SetUserInformation(orderModel.ShoppingCartModel);

                //Void or Refund transactions               

                string errorMessage = string.Empty;
                BooleanModel booleanModel = CapturePaymentAndCreateHistory(gatewayResponse, orderModel, model);

                VoidRefundPayment(orderModel,true,out errorMessage);

                OrderModel order = _orderClient.UpdateOrder(orderModel);

                if (!gatewayResponse?.IsGatewayPreAuthorize ?? true)
                    UpdateCapturedPaymentDetails(orderModel.OmsOrderId, gatewayResponse.Token, false, booleanModel, out errorMessage);

                return new SubmitOrderViewModel { OrderId = order.OmsOrderId, ReceiptHtml = !IsNull(order.ReceiptHtml) ? WebUtility.HtmlEncode(order.ReceiptHtml) : order.ReceiptHtml, IsEmailSend = order.IsEmailSend };
            }
            catch (ZnodeException exception)
            {
                ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                switch (exception.ErrorCode)
                {
                    case ErrorCodes.MinAndMaxSelectedQuantityError:
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), exception.ErrorMessage);

                    case ErrorCodes.OutOfStockException:
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel() { }, Admin_Resources.OutOfStockException);

                    case ErrorCodes.PaymentCaptureError:
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel() { }, exception.ErrorMessage);

                    default:
                        return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), Admin_Resources.ErrorUnablePlaceOrder);
                }
            }
            catch (Exception exception)
            {
                ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (SubmitOrderViewModel)GetViewModelWithErrorMessage(new SubmitOrderViewModel(), Admin_Resources.ErrorUnablePlaceOrder);
            }
        }

        //To capture payment and create order history.
        protected BooleanModel CapturePaymentAndCreateHistory(GatewayResponseModel gatewayResponse, OrderModel orderModel, SubmitPaymentModel model)
        {
            BooleanModel booleanModel = new BooleanModel();
            if (!gatewayResponse?.IsGatewayPreAuthorize ?? true)
            {
                booleanModel = CapturePayment(gatewayResponse.Token);
                bool isPaymentCaptured = IsNotNull(booleanModel) && !booleanModel.HasError;
                string message = isPaymentCaptured ? Admin_Resources.PaymentCaptureSuccessMessage : Admin_Resources.PaymentCaptureErrorMessage;
                _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = gatewayResponse.Token, OrderAmount = Convert.ToDecimal(model.Total) });
                if (!isPaymentCaptured)
                    throw new ZnodeException(ErrorCodes.PaymentCaptureError, Admin_Resources.PaymentCaptureErrorMessage);
            }

            return booleanModel;
        }

        //Set AllowBackOrder and TrackInventory.
        public static void TrackInventoryData(ref bool AllowBackOrder, ref bool TrackInventory, string inventorySetting)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            switch (inventorySetting)
            {
                case ZnodeConstant.DisablePurchasing:
                    AllowBackOrder = false;
                    TrackInventory = true;
                    break;

                case ZnodeConstant.AllowBackOrdering:
                    AllowBackOrder = true;
                    TrackInventory = true;
                    break;

                case ZnodeConstant.DontTrackInventory:
                    AllowBackOrder = false;
                    TrackInventory = false;
                    break;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Tracking Url by OrderId
        public virtual void SetTrackingUrlByOrderId(int omsOrderId, List<CartItemViewModel> cartItemViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = omsOrderId });
            //Get Cart from session.
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId);
            if (IsNotNull(orderModel))
                cartItemViewModel?.ForEach(x => x.TrackingUrl = orderModel?.TrackingUrl);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        public virtual bool CheckForGuestUser(int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId });
            UserModel userAccountData = _userClient.GetUserAccountData(userId);
            if (HelperUtility.IsNotNull(userAccountData))
                return string.IsNullOrEmpty(userAccountData.AspNetUserId) ? true : false;
            else
                return false;
        }

        public virtual bool SendPOEmail(SendInvoiceViewModel invoiceModel)
            => _orderClient.SendPOEmail(invoiceModel.ToModel<SendInvoiceModel>());

        //check capture is disable return true if disable else false 
        public virtual bool CaptureDisable(int paymentSettingId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            PaymentSettingViewModel paymentSetting = _paymentAgent.GetPaymentSetting(paymentSettingId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return paymentSetting?.IsCaptureDisable ?? false;
        }

        //to get payment api header
        public virtual AjaxHeadersModel GetPaymentAPIHeader()
        {
            var _header = GetClient<MediaManagerClient>();
            return new AjaxHeadersModel { Authorization = _header.GetAuthorizationHeader(string.Empty, string.Empty, ZnodeAdminSettings.PaymentApplicationUrl) };
        }
        #region Quick Order

        public virtual List<AutoComplete> GetProductListBySKU(string sku)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(sku))
            {
                FilterCollection filters = new FilterCollection();

                ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(AdminConstants.CartModelSessionKey);
                if (IsNotNull(cartModel))
                    filters = SetQuickOrderListFilter(sku, cartModel.PublishedCatalogId, cartModel.PortalId, cartModel.UserId.GetValueOrDefault(), cartModel.LocaleId);

                ExpandCollection expands = new ExpandCollection();

                //Set Filters and Expands
                SetFiltersAndExpands(ref filters, expands);

                //Get published product list.
                PublishProductListModel productListModel = _publishProductClient.GetPublishProductList(expands, filters, null, 0, 0);

                List<AutoComplete> _autoComplete = new List<AutoComplete>();

                if ((productListModel?.PublishProducts?.Count > 0))
                {
                    List<PublishProductsViewModel> products = productListModel.PublishProducts?.ToViewModel<PublishProductsViewModel>()?.ToList();
                    //Assign AutoCompleteLabel and Cart quantity to each product
                    products.ForEach(item =>
                    {
                        AutoComplete _item = new AutoComplete();

                        _item.Name = HttpUtility.HtmlDecode(string.Format(WebStore_Resources.AutoCompleteLabelQuickOrder,
                        item.Name, item.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ShortDescription)?.AttributeValues, item.SKU, string.Empty,
                        GetInventoryMessage(item), item.ImageSmallThumbnailPath));

                        bool? isCallForPricing = false;
                        if (!Convert.ToBoolean(item.Attributes.Where(x => x.AttributeCode == ZnodeConstant.CallForPricing)?.FirstOrDefault()?.AttributeValues))
                            isCallForPricing = item?.Promotions.Any(x => x.PromotionType?.Replace(" ", "") == ZnodeConstant.CallForPricing);
                        else
                            isCallForPricing = Convert.ToBoolean(item.Attributes.Where(x => x.AttributeCode == ZnodeConstant.CallForPricing)?.FirstOrDefault()?.AttributeValues);

                        _item.Id = item.PublishProductId;
                        _item.DisplayText = item.SKU;
                        _item.Properties.Add("CartQuantity", GetOrderedItemQuantity(item.SKU));
                        _item.Properties.Add("ProductName", item.Name);
                        _item.Properties.Add("Quantity", item.Quantity);
                        _item.Properties.Add("ProductType", item.Attributes.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Value);
                        _item.Properties.Add("CallForPricing", isCallForPricing);
                        _item.Properties.Add("TrackInventory", item.Attributes.Where(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.FirstOrDefault()?.AttributeValues);
                        _item.Properties.Add("OutOfStockMessage", string.IsNullOrEmpty(item.OutOfStockMessage) ? WebStore_Resources.TextOutofStock : item.OutOfStockMessage);
                        _item.Properties.Add("MaxQuantity", item.Attributes.Where(x => x.AttributeCode == ZnodeConstant.MaximumQuantity)?.FirstOrDefault()?.AttributeValues);
                        _item.Properties.Add("MinQuantity", item.Attributes.Where(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.FirstOrDefault()?.AttributeValues);
                        _item.Properties.Add("RetailPrice", item.RetailPrice);
                        _item.Properties.Add("ImagePath", item.ImageSmallPath);
                        _item.Properties.Add("IsPersonalisable", item.Attributes.Where(x => x.IsPersonalizable == true).Select(x => x.IsPersonalizable).FirstOrDefault());
                        _autoComplete.Add(_item);
                    });
                }
                ZnodeLogging.LogMessage("AutoComplete list count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AutoCompleteListCount = _autoComplete.Count });
                return _autoComplete;
            }
            else
                return new List<AutoComplete>();
        }

        public virtual bool GetOrderStateValueById(int omsOrderStateId)
            => _orderClient.GetOrderStateValueById(omsOrderStateId).IsEdit;

        #endregion Quick Order

        //To get payment options
        public virtual PaymentSettingViewModel GetPaymentMethods(int portalId, int userId)
        {
            //Payment option list
            PaymentSettingViewModel paymentSettingViewModel = new PaymentSettingViewModel() { PaymentTypeList = BindPaymentList(userId, portalId) };

            return paymentSettingViewModel;
        }

        //Reorder complete order
        public virtual CreateOrderViewModel ReorderCompleteOrder(int userId, int portalId, int orderId)
        {
            CreateOrderViewModel createOrderViewModel = null;
            bool response = false;

            if (orderId > 0)
            {
                response = _orderClient.ReorderCompleteOrder(orderId, portalId, userId);
            }

            createOrderViewModel = GetCreateOrderDetails(portalId);

            if (response)
            {               
                _cartAgent.RemoveCartSession(userId);
            }

            return createOrderViewModel;
        }


        //Get User Details by user id.
        public virtual UserModel GetUserDetailById(int userId, int portalId)
        {
            ZnodeLogging.LogMessage("UserId to get user data:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId });
            return _userClient.GetUserDetailById(userId, portalId);
        }

        //Get payment history for manage.
        public virtual PaymentHistoryListViewModel GetPaymentHistory(int orderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = orderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + orderId);

            PaymentHistoryListViewModel listViewModel = new PaymentHistoryListViewModel { List = orderModel.PaymentHistoryList?.PaymentHistoryList?.ToViewModel<PaymentHistoryViewModel>().ToList() };
            SetAmountWithCurrency(orderModel.CultureCode, listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listViewModel;
        }
        #endregion Order

        #region Quote

        //Create quote for customer.
        public virtual CreateOrderViewModel CreateQuoteForCustomer(int userId, int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId, AccountId = accountId });
            if (userId > 0)
            {
                //Get the user details.
                CreateOrderViewModel createQuote = GetUserFullDetails(new CartParameterModel { IsQuote = true, UserId = userId });

                if (HelperUtility.IsNotNull(createQuote))
                {
                    //Map the fields.
                    createQuote.StoreName = createQuote.PortalList?.Where(x => x.Value == createQuote.UserAddressDataViewModel?.PortalId.ToString())?.FirstOrDefault()?.Text;
                    createQuote.CustomerName = createQuote.UserAddressDataViewModel?.FullName;
                    createQuote.AccountId = accountId;
                    createQuote.UserId = userId;
                    return createQuote;
                }
            }
            return new CreateOrderViewModel { HasError = true, AccountId = accountId, UserId = userId };
        }

        //Get all details of customer.
        public virtual UserAddressDataViewModel GetCustomerDetailsForQuote(CartParameterModel cartParameter, List<SelectListItem> portalList = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalList count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalListCount = portalList?.Count });
            UserAddressDataViewModel userAddressDataViewModel = new UserAddressDataViewModel();
            //Get parent Account Details of user.
            GetAccountdetails(cartParameter.UserId.GetValueOrDefault(), userAddressDataViewModel);

            if (DefaultSettingHelper.AllowGlobalLevelUserCreation)
                userAddressDataViewModel.PortalId = Convert.ToInt32(portalList?.FirstOrDefault()?.Value);

            //Get Portal Catalog By Portal Id.
            GetPortalCatalogByPortalId(userAddressDataViewModel.PortalId, userAddressDataViewModel);

            //Set shipping and billing address of user.
            GetUserAddress(userAddressDataViewModel);

            //Set the fields.
            cartParameter.PortalId = userAddressDataViewModel.PortalId;
            cartParameter.PublishedCatalogId = userAddressDataViewModel.PortalCatalogId;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddressDataViewModel;
        }

        #endregion Quote

        #endregion Public Methods

        #region Protected  Methods

        /// <summary>
        /// Get the product SKU according to the product type.
        /// </summary>
        /// <remarks>
        /// 1. This method is used for maintaining the line item history session.
        /// 2. This method is also used in CartAgent.cs
        /// </remarks>
        /// <param name="cartItem">Shopping cart item</param>
        /// <returns>SKU</returns>
        internal static string GetProductSKU(ShoppingCartItemModel cartItem)
        {
            if (cartItem.GroupProducts.Any())
            {
                if (!string.IsNullOrEmpty(cartItem.AddOnProductSKUs))
                    return (!string.IsNullOrEmpty(cartItem.GroupId)) ? $"{cartItem.SKU}_{cartItem.GroupProducts.FirstOrDefault()?.Sku}_{cartItem.AddOnProductSKUs}_{cartItem.GroupId}" : $"{cartItem.SKU}_{cartItem.GroupProducts.FirstOrDefault()?.Sku}_{cartItem.AddOnProductSKUs}";
                else if (!string.IsNullOrEmpty(cartItem.AutoAddonSKUs))
                    return (!string.IsNullOrEmpty(cartItem.GroupId)) ? $"{cartItem.SKU}_{cartItem.GroupProducts.FirstOrDefault()?.Sku}_{cartItem.AutoAddonSKUs}_{cartItem.GroupId}" : $"{cartItem.SKU}_{cartItem.GroupProducts.FirstOrDefault()?.Sku}_{cartItem.AutoAddonSKUs}";
                else
                    return (!string.IsNullOrEmpty(cartItem.GroupId)) ? $"{cartItem.SKU}_{cartItem.GroupProducts.FirstOrDefault()?.Sku}_{cartItem.GroupId}" : $"{cartItem.SKU}_{cartItem.GroupProducts.FirstOrDefault()?.Sku}";
            }
            if (!string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs))
                return (!string.IsNullOrEmpty(cartItem.GroupId)) ? $"{cartItem.SKU}_{cartItem.ConfigurableProductSKUs}_{cartItem.GroupId}" : $"{cartItem.SKU}_{cartItem.ConfigurableProductSKUs}";
            if (!string.IsNullOrEmpty(cartItem.AddOnProductSKUs))
                return (!string.IsNullOrEmpty(cartItem.GroupId)) ? $"{cartItem.SKU}_{cartItem.AddOnProductSKUs}_{cartItem.GroupId}" : $"{cartItem.SKU}_{cartItem.AddOnProductSKUs}";

            if (!string.IsNullOrEmpty(cartItem.AutoAddonSKUs))
                return (!string.IsNullOrEmpty(cartItem.GroupId)) ? $"{cartItem.SKU}_{cartItem.AutoAddonSKUs}_{cartItem.GroupId}" : $"{cartItem.SKU}_{cartItem.AutoAddonSKUs}";

            return (!string.IsNullOrEmpty(cartItem.GroupId)) ? $"{cartItem.SKU}_{cartItem.GroupId}" : $"{cartItem.SKU}";
        }

        /// <summary>
        /// Get the <see cref="OrderLineItemHistoryModel"/> object for a cart item.
        /// </summary>
        /// <remarks>
        /// 1. This method is used for maintaining the line item history session.
        /// 2. This method is also used in CartAgent.cs
        /// </remarks>
        /// <param name="cartItem">Shopping cart item for which the history is to be maintained.</param>
        /// <returns></returns>
        internal static Tuple<string, string> GetItemHistory(ShoppingCartItemModel cartItem)
        {
            if (cartItem.GroupProducts.Any())
                return Tuple.Create(GetProductSKU(cartItem), $"{cartItem.GroupProducts.FirstOrDefault()?.Quantity}");

            return Tuple.Create(GetProductSKU(cartItem), $"{cartItem.Quantity}");
        }

        protected virtual void SaveLineItemHistorySession(List<ShoppingCartItemModel> ShoppingCartItems)
        {
            if (ShoppingCartItems.Any())
            {
                ZnodeLogging.LogMessage("ShoppingCartItems count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShoppingCartItemsCount = ShoppingCartItems?.Count });
                IDictionary<string, Tuple<string, string>> lineItemHistory = new Dictionary<string, Tuple<string, string>>();
                foreach (var item in ShoppingCartItems)
                {
                    if (lineItemHistory.Any(x => x.Key != item.SKU))
                        lineItemHistory.Add(GetProductSKU(item), GetItemHistory(item));
                }

                SaveInSession(AdminConstants.LineItemHistorySession, lineItemHistory);
            }
        }

        //Get quantity of ordered line item.
        protected virtual decimal GetOrderLineItemQuantity(OrderModel orderModel, ShoppingCartItemModel cartItemModel)
        {
            return (cartItemModel.GroupProducts?.Count > 0 ?
                    (orderModel.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartItemModel.OmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId == 4)?.Quantity).GetValueOrDefault()
                    : !string.IsNullOrEmpty(cartItemModel.ConfigurableProductSKUs) ? (orderModel.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartItemModel.OmsOrderLineItemsId)?.Quantity).GetValueOrDefault() :
                    (orderModel.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartItemModel.OmsOrderLineItemsId)?.Quantity).GetValueOrDefault());
        }

        protected virtual ReturnOrderLineItemModel GetReturnLineItem(ShoppingCartItemModel cartItemModel, ManageOrderDataModel orderDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(cartItemModel))
            {
                ReturnOrderLineItemModel returnLineItem = new ReturnOrderLineItemModel();
                returnLineItem.Description = cartItemModel.Description;
                returnLineItem.ExtendedPrice = orderDataModel?.CustomQuantity > 0 ? cartItemModel.UnitPrice * orderDataModel.CustomQuantity : cartItemModel.ExtendedPrice;
                returnLineItem.ProductId = cartItemModel.ProductId;
                returnLineItem.Quantity = orderDataModel?.CustomQuantity > 0 ? orderDataModel.CustomQuantity : cartItemModel.Quantity;
                returnLineItem.ShippingCost = cartItemModel.ShippingCost / orderDataModel?.Quantity * returnLineItem.Quantity ?? 0;
                returnLineItem.ProductDiscountAmount = cartItemModel.ProductDiscountAmount;
                returnLineItem.ShippingOptionId = cartItemModel.ShippingOptionId;
                returnLineItem.SKU = cartItemModel.SKU;
                returnLineItem.UnitPrice = cartItemModel.UnitPrice;
                returnLineItem.ExternalId = cartItemModel.ExternalId;
                returnLineItem.CartDescription = cartItemModel.CartDescription;
                returnLineItem.CurrencyCode = cartItemModel.CurrencyCode;
                returnLineItem.ImagePath = cartItemModel.ImagePath;
                returnLineItem.MediaConfigurationId = cartItemModel.MediaConfigurationId;
                returnLineItem.ProductName = cartItemModel.ProductName;
                returnLineItem.ProductType = cartItemModel.ProductType;
                returnLineItem.ImageMediumPath = cartItemModel.ImageMediumPath;
                returnLineItem.AddOnProductSKUs = cartItemModel.AddOnProductSKUs;
                returnLineItem.BundleProductSKUs = cartItemModel.BundleProductSKUs;
                returnLineItem.ConfigurableProductSKUs = cartItemModel.ConfigurableProductSKUs;
                returnLineItem.ProductCode = cartItemModel.ProductCode;
                returnLineItem.TrackingNumber = cartItemModel.TrackingNumber;
                returnLineItem.UOM = cartItemModel.UOM;
                returnLineItem.ChildProductId = cartItemModel.ChildProductId;
                returnLineItem.IsEditStatus = cartItemModel.IsEditStatus;
                returnLineItem.ShipSeperately = cartItemModel.ShipSeperately;
                returnLineItem.OmsOrderStatusId = cartItemModel.OmsOrderStatusId;
                returnLineItem.OrderLineItemStatus = !string.IsNullOrEmpty(orderDataModel.OrderLineItemStatus) ? orderDataModel.OrderLineItemStatus : cartItemModel.OrderLineItemStatus;
                returnLineItem.CustomText = cartItemModel.CustomText;
                returnLineItem.TaxCost = cartItemModel.TaxCost;
                returnLineItem.ImportDuty = cartItemModel.ImportDuty;
                returnLineItem.PersonaliseValuesList = cartItemModel.PersonaliseValuesList;
                returnLineItem.PersonaliseValuesDetail = cartItemModel.PersonaliseValuesDetail;
                returnLineItem.ReasonForReturn = orderDataModel.ReasonForReturn;
                returnLineItem.ReasonForReturnId = orderDataModel.ReasonForReturnId;
                returnLineItem.GroupProducts = GetCustomGroupProductItems(cartItemModel, orderDataModel);
                returnLineItem.OmsOrderLineItemsId = cartItemModel.OmsOrderLineItemsId;
                returnLineItem.AddOnLineItemId = cartItemModel.AddOnLineItemId;
                returnLineItem.Custom1 = cartItemModel.Custom1;
                returnLineItem.Custom2 = cartItemModel.Custom2;
                returnLineItem.Custom3 = cartItemModel.Custom3;
                returnLineItem.Custom4 = cartItemModel.Custom4;
                returnLineItem.Custom5 = cartItemModel.Custom5;
                returnLineItem.IsShippingReturn = cartItemModel.IsShippingReturn;
                returnLineItem.PartialRefundAmount = cartItemModel.PartialRefundAmount;
                returnLineItem.AdditionalCost = cartItemModel.AdditionalCost;
                returnLineItem.PaymentStatusId = cartItemModel.PaymentStatusId.GetValueOrDefault();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return returnLineItem;
            }
            return null;
        }

        //Bind dropdown list for all shoppingcart items.
        protected virtual CartViewModel GetCartOrderStatusList(ShoppingCartModel cartModel, string trackingUrl = "")
        {
            if (cartModel?.ShoppingCartItems?.Count > 0)
            {
                CartViewModel cartViewModel = cartModel?.ToViewModel<CartViewModel>();
                cartViewModel.ShoppingCartItems.ForEach(x => x.TrackingUrl = trackingUrl);

                List<SelectListItem> orderStatusList = BindManageOrderStatus(new FilterTuple(ZnodeOmsOrderStateEnum.IsOrderLineItemState.ToString().ToLower(), FilterOperators.Equals, "true"));
                ZnodeLogging.LogMessage("OrderStatusList count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderStatusListCount = orderStatusList.Count });
                cartViewModel?.ShoppingCartItems?.ForEach(x => x.ShippingStatusList = orderStatusList);

                return cartViewModel;
            }
            return new CartViewModel() { UserId = cartModel.UserId.GetValueOrDefault(), LocaleId = cartModel.LocaleId };
        }

        protected virtual void GetOrderLineItemStatus(ShoppingCartModel cart, ManageOrderDataModel orderDataModel)
        {
            if (orderDataModel.OrderLineItemStatusId > 0)
            {
                if(string.Equals(orderDataModel?.OrderLineItemStatus, AdminConstants.PendingApproval, StringComparison.InvariantCultureIgnoreCase))
                {
                    cart?.ShoppingCartItems?.Where(x => x.ExternalId == orderDataModel.Guid)?.Select(
                        y =>
                        {
                            y.TrackingNumber = orderDataModel.TrackingNumber;
                            return y;
                        })?.FirstOrDefault();
                }
                else
                {
                    FilterCollection filters = new FilterCollection();
                    filters.Add(new FilterTuple(ZnodeOmsOrderStateEnum.IsAccountStatus.ToString().ToLower(), FilterOperators.Equals, "false"));
                    ZnodeLogging.LogMessage("Filters to get order states: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
                    OrderStateModel orderState = _orderStateClient.GetOrderStates(null, filters, null, null, null)?.OrderStates?.FirstOrDefault(x => x.OrderStateId == orderDataModel.OrderLineItemStatusId);

                    cart?.ShoppingCartItems?.Where(x => x.ExternalId == orderDataModel.Guid)?.Select(
                        y =>
                        {
                            y.TrackingNumber = orderDataModel.TrackingNumber;
                            y.OrderLineItemStatus = orderState?.OrderStateName;
                            y.IsEditStatus = (orderState?.IsEdit).GetValueOrDefault(true);
                            y.OmsOrderStatusId = (orderState?.OrderStateId).GetValueOrDefault();
                            return y;
                        })?.FirstOrDefault();
                }
            }
        }

        //Update custom data for cart items.
        protected virtual void GetUpdatedOrderLineItem(ShoppingCartModel cartModel, ShoppingCartItemModel shoppingCartItem, ManageOrderDataModel orderDataModel, OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string sku = string.Empty;
            OrderLineItemHistoryModel orderLineItemHistoryModel = new OrderLineItemHistoryModel();
            //assign custom unit price, shipping cost for line item.
            if (IsNotNull(shoppingCartItem))
            {
                sku = orderDataModel.ProductId > 0 ? shoppingCartItem.ProductName + "-" + Convert.ToString(shoppingCartItem.GroupProducts?.Where(y => y.ProductId == orderDataModel.ProductId).Select(s => s.Sku).FirstOrDefault()) : sku = shoppingCartItem.SKU;

                //Set sku of the actual product irrespective of any product type
                orderLineItemHistoryModel.SKU = (orderDataModel.ProductId > 0)
                                    ? shoppingCartItem.GroupProducts?.FirstOrDefault(y => y.ProductId == orderDataModel.ProductId)?.Sku
                                    : !string.IsNullOrEmpty(shoppingCartItem.ConfigurableProductSKUs) ? shoppingCartItem.ConfigurableProductSKUs : shoppingCartItem.SKU;

                ZnodeLogging.LogMessage("SKU of the actual product: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SKU = orderLineItemHistoryModel.SKU });

                RemoveKeyFromDictionary(orderModel, sku, true);

                if (orderDataModel.ProductId > 0)
                {
                    if (!Equals(shoppingCartItem?.GroupProducts?.FirstOrDefault().Quantity, orderDataModel.Quantity))
                        orderLineItemHistoryModel.OrderLineQuantity = orderDataModel.Quantity.ToInventoryRoundOff();
                    shoppingCartItem.GroupProducts?.Where(y => y.ProductId == orderDataModel.ProductId)?.Select(z => { z.Quantity = orderDataModel.Quantity; return z; })?.FirstOrDefault();
                }
                else
                {
                    if (!Equals(shoppingCartItem.Quantity, orderDataModel.Quantity))
                        orderLineItemHistoryModel.OrderLineQuantity = orderDataModel.Quantity.ToInventoryRoundOff();
                }

                shoppingCartItem.CustomUnitPrice = orderDataModel.UnitPrice;
                if (IsNotNull(orderDataModel.UnitPrice))
                {
                    if (!Equals(shoppingCartItem.UnitPrice, orderDataModel.UnitPrice))
                        orderLineItemHistoryModel.OrderLineUnitPrice = HelperMethods.FormatPriceWithCurrency(Convert.ToDecimal(orderDataModel.UnitPrice), orderModel.CultureCode);

                    shoppingCartItem.ExtendedPrice = orderDataModel.UnitPrice.GetValueOrDefault() * orderDataModel.Quantity;
                }

                if (orderDataModel.OrderLineItemStatusId > 0 && !Equals(shoppingCartItem.OmsOrderStatusId, orderDataModel.OrderLineItemStatusId) && !string.IsNullOrEmpty(orderDataModel.OrderLineItemStatus))
                {
                    orderLineItemHistoryModel.OrderUpdatedStatus = orderDataModel.OrderLineItemStatus;
                    shoppingCartItem.IsItemStateChanged = true;
                }

                if (!Equals(orderDataModel.TrackingNumber, shoppingCartItem.TrackingNumber))
                    orderLineItemHistoryModel.OrderTrackingNumber = orderDataModel.TrackingNumber;

                if (HelperUtility.IsNotNull(orderDataModel.PartialRefundAmount))
                {
                    decimal? refundamount = HelperUtility.IsNotNull(orderDataModel.PartialRefundAmount) ? orderDataModel.PartialRefundAmount : 0;
                    refundamount += HelperUtility.IsNotNull(shoppingCartItem.PartialRefundAmount) ? shoppingCartItem.PartialRefundAmount : 0;
                    if (shoppingCartItem.ExtendedPrice < refundamount)
                        refundamount = shoppingCartItem.ExtendedPrice;

                    orderDataModel.PartialRefundAmount = refundamount;
                    shoppingCartItem.PartialRefundAmount = refundamount;
                    orderLineItemHistoryModel.PartialRefundAmount = HelperMethods.FormatPriceWithCurrency(orderDataModel.PartialRefundAmount ?? 0, orderModel.CultureCode);
                }

                if (IsNotNull(orderLineItemHistoryModel))
                {
                    orderLineItemHistoryModel.ProductName = shoppingCartItem.ProductName;
                    orderLineItemHistoryModel.Quantity = orderDataModel.CustomQuantity > 0 ? orderDataModel.CustomQuantity.ToInventoryRoundOff() : (orderDataModel.Quantity > shoppingCartItem.Quantity || orderDataModel.Quantity < shoppingCartItem.Quantity) ? orderDataModel.Quantity.ToInventoryRoundOff() : shoppingCartItem.Quantity.ToInventoryRoundOff();
                    orderLineItemHistoryModel.OmsOrderLineItemsId = shoppingCartItem.OmsOrderLineItemsId;
                    orderLineItemHistoryModel.TaxCost = shoppingCartItem.TaxCost / orderDataModel?.Quantity * Convert.ToDecimal(orderLineItemHistoryModel.Quantity) ?? 0;
                    orderLineItemHistoryModel.ImportDuty = shoppingCartItem.ImportDuty / orderDataModel?.Quantity * Convert.ToDecimal(orderLineItemHistoryModel.Quantity) ?? 0;
                    orderLineItemHistoryModel.SubTotal = Convert.ToDecimal(orderLineItemHistoryModel.Quantity) * shoppingCartItem.UnitPrice;
                    orderLineItemHistoryModel.DiscountAmount = Convert.ToDecimal(shoppingCartItem.DiscountAmount) / orderDataModel?.Quantity * Convert.ToDecimal(orderLineItemHistoryModel.Quantity) ?? 0;
                    OrderLineHistory(orderModel, sku, orderLineItemHistoryModel);
                }

                if ((!Equals(orderDataModel.TrackingNumber, shoppingCartItem.TrackingNumber) || (orderDataModel.OrderLineItemStatusId > 0 && orderDataModel.OrderLineItemStatusId != shoppingCartItem.OmsOrderStatusId)) && (orderDataModel.CustomQuantity > 0 && orderDataModel.Quantity > orderDataModel.CustomQuantity))
                {
                    ShoppingCartItemModel customCartItem = GetCustomCartItem(shoppingCartItem, orderDataModel, cartModel.TaxCost);
                    customCartItem.GroupProducts = GetCustomGroupProductItems(shoppingCartItem, orderDataModel);

                    orderDataModel.Guid = customCartItem.ExternalId;

                    cartModel.ShoppingCartItems.Insert(cartModel.ShoppingCartItems.Count, customCartItem);

                    if (shoppingCartItem.GroupProducts?.Count > 0)
                        shoppingCartItem.GroupProducts?.ForEach(x => x.Quantity = orderDataModel.Quantity - orderDataModel.CustomQuantity);
                    else
                        shoppingCartItem.Quantity = orderDataModel.Quantity - orderDataModel.CustomQuantity;
                }
                else
                {
                    shoppingCartItem.TrackingNumber = orderDataModel.TrackingNumber;
                    shoppingCartItem.Quantity = orderDataModel.Quantity;
                    shoppingCartItem.OmsOrderStatusId = orderDataModel.OrderLineItemStatusId > 0 ? orderDataModel.OrderLineItemStatusId : shoppingCartItem.OmsOrderStatusId;
                    shoppingCartItem.OrderLineItemStatus = !string.IsNullOrEmpty(orderDataModel.OrderLineItemStatus) && orderDataModel.OrderLineItemStatusId > 0 ? orderDataModel.OrderLineItemStatus : shoppingCartItem.OrderLineItemStatus;
                    shoppingCartItem.ExtendedPrice = IsNotNull(orderDataModel.UnitPrice) ? orderDataModel.UnitPrice.GetValueOrDefault() * orderDataModel.Quantity : shoppingCartItem.ExtendedPrice;
                    shoppingCartItem.IsShippingReturn = orderDataModel.IsShippingReturn;
                    shoppingCartItem.PartialRefundAmount = IsNotNull(orderDataModel.PartialRefundAmount) ? orderDataModel.PartialRefundAmount : shoppingCartItem.PartialRefundAmount;
                }
                cartModel.ShoppingCartItems.Insert(cartModel.ShoppingCartItems.FindIndex(x => x.ExternalId == shoppingCartItem.ExternalId), shoppingCartItem);

                if (orderModel.ReturnItemList?.ReturnItemList?.Count > 0)
                    cartModel.ShoppingCartItems.Remove(cartModel.ShoppingCartItems.LastOrDefault(x => x.ExternalId == shoppingCartItem.ExternalId));
                else
                    cartModel.ShoppingCartItems.Remove(cartModel.ShoppingCartItems.LastOrDefault(x => x.ExternalId == shoppingCartItem.ExternalId));

                ZnodeLogging.LogMessage("ShoppingCartItem model with CustomUnitPrice, OmsOrderStatusId and ExternalId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CustomUnitPrice = shoppingCartItem.CustomUnitPrice, OmsOrderStatusId = shoppingCartItem.OmsOrderStatusId, ExternalId = shoppingCartItem.ExternalId });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get the list of Group product for custom cart item.
        protected virtual List<AssociatedProductModel> GetCustomGroupProductItems(ShoppingCartItemModel shoppingCartItem, ManageOrderDataModel orderDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("ShoppingCartItemModel with GroupProducts count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { GroupProductsCount = shoppingCartItem?.GroupProducts?.Count });
            List<AssociatedProductModel> groupProducts = new List<AssociatedProductModel>();
            if (shoppingCartItem?.GroupProducts?.Count > 0)
            {
                foreach (var groupProduct in shoppingCartItem.GroupProducts)
                {
                    AssociatedProductModel product = new AssociatedProductModel();
                    product.Sequence = groupProduct.Sequence;
                    product.ProductId = groupProduct.ProductId;
                    product.OmsQuoteId = groupProduct.OmsQuoteId;
                    product.OmsTemplateId = groupProduct.OmsTemplateId;
                    product.OmsQuoteLineItemId = groupProduct.OmsQuoteLineItemId;
                    product.OmsTemplateLineItemId = groupProduct.OmsTemplateLineItemId;
                    product.ParentOmsQuoteLineItemId = groupProduct.ParentOmsQuoteLineItemId;
                    product.ParentOmsTemplateLineItemId = groupProduct.ParentOmsTemplateLineItemId;
                    product.OrderLineItemRelationshipTypeId = groupProduct.OrderLineItemRelationshipTypeId;
                    product.Quantity = orderDataModel?.CustomQuantity > 0 ? orderDataModel.CustomQuantity : groupProduct.Quantity;
                    product.UnitPrice = groupProduct.UnitPrice;
                    product.MinimumQuantity = groupProduct.MinimumQuantity;
                    product.MaximumQuantity = groupProduct.MaximumQuantity;
                    product.Sku = groupProduct.Sku;
                    product.ExternalId = groupProduct.ExternalId;
                    product.CustomText = groupProduct.CustomText;
                    product.ProductName = groupProduct.ProductName;
                    product.CurrencyCode = groupProduct.CurrencyCode;
                    product.InStockMessage = groupProduct.InStockMessage;
                    product.CartAddOnDetails = groupProduct.CartAddOnDetails;
                    product.BackOrderMessage = groupProduct.BackOrderMessage;
                    product.InventoryMessage = groupProduct.InventoryMessage;
                    product.OutOfStockMessage = groupProduct.OutOfStockMessage;
                    groupProducts.Add(product);
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return groupProducts;
        }

        protected virtual ShoppingCartItemModel GetCustomCartItem(ShoppingCartItemModel itemModel, ManageOrderDataModel orderDataModel = null, decimal? cartTaxCost = null)
            => IsNotNull(itemModel) ? new ShoppingCartItemModel
            {
                Description = itemModel.Description,
                ExtendedPrice = IsNotNull(orderDataModel?.UnitPrice) ? orderDataModel.UnitPrice.GetValueOrDefault() * orderDataModel.CustomQuantity : itemModel.ExtendedPrice,
                ExternalId = IsNull(orderDataModel) ? itemModel.ExternalId : Guid.NewGuid().ToString(),
                ProductId = itemModel.ProductId,
                ParentProductId = itemModel.ParentProductId,
                Quantity = itemModel.GroupProducts?.Count > 0 ? itemModel.Quantity : orderDataModel?.CustomQuantity > 0 ? orderDataModel.CustomQuantity : itemModel.Quantity,
                ShippingCost = itemModel.ShippingCost,
                CustomShippingCost = itemModel.CustomShippingCost,
                ProductDiscountAmount = itemModel.ProductDiscountAmount,
                ShippingOptionId = itemModel.ShippingOptionId,
                SKU = itemModel.SKU,
                IsActive = itemModel.IsActive,
                UnitPrice = itemModel.UnitPrice,
                CustomUnitPrice = itemModel.CustomUnitPrice,
                InsufficientQuantity = itemModel.InsufficientQuantity,
                CartDescription = itemModel.CartDescription,
                CurrencyCode = itemModel.CurrencyCode,
                ImagePath = itemModel.ImagePath,
                MediaConfigurationId = itemModel.MediaConfigurationId,
                ProductName = itemModel.ProductName,
                ProductType = itemModel.ProductType,
                ImageMediumPath = itemModel.ImageMediumPath,
                MaxQuantity = itemModel.MaxQuantity,
                MinQuantity = itemModel.MinQuantity,
                AddOnProductSKUs = itemModel.AddOnProductSKUs,
                BundleProductSKUs = itemModel.BundleProductSKUs,
                ConfigurableProductSKUs = itemModel.ConfigurableProductSKUs,
                GroupProducts = IsNotNull(orderDataModel) ? null : itemModel.GroupProducts,
                QuantityOnHand = itemModel.QuantityOnHand,
                SeoPageName = itemModel.SeoPageName,
                ProductCode = itemModel.ProductCode,
                TrackingNumber = !string.IsNullOrEmpty(orderDataModel?.TrackingNumber) ? orderDataModel.TrackingNumber : itemModel.TrackingNumber,
                UOM = itemModel.UOM,
                TrackInventory = itemModel.TrackInventory,
                AllowBackOrder = itemModel.AllowBackOrder,
                IsEditStatus = itemModel.IsEditStatus,
                IsSendEmail = itemModel.IsSendEmail,
                ShipSeperately = itemModel.ShipSeperately,
                OmsQuoteId = itemModel.OmsQuoteId,
                OmsQuoteLineItemId = itemModel.OmsQuoteLineItemId,
                ChildProductId = itemModel.ChildProductId,
                OmsOrderLineItemsId = itemModel.OmsOrderLineItemsId,
                OmsOrderStatusId = orderDataModel?.OrderLineItemStatusId > 0 ? orderDataModel.OrderLineItemStatusId : itemModel.OmsOrderStatusId,
                OrderLineItemStatus = orderDataModel?.OrderLineItemStatusId > 0 ? orderDataModel.OrderLineItemStatus : itemModel.OrderLineItemStatus,
                ParentOmsQuoteLineItemId = itemModel.ParentOmsQuoteLineItemId,
                PersonaliseValuesDetail = itemModel.PersonaliseValuesDetail,
                OrderLineItemRelationshipTypeId = itemModel.OrderLineItemRelationshipTypeId,
                Sequence = itemModel.Sequence,
                CustomText = itemModel.CustomText,
                CartAddOnDetails = itemModel.CartAddOnDetails,
                Product = itemModel.Product,
                ShippingAddress = itemModel.ShippingAddress,
                MultipleShipToAddress = itemModel.MultipleShipToAddress,
                TaxCost = IsNotNull(orderDataModel) && cartTaxCost > 0 ? (itemModel.TaxCost / orderDataModel.Quantity) * orderDataModel.CustomQuantity : itemModel.TaxCost,
                PersonaliseValuesList = itemModel.PersonaliseValuesList,
                IsShippingReturn = IsNotNull(orderDataModel) ? orderDataModel.IsShippingReturn : itemModel.IsShippingReturn,
                PartialRefundAmount = IsNotNull(orderDataModel?.PartialRefundAmount) ? orderDataModel.PartialRefundAmount : itemModel.PartialRefundAmount,
                Custom1 = itemModel.Custom1,
                Custom2 = itemModel.Custom2,
                Custom3 = itemModel.Custom3,
                Custom4 = itemModel.Custom4,
                Custom5 = itemModel.Custom5,
                AdditionalCost = itemModel.AdditionalCost ,
                PerQuantityShippingCost =itemModel.PerQuantityShippingCost,
                PerQuantityShippingDiscount = itemModel.PerQuantityShippingDiscount,
                IsItemStateChanged = itemModel.IsItemStateChanged
            } : new ShoppingCartItemModel();

        //Set Expand For Order Status
        protected virtual ExpandCollection SetExpandForOrderStatus()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.OrderLineItems);
            expands.Add(ExpandKeys.PaymentType);
            expands.Add(ExpandKeys.OmsOrderState);
            expands.Add(ExpandKeys.OmsPaymentState);
            expands.Add(ExpandKeys.ShippingType);
            expands.Add(ExpandKeys.UserDetails);
            return expands;
        }

        //Bind order payment state list.
        public virtual List<SelectListItem> ToOrderPaymentStateList(List<OrderPaymentStateModel> orderPaymentStateList, string paymentType)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderPaymentStateListCount = orderPaymentStateList?.Count, PaymentType = paymentType });
            if (!string.IsNullOrEmpty(paymentType))
            {
                List<SelectListItem> orderStateItems = new List<SelectListItem>();
                if (orderPaymentStateList?.Count() > 0)
                    orderStateItems = (from item in orderPaymentStateList
                                       where item.Name == ZnodeConstant.RECEIVED.ToString() || item.Name == ZnodeConstant.PENDING.ToString()
                                       orderby item.Name ascending
                                       select new SelectListItem
                                       {
                                           Text = item.Name,
                                           Value = item.OmsPaymentStateId.ToString()
                                       }).ToList();

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return orderStateItems;
            }
            return new List<SelectListItem>();
        }

        //Get dynamic attribute controls
        protected virtual List<PIMProductAttributeValuesViewModel> GetAttributeControls(List<PIMProductAttributeValuesViewModel> attributeValueList, List<string> distinctAttributeCodes, Dictionary<string, string> personliseValues)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AttributeValueListCount = attributeValueList?.Count, DistinctAttributeCodesCount = distinctAttributeCodes?.Count });
            int indexValue = 0;
            List<PIMProductAttributeValuesViewModel> finalAttributeList = new List<PIMProductAttributeValuesViewModel>();
            if (IsNotNull(attributeValueList) && IsNotNull(distinctAttributeCodes))
            {
                finalAttributeList = attributeValueList.Where(x => x != null).GroupBy(x => x.AttributeCode + x.PimAttributeFamilyId).Select(g => g.First()).ToList();

                foreach (string item in distinctAttributeCodes)
                {
                    List<PIMProductAttributeValuesViewModel> attributesList = attributeValueList.Where(x => x != null && x.AttributeCode + x.PimAttributeFamilyId == item?.ToString()).ToList();

                    //Appended keys with property name {AttributeCode}[0]_{PimAttributeId}[1]_{PimAttributeDefaultValueId}[2]_{PimAttributeValueId}[3]_{PimAttributeFamilyId}[4].
                    string controlName = $"{attributesList[0].AttributeCode}";

                    finalAttributeList[indexValue].ControlProperty.Id = controlName;
                    finalAttributeList[indexValue].ControlProperty.ControlType = attributesList[0].AttributeTypeName;
                    finalAttributeList[indexValue].ControlProperty.Name = controlName;
                    finalAttributeList[indexValue].ControlProperty.ControlLabel = attributesList[0].AttributeName;
                    finalAttributeList[indexValue].ControlProperty.Value = string.IsNullOrEmpty(attributesList[0].AttributeValue) ? attributesList[0].AttributeDefaultValue : attributesList[0].AttributeValue;
                    if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.MultiSelect.ToString()) || Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.SimpleSelect.ToString()))
                    {
                        finalAttributeList[indexValue].ControlProperty.SelectOptions = new List<SelectListItem>();
                        var SelectOptionsList = attributesList.Select(x => new { x.AttributeDefaultValue, x.AttributeDefaultValueCode }).ToList();
                        finalAttributeList[indexValue].ControlProperty.Value = attributesList[0].AttributeValue;
                        foreach (var SelectOptions in SelectOptionsList)
                        {
                            if (!string.IsNullOrEmpty(SelectOptions?.AttributeDefaultValueCode))
                            {
                                finalAttributeList[indexValue].ControlProperty.SelectOptions.Add(new SelectListItem() { Text = SelectOptions.AttributeDefaultValue, Value = SelectOptions.AttributeDefaultValueCode });
                                finalAttributeList[indexValue].ControlProperty.CSSClass = finalAttributeList[indexValue].AttributeCode;
                            }
                        }
                    }
                    if (attributesList[0].IsRequired)
                    {
                        if (IsKeyNotPresent(ZnodeConstant.IsRequired, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                            finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(ZnodeConstant.IsRequired, attributesList[0].IsRequired);
                    }

                    if (Equals(Regex.Replace(attributesList[0].AttributeTypeName, @"\s", ""), ControlTypes.Label.ToString()))
                        finalAttributeList[indexValue].ControlProperty.Value = attributesList[0].AttributeDefaultValue;

                    foreach (var dataItem in attributesList)
                    {
                        if (!Equals(dataItem.ValidationName, null) && !Equals(dataItem.ValidationName, ZnodeConstant.Extensions))
                        {
                            if (Equals(dataItem.ControlName, ZnodeConstant.Select) || Equals(dataItem.ControlName, ZnodeConstant.MultiSelect) || Equals(dataItem.ControlName, ControlTypes.SimpleSelect.ToString()))
                            {
                                if (IsKeyNotPresent(dataItem.ValidationName, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(dataItem.ValidationName, dataItem.SubValidationName);
                            }
                            else
                            {
                                if (IsKeyNotPresent(dataItem.ValidationName, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(dataItem.ValidationName, dataItem.ValidationValue);
                            }
                        }
                        else if ((attributesList.Select(x => x.ValidationName == ZnodeConstant.Extensions).ToList()).Any(m => m))
                        {
                            if (finalAttributeList[indexValue].ControlProperty.htmlAttributes.ContainsKey(ZnodeConstant.Extensions) == false)
                            {
                                if (IsKeyNotPresent(ZnodeConstant.Extensions, finalAttributeList[indexValue].ControlProperty.htmlAttributes))
                                {
                                    string result = string.Join(",", attributesList.Where(x => x.ValidationName == ZnodeConstant.Extensions).Select(k => k.SubValidationName).ToArray());
                                    finalAttributeList[indexValue].ControlProperty.htmlAttributes.Add(ZnodeConstant.Extensions, result);
                                }
                            }
                        }
                    }
                    indexValue++;
                }
            }

            finalAttributeList.ForEach(
                      x =>
                      {
                          string value = personliseValues.Where(y => y.Key == x.AttributeCode)?.FirstOrDefault().Value;
                          x.ControlProperty.htmlAttributes?.Add("IsPersonalizable", "True");
                          if (x.AttributeTypeName == ControlTypes.Text.ToString())
                              x.ControlProperty.htmlAttributes?.Add("placeholder", value);
                      });

            ZnodeLogging.LogMessage("Attribute controls list count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AttributeControlsListCount = finalAttributeList?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return finalAttributeList;
        }

        //This method returns true if key is not present in dictionary else return false.
        protected virtual bool IsKeyNotPresent(string key, IDictionary<string, object> source)
        {
            if (IsNotNull(source) && !string.IsNullOrEmpty(key))
                return !source.ContainsKey(key);
            return false;
        }

        public virtual ShippingListViewModel GetShippingListAndRates(ShoppingCartModel shoppingCart, bool excludeCustomShippingFromCreateOrder = false)
        {
            if (IsNull(shoppingCart)) return new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };

            string zipCode = shoppingCart?.ShippingAddress?.PostalCode;
            shoppingCart.PublishStateId = DefaultSettingHelper.GetCurrentOrDefaultAppType(ZnodePublishStatesEnum.PRODUCTION);
            //If custom shipping cost is passed then it will return all the shipping with custom shipping cost only.
            if (excludeCustomShippingFromCreateOrder)
                shoppingCart.CustomShippingCost = null;
            ShippingListViewModel listViewModel = new ShippingListViewModel { ShippingList = _shoppingCartClient.GetShippingEstimates(zipCode, shoppingCart)?.ShippingList?.ToViewModel<ShippingViewModel>().ToList() };
            string cultureCode = shoppingCart?.CultureCode;
            listViewModel?.ShippingList?.ToList().ForEach(x => x.FormattedShippingRate = HelperMethods.FormatPriceWithCurrency(x.ShippingRate, cultureCode));
            listViewModel?.ShippingList?.ToList().ForEach(x => x.FormattedShippingRateWithoutDiscount = (x?.ShippingRateWithoutDiscount > 0) ? HelperMethods.FormatPriceWithCurrency(x?.ShippingRateWithoutDiscount, cultureCode) : string.Empty);
            listViewModel.CustomShippingCost = shoppingCart?.CustomShippingCost;
            ZnodeLogging.LogMessage("ShippingList count and CustomShippingCost: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingListCount = listViewModel?.ShippingList?.Count, CustomShippingCost = listViewModel?.CustomShippingCost });

            IAdminHelper _adminHelper = GetService<IAdminHelper>();

            if (!listViewModel.InHandDate.HasValue)
            {
                listViewModel.InHandDate = _adminHelper.GetInHandDate();
            }

            listViewModel.ShippingConstraints = _adminHelper.GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete);

            return listViewModel?.ShippingList?.Count > 0 ? listViewModel : new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };
        }

        //Map order total to cart
        protected virtual OrderTotalViewModel MapOrderTotalToCart(CartViewModel cartViewModel)
        {
            return new OrderTotalViewModel()
            {
                CSRDiscountAmount = cartViewModel.CSRDiscountAmount,
                CurrencyCode = cartViewModel.CurrencyCode,
                DiscountAmount = cartViewModel.Discount,
                GiftCardAmount = cartViewModel.GiftCardAmount,
                ShippingCost = cartViewModel.ShippingCost,
                SubTotal = cartViewModel.SubTotal,
                TaxCost = cartViewModel.TaxCost,
                Total = cartViewModel.Total
            };
        }

        //Get Customer Associated Profile List
        protected virtual ProfileListModel GetCustomerAssociatedProfileList(CreateOrderViewModel createOrderViewModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, createOrderViewModel.UserAddressDataViewModel.UserId.ToString()));

            //Get list of associated profile.
            ZnodeLogging.LogMessage("Filters profile list:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            ProfileListModel profileList = _customerClient.GetAssociatedProfilelist(null, filters, null, null, null);
            return profileList;
        }

        //Get list of product Attributes.
        protected virtual ConfigurableAttributeViewModel GetProductAttribute(int productId, ParameterProductModel model, List<PublishAttributeViewModel> attribute, bool isDefaultAssociatedProduct)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (productId > 0)
            {
                ConfigurableAttributeViewModel configurableData = new ConfigurableAttributeViewModel();

                //Get configurable attributes.
                ConfigurableAttributeListModel attributes = _publishProductClient.GetProductAttribute(productId, model);

                if (attributes?.Attributes?.Count > 0)
                {
                    //Get the selected configurable attributes.
                    List<PublishAttributeViewModel> viewModel = attributes?.Attributes.ToViewModel<PublishAttributeViewModel>().ToList();
                    foreach (PublishAttributeViewModel configurableAttribute in viewModel)
                    {
                        foreach (ProductAttributesViewModel productAttribute in configurableAttribute.ConfigurableAttribute)
                        {
                            configurableAttribute.SelectedAttributeValue = new[] { (model.SelectedAttributes[configurableAttribute.AttributeCode]) };
                            if (isDefaultAssociatedProduct && productAttribute.AttributeValue == model.SelectedValue)
                                productAttribute.IsDisabled = true;
                        }
                    }

                    //Remove all configurable attributes and add newly assign configurable attributes.
                    foreach (PublishAttributeModel configurableAttribute in attributes.Attributes)
                        attribute.RemoveAll(x => x.AttributeCode == configurableAttribute.AttributeCode);

                    attribute.AddRange(viewModel);

                    configurableData.ConfigurableAttributes = attribute;
                    //Get selected color value.
                    string selectedColorOption = configurableData.ConfigurableAttributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ColorAttributeCode)?.SelectedAttributeValue[0];

                    ZnodeLogging.LogMessage("ConfigurableAttributes count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ConfigurableAttributesCount = configurableData?.ConfigurableAttributes?.Count });
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return configurableData;
                }
            }
            return null;
        }

        //Check whether shipping address is valid or not.
        protected virtual BooleanModel IsValidAddressForCheckout(CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("ShippingAddress with Id: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.AddressId });
            if (createOrderViewModel.EnableAddressValidation)
            {
                if (IsNotNull(createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress))
                {
                    createOrderViewModel.UserAddressDataViewModel.ShippingAddress.PublishStateId = (byte)ZnodePublishStatesEnum.PRODUCTION;
                    createOrderViewModel.UserAddressDataViewModel.ShippingAddress.PortalId = createOrderViewModel.PortalId;
                }
                //Do not allow the customer to go to next page if valid shipping address required is enabled.
                return _shippingClient.IsShippingAddressValid(createOrderViewModel.UserAddressDataViewModel.ShippingAddress.ToModel<AddressModel>());
            }
            return new BooleanModel { IsSuccess = true };
        }

        //Check whether shipping address is valid or not.
        protected virtual BooleanModel IsValidAddressForCheckout(bool enableAddressValidation, AddressModel address)
        {
            ZnodeLogging.LogMessage("AddressModel with Id: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = address?.AddressId });
            if (enableAddressValidation)
            {
                address.PublishStateId = (byte)ZnodePublishStatesEnum.PRODUCTION;
                //Do not allow the customer to go to next page if valid shipping address required is enabled.
                return _shippingClient.IsShippingAddressValid(address);
            }
            return new BooleanModel { IsSuccess = true };
        }

        //Get Portal Catalog By Portal Id.
        public virtual void GetPortalCatalogByPortalId(int portalId, UserAddressDataViewModel userAddressDataViewModel)
        {
            //Get Associated Portal Catalog By PortalId.
            ZnodeLogging.LogMessage("PortalId to get portal:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = portalId });
            PortalModel portal = _portalClient.GetPortal(portalId, new ExpandCollection { ZnodePortalEnum.ZnodePortalLocales.ToString(), ZnodePortalEnum.ZnodePortalUnits.ToString(), ZnodePortalEnum.ZnodePortalCatalogs.ToString() });

            if (IsNotNull(portal))
            {
                ZnodeLogging.LogMessage("UserAddressDataViewModel with AccountId, UserId and PortalCatalogId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AccountId = userAddressDataViewModel?.AccountId, UserId = userAddressDataViewModel?.UserId, PortalCatalogId = userAddressDataViewModel?.PortalCatalogId });
                //set Portal related data in userAddressDataViewModel.
                userAddressDataViewModel.PortalCatalogId = (userAddressDataViewModel.PortalCatalogId <= 0) ? portal.PublishCatalogId.GetValueOrDefault() : userAddressDataViewModel.PortalCatalogId;
                userAddressDataViewModel.PortalId = portalId;
                userAddressDataViewModel.LocaleId = portal.LocaleId.GetValueOrDefault();
                userAddressDataViewModel.MediaServerThumbnailUrl = portal.MediaServerThumbnailUrl;
                userAddressDataViewModel.MediaServerUrl = portal.MediaServerUrl;
                userAddressDataViewModel.EnableAddressValidation = portal.SelectedPortalFeatures.Any(p => p.PortalFeatureName == StoreFeature.Address_Validation.ToString());
                userAddressDataViewModel.RequireValidatedAddress = portal.SelectedPortalFeatures.Any(p => p.PortalFeatureName == StoreFeature.Require_Validated_Address.ToString());
                userAddressDataViewModel.IsMultipleCouponCodeAllowed = portal.SelectedPortalFeatures.Any(p => p.PortalFeatureName == StoreFeature.Allow_multiple_coupons.ToString());
                userAddressDataViewModel.InStockMessage = portal.InStockMsg;
                userAddressDataViewModel.OutOfStockMessage = portal.OutOfStockMsg;
                userAddressDataViewModel.BackOrderMessage = portal.BackOrderMsg;
                userAddressDataViewModel.CurrencyCode = portal.DefaultCurrency;
            }
        }

        //Create edit new customer's shipping/billing address.
        protected virtual UserAddressDataViewModel CreateEditCustomerAddress(UserAddressDataViewModel userAddressDataViewModel)
        {
            //Get Shipping/Billing Address of user.
            userAddressDataViewModel = GetUsersShippingBillingAddress(userAddressDataViewModel);

            //Create new Customer.
            userAddressDataViewModel = _orderClient.CreateNewCustomer(userAddressDataViewModel?.ToModel<UserAddressModel>())?.ToViewModel<UserAddressDataViewModel>();
            return userAddressDataViewModel;
        }

        //to get order total for submit order
        protected virtual string GetOrderTotal(int? paymentTypeId = null, int? userId = 0)
        {
            decimal? total = 0;
            //Get shopping Cart from Session or cookie

            ShoppingCartModel shoppingCart = GetCartModelFromSession(userId) ??
                           _cartAgent.GetCartFromCookie();
            if (IsNotNull(shoppingCart))
            {
                total = shoppingCart.Total;
                if (!Equals(shoppingCart.OmsOrderId, null) && shoppingCart.OmsOrderId > 0)
                {
                    OrderModel ordermodel = _orderClient.GetOrderById(shoppingCart.OmsOrderId.GetValueOrDefault(), null);
                    int? orderPaymentTypeId = shoppingCart?.Payment?.PaymentSetting?.PaymentTypeId ?? paymentTypeId;
                    if (Equals(ordermodel.PaymentTypeId, orderPaymentTypeId))
                    {
                        total = shoppingCart.Total;
                    }
                }
            }
            ZnodeLogging.LogMessage("Order total: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderTotal = total });
            return Convert.ToString(total);
        }

        //to get order total for submit order
        protected virtual bool CreditCardEnabled(int? paymentTypeId = null, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PaymentTypeId = paymentTypeId });
            bool ccEnabled = true;
            //Get shopping Cart from Session or cookie
            ShoppingCartModel shoppingCart = GetCartModelFromSession(userId) ??
                           _cartAgent.GetCartFromCookie();

            if (IsNotNull(shoppingCart))
            {
                if (!Equals(shoppingCart.OmsOrderId, null) && shoppingCart.OmsOrderId > 0)
                {
                    OrderModel ordermodel = _orderClient.GetOrderById(shoppingCart.OmsOrderId.GetValueOrDefault(), null);
                    int? orderPaymentTypeId = shoppingCart?.Payment?.PaymentSetting?.PaymentTypeId ?? paymentTypeId;
                    if (Equals(ordermodel.PaymentTypeId, orderPaymentTypeId) && (Convert.ToDecimal(shoppingCart.Total - ordermodel.Total) <= 0))
                        ccEnabled = false;
                }
            }
            return ccEnabled;
        }

        //Set filters to get customerList.
        protected virtual void SetCustomerListFilters(FilterCollection filters, int portalId, int accountId = 0, bool isAccountCustomer = false)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = portalId, AccountId = accountId, IsAccountCustomer = isAccountCustomer });
            //Add portalId filter.
            HelperMethods.SetPortalIdFilters(filters, portalId);

            if (filters.Exists(x => x.Item1 == ZnodeUserEnum.IsActive.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.IsActive.ToString());

            filters.Add(new FilterTuple(ZnodeUserEnum.IsActive.ToString(), FilterOperators.Equals, Convert.ToString(1)));

            if (isAccountCustomer)
            {
                if (filters.Exists(x => x.Item1.ToLower() == ZnodeAccountPermissionEnum.AccountId.ToString().ToLower()))
                    //If AccountId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1.ToLower() == ZnodeAccountPermissionEnum.AccountId.ToString().ToLower());
                if (accountId > 0)
                    filters.Add(new FilterTuple(ZnodeAccountPermissionEnum.AccountId.ToString(), FilterOperators.Equals, Convert.ToString(accountId)));
            }
            ZnodeLogging.LogMessage("Filters to get customerList: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
        }

        //Set filters to get GuestUser.
        protected virtual void SetGuestUserListFilters(FilterCollection filters)
        {
            if (filters.Exists(x => x.Item1.ToLower() == View_CustomerUserDetailEnum.IsGuestUser.ToString().ToLower()))
                //If IsGuestUser Already present in filters Remove It
                filters.RemoveAll(x => x.Item1.ToLower() == View_CustomerUserDetailEnum.IsGuestUser.ToString().ToLower());

            filters.Add(new FilterTuple(View_CustomerUserDetailEnum.IsGuestUser.ToString(), FilterOperators.Equals, AdminConstants.False));
            ZnodeLogging.LogMessage("Filters to get GuestUse: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
        }

        //Save Shipping nd Billing Address in Session
        protected virtual void SaveAddressInSession(UserAddressDataViewModel userAddressDataViewModel)
        {
            SaveInSession(AdminConstants.ShippingAddressKey, userAddressDataViewModel.ShippingAddress);
            SaveInSession(AdminConstants.BillingAddressKey, userAddressDataViewModel.BillingAddress);
        }

        //Set shipping and billing address of user.
        protected virtual void GetUserAddress(UserAddressDataViewModel userAddressDataViewModel)
        {
            //Get B2B account or user address list.
            AddressListModel addressList = GetAddressListOfUserAndAccount(userAddressDataViewModel.UserId, userAddressDataViewModel.AccountId.GetValueOrDefault());

            //Map AddressListModel to AddressListViewModel.
            AddressListViewModel addressListViewModel = new AddressListViewModel { AddressList = addressList?.AddressList?.ToViewModel<AddressViewModel>().ToList() };

            //Set user's default shipping billing address.
            SetUserDefaultAddress(userAddressDataViewModel, addressList, addressListViewModel);

            if (IsNotNull(userAddressDataViewModel.ShippingAddress) && IsNotNull(userAddressDataViewModel.BillingAddress))
                userAddressDataViewModel.UseSameAsBillingAddress = Equals(userAddressDataViewModel.ShippingAddress.AddressId, userAddressDataViewModel.BillingAddress.AddressId);

            //Set address list in address session.
            SaveAddressListToSession(userAddressDataViewModel.UserId, addressList?.AddressList?.ToViewModel<AddressViewModel>().ToList());

            ZnodeLogging.LogMessage("ShippingAddressId and BillingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingAddressId = userAddressDataViewModel?.ShippingAddress?.AddressId, BillingAddressId = userAddressDataViewModel?.BillingAddress?.AddressId });
        }

        //Set user's default shipping billing address.
        protected virtual void SetUserDefaultAddress(UserAddressDataViewModel userAddressDataViewModel, AddressListModel addressList, AddressListViewModel addressListViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            // Map shipping billing address of user userAddressDataViewModel.
            userAddressDataViewModel.ShippingAddress = addressListViewModel.AddressList?.Where(x => x.IsDefaultShipping)?.FirstOrDefault();
            userAddressDataViewModel.BillingAddress = addressListViewModel.AddressList?.Where(x => x.IsDefaultBilling)?.FirstOrDefault();
            userAddressDataViewModel.UsersAddressNameList = OrderViewModelMap.ToListItems(addressList.AddressList);
            userAddressDataViewModel.UserBillingAddressId = Convert.ToInt32(addressListViewModel.AddressList?.Where(x => x.IsDefaultBilling)?.Select(m => m.UserAddressId).FirstOrDefault());
            userAddressDataViewModel.UserShippingAddressId = Convert.ToInt32(addressListViewModel.AddressList?.Where(x => x.IsDefaultShipping)?.Select(m => m.UserAddressId).FirstOrDefault());

            ZnodeLogging.LogMessage("UserBillingAddressId, UserShippingAddressId and UsersAddressNameList count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserBillingAddressId = userAddressDataViewModel?.UserBillingAddressId, UserShippingAddressId = userAddressDataViewModel?.UserShippingAddressId, UsersAddressNameListCount = userAddressDataViewModel?.UsersAddressNameList?.Count });
            if (IsNull(userAddressDataViewModel.ShippingAddress))
                userAddressDataViewModel.ShippingAddress = new AddressViewModel();

            if (IsNull(userAddressDataViewModel.BillingAddress))
                userAddressDataViewModel.BillingAddress = new AddressViewModel();

            // get address if customer not having addresses.
            if (IsNull(addressList?.AddressList))
            {
                userAddressDataViewModel.ShippingAddress = new AddressViewModel();
                userAddressDataViewModel.BillingAddress = new AddressViewModel();
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get parent Account Details of user.
        protected virtual void GetAccountdetails(int userId, UserAddressDataViewModel userAddressDataViewModel)
        {
            //Get parent account by user Id.
            ZnodeLogging.LogMessage("UserId to get user account data:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId });
            UserModel userModel = _userClient.GetUserAccountData(userId, new ExpandCollection { ExpandKeys.Profiles }, userAddressDataViewModel.PortalId);

            //Assign account details.
            if (IsNotNull(userModel))
            {
                userAddressDataViewModel.AccountId = userModel.AccountId;
                userAddressDataViewModel.Email = userModel.Email;
                userAddressDataViewModel.FullName = userModel.User.Username + (userModel.FullName?.Trim().Length > 0 ? " | " + userModel.FullName : "");
                userAddressDataViewModel.CustomerPaymentGUID = userModel.CustomerPaymentGUID;
                userAddressDataViewModel.BudgetAmount = userModel.BudgetAmount;
                userAddressDataViewModel.PermissionCode = userModel.PermissionCode;
                userAddressDataViewModel.ProfileId = (userModel.Profiles?.Where(x => x.IsDefault.GetValueOrDefault())?.FirstOrDefault()?.ProfileId).GetValueOrDefault();
                userAddressDataViewModel.PortalId = userModel.PortalId.GetValueOrDefault();
                userAddressDataViewModel.PortalCatalogId = userModel.PublishCatalogId.GetValueOrDefault();
                userAddressDataViewModel.PhoneNumber = userModel?.PhoneNumber;
                userAddressDataViewModel.IsAddEditAddressAllow = HelperMethods.IsAddEditAddressAllow(userModel.RoleName);
            }

            userAddressDataViewModel.UserId = userId;
        }

        //Get the Expand collection with expand keys.
        protected virtual ExpandCollection GetExpands()
        => new ExpandCollection { ZnodeUserAddressEnum.ZnodeAddress.ToString() };

        //Get Shipping/Billing Address of user.
        protected virtual UserAddressDataViewModel GetUsersShippingBillingAddress(UserAddressDataViewModel userAddressDataViewModel)
        {
            if (userAddressDataViewModel.UseSameAsBillingAddress)
            {
                SetShippingDetails(userAddressDataViewModel);
            }
            else
            {
                SetBillingShippingDetails(userAddressDataViewModel);
            }
            return userAddressDataViewModel;
        }

        //set default setting for billing / shipping address.
        protected virtual void SetBillingShippingDetails(UserAddressDataViewModel userAddressDataViewModel)
        {
            userAddressDataViewModel.ShippingAddress.UserAddressId = userAddressDataViewModel.UserShippingAddressId > 0 ? userAddressDataViewModel.UserShippingAddressId : 0;
            userAddressDataViewModel.BillingAddress.UserAddressId = userAddressDataViewModel.UserBillingAddressId > 0 ? userAddressDataViewModel.UserBillingAddressId : 0;
            userAddressDataViewModel.BillingAddress.Address1 = string.IsNullOrEmpty(userAddressDataViewModel.BillingAddress.Address1) ? "Default Address" : userAddressDataViewModel.BillingAddress.Address1;
            userAddressDataViewModel.ShippingAddress.Address1 = string.IsNullOrEmpty(userAddressDataViewModel.ShippingAddress.Address1) ? "Default Address" : userAddressDataViewModel.ShippingAddress.Address1;
            userAddressDataViewModel.BillingAddress.IsDefaultBilling = true;
            userAddressDataViewModel.ShippingAddress.IsDefaultShipping = true;
            userAddressDataViewModel.ShippingAddress.IsDefaultBilling = false;
            userAddressDataViewModel.BillingAddress.IsDefaultShipping = false;
        }

        //set default setting for shipping address.
        protected virtual void SetShippingDetails(UserAddressDataViewModel userAddressDataViewModel)
        {
            userAddressDataViewModel.ShippingAddress.Address1 = string.IsNullOrEmpty(userAddressDataViewModel.BillingAddress.Address1) ? "Default Address" : userAddressDataViewModel.BillingAddress.Address1;
            userAddressDataViewModel.ShippingAddress.IsDefaultBilling = true;
            userAddressDataViewModel.ShippingAddress.IsDefaultShipping = true;
            userAddressDataViewModel.ShippingAddress.UserId = userAddressDataViewModel.UserId;
            userAddressDataViewModel.ShippingAddress.UserAddressId = userAddressDataViewModel.UserBillingAddressId > 0 ? userAddressDataViewModel.UserBillingAddressId : 0;
            userAddressDataViewModel.UserId = userAddressDataViewModel.UserId;
        }

        //Get Country list to create new customer.
        protected virtual UserAddressDataViewModel GetShippingBillingAddresCountries(UserAddressDataViewModel userAddressDataViewModel)
        {
            //Get portal associated country list.
            List<SelectListItem> countryList = HelperMethods.GetPortalAssociatedCountries(userAddressDataViewModel.PortalId);
            userAddressDataViewModel.ShippingAddress = new AddressViewModel { Countries = countryList };
            userAddressDataViewModel.BillingAddress = new AddressViewModel { Countries = countryList };
            ZnodeLogging.LogMessage("Country list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CountryListCount = countryList?.Count });
            return userAddressDataViewModel;
        }

        //Get Product type of product.
        protected virtual void GetProductType(PublishProductsViewModel products)
        {
            if (IsNotNull(products))
                products.ProductType = products.Attributes?.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Value;
        }

        //Get Product Price With Currency.
        protected virtual void GetProductPriceWithCurrency(PublishProductsViewModel products, string cultureCode)
        {
            if (IsNotNull(products))
            {
                products.RetailPriceWithCurrency = IsNull(products.RetailPrice) ? string.Empty : HelperMethods.FormatPriceWithCurrency(products.RetailPrice, cultureCode);
                products.SalesPriceWithCurrency = IsNull(products.SalesPrice) ? string.Empty : HelperMethods.FormatPriceWithCurrency(products.SalesPrice, cultureCode);
            }
        }

        //Set AllowBackOrder and TrackInventory of product for add to cart.
        protected virtual void SetInventorySetting(PublishProductsViewModel products, string inventorySetting, decimal combinedQuantity)
        {
            switch (inventorySetting)
            {
                case AdminConstants.DisablePurchasing:
                    products.AllowAddToCart = (products.Quantity > combinedQuantity);
                    break;

                case AdminConstants.AllowBackOrdering:
                    products.AllowAddToCart = true;
                    break;

                case AdminConstants.DontTrackInventory:
                    products.AllowAddToCart = true;
                    break;

                default:
                    //Between true if want to include min and max number in comparison.
                    products.AllowAddToCart = Between(Convert.ToDecimal(combinedQuantity), Convert.ToDecimal(products.MinQuantity), Convert.ToDecimal(products.MaxQuantity), true);
                    break;
            }
        }

        //Set payment options by profile id.
        protected virtual PaymentSettingListViewModel SetPaymentOptionByProfile(PaymentSettingListModel paymentOptionListModel, ProfileListModel profileList)
        {
            if (profileList?.Profiles?.Count > 0)
                paymentOptionListModel.PaymentSettings = paymentOptionListModel?.PaymentSettings.Where(p => profileList.Profiles.Any(p2 => p2.ProfileId == p.ProfileId || p.ProfileId == null)).ToList();

            return new PaymentSettingListViewModel { PaymentSettings = paymentOptionListModel?.PaymentSettings?.ToViewModel<PaymentSettingViewModel>().ToList() };
        }

        //Bind all payment option to Select List Item type.
        protected virtual List<BaseDropDownOptions> BindPaymentOptionToSelectListItem(List<PaymentSettingViewModel> listViewModel)
        {
            List<BaseDropDownOptions> paymentTypeItems = new List<BaseDropDownOptions>();
            listViewModel.ToList().ForEach(x =>
            {
                paymentTypeItems.Add(new BaseDropDownOptions()
                {
                    Id = x.PaymentCode,
                    Text = x.PaymentDisplayName,
                    Value = x.PaymentSettingId.ToString(),
                    Type = x.PaymentTypeName
                });
            });
            ZnodeLogging.LogMessage("PaymentTypeItems count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PaymentTypeItemsCount = paymentTypeItems.Count });
            return paymentTypeItems;
        }

        //Check product inventory and allow to add to cart.
        protected virtual void CheckInventory(PublishProductsViewModel products, int? userId = 0)
        {
            if (IsNotNull(products.SKU))
            {
                products.MinQuantity = Convert.ToDecimal(products.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.AttributeValues);
                products.MaxQuantity = Convert.ToDecimal(products.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MaximumQuantity)?.AttributeValues);

                decimal selectedQuantity = products.MinQuantity.GetValueOrDefault();

                //Get quantity of product from cart.
                decimal cartQuantity = GetOrderedItemQuantity(products.SKU, 0, userId);

                decimal combinedQuantity = selectedQuantity + cartQuantity;

                //Get inventory setting for product.
                List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeList(products);
                if (inventorySetting?.Count > 0)
                    //Set AllowAddToCart for add to cart.
                    SetInventorySetting(products, inventorySetting.FirstOrDefault().Code, combinedQuantity);
            }
        }

        protected virtual List<AttributesSelectValuesViewModel> GetOutOfStockOptionsAttributeList(PublishProductsViewModel products)
            => products.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);

        private static List<AttributesSelectValuesViewModel> GetOutOfStockOptionsAttributeListForBundle(AssociatedPublishedBundleProductViewModel products)
            => products.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);

        public virtual List<AttributesSelectValuesViewModel> GetAddOnOutOfStockOptionsAttributeList(AddOnValuesViewModel products)
            => products.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);

        //Get Updated Product Price.
        public virtual void GetProductFinalPrice(PublishProductsViewModel viewModel, List<AddOnViewModel> addOns, decimal minQuantity, string addOnSkus)
        {
            bool isAddonPriceAvailable = true;
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddOnsCount = addOns?.Count, MinQuantity = minQuantity, AddOnSkus = addOnSkus });
            //Apply tier price if any.
            if (viewModel.TierPriceList?.Count > 0 && viewModel.TierPriceList.Where(x => minQuantity >= x.MinQuantity)?.Count() > 0)
                viewModel.ProductPrice = viewModel.TierPriceList.FirstOrDefault(x => minQuantity >= x.MinQuantity && minQuantity < x.MaxQuantity)?.Price * minQuantity;
            else
                viewModel.ProductPrice = (minQuantity > 0 && IsNotNull(viewModel.SalesPrice)) ? viewModel.SalesPrice * minQuantity : viewModel.PromotionalPrice > 0 ? viewModel.PromotionalPrice * minQuantity : viewModel.RetailPrice * minQuantity;

            //Get Add on product price if any.
            if (addOns?.Count > 0)
            {
                decimal? addonPrice = 0.00M;

                //Check if selected add ons are empty.
                if (!string.IsNullOrEmpty(addOnSkus))
                {
                    foreach (string addOn in addOnSkus.Split(','))
                    {
                        AddOnValuesViewModel addOnValue = addOns.SelectMany(
                                    y => y.AddOnValues.Where(x => x.SKU == addOn))?.FirstOrDefault();
                        if (IsNotNull(addOnValue))
                            addonPrice = addonPrice + Convert.ToDecimal(IsNotNull(addOnValue.SalesPrice) ? addOnValue.SalesPrice : addOnValue.RetailPrice) * minQuantity;

                        if (Convert.ToDecimal(HelperUtility.IsNotNull(addOnValue.SalesPrice) ? addOnValue?.SalesPrice : addOnValue?.RetailPrice) == 0)
                            isAddonPriceAvailable = false;
                    }
                }

                viewModel.ProductPrice = addonPrice > 0 ? viewModel.ProductPrice + addonPrice : viewModel.ProductPrice;
                ZnodeLogging.LogMessage("ProductPrice: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ProductPrice = viewModel?.ProductPrice });
                //Check add on price.
                if (!isAddonPriceAvailable)
                {
                    viewModel.ShowAddToCart = false;
                    viewModel.InventoryMessage = Convert.ToBoolean(viewModel?.Attributes?.Value(ZnodeConstant.CallForPricing)) ? string.Empty : WebStore_Resources.ErrorAddOnPrice;
                }
            }
            //Check product final price.
            if (IsNull(viewModel.ProductPrice))
            {
                viewModel.ShowAddToCart = false;
                viewModel.InventoryMessage = Convert.ToBoolean(viewModel?.Attributes?.Value(ZnodeConstant.CallForPricing)) ? string.Empty : Admin_Resources.ErrorPriceNotAssociate;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        // Set bundle product inventorymessage  and ShowAddToCart based on the child product
        public virtual void CheckBundleProductsInventory(PublishProductsViewModel viewModel, int omsOrderId = 0, int? userId = 0, decimal selectedQuantity= 0)
        {

            foreach (AssociatedPublishedBundleProductViewModel publishBundleProduct in viewModel.PublishBundleProducts)
            {
                string minQuantity = publishBundleProduct.Attributes?.Value(ZnodeConstant.MinimumQuantity);
                decimal quantity = selectedQuantity != 0 ? selectedQuantity : Convert.ToDecimal(minQuantity);
                CheckBundleProductItemsInventory(publishBundleProduct, quantity, 0, userId);
                if(!publishBundleProduct.ShowAddToCart)
                {
                    viewModel.InventoryMessage = publishBundleProduct.InventoryMessage;
                    viewModel.ShowAddToCart = publishBundleProduct.ShowAddToCart;
                    break;
                }
            }
            if (string.IsNullOrEmpty(viewModel.InventoryMessage))
            {
                string parentMinQuantity = viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity);
                decimal? parentQuantity = selectedQuantity != 0 ? selectedQuantity : Convert.ToDecimal(parentMinQuantity);
                BundleProductParentCheckInventory(viewModel, parentQuantity, 0, userId);
            }
        }

        public virtual void CheckBundleProductItemsInventory(AssociatedPublishedBundleProductViewModel viewModel, decimal? quantity, int omsOrderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeListForBundle(viewModel);
            string inventorySettingCode = inventorySetting.FirstOrDefault().Code;
            decimal selectedQuantity = quantity.GetValueOrDefault();
            decimal cartQuantity = GetOrderedItemQuantity(viewModel.SKU, omsOrderId, userId);
            decimal combinedQuantity = (selectedQuantity + cartQuantity) * viewModel.AssociatedQuantity;

            ZnodeLogging.LogMessage("selectedQuantity, cartQuantity and combinedQuantity: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SelectedQuantity = selectedQuantity, CartQuantity = cartQuantity, CombinedQuantity = combinedQuantity });

            if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                {
                    viewModel.InventoryMessage = string.Format(Admin_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                    viewModel.ShowAddToCart = false;
                    return;
                }
                else
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : Admin_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                    return;
                }
            }
            if (IsNotNull(viewModel?.Quantity))
            {
                bool AllowBackOrder = false;
                bool TrackInventory = false;

                if (inventorySetting?.Count > 0)
                    TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySettingCode);

                if (viewModel.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory)
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : Admin_Resources.TextExceedingQuantity;
                    viewModel.ShowAddToCart = false;
                    return;
                }
                else if (viewModel.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.BackOrderMessage) ? viewModel.BackOrderMessage : Admin_Resources.TextBackOrderMessage;
                    viewModel.ShowAddToCart = true;
                    return;
                }
                viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : Admin_Resources.TextInstock;
                viewModel.ShowAddToCart = true;
            }
            else
            {
                viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : Admin_Resources.TextOutofStock;
                viewModel.ShowAddToCart = false;
                return;
            }
        }

        // Check the bundle child product inventory and update the InventoryMessage and ShowAddToCart based on child inventory and associated child quantity
        public virtual void BundleProductParentCheckInventory(PublishProductsViewModel viewModel, decimal? quantity, int omsOrderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            // Add sku of child product in SKU property to check inventory of child product.
            SwapSkuOfConfigurableProduct(viewModel);
            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeList(viewModel);
            string inventorySettingCode = inventorySetting.FirstOrDefault().Code;

            decimal selectedQuantity = quantity.GetValueOrDefault();

            decimal cartQuantity = GetOrderedItemQuantity(viewModel.SKU, omsOrderId, userId);

            decimal combinedQuantity = selectedQuantity + cartQuantity;

            if (IsNotNull(viewModel?.Quantity))
            {
                if (!Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                {
                    viewModel.InventoryMessage = string.Format(Admin_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                    viewModel.ShowAddToCart = false;
                    return;
                }
                else
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : Admin_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                    return;
                }
            }
        }

        // Check if the entered quantity is between the minimum and maximum quantity range.
        protected virtual bool ValidateMinMaxQuantity(decimal combinedQuantity, PublishProductsViewModel viewModel)
        {
            if (!Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
            {
                viewModel.InventoryMessage = string.Format(Admin_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                viewModel.ShowAddToCart = false;
                return false;
            }
            return true;
        }


        //Check Inventory
        public virtual void CheckInventory(PublishProductsViewModel viewModel, decimal? quantity, int omsOrderId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            // Add sku of child product in SKU property to check inventory of child product.
            SwapSkuOfConfigurableProduct(viewModel);
            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeList(viewModel);
            string inventorySettingCode = inventorySetting.FirstOrDefault().Code;

            decimal selectedQuantity = quantity.GetValueOrDefault();

            decimal cartQuantity = GetOrderedItemQuantity(viewModel.SKU, omsOrderId, userId);

            decimal combinedQuantity = selectedQuantity + cartQuantity;

            ZnodeLogging.LogMessage("selectedQuantity, cartQuantity and combinedQuantity: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SelectedQuantity = selectedQuantity, CartQuantity = cartQuantity, CombinedQuantity = combinedQuantity });
            //Re - swap the SKUs of configurable product to previous.
            SwapSkuOfConfigurableProduct(viewModel);
            if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(viewModel?.ConfigurableData?.CombinationErrorMessage,WebStore_Resources.ProductCombinationErrorMessage,StringComparison.InvariantCultureIgnoreCase))
            {
                if (!Between(combinedQuantity, Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                {
                    viewModel.InventoryMessage = string.Format(Admin_Resources.WarningSelectedQuantity, viewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity), viewModel.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                    viewModel.ShowAddToCart = false;
                    return;
                }
                else
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : Admin_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                    return;
                }
            }
            if (IsNotNull(viewModel?.Quantity))
            {
                bool AllowBackOrder = false;
                bool TrackInventory = false;

                if (inventorySetting?.Count > 0)
                    TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySettingCode);

                if (viewModel.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory)
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : Admin_Resources.TextOutofStock;
                    viewModel.ShowAddToCart = false;
                    return;
                }
                else if (viewModel.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                {
                    if (ValidateMinMaxQuantity(combinedQuantity,viewModel))
                    {
                        viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.BackOrderMessage) ? viewModel.BackOrderMessage : Admin_Resources.TextBackOrderMessage;
                        viewModel.ShowAddToCart = true;
                        return;
                    }
                }

                if (ValidateMinMaxQuantity(combinedQuantity, viewModel))
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) || string.Equals(viewModel?.ConfigurableData?.CombinationErrorMessage, WebStore_Resources.ProductCombinationErrorMessage, StringComparison.InvariantCultureIgnoreCase) ? viewModel.InStockMessage : Admin_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                }
            }
            else
            {
                viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : Admin_Resources.TextOutofStock;
                viewModel.ShowAddToCart = false;
                return;
            }
        }

        //Check Add on inventory.
        public virtual void CheckAddOnInventory(PublishProductsViewModel model, string addOnIds, decimal quantity)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddOnIds = addOnIds, Quantity = quantity });
            string[] selectedAddOn = !string.IsNullOrEmpty(addOnIds) ? addOnIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : null;
            bool AllowBackOrder = false;
            bool TrackInventory = false;

            if (selectedAddOn?.Length > 0)
            {
                foreach (string addOnSKU in selectedAddOn)
                {
                    AddOnViewModel addOn = null;
                    if (!string.IsNullOrEmpty(addOnSKU))
                        addOn =
                            model.AddOns.FirstOrDefault(
                                x => x.AddOnValues.Any(y => y.SKU == addOnSKU));

                    if (IsNotNull(addOn))
                    {
                        AddOnValuesViewModel addOnValue = addOn.AddOnValues.FirstOrDefault(y => y.SKU == addOnSKU);

                        if (IsNotNull(addOnValue))
                        {
                            decimal selectedQuantity = quantity > 0 ? quantity : Convert.ToDecimal(model.Attributes?.Value(ZnodeConstant.MinimumQuantity));

                            decimal cartQuantity = GetOrderedItemQuantity(addOnSKU);

                            decimal combinedQuantity = selectedQuantity + cartQuantity;

                            List<AttributesSelectValuesViewModel> inventorySetting = GetAddOnOutOfStockOptionsAttributeList(addOnValue);
                            if (inventorySetting?.Count > 0)
                            {
                                TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySetting.FirstOrDefault().Code);

                                if (addOnValue.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory)
                                {
                                    model.InventoryMessage = !string.IsNullOrEmpty(model.OutOfStockMessage) ? model.OutOfStockMessage : Admin_Resources.TextOutofStock;
                                    addOn.IsOutOfStock = true;
                                    model.ShowAddToCart = false;
                                    return;
                                }
                                else if (addOnValue.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                                {
                                    model.InventoryMessage = !string.IsNullOrEmpty(model.BackOrderMessage) ? model.BackOrderMessage : Admin_Resources.TextBackOrderMessage;
                                    model.ShowAddToCart = true;
                                    return;
                                }
                                if (!HelperUtility.Between(combinedQuantity, Convert.ToDecimal(addOnValue.Attributes?.Value(ZnodeConstant.MinimumQuantity)), Convert.ToDecimal(addOnValue.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                                {
                                    model.InventoryMessage = string.Format(Admin_Resources.WarningSelectedQuantity, addOnValue.Attributes?.Value(ZnodeConstant.MinimumQuantity), addOnValue.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                                    model.ShowAddToCart = false;
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(model.InventoryMessage) && model.ShowAddToCart)
            {
                model.InventoryMessage = !string.IsNullOrEmpty(model.InStockMessage) ? model.InStockMessage : Admin_Resources.TextInstock;
                model.ShowAddToCart = true;
            }
        }

        //Get product expands.
        public virtual ExpandCollection GetProductExpands()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Promotions);
            expands.Add(ExpandKeys.Inventory);
            expands.Add(ExpandKeys.ProductReviews);
            expands.Add(ExpandKeys.Pricing);
            expands.Add(ExpandKeys.ProductTemplate);
            expands.Add(ExpandKeys.AddOns);
            ZnodeLogging.LogMessage("Product expands: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands });
            return expands;
        }

        // Bind Countries to Address.
        protected virtual void BindCountriesToAddress(CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get portal associated country list.
            List<SelectListItem> countries = HelperMethods.GetPortalAssociatedCountries(createOrderViewModel.PortalId);
            ZnodeLogging.LogMessage("Countries list count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CountriesCount = countries.Count });
            //If the billing address is null then initialize it with new addressviewmodel.
            if (IsNull(createOrderViewModel.UserAddressDataViewModel.BillingAddress))
                createOrderViewModel.UserAddressDataViewModel.BillingAddress = new AddressViewModel();
            createOrderViewModel.UserAddressDataViewModel.BillingAddress.Countries = countries;

            //If the shipping address is null then initialize it with new addressviewmodel.
            if (IsNull(createOrderViewModel.UserAddressDataViewModel.ShippingAddress))
                createOrderViewModel.UserAddressDataViewModel.ShippingAddress = new AddressViewModel();
            createOrderViewModel.UserAddressDataViewModel.ShippingAddress.Countries = countries;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Filters and Espands to Get Publish Product List
        protected virtual void SetFiltersAndExpands(ref FilterCollection filters, ExpandCollection expands)
        {
            SetLocaleFilterIfNotPresent(ref filters);
            if (!filters.Any(x => string.Equals(x.FilterName, FilterKeys.ZnodeCategoryIds)))
                filters.Add(FilterKeys.ZnodeCategoryIds, FilterOperators.NotEquals, "0");

            filters.Add(FilterKeys.fromOrder.ToString(), FilterOperators.Equals, "true");
            filters.Add(FilterKeys.ProductIndex.ToString(), FilterOperators.Equals, ZnodeConstant.DefaultPublishProductIndex.ToString());

            expands.Add(ExpandKeys.Inventory);
            expands.Add(ExpandKeys.Pricing);
            expands.Add(ExpandKeys.Promotions);
            ZnodeLogging.LogMessage("Filters and expands: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Expands = expands });
        }

        //Set Payment details.
        public virtual void SetUsersPaymentDetails(int paymentSettingId, ShoppingCartModel model, bool isCOD = true)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            PaymentSettingModel paymentSetting = isCOD ? _paymentClient.GetPaymentSetting(paymentSettingId, false, new ExpandCollection { ZnodePaymentSettingEnum.ZnodePaymentType.ToString() })
                : _paymentAgent.GetPaymentSetting(paymentSettingId)?.ToModel<PaymentSettingModel>();

            if (IsNull(paymentSetting))
                paymentSetting = new PaymentSettingModel();

            string paymentName = string.Empty;
            if (IsNotNull(paymentSetting))
                paymentName = paymentSetting.PaymentTypeName;

            model.Payment = new PaymentModel
            {
                BillingAddress = model.BillingAddress,
                ShippingAddress = model.ShippingAddress,
                PaymentSetting = paymentSetting,
                PaymentName = paymentName,
                PaymentDisplayName = paymentSetting?.PaymentDisplayName,
                IsPreAuthorize = paymentSetting.PreAuthorize,

            };
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        // Get shipping details.
        protected virtual void GetUsersShippingDetails(CreateOrderViewModel createOrderViewModel, ShoppingCartModel model)
        {
            model.Shipping = _shippingAgent.GetShippingById(model.ShippingId).ToModel<OrderShippingModel>();
            model.Shipping.ShippingHandlingCharge = model?.ShippingHandlingCharges == 0 ? model.Shipping.ShippingHandlingCharge : model.ShippingHandlingCharges;
            model.Shipping.AccountNumber = createOrderViewModel?.AccountNumber;
            model.Shipping.ShippingMethod = createOrderViewModel?.ShippingMethod;
        }

        //Get all address list of customer.
        protected virtual void GetUsersAllAddressList(CreateOrderViewModel createOrderViewModel, ShoppingCartModel model)
        {
            FilterCollection filters = new FilterCollection();

            //Set user id in filter for getting address list.
            HelperMethods.SetUserIdFilters(filters, createOrderViewModel.UserId);

            //Get user address list.
            ZnodeLogging.LogMessage("Filters to get address list: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            AddressListModel addressList = _customerClient.GetAddressList(GetExpands(), filters, null, null, null);
            model.UserDetails.Addresses = addressList?.AddressList?.Count > 0 ? addressList.AddressList : new List<AddressModel>();
        }

        //Multiple shipping address.
        protected virtual void SetMultipleShippingAddress(CreateOrderViewModel createOrderViewModel)
        {
            List<OrderShipmentModel> multipleShipToAddress = new List<OrderShipmentModel>();
            multipleShipToAddress.Add(new OrderShipmentModel { AddressId = createOrderViewModel.UserAddressDataViewModel.ShippingAddress.AddressId, Quantity = 1 });
            ZnodeLogging.LogMessage("MultipleShippingAddress list count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { MultipleShipToAddressCount = multipleShipToAddress });
        }

        //Set Shipping billing address.
        protected virtual void SetUserShippingBillingAddress(CreateOrderViewModel createOrderViewModel, ShoppingCartModel model)
        {
            model.BillingAddress = createOrderViewModel.UserAddressDataViewModel.BillingAddress.ToModel<AddressModel>();
            model.ShippingAddress = createOrderViewModel.UserAddressDataViewModel.ShippingAddress.ToModel<AddressModel>();
            ZnodeLogging.LogMessage("BillingAddressId and ShippingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddressId = model?.BillingAddress?.AddressId, ShippingAddressId = model?.ShippingAddress?.AddressId });
        }

        //Set user details.
        protected virtual void SetUserDetails(CreateOrderViewModel createOrderViewModel, ShoppingCartModel model)
        {
            SetUserInformation(model);
            model.UserDetails.Email = createOrderViewModel.UserAddressDataViewModel.Email ?? model.UserDetails.Email;
            model.AdditionalInstructions = createOrderViewModel.AdditionalInstructions;
            model.PurchaseOrderNumber = createOrderViewModel.PurchaseOrderNumber;
            model.JobName = createOrderViewModel.JobName;
        }

        public virtual void SetUserInformation(ShoppingCartModel model)
        {
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = (int)model.UserId });
            model.UserDetails = _userClient.GetUserAccountData((int)model.UserId, new ExpandCollection { ExpandKeys.Profiles });
            model.UserDetails.UserId = (int)model.UserId;
            model.UserDetails.ProfileId = (model.UserDetails.Profiles?.Where(x => x.IsDefault.GetValueOrDefault())?.FirstOrDefault()?.ProfileId).GetValueOrDefault();
            model.ProfileId = model.UserDetails.ProfileId;
        }

        //Create/update customer address.
        protected virtual void CreateUpdateUserAddress(CreateOrderViewModel createOrderViewModel)
        {
            createOrderViewModel.UserAddressDataViewModel.UserId = createOrderViewModel.UserId;
            createOrderViewModel.UserAddressDataViewModel.UserShippingAddressId = createOrderViewModel.UserShippingAddressId;
            createOrderViewModel.UserAddressDataViewModel.UserBillingAddressId = createOrderViewModel.UserBillingAddressId;

            CreateUpdateCustomerAddress(createOrderViewModel.UserAddressDataViewModel);
        }

        //Get shipping details by shipping option.
        protected virtual void GetShippingDetailsByShippingId(CreateOrderViewModel createOrderViewModel, ShoppingCartModel shoppingCartModel)
        {
            //Get ShippingId and set to shipping option.
            createOrderViewModel.ShippingId = createOrderViewModel.ShippingListViewModel.ShippingList.FirstOrDefault()?.ShippingId;

            //Get shipping details by shipping option.
            ShippingViewModel shippingDetails = createOrderViewModel.ShippingId > 0 ? _shippingAgent.GetShippingById(createOrderViewModel.ShippingId.GetValueOrDefault()) : new ShippingViewModel();

            shoppingCartModel.Shipping = IsNotNull(shippingDetails) ? shippingDetails.ToModel<OrderShippingModel>() : new OrderShippingModel();
        }

        //Set Ordered Information of user.
        protected virtual void SetOrderedInformationOfUser(OrderModel orderDetails, CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            createOrderViewModel.OrderId = orderDetails.OmsOrderId;
            //Get user details.
            createOrderViewModel.UserAddressDataViewModel = GetCustomerDetailsForUpdateOrder(orderDetails);
            createOrderViewModel.UserId = orderDetails.UserId;

            //Set store name and portal id.
            createOrderViewModel.StoreName = orderDetails.StoreName;
            createOrderViewModel.PortalId = orderDetails.PortalId;
            ZnodeLogging.LogMessage("OrderId, UserId and PortalId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderId = createOrderViewModel?.OrderId, UserId = createOrderViewModel?.UserId, PortalId = createOrderViewModel?.PortalId });
            //Set Customer name.
            createOrderViewModel.CustomerName = createOrderViewModel.UserAddressDataViewModel.FullName;

            createOrderViewModel.OrderNotes = orderDetails.OrderNotes?.ToViewModel<OrderNotesViewModel>().ToList();

            //Get Customer Associated Profile List.
            ProfileListModel profileList = GetCustomerAssociatedProfileList(createOrderViewModel);

            //Shipping methods
            createOrderViewModel.ShippingListViewModel = BindShippingList(createOrderViewModel.UserId);
            createOrderViewModel.ShippingListViewModel.SelectedShippingCode = orderDetails.ShippingId;
            createOrderViewModel.ShippingListViewModel.OrderID = orderDetails.OmsOrderId;
            createOrderViewModel.ShippingListViewModel.ShippingId = orderDetails.ShippingId;
            //Payment option list
            createOrderViewModel.PaymentSettingViewModel = new PaymentSettingViewModel() { PaymentTypeList = BindPaymentList(orderDetails.UserId, orderDetails.PortalId) };
            createOrderViewModel.PaymentSettingViewModel.PaymentTypeId = orderDetails.PaymentSettingId.GetValueOrDefault();

            //Set personalize attribute into cartviewmodel
            SetPersonalizeForShoppingCart(orderDetails.ShoppingCartModel, orderDetails.OrderLineItems);

            //Set cart details.
            createOrderViewModel.CartViewModel = orderDetails.ShoppingCartModel.ToViewModel<CartViewModel>();
            createOrderViewModel.PurchaseOrderNumber = orderDetails.PurchaseOrderNumber;
            createOrderViewModel.OrderNumber = orderDetails.OrderNumber;

            //Set shipping details in orderModel and save in session.
            orderDetails.ShoppingCartModel.Shipping = _shippingAgent.GetShippingById(createOrderViewModel.ShippingListViewModel.SelectedShippingCode).ToModel<OrderShippingModel>();

            orderDetails.ShoppingCartModel.ShippingAddress = createOrderViewModel.UserAddressDataViewModel.ShippingAddress.ToModel<AddressModel>();

            //Set personalize attribute into ShoppingCartModel
            SetPersonalizeForShoppingCart(orderDetails.ShoppingCartModel, orderDetails.OrderLineItems);

            SaveCartModelInSession(createOrderViewModel?.UserId, orderDetails.ShoppingCartModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Ordered Information of user in case of reorder.
        protected virtual void SetOrderedInformationOfUserInReorder(OrderModel orderDetails, CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            createOrderViewModel.OrderId = 0;
            //Get user details.
            createOrderViewModel.UserAddressDataViewModel = GetCustomerDetailsForUpdateOrder(orderDetails);
            createOrderViewModel.UserId = orderDetails.UserId;

            //Set store name and portal id.
            createOrderViewModel.StoreName = orderDetails.StoreName;
            createOrderViewModel.PortalId = orderDetails.PortalId;

            //Set Customer name.
            createOrderViewModel.CustomerName = createOrderViewModel.UserAddressDataViewModel.FullName;

            createOrderViewModel.OrderNotes = orderDetails.OrderNotes.ToViewModel<OrderNotesViewModel>().ToList();

            //Get Customer Associated Profile List.
            ProfileListModel profileList = GetCustomerAssociatedProfileList(createOrderViewModel);

            //Shipping methods
            createOrderViewModel.ShippingListViewModel = BindShippingList(createOrderViewModel.UserId);
            createOrderViewModel.ShippingListViewModel.SelectedShippingCode = orderDetails.ShippingId;
            createOrderViewModel.ShippingListViewModel.OrderID = 0;
            createOrderViewModel.ShippingListViewModel.ShippingId = orderDetails.ShippingId;
            //Payment option list
            createOrderViewModel.PaymentSettingViewModel = new PaymentSettingViewModel() { PaymentTypeList = BindPaymentList(orderDetails.UserId, orderDetails.PortalId) };
            createOrderViewModel.PaymentSettingViewModel.PaymentTypeId = orderDetails.PaymentSettingId.GetValueOrDefault();

            //Set personalize attribute into cartviewmodel
            SetPersonalizeForShoppingCart(orderDetails.ShoppingCartModel, orderDetails.OrderLineItems);

            //Set cart details.
            createOrderViewModel.CartViewModel = orderDetails.ShoppingCartModel.ToViewModel<CartViewModel>();
            createOrderViewModel.PurchaseOrderNumber = orderDetails.PurchaseOrderNumber;
            createOrderViewModel.OrderNumber = string.Empty;

            //Set shipping details in orderModel and save in session.
            orderDetails.ShoppingCartModel.Shipping = _shippingAgent.GetShippingById(createOrderViewModel.ShippingListViewModel.SelectedShippingCode).ToModel<OrderShippingModel>();

            orderDetails.ShoppingCartModel.ShippingAddress = createOrderViewModel.UserAddressDataViewModel.ShippingAddress.ToModel<AddressModel>();

            orderDetails.OmsOrderId = 0;

            orderDetails.ShoppingCartModel.OmsOrderId = 0;

            orderDetails.OrderNumber = string.Empty;

            //Set personalize attribute into ShoppingCartModel
            SetPersonalizeForShoppingCart(orderDetails.ShoppingCartModel, orderDetails.OrderLineItems);

            SaveCartModelInSession(createOrderViewModel?.UserId, orderDetails.ShoppingCartModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Ordered Information of user.
        public virtual void SetOrderedInformationOfUser(AccountQuoteModel orderDetails, CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get user details.
            createOrderViewModel.UserAddressDataViewModel = GetCustomerDetailsForUpdateOrder(orderDetails);
            createOrderViewModel.UserId = orderDetails.UserId;
            createOrderViewModel.AccountId = createOrderViewModel.UserAddressDataViewModel.AccountId.GetValueOrDefault();
            //Set store name and portal id.
            createOrderViewModel.StoreName = new StoreAgent(GetClient<PortalClient>(), GetClient<EcommerceCatalogClient>(), GetClient<ThemeClient>(), GetClient<DomainClient>(), GetClient<PriceClient>(), GetClient<OrderStateClient>(),
                GetClient<ProductReviewStateClient>(), GetClient<PortalProfileClient>(), GetClient<WarehouseClient>(), GetClient<CSSClient>(), GetClient<ManageMessageClient>(), GetClient<ContentPageClient>(), GetClient<TaxClassClient>(),
                GetClient<PaymentClient>(), GetClient<ShippingClient>(), GetClient<PortalCountryClient>(), GetClient<TagManagerClient>(), GetClient<GeneralSettingClient>()).GetStore(orderDetails.PortalId)?.StoreName;

            createOrderViewModel.PortalId = orderDetails.PortalId;

            //Set Customer name.
            createOrderViewModel.CustomerName = createOrderViewModel.UserAddressDataViewModel.FullName;

            //Bind Countries to Address.
            BindCountriesToAddress(createOrderViewModel);

            //Get Customer Associated Profile List.
            ProfileListModel profileList = GetCustomerAssociatedProfileList(createOrderViewModel);

            //Shipping methods
            createOrderViewModel.ShippingListViewModel = BindShippingList(createOrderViewModel.UserId);
            createOrderViewModel.ShippingListViewModel.SelectedShippingCode = orderDetails.ShippingId;
            createOrderViewModel.ShippingListViewModel.ShippingId = orderDetails.ShippingId;

            //Payment option list
            createOrderViewModel.PaymentSettingViewModel = new PaymentSettingViewModel() { PaymentTypeList = BindPaymentList(orderDetails.UserId, orderDetails.PortalId) };

            //Set cart details.
            createOrderViewModel.CartViewModel = orderDetails.ShoppingCart.ToViewModel<CartViewModel>();
            createOrderViewModel.CartViewModel.UserId = orderDetails.UserId;

            //Set additional notes.
            createOrderViewModel.OrderNotes = orderDetails.OrderNotes?.ToViewModel<OrderNotesViewModel>()?.ToList();

            //Set shipping details in orderModel and save in session.
            orderDetails.ShoppingCart.Shipping = _shippingAgent.GetShippingById(createOrderViewModel.ShippingListViewModel.SelectedShippingCode).ToModel<OrderShippingModel>();
            if (IsNotNull(orderDetails.ShoppingCart.Shipping))
                orderDetails.ShoppingCart.Shipping.ShippingCountryCode = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.CountryName;
            orderDetails.ShoppingCart.IsQuoteOrder = true;
            orderDetails.ShoppingCart.OmsQuoteId = orderDetails.OmsQuoteId;
            orderDetails.ShoppingCart.UserId = orderDetails.UserId;

            orderDetails.ShoppingCart.CurrencyCode = string.IsNullOrEmpty(orderDetails.ShoppingCart.CurrencyCode) ? DefaultSettingHelper.DefaultCurrency : orderDetails.ShoppingCart.CurrencyCode;
            orderDetails.ShoppingCart.CultureCode = string.IsNullOrEmpty(orderDetails.ShoppingCart.CultureCode) ? DefaultSettingHelper.DefaultCulture : orderDetails.ShoppingCart.CultureCode;
            SaveCartModelInSession(createOrderViewModel?.UserId, orderDetails.ShoppingCart);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Expands For Order Details.
        protected virtual ExpandCollection SetExpandsForOrderDetails()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.OrderLineItems);
            expands.Add(ExpandKeys.Store);
            expands.Add(ExpandKeys.PaymentType);
            expands.Add(ExpandKeys.OmsOrderState);
            expands.Add(ExpandKeys.ShoppingCart);
            expands.Add(ExpandKeys.OrderNotes);
            ZnodeLogging.LogMessage("Expands for order details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands });
            return expands;
        }

        //To Set Personalize attribute in ShoppingCartModel.
        protected virtual void SetPersonalizeForShoppingCart(ShoppingCartModel shoppingCartModel, List<OrderLineItemModel> orderLineItemModel)
        {
            List<OrderLineItemModel> orderLineModel = orderLineItemModel?.Where(w => w.ParentOmsOrderLineItemsId == null && w.OrderLineItemState != ZnodeOrderStatusEnum.RETURNED.ToString() && w.PersonaliseValueList.Count > 0).ToList();
            ZnodeLogging.LogMessage("OrderLineItemModel list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderLineModelCount = orderLineModel?.Count });
            if (IsNotNull(orderLineModel))
                foreach (ShoppingCartItemModel shoppingCart in shoppingCartModel.ShoppingCartItems)
                {
                    shoppingCart.PersonaliseValuesList = GetPersonaliseAttributeById(shoppingCart.OmsOrderLineItemsId, orderLineModel);
                    shoppingCart.PersonaliseValuesDetail = orderLineItemModel.Where(x => x.OmsOrderLineItemsId == shoppingCart.OmsOrderLineItemsId)?.FirstOrDefault()?.PersonaliseValuesDetail;
                    shoppingCart.GroupId = orderLineItemModel.Where(x => x.OmsOrderLineItemsId == shoppingCart.OmsOrderLineItemsId).Select(y => y.GroupId).FirstOrDefault();
                }
            ZnodeLogging.LogMessage("ShoppingCartItems count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShoppingCartItemsCount = shoppingCartModel?.ShoppingCartItems?.Count });
        }

        //Get orderlineItem by Sku.
        protected virtual Dictionary<string, object> GetPersonaliseAttributeById(int omsOrderLineItemsId, List<OrderLineItemModel> orderLineItemModel)
         => orderLineItemModel.Where(x => x.OmsOrderLineItemsId == omsOrderLineItemsId)?.FirstOrDefault()?.PersonaliseValueList;

        //Get attribute values and code.
        protected virtual Dictionary<string, string> GetAttributeValues(string codes, string values)
        {
            //Attribute Code And Value
            string[] Codes = codes.Split(',');
            string[] Values = values.Split(',');
            Dictionary<string, string> SelectedAttributes = new Dictionary<string, string>();

            //Add code and value pair
            for (int i = 0; i < Codes.Length; i++)
                SelectedAttributes.Add(Codes[i], Values[i]);
            return SelectedAttributes;
        }

        //Map all data required to submit order to shopping cart model
        protected virtual void SetShoppingCartModel(ShoppingCartModel model, CreateOrderViewModel createOrderViewModel)
        {
            SetShoppingCart(model, createOrderViewModel);

            //Set Payment details.
            SetUsersPaymentDetails(createOrderViewModel.PaymentTypeId, model);

            //Set order quote true if quote id is greater than 0.
            model.IsQuoteOrder = model.OmsQuoteId > 0;

            model.IsFromAdminOrder = true;
        }

        protected virtual void SetShoppingCartModelForQuote(ShoppingCartModel model, CreateOrderViewModel createOrderViewModel)
            => SetShoppingCart(model, createOrderViewModel);

        protected virtual void SetShoppingCart(ShoppingCartModel model, CreateOrderViewModel createOrderViewModel)
        {
            SetShoppingCartDetails(model, createOrderViewModel);

            //Set user details.
            SetUserDetails(createOrderViewModel, model);


            // Get shipping details.
            GetUsersShippingDetails(createOrderViewModel, model);
        }

        //Set Shopping Cart Details.
        protected virtual void SetShoppingCartDetails(ShoppingCartModel model, CreateOrderViewModel createOrderViewModel)
        {
            model.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            model.OrderDate = DateTime.Now;
            model.AdditionalInstructions = createOrderViewModel.AdditionalInstructions;
            model.AdditionalNotes = createOrderViewModel.AdditionalNotes;
            model.PurchaseOrderNumber = createOrderViewModel.PurchaseOrderNumber;
            model.PublishStateId = (byte)ZnodePublishStatesEnum.PRODUCTION;
            model.InHandDate = createOrderViewModel.InHandDate;
            model.ShippingConstraintCode = createOrderViewModel.ShippingConstraintCode;
        }

        //Set created by and modified by user.
        public virtual void SetCreatedByUser(int? userId)
        {
            if (userId > 0)
            {
                int loginId = _shoppingCartClient.UserId;
                int selectedUserId = userId.GetValueOrDefault();
                Int32 sessionUserId = GetSessionUser();

                _shoppingCartClient.LoginAs = sessionUserId;
                _shoppingCartClient.UserId = selectedUserId;

                _orderClient.LoginAs = sessionUserId;
                _orderClient.UserId = selectedUserId;
            }
        }

        //Get Customer Address by Address Id
        protected virtual AddressModel GetCustomerAddress(int addressId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressId.ToString()));

            //expand for address.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString());

            ZnodeLogging.LogMessage("Filters and Expands to get customer address: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Expands = expands });
            return _customerClient.GetCustomerAddress(expands, filters);
        }

        //Get account address by address id
        protected virtual AddressModel GetAccountAddress(int addressId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressId.ToString()));

            //expand for address.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString());

            ZnodeLogging.LogMessage("Filters and Expands to get account address: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Expands = expands });
            return _accountClient.GetAccountAddress(expands, filters);
        }

        //Map shopping cart Model with Submit Payment ViewModel.
        protected virtual void MapShoppingCartModel(ShoppingCartModel shoppingCart, SubmitPaymentViewModel submitPaymentViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            shoppingCart.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            shoppingCart.PublishedCatalogId = !Equals(submitPaymentViewModel.PortalCatalogId, 0) ? submitPaymentViewModel.PortalCatalogId : shoppingCart.PublishedCatalogId;
            shoppingCart.PortalId = !Equals(submitPaymentViewModel.PortalId, 0) ? submitPaymentViewModel.PortalId : shoppingCart.PortalId;
            shoppingCart.OrderDate = DateTime.Now;
            submitPaymentViewModel.UserId = !Equals(submitPaymentViewModel.UserId, 0) ? submitPaymentViewModel.UserId : shoppingCart.UserDetails.UserId;
            submitPaymentViewModel.ShippingAddressId = !Equals(submitPaymentViewModel.ShippingAddressId, 0) ? submitPaymentViewModel.ShippingAddressId : shoppingCart.ShippingAddress?.AddressId ?? 0;
            submitPaymentViewModel.BillingAddressId = !Equals(submitPaymentViewModel.BillingAddressId, 0) ? submitPaymentViewModel.BillingAddressId : shoppingCart.BillingAddress?.AddressId ?? 0;
            shoppingCart.CreditCardNumber = submitPaymentViewModel?.CreditCardNumber;
            shoppingCart.CardType = submitPaymentViewModel?.CardType;
            shoppingCart.CreditCardExpMonth = submitPaymentViewModel?.CreditCardExpMonth;
            shoppingCart.CreditCardExpYear = submitPaymentViewModel?.CreditCardExpYear;
            shoppingCart.Shipping.AccountNumber = submitPaymentViewModel?.AccountNumber;
            shoppingCart.Shipping.ShippingMethod = submitPaymentViewModel?.ShippingMethod;
            shoppingCart.InHandDate = submitPaymentViewModel?.InHandDate;
            shoppingCart.JobName = submitPaymentViewModel.JobName;
            shoppingCart.ShippingConstraintCode = submitPaymentViewModel.ShippingConstraintCode;
            shoppingCart.CardType = submitPaymentViewModel.CardType;
            shoppingCart.CcCardExpiration = submitPaymentViewModel.CardExpiration;
            shoppingCart.TransactionId = submitPaymentViewModel.TransactionId;
            shoppingCart.UserId = submitPaymentViewModel.UserId;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set ShoppingCartModel And Calculate.
        protected virtual void SetShoppingCartModelAndCalculate(AddressViewModel addressViewModel, CreateOrderViewModel createOrderViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get Cart from session.
            ShoppingCartModel cart = GetCartModelFromSession(addressViewModel?.UserId, addressViewModel.IsQuote);

            cart.Shipping.ShippingCountryCode = createOrderViewModel.UserAddressDataViewModel.ShippingAddress.CountryName;

            //Get shipping details from session.
            AddressViewModel shippingAddress = createOrderViewModel.UserAddressDataViewModel.ShippingAddress;
            AddressViewModel billingAddress = createOrderViewModel.UserAddressDataViewModel.BillingAddress;

            cart.ShippingAddress = shippingAddress?.ToModel<AddressModel>();
            cart.BillingAddress = billingAddress?.ToModel<AddressModel>();
            cart.ShoppingCartItems.ForEach(item => item.InsufficientQuantity = false);
            ZnodeLogging.LogMessage("ShippingCountryCode, ShippingAddress with Id and ShoppingCartItems count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingCountryCode = cart?.Shipping?.ShippingCountryCode, ShippingAddressId = cart?.ShippingAddress?.AddressId, ShoppingCartItemsCount = cart?.ShoppingCartItems?.Count });

            //Set login user and selected user in cache.
            SetCreatedByUser(cart.UserId);

            CartViewModel cartViewModel = new CartViewModel();
            if (cart?.ShoppingCartItems?.Count > 0 && addressViewModel.IsShippingAddressChange)
            {
                cart.Shipping = null;
                cart = _shoppingCartClient.Calculate(cart);
            }
            //save Shopping cart in Session
            SaveCartModelInSession(addressViewModel?.UserId, cart, addressViewModel.IsQuote);

            createOrderViewModel.CartViewModel = cart.ToViewModel<CartViewModel>();
        }

        //Set ShoppingCartModel And Calculate for manage.
        protected virtual void SetShoppingCartModelAndCalculateForManage(AddressViewModel addressViewModel, CreateOrderViewModel createOrderViewModel, string fromBillingShipping = "", int omsOrderId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get Cart from session.
            ZnodeLogging.LogMessage("OmsOrderId to get cart from session: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = omsOrderId });
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId);

            ShoppingCartModel cart = orderModel?.ShoppingCartModel;

            if (!string.IsNullOrEmpty(fromBillingShipping))
            {
                if (Equals(fromBillingShipping.ToLower(), Admin_Resources.LabelBilling.ToLower()))
                {
                    orderModel.OrderHistory.Remove(ZnodeConstant.OrderBillingAddress);
                    orderModel.OrderHistory.Add(ZnodeConstant.OrderBillingAddress, Admin_Resources.UpdateBillingAddress);

                    //Get shipping details from session.
                    AddressViewModel billingAddress = createOrderViewModel.UserAddressDataViewModel.BillingAddress;
                    ZnodeLogging.LogMessage("BillingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddressId = billingAddress.AddressId });
                    cart.BillingAddress = billingAddress?.ToModel<AddressModel>();
                    orderModel.BillingAddress = billingAddress?.ToModel<AddressModel>();
                    addressViewModel.SelectedBillingId = billingAddress.AddressId;

                    CheckDefaultBillingShippingAddress(addressViewModel, orderModel, cart, billingAddress);
                }
                else
                {
                    orderModel.OrderHistory.Remove(ZnodeConstant.OrderShippingAddress);
                    orderModel.OrderHistory.Add(ZnodeConstant.OrderShippingAddress, Admin_Resources.UpdateShippingAddress);
                    cart.IsShippingRecalculate = IsShippingRecalculateRequired(addressViewModel, cart.ShippingAddress?.StateName, cart.ShippingAddress?.PostalCode);
                    if(cart.IsShippingRecalculate)
                    {
                        cart.IsShippingDiscountRecalculate = true;
                        cart.InvalidOrderLevelShippingPromotion = new List<PromotionModel>();
                    }
                    cart.Shipping.ShippingCountryCode = createOrderViewModel.UserAddressDataViewModel.ShippingAddress.CountryName;
                    //Get shipping details from session.
                    AddressViewModel shippingAddress = createOrderViewModel.UserAddressDataViewModel.ShippingAddress;
                    ZnodeLogging.LogMessage("ShippingCountryCode and ShippingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingCountryCode = cart?.Shipping?.ShippingCountryCode, shippingAddress = shippingAddress?.AddressId });
                    cart.ShippingAddress = shippingAddress?.ToModel<AddressModel>();
                    orderModel.ShippingAddress = shippingAddress?.ToModel<AddressModel>();
                    addressViewModel.SelectedShippingId = shippingAddress.SelectedShippingId;

                    CheckDefaultBillingShippingAddress(addressViewModel, orderModel, cart, shippingAddress);
                }
            }

            cart.ShoppingCartItems.ForEach(item => item.InsufficientQuantity = false);

            //Set login user and selected user in cache.
            SetCreatedByUser(cart.UserId);

            CartViewModel cartViewModel = new CartViewModel();
            if (cart?.ShoppingCartItems?.Count > 0)
            {
                cart = GetCalculatedShoppingCartForEditOrder(cart);
            }

            orderModel.ShoppingCartModel = cart;

            cart.IsShippingRecalculate = false;
            //save Shopping cart in Session
            SaveInSession(AdminConstants.OMSOrderSessionKey + omsOrderId, orderModel);

            UserModel userAccountData = _userClient.GetUserAccountData(orderModel.UserId);
            ZnodeLogging.LogMessage("User account data with user Id: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userAccountData?.UserId });
            if (HelperUtility.IsNotNull(userAccountData))
                orderModel.BillingAddress.IsGuest = string.IsNullOrEmpty(userAccountData.AspNetUserId) ? true : false;

            //Map Customer name
            createOrderViewModel.CustomerName = string.IsNullOrEmpty(userAccountData?.AspNetUserId) ? orderModel?.BillingAddress?.FirstName + " " + orderModel?.BillingAddress?.LastName : orderModel?.FirstName + " " + orderModel?.LastName;

            createOrderViewModel.UserName = orderModel?.UserName;
            createOrderViewModel.UserId = orderModel.UserId;

            createOrderViewModel.CartViewModel = GetCartOrderStatusList(cart, orderModel.TrackingUrl);
            createOrderViewModel.CartViewModel.OrderState = orderModel.OrderState;
            if (!string.IsNullOrEmpty(cart.Shipping.ResponseMessage))
            {
                createOrderViewModel.CartViewModel.HasError = true;
                createOrderViewModel.CartViewModel.ShippingErrorMessage = cart.Shipping.ResponseMessage;
            }
            ZnodeLogging.LogMessage("CreateOrderViewModel with PortalId, CatalogId, AccountId and UserId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = createOrderViewModel?.PortalId, CatalogId = createOrderViewModel?.CatalogId, AccountId = createOrderViewModel?.AccountId, UserId = createOrderViewModel?.UserId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        protected virtual void CheckDefaultBillingShippingAddress(AddressViewModel addressViewModel, OrderModel orderModel, ShoppingCartModel cart, AddressViewModel updatedAddress)
        {
            if (Equals(addressViewModel.SelectedBillingId, addressViewModel.SelectedShippingId))
            {
                cart.ShippingAddress = updatedAddress?.ToModel<AddressModel>();
                orderModel.ShippingAddress = updatedAddress?.ToModel<AddressModel>();
                cart.BillingAddress = updatedAddress?.ToModel<AddressModel>();
                orderModel.BillingAddress = updatedAddress?.ToModel<AddressModel>();
            }
        }

        //Set shipping and billing Address To CreateOrderViewModel.
        protected virtual void SetAddressToCreateOrderViewModel(AddressViewModel addressViewModel, CreateOrderViewModel createOrderViewModel, AddressViewModel updatedAddress, AddressListModel addressList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get shipping and billing address id as per IsShippingAddressChange flag.
            int shippingAddressId = addressViewModel.IsShippingAddressChange ? updatedAddress.AddressId : addressViewModel.SelectedShippingId;
            int billingAddressId = !addressViewModel.IsShippingAddressChange ? updatedAddress.AddressId : addressViewModel.SelectedBillingId.Equals(0) ? shippingAddressId : addressViewModel.SelectedBillingId;
            //Get portal associated country dropdown.
            List<SelectListItem> countryList = HelperMethods.GetPortalAssociatedCountries(createOrderViewModel.PortalId);
            createOrderViewModel.UserAddressDataViewModel.ShippingAddress = (addressList?.AddressList?.Where(w => w.AddressId == shippingAddressId).FirstOrDefault())?.ToViewModel<AddressViewModel>()
                                                                             ?? (addressViewModel.IsShippingAddressChange ? updatedAddress : addressViewModel);
            createOrderViewModel.UserAddressDataViewModel.BillingAddress = (addressList?.AddressList?.FirstOrDefault(w => w.AddressId == billingAddressId))?.ToViewModel<AddressViewModel>()
                                                                            ?? (!addressViewModel.IsShippingAddressChange ? updatedAddress :
                                                                                (addressViewModel.SelectedBillingId.Equals(0) ? createOrderViewModel.UserAddressDataViewModel.ShippingAddress : addressViewModel));
            createOrderViewModel.UserAddressDataViewModel.UserId = addressViewModel.UserId;
            createOrderViewModel.UserAddressDataViewModel.ShippingAddress.Countries = countryList;
            createOrderViewModel.UserAddressDataViewModel.BillingAddress.Countries = countryList;
            createOrderViewModel.UserAddressDataViewModel.UsersAddressNameList = OrderViewModelMap.ToListItems(addressList?.AddressList);
            ZnodeLogging.LogMessage("ShippingAddressId and BillingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingAddressId = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.AddressId, BillingAddressId = createOrderViewModel?.UserAddressDataViewModel?.BillingAddress?.AddressId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get Address List Of User And Account.
        protected virtual AddressListModel GetAddressListOfUserAndAccount(int userId, int accountId)
        {
            ZnodeLogging.LogMessage("UserId and accountId to get address list of user and account:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId, AccountId = accountId });
            FilterCollection filters = new FilterCollection();
            if (accountId > 0)
            {
                //Set filters for account id.
                HelperMethods.SetAccountIdFilters(filters, accountId);
                return _accountClient.GetAddressList(GetExpands(), filters, null, null, null);
            }
            else
            {
                //Set filters for user id.
                HelperMethods.SetUserIdFilters(filters, userId);
                return _customerClient.GetAddressList(GetExpands(), filters, null, null, null);
            }
        }

        protected virtual AddressListModel GetAddressListByAddressId(int addressId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressId.ToString()));
            AddressListModel addresslistmodel = _addressClient.GetAddressList(filters, null, null, null);
            if (addresslistmodel?.AddressList?.Count() > 0)
                addresslistmodel.AddressList.FirstOrDefault().DontAddUpdateAddress = true;

            return addresslistmodel;
        }

        //Set Expand For Order Details.
        protected virtual ExpandCollection SetExpandForOrderDetails(bool isManageOrder = false)
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsPaymentState.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString());
            if (!isManageOrder)
                expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrder.ToString());
            expands.Add(ExpandKeys.ZnodeShipping);
            expands.Add(ExpandKeys.ZnodeUser);
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsHistories.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString());
            expands.Add(ExpandKeys.IsFromOrderReceipt);
            ZnodeLogging.LogMessage("ExpandForOrderDetails: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands });
            return expands;
        }

        //Get product filter.
        public virtual FilterCollection GetProductFilters(int portalId, int localeId, int catalogId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString());
            if(localeId > 0)
                filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());
            ZnodeLogging.LogMessage("Product filters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ProductFilters = filters });
            return filters;
        }

        //Set filter for User Id.
        protected virtual void SetUserIdFilter(FilterCollection filters, int userId)
        {
            //Checking For UserId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeUserEnum.UserId.ToString()))
                //If UserId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());

            //Add New UserId Into filters
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            ZnodeLogging.LogMessage("UserIdFilter: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserIdFilter = filters });
        }

        //Bind required data to order list view model.
        protected virtual OrdersListViewModel BindDataToViewModel(OrdersListModel ordersListModel, OrdersListViewModel ordersListViewModel, int accountId, int userId)
        {
            //Get Order State List.
            ordersListViewModel.CustomerName = ordersListModel.CustomerName;
            ordersListViewModel.AccountId = accountId;
            ordersListViewModel.UserId = userId;
            return ordersListViewModel;
        }

        //Set Ordered Shipping Address with user's shipping address.
        protected virtual void SetOrderedShippingAddress(OrderModel orderModel, UserAddressDataViewModel userModel, AddressListModel addressList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderShipmentModel orderShipment = orderModel?.OrderLineItems?.FirstOrDefault()?.ZnodeOmsOrderShipment;

            if (IsNotNull(orderShipment))
            {
                userModel.ShippingAddress.FirstName = orderShipment.ShipToFirstName;
                userModel.ShippingAddress.LastName = orderShipment.ShipToLastName;
                userModel.ShippingAddress.CompanyName = orderShipment.ShipToCompanyName;
                userModel.ShippingAddress.Address1 = orderShipment.ShipToStreet1;
                userModel.ShippingAddress.Address2 = orderShipment.ShipToStreet2;
                userModel.ShippingAddress.CityName = orderShipment.ShipToCity;
                userModel.ShippingAddress.StateName = orderShipment.ShipToStateCode;
                userModel.ShippingAddress.PostalCode = orderShipment.ShipToPostalCode;
                userModel.ShippingAddress.CountryName = orderShipment.ShipToCountry;
                userModel.ShippingAddress.PhoneNumber = orderShipment.ShipToPhoneNumber;
                userModel.ShippingAddress.IsDefaultShipping = true;
                userModel.ShippingAddress.AddressId = orderShipment.AddressId;
                userModel.ShippingAddress.DisplayName = addressList?.AddressList?.Where(w => w.AddressId == orderShipment.AddressId).FirstOrDefault()?.DisplayName;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Ordered Shipping Address with user's shipping address.
        protected virtual void SetOrderedShippingAddress(AccountQuoteModel orderModel, UserAddressDataViewModel userModel)
        {
            if (IsNotNull(orderModel?.ShippingAddressModel))
            {
                ZnodeLogging.LogMessage("ShippingAddressModel with Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingAddressId = orderModel?.ShippingAddressModel?.AddressId });
                userModel.ShippingAddress = orderModel.ShippingAddressModel.ToViewModel<AddressViewModel>();
            }
        }

        //Set Ordered Billing Address with user's Billing address.
        protected virtual void SetOrderedBillingAddress(OrderModel orderModel, UserAddressDataViewModel userModel, AddressListModel addressList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(orderModel))
            {
                userModel.BillingAddress.FirstName = orderModel.BillingAddress.FirstName;
                userModel.BillingAddress.LastName = orderModel.BillingAddress.LastName;
                userModel.BillingAddress.CompanyName = orderModel.BillingAddress.DisplayName;
                userModel.BillingAddress.CityName = orderModel.BillingAddress.CityName;
                userModel.BillingAddress.StateName = orderModel.BillingAddress.StateCode;
                userModel.BillingAddress.PostalCode = orderModel.BillingAddress.PostalCode;
                userModel.BillingAddress.PhoneNumber = orderModel.BillingAddress.PhoneNumber;
                userModel.BillingAddress.Address1 = orderModel.BillingAddress.Address1;
                userModel.BillingAddress.Address2 = orderModel.BillingAddress.Address2;
                userModel.BillingAddress.IsDefaultBilling = true;
                userModel.BillingAddress.AddressId = orderModel.AddressId;
                userModel.BillingAddress.CountryName = orderModel.BillingAddress.CountryName;
                userModel.BillingAddress.DisplayName = addressList?.AddressList?.Where(w => w.AddressId == orderModel.AddressId).FirstOrDefault()?.DisplayName;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Ordered Billing Address with user's Billing address.
        protected virtual void SetOrderedBillingAddress(AccountQuoteModel orderModel, UserAddressDataViewModel userModel)
        {
            if (IsNotNull(orderModel))
            {
                ZnodeLogging.LogMessage("BillingAddressModel with Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddressId = orderModel?.BillingAddressModel?.AddressId });
                userModel.BillingAddress = orderModel.BillingAddressModel.ToViewModel<AddressViewModel>();
            }
        }

        //Get localeId and catalogId by portal id
        protected virtual void GetPortalDetailsById(int portalId, out int localeId, out int catalogId)
        {
            localeId = 0;
            catalogId = 0;

            //Get Associated Portal Catalog By PortalId.
            PortalModel portal = _portalClient.GetPortal(portalId, new ExpandCollection { ZnodePortalEnum.ZnodePortalLocales.ToString(), ZnodePortalEnum.ZnodePortalUnits.ToString(), ZnodePortalEnum.ZnodePortalCatalogs.ToString() });

            if (IsNotNull(portal))
            {
                catalogId = portal.PublishCatalogId.GetValueOrDefault();
                localeId = portal.LocaleId.GetValueOrDefault();
            }
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { portalId = portalId, localeId = localeId, catalogId = catalogId });
        }

        //to get payment setting by id
        protected virtual PaymentSettingModel GetPaymentSetting(int paymentSettingId)
        {
            string gatwayname = string.Empty;
            FilterCollection filters = new FilterCollection();

            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "1"));
            filters.Add(new FilterTuple(ZnodePaymentSettingEnum.PaymentSettingId.ToString(), FilterOperators.Equals, paymentSettingId.ToString()));

            PaymentSettingModel paymentSetting = _paymentClient.GetPaymentSettings(null, filters, null, null, null)?.PaymentSettings?.FirstOrDefault();
            PaymentDetailsViewModel model = new PaymentDetailsViewModel();
            return paymentSetting ?? new PaymentSettingModel();
        }

        //Set Filters For Payment List.
        protected virtual FilterCollection SetFiltersForPaymentList(int userId, int portalId = 0)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "1"));
            filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, userId.ToString()));
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
            filters.Add(new FilterTuple(FilterKeys.IsAssociated, FilterOperators.Equals, "true"));
            return filters;
        }

        protected virtual void SetInvoiceToolMenu(OrdersListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonGenerateInvoice, JSFunctionName = "Order.prototype.GenerateInvoice(this)", ControllerName = "Order", ActionName = "DownloadPDF" });
            }
        }

        //Check group product quantity.
        protected virtual void CheckGroupInventory(GroupProductViewModel viewModel, decimal? quantity, int omsOrderId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Quantity = quantity, OmsOrderId = omsOrderId });
            if (IsNotNull(viewModel))
            {
                List<AttributesSelectValuesViewModel> inventorySetting = viewModel.Attributes?.SelectAttributeList(ZnodeConstant.OutOfStockOptions);
                string inventorySettingCode = inventorySetting.FirstOrDefault().Code;

                if (string.Equals(ZnodeConstant.DontTrackInventory, inventorySettingCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : Admin_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                    return;
                }

                if (IsNotNull(viewModel.Quantity))
                {
                    bool AllowBackOrder = false;
                    bool TrackInventory = false;
                    decimal selectedQuantity = quantity.GetValueOrDefault();

                    decimal cartQuantity = GetGroupProductOrderedItemQuantity(viewModel.SKU, omsOrderId);

                    decimal combinedQuantity = selectedQuantity + cartQuantity;

                    if (inventorySetting?.Count > 0)
                        TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySettingCode);

                    if (viewModel.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory)
                    {
                        viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.OutOfStockMessage) ? viewModel.OutOfStockMessage : Admin_Resources.TextOutofStock;
                        viewModel.ShowAddToCart = false;
                        return;
                    }
                    else if (viewModel.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                    {
                        viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.BackOrderMessage) ? viewModel.BackOrderMessage : Admin_Resources.TextBackOrderMessage;
                        viewModel.ShowAddToCart = true;
                        return;
                    }

                    decimal minimumQuantity = Convert.ToDecimal(viewModel.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.AttributeValues);
                    decimal maximumQuantity = Convert.ToDecimal(viewModel.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MaximumQuantity)?.AttributeValues);
                    if (!Between(combinedQuantity, minimumQuantity, maximumQuantity, true))
                    {
                        viewModel.InventoryMessage = string.Format(Admin_Resources.WarningSelectedQuantityWithProductName, minimumQuantity, maximumQuantity, viewModel.Name);
                        viewModel.ShowAddToCart = false;
                        return;
                    }
                    viewModel.InventoryMessage = !string.IsNullOrEmpty(viewModel.InStockMessage) ? viewModel.InStockMessage : Admin_Resources.TextInstock;
                    viewModel.ShowAddToCart = true;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        /// <summary>
        /// Get ordered items quantity by the given sku for group products.
        /// </summary>
        /// <param name="sku">Published product sku</param>
        /// <returns>cart Quantity</returns>
        protected virtual decimal GetGroupProductOrderedItemQuantity(string sku, int omsOrderId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            CartViewModel cart = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<OrderStateClient>(), GetClient<PortalClient>(), GetClient<UserClient>()).GetCart(omsOrderId);
            decimal? cartQuantity = 0.00M;

            if (cart?.ShoppingCartItems?.Count > 0)
            {
                cartQuantity = (
                   from CartItemViewModel item in cart.ShoppingCartItems
                   from AssociatedProductModel groupProduct in item.GroupProducts
                   where !Equals(groupProduct, null) && !Equals(item.AddOnProductSKUs, null)
                   where string.Equals(sku, groupProduct.Sku, StringComparison.OrdinalIgnoreCase) || item.AddOnProductSKUs.Split(',').Contains(sku)
                   select groupProduct.Quantity
                   ).Sum();
            }
            ZnodeLogging.LogMessage("Ordered items quantity: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CartQuantity = cartQuantity });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return cartQuantity.GetValueOrDefault();
        }

        //Get the list of products associated to group products.
        protected virtual List<GroupProductViewModel> GetGroupProducts(int productId, int localeId, int portalId, int userId, int? catalogId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ProductId = productId, LocaleId = localeId, PortalId = portalId, UserId = userId, CatalogId = catalogId });
            //Set filters to get associated products.
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, productId.ToString());
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());
            if (catalogId != null && catalogId != 0)
                filters.Add(FilterKeys.PublishCatalogId, FilterOperators.Equals, catalogId.ToString());

            SetUserId(userId);
            WebStoreGroupProductListModel groupProducts = _publishProductClient.GetGroupProductList(filters);

            ZnodeLogging.LogMessage("GroupProducts list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { GroupProductsListCount = groupProducts?.GroupProducts?.Count });
            if (groupProducts?.GroupProducts?.Count > 0)
                return groupProducts.GroupProducts.Count > 0 ? groupProducts.GroupProducts.ToViewModel<GroupProductViewModel>().ToList() : new List<GroupProductViewModel>();

            return new List<GroupProductViewModel>();
        }

        //Set user id in client header.
        public virtual void SetUserId(int userId)
        {
            if (userId > 0)
                _publishProductClient.UserId = userId;
        }

        public void GetConfigurableValues(PublishProductModel model, PublishProductsViewModel viewModel)
        {
            viewModel.ConfigurableData = new ConfigurableAttributeViewModel();
            //Select Is Configurable Attributes list
            viewModel.ConfigurableData.ConfigurableAttributes = viewModel.Attributes.Where(x => x.IsConfigurable && x.ConfigurableAttribute?.Count > 0).ToList();
            //Assign select attribute values.
            viewModel.ConfigurableData.ConfigurableAttributes.ForEach(x => x.SelectedAttributeValue = new[] { x.ConfigurableAttribute?.FirstOrDefault()?.AttributeValue });
            ZnodeLogging.LogMessage("ConfigurableAttributes count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ConfigurableAttributesCount = viewModel?.ConfigurableData?.ConfigurableAttributes?.Count });
        }

        //Get parameter model for configurable attribute.
        protected virtual ParameterProductModel GetConfigurableParameterModel(int productId, int localeId, int portalId, string selectedCode, string selectedValue, Dictionary<string, string> SelectedAttributes, int catalogId)
        {
            ParameterProductModel productAttribute = new ParameterProductModel();
            productAttribute.ParentProductId = productId;
            productAttribute.LocaleId = localeId;
            productAttribute.SelectedAttributes = SelectedAttributes;
            productAttribute.PortalId = portalId;
            productAttribute.SelectedCode = selectedCode;
            productAttribute.SelectedValue = selectedValue;
            productAttribute.PublishCatalogId = catalogId;
            ZnodeLogging.LogMessage("ConfigurableParameterModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ConfigurableParameterModel = productAttribute });
            return productAttribute;
        }

        //Map configurable product data.
        protected virtual void MapConfigurableProductData(int productId, PublishProductsViewModel viewModel, ConfigurableAttributeViewModel configurableData)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            viewModel.ConfigurableData = new ConfigurableAttributeViewModel();
            //Assign list of configurable attribute
            viewModel.ConfigurableData.ConfigurableAttributes = configurableData.ConfigurableAttributes;

            viewModel.ConfigurableData.CombinationErrorMessage = configurableData.CombinationErrorMessage;

            string minQuantity = viewModel?.Attributes?.Value(ZnodeConstant.MinimumQuantity);
            decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);
            string addonSKu = string.Join(",", viewModel.AddOns?.Where(x => x.IsRequired)?.Select(y => y.AddOnValues?.First().SKU));
            //Check Product Inventory
            CheckInventory(viewModel, quantity);

            if (!string.IsNullOrEmpty(addonSKu))
                //Check Associated addon inventory.
                CheckAddOnInventory(viewModel, addonSKu, quantity);

            GetProductFinalPrice(viewModel, viewModel.AddOns, quantity, addonSKu);

            if (IsNull(viewModel.ProductPrice))
            {
                viewModel.ShowAddToCart = false;
                viewModel.InventoryMessage = Admin_Resources.ErrorPriceNotAssociate;
            }
            viewModel.ParentProductId = productId;
            viewModel.IsConfigurable = true;
            if (viewModel.IsDefaultConfigurableProduct)
                viewModel.ShowAddToCart = false;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get group product quantity according to quantity.
        protected virtual void GetGroupProductFinalPrice(GroupProductViewModel viewModel, decimal minQuantity)
        {
            viewModel.SalesPrice = viewModel.SalesPrice > 0 ? viewModel.SalesPrice * minQuantity : viewModel.RetailPrice;
            viewModel.RetailPrice = viewModel.SalesPrice < 1 ? viewModel.RetailPrice * minQuantity : viewModel.RetailPrice;
        }

        //Set order list data to OrderViewModel.
        protected virtual void SetOrderListData(OrderViewModel order)
        {
            if (IsNotNull(order))
            {
                order.OrderTotalWithCurrency = HelperMethods.FormatPriceWithCurrency(order.Total, order.CultureCode);
                order.Total = order.Total;
                order.Tax = HelperMethods.FormatPriceWithCurrency(order.TaxCost, order.CultureCode);
                order.Shipping = HelperMethods.FormatPriceWithCurrency(order.ShippingCost, order.CultureCode);
                order.SubTotalAmount = HelperMethods.FormatPriceWithCurrency(order.SubTotal, order.CultureCode);
            }
        }

        //For Getting personalise attribute.
        protected virtual string GetPersonaliseAttributes(string productName, Dictionary<string, object> personaliseValueList)
        {
            if (IsNotNull(personaliseValueList))
                foreach (var personaliseAttribute in personaliseValueList)
                    productName += $"{"<p>"} { personaliseAttribute.Key}{" : "}{personaliseAttribute.Value}{"</p>"}";
            return productName;
        }

        //Create Single Order Line Item.
        public virtual void CreateSingleOrderLineItem(OrderViewModel orderModel, List<OrderLineItemViewModel> orderLineItemListModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            foreach (OrderLineItemViewModel _lineItems in orderModel.OrderLineItems)
            {
                bool isGroupProduct = false;
                OrderLineItemViewModel orderLineItemModel = new OrderLineItemViewModel();
                orderLineItemModel = _lineItems;
                if (!CheckForReturnInLineItem(orderLineItemModel?.OrderLineItemStateId))
                {
                    if (HelperUtility.IsNull(_lineItems.ParentOmsOrderLineItemsId))
                    {
                        List<OrderLineItemViewModel> childItems = orderModel.OrderLineItems.Where(x => x.ParentOmsOrderLineItemsId == _lineItems.OmsOrderLineItemsId).ToList();
                        foreach (OrderLineItemViewModel orderLineItem in childItems)
                        {
                            orderLineItemModel.OmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId;
                            orderLineItemModel.OrderLineItemState = orderLineItem.OrderLineItemState;
                            orderLineItemModel.OrderLineItemStateId = orderLineItem.OrderLineItemStateId;
                            if (orderLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)
                            {
                                orderLineItemModel.OrderLineItemRelationshipTypeId = (int)ZnodeCartItemRelationshipTypeEnum.Configurable;
                                orderLineItemModel.ProductName = orderLineItem.ProductName;
                                orderLineItemModel.Sku = orderLineItem.Sku;
                                orderLineItemModel.Quantity = orderLineItem.Quantity;
                                orderLineItemModel.Price = orderLineItem.Price;
                                orderLineItemModel.Total = (orderLineItem.Quantity * orderLineItem.Price);
                            }
                            if (orderLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns)
                            {
                                orderLineItemModel.Total = (orderLineItem.Quantity * (orderLineItem.Price + _lineItems.Price));
                                orderLineItemModel.Sku = orderLineItem.Sku;
                                orderLineItemModel.ProductName = _lineItems.ProductName;
                                orderLineItemModel.Price = (orderLineItem.Price + _lineItems.Price);
                            }
                            if (orderLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)
                            {
                                OrderLineItemViewModel orderLineItems = new OrderLineItemViewModel();
                                orderLineItems = orderLineItem;
                                orderLineItems.OrderLineItemRelationshipTypeId = (int)ZnodeCartItemRelationshipTypeEnum.Group;
                                orderLineItems.ProductName = _lineItems.ProductName;
                                orderLineItems.Description = orderLineItem.Description;
                                orderLineItems.Sku = orderLineItem.Sku;
                                orderLineItems.Quantity = orderLineItem.Quantity;
                                orderLineItems.Price = orderLineItem.Price;
                                orderLineItems.Total = (orderLineItem.Quantity * orderLineItem.Price);
                                orderLineItemListModel.Add(orderLineItems);
                                isGroupProduct = true;
                            }
                        }
                        orderLineItemModel.Total = (_lineItems.Quantity * _lineItems.Price);
                        if (!isGroupProduct) orderLineItemListModel.Add(orderLineItemModel);
                    }
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        protected bool CheckForReturnInLineItem(int? omsOrderStateId)
        {
            var lineItem = BindOrderStatus(new FilterTuple(ZnodeOmsOrderStateEnum.OrderStateName.ToString(), FilterOperators.Contains, ZnodeOrderStatusEnum.RETURNED.ToString()))?.FirstOrDefault();
            return (omsOrderStateId == Convert.ToInt32(lineItem.Value));
        }

        //Set Order Invoice Filters.
        protected virtual ExpandCollection SetOrderInvoiceFilters()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.OrderLineItems);
            expands.Add(ExpandKeys.OrderShipment);
            expands.Add(ExpandKeys.PaymentType);
            expands.Add(ExpandKeys.UserDetails);
            expands.Add(ExpandKeys.OmsPaymentState);
            expands.Add(ExpandKeys.ShippingType);
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString());
            return expands;
        }

        //Set Ordered Billing and Shipping Addresses.
        protected virtual void SetOrderedBillingShippingAddresses(OrderModel orderModel, UserAddressDataViewModel userModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(userModel.ShippingAddress))
                userModel.ShippingAddress = new AddressViewModel();

            if (IsNull(userModel.BillingAddress))
                userModel.BillingAddress = new AddressViewModel();

            //Get B2B account or user address list.
            AddressListModel addressList = GetAddressListOfUserAndAccount(orderModel.UserId, GetCustomerAccountId(orderModel.UserId));

            //Set Ordered Billing Address with user's Billing address.
            SetOrderedBillingAddress(orderModel, userModel, addressList);

            //Set Ordered Shipping Address with user's shipping address.
            SetOrderedShippingAddress(orderModel, userModel, addressList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Ordered Billing and Shipping Addresses.
        protected virtual void SetOrderedBillingShippingAddresses(AccountQuoteModel orderModel, UserAddressDataViewModel userModel)
        {
            if (IsNull(userModel.ShippingAddress))
                userModel.ShippingAddress = new AddressViewModel();

            if (IsNull(userModel.BillingAddress))
                userModel.BillingAddress = new AddressViewModel();

            //Set Ordered Billing Address with user's Billing address.
            SetOrderedBillingAddress(orderModel, userModel);

            //Set Ordered Shipping Address with user's shipping address.
            SetOrderedShippingAddress(orderModel, userModel);
        }

        //Return account id if customer is B2B if not returns 0.
        protected virtual int GetCustomerAccountId(int userId)
        {
            UserModel userAccountData = _userClient.GetUserAccountData(userId);
            ZnodeLogging.LogMessage("UserId and customer account Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId, CustomerAccountId = userAccountData?.AccountId });
            return userAccountData?.AccountId > 0 ? userAccountData.AccountId.GetValueOrDefault() : 0;
        }

        //Set Expands For Update Payment Status.
        protected virtual ExpandCollection SetExpandsForUpdatePaymentStatus()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.PaymentType);
            expands.Add(ExpandKeys.OmsPaymentState);
            return expands;
        }

        //Remove Invalid coupon code/giftcard.
        protected virtual void RemoveInvalidDiscountCode(ShoppingCartModel cartModel)
        {
            //Remove invalid coupon code.
            if (cartModel.Coupons?.Count > 0)
                cartModel.Coupons.RemoveAll(x => !x.CouponApplied);
        }

        protected virtual FilterCollection SetQuickOrderListFilter(string sku, int portalCatalogId, int portalId, int userId, int localeId)
        {
            FilterCollection filters = new FilterCollection();

            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, FilterKeys.ActiveTrueValue));
            filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, userId.ToString()));

            //Add portalCatalogId and portalId in FilterCollectionDataModel.
            if (portalCatalogId > 0)
                filters.Add(new FilterTuple(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, Convert.ToString(portalCatalogId)));
            if (portalId > 0)
                filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(portalId)));
            if (localeId > 0)
                filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, Convert.ToString(localeId)));
            if (!string.IsNullOrEmpty(sku))
                filters.Add(new FilterTuple(FilterKeys.SKU, FilterOperators.Contains, sku));

            ZnodeLogging.LogMessage("QuickOrderListFilter: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            return filters;
        }

        //Get inventory message.
        public virtual string GetInventoryMessage(PublishProductsViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string inventoryMessage = string.Empty;
            List<AttributesSelectValuesViewModel> inventorySetting = GetOutOfStockOptionsAttributeList(model);
            if (inventorySetting?.Count > 0)
            {
                if (IsNotNull(model.Quantity) && model.Quantity > 0)
                {
                    bool AllowBackOrder = false;
                    bool TrackInventory = false;
                    string minQuantity = model?.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.AttributeValues;
                    decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);
                    decimal combinedQuantity = quantity + GetOrderedItemQuantity(model.SKU);

                    TrackInventoryData(ref AllowBackOrder, ref TrackInventory, inventorySetting.FirstOrDefault().Code);

                    if (model.Quantity < combinedQuantity && !AllowBackOrder && TrackInventory)
                        inventoryMessage = !string.IsNullOrEmpty(model.OutOfStockMessage) ? model.OutOfStockMessage : WebStore_Resources.TextOutofStock;
                    else if (model.Quantity < combinedQuantity && AllowBackOrder && TrackInventory)
                        inventoryMessage = !string.IsNullOrEmpty(model.BackOrderMessage) ? model.BackOrderMessage : WebStore_Resources.TextBackOrderMessage;

                    if (!Between(combinedQuantity, Convert.ToDecimal(minQuantity), Convert.ToDecimal(model.Attributes?.Value(ZnodeConstant.MaximumQuantity)), true))
                        inventoryMessage = string.Format(WebStore_Resources.WarningSelectedQuantity, minQuantity, model.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                }
                else
                    inventoryMessage = !string.IsNullOrEmpty(model.OutOfStockMessage) ? model.OutOfStockMessage : WebStore_Resources.TextOutofStock;
            }
            ZnodeLogging.LogMessage("Inventory message: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { InventoryMessage = inventoryMessage });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return inventoryMessage;
        }

        //Get the list of customer on the basis of portalId,and search term(Phone Number or User Name).
        public virtual List<CustomerViewModel> GetCustomerList(string searchTerm, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            UserListModel userList = null;

            if (!string.IsNullOrEmpty(searchTerm) && portalId > 0)
            {
                FilterCollection filters = AddPortalIdAndSearchTermInFilters(new FilterCollection(), portalId, searchTerm);

                ZnodeLogging.LogMessage("parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, filters);

                userList = _userClient.GetCustomerAccountList(HttpContext.Current.User.Identity.Name, filters, new SortCollection(), null, null, null);
            }

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return userList?.Users?.Count > 0 ? userList.Users.ToViewModel<CustomerViewModel>().ToList() : new List<CustomerViewModel>();
        }

        //Get Payment State List.
        protected virtual List<SelectListItem> GetPaymentStateList(string PaymentType)
            => ToOrderPaymentStateList(_orderClient.GetPaymentStateList()?.PaymentStateList, PaymentType);

        //Remove all filters from FilterCollectionDataModel
        protected virtual void RemoveFiltersForProductList(FilterCollectionDataModel model)
        {
            model.Filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.CatalogId.ToString().ToLower());
            model.Filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.PortalId.ToString().ToLower());
            model.Filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.ZnodeCatalogId.ToString().ToLower());
            model.Filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.IsActive.ToString().ToLower());
            model.Filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.UserId.ToString().ToLower());
            model.Filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.fromOrder.ToString().ToLower());
            model.Filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.ProductIndex.ToString().ToLower());
        }

        // Remove key from dictionary.
        public virtual void RemoveKeyFromDictionary(OrderModel orderModel, string key, bool isFromLineItem = false)
        {
            if (IsNotNull(orderModel.OrderHistory) && !isFromLineItem)
            {
                if (orderModel.OrderHistory.ContainsKey(key))
                    orderModel.OrderHistory.Remove(key);
            }
            else
            {
                if (orderModel.OrderLineItemHistory.ContainsKey(key))
                    orderModel.OrderLineItemHistory.Remove(key);
            }
        }

        // Add key and value in dictionary.
        protected virtual void OrderHistory(OrderModel orderModel, string settingType, string oldValue, string newValue = "") => orderModel.OrderHistory?.Add(settingType, newValue);

        // Add key and value in order line dictionary.
        protected virtual void OrderLineHistory(OrderModel orderModel, string key, OrderLineItemHistoryModel lineHistory) => orderModel.OrderLineItemHistory?.Add(key, lineHistory);

        //Save Customer Payment Guid for Save Credit Card
        protected virtual void SaveCustomerPaymentGuid(int userId, string customerGuid, string customerPaymentGUID)
        {
            if (!string.IsNullOrEmpty(customerGuid) && string.IsNullOrEmpty(customerPaymentGUID))
            {
                UserModel userModel = _userClient.GetUserAccountData(userId);
                userModel.CustomerPaymentGUID = customerGuid;
                _userClient.UpdateCustomerAccount(userModel);
            }
        }

        protected virtual decimal GetQuantityOnHandBySku(ShoppingCartItemModel cartItem, int portalId, int productId)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = portalId, ProductId = productId });
            //Get the sku of product of which quantity needs to update.
            string sku = string.Empty;

            //Get selected sku.
            sku = productId > 0 ? cartItem.GroupProducts?.Where(x => x.ProductId == productId)?.FirstOrDefault()?.Sku
                                      : !string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs) ? cartItem.ConfigurableProductSKUs : cartItem.SKU;

            bool isBundleProduct = !string.IsNullOrEmpty(cartItem?.BundleProductSKUs) ? true : false;

            ProductInventoryPriceListModel productInventory = _publishProductClient.GetProductPriceAndInventory(new ParameterInventoryPriceModel { Parameter = sku, PortalId = portalId, IsBundleProduct= isBundleProduct, BundleProductParentSKU = cartItem.SKU});

            decimal quantityOnHand = (productInventory?.ProductList?.Where(w => w.SKU == sku)?.FirstOrDefault().Quantity).GetValueOrDefault();
            ZnodeLogging.LogMessage("QuantityOnHand: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { QuantityOnHand = quantityOnHand });
            return quantityOnHand;
        }

        //Set Gift Card Number and CSR Discount Amount Data For Calculation
        protected virtual void SetCartDataForCalculation(ShoppingCartModel cartModel, string GiftCardNumber, decimal CSRDiscountAmount)
        {
            cartModel.GiftCardNumber = GiftCardNumber;
            cartModel.CSRDiscountAmount = CSRDiscountAmount;
        }

        // Get calculated shopping cart model.
        public virtual ShoppingCartModel GetCalculatedShoppingCartForEditOrder(ShoppingCartModel shoppingCartModel, bool isEdit = true)
        {
            UserModel userModel = shoppingCartModel.UserDetails;
            ShoppingCartModel calculatedShoppingCartModel = _shoppingCartClient.Calculate(shoppingCartModel);
            ZnodeLogging.LogMessage("calculatedShoppingCartModel with Id: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderStatusId = calculatedShoppingCartModel?.OmsOrderStatusId });

            if (IsNotNull(calculatedShoppingCartModel) && isEdit)
            {
                //calculatedShoppingCartModel.ShippingCost = (calculatedShoppingCartModel.ShippingCost + calculatedShoppingCartModel.ShippingDifference);
                calculatedShoppingCartModel.UserDetails = userModel;
                calculatedShoppingCartModel.IsRemoveShippingDiscount = shoppingCartModel.IsRemoveShippingDiscount;
            }
            return calculatedShoppingCartModel;
        }

        //Void or Refund transactions
        public virtual bool VoidRefundPayment(OrderModel orderModel, bool isSaveOrder, out string message, int? OmsOrderId = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = OmsOrderId, PaymentStatus = orderModel?.PaymentStatus, OmsOrderDetailsId = orderModel?.OmsOrderDetailsId });
            BooleanModel booleanModel = new BooleanModel();
            message = string.Empty;
            if (string.Equals(orderModel.PaymentStatus, ZnodeConstant.CAPTURED.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                TransactionDetailsModel transactionModel = _paymentClient.GetTransactionStatusDetails(orderModel.PaymentTransactionToken);
                PaymentSettingModel paymentSetting = GetPaymentSetting(orderModel.PaymentSettingId ?? 0);
                if (paymentSetting?.GatewayCode == AdminConstants.Payflow || paymentSetting?.GatewayCode == AdminConstants.Stripe || string.Equals(paymentSetting?.GatewayCode, AdminConstants.Braintree, StringComparison.CurrentCultureIgnoreCase))
                    transactionModel.TransactionStatus = paymentStatusSettledSuccessfully;
                if ((paymentSetting?.GatewayCode == AdminConstants.CardConnect || paymentSetting?.GatewayCode == AdminConstants.CyberSource) && (transactionModel.IsRefundable && !transactionModel.IsVoidable))
                    transactionModel.TransactionStatus = paymentStatusSettledSuccessfully;
                if (paymentSetting?.GatewayCode == AdminConstants.PayPalExpressCheckout && string.Equals(transactionModel?.TransactionStatus, paymentStatusPartiallyRefund, StringComparison.InvariantCultureIgnoreCase))
                    transactionModel.TransactionStatus = paymentStatusSettledSuccessfully;
                try
                {
                    // Added, Or condition for Paypal Partial refund
                    booleanModel = (string.Equals(transactionModel?.TransactionStatus, paymentStatusSettledSuccessfully, StringComparison.InvariantCultureIgnoreCase)) ? _paymentClient.RefundPayment(new RefundPaymentModel { Token = orderModel.PaymentTransactionToken, IsCompleteOrderRefund = true })
                        : _paymentClient.VoidPayment(orderModel.PaymentTransactionToken);

                    if (IsNotNull(OmsOrderId) && !(booleanModel?.HasError ?? true))
                        _orderClient.UpdateOrderPaymentStatus(OmsOrderId.Value, ZnodeConstant.VOIDED.ToString());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                }

                if (booleanModel.HasError)
                    message = (string.Equals(transactionModel?.TransactionStatus, paymentStatusSettledSuccessfully, StringComparison.InvariantCultureIgnoreCase) || string.Equals(transactionModel?.TransactionStatus, paymentStatusPartiallyRefund, StringComparison.InvariantCultureIgnoreCase))
                        ? isSaveOrder == true ? Admin_Resources.ContinueSaveRefundErrorMessage : Admin_Resources.AbortSaveRefundErrorMessage
                        : isSaveOrder == true ? Admin_Resources.ContinueSaveVoidErrorMessage : Admin_Resources.AbortSaveVoidErrorMessage;
                else
                    message = (string.Equals(transactionModel?.TransactionStatus, paymentStatusSettledSuccessfully, StringComparison.InvariantCultureIgnoreCase) || string.Equals(transactionModel?.TransactionStatus, paymentStatusPartiallyRefund, StringComparison.InvariantCultureIgnoreCase)) ? Admin_Resources.PaymentRefundSuccessMessage : Admin_Resources.PaymentVoidSuccessMessage;

                string refundTransactionId = _paymentClient.GetRefundTransactionId(orderModel.PaymentTransactionToken);
                _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = string.IsNullOrEmpty(refundTransactionId) ? orderModel.PaymentTransactionToken : refundTransactionId , OrderAmount = orderModel.Total < 0 ? orderModel.Total : -(orderModel.Total) });
                return !(booleanModel?.HasError ?? true);
            }
            else if (string.Equals(orderModel.PaymentStatus, ZnodeConstant.AUTHORIZED.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    booleanModel = _paymentClient.VoidPayment(orderModel.PaymentTransactionToken);
                    if (IsNotNull(OmsOrderId) && !(booleanModel?.HasError ?? true))
                        _orderClient.UpdateOrderPaymentStatus(OmsOrderId.Value, ZnodeConstant.VOIDED.ToString());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                }
                message = (booleanModel?.HasError ?? true)
                    ? isSaveOrder == true ? Admin_Resources.ContinueSaveVoidErrorMessage : Admin_Resources.AbortSaveVoidErrorMessage
                    : Admin_Resources.PaymentVoidSuccessMessage;
                _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = orderModel.PaymentTransactionToken, OrderAmount = orderModel.Total < 0 ? orderModel.Total : -(orderModel.Total) });
                return !(booleanModel?.HasError ?? true);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return true;
        }

        //Void or Refund transactions
        protected virtual void AmazonVoidRefundPayment(OrderModel orderModel, int? OmsOrderId = null)
        {
            BooleanModel booleanModel = null;
            string message = string.Empty;
            if (string.Equals(orderModel.PaymentStatus, ZnodeConstant.CAPTURED.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    booleanModel = _paymentClient.AmazonPayRefund(new RefundPaymentModel { Token = orderModel.PaymentTransactionToken, IsCompleteOrderRefund = true });
                    if (IsNotNull(OmsOrderId) && !(booleanModel?.HasError ?? true))
                        _orderClient.UpdateOrderPaymentStatus(OmsOrderId.Value, ZnodeConstant.REFUNDED.ToString());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                }
                message = (booleanModel?.HasError ?? true) ? Admin_Resources.PaymentRefundErrorMessage : Admin_Resources.PaymentRefundSuccessMessage;
                _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = orderModel.PaymentTransactionToken, OrderAmount = orderModel.Total < 0 ? orderModel.Total : -(orderModel.Total) });
            }
            else if (string.Equals(orderModel.PaymentStatus, ZnodeConstant.AUTHORIZED.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    booleanModel = _paymentClient.AmazonVoidPayment(orderModel.PaymentTransactionToken);
                    if (IsNotNull(OmsOrderId) && !(booleanModel?.HasError ?? true))
                        _orderClient.UpdateOrderPaymentStatus(OmsOrderId.Value, ZnodeConstant.VOIDED.ToString());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                }
                message = (booleanModel?.HasError ?? true) ? Admin_Resources.PaymentVoidErrorMessage : Admin_Resources.PaymentVoidSuccessMessage;
                _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = message, TransactionId = orderModel.PaymentTransactionToken, OrderAmount = orderModel.Total < 0 ? orderModel.Total : -(orderModel.Total) });
            }
        }

        //To refund order line item transaction amount by transactionId
        public virtual bool RefundPaymentByAmount(int orderDetailsId, string transactionId, decimal transactionAmount, out string errorMessage,OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderDetailsId = orderDetailsId, TransactionAmount = transactionAmount });
            BooleanModel booleanModel = null;
            errorMessage = "";
            string message = string.Empty;
            decimal refundAmount = transactionAmount;
            if (refundAmount < 0)
                refundAmount = -(refundAmount);

            TransactionDetailsModel transactionModel = _paymentClient.GetTransactionStatusDetails(transactionId);

            if (string.IsNullOrEmpty(transactionModel?.TransactionStatus) || string.Equals(transactionModel?.TransactionStatus, paymentStatusSettledSuccessfully, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(transactionModel?.TransactionStatus, AdminConstants.PaymentSettled, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(transactionModel?.TransactionStatus, AdminConstants.Completed, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(transactionModel?.TransactionStatus, paymentStatusPartiallyRefund, StringComparison.InvariantCultureIgnoreCase))
            {
                RefundPaymentModel refundPaymentModel = new RefundPaymentModel();
                refundPaymentModel.BillingAddress = orderModel.BillingAddress;
                refundPaymentModel.ShippingAddress = orderModel.ShippingAddress;
                refundPaymentModel.RefundAmount = refundAmount;
                refundPaymentModel.Token = transactionId;
                refundPaymentModel.ShippingAddress.GatewayCode = transactionModel.GatewayCode;
                booleanModel = _paymentClient.RefundPayment(refundPaymentModel);
                errorMessage = message = (booleanModel?.HasError ?? true) ? Admin_Resources.AbortSaveRefundErrorMessage : Admin_Resources.PaymentRefundSuccessMessage;
            }
            else
                message = errorMessage = Admin_Resources.RefundFailedMessage;
            string refundTransactionId = _paymentClient.GetRefundTransactionId(transactionId);
            _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderDetailsId, Message = message,TransactionId= string.IsNullOrEmpty(refundTransactionId)? transactionId : refundTransactionId,OrderAmount = transactionAmount < 0 ? transactionAmount : -(transactionAmount) });
            return !(booleanModel?.HasError ?? true);
        }

        //to refund order line item transaction amount by transactionId
        protected virtual bool RefundAmazonPaymentByAmount(int orderDetailsId, string transactionId, decimal transactionAmount)
        {
            decimal refundAmount = transactionAmount;
            if (refundAmount < 0)
                refundAmount = -(refundAmount);
            BooleanModel booleanModel = _paymentClient.AmazonPayRefund(new RefundPaymentModel { Token = transactionId, RefundAmount = refundAmount });
            string message = (booleanModel?.HasError ?? true) ? Admin_Resources.PaymentRefundErrorMessage : Admin_Resources.PaymentRefundSuccessMessage;
            _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderDetailsId, Message = message, TransactionId = transactionId, OrderAmount = transactionAmount < 0 ? transactionAmount : -(transactionAmount) });
            return (!booleanModel?.HasError ?? true);
        }

        //to refund order line item in case of Paypal express by giftCard.
        public virtual bool RefundPaymentByGiftCard(int orderDetailsId, string transactionId, decimal transactionAmount)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderDetailsId = orderDetailsId, TransactionAmount = transactionAmount });
            decimal refundAmount = transactionAmount;
            if (refundAmount < 0)
                refundAmount = -(refundAmount);
            _orderClient.CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderDetailsId, Message = Admin_Resources.PaymentRefundByGiftCard, TransactionId = transactionId, OrderAmount = transactionAmount });
            return true;
        }

        // Map order line status message.
        protected virtual void OrderLineHistoryMessage(List<OrderLineItemHistoryModel> lineHistory, OrderLineItemHistoryModel orderLineItemHistoryModel) => lineHistory.Add(orderLineItemHistoryModel);

        protected virtual bool ValidateOrderOnManage(OrderModel orderModel)
        {
            if (orderModel.OrderLineItemHistory.Count > 0 || orderModel.OrderHistory.Count > 0 || !string.IsNullOrEmpty(orderModel.AdditionalInstructions) || !string.IsNullOrEmpty(orderModel.ExternalId))
            {
                if (orderModel.ShoppingCartModel?.ShoppingCartItems?.Count > 0)
                    return true;
                else if (orderModel.ShoppingCartModel?.ShoppingCartItems?.Count < 1 && orderModel.ReturnItemList?.ReturnItemList?.Count > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        //Set Tracking Url.
        protected virtual string SetTrackingUrl(string trackingNo, string trackingUrl)
         => IsNotNull(trackingUrl) ? $"<a target=_blank href={trackingUrl + trackingNo}>{trackingNo}</a>" : trackingNo;

        protected virtual string SetUserName(OrderModel orderModel, UserModel userAccountData)
        => string.IsNullOrEmpty(userAccountData.AspNetUserId) ? orderModel?.BillingAddress?.FirstName + " " + orderModel?.BillingAddress?.LastName : orderModel?.FirstName + " " + orderModel?.LastName;

        //To Set personalise attribute in ShoppingCartModel.
        protected virtual void SetAdditionalCostForShoppingCart(CartViewModel cartViewModel, List<OrderLineItemModel> orderLineItemModel)
        {
            if (IsNotNull(cartViewModel) && IsNotNull(cartViewModel.ShoppingCartItems))
                cartViewModel.ShoppingCartItems.ForEach(x =>
                {
                    var _item = orderLineItemModel.FirstOrDefault(y => y.OmsOrderLineItemsId == x.OmsOrderLineItemsId);

                    x.AdditionalCost = _item?.AdditionalCost;
                    x.CartDescription = _item?.Description;
                    //x.DownloadableProductKey = _item?.DownloadableProductKey;
                });
        }

        //If it doesn't have personalize attribute then Add parent product's personalize attribute

        //UpdateReturnShippingHistory in session
        public bool UpdateReturnShippingHistory(int omsOrderLineItemsId, int omsOrderId, bool isInsert = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderLineItemsId = omsOrderLineItemsId, OmsOrderId = omsOrderId, IsInsert = isInsert });
            bool success = false;
            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId);

            List<OrderLineItemHistoryModel> orderLineHistoryList = orderModel.OrderLineItemHistory.Select(s => (OrderLineItemHistoryModel)s.Value).ToList();

            ReturnOrderLineItemModel returnOrderLineItemModel = orderModel?.ReturnItemList?.ReturnItemList?.FirstOrDefault(o => o.OmsOrderLineItemsId == omsOrderLineItemsId);
            if (IsNotNull(returnOrderLineItemModel))
            {
                decimal? shippingCost = returnOrderLineItemModel?.ShippingCost;

                OrderLineItemHistoryModel orderLineItemHistoryModel = orderLineHistoryList.FirstOrDefault(s => s.SKU == returnOrderLineItemModel.SKU) ??
                                                     new OrderLineItemHistoryModel()
                                                     {
                                                         OmsOrderLineItemsId = returnOrderLineItemModel?.OmsOrderLineItemsId ?? 0,
                                                         OrderLineQuantity = string.Empty,
                                                         OrderTrackingNumber = string.Empty,
                                                         Quantity = string.Empty,
                                                         PartialRefundAmount = string.Empty,
                                                         IsShippingReturn = true,
                                                         ProductName = (returnOrderLineItemModel?.ProductName ?? string.Empty),
                                                         ReturnShippingAmount = (shippingCost ?? 0).ToString(),
                                                         SKU = (returnOrderLineItemModel?.SKU ?? string.Empty).ToString(),
                                                     };

                orderLineItemHistoryModel.ReturnShippingAmount = HelperMethods.FormatPriceWithCurrency((isInsert == true) ? (shippingCost ?? 0) : 0, orderModel.CultureCode);

                if (orderLineHistoryList?.Count() == 0 || IsNull(orderLineHistoryList))
                {
                    if (isInsert && shippingCost > 0)
                        OrderLineHistory(orderModel, returnOrderLineItemModel.SKU, orderLineItemHistoryModel);
                }
                else
                {
                    RemoveKeyFromDictionary(orderModel, returnOrderLineItemModel.SKU, true);
                    if (isInsert && shippingCost > 0)
                        OrderLineHistory(orderModel, returnOrderLineItemModel.SKU, orderLineItemHistoryModel);
                }

                SaveInSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId, orderModel);
                success = true;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return success;
        }

        //Add Portal id in filter collection
        public virtual void AddPortalIdInFilters(FilterCollection filters, int portalId)
        {
            if (portalId > 0)
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            }
        }

        //Map store filter values in view model
        protected virtual void BindStoreFilterValues(OrdersListViewModel orderListModel, int portalId, string portalName)
        {
            orderListModel.PortalName = string.IsNullOrEmpty(portalName) ? Admin_Resources.DefaultAllStores : portalName;
            orderListModel.PortalId = portalId;
        }

        //Add Portal id and search term in filter collection
        protected virtual FilterCollection AddPortalIdAndSearchTermInFilters(FilterCollection filters, int portalId, string searchTerm)
        {
            //remove old filters.
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodePortalAccountEnum.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, $"{AspNetUserEnum.UserName.ToString()}|{AspNetUserEnum.PhoneNumber.ToString()}", StringComparison.InvariantCultureIgnoreCase));

            filters.Add(new FilterTuple($"{AspNetUserEnum.UserName.ToString()}|{AspNetUserEnum.PhoneNumber.ToString()}", FilterOperators.StartsWith, searchTerm));
            filters.Add(new FilterTuple(ZnodePortalAccountEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));

            return filters;
        }

        /// <summary>
        /// This method used to save Address List in session
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="addressViewModel">:List of addressViewModel</param>
        /// <returns></returns>
        public void SaveAddressListToSession(int userId, List<AddressViewModel> addressViewModel)
        {
            if (userId > 0)
                SaveInSession($"{AdminConstants.AddressList}_{userId}", addressViewModel);
            else
                SaveInSession(AdminConstants.AddressList, addressViewModel);
        }

        /// <summary>
        /// This method used to get Address List from session
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>(return the value of address list from the session)</returns>
        public List<AddressViewModel> GetAddressListFromSession(int userId)
        {
            List<AddressViewModel> addressViewModel = null;
            if (userId > 0)
                addressViewModel = GetFromSession<List<AddressViewModel>>($"{AdminConstants.AddressList}_{userId}");
            else
                addressViewModel = GetFromSession<List<AddressViewModel>>(AdminConstants.AddressList);

            return addressViewModel;
        }

        /// <summary>
        /// This method used to remove Address List in session
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        public void RemoveAddressListSessionFrom(int userId)
        {
            if (userId > 0)
                RemoveInSession($"{AdminConstants.AddressList}_{userId}");
            else
                RemoveInSession(AdminConstants.AddressList);
        }

        //Get Failed Order Transaction List.
        public virtual FailedOrderTransactionListViewModel FailedOrderTransactionList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            DateRangePickerHelper.FormatFilterForDateTimeRange(_filters, DateTimeRange.Last_Day.ToString(), DateTimeRange.All_Transactions.ToString());

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = _filters, Sorts = sortCollection });
            FailedOrderTransactionListModel failedOrderTransactionList = _orderClient.FailedOrderTransactionList(_filters, sortCollection, pageIndex, recordPerPage);
            FailedOrderTransactionListViewModel failedOrderTransactionListVM = new FailedOrderTransactionListViewModel {
                FailedOrderTransactionListVM = failedOrderTransactionList.FailedOrderTransactionModel.ToViewModel<FailedOrderTransactionViewModel>().ToList()
            };
            SetListPagingData(failedOrderTransactionListVM, failedOrderTransactionList);

            if (failedOrderTransactionListVM?.FailedOrderTransactionListVM?.Count > 0)
            {
                foreach (FailedOrderTransactionViewModel failedOrderTransaction in failedOrderTransactionListVM.FailedOrderTransactionListVM)
                    SetFailedOrderListData(failedOrderTransaction);
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return failedOrderTransactionListVM;

        }

        //Format the price to display the dollar sign.
        protected virtual void SetFailedOrderListData(FailedOrderTransactionViewModel failedOrderTransactionViewModel)
        {
            if (IsNotNull(failedOrderTransactionViewModel))
            {
                failedOrderTransactionViewModel.FailedOrderTotalWithCurrency = HelperMethods.FormatPriceWithCurrency(failedOrderTransactionViewModel?.TotalAmount, DefaultSettingHelper.DefaultCulture);
            }
        }

        protected virtual ShoppingCartModel SetAddressDetailsToShoppingCart(CartParameterModel cartParameter, CreateOrderViewModel createOrderViewModel, bool isQuote = false)
        {
            ShoppingCartModel shoppingCartModel = new ShoppingCartModel();
            shoppingCartModel.ShippingAddress = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress.ToModel<AddressModel>();
            shoppingCartModel.BillingAddress = createOrderViewModel?.UserAddressDataViewModel?.BillingAddress.ToModel<AddressModel>();
            shoppingCartModel.UserId = cartParameter.UserId.GetValueOrDefault();
            shoppingCartModel.PortalId = cartParameter.PortalId;
            shoppingCartModel.PublishedCatalogId = createOrderViewModel.UserAddressDataViewModel.PortalCatalogId;
            shoppingCartModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

            SaveCartModelInSession(cartParameter.UserId, shoppingCartModel, isQuote);
            return shoppingCartModel;
        }

        /// <summary>
        /// To remove the LocalId filter from filtercollection
        /// </summary>
        /// <param name="filters"></param>
        protected virtual void RemoveLocalIdFromFilters(FilterCollection filters)
        {
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.LocaleId.ToString());
            }
        }


        protected virtual void SetCartValueInOrderModel(OrderModel orderModel)
        {
            if (orderModel.ReturnItemList?.ReturnItemList?.Count > 0)
                orderModel.ReturnItemList.CSRDiscount = Convert.ToDecimal(orderModel?.ShoppingCartModel.ReturnCSRDiscount.ToPriceRoundOff());
        }

        protected virtual bool IsShippingRecalculateRequired(AddressViewModel updatedShippingAddress, string omsStateCode, string omsPostalCode)
        {
            bool isShippingRecalculateRequired = false;
            if (HelperUtility.IsNotNull(updatedShippingAddress))
            {
                if (updatedShippingAddress.StateName != omsStateCode ||
                    updatedShippingAddress.PostalCode != omsPostalCode)
                    isShippingRecalculateRequired = true;
            }
            return isShippingRecalculateRequired;
        }

        //Update the shipping amount and handling charge of order
        protected virtual void UpdateOrderLineItemShippingAmount(ShoppingCartModel cart, ShoppingCartItemModel cartItemModel, ManageOrderDataModel orderDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(cartItemModel))
            {
                cartItemModel.PerQuantityShippingCost = orderDataModel.OrderLineItemShippingCost / (orderDataModel.CustomQuantity > 0 ? orderDataModel.CustomQuantity : orderDataModel.Quantity);
                cart.IsRemoveShippingDiscount = true;
                cart.ShippingCost = 0m;
                cart.ShippingHandlingCharges = 0m;
                cart.Shipping.ShippingDiscount = 0m;
                cart.Shipping.ShippingHandlingCharge = 0m;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set order line history for edit shipping
        protected virtual void SetEditShippingOrderHistory(ShoppingCartModel cart, ShoppingCartItemModel cartItemModel, ManageOrderDataModel orderDataModel, OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            string sku = string.Empty;
            string key = string.Empty;
            decimal originalShippingAmount = 0;
            OrderLineItemHistoryModel orderLineItemHistoryModel = new OrderLineItemHistoryModel();

            sku =  (orderDataModel.ProductId > 0)
                    ? cartItemModel.GroupProducts?.FirstOrDefault(y => y.ProductId == orderDataModel.ProductId)?.Sku
                    : !string.IsNullOrEmpty(cartItemModel.ConfigurableProductSKUs) ? cartItemModel.ConfigurableProductSKUs : cartItemModel.SKU;

            key = $"{sku}_EditShipping";
            originalShippingAmount = orderDataModel.OriginalOrderLineItemShippingCost;

            if (HelperUtility.IsNotNull(orderModel.OrderLineItemHistory) && orderModel.OrderLineItemHistory.ContainsKey(key))
            {
                originalShippingAmount = orderModel.OrderLineItemHistory.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.InvariantCultureIgnoreCase)).Value.OriginalOrderLineItemShippingCost;
                orderModel.OrderLineItemHistory.Remove(key);
            }

            orderLineItemHistoryModel.SKU = sku;
            orderLineItemHistoryModel.NewOrderLineItemShippingCost = orderDataModel.OrderLineItemShippingCost;
            orderLineItemHistoryModel.IsOrderLineItemShippingUpdate = true;
            orderLineItemHistoryModel.ProductName = cartItemModel.ProductName;
            orderLineItemHistoryModel.OmsOrderLineItemsId = cartItemModel.OmsOrderLineItemsId;
            orderLineItemHistoryModel.OriginalOrderLineItemShippingCost = originalShippingAmount;

            OrderLineHistory(orderModel, key, orderLineItemHistoryModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        protected virtual void SetAmountWithCurrency(string cultureCode, PaymentHistoryListViewModel paymentHistoryListViewModel)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CultureCode = cultureCode });
            if (IsNotNull(paymentHistoryListViewModel))
            {
                foreach (PaymentHistoryViewModel paymentHistory in paymentHistoryListViewModel.List)
                {
                    paymentHistory.OrderAmountWithCurrency = IsNull(paymentHistory.Amount) ? string.Empty : HelperMethods.FormatPriceWithCurrency(paymentHistory.Amount, cultureCode);
                    paymentHistory.RemainingAmountWithCurrency = IsNull(paymentHistory.RemainingOrderAmount) ? string.Empty : HelperMethods.FormatPriceWithCurrency(paymentHistory.RemainingOrderAmount, cultureCode);
                    paymentHistory.OrderDateWithTime = Convert.ToDateTime(paymentHistory.CreatedDate).ToString(HelperMethods.GetStringDateTimeFormat());
                }
            }
        }

        //Set subtotal and ordertotal in model
        protected virtual void SetCartSummaryDefaultValues(CartViewModel cartViewModel)
        {
            cartViewModel.SubTotal = cartViewModel.SubTotal ?? 0;
            cartViewModel.OrderTotalWithoutVoucher = cartViewModel.OrderTotalWithoutVoucher ?? 0;
        }

        //Get the current logged in user id from the session
        private Int32 GetSessionUser()
        {
            Int32 sessionUserId = 0;
            Int32.TryParse(SessionProxyHelper.GetUserDetails()?.UserId.ToString(), out sessionUserId);
            return sessionUserId;
        }
        #endregion Protected Methods
    }
}