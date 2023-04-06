using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.WebStore.Agents
{
    public class QuoteAgent : BaseAgent, IQuoteAgent
    {
        #region Private Variables
        protected readonly IQuoteClient _quoteClient;
        protected readonly IShippingClient _shippingsClient;
        protected readonly IUserAgent _userAgent;     
        protected readonly ICartAgent _cartAgent;
        protected readonly IPaymentAgent _paymentAgent;
        #endregion

        public QuoteAgent(IQuoteClient quoteClient, IShippingClient shippingsClient)
        {
            _quoteClient = GetClient<IQuoteClient>(quoteClient);
            _shippingsClient = GetClient<IShippingClient>(shippingsClient);
            _userAgent = GetService<IUserAgent>();
            _cartAgent = GetService<ICartAgent>();
            _paymentAgent = GetService<IPaymentAgent>();
        }

        #region Public Methods

        /// <summary>
        /// Create Quote
        /// </summary>
        /// <param name="quoteCreateViewModel"></param>
        /// <returns></returns>
        public virtual QuoteCreateViewModel Create(QuoteCreateViewModel quoteCreateViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(quoteCreateViewModel))
                {
                    //Get cart from session or by cookie.
                    ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ?? _cartAgent.GetCartFromCookie();

                    if (IsNotNull(cartModel))
                    {
                        //Send shipping address in cart for validation
                        BooleanModel booleanModel = IsValidAddressForCheckout(cartModel?.ShippingAddress, quoteCreateViewModel.ShippingAddressId);
                        if ((!booleanModel.IsSuccess) && !(bool)PortalAgent.CurrentPortal.PortalFeatureValues.Where(x => x.Key.Contains(StoreFeature.Require_Validated_Address.ToString()))?.FirstOrDefault().Value)
                        {
                            return (QuoteCreateViewModel)GetViewModelWithErrorMessage(new QuoteCreateViewModel(), booleanModel.ErrorMessage ?? WebStore_Resources.AddressValidationFailed);
                        }

                        //Set details from shopping cart
                        SetShoppingCartDetailsForQuotes(quoteCreateViewModel, cartModel);

                        quoteCreateViewModel.OmsQuoteStatus = ZnodeOrderStatusEnum.SUBMITTED.ToString();
                        quoteCreateViewModel.QuoteTypeCode = ZnodeConstant.Quote;

                        int quoteExpiredInDays = GlobalAttributeHelper.GetQuoteExpireDays();
                        quoteCreateViewModel.QuoteExpirationDate = DateTime.Now.AddDays(quoteExpiredInDays);

                        if (IsNotNull(PortalAgent.CurrentPortal.PublishState))
                            quoteCreateViewModel.PublishStateId = (byte)PortalAgent.CurrentPortal.PublishState;

                        QuoteCreateViewModel createdQuoteViewModel = _quoteClient.Create(quoteCreateViewModel.ToModel<QuoteCreateModel>()).ToViewModel<QuoteCreateViewModel>();
                        if (IsNotNull(createdQuoteViewModel) && !createdQuoteViewModel.HasError)
                        {
                            _cartAgent.ClearCartCountFromSession();
                            _userAgent.ClearUserDashboardPendingOrderDetailsCountFromSession(createdQuoteViewModel.UserId);
                            SaveInSession(WebStoreConstants.CartModelSessionKey, new ShoppingCartModel());
                            return createdQuoteViewModel;
                        }
                        return (QuoteCreateViewModel)GetViewModelWithErrorMessage(new QuoteCreateViewModel(), createdQuoteViewModel.ErrorMessage);
                    }
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return (QuoteCreateViewModel)GetViewModelWithErrorMessage(new QuoteCreateViewModel(), WebStore_Resources.ErrorFailedToCreate);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (QuoteCreateViewModel)GetViewModelWithErrorMessage(new QuoteCreateViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }

        //Get Quote View by omsQuoteId.
        public virtual QuoteResponseViewModel GetQuoteReceipt(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (omsQuoteId > 0)
                {
                    //Get User details from session.
                    UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
                    
                    QuoteResponseViewModel quoteModel = _quoteClient.GetQuoteReceipt(omsQuoteId)?.ToViewModel<QuoteResponseViewModel>();

                    //Validate User.
                    if (HelperUtility.IsNotNull(userViewModel) && userViewModel.UserId > 0)
                    {
                        if (!HelperUtility.IsValidIdInQueryString(userViewModel.UserId, quoteModel.CreatedBy))
                        {
                            return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), WebStore_Resources.ErrorAccessDenied);
                        }
                    }

                    if (IsNotNull(quoteModel))
                    {
                        return quoteModel;

                    }
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), WebStore_Resources.ErrorFailedToCreate);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }

        //Get Quote List
        public virtual List<QuoteViewModel> GetQuoteList()
        {
            UserViewModel userDetails = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            int userId = Convert.ToInt32(userDetails?.UserId);

            FilterCollection _filters = new FilterCollection();

            //Set userId Filter.
            SetUserIdFilter(_filters, userId);

            //Set QuoteTypeId Filter.
            SetQuoteTypeIdFilter(_filters);

            //Add portal id in filter collection.
            AddPortalIdInFilters(_filters, PortalAgent.CurrentPortal.PortalId);

            QuoteListModel quoteListModel = GetQuotes(_filters, 1, 3, null);

            List<QuoteViewModel> quoteList = quoteListModel?.Quotes?.ToViewModel<QuoteViewModel>()?.ToList();

            return quoteList;
        }

        //Get quote list.
        public virtual QuoteListViewModel GetQuoteList(FilterCollection filters, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            QuoteListModel quoteList = GetQuotes(filters, pageIndex, recordPerPage, sortCollection);

            QuoteListViewModel quoteListViewModel = new QuoteListViewModel { Quotes = quoteList?.Quotes.ToViewModel<QuoteViewModel>().ToList() };

            SetListPagingData(quoteListViewModel, quoteList);

            return IsNotNull(quoteListViewModel?.Quotes) ? quoteListViewModel : new QuoteListViewModel();
        }

        //Get Quote View by omsQuoteId.
        public virtual QuoteResponseViewModel GetQuote(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (omsQuoteId > 0)
                {
                    QuoteResponseViewModel quoteModel = _quoteClient.GetQuoteReceipt(omsQuoteId)?.ToViewModel<QuoteResponseViewModel>();
                    quoteModel.EnableConvertToOrder = IsQuoteValidForConvertToOrder(quoteModel);
                    if (IsNotNull(quoteModel))
                    {
                        //Get User details from session.
                        UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

                        //Validate User.
                        if(HelperUtility.IsNotNull(userViewModel))
                        {
                            if (!HelperUtility.IsValidIdInQueryString(userViewModel.UserId, quoteModel.UserId))
                            {
                                return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), WebStore_Resources.ErrorAccessDenied);
                            }
                        }
                        
                        quoteModel.CustomerPaymentGUID = userViewModel?.CustomerPaymentGUID;
                        return quoteModel;
                    }
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), WebStore_Resources.ErrorFailedToCreate);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (QuoteResponseViewModel)GetViewModelWithErrorMessage(new QuoteResponseViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }

        // Convert Quote to Order.
        public virtual OrdersViewModel ConvertQuoteToOrder(ConvertQuoteToOrderViewModel convertToOrderModel)
        {
            try
            {
                if (IsNotNull(convertToOrderModel))
                {
                    OrdersViewModel ordersViewModel = _quoteClient.ConvertQuoteToOrder(convertToOrderModel.ToModel<ConvertQuoteToOrderModel>())?.ToViewModel<OrdersViewModel>();
                    if (IsNotNull(ordersViewModel))
                    {
                        RemoveCookie(WebStoreConstants.UserOrderReceiptOrderId);
                        CookieHelper.SetCookie(WebStoreConstants.UserOrderReceiptOrderId, Convert.ToString(ordersViewModel.OmsOrderId), 60);
                    }
                    return ordersViewModel;

                }
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ErrorFailedToCreate);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }

        //Set paypal token and payment details
        public virtual ConvertQuoteToOrderViewModel SetPayPalToken(string token, int paymentSettingId, string paymentCode, int quoteId)
        {
            ConvertQuoteToOrderViewModel convertToOrderViewModel = new ConvertQuoteToOrderViewModel();
            convertToOrderViewModel.OmsQuoteId = quoteId;
            convertToOrderViewModel.PaymentDetails = new SubmitPaymentDetailsViewModel();
            convertToOrderViewModel.PaymentDetails.PaymentSettingId = paymentSettingId;
            convertToOrderViewModel.PaymentDetails.IsFromPayPalExpress = true;
            convertToOrderViewModel.PaymentDetails.PayPalToken = token;
            convertToOrderViewModel.PaymentDetails.PaymentType = "PayPalExpress";
            convertToOrderViewModel.PaymentDetails.PaymentCode = paymentCode;
            return convertToOrderViewModel;
        }

        // Map data which is comes from amazon return url.
        public virtual ConvertQuoteToOrderViewModel SetAmazonPayDetails(string amazonOrderReferenceId, string paymentType, int paymentSettingId, string paymentCode, int quoteId, string captureId, string orderNumber)
        {
            ConvertQuoteToOrderViewModel convertToOrderViewModel = new ConvertQuoteToOrderViewModel();
            convertToOrderViewModel.OmsQuoteId = quoteId;
            convertToOrderViewModel.PaymentDetails = new SubmitPaymentDetailsViewModel();
            convertToOrderViewModel.PaymentDetails.AmazonOrderReferenceId = amazonOrderReferenceId;
            convertToOrderViewModel.PaymentDetails.PaymentType = paymentType;
            convertToOrderViewModel.PaymentDetails.PaymentSettingId = paymentSettingId;
            convertToOrderViewModel.PaymentDetails.PaymentCode = paymentCode;
            convertToOrderViewModel.PaymentDetails.PaymentToken = captureId;
            convertToOrderViewModel.PaymentDetails.IsFromAmazonPay = true;
            convertToOrderViewModel.PaymentDetails.OrderId = orderNumber;
            return convertToOrderViewModel;
        }

        #endregion

        #region Protected Methods

        //Check whether shipping address is valid or not.
        protected virtual BooleanModel IsValidAddressForCheckout(AddressModel shippingAddress, int shippingAddressId)
        {
            if ((bool)PortalAgent.CurrentPortal.PortalFeatureValues.Where(x => x.Key.Contains(StoreFeature.Address_Validation.ToString()))?.FirstOrDefault().Value)
            {
                if (IsNull(shippingAddress) || shippingAddress?.AddressId == 0)
                {
                    //if it is not available then only send shipping address from user address list for validation in USPS.
                    //Get the list of all addresses associated to current logged in user.
                    List<AddressModel> userAddresses = GetUserAddressList();
                    shippingAddress = userAddresses?.Where(x => x.AddressId == shippingAddressId)?.FirstOrDefault();
                }

                if (shippingAddress != null) shippingAddress.PublishStateId = (byte)PortalAgent.CurrentPortal.PublishState;

                //Do not allow the customer to go to next page if valid shipping address required is enabled.
                return _shippingsClient.IsShippingAddressValid(shippingAddress);
            }

            return new BooleanModel { IsSuccess = true };
        }

        // Set QuoteTypeId Filter.
        protected virtual void SetQuoteTypeIdFilter(FilterCollection filters)
        {
            filters?.RemoveAll(x => string.Equals(x.FilterName, ZnodeOmsQuoteEnum.OmsQuoteTypeId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            filters.Add(new FilterTuple(FilterKeys.OmsQuoteTypeId, FilterOperators.Equals, WebStoreConstants.OmsQuoteTypeId));

        }
        //Add Portal id in filter collection
        protected virtual void AddPortalIdInFilters(FilterCollection filters, int portalId)
        {
            if (portalId > 0)
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            }
        }
        // Set userId Filter.
        protected virtual void SetUserIdFilter(FilterCollection filters, int userId)
        {
            if (userId > 0)
            {
                filters?.RemoveAll(x => string.Equals(x.FilterName, ZnodeOmsQuoteEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, userId.ToString()));
            }
        }

        //Get quote list.
        protected virtual QuoteListModel GetQuotes(FilterCollection filters, int? pageIndex, int? recordPerPage, SortCollection sortCollection = null)
        {
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            UserViewModel userDetails = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            int userId = Convert.ToInt32(userDetails?.UserId);

            //Set userId Filter.
            SetUserIdFilter(filters, userId);

            //Set QuoteTypeId Filter.
            SetQuoteTypeIdFilter(filters);

            //Add portal id in filter collection.
            AddPortalIdInFilters(filters, PortalAgent.CurrentPortal.PortalId);

            QuoteListModel quoteList = _quoteClient.GetQuoteList(filters, sortCollection, pageIndex, recordPerPage);

            return quoteList;
        }

        //Check if the Quote is Valid For Convert To an Order
        protected virtual bool IsQuoteValidForConvertToOrder(QuoteResponseViewModel quoteModel)
        {
            return (IsNotNull(quoteModel.QuoteExpirationDate) && quoteModel.QuoteExpirationDate == DateTime.Now ||
                    (string.Equals(quoteModel.QuoteStatus, ZnodeOrderStatusEnum.EXPIRED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(quoteModel.QuoteStatus, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(quoteModel.QuoteStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(quoteModel.QuoteStatus, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    || quoteModel.IsConvertedToOrder) ? false : true;
        }
        #endregion

        #region Private Methods

        //Get all address list of customer.
        private List<AddressModel> GetUserAddressList()
        {
            return _userAgent.GetAddressList()?.AddressList?.ToModel<AddressModel>()?.ToList();
        }

        //Bind all details of shopping  cart model.
        private void SetShoppingCartDetailsForQuotes(QuoteCreateViewModel quoteCreateViewModel, ShoppingCartModel cartModel)
        {
            if (IsNotNull(cartModel))
            {
                quoteCreateViewModel.CreatedDate = DateTime.Now.ToString();
                quoteCreateViewModel.UserId = cartModel.UserId.GetValueOrDefault();
                quoteCreateViewModel.PortalId = cartModel.PortalId;

                quoteCreateViewModel.QuoteExpirationDate = DateTime.Now;

                quoteCreateViewModel.ShippingCost = cartModel.ShippingCost;
                quoteCreateViewModel.TaxCost = cartModel.TaxCost;
                quoteCreateViewModel.ImportDuty = cartModel.ImportDuty.GetValueOrDefault();
                quoteCreateViewModel.TaxSummaryList = cartModel.TaxSummaryList;
                quoteCreateViewModel.QuoteTotal = cartModel.Total;

                quoteCreateViewModel.ShippingId = quoteCreateViewModel.ShippingId == 0 ? cartModel.ShippingId : quoteCreateViewModel.ShippingId;

                quoteCreateViewModel.AdditionalInstruction = quoteCreateViewModel.AdditionalInstruction;
                quoteCreateViewModel.PublishStateId = (byte)ZnodePublishStatesEnum.PRODUCTION;
                quoteCreateViewModel.InHandDate = quoteCreateViewModel.InHandDate;
                quoteCreateViewModel.ShippingConstraintCode = quoteCreateViewModel.ShippingConstraintCode;
                quoteCreateViewModel.productDetails = cartModel.ShoppingCartItems.ToViewModel<ProductDetailModel>().ToList();
                quoteCreateViewModel.ShippingHandlingCharges = cartModel.Shipping.ShippingHandlingCharge > 0 ? cartModel.Shipping.ShippingHandlingCharge : cartModel.ShippingHandlingCharges;
            }
        }
        #endregion
    }
}
