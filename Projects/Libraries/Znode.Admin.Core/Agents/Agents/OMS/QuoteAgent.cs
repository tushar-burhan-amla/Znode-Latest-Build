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
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin.Agents
{
    public class QuoteAgent : BaseAgent, IQuoteAgent
    {
        #region Private Variable       
        private readonly IQuoteClient _quoteClient;
        private readonly IOrderStateClient _orderStateClient;
        private readonly IOrderClient _orderClient;
        private readonly IShoppingCartClient _shoppingCartClient;
        private readonly IUserClient _userClient;
        private readonly IShippingClient _shippingClient;
        private readonly ICustomerClient _customerClient;
        private readonly IAccountClient _accountClient;
        private readonly IPortalClient _portalClient;
        private readonly ICartAgent _cartAgent;
        private readonly IGlobalAttributeEntityClient _globalAttributeEntityClient;
        #endregion Private Variable

        #region Constructor

        public QuoteAgent(IQuoteClient quoteClient, IOrderStateClient orderStateClient, IOrderClient orderClient, IShoppingCartClient shoppingCartClient,
            IUserClient userClient, IShippingClient shippingClient,
            IShippingAgent shippingAgent, ICustomerClient customerClient, IAccountClient accountClient, IPortalClient portalClient, IGlobalAttributeEntityClient globalAttributeEntityClient)
        {
            _quoteClient = GetClient<IQuoteClient>(quoteClient);
            _orderClient = GetClient<IOrderClient>(orderClient);
            _orderStateClient = GetClient<IOrderStateClient>(orderStateClient);
            _shoppingCartClient = GetClient<IShoppingCartClient>(shoppingCartClient);
            _userClient = GetClient<IUserClient>(userClient);
            _shippingClient = GetClient<IShippingClient>(shippingClient);
            _customerClient = GetClient<ICustomerClient>(customerClient);
            _accountClient = GetClient<IAccountClient>(accountClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _cartAgent = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<OrderStateClient>(), GetClient<PortalClient>(), GetClient<UserClient>());
            _globalAttributeEntityClient = GetClient<IGlobalAttributeEntityClient>(globalAttributeEntityClient);
        }

        #endregion Constructor

        #region Public Methods
        //Get Quote List
        public virtual QuoteListViewModel GetQuoteList(FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            try
            {
                //Remove DateTimeRange Filter From Cookie.
                DateRangePickerHelper.RemoveDateTimeRangeFiltersFromCookies(GridListType.ZnodeOmsRequestQuote.ToString(), model);

                FilterCollection _filters = new FilterCollection();
                _filters.AddRange(model.Filters);

                int portalId = GetPortalIdFromFilters(_filters);
                if (portalId <= 0)
                {
                    RemoveFilters(_filters, FilterKeys.PortalId);
                }

                //Set QuoteTypeId Filter.
                SetQuoteTypeIdFilter(_filters);

                DateRangePickerHelper.FormatFilterForDateTimeRange(_filters, DateTimeRange.Last_30_Days.ToString(), DateTimeRange.All_Orders.ToString());

                ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = _filters, Sorts = model.SortCollection });
                QuoteListModel quoteList = _quoteClient.GetQuoteList(_filters, model.SortCollection, model.Page, model.RecordPerPage);
                QuoteListViewModel quoteListViewModel = new QuoteListViewModel { Quotes = quoteList?.Quotes?.ToViewModel<QuoteViewModel>()?.ToList(), PortalName = string.IsNullOrEmpty(quoteList.PortalName) ? Admin_Resources.DefaultAllStores : quoteList.PortalName, PortalId = portalId };

                if (quoteList?.Quotes?.Count > 0 && IsNotNull(quoteListViewModel))
                {
                    SetListPagingData(quoteListViewModel, quoteList);
                }


                //Get the grid model.
                quoteListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteListViewModel?.Quotes, GridListType.ZnodeOmsRequestQuote.ToString(), string.Empty, null, true, true, null);
                quoteListViewModel.GridModel.TotalRecordCount = quoteListViewModel.TotalResults;

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return quoteListViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (QuoteListViewModel) GetViewModelWithErrorMessage(new QuoteListViewModel(), Admin_Resources.ProcessingFailedError);
            }

        }

        //Get quote details by quote id.
        public virtual QuoteResponseViewModel GetQuoteDetails(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (omsQuoteId > 0)
                {
                    if (omsQuoteId > 0)
                    {
                        QuoteResponseModel quote = _quoteClient.GetQuoteById(omsQuoteId);
                        if (IsNotNull(quote))
                        {
                            return FillQuoteDetails(omsQuoteId, quote);
                        }
                    }
                    return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), Admin_Resources.QuoteProcessingFailedError);
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorAccessMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), Admin_Resources.QuoteProcessingFailedError); ;
            }
            return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), Admin_Resources.QuoteProcessingFailedError); ;
        }

        //Get Quote Status List
        public virtual OrderStatusList BindManageQuoteStatus(string quoteStatus, int omsQuoteId)
        {
            try
            {
                if (!string.IsNullOrEmpty(quoteStatus) && omsQuoteId > 0)
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { QuoteStatus = quoteStatus });
                    List<SelectListItem> statusList;
                    statusList = GetQuoteStatusList(new FilterTuple(ZnodeOmsOrderStateEnum.IsQuoteState.ToString().ToLower(), FilterOperators.Equals, "true"));
                    return new OrderStatusList()
                    {
                        listItem = statusList,
                        SelectedItemValue = quoteStatus,
                        OmsQuoteId = omsQuoteId,
                        SelectedItemId = Convert.ToInt32(statusList.FirstOrDefault(x => x.Text.ToLower() == quoteStatus.ToLower()).Value)
                    };
                }
                else
                    return new OrderStatusList();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (OrderStatusList)GetViewModelWithErrorMessage(new OrderStatusList(), Admin_Resources.QuoteStatusErrorMessage);
            }
        }

        // Update Quote Status in session.
        public virtual OrderStatusList UpdateQuoteStatus(OrderStatusList quoteStatus)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(quoteStatus) && quoteStatus?.OmsQuoteId > 0)
                {
                    ZnodeLogging.LogMessage("QuoteStatusList model with SelectedItemId and OmsQuoteId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SelectedItemId = quoteStatus?.SelectedItemId, OmsQuoteId = quoteStatus?.OmsQuoteId });
                    QuoteSessionModel quoteSessionModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + quoteStatus?.OmsQuoteId);

                    if (IsNotNull(quoteSessionModel))
                    {
                        OrderStateListModel quoteStateListModel = _orderStateClient.GetOrderStates(null, null, null, null, null);
                        RemoveKeyFromDictionary(quoteSessionModel, ZnodeConstant.QuoteUpdatedStatus);

                        UpdateQuoteHistory(quoteSessionModel, ZnodeConstant.QuoteUpdatedStatus, quoteStatus.SelectedItemValue);

                        quoteSessionModel.OmsQuoteStateId = quoteStatus.SelectedItemId;
                        quoteStatus.SuccessMessage = Admin_Resources.QuoteStatusUpdated;

                        SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + quoteStatus?.OmsQuoteId, quoteSessionModel);
                    }
                }
                else
                {
                    quoteStatus.HasError = true;
                    quoteStatus.ErrorMessage = Admin_Resources.ErrorMessageFailedStatus;
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return quoteStatus;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                quoteStatus.HasError = true;
                quoteStatus.ErrorMessage = Admin_Resources.ErrorMessageFailedStatus;
                return quoteStatus;
            }

        }

        // Get Quote status Edit value.
        public virtual bool GetQuoteStateValueById(int omsQuoteStateId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (omsQuoteStateId > 0)
                {
                    return _orderClient.GetOrderStateValueById(omsQuoteStateId).IsEdit;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Update Customer Shipping's Shipping Account Number.
        public virtual bool UpdateShippingAccountNumber(int omsQuoteId, string shippingAccountNumber)
        {
            try
            {
                if (omsQuoteId > 0 && !(string.IsNullOrEmpty(shippingAccountNumber)))
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId, ShippingAccountNumber = shippingAccountNumber });
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);

                    if (IsNull(quoteModel))
                        return false;

                    RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderShippingAccountNumber);
                    UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderShippingAccountNumber, quoteModel.AccountNumber);

                    quoteModel.AccountNumber = shippingAccountNumber;
                    SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
        }

        //Update Customer Shipping's Shipping Method.
        public virtual bool UpdateShippingMethod(int omsQuoteId, string shippingMethod)
        {
            try
            {
                if (omsQuoteId > 0 && !(string.IsNullOrEmpty(shippingMethod)))
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId, ShippingMethod = shippingMethod });
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);

                    if (IsNull(quoteModel))
                        return false;

                    RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderShippingMethod);
                    UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderShippingMethod, quoteModel.ShippingMethod);

                    quoteModel.ShippingMethod = shippingMethod;
                    SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
        }

        //Update the InHandDate in session against the omsQuoteId.
        public virtual bool UpdateInHandDate(int omsQuoteId, DateTime InHandDate)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId });
            try
            {
                if (omsQuoteId > 0 && IsNotNull(InHandDate))
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);

                    if (IsNull(quoteModel))
                        return false;

                    RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderInHandsDate);
                    UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderInHandsDate, quoteModel.InHandDate.ToString());

                    quoteModel.InHandDate = InHandDate;
                    SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
        }

        //Update the QuoteExpirationDate in session against the omsQuoteId.
        public virtual bool UpdateQuoteExpirationDate(int omsQuoteId, DateTime QuoteExpirationDate)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId });
            try
            {
                if (omsQuoteId > 0 && IsNotNull(QuoteExpirationDate))
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);

                    if (IsNull(quoteModel))
                        return false;

                    RemoveKeyFromDictionary(quoteModel, ZnodeConstant.QuoteExpirationDate);
                    UpdateQuoteHistory(quoteModel, ZnodeConstant.QuoteExpirationDate, quoteModel.QuoteExpirationDate.ToString());

                    quoteModel.QuoteExpirationDate = QuoteExpirationDate;
                    SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
        }

        //Get Shipping list for manage quote
        public virtual ShippingListViewModel GetShippingListForManage(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { QuoteId = omsQuoteId });
            try
            {
                if (omsQuoteId > 0)
                {
                    QuoteSessionModel quoteSessionModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                    if (IsNotNull(quoteSessionModel))
                    {
                        ShoppingCartModel shoppingCart = GetShoppingCartModel(quoteSessionModel);

                        return GetShippingList(shoppingCart);
                    }
                }
                return new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (ShippingListViewModel)GetViewModelWithErrorMessage(new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() }, Admin_Resources.ErrorShippingOptionGet);
            }
        }

        //Calculate Shipping Cost for manage Quote
        public virtual QuoteCartViewModel GetShippingChargesForManage(int omsQuoteId, int shippingId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingId = shippingId, OmsQuoteId = omsQuoteId });
            try
            {
                if(omsQuoteId > 0 && shippingId > 0)
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                    if (IsNull(quoteModel))
                        return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.UnableToUpdateShippingMethod);

                    ShoppingCartModel cart = GetShoppingCartModel(quoteModel);
                    if (IsNotNull(cart))
                    {
                        //Get Shipping details by shipping id.
                        string oldShippingType = _shippingClient.GetShipping(quoteModel.CartInformation.ShippingId)?.Description;

                        RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderShippingType);
                        UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderShippingType, oldShippingType);

                        //To get the shipping cost as per selected shipping options
                        ResetCustomShippingCost(cart);

                        //Assign shipping details to cart, quoteModel and Calculate cart.
                        cart = MapShippingDetailsAndCalculateCart(shippingId, cart, quoteModel);

                        quoteModel.CartInformation = cart.ToViewModel<QuoteCartViewModel>();

                        //Save Cart details in session.
                        SaveInSession(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);

                        if (!string.IsNullOrEmpty(cart.Shipping.ResponseMessage))
                        {
                            quoteModel.CartInformation.HasError = true;
                            quoteModel.CartInformation.ShippingErrorMessage = cart.Shipping.ResponseMessage;
                        }
                        return quoteModel.CartInformation;
                    }
                }
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.UnableToUpdateShippingMethod);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.UnableToUpdateShippingMethod);
            }
        }

        //Update Customer Shipping Cost
        public virtual QuoteCartViewModel UpdateShippingHandling(int omsQuoteId, string shippingCost)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { QuoteId = omsQuoteId, shippingCost = shippingCost });
            try
            {
                if (omsQuoteId < 0)
                    return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.UnableToUpdateShippingMethod);

                QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                shippingCost = string.IsNullOrEmpty(shippingCost) ? "0.0" : shippingCost;

                if (IsNotNull(quoteModel))
                {
                    ShoppingCartModel cart = GetShoppingCartModel(quoteModel);

                    if (IsNotNull(cart) && IsNotNull(quoteModel))
                    {
                        RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderShippingCost);
                        UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderShippingCost, Convert.ToString(quoteModel.CartInformation.ShippingCost));

                        cart.CustomShippingCost = Convert.ToDecimal(shippingCost);
                        cart = GetCalculatedShoppingCartForEditQuote(cart);

                        quoteModel.CartInformation = cart.ToViewModel<QuoteCartViewModel>();

                        if (!string.IsNullOrEmpty(cart.Shipping.ResponseMessage))
                        {
                            quoteModel.CartInformation.HasError = true;
                            quoteModel.CartInformation.ShippingErrorMessage = cart.Shipping.ResponseMessage;
                        }

                        SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                        ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                        return quoteModel.CartInformation;
                    }
                }
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.UnableToUpdateShippingMethod);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.UnableToUpdateShippingMethod);
            }
        }

        //Get Shopping Cart.
        public virtual QuoteCartViewModel GetQuoteShoppingCart(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (omsQuoteId > 0)
                {
                    QuoteSessionModel quoteSessionModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);

                    if (IsNotNull(quoteSessionModel))
                    {
                        ShoppingCartModel cartModel = GetShoppingCartModel(quoteSessionModel);

                        if (IsNotNull(cartModel))
                            return cartModel?.ToViewModel<QuoteCartViewModel>();
                    }
                }
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.ErrorFailedToLoadCart);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.ErrorFailedToLoadCart);
            }
        }

        //Get address details of customer.
        public virtual AddressViewModel GetUserAddressForManageQuote(ManageAddressModel addressDetail)
        {
            try
            {
                if (IsNotNull(addressDetail))
                {
                    AddressViewModel addresViewModel = GetAddressDefaultData(addressDetail, GetCustomerAccountId(addressDetail.UserId));
                    if (IsNotNull(addresViewModel))
                    {
                        addresViewModel?.UsersAddressNameList?.Insert(0, new SelectListItem { Text = AdminConstants.AddNewAddress, Value = AdminConstants.Zero.ToString() });

                        addresViewModel.IsShippingBillingDifferent = true;
                        addresViewModel.SelectedBillingId = addressDetail.BillingAddressId;
                        addresViewModel.SelectedShippingId = addressDetail.ShippingAddressId;
                        addresViewModel.FromBillingShipping = addressDetail.FromBillingShipping;
                        addresViewModel.OmsQuoteId = addressDetail.OmsQuoteId;
                        if ((addresViewModel.IsShipping && addresViewModel.IsBilling) || (addresViewModel.IsDefaultBilling && addresViewModel.IsDefaultShipping))
                        {
                            addresViewModel.IsShippingBillingDifferent = false;
                        }
                        return addresViewModel;
                    }
                }
                return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), Admin_Resources.AddressErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), Admin_Resources.AddressErrorMessage);
            }
        }

        //Get Address Default Data.
        public virtual AddressViewModel GetAddressDefaultData(ManageAddressModel addressDetail, int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = addressDetail.SelectedAddressId, PortalId = addressDetail.PortalId });
            try
            {
                if (IsNotNull(addressDetail))
                {
                    AddressListModel addressList = GetAddressListOfUserAndAccount(addressDetail.UserId, accountId);

                    //Get address from address list by selected address id.
                    AddressViewModel addressViewModel = addressList?.AddressList?.FirstOrDefault(w => w.AddressId == addressDetail.SelectedAddressId)?.ToViewModel<AddressViewModel>();
                    if (IsNull(addressViewModel))
                    {
                        addressViewModel = new AddressViewModel();
                        addressList = GetAddressListByAddressId(addressDetail.SelectedAddressId);
                        addressViewModel = (addressList?.AddressList?.Where(w => w.AddressId == addressDetail.SelectedAddressId).FirstOrDefault())?.ToViewModel<AddressViewModel>();
                    }
                    else
                        addressViewModel.DontAddUpdateAddress = false;

                    //Get User address name list to bind in dropdown.
                    addressViewModel.UsersAddressNameList = OrderViewModelMap.ToListItems(addressList.AddressList);

                    //Get portal associated country dropdown.
                    addressViewModel.Countries = HelperMethods.GetPortalAssociatedCountries(addressDetail.PortalId);

                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return addressViewModel;
                }
                return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), Admin_Resources.AddressErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), Admin_Resources.AddressErrorMessage);
            }
        }

        //Update Customer Address And Calculate.
        public virtual QuoteSessionModel UpdateCustomerAddressAndCalculate(AddressViewModel addressViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if(IsNotNull(addressViewModel))
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + addressViewModel.OmsQuoteId);
                    if (IsNull(quoteModel))
                        return (QuoteSessionModel)GetViewModelWithErrorMessage(new QuoteSessionModel(), Admin_Resources.AddressUpdateErrorMessage);


                    quoteModel.PortalId = addressViewModel.PortalId;
                    //validate address
                    BooleanModel booleanModel = IsValidateAddress(quoteModel, addressViewModel);
                    if (IsNotNull(booleanModel) && !booleanModel.IsSuccess)
                        return (QuoteSessionModel)GetViewModelWithErrorMessage(quoteModel, booleanModel.ErrorMessage ?? Admin_Resources.AddressValidationFailed);

                    AddressViewModel updatedAddress = UpdatedAddress(addressViewModel);
                    if (IsNull(updatedAddress))
                        return (QuoteSessionModel)GetViewModelWithErrorMessage(new QuoteSessionModel(), Admin_Resources.AddressUpdateErrorMessage);

                    updatedAddress.IsShippingBillingDifferent = addressViewModel.IsShippingBillingDifferent;

                    AddressListModel addressList = SetBillingshippingId(addressViewModel);

                    if (IsNotNull(addressList))
                        SetAddressToQuoteModel(addressViewModel, quoteModel, updatedAddress, addressList);

                    //Set shipping address, ShippingCountryCode, and calculate.
                    SetCartModelAndCalculate(addressViewModel, quoteModel, addressViewModel.OmsQuoteId, addressViewModel?.FromBillingShipping);

                    return quoteModel;
                }
                return (QuoteSessionModel)GetViewModelWithErrorMessage(new QuoteSessionModel(), Admin_Resources.AddressUpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (QuoteSessionModel)GetViewModelWithErrorMessage(new QuoteSessionModel(), Admin_Resources.AddressUpdateErrorMessage);
            }
        }

        //Map Updated Customer Address.
        public virtual CustomerInfoViewModel MapUpdatedCustomerAddress(int omsQuoteId)
        {
            if(omsQuoteId > 0)
            {
                QuoteSessionModel quoteSessionModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                if (IsNotNull(quoteSessionModel))
                {
                    return new CustomerInfoViewModel
                    {
                        CustomerName = quoteSessionModel?.CustomerInformation?.CustomerName,
                        UserName = quoteSessionModel?.CustomerInformation?.UserName,
                        CustomerId = quoteSessionModel.UserId,
                        ShippingAddress = quoteSessionModel.ShippingAddressModel?.ToViewModel<AddressViewModel>(),
                        BillingAddress = quoteSessionModel.BillingAddressModel?.ToViewModel<AddressViewModel>(),
                        OmsQuoteId = quoteSessionModel.OmsQuoteId,
                        Email = quoteSessionModel?.CustomerInformation?.Email,
                        PhoneNumber = quoteSessionModel?.CustomerInformation?.PhoneNumber
                    };
                }
            }
            return new CustomerInfoViewModel();
        }

        // Removes cart item from the shopping cart.
        public virtual bool RemoveQuoteCartItem(int omsQuoteId, string guid)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (omsQuoteId > 0 && !string.IsNullOrEmpty(guid))
                {
                    // Get cart from Session.
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                    if (IsNull(quoteModel))
                        return false;

                    ShoppingCartModel cart = GetShoppingCartModel(quoteModel);

                    if (IsNull(cart) || (cart?.ShoppingCartItems?.Count <= 1) || IsNull(cart?.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == guid)))
                        return false;

                    ShoppingCartItemModel item = cart?.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == guid);

                    //Calculate CustomShippingCost 
                    cart.CustomShippingCost = cart?.ShippingCost - item?.ShippingCost;

                    // Remove item 
                    cart?.ShoppingCartItems?.Remove(item);
                    RemoveHistoryForCartLineItem(item, quoteModel);

                    //Save Cart details in session.
                    QuoteCartViewModel cartModel = cart.ToViewModel<QuoteCartViewModel>();
                    quoteModel.CartInformation = cartModel;
                    SaveInSession(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
        }

        //Calculated shopping cart
        public virtual QuoteCartViewModel CalculateShoppingCart(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (omsQuoteId > 0)
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                    if (IsNull(quoteModel))
                        return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.ErrorFailedToCalculateCart);

                    ShoppingCartModel cartModel = GetCalculatedShoppingCartForEditQuote(GetShoppingCartModel(quoteModel));
                    quoteModel.CartInformation = cartModel.ToViewModel<QuoteCartViewModel>();

                    SaveInSession(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                    return quoteModel.CartInformation;
                }
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.ErrorFailedToCalculateCart);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.ErrorFailedToCalculateCart);
            }
        }

        //To change list of parameters into model
        public virtual QuoteCartViewModel UpdateQuoteLineItemDetails(ManageOrderDataModel quoteDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(quoteDataModel))
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + quoteDataModel.OmsQuoteId);
                    if (IsNotNull(quoteModel))
                    {
                        ShoppingCartModel cart = GetShoppingCartModel(quoteModel);
                        if (IsNotNull(cart) && cart?.ShoppingCartItems?.Count > 0)
                        {
                            // Check if item exists.
                            ShoppingCartItemModel cartItem = cart?.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == quoteDataModel.Guid);
                            //If line item shipping change
                            if (quoteDataModel.IsShippingEdit)
                            {
                                cart.CustomShippingCost = cart.ShippingCost - cartItem.ShippingCost + quoteDataModel.ShippingCost;
                            }

                            if (IsNotNull(cartItem))
                            {
                                QuoteCartViewModel cartViewModel = ValidateCartItemInventory(quoteModel, cartItem, quoteDataModel, cart);
                                if (IsNotNull(cartViewModel) && cartViewModel.HasError)
                                {
                                    GetCalculatedCartForLineItem(cart, quoteDataModel, quoteModel, cartItem);
                                    return cartViewModel;
                                }                                
                                GetCalculatedCartForLineItem(cart, quoteDataModel, quoteModel, cartItem);
                                return quoteModel.CartInformation;
                            }
                        }
                    }
                }
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.ErrorFailedToUpdate);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), Admin_Resources.ErrorFailedToUpdate);
            }
        }

        //Update Tax Exempt for Manage
        public virtual QuoteCartViewModel UpdateTaxExemptForManage(int omsQuoteId, bool isTaxExempt)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId, IsTaxExempt = isTaxExempt });
            try
            {
                if (omsQuoteId > 0)
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                    if (IsNotNull(quoteModel))
                    {
                        ShoppingCartModel cart = GetShoppingCartModel(quoteModel);

                        if (IsNotNull(cart))
                        {
                            if (isTaxExempt)
                            {
                                cart.CustomTaxCost = Convert.ToDecimal(0);
                                cart.IsTaxCostEdited = true;
                                quoteModel.CartInformation.IsTaxExempt = true;

                                RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderTax);
                                UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderTax, Convert.ToString(quoteModel.CartInformation.TaxCost));
                            }
                            else
                            {
                                cart.CustomTaxCost = null;
                                cart.IsTaxCostEdited = false;
                                quoteModel.CartInformation.IsTaxExempt = false;

                                RemoveKeyFromDictionary(quoteModel, ZnodeConstant.QuoteUnexemptedTax);
                                UpdateQuoteHistory(quoteModel, ZnodeConstant.QuoteUnexemptedTax, Convert.ToString(quoteModel.CartInformation.TaxCost));
                            }

                            cart = GetCalculatedShoppingCartForEditQuote(cart);
                            quoteModel.CartInformation = cart.ToViewModel<QuoteCartViewModel>();
                            quoteModel.CartInformation.IsTaxExempt = isTaxExempt;
                            SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                            return quoteModel.CartInformation;
                        }
                    }
                }
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), "Failed to Update Tax Cost");
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return (QuoteCartViewModel)GetViewModelWithErrorMessage(new QuoteCartViewModel(), "Failed to Update Tax Cost");
            }
        }

        //Get quote details by quote id.
        public virtual QuoteResponseViewModel PrintManageQuote(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (omsQuoteId > 0)
                {
                    IPortalClient _portalClient = GetClient<PortalClient>();

                    QuoteSessionModel quoteSessionModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                    if (IsNotNull(quoteSessionModel))
                    {
                        QuoteResponseViewModel quoteViewModel = quoteSessionModel.ToModel<QuoteResponseViewModel>();
                        if (IsNotNull(quoteViewModel))
                        {
                            PortalModel portalModel = _portalClient.GetPortal(quoteViewModel.PortalId, null);
                            if (IsNotNull(portalModel))
                            {
                                quoteViewModel.CustomerServiceEmail = portalModel.CustomerServiceEmail;
                                quoteViewModel.CustomerServicePhoneNumber = portalModel.CustomerServicePhoneNumber;
                            }
                            return quoteViewModel;
                        }
                    }
                }
                return new QuoteResponseViewModel();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), ex.Message);
            }
        }

        //Update the Job Name in session against the omsQuoteId
        public virtual bool UpdateJobName(int omsQuoteId, string jobName)
        {
            ZnodeLogging.LogMessage("Agent UpdateJobName method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId, JobName = jobName });
            try
            {
                if (omsQuoteId > 0)
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                    if (IsNotNull(quoteModel))
                    {
                        RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderJobName);
                        UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderJobName, jobName);

                        quoteModel.JobName = jobName;
                        SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                        ZnodeLogging.LogMessage("Agent UpdateJobName method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
        }

        //Update the Shipping Constraint Code in session against the omsQuoteId
        public virtual bool UpdateShippingConstraintCode(int omsQuoteId, string shippingConstraintCode)
        {
            ZnodeLogging.LogMessage("Agent UpdateShippingConstraintCode method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId, ShippingConstraintCode = shippingConstraintCode });
            try
            {
                if (omsQuoteId > 0)
                {
                    QuoteSessionModel quoteModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                    if (IsNotNull(quoteModel))
                    {
                        RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderShippingConstraintCode);
                        UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderShippingConstraintCode,
                            GetEnumDescriptionValue((ShippingConstraintsEnum)Enum.Parse(typeof(ShippingConstraintsEnum), shippingConstraintCode)));

                        quoteModel.ShippingConstraintCode = shippingConstraintCode;
                        SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);
                        ZnodeLogging.LogMessage("Agent UpdateShippingConstraintCode method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
        }

        // Update manage Quote.
        public virtual BooleanModel UpdateQuote(int omsQuoteId, string additionalNote)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId, AdditionalNote = additionalNote });
            try
            {
                QuoteSessionModel quoteSessionModel = GetFromSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                if (IsNotNull(quoteSessionModel))
                {
                    UpdateQuoteModel quoteModel = quoteSessionModel.ToModel<UpdateQuoteModel>();
                    quoteModel.AdditionalInstructions = additionalNote;
                    quoteModel.ShippingHandlingCharges = quoteSessionModel.CartInformation.ShippingHandlingCharges;
                    if (ValidateQuoteOnManage(quoteSessionModel, additionalNote))
                    {
                        GetQuoteHistory(quoteSessionModel, quoteModel);
                        BooleanModel isQuoteUpdated = _quoteClient.UpdateQuote(quoteModel);
                        if (isQuoteUpdated.IsSuccess)
                        {
                            RemoveInSession(AdminConstants.OMSQuoteSessionKey + omsQuoteId);
                            return isQuoteUpdated;
                        }
                    }
                    else
                        return new BooleanModel { IsSuccess = true };
                }
                return new BooleanModel { IsSuccess = false , ErrorMessage = Admin_Resources.ErrorQuoteModelNull };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return new BooleanModel { IsSuccess = true, ErrorMessage = Admin_Resources.QuoteProcessingFailedError };
            }
        }

        //Get Portal Catalog By Portal Id.
        public virtual void SetPortalAndUserDetails(int portalId, UserAddressDataViewModel userAddressDataViewModel)
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

        //Get list of Portals, Catalogs and Accounts on the basis of which create new order.
        public virtual QuoteCreateViewModel GetCreateQuoteDetails(int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            QuoteCreateViewModel createQuoteViewModel = new QuoteCreateViewModel();

            //Get All Portal list.
            createQuoteViewModel.PortalList = OrderViewModelMap.ToListItems(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);

            createQuoteViewModel.UserAddressDataViewModel = new UserAddressDataViewModel()
            {
                ShippingAddress = new AddressViewModel(),
                BillingAddress = new AddressViewModel(),
            };

            //Get Portal id from list.
            portalId = portalId.Equals(0) ? createQuoteViewModel.PortalList?.Count > 0 ? Convert.ToInt32(createQuoteViewModel.PortalList.First().Value) : 0 : portalId;

            // Get Publish catalog id by First portal id.
            SetPortalAndUserDetails(portalId, createQuoteViewModel.UserAddressDataViewModel);

            IAdminHelper _adminHelper = GetService<IAdminHelper>();

            createQuoteViewModel.ShippingListViewModel = new ShippingListViewModel()
            {
                ShippingList = new List<ShippingViewModel>(),
                InHandDate = _adminHelper.GetInHandDate(),
                ShippingConstraints = _adminHelper.GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete)
            };

            //Set Quote Expiration Date
            GlobalAttributeEntityDetailsModel globalAttributes = _globalAttributeEntityClient.GetEntityAttributeDetails(portalId, ZnodeConstant.Store);
            int quoteExpiredInDays = Convert.ToInt32(globalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, "QuoteExpireInDays", StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);
            createQuoteViewModel.QuoteExpirationDate = _adminHelper.GetQuoteExpirationDate(quoteExpiredInDays);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return createQuoteViewModel;
        }

        //Submit quote request.
        public virtual QuoteCreateViewModel SubmitQuote(QuoteCreateViewModel quoteCreateViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(quoteCreateViewModel))
            {
                //Sets the user id for quote.
                SetCreatedByUser(quoteCreateViewModel.UserId);

                //Get the cart from session.
                ShoppingCartModel cartModel = GetCartModelFromSession(quoteCreateViewModel?.UserId, true) ??
                           _cartAgent.GetCartFromCookie();

                if (IsNotNull(cartModel))
                {
                    //Bind address to check address is valid or not in case of enableaddressvalidation flag on for store.
                    quoteCreateViewModel.UserAddressDataViewModel = new UserAddressDataViewModel() { ShippingAddress = cartModel.ShippingAddress?.ToViewModel<AddressViewModel>() };
                    BooleanModel booleanModel = IsValidAddress(quoteCreateViewModel);
                    if (!booleanModel.IsSuccess)
                        return (QuoteCreateViewModel)GetViewModelWithErrorMessage(quoteCreateViewModel, booleanModel.ErrorMessage ?? Admin_Resources.AddressValidationFailed);

                    try
                    {
                        //Map all data required to submit quote to shopping cart model
                        SetShoppingCartDetailsForQuotes(quoteCreateViewModel, cartModel);

                        quoteCreateViewModel.OmsQuoteStatus = ZnodeOrderStatusEnum.SUBMITTED.ToString();
                        if ((cartModel.ShoppingCartItems?.Count > 0) && cartModel.ShoppingCartItems.Any(x => x.CustomUnitPrice > 0))
                        {
                            quoteCreateViewModel.OmsQuoteStatus = ZnodeConstant.IN_REVIEW.ToString();
                        }

                        quoteCreateViewModel.QuoteTypeCode = ZnodeConstant.Quote;
                        quoteCreateViewModel.PublishStateId = (byte)ZnodePublishStatesEnum.PRODUCTION;

                        //Create Quote.
                        QuoteCreateViewModel createdQuote = _quoteClient.Create(quoteCreateViewModel.ToModel<QuoteCreateModel>()).ToViewModel<QuoteCreateViewModel>();

                        if(IsNotNull(createdQuote))
                        {
                            RemoveInSession(ZnodeConstant.PendingQuotesKey + quoteCreateViewModel.UserId);
                        }
                        if (IsNull(createdQuote))
                            return (QuoteCreateViewModel)GetViewModelWithErrorMessage(quoteCreateViewModel, Admin_Resources.ErrorSubmitQuote);

                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        return (QuoteCreateViewModel)GetViewModelWithErrorMessage(new QuoteCreateViewModel(), Admin_Resources.ErrorFailedToCreate);
                    }
                }
                return quoteCreateViewModel;
            }
            return new QuoteCreateViewModel();
        }

        // Convert Quote to Order.
        public virtual OrdersViewModel SaveAndConvertQuoteToOrder(ConvertQuoteToOrderViewModel convertToOrderModel)
        {
            try
            {
                if (IsNotNull(convertToOrderModel) && convertToOrderModel.OmsQuoteId > 0)
                {
                    BooleanModel isQuoteUpdated = UpdateQuote(convertToOrderModel.OmsQuoteId, convertToOrderModel.AdditionalInstructions);
                    if (isQuoteUpdated.IsSuccess)
                    {
                        OrdersViewModel ordersViewModel = _quoteClient.ConvertQuoteToOrder(convertToOrderModel.ToModel<ConvertQuoteToOrderModel>())?.ToViewModel<OrdersViewModel>();
                        return ordersViewModel;
                    }
                    else
                    {
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), isQuoteUpdated.ErrorMessage);
                    }
                }
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), Admin_Resources.ConvertQuoteToOrderErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), Admin_Resources.ProcessingFailedError);
            }
        }
        #endregion

        #region Private Methods
        // Set QuoteTypeId Filter.
        private void SetQuoteTypeIdFilter(FilterCollection filters)
        {
            filters?.RemoveAll(x => string.Equals(x.FilterName, ZnodeOmsQuoteEnum.OmsQuoteTypeId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            filters.Add(new FilterTuple(FilterKeys.OmsQuoteTypeId, FilterOperators.Equals, AdminConstants.OmsQuoteTypeId));
        }

        private int GetPortalIdFromFilters(FilterCollection filters)
        {
            int portalId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, FilterKeys.PortalId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            return portalId;
        }

        //To update filter with specified details.
        private void RemoveFilters(FilterCollection filters, string filterName)
        {
            if (Equals(filters?.Exists(x => (x.FilterName.Equals(filterName, StringComparison.InvariantCultureIgnoreCase))), true))
            {
                filters?.RemoveAll(x => (x.FilterName.Equals(filterName, StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        // Remove key from dictionary.
        private void RemoveKeyFromDictionary(QuoteSessionModel quoteSessionModel, string key, bool isFromLineItem = false)
        {
            if (!isFromLineItem)
            {
                if (IsNotNull(quoteSessionModel.QuoteHistory))
                    if (quoteSessionModel.QuoteHistory.ContainsKey(key))
                    {
                        quoteSessionModel.QuoteHistory.Remove(key);
                    }
            }
            else
            {
                if (IsNotNull(quoteSessionModel.QuoteLineItemHistory))
                    if (quoteSessionModel.QuoteLineItemHistory.ContainsKey(key))
                    {
                        quoteSessionModel.QuoteLineItemHistory.Remove(key);
                    }
            }
        }
        // Add key and value in dictionary.
        private void UpdateQuoteHistory(QuoteSessionModel quoteSessionModel, string settingType, string oldValue = "")
        {
            quoteSessionModel.QuoteHistory?.Add(settingType, oldValue);
        }

        //Set User Details in ShoppingCartModel.
        private void SetUserInformation(ShoppingCartModel model)
        {
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = (int)model.UserId });
            model.UserDetails = _userClient.GetUserAccountData((int)model.UserId, new ExpandCollection { ExpandKeys.Profiles });
            model.UserDetails.UserId = (int)model.UserId;
            model.UserDetails.ProfileId = (model.UserDetails.Profiles?.Where(x => x.IsDefault.GetValueOrDefault())?.FirstOrDefault()?.ProfileId).GetValueOrDefault();
            model.ProfileId = model.UserDetails.ProfileId;
        }

        //Get the Expand collection with expand keys.
        private ExpandCollection GetExpands()
        => new ExpandCollection { ZnodeUserAddressEnum.ZnodeAddress.ToString() };

        //Bind all details of shopping  cart model.
        private void SetShoppingCartDetailsForQuotes(QuoteCreateViewModel quoteCreateViewModel, ShoppingCartModel cartModel)
        {
            if (IsNotNull(cartModel))
            {
                quoteCreateViewModel.UserId = cartModel.UserId.GetValueOrDefault();
                quoteCreateViewModel.PortalId = cartModel.PortalId;
                quoteCreateViewModel.ShippingCost = cartModel.ShippingCost;
                quoteCreateViewModel.TaxCost = cartModel.TaxCost;
                quoteCreateViewModel.ImportDuty = cartModel.ImportDuty.GetValueOrDefault();
                quoteCreateViewModel.TaxSummaryList = cartModel.TaxSummaryList;
                quoteCreateViewModel.QuoteTotal = cartModel.Total;
                quoteCreateViewModel.ShippingHandlingCharges = cartModel.ShippingHandlingCharges;

                quoteCreateViewModel.ShippingId = quoteCreateViewModel.ShippingId == 0 ? cartModel.ShippingId : quoteCreateViewModel.ShippingId;
                quoteCreateViewModel.BillingAddressId = cartModel.BillingAddress.AddressId;
                quoteCreateViewModel.ShippingAddressId = cartModel.ShippingAddress.AddressId;
                quoteCreateViewModel.productDetails = cartModel.ShoppingCartItems.ToViewModel<ProductDetailModel>().ToList();
                quoteCreateViewModel.IsTaxExempt = IsNotNull(cartModel.CustomTaxCost);
            }
        }

        #endregion

        #region Protected Methods
        //Fill the quote details and return the view model
        protected virtual QuoteResponseViewModel FillQuoteDetails(int omsQuoteId, QuoteResponseModel quote)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = omsQuoteId });

            QuoteResponseViewModel quoteViewModel = quote.ToViewModel<QuoteResponseViewModel>();
            if (IsNotNull(quote) && IsNotNull(quoteViewModel))
            {
                quoteViewModel.QuoteInformation = GetQuoteInformation(quote);
                quoteViewModel.CustomerInformation = GetCustomerInformation(quote);
                quoteViewModel.CartInformation = GetQuoteLineItems(quote);
                SetPortalInformation(quoteViewModel);

                QuoteSessionModel quoteSessionModel = quoteViewModel.ToModel<QuoteSessionModel>();
                if (IsNotNull(quoteSessionModel))
                {
                    quoteSessionModel.QuoteHistory = new Dictionary<string, string>();
                    quoteSessionModel.QuoteLineItemHistory = new Dictionary<string, QuoteLineItemHistoryModel>();
                    SaveInSession<QuoteSessionModel>(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteSessionModel);
                }
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return quoteViewModel;
        }

        //Get Information of Customer (Customer Name Billing/Shipping Address etc.)
        protected virtual CustomerInfoViewModel GetCustomerInformation(QuoteResponseModel quote)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(quote))
            {
                return new CustomerInfoViewModel
                {
                    CustomerName = quote?.FirstName + " " + quote?.LastName,
                    UserName = quote.UserName,
                    CustomerId = quote.UserId,
                    PhoneNumber = quote.PhoneNumber,
                    ShippingAddress = quote.ShippingAddressModel?.ToViewModel<AddressViewModel>(),
                    BillingAddress = quote.BillingAddressModel?.ToViewModel<AddressViewModel>(),
                    OmsQuoteId = quote.OmsQuoteId,
                    Email= quote.Email,
                    CustomerGUID = quote.CustomerGUID
                };
            }
            return new CustomerInfoViewModel();
        }

        //Get Information of quote ( quote status, shipping)
        protected virtual QuoteInfoViewModel GetQuoteInformation(QuoteResponseModel quote)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(quote))
            {
                return new QuoteInfoViewModel
                {
                    OmsQuoteId = quote.OmsQuoteId,
                    CreatedByName = quote.CreatedByName,
                    StoreName = quote.StoreName,
                    QuoteNumber = quote.QuoteNumber,
                    OmsQuoteStatus = quote.QuoteStatus,
                    userId = quote.UserId,
                    PortalId = quote.PortalId,
                    QuoteDateWithTime = Convert.ToDateTime(quote.CreatedDate).ToString(HelperMethods.GetStringDateTimeFormat()),
                    InHandDate = quote.InHandDate,
                    QuoteExpirationDate = quote.QuoteExpirationDate,
                    ShippingTypeDescription = quote.ShippingDiscountDescription,
                    ShippingTypeClassName = quote.ShippingTypeClassName,
                    AccountNumber = quote?.AccountNumber,
                    ShippingMethod = quote?.ShippingMethod,
                    ShippingId = quote.ShippingId,
                    JobName = quote.JobName,
                    UserName = quote.UserName,
                    ShippingConstraintCode = quote.ShippingConstraintCode,
                    OrderNumber = quote.OrderNumber,
                    OmsOrderId = quote.OmsOrderId,
                    ShippingConstraints = GetService<IAdminHelper>().GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete).
                    Select(s => new ShippingConstraintsViewModel
                    {
                        IsSelected = string.IsNullOrWhiteSpace(quote.ShippingConstraintCode) ? false : quote.ShippingConstraintCode.Equals(s.ShippingConstraintCode),
                        ShippingConstraintCode = s.ShippingConstraintCode,
                        Description = s.Description
                    }).ToList()
                };
            }
            return new QuoteInfoViewModel();
        }

        //Get Information of All quote line items (Unit price, quantity, shipping status, etc.)
        protected virtual QuoteCartViewModel GetQuoteLineItems(QuoteResponseModel quote)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(quote?.ShoppingCartItems))
            {
                QuoteCartViewModel quoteCartViewModel = quote.ToViewModel<QuoteCartViewModel>();

                return quoteCartViewModel;
            }
            return new QuoteCartViewModel();
        }

        //Get Quote status List on the basis of IsQuoteState filter value
        protected virtual List<SelectListItem> GetQuoteStatusList(FilterTuple filter = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            if (filter != null)
            {
                filters.Add(filter);
                ZnodeLogging.LogMessage("Filters to get quote states: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
                return StoreViewModelMap.ToOrderStateList(_orderStateClient.GetOrderStates(null, filters, null, null, null)?.OrderStates, true);
            }
            return new List<SelectListItem>();
        }

        // Get shopping cart model.
        protected virtual ShoppingCartModel GetShoppingCartModel(QuoteSessionModel quoteModel)
        {
            if (IsNotNull(quoteModel))
            {
                ShoppingCartModel shoppingCart = quoteModel.ToModel<ShoppingCartModel>();

                if (IsNotNull(shoppingCart))
                {
                    if (quoteModel.CartInformation.IsTaxExempt)
                    {
                        shoppingCart.CustomTaxCost = Convert.ToDecimal(0);
                        shoppingCart.IsTaxCostEdited = true;
                    }
                    //Disable calculation of promotion and coupon.
                    shoppingCart.IsCalculatePromotionAndCoupon = false;
                    shoppingCart.IsQuoteOrder = true;
                    shoppingCart.CustomShippingCost = quoteModel.CartInformation.CustomShippingCost;
                    shoppingCart.ShoppingCartItems = quoteModel.CartInformation.ShoppingCartItems.ToList();
                    shoppingCart.ShippingCost = quoteModel.CartInformation.ShippingCost;
                    shoppingCart.ShippingHandlingCharges = quoteModel.CartInformation.ShippingHandlingCharges;
                    SetUserInformation(shoppingCart);
                }
                return shoppingCart;
            }
            return null;
        }

        //Get Shipping List
        protected virtual ShippingListViewModel GetShippingList(ShoppingCartModel shoppingCart)
        {
            if (IsNull(shoppingCart)) return new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };

            string zipCode = shoppingCart?.ShippingAddress?.PostalCode;

            shoppingCart.PublishStateId = DefaultSettingHelper.GetCurrentOrDefaultAppType(ZnodePublishStatesEnum.PRODUCTION);

            ShippingListViewModel listViewModel = new ShippingListViewModel { ShippingList = _shoppingCartClient.GetShippingEstimates(zipCode, shoppingCart)?.ShippingList?.ToViewModel<ShippingViewModel>().ToList() };

            ZnodeLogging.LogMessage("ShippingList count : ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingListCount = listViewModel?.ShippingList?.Count });
            return listViewModel?.ShippingList?.Count > 0 ? listViewModel : new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };
        }

        // Get calculated shopping cart model.
        protected virtual ShoppingCartModel GetCalculatedShoppingCartForEditQuote(ShoppingCartModel shoppingCartModel)
        {
            if (IsNotNull(shoppingCartModel))
            {
                ShoppingCartModel calculatedShoppingCartModel = _shoppingCartClient.Calculate(shoppingCartModel);

                if (IsNotNull(calculatedShoppingCartModel))
                {
                    calculatedShoppingCartModel.ShippingCost = (calculatedShoppingCartModel.ShippingCost + calculatedShoppingCartModel.ShippingDifference);
                }
                return calculatedShoppingCartModel;
            }
            return null;
        }

        //Map Shipping Details to cart and quoteModel.
        protected virtual ShoppingCartModel MapShippingDetailsAndCalculateCart(int shippingId, ShoppingCartModel cart, QuoteSessionModel quoteModel)
        {
            if(shippingId > 0)
            {
                ShippingViewModel selectedShippingOption = _shippingClient.GetShipping(shippingId)?.ToViewModel<ShippingViewModel>();

                if (IsNotNull(selectedShippingOption) && IsNotNull(cart))
                {
                    //Assign shipping details to cart.
                    cart.Shipping = new OrderShippingModel
                    {
                        ShippingId = IsNull(selectedShippingOption) ? 0 : selectedShippingOption.ShippingId,
                        ShippingDiscountDescription = selectedShippingOption?.Description,
                        ShippingCountryCode = string.IsNullOrEmpty(cart?.ShippingAddress?.CountryName) ? string.Empty : cart?.ShippingAddress?.CountryName,
                        ShippingName = selectedShippingOption?.Description
                    };

                    //Calculate ShoppingCartModel
                    cart = GetCalculatedShoppingCartForEditQuote(cart);

                    //Assign shipping details to quoteModel.
                    if (IsNotNull(quoteModel) && IsNotNull(cart))
                    {
                        if (string.IsNullOrEmpty(cart.Shipping.ResponseMessage))
                        {
                            quoteModel.ShippingId = selectedShippingOption.ShippingId;
                            quoteModel.ShippingCost = cart.ShippingCost;
                            quoteModel.ShippingDiscountDescription = selectedShippingOption.Description;
                            quoteModel.ShippingType = selectedShippingOption.Description;
                        }
                    }
                }
                return cart;
            }
            return null;
        }

        //Set ShoppingCartModel And Calculate for manage.
        protected virtual void SetCartModelAndCalculate(AddressViewModel addressViewModel, QuoteSessionModel quoteModel, int omsQuoteId, string fromBillingShipping = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);          
     
            if (IsNotNull(quoteModel) && IsNotNull(addressViewModel))
            {
                ShoppingCartModel cart = GetShoppingCartModel(quoteModel);
                if (IsNotNull(cart))
                {
                    //Map updated Address in ShoppingCartModel
                    MapUpdatedAddressInCart(cart, addressViewModel, quoteModel, fromBillingShipping);

                    ResetCustomShippingCost(cart);

                    if (cart?.ShoppingCartItems?.Count > 0)
                        cart = GetCalculatedShoppingCartForEditQuote(cart);

                    quoteModel.CartInformation = cart.ToViewModel<QuoteCartViewModel>();

                    //save Shopping cart in Session
                    SaveInSession(AdminConstants.OMSQuoteSessionKey + omsQuoteId, quoteModel);

                    if (!string.IsNullOrEmpty(cart.Shipping.ResponseMessage))
                    {
                        quoteModel.CartInformation.HasError = true;
                        quoteModel.CartInformation.ShippingErrorMessage = cart.Shipping.ResponseMessage;
                    }
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                }
            }
        }

        //Map updated Address in ShoppingCartModel
        protected virtual void MapUpdatedAddressInCart(ShoppingCartModel cart, AddressViewModel addressViewModel, QuoteSessionModel quoteModel, string fromBillingShipping = "")
        {
            if (!string.IsNullOrEmpty(fromBillingShipping) && IsNotNull(quoteModel) && IsNotNull(addressViewModel))
            {
                if (Equals(fromBillingShipping.ToLower(), Admin_Resources.LabelBilling.ToLower()))
                {
                    RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderBillingAddress);
                    UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderBillingAddress, Admin_Resources.UpdateBillingAddress);

                    cart.BillingAddress = quoteModel.BillingAddressModel;
                    CheckDefaultBillingShippingAddress(addressViewModel, quoteModel, cart, quoteModel.BillingAddressModel);
                }
                else
                {
                    RemoveKeyFromDictionary(quoteModel, ZnodeConstant.OrderShippingAddress);
                    UpdateQuoteHistory(quoteModel, ZnodeConstant.OrderShippingAddress, Admin_Resources.UpdateShippingAddress);

                    cart.Shipping.ShippingCountryCode = quoteModel.ShippingAddressModel.CountryName;
                    cart.ShippingAddress = quoteModel.ShippingAddressModel;
                    CheckDefaultBillingShippingAddress(addressViewModel, quoteModel, cart, quoteModel.ShippingAddressModel);
                }
                ZnodeLogging.LogMessage("ShippingCountryCode and ShippingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingCountryCode = cart?.Shipping?.ShippingCountryCode, shippingAddressId = cart.ShippingAddress?.AddressId });
            }
        }

        //Validate User Address on the basis of flag EnableAddressValidation
        protected virtual BooleanModel IsValidateAddress(QuoteSessionModel quoteModel, AddressViewModel addressViewModel)
        {
            BooleanModel booleanModel = null;
            IPortalClient _portalClient = GetClient<PortalClient>();
            PortalModel portal = _portalClient.GetPortal(quoteModel.PortalId, new ExpandCollection { ZnodePortalEnum.ZnodePortalLocales.ToString(), ZnodePortalEnum.ZnodePortalUnits.ToString(), ZnodePortalEnum.ZnodePortalCatalogs.ToString() });
            if(IsNotNull(portal))
            {
                quoteModel.EnableAddressValidation = portal.SelectedPortalFeatures.Any(p => p.PortalFeatureName == StoreFeature.Address_Validation.ToString());

                booleanModel = IsAddressValid(quoteModel.EnableAddressValidation, addressViewModel.ToModel<AddressModel>());
            }
            return booleanModel;
        }

        //Check whether shipping address is valid or not.
        protected virtual BooleanModel IsAddressValid(bool enableAddressValidation, AddressModel address)
        {
            ZnodeLogging.LogMessage("AddressModel with Id: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = address?.AddressId });
            if (enableAddressValidation)
            {
                address.PublishStateId = (byte)ZnodePublishStatesEnum.PRODUCTION;
                return _shippingClient.IsShippingAddressValid(address);
            }
            return new BooleanModel { IsSuccess = true };
        }

        //Update Customer Address as per accountId and userId
        protected virtual AddressViewModel UpdatedAddress(AddressViewModel addressViewModel)
        {
            AddressViewModel updatedAddress = null;
            if (IsNotNull(addressViewModel))
            {
                if (addressViewModel.AccountId > 0)
                {
                    if (addressViewModel.AddressId.Equals(0))
                        updatedAddress = _accountClient.CreateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                    else
                        updatedAddress = _accountClient.UpdateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                }
                else
                {
                    if (addressViewModel.AddressId.Equals(0))
                        updatedAddress = _customerClient.CreateCustomerAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                    else
                        updatedAddress = _customerClient.UpdateCustomerAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>(); ;
                }
            }
            return updatedAddress;
        }

        //Set selected Billing shipping Id.
        protected virtual AddressListModel SetBillingshippingId(AddressViewModel addressViewModel)
        {
            if (IsNotNull(addressViewModel))
            {
                //Get B2B account or user address list.
                AddressListModel addressList = GetAddressListOfUserAndAccount(addressViewModel.UserId, addressViewModel.AccountId);
                if (HelperUtility.IsNotNull(addressList))
                {
                    if (addressViewModel.SelectedShippingId == 0 && addressList.AddressList.FirstOrDefault().IsShipping)
                        addressViewModel.SelectedShippingId = addressList.AddressList.First(x => x.IsShipping).AddressId;

                    if (addressViewModel.SelectedBillingId == 0 && addressList.AddressList.FirstOrDefault().IsBilling)
                        addressViewModel.SelectedBillingId = addressList.AddressList.First(x => x.IsBilling).AddressId;
                }
                return addressList;
            }
            return null;
        }

        //To check default billing and shipping address
        protected virtual void CheckDefaultBillingShippingAddress(AddressViewModel addressViewModel, QuoteSessionModel quoteModel, ShoppingCartModel cart, AddressModel updatedAddress)
        {
            if (Equals(addressViewModel.SelectedBillingId, addressViewModel.SelectedShippingId))
            {
                cart.ShippingAddress = updatedAddress;
                quoteModel.ShippingAddressModel = updatedAddress;
                cart.BillingAddress = updatedAddress;
                quoteModel.BillingAddressModel = updatedAddress;
            }
        }

        //Set shipping and billing Address To QuoteSessionModel.
        protected virtual void SetAddressToQuoteModel(AddressViewModel addressViewModel, QuoteSessionModel quoteModel, AddressViewModel updatedAddress, AddressListModel addressList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Get shipping address id as per IsShippingAddressChange flag.
            int shippingAddressId = addressViewModel.IsShippingAddressChange ? updatedAddress.AddressId : addressViewModel.SelectedShippingId;
            int billingAddressId = 0;

            //Set Shipping Address
            if (IsNotNull(addressList?.AddressList?.FirstOrDefault(w => w.AddressId == shippingAddressId)))
            {
                quoteModel.ShippingAddressModel = addressList?.AddressList?.FirstOrDefault(w => w.AddressId == shippingAddressId);
            }
            else if (addressViewModel.IsShippingAddressChange)
            {
                quoteModel.ShippingAddressModel = updatedAddress.ToModel<AddressModel>();
            }
            else
                quoteModel.ShippingAddressModel = addressViewModel.ToModel<AddressModel>();

            //Get billing address id as per IsShippingAddressChange flag.
            if (!addressViewModel.IsShippingAddressChange)
            {
                billingAddressId = updatedAddress.AddressId;
            }
            else if (addressViewModel.SelectedBillingId.Equals(0))
            {
                billingAddressId = shippingAddressId;
            }
            else
                billingAddressId = addressViewModel.SelectedBillingId;

            //Set BillingAddress
            if (IsNotNull(addressList?.AddressList?.FirstOrDefault(w => w.AddressId == billingAddressId)))
            {
                quoteModel.BillingAddressModel = addressList?.AddressList?.FirstOrDefault(w => w.AddressId == billingAddressId);
            }
            else if (!addressViewModel.IsShippingAddressChange)
            {
                quoteModel.BillingAddressModel = updatedAddress.ToModel<AddressModel>();
            }
            else if (addressViewModel.SelectedBillingId.Equals(0))
            {
                quoteModel.BillingAddressModel = quoteModel.ShippingAddressModel;
            }
            else
                quoteModel.BillingAddressModel = addressViewModel.ToModel<AddressModel>();

            ZnodeLogging.LogMessage("ShippingAddressId and BillingAddressId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ShippingAddressId = quoteModel?.ShippingAddressModel?.AddressId, BillingAddressId = quoteModel?.BillingAddressModel?.AddressId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Return account id if customer is B2B if not returns 0.
        protected virtual int GetCustomerAccountId(int userId)
        {
            int? AccountId = _userClient.GetUserAccountData(userId)?.AccountId;
            ZnodeLogging.LogMessage("UserId and customer account Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId, CustomerAccountId = AccountId });
            return AccountId > 0 ? AccountId.GetValueOrDefault() : 0;
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

        //Get Address List by address Id
        protected virtual AddressListModel GetAddressListByAddressId(int addressId)
        {
            IAddressClient _addressClient = GetClient<AddressClient>();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressId.ToString()));
            AddressListModel addresslistmodel = _addressClient.GetAddressList(filters, null, null, null);
            addresslistmodel.AddressList.FirstOrDefault().DontAddUpdateAddress = true;

            return addresslistmodel;
        }

        //Validate Save Quote
        protected virtual bool ValidateQuoteOnManage(QuoteSessionModel quoteModel, string additionalInstructions)
        {
            if (quoteModel.QuoteLineItemHistory.Count > 0 || quoteModel.QuoteHistory.Count > 0 || !string.IsNullOrEmpty(additionalInstructions))
                return true;
            return false;
        }

        // Remove cart line product history.
        protected virtual void RemoveHistoryForCartLineItem(ShoppingCartItemModel shoppingCartItem, QuoteSessionModel quoteModel)
        {
            if (IsNotNull(shoppingCartItem) && IsNotNull(quoteModel))
            {
                QuoteLineItemHistoryModel quoteLineItemHistoryModel = new QuoteLineItemHistoryModel()
                {
                    IsDeleteLineItem = true,
                    ProductName = shoppingCartItem.ProductName,
                    Quantity = shoppingCartItem.Quantity.ToInventoryRoundOff(),
                    SKU = shoppingCartItem.SKU,
                    OmsQuoteLineItemsId = shoppingCartItem.OmsQuoteLineItemId
                };
                RemoveKeyFromDictionary(quoteModel, shoppingCartItem.ExternalId, true);
                UpdateQuoteItemHistory(quoteModel, shoppingCartItem.ExternalId, quoteLineItemHistoryModel);
            }
        }

        // Add key and value in quote line dictionary.
        protected virtual void UpdateQuoteItemHistory(QuoteSessionModel quoteModel, string key, QuoteLineItemHistoryModel lineHistory)
        {
            if (IsNotNull(quoteModel))
                quoteModel.QuoteLineItemHistory?.Add(key, lineHistory);
        }

        //Create History Of Quote
        protected virtual void GetQuoteHistory(QuoteSessionModel model, UpdateQuoteModel quoteModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            quoteModel.QuoteHistory = string.Empty;

            if (IsNotNull(model?.QuoteHistory))
            {
                foreach (var history in model?.QuoteHistory)
                {
                    if (!string.IsNullOrEmpty(history.Key) &&
                        history.Key == ZnodeConstant.QuoteUpdatedStatus ||
                        history.Key == ZnodeConstant.OrderBillingAddress ||
                        history.Key == ZnodeConstant.OrderShippingAddress ||
                        history.Key == ZnodeConstant.OrderShippingCost ||
                        history.Key == ZnodeConstant.OrderTax ||
                        history.Key == ZnodeConstant.QuoteUnexemptedTax ||
                        history.Key == ZnodeConstant.OrderShippingType ||
                        history.Key == ZnodeConstant.OrderInHandsDate ||
                        history.Key == ZnodeConstant.QuoteExpirationDate||
                        history.Key == ZnodeConstant.OrderJobName ||
                        history.Key == ZnodeConstant.OrderShippingConstraintCode)
                    {
                        quoteModel.QuoteHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(history.Key, model), quoteModel.QuoteHistory);
                    }
                }
            }
            if (HelperUtility.IsNotNull(model?.QuoteLineItemHistory) && model?.QuoteLineItemHistory.Count > 0)
            {
                foreach (var item in model?.QuoteLineItemHistory)
                {
                    quoteModel.QuoteHistory = GenerateQuoteLineItemHistory(quoteModel, item.Key, item.Value, model.CultureCode);
                }
            }
        }

        //to Generate quote line item changes
        protected virtual string GenerateQuoteLineItemHistory(UpdateQuoteModel model, string lineItemSku, QuoteLineItemHistoryModel lineItemHistory, string cultureCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(lineItemHistory))
            {
                List<QuoteLineItemModel> oldLineItemValue = _quoteClient.GetQuoteLineItemByQuoteId(model.OmsQuoteId);
                if (oldLineItemValue?.Where(w => w.SKU == lineItemSku)?.Count() == 0 || oldLineItemValue?.FirstOrDefault(w => w.SKU == lineItemSku)?.Quantity == 0)
                    lineItemSku = lineItemHistory.SKU;
                string oldQuantity = string.Empty;
                string oldUnitPrice = string.Empty;
                string oldShippingCost = string.Empty;
                QuoteLineItemModel parentQuoteLineItem = oldLineItemValue?.FirstOrDefault(x => x.SKU == lineItemSku);
                QuoteLineItemModel childQuoteLineItem = oldLineItemValue?.FirstOrDefault(x => x.ParentOmsQuoteLineItemId == parentQuoteLineItem?.OmsQuoteLineItemId && x.OrderLineItemRelationshipTypeId != null);

                if (childQuoteLineItem?.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles)
                {
                    oldQuantity = Convert.ToString(childQuoteLineItem?.Quantity);
                    oldUnitPrice = Convert.ToString(childQuoteLineItem?.Price.GetValueOrDefault());
                    oldShippingCost = Convert.ToString(childQuoteLineItem?.Price.GetValueOrDefault());
                }
                else
                {
                    QuoteLineItemModel quoteLineItem = oldLineItemValue.FirstOrDefault(w => w.SKU == lineItemSku && w.ParentOmsQuoteLineItemId != null);

                    oldQuantity = Convert.ToString(quoteLineItem?.Quantity);
                    oldUnitPrice = Convert.ToString(quoteLineItem?.Price.GetValueOrDefault());
                    oldShippingCost = Convert.ToString(quoteLineItem?.ShippingCost);
                }
                
                string productName = lineItemHistory.ProductName;
                string qty = Convert.ToString(lineItemHistory.Quantity);

                if (lineItemHistory.IsDeleteLineItem)
                    model.QuoteHistory = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.QuoteLineItemDeleted, lineItemHistory.SKU, lineItemHistory.ProductName, qty), model.QuoteHistory);

                if (lineItemHistory.IsUpdateQuantity && !Equals(lineItemHistory.Quantity, "0"))
                    model.QuoteHistory = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.QuoteLineItemQuantityChanged, lineItemSku, productName, oldQuantity, lineItemHistory.Quantity), model.QuoteHistory);

                if (lineItemHistory.IsUpdateUnitPrice)
                    model.QuoteHistory = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.QuoteLineItemPriceChanged, lineItemSku, productName, oldUnitPrice, lineItemHistory.UnitPrice), model.QuoteHistory);

                if (lineItemHistory.IsUpdateLineItemShippingPrice)
                    model.QuoteHistory = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.QuoteLineItemShippingPriceChanged, lineItemSku, productName, HelperMethods.FormatPriceWithCurrency(Convert.ToDecimal(oldShippingCost), cultureCode), lineItemHistory.ShippingCost), model.QuoteHistory);

            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return model.QuoteHistory;
        }

        //to get consolidated history message
        protected virtual string GetConsolidatedHistoryMessage(string message, string mergedMessage)
        {
            if (!string.IsNullOrEmpty(mergedMessage))
                mergedMessage += $"<br/>{ message}";
            else
                mergedMessage = message;

            return mergedMessage;
        }

        //to check get history for provided key
        protected virtual string GetHistoryMessageByKey(string key, QuoteSessionModel model)
        {
            string val = string.Empty;
            model.QuoteHistory.TryGetValue(key, out val);

            switch (key)
            {
                case ZnodeConstant.QuoteUpdatedStatus:
                    if (val != model.QuoteStatus)
                    {
                        if (val == ZnodeConstant.Approved)
                            return string.Format(Admin_Resources.QuoteHistoryConvertToOrderStatus, model.QuoteStatus, val);
                        else
                            return string.Format(Admin_Resources.QuoteHistoryUpdatedStatus, model.QuoteStatus, val);
                    }
                    return string.Empty;

                case ZnodeConstant.OrderBillingAddress:
                    return $" {val}";

                case ZnodeConstant.OrderShippingAddress:
                    return $" {val}";

                case ZnodeConstant.OrderShippingType:
                    return string.Format(Admin_Resources.OrderHistoryShippingType, val, model.ShippingType);

                case ZnodeConstant.OrderShippingCost:
                    return string.Format(Admin_Resources.OrderHistoryShippingAmount, val, model.CartInformation.ShippingCost);

                case ZnodeConstant.OrderTax:
                    return string.Format(Admin_Resources.QuoteTaxExempted, val);

                case ZnodeConstant.QuoteUnexemptedTax:
                    return string.Format(Admin_Resources.QuoteTaxUnExempted, val);

                default:
                    return string.Format(Admin_Resources.OrderHistory, key, val);
            }
        }

        //Get In hand Quantity of product
        protected virtual decimal GetQuantityOnHandBySku(ShoppingCartItemModel cartItem, int portalId, int productId)
        {
            decimal quantityOnHand = 0;
            if (IsNotNull(cartItem) && portalId > 0)
            {
                ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = portalId, ProductId = productId });
                //Get the sku of product of which quantity needs to update.               
                string sku = GetProductSku(cartItem, productId);

                IPublishProductClient _publishProductClient = GetClient<PublishProductClient>();
                               
                ProductInventoryPriceListModel productInventory = _publishProductClient.GetProductPriceAndInventory(new ParameterInventoryPriceModel { Parameter = sku, PortalId = portalId });
                quantityOnHand = (productInventory?.ProductList?.FirstOrDefault(w => w.SKU == sku)?.Quantity).GetValueOrDefault();
                
                ZnodeLogging.LogMessage("QuantityOnHand: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { QuantityOnHand = quantityOnHand });
            }
            return quantityOnHand;
        }

        //Update custom data for cart items.
        protected virtual void GetUpdatedQuoteLineItem(ShoppingCartModel cartModel, ShoppingCartItemModel shoppingCartItem, ManageOrderDataModel quoteDataModel, QuoteSessionModel quoteModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string sku = string.Empty;
            QuoteLineItemHistoryModel quoteLineItemHistoryModel = new QuoteLineItemHistoryModel();

            if (IsNotNull(shoppingCartItem) && IsNotNull(quoteDataModel))
            {
                sku = quoteDataModel.ProductId > 0
                    ? shoppingCartItem.ProductName + "-" + Convert.ToString(shoppingCartItem.GroupProducts?.FirstOrDefault(y => y.ProductId == quoteDataModel.ProductId).Sku)
                    : shoppingCartItem.SKU;

                //Set sku of the actual product 
                quoteLineItemHistoryModel.SKU = GetProductSku(shoppingCartItem, quoteDataModel.ProductId);
                ZnodeLogging.LogMessage("SKU of the actual product: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SKU = quoteLineItemHistoryModel.SKU });

                if (IsNotNull(cartModel) && IsNotNull(quoteModel))
                {
                    UpdateQuoteItemHistoryModel(quoteDataModel, shoppingCartItem, quoteLineItemHistoryModel, quoteModel.CultureCode);                   
                    RemoveKeyFromDictionary(quoteModel, sku, true);
                    UpdateQuoteItemHistory(quoteModel, sku, quoteLineItemHistoryModel);
                    GetCartWithUpdatedItem(cartModel, shoppingCartItem, quoteDataModel);

                }
                ZnodeLogging.LogMessage("ShoppingCartItem model with Quantity, UnitPrice, ExternalId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Quantity = shoppingCartItem.Quantity, UnitPrice = shoppingCartItem.UnitPrice, ExternalId = shoppingCartItem.ExternalId });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get Cart with updated cart item
        protected virtual void GetCartWithUpdatedItem(ShoppingCartModel cartModel, ShoppingCartItemModel shoppingCartItem, ManageOrderDataModel quoteDataModel)
        {
            if (IsNotNull(cartModel) && IsNotNull(shoppingCartItem) && IsNotNull(quoteDataModel))
            {
                shoppingCartItem.Quantity = quoteDataModel.Quantity;
                shoppingCartItem.UnitPrice = quoteDataModel.UnitPrice.GetValueOrDefault();
                shoppingCartItem.ExtendedPrice = IsNotNull(quoteDataModel.UnitPrice) ? quoteDataModel.UnitPrice.GetValueOrDefault() * quoteDataModel.Quantity : shoppingCartItem.ExtendedPrice;
                shoppingCartItem.IsPriceEdit = (shoppingCartItem.UnitPrice != shoppingCartItem.InitialPrice) ? true : false;
                shoppingCartItem.ShippingCost = quoteDataModel.ShippingCost;

                if (quoteDataModel.IsShippingEdit)
                    shoppingCartItem.CustomShippingCost = quoteDataModel.ShippingCost;
                else
                    ResetCustomShippingCost(cartModel);

                cartModel.ShoppingCartItems.Insert(cartModel.ShoppingCartItems.FindIndex(x => x.ExternalId == shoppingCartItem.ExternalId), shoppingCartItem);
                cartModel.ShoppingCartItems.Remove(cartModel.ShoppingCartItems.LastOrDefault(x => x.ExternalId == shoppingCartItem.ExternalId));
            }
        }
        //Validate inventory of lineItem 
        protected virtual QuoteCartViewModel ValidateCartItemInventory(QuoteSessionModel quoteModel, ShoppingCartItemModel cartItem, ManageOrderDataModel quoteDataModel, ShoppingCartModel cart)
        {
            if (IsNotNull(cartItem) && IsNotNull(quoteDataModel) && IsNotNull(quoteModel))
            {
                ZnodeLogging.LogMessage("QuotelineItemQuantity: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { QuotelineItemQuantity = cartItem.Quantity });
                decimal quotelineItemQuantity = GetQuoteLineItemQuantity(quoteModel, cartItem);
                if (!cartItem.IsActive)
                {
                    QuoteCartViewModel cartViewModel = quoteModel.CartInformation.ToModel<QuoteCartViewModel>();
                    cartViewModel.HasError = true;
                    cartViewModel.ErrorMessage = Admin_Resources.ErrorProductDisabled;
                    return cartViewModel;
                }
                if (IsNotNull(cart))
                {
                    decimal orderedQuantity = quoteDataModel.Quantity;
                    if (cartItem.BundleProducts == null || cartItem.BundleProducts.Count == 0)
                    {
                        //Get inventory of sku and Check Inventory
                        decimal quantityOnHand = GetQuantityOnHandBySku(cartItem, cart.PortalId, quoteDataModel.ProductId);
                        if (quantityOnHand < orderedQuantity && cartItem.TrackInventory && !cartItem.AllowBackOrder)
                        {
                            QuoteCartViewModel cartViewModel = quoteModel.CartInformation.ToModel<QuoteCartViewModel>();
                            cartViewModel.HasError = true;
                            cartViewModel.ErrorMessage = Admin_Resources.TextOutofStock;
                            return cartViewModel;
                        }
                    }
                    else
                    {                        
                        if (cartItem?.BundleProducts?.Count > 0)
                        {
                            IPublishProductClient _publishProductClient = GetClient<PublishProductClient>();
                            string skus = string.Empty;
                            if (string.IsNullOrEmpty(cartItem.BundleProductSKUs))
                            {
                                skus = string.Join(",",cartItem.BundleProducts.Select(d => d.SKU));
                            }
                            ProductInventoryPriceListModel productInventory = _publishProductClient.GetProductPriceAndInventory(new ParameterInventoryPriceModel { Parameter = skus, PortalId = cart.PortalId, IsBundleProduct = true,BundleProductParentSKU = cartItem.SKU });
                            foreach (var BundleProduct in cartItem.BundleProducts)
                            {
                                bool TrackInventory = BundleProduct.TrackInventory != null ? BundleProduct.TrackInventory.Value : false;
                                bool AllowBackOrder = BundleProduct.AllowBackOrder != null ? BundleProduct.AllowBackOrder.Value : false;
                                var quantityOnHand = productInventory?.ProductList?.FirstOrDefault(d => d.SKU == BundleProduct.SKU).Quantity.Value;
                                if(TrackInventory && !AllowBackOrder)
                                {
                                    if (quantityOnHand < orderedQuantity)
                                    {
                                        QuoteCartViewModel cartViewModel = quoteModel.CartInformation.ToModel<QuoteCartViewModel>();
                                        cartViewModel.HasError = true;
                                        cartViewModel.ErrorMessage = Admin_Resources.TextOutofStock;
                                        return cartViewModel;
                                    }
                                }
                                
                            }
                        }
                        
                    }
                }
            }
            return null;
        }

        //Calculate Cart for line Item
        protected virtual ShoppingCartModel GetCalculatedCartForLineItem(ShoppingCartModel cart, ManageOrderDataModel quoteDataModel, QuoteSessionModel quoteModel, ShoppingCartItemModel cartItem)
        {
            if (IsNotNull(cart) && IsNotNull(quoteDataModel) && IsNotNull(quoteModel) && IsNotNull(cartItem))
            {
                GetUpdatedQuoteLineItem(cart, cartItem, quoteDataModel, quoteModel);
                cart = GetCalculatedShoppingCartForEditQuote(cart);
                quoteModel.CartInformation = cart.ToViewModel<QuoteCartViewModel>();

                SaveInSession(AdminConstants.OMSQuoteSessionKey + quoteDataModel.OmsQuoteId, quoteModel);
                return cart;
            }
            return null;
        }

        //Get quantity of ordered line item.
        protected virtual decimal GetQuoteLineItemQuantity(QuoteSessionModel quoteModel, ShoppingCartItemModel cartItemModel)
        {
            List<QuoteLineItemModel> quoteLineItems = _quoteClient.GetQuoteLineItemByQuoteId(quoteModel.OmsQuoteId);
            if (IsNotNull(quoteModel) && IsNotNull(cartItemModel))
            {
                if (cartItemModel.GroupProducts?.Count > 0)
                {
                    return (quoteLineItems?.FirstOrDefault(x => x.OmsQuoteLineItemId == cartItemModel.OmsQuoteLineItemId && x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)?.Quantity).GetValueOrDefault();
                }
                else if (!string.IsNullOrEmpty(cartItemModel.ConfigurableProductSKUs))
                {
                    return (quoteLineItems?.FirstOrDefault(x => x.OmsQuoteLineItemId == cartItemModel.OmsQuoteLineItemId)?.Quantity).GetValueOrDefault();
                }
                else
                    return (quoteLineItems?.FirstOrDefault(x => x.OmsQuoteLineItemId == cartItemModel.OmsQuoteLineItemId)?.Quantity).GetValueOrDefault();
            }
            return 0;
        }

        //Get the sku of product of which quantity needs to update.       
        protected virtual string GetProductSku(ShoppingCartItemModel cartItem, int productId)
        {
            string sku = string.Empty;
            if (IsNotNull(cartItem))
            {
                if (productId > 0)
                {
                    sku = cartItem.GroupProducts?.FirstOrDefault(x => x.ProductId == productId)?.Sku;
                }
                else if (!string.IsNullOrEmpty(cartItem.ConfigurableProductSKUs))
                {
                    sku = cartItem.ConfigurableProductSKUs;
                }
                else
                    sku = cartItem.SKU;
            }
            return sku;
        }

        //Map QuoteLineItemHistoryModel 
        protected virtual void UpdateQuoteItemHistoryModel(ManageOrderDataModel quoteDataModel, ShoppingCartItemModel shoppingCartItem, QuoteLineItemHistoryModel quoteLineItemHistoryModel, string cultureCode = "")
        {
            UpdateQuoteItemPrice(quoteDataModel, shoppingCartItem, quoteLineItemHistoryModel, cultureCode);
            UpdateQuoteItemQuantity(quoteDataModel, shoppingCartItem, quoteLineItemHistoryModel);
            UpdateQuoteItemShippingPrice(quoteDataModel, shoppingCartItem, quoteLineItemHistoryModel, cultureCode);

            if (IsNotNull(quoteLineItemHistoryModel))
            {
                quoteLineItemHistoryModel.ProductName = shoppingCartItem.ProductName;
                if (quoteDataModel.CustomQuantity > 0)
                {
                    quoteLineItemHistoryModel.Quantity = quoteDataModel.CustomQuantity.ToInventoryRoundOff();
                }
                else if (!Equals(quoteDataModel.Quantity, shoppingCartItem.Quantity))
                {
                    quoteLineItemHistoryModel.Quantity = quoteDataModel.Quantity.ToInventoryRoundOff();
                }
                else
                    quoteLineItemHistoryModel.Quantity = shoppingCartItem.Quantity.ToInventoryRoundOff();

                quoteLineItemHistoryModel.OmsQuoteLineItemsId = shoppingCartItem.OmsQuoteLineItemId;
                quoteLineItemHistoryModel.SubTotal = Convert.ToDecimal(quoteLineItemHistoryModel.Quantity) * shoppingCartItem.UnitPrice;
            }
        }

        //Update Quote Item Price.
        protected virtual void UpdateQuoteItemPrice(ManageOrderDataModel quoteDataModel, ShoppingCartItemModel shoppingCartItem, QuoteLineItemHistoryModel quoteLineItemHistoryModel, string cultureCode = "")
        {
            if (IsNotNull(quoteDataModel) && IsNotNull(shoppingCartItem) && IsNotNull(quoteLineItemHistoryModel))
            {
                if (IsNotNull(quoteDataModel.UnitPrice))
                {
                    if (!Equals(shoppingCartItem.UnitPrice, quoteDataModel.UnitPrice))
                    {
                        quoteLineItemHistoryModel.IsUpdateUnitPrice = true;
                        quoteLineItemHistoryModel.UnitPrice = HelperMethods.FormatPriceWithCurrency(Convert.ToDecimal(quoteDataModel.UnitPrice), cultureCode);
                    }
                    shoppingCartItem.CustomUnitPrice = quoteDataModel.UnitPrice.GetValueOrDefault();
                    shoppingCartItem.ExtendedPrice = quoteDataModel.UnitPrice.GetValueOrDefault() * quoteDataModel.Quantity;
                }
            }
        }

        //Update Quote Item Quantity.
        protected virtual void UpdateQuoteItemQuantity(ManageOrderDataModel quoteDataModel, ShoppingCartItemModel shoppingCartItem, QuoteLineItemHistoryModel quoteLineItemHistoryModel)
        {
            if (IsNotNull(quoteDataModel) && IsNotNull(shoppingCartItem) && IsNotNull(quoteLineItemHistoryModel))
            {
                if (quoteDataModel.ProductId > 0)
                {
                    if (!Equals(shoppingCartItem?.GroupProducts?.FirstOrDefault().Quantity, quoteDataModel.Quantity))
                    {
                        quoteLineItemHistoryModel.IsUpdateQuantity = true;
                        quoteLineItemHistoryModel.Quantity = quoteDataModel.Quantity.ToInventoryRoundOff();
                    }
                    else
                        shoppingCartItem?.GroupProducts?.Where(y => y.ProductId == quoteDataModel.ProductId)?.Select(z => { z.Quantity = quoteDataModel.Quantity; return z; })?.FirstOrDefault();
                }
                else
                {
                    if (!Equals(shoppingCartItem.Quantity, quoteDataModel.Quantity))
                    {
                        quoteLineItemHistoryModel.IsUpdateQuantity = true;
                        quoteLineItemHistoryModel.Quantity = quoteDataModel.Quantity.ToInventoryRoundOff();
                    }
                }
            }
        }

        //Get Portal Information 
        protected virtual void SetPortalInformation(QuoteResponseViewModel quoteViewModel)
        {
            IPortalClient _portalClient = GetClient<PortalClient>();
            if (IsNotNull(quoteViewModel))
            {
                PortalModel portalModel = _portalClient.GetPortal(quoteViewModel.PortalId, null);
                if (IsNotNull(portalModel))
                {
                    quoteViewModel.CustomerServiceEmail = portalModel.CustomerServiceEmail;
                    quoteViewModel.CustomerServicePhoneNumber = portalModel.CustomerServicePhoneNumber;
                }
            }
        }

        //Set created by and modified by user.
        protected virtual void SetCreatedByUser(int? userId)
        {
            if (userId > 0)
            {
                int loginId = _shoppingCartClient.UserId;
                int selectedUserId = userId.GetValueOrDefault();

                _shoppingCartClient.LoginAs = loginId;
                _shoppingCartClient.UserId = selectedUserId;

                _orderClient.LoginAs = loginId;
                _orderClient.UserId = selectedUserId;
            }
        }

        //Check whether shipping address is valid or not.
        protected virtual BooleanModel IsValidAddress(QuoteCreateViewModel quoteCreateViewModel)
        {
            ZnodeLogging.LogMessage("ShippingAddress with Id: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = quoteCreateViewModel?.UserAddressDataViewModel?.ShippingAddress?.AddressId });
            if (quoteCreateViewModel.EnableAddressValidation)
            {
                if (IsNotNull(quoteCreateViewModel?.UserAddressDataViewModel?.ShippingAddress))
                {
                    quoteCreateViewModel.UserAddressDataViewModel.ShippingAddress.PublishStateId = (byte)ZnodePublishStatesEnum.PRODUCTION;
                    quoteCreateViewModel.UserAddressDataViewModel.ShippingAddress.PortalId = quoteCreateViewModel.PortalId;
                }
                //Do not allow the customer to go to next page if valid shipping address required is enabled.
                return _shippingClient.IsShippingAddressValid(quoteCreateViewModel.UserAddressDataViewModel?.ShippingAddress?.ToModel<AddressModel>());
            }
            return new BooleanModel { IsSuccess = true };
        }

        //To get the shipping cost as per selected shipping options
        protected virtual void ResetCustomShippingCost(ShoppingCartModel cart)
        {
            cart.ShoppingCartItems.ForEach(x => x.CustomShippingCost = 0);
        }

        //Update Quote Item Price.
        protected virtual void UpdateQuoteItemShippingPrice(ManageOrderDataModel quoteDataModel, ShoppingCartItemModel shoppingCartItem, QuoteLineItemHistoryModel quoteLineItemHistoryModel, string cultureCode = "")
        {
            if (IsNotNull(quoteDataModel) && IsNotNull(shoppingCartItem) && IsNotNull(quoteLineItemHistoryModel))
            {
                if (quoteDataModel.IsShippingEdit)
                {
                    quoteLineItemHistoryModel.IsUpdateLineItemShippingPrice = true;
                    quoteLineItemHistoryModel.ShippingCost = HelperMethods.FormatPriceWithCurrency(Convert.ToDecimal(quoteDataModel.ShippingCost), cultureCode);
                }           
            }
        }
        #endregion

    }
}
