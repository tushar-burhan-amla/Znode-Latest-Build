using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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

namespace Znode.Engine.Admin.Agents
{
    public class RMAReturnAgent : BaseAgent, IRMAReturnAgent
    {
        #region Private Variables
        private readonly IRMAReturnClient _rmaReturnClient;
        private readonly IShoppingCartClient _shoppingCartClient;
        private readonly IOrderClient _orderClient;
        private readonly IUserClient _userClient;
        private readonly IPaymentClient _paymentClient;
        private IOrderAgent _orderAgent;
        private readonly IRMAConfigurationClient _rmaConfigurationClient;
        #endregion

        #region Constructor
        public RMAReturnAgent(IRMAReturnClient rmaReturnClient, IShoppingCartClient shoppingCartClient, IOrderClient orderClient, IUserClient userClient, IPaymentClient paymentClient, IRMAConfigurationClient rmaConfigurationClient)
        {
            _rmaReturnClient = GetClient<IRMAReturnClient>(rmaReturnClient);
            _shoppingCartClient = GetClient<IShoppingCartClient>(shoppingCartClient);
            _orderClient = GetClient<IOrderClient>(orderClient);
            _userClient = GetClient<IUserClient>(userClient);
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
            _rmaConfigurationClient = GetClient<IRMAConfigurationClient>(rmaConfigurationClient);
        }
        #endregion 

        #region Public methods
        //Get returns list
        public virtual RMAReturnListViewModel GetReturnList(FilterCollectionDataModel model, int portalId = 0, string portalName = null)
        {
            FilterCollection _filters = SetFiltersForReturnList(model?.Filters, portalId);

            RMAReturnListModel returnList = _rmaReturnClient.GetReturnList(null, _filters, model?.SortCollection, model?.Page, model?.RecordPerPage);
            RMAReturnListViewModel returnListViewModel = new RMAReturnListViewModel { ReturnList = returnList?.Returns?.ToViewModel<RMAReturnViewModel>()?.ToList() };
            SetListPagingData(returnListViewModel, returnList);
            if (returnListViewModel?.ReturnList?.Count > 0)
            {
                foreach (RMAReturnViewModel returns in returnListViewModel.ReturnList)
                    SetReturnListData(returns);
            }
            else
                returnListViewModel = new RMAReturnListViewModel();
            BindStoreFilterValues(returnListViewModel, portalId, portalName);
            return returnListViewModel;
        }

        //Manage return
        public virtual RMAReturnViewModel ManageReturn(string returnNumber)
        {
            try
            {
                RMAReturnViewModel returnModel = _rmaReturnClient.GetReturnDetailsForAdmin(returnNumber)?.ToViewModel<RMAReturnViewModel>();
                bool isReturnProcessComplete = IsReturnProcessComplete(returnModel.RmaReturnStateId);
                if (IsNotNull(returnModel) && IsNotNull(returnModel.RMAOrderModel) && (isReturnProcessComplete || returnModel?.RMAOrderModel?.ShoppingCartModel?.ShoppingCartItems.Count > 0))
                {
                    List<SelectListItem> orderStatusList = BindManageReturnStatus(new FilterTuple(ZnodeRmaReturnStateEnum.IsReturnLineItemState.ToString()?.ToLower(), FilterOperators.Equals, AdminConstants.True));
                    foreach (RMAReturnLineItemViewModel returnLineItem in returnModel?.ReturnLineItems)
                    {
                        ShoppingCartItemModel cartLineitem = returnModel.RMAOrderModel?.ShoppingCartModel?.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId && (string.Equals(x.OrderLineItemStatus, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(x.OrderLineItemStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(x.OrderLineItemStatus, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.InvariantCultureIgnoreCase)));
                        if (IsNotNull(cartLineitem))
                            cartLineitem.PersonaliseValuesDetail = returnModel?.RMAOrderModel?.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartLineitem.OmsOrderLineItemsId)?.PersonaliseValuesDetail;

                        if (IsNotNull(cartLineitem) || isReturnProcessComplete)
                            BindReturnLineItemData(orderStatusList, returnLineItem, cartLineitem);
                        else
                            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ShoppingCartNotNull);
                    }
                    SetReturnHistoryAmountWithCurrency(returnModel.CultureCode, returnModel.ReturnHistoryAndNotesList);
                    BindOrderReturnData(returnModel);
                    SaveInSession<RMAReturnViewModel>(AdminConstants.RMAReturnSessionKey + returnModel.ReturnNumber, returnModel);
                }
                return IsNotNull(returnModel) ? returnModel : new RMAReturnViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorAccessMessage);
                }
            }
            return new RMAReturnViewModel();
        }

//Get Return Status
public virtual RMAReturnStatusList GetReturnStatusList(string returnStatus)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ReturnStatus = returnStatus });

            if (!string.IsNullOrEmpty(returnStatus))
            {
                List<SelectListItem> statusList;
                statusList = BindManageReturnStatus(new FilterTuple(ZnodeRmaReturnStateEnum.IsReturnState.ToString()?.ToLower(), FilterOperators.Equals, AdminConstants.True));
                return new RMAReturnStatusList()
                {
                    ReturnStatusList = statusList,
                    SelectedItemValue = returnStatus,
                    SelectedItemId = Convert.ToInt32(statusList.FirstOrDefault(x => x.Text == returnStatus).Value)
                };
            }
            else
            {
                return new RMAReturnStatusList()
                {
                    ReturnStatusList = new List<SelectListItem>(),
                    SelectedItemId = 0
                };
            }
        }

        //Submit order return details.
        public virtual RMAReturnViewModel SubmitOrderReturn(string returnNumber, string notes)
        {
            RMAReturnViewModel returnViewModel = GetFromSession<RMAReturnViewModel>(AdminConstants.RMAReturnSessionKey + returnNumber);
            if (IsNotNull(returnViewModel) && IsNotNull(returnViewModel.OrderNumber) && returnViewModel?.ReturnLineItems?.Count > 0)
            {
                _orderAgent = DependencyResolver.Current.GetService<IOrderAgent>();
                if (returnViewModel.ReturnHistory?.Count < 1 && returnViewModel.ReturnLineItemHistory?.Count < 1)
                {
                    if (!string.IsNullOrEmpty(notes))
                    {
                        bool isNotesSaved = SaveReturnNotes(returnViewModel, notes);
                        return isNotesSaved ? new RMAReturnViewModel() { HasError = false, ErrorMessage = Admin_Resources.UpdateMessage } : new RMAReturnViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToUpdate };
                    }
                    return new RMAReturnViewModel() { HasError = false, ErrorMessage = Admin_Resources.UpdateMessage };
                }
                try
                {
                    returnViewModel.IsAdminRequest = true;
                    returnViewModel.Notes = notes;
                    returnViewModel?.ReturnLineItems?.Where(x => x.RmaReasonForReturnId == (int)ZnodeReturnStateEnum.REJECTED)?.ToList()?.ForEach(y => y.ReturnedQuantity = 0);
                    if (returnViewModel?.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || returnViewModel?.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED)
                    {
                        OrderModel orderModel = _orderClient.GetOrderById(returnViewModel.OmsOrderId, SetExpandForOrderDetails(true));
                        if (IsNotNull(orderModel) && orderModel?.OmsOrderId > 0 && IsNotNull(orderModel?.ShoppingCartModel))
                        {
                            orderModel.OrderOldValue.OrderLineItems = orderModel.OrderLineItems?.Where(x => !string.Equals(x.OrderLineItemState, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase))?.ToList();
                            orderModel.OrderOldValue.OrderLineItems.ForEach(s => s.Quantity = Convert.ToDecimal(s.Quantity.ToInventoryRoundOff()));
                            orderModel.OrderOldValue.ShippingAmount = HelperMethods.FormatPriceWithCurrency(orderModel.ShippingCost, orderModel.CultureCode);
                            orderModel.OrderOldValue.OrderState = orderModel.OrderState;
                            orderModel.OrderOldValue.ShippingId = orderModel.ShippingId;
                            orderModel.ReturnItemList.ReturnItemList.ForEach(x => x.PersonaliseValuesDetail = orderModel.OrderLineItems?.FirstOrDefault(order => order.OmsOrderLineItemsId == x.OmsOrderLineItemsId)?.PersonaliseValuesDetail);
                            UpdateOrderLineItemDetails(orderModel, returnViewModel);

                            if (ValidateOrderUpdate(orderModel))
                            {
                                orderModel.ShoppingCartModel.BillingAddress = orderModel.BillingAddress;
                                orderModel.ShoppingCartModel.ShippingAddress = orderModel.ShippingAddress;
                                orderModel.ShoppingCartModel.UserId = orderModel.UserId;
                                //Setting the SkipPreprocessing for UI webstore order validations.
                                orderModel.ShoppingCartModel.SkipPreprocessing = true;

                                _orderAgent.SetUserInformation(orderModel.ShoppingCartModel);
                                _orderAgent.SetUsersPaymentDetails(Convert.ToInt32(orderModel.PaymentSettingId), orderModel.ShoppingCartModel, (String.Equals(orderModel.PaymentType, ZnodeConstant.COD, StringComparison.OrdinalIgnoreCase)));

                                orderModel.ShoppingCartModel.OrderDate = DateTime.Now;

                                OrderModel updatedOrderModel = _orderClient.UpdateOrder(orderModel);

                                string shippingErrorMesage = updatedOrderModel?.ShoppingCartModel?.Shipping?.ResponseMessage;

                                OrderViewModel updatedOrderViewModel = updatedOrderModel.ToViewModel<OrderViewModel>();
                                if (!string.IsNullOrEmpty(shippingErrorMesage))
                                {
                                    returnViewModel.HasError = true;
                                    returnViewModel.ErrorMessage = shippingErrorMesage;
                                    return returnViewModel;
                                }
                                if (IsNotNull(updatedOrderViewModel) && !updatedOrderViewModel.HasError)
                                {
                                    if (returnViewModel.VoucherAmount != null)
                                        returnViewModel.OverDueAmount = -(returnViewModel.TotalReturnAmount - returnViewModel.VoucherAmount ?? 0);
                                    else
                                        returnViewModel.OverDueAmount = -(returnViewModel.TotalReturnAmount);
                                    returnViewModel = _rmaReturnClient.SubmitOrderReturn(returnViewModel.ToModel<RMAReturnModel>())?.ToViewModel<RMAReturnViewModel>();
                                    if (!returnViewModel.HasError)
                                        RemoveInSession(AdminConstants.RMAReturnSessionKey + returnNumber);

                                    return new RMAReturnViewModel() { ErrorMessage = returnViewModel.HasError ? Admin_Resources.ErrorFailedToUpdate : Admin_Resources.UpdateMessage };
                                }
                            }
                        }
                        else
                        {
                            throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);
                        }
                    }
                    else if (returnViewModel?.RmaReturnStateId == (int)ZnodeReturnStateEnum.REFUND_PROCESSED)
                    {
                        var data = _orderAgent.GetCustomerDetails(returnViewModel.PortalId, returnViewModel.UserId);

                        returnViewModel.RMAOrderModel.BillingAddress.FirstName = data.BillingAddress.FirstName;
                        returnViewModel.RMAOrderModel.BillingAddress.LastName = data.BillingAddress.LastName;
                        returnViewModel.RMAOrderModel.BillingAddress.CityName = data.BillingAddress.CityName;
                        returnViewModel.RMAOrderModel.BillingAddress.EmailAddress = data.Email;
                        returnViewModel.RMAOrderModel.BillingAddress.CountryName = data.BillingAddress.CountryName;
                        returnViewModel.RMAOrderModel.BillingAddress.PostalCode = data.BillingAddress.PostalCode;
                        returnViewModel.RMAOrderModel.BillingAddress.PhoneNumber = data.BillingAddress.PhoneNumber;
                        returnViewModel.RMAOrderModel.BillingAddress.CompanyName = data.BillingAddress.CompanyName;


                        returnViewModel.RMAOrderModel.ShippingAddress.FirstName = data.ShippingAddress.FirstName;
                        returnViewModel.RMAOrderModel.ShippingAddress.LastName = data.ShippingAddress.LastName;
                        returnViewModel.RMAOrderModel.ShippingAddress.CityName = data.ShippingAddress.CityName;
                        returnViewModel.RMAOrderModel.ShippingAddress.EmailAddress = data.Email;
                        returnViewModel.RMAOrderModel.ShippingAddress.CountryName = data.ShippingAddress.CountryName;
                        returnViewModel.RMAOrderModel.ShippingAddress.PostalCode = data.ShippingAddress.PostalCode;
                        returnViewModel.RMAOrderModel.ShippingAddress.PhoneNumber = data.ShippingAddress.PhoneNumber;
                        returnViewModel.RMAOrderModel.ShippingAddress.CompanyName = data.ShippingAddress.CompanyName;
                        returnViewModel.RMAOrderModel.ShippingAddress.StateName = data.ShippingAddress.StateName;
                        returnViewModel.RMAOrderModel.ShippingAddress.Address1 = data.ShippingAddress.Address1;

                        bool status = false;
                        if (!returnViewModel.IsRefundProcess)
                        {
                            string errorMessage = returnViewModel?.RMAOrderModel?.PaymentType == ZnodeConstant.COD ? Admin_Resources.PaymentRefundSuccessMessage : string.Empty;
                            string transactionId = returnViewModel?.RMAOrderModel?.PaymentTransactionToken;
                            decimal overDueAmount = IsNotNull(returnViewModel?.OverDueAmount) ? returnViewModel.OverDueAmount : 0;
                            if (overDueAmount != 0)
                            {
                                status = returnViewModel?.RMAOrderModel?.PaymentType == ZnodeConstant.COD ? true : RefundProcess(returnViewModel, returnViewModel.OverDueAmount, out errorMessage);
                            }
                            else
                            {
                                //When overDueAmount is 0, setting status as true that means Approved status will be updated to Refund Process in database.
                                status = true;
                            }
                            if (status)
                            {
                                returnViewModel.IsRefundProcess = true;
                                returnViewModel = _rmaReturnClient.SubmitOrderReturn(returnViewModel.ToModel<RMAReturnModel>()).ToViewModel<RMAReturnViewModel>();
                            }
                            else if (!string.IsNullOrEmpty(notes))
                            {
                                SaveReturnNotes(returnViewModel, notes);
                            }
                            
                            CreateRefundHistory(returnViewModel, errorMessage, transactionId, overDueAmount);
                            return new RMAReturnViewModel() { HasError = !status, ErrorMessage = errorMessage };
                        }
                        else
                        {
                            return new RMAReturnViewModel() { HasError = false, ErrorMessage = Admin_Resources.PaymentRefundSuccessMessage };
                        }
                    }
                    else
                    {
                        returnViewModel = _rmaReturnClient.SubmitOrderReturn(returnViewModel.ToModel<RMAReturnModel>()).ToViewModel<RMAReturnViewModel>();
                        return new RMAReturnViewModel() { ErrorMessage = returnViewModel.HasError ? Admin_Resources.ErrorFailedToUpdate : Admin_Resources.UpdateMessage };
                    }
                }
                catch (ZnodeException exception)
                {
                    ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                    switch (exception.ErrorCode)
                    {
                        case ErrorCodes.NotOrderEligibleForReturn:
                            return new RMAReturnViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorOrderNotEligibleForReturn };
                        case ErrorCodes.NullModel:
                            return new RMAReturnViewModel() { HasError = true, ErrorMessage = ZnodeConstant.NullModelError };
                        default:
                            return new RMAReturnViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    return new RMAReturnViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
                }
            }
            return new RMAReturnViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
        }

        //Update Order Return Line Item
        public virtual RMAReturnViewModel UpdateOrderReturnLineItem(RMAReturnLineItemViewModel orderReturnLineItemModel, string returnNumber)
        {
            RMAReturnViewModel returnViewModel = GetFromSession<RMAReturnViewModel>(AdminConstants.RMAReturnSessionKey + returnNumber);
            RMAReturnLineItemViewModel returnLineItemViewModel = returnViewModel?.ReturnLineItems?.FirstOrDefault(x => x.RmaReturnLineItemsId == orderReturnLineItemModel.RmaReturnLineItemsId);
            bool hasError = true;
            string errorMessage = string.Empty;
            if (IsValidUpdateOrderReturnData(returnViewModel, returnLineItemViewModel, orderReturnLineItemModel, ref hasError, ref errorMessage))
            {
                ShoppingCartItemModel shoppingCartItem = returnViewModel?.RMAOrderModel?.ShoppingCartModel?.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == orderReturnLineItemModel.OmsOrderLineItemsId &&string.Equals( x.OrderLineItemStatus,ZnodeOrderStatusEnum.SHIPPED.ToString(),StringComparison.InvariantCultureIgnoreCase));
                returnLineItemViewModel.ReturnedQuantity = orderReturnLineItemModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.REJECTED ? 0 : orderReturnLineItemModel.ReturnedQuantity;
                returnLineItemViewModel.RmaReturnStateId = orderReturnLineItemModel.RmaReturnStateId;
                returnLineItemViewModel.ReturnStatus = orderReturnLineItemModel.ReturnStatus;
                returnLineItemViewModel.IsShippingReturn = orderReturnLineItemModel.IsShippingReturn;
                RMAReturnCalculateModel calculateModel = new RMAReturnCalculateModel()
                {
                    UserId = returnViewModel.UserId,
                    PortalId = returnViewModel.PortalId,
                    OrderNumber = returnViewModel.OrderNumber,
                    IsAdminRequest = true
                };
                List<RMAReturnCalculateLineItemModel> returnCalculateLineItemList = new List<RMAReturnCalculateLineItemModel>();
                foreach (RMAReturnLineItemViewModel returnLineItem in returnViewModel?.ReturnLineItems)
                {
                    returnCalculateLineItemList.Add(new RMAReturnCalculateLineItemModel()
                    {
                        ExpectedReturnQuantity = returnLineItem.ExpectedReturnQuantity,
                        ReturnedQuantity = returnLineItem.ReturnedQuantity,
                        IsShippingReturn = returnLineItem.IsShippingReturn,
                        OmsOrderLineItemsId = (int)returnLineItem.OmsOrderLineItemsId,
                        RmaReturnLineItemsId = returnLineItem.RmaReturnLineItemsId
                    });
                }
                calculateModel.ReturnCalculateLineItemList = returnCalculateLineItemList;
                calculateModel = _rmaReturnClient.CalculateOrderReturn(calculateModel);

                RMAReturnCalculateLineItemModel returnCalculateLineItemModel = calculateModel?.ReturnCalculateLineItemList?.FirstOrDefault(x => x.RmaReturnLineItemsId == orderReturnLineItemModel.RmaReturnLineItemsId);
                if (IsNotNull(returnCalculateLineItemModel))
                {
                    returnLineItemViewModel.Total = returnCalculateLineItemModel.TotalLineItemPrice;
                    returnLineItemViewModel.ShippingCost = returnCalculateLineItemModel.ShippingCost;
                    returnLineItemViewModel.TaxCost = returnCalculateLineItemModel.TaxCost;
                    returnLineItemViewModel.ImportDuty = returnCalculateLineItemModel.ImportDuty;
                    returnLineItemViewModel.PerQuantityShippingCost = returnCalculateLineItemModel.PerQuantityShippingCost;
                    returnLineItemViewModel.PerQuantityShippingDiscount = returnCalculateLineItemModel.PerQuantityShippingDiscount;
                    returnLineItemViewModel.PerQuantityCSRDiscount = returnCalculateLineItemModel.PerQuantityCSRDiscount;
                    returnLineItemViewModel.PerQuantityLineItemDiscount = returnCalculateLineItemModel.PerQuantityLineItemDiscount;
                    returnLineItemViewModel.PerQuantityOrderLevelDiscountOnLineItem = returnCalculateLineItemModel.PerQuantityOrderLevelDiscountOnLineItem;
                    returnLineItemViewModel.PerQuantityVoucherAmount = returnCalculateLineItemModel.PerQuantityVoucherAmount;
                    UpdateReturnLineItemHistory(returnViewModel, returnLineItemViewModel);

                    BindTotalSummaryData(returnViewModel, calculateModel);
                    SaveInSession<RMAReturnViewModel>(AdminConstants.RMAReturnSessionKey + returnNumber, returnViewModel);
                    returnViewModel.HasError = hasError;
                    returnViewModel.ErrorMessage = errorMessage;
                }
                else
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorFailToCalculateReturnTotal);
                }
            }
            return returnViewModel;
        }

        //Update Order Return Status
        public virtual RMAReturnViewModel UpdateOrderReturnStatus(int returnStatusCode, string returnNumber)
        {
            if (!string.IsNullOrEmpty(returnNumber) && returnStatusCode > 0)
            {
                RMAReturnViewModel returnViewModel = GetFromSession<RMAReturnViewModel>(AdminConstants.RMAReturnSessionKey + returnNumber);
                bool hasError = true;
                string errorMessage = "";
                if (IsNotNull(returnViewModel))
                {
                    UpdateReturnStatus(returnStatusCode, returnViewModel, ref hasError, ref errorMessage);

                    if (!hasError)
                    {
                        RemoveKeyFromDictionaryForReturn(returnViewModel, ZnodeConstant.ReturnUpdatedStatus);
                        if (!Equals(returnViewModel.OldReturnStatus, returnViewModel.ReturnStatus))
                            ReturnHistory(returnViewModel, ZnodeConstant.ReturnUpdatedStatus, returnViewModel.ReturnStatus);
                        SaveInSession<RMAReturnViewModel>(AdminConstants.RMAReturnSessionKey + returnNumber, returnViewModel);
                    }
                    returnViewModel.HasError = hasError;
                    returnViewModel.ErrorMessage = errorMessage;
                    return returnViewModel;
                }
            }
            return new RMAReturnViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorUpdateReturnStatus };
        }

        //Print order return receipt by return number
        public virtual RMAReturnViewModel PrintReturnReceipt(string returnNumber)
        {
            RMAReturnViewModel returnViewModel = _rmaReturnClient.GetReturnDetails(returnNumber, SetExpandForReturnDetails())?.ToViewModel<RMAReturnViewModel>();
            SetPortalDetails(returnViewModel);
            return IsNotNull(returnViewModel) ? returnViewModel : new RMAReturnViewModel();
        }      

        //Check if the order is eligible for return
        public virtual bool IsOrderEligibleForReturn(string orderNumber, int userId, int portalId)
        {
            if (!string.IsNullOrEmpty(orderNumber) && userId > 0 && portalId > 0)
                return _rmaReturnClient.IsOrderEligibleForReturn(userId, portalId, orderNumber);
            else
                return false;
        }

        //Get order details by order number for return.
        public virtual RMAReturnOrderDetailViewModel GetOrderDetailsForReturn(string orderNumber, int userId)
        {
            if (userId > 0 && !string.IsNullOrEmpty(orderNumber))
            {
                bool isFromAdmin = true;
                // Get order details by order number for return.
                OrderModel orderModel = _rmaReturnClient.GetOrderDetailsForReturn(userId, orderNumber, isFromAdmin);
                if (IsNotNull(orderModel))
                {
                    RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel = new RMAReturnOrderDetailViewModel();
                    rmaReturnOrderDetailViewModel = GetReturnOrderDetail(orderModel);
                    rmaReturnOrderDetailViewModel.OrderNumber = orderNumber;
                    return rmaReturnOrderDetailViewModel;
                }
            }
            return new RMAReturnOrderDetailViewModel();
        }

        //Perform calculations for an order return line item.
        public virtual RMAReturnCalculateViewModel CalculateOrderReturn(RMAReturnCalculateViewModel returnCalculateViewModel)
        {
            if (IsNotNull(returnCalculateViewModel) && IsNotNull(returnCalculateViewModel.OrderNumber) && returnCalculateViewModel?.ReturnCalculateLineItemList?.Count > 0)
            {
                try
                {
                    //IsAdminRequest
                    RMAReturnCalculateModel rMAReturnCalculateModel = returnCalculateViewModel.ToModel<RMAReturnCalculateModel>();
                    rMAReturnCalculateModel.IsAdminRequest = true;
                    returnCalculateViewModel = _rmaReturnClient.CalculateOrderReturn(rMAReturnCalculateModel).ToViewModel<RMAReturnCalculateViewModel>();
                    if (IsNull(returnCalculateViewModel))
                    {
                        return new RMAReturnCalculateViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailToCalculateReturnTotal };
                    }
                    else if (returnCalculateViewModel.HasError)
                    {
                        returnCalculateViewModel.ErrorMessage = WebStore_Resources.ErrorFailToCalculateReturnTotal;
                    }
                    else
                    {
                        returnCalculateViewModel.ErrorMessage = string.Empty;
                        returnCalculateViewModel?.ReturnCalculateLineItemList?.ForEach(x => x.TotalLineItemPriceWithCurrency = HelperMethods.FormatPriceWithCurrency(x.TotalLineItemPrice, returnCalculateViewModel.CultureCode));
                    }
                    return returnCalculateViewModel;

                }

                catch (ZnodeException exception)
                {
                    ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                    return new RMAReturnCalculateViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailToCalculateReturnTotal };
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    return new RMAReturnCalculateViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailToCalculateReturnTotal };
                }
            }
            else
            {
                return new RMAReturnCalculateViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailToCalculateReturnTotal };
            }
        }

        //Submit order return
        public virtual RMAReturnViewModel SubmitCreateReturn(RMAReturnViewModel returnViewModel)
        {
            if (IsNotNull(returnViewModel) && IsNotNull(returnViewModel.OrderNumber) && returnViewModel?.ReturnLineItems?.Count > 0)
            {
                try
                {                  
                    returnViewModel = _rmaReturnClient.SubmitOrderReturn(returnViewModel.ToModel<RMAReturnModel>()).ToViewModel<RMAReturnViewModel>();
                    return new RMAReturnViewModel() { ErrorMessage = returnViewModel.HasError ? WebStore_Resources.ErrorSubmitReturn : WebStore_Resources.SuccessSubmitReturn, ReturnNumber = returnViewModel.ReturnNumber };
                }
                catch (ZnodeException exception)
                {
                    ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    switch (exception.ErrorCode)
                    {
                        case ErrorCodes.InvalidData:
                            return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.TextInvalidData };

                        case ErrorCodes.NotOrderEligibleForReturn:
                            return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorOrderNotEligibleForReturn };

                        default:
                            return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorSubmitReturn };
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                }
            }
            return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorSubmitReturn };
        }

        //Get order return details by return number
        public virtual RMAReturnViewModel GetReturnDetails(string returnNumber, bool isReturnDetailsReceipt = true)
        {
            RMAReturnViewModel returnModel = _rmaReturnClient.GetReturnDetails(returnNumber, SetExpandForReturnDetails(isReturnDetailsReceipt))?.ToViewModel<RMAReturnViewModel>();
            SetPortalDetails(returnModel);
            if (IsNotNull(returnModel))
            {
                returnModel.IsReturnDetailsReceipt = isReturnDetailsReceipt;
                if (isReturnDetailsReceipt)
                    returnModel.ReturnNotes = BindNotesDetails(returnModel.ReturnNotes);
            }
            return IsNotNull(returnModel) ? returnModel : new RMAReturnViewModel();
        }

        //Validate Return Item for create return.
        public virtual RMAReturnViewModel IsValidReturnItems(RMAReturnViewModel returnViewModel)
        {
            if (IsNotNull(returnViewModel) && returnViewModel?.ReturnLineItems?.Count > 0)
            {
                try
                {
                   returnViewModel = _rmaReturnClient.IsValidReturnItems(returnViewModel.ToModel<RMAReturnModel>()).ToViewModel<RMAReturnViewModel>();
                    return returnViewModel;
                }

                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    return new RMAReturnViewModel() { HasError = true, ErrorMessage = ex.Message };
                }
            }
            return new RMAReturnViewModel() { HasError = true, ErrorMessage = Admin_Resources.TextInvalidData };
        }
        #endregion

        #region Protected methods   
        //Convert Enumerable list of Order state model to select lit items.
        protected virtual List<SelectListItem> ToReturnStateList(IEnumerable<RMAReturnStateModel> returnStateList)
        {
            List<SelectListItem> returnStateItems = new List<SelectListItem>();
            if (returnStateList?.Count() > 0)
                returnStateItems = (from item in returnStateList
                                    orderby item.DisplayOrder ascending
                                    select new SelectListItem
                                    {
                                        Text = item.ReturnStateName,
                                        Value = item.RmaReturnStateId.ToString()
                                    }).ToList();


            int index = returnStateItems.FindIndex(x => string.Equals(x.Text, ZnodeConstant.ReturnStateNotSubmitted, StringComparison.CurrentCultureIgnoreCase));
            if (index != -1)
                returnStateItems.RemoveAt(index);
            return returnStateItems;
        }

        //Set Return History Amount With Currency
        protected virtual void SetReturnHistoryAmountWithCurrency(string cultureCode, List<RMAReturnHistoryViewModel> returnHistoryList)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CultureCode = cultureCode });
            if (IsNotNull(returnHistoryList) && returnHistoryList.Count > 0)
            {
                foreach (RMAReturnHistoryViewModel returnHistory in returnHistoryList)
                {
                    returnHistory.ReturnAmountWithCurrency = IsNull(returnHistory.ReturnAmount) ? string.Empty : HelperMethods.FormatPriceWithCurrency(returnHistory.ReturnAmount, cultureCode);
                    returnHistory.ReturnDateWithTime = Convert.ToDateTime(returnHistory.CreatedDate).ToString(HelperMethods.GetStringDateTimeFormat());
                }
            }
        }

        //Bind Order Return Data
        protected virtual void BindOrderReturnData(RMAReturnViewModel returnModel)
        {
            returnModel.ReturnDateWithTime = Convert.ToDateTime(returnModel.ReturnDate).ToString(HelperMethods.GetStringDateTimeFormat());
            returnModel.PhoneNumber = returnModel.RMAOrderModel?.BillingAddress?.PhoneNumber;
            returnModel.PaymentType = returnModel?.RMAOrderModel?.PaymentType;
            returnModel.PaymentDisplayName = returnModel?.RMAOrderModel?.PaymentDisplayName;
            returnModel.CreditCardNumber = returnModel?.RMAOrderModel?.CreditCardNumber;
            returnModel.OldReturnStatus = returnModel.ReturnStatus;
            returnModel.OldRmaReturnStateId = returnModel.RmaReturnStateId;
            returnModel.OldReturnLineItems?.ForEach(x => { if (x.ReturnedQuantity < 1) x.ReturnedQuantity = x.ExpectedReturnQuantity; });
            BindTotalSummaryData(returnModel);
        }

        //Bind Total Summary Data
        protected virtual void BindTotalSummaryData(RMAReturnViewModel returnViewModel, RMAReturnCalculateModel calculateModel = null)
        {
            if (IsNotNull(calculateModel))
            {
                BindCalculateTotalSummaryData(returnViewModel, calculateModel);  
            }
            if (IsNotNull(returnViewModel))
            {
                if (returnViewModel?.ReturnLineItems?.Count > 0 && returnViewModel.OldRmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED && returnViewModel?.RmaReturnStateId == (int)ZnodeReturnStateEnum.REJECTED)
                {
                    BindRejectedTotalSummaryData(returnViewModel);
                }
                returnViewModel.ReturnTotalModel = new RMAReturnTotalViewModel()
                {
                    SubTotal = returnViewModel.SubTotal,
                    ReturnTaxCost = returnViewModel.ReturnTaxCost,
                    ReturnShippingCost = returnViewModel.ReturnShippingCost,
                    DiscountAmount = returnViewModel.DiscountAmount,
                    CSRDiscount = returnViewModel.CSRDiscount,
                    CultureCode = returnViewModel.CultureCode,
                    ReturnShippingDiscount = returnViewModel.ReturnShippingDiscount,
                    ReturnCharges = returnViewModel.ReturnCharges,
                    TotalReturnAmount = returnViewModel.TotalReturnAmount,
                    VoucherAmount = returnViewModel.VoucherAmount,
                    ReturnImportDuty = returnViewModel.ReturnImportDuty
                };
                BindVoucherAmountData(returnViewModel);
            }
        }
        protected virtual void BindVoucherAmountData(RMAReturnViewModel returnViewModel)
        {
            if (IsNotNull(returnViewModel) && (returnViewModel.VoucherAmount == null) && (returnViewModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || returnViewModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED) && returnViewModel.RMAOrderModel?.GiftCardAmount > 0)
            {
                if (returnViewModel.RMAOrderModel?.GiftCardAmount >= returnViewModel.ReturnTotalModel.TotalReturnAmount)
                {
                    returnViewModel.ReturnTotalModel.VoucherAmount = returnViewModel.ReturnTotalModel.TotalReturnAmount;
                    returnViewModel.VoucherAmount = returnViewModel.ReturnTotalModel.TotalReturnAmount;
                }
                else
                {
                    returnViewModel.ReturnTotalModel.VoucherAmount = returnViewModel.RMAOrderModel?.GiftCardAmount;
                    returnViewModel.VoucherAmount = returnViewModel.RMAOrderModel?.GiftCardAmount;
                }
            }
        }

        protected virtual void BindRejectedTotalSummaryData(RMAReturnViewModel returnViewModel)
        {
            if(HelperUtility.IsNotNull(returnViewModel))
            {
                returnViewModel.SubTotal = returnViewModel.DiscountAmount = returnViewModel.ReturnTaxCost = returnViewModel.TotalReturnAmount = returnViewModel.CSRDiscount = 0;
                returnViewModel.ReturnShippingCost = 0;
            }
            
        }

        //Bind Calculate Total Summary Data
        protected virtual void BindCalculateTotalSummaryData(RMAReturnViewModel returnViewModel, RMAReturnCalculateModel calculateModel)
        {
            returnViewModel.SubTotal = calculateModel.ReturnSubTotal;
            returnViewModel.ReturnTaxCost = calculateModel.ReturnTaxCost;
            returnViewModel.ReturnShippingCost = calculateModel.ReturnShippingCost;
            returnViewModel.DiscountAmount = calculateModel.Discount;
            returnViewModel.CSRDiscount = calculateModel.CSRDiscount;
            returnViewModel.ReturnShippingDiscount = calculateModel.ReturnShippingDiscount;
            returnViewModel.TotalReturnAmount = calculateModel.ReturnTotal;
            returnViewModel.ReturnCharges = calculateModel.ReturnCharges;
            foreach (RMAReturnCalculateLineItemModel calculatedLineItem in calculateModel.ReturnCalculateLineItemList)
            {
                returnViewModel.ReturnLineItems?.Where(t => t.RmaReturnLineItemsId == calculatedLineItem?.RmaReturnLineItemsId)?.Select(t => { t.TaxCost = calculatedLineItem.TaxCost; return t; })?.ToList();
            }
        }

        //Bind Return Line Item Data
        protected virtual void BindReturnLineItemData(List<SelectListItem> orderStatusList, RMAReturnLineItemViewModel returnLineItem, ShoppingCartItemModel cartLineitem)
        {
            returnLineItem.ReturnStatusList = orderStatusList;
            returnLineItem.ReturnedQuantity = string.IsNullOrEmpty(returnLineItem?.ReturnedQuantity?.ToString()) ? returnLineItem.ExpectedReturnQuantity : returnLineItem.ReturnedQuantity;
            returnLineItem.Total = returnLineItem.Price * (decimal)returnLineItem.ReturnedQuantity;
            if (IsNotNull(cartLineitem))
            {
                returnLineItem.ImagePath = cartLineitem.ImagePath;
                returnLineItem.ShippingCost = string.IsNullOrEmpty(returnLineItem?.ShippingCost?.ToString()) ? cartLineitem.ShippingCost / cartLineitem?.Quantity * returnLineItem.ReturnedQuantity ?? 0 : returnLineItem.ShippingCost;
                returnLineItem.TaxCost = cartLineitem.TaxCost > 0 && returnLineItem.ReturnedQuantity != 0 ? (cartLineitem.TaxCost / cartLineitem.Quantity) * (decimal)returnLineItem.ReturnedQuantity : 0;
                returnLineItem.ProductId = cartLineitem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group ? cartLineitem?.GroupProducts?.FirstOrDefault()?.ProductId : 0;
                returnLineItem.PersonaliseValuesDetail = cartLineitem.PersonaliseValuesDetail;
                returnLineItem.PersonaliseValuesList = cartLineitem.PersonaliseValuesList;
                returnLineItem.PerQuantityLineItemDiscount = cartLineitem.PerQuantityLineItemDiscount;
                returnLineItem.PerQuantityOrderLevelDiscountOnLineItem = cartLineitem.PerQuantityOrderLevelDiscountOnLineItem;
                returnLineItem.PerQuantityCSRDiscount = cartLineitem.PerQuantityCSRDiscount;
                returnLineItem.PerQuantityShippingDiscount = cartLineitem.PerQuantityShippingDiscount;
                returnLineItem.PaymentStatusId = cartLineitem.PaymentStatusId;
                returnLineItem.PerQuantityVoucherAmount = cartLineitem.PerQuantityVoucherAmount;
                returnLineItem.ParentOmsOrderLineItemsId = cartLineitem.ParentOmsOrderLineItemsId;
                returnLineItem.ImportDuty = cartLineitem.ImportDuty > 0 && returnLineItem.ReturnedQuantity != 0 ? (cartLineitem.ImportDuty / cartLineitem.Quantity) * (decimal)returnLineItem.ReturnedQuantity : 0;

            }
        }

        //Is Valid Update Order Return Data
        protected virtual bool IsValidUpdateOrderReturnData(RMAReturnViewModel returnViewModel, RMAReturnLineItemViewModel returnLineItemViewModel, RMAReturnLineItemViewModel orderReturnLineItemModel, ref bool hasError, ref string errorMessage)
        {
            if (IsNull(returnViewModel) || IsNull(returnViewModel.OrderNumber) || returnViewModel?.ReturnLineItems?.Count == 0 || returnViewModel.UserId == 0 || returnViewModel.PortalId == 0)
            {
                hasError = true;
                errorMessage = Admin_Resources.ErrorFailToCalculateReturnTotal;
                return false;
            }

            if (IsNotNull(returnLineItemViewModel) && IsNotNull(orderReturnLineItemModel))
            {
                if (returnLineItemViewModel.ExpectedReturnQuantity < orderReturnLineItemModel.ReturnedQuantity || returnLineItemViewModel.Total < orderReturnLineItemModel.RefundAmount)
                {
                    hasError = true;
                    errorMessage = Admin_Resources.ErrorReturned;
                    return false;
                }
                else
                {
                    hasError = false;
                    errorMessage = string.Empty;
                    return true;
                }
            }
            else
                return false;
        }

        //To update filter with specified details.
        protected virtual FilterCollection SetFiltersForReturnList(FilterCollection filters, int portalId)
        {
            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);
            _filters.RemoveAll(x => string.Equals(x.Item1, FilterKeys.IsFromAdmin.ToString(), StringComparison.InvariantCultureIgnoreCase));
            _filters.Add(new FilterTuple(FilterKeys.IsFromAdmin, FilterOperators.Equals, FilterKeys.ActiveTrueValue));

            //Add portal id in filter collection.
            DependencyResolver.Current.GetService<IOrderAgent>().AddPortalIdInFilters(_filters, portalId);

            DateRangePickerHelper.FormatFilterForDateTimeRange(_filters, DateTimeRange.Last_30_Days.ToString(), DateTimeRange.All_Returns.ToString());
            return _filters;
        }

        //Update return status according to status code
        protected virtual void UpdateReturnStatus(int returnStatusCode, RMAReturnViewModel returnViewModel, ref bool hasError, ref string errorMessage)
        {
            if (IsNotNull(returnViewModel) && returnViewModel?.ReturnLineItems?.Count > 0)
            {
                hasError = true;
                switch (returnStatusCode)
                {
                    case (int)ZnodeReturnStateEnum.SUBMITTED:
                        if (returnViewModel.RmaReturnStateId != returnStatusCode)
                            errorMessage = Admin_Resources.ErrorUpdateSubmittedReturnStatus;
                        else
                            UpdateReturnStatusInViewModel(returnStatusCode, returnViewModel, ref hasError, ref errorMessage);
                        break;

                    case (int)ZnodeReturnStateEnum.RECEIVED:
                        if (returnViewModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || returnViewModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED)
                            errorMessage = Admin_Resources.ErrorUpdateReturnStatusToRefundProcessed;
                        else
                            UpdateReturnStatusInViewModel(returnStatusCode, returnViewModel, ref hasError, ref errorMessage);
                        break;

                    case (int)ZnodeReturnStateEnum.IN_REVIEW:
                        if (returnViewModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || returnViewModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED)
                            errorMessage = Admin_Resources.ErrorUpdateReturnStatusToRefundProcessed;
                        else
                            UpdateReturnStatusInViewModel(returnStatusCode, returnViewModel, ref hasError, ref errorMessage);
                        break;

                    case (int)ZnodeReturnStateEnum.APPROVED:
                        if (returnViewModel.ReturnLineItems.Any(x => x.RmaReturnStateId != (int)ZnodeReturnStateEnum.APPROVED && x.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED) || !returnViewModel.ReturnLineItems.Any(x => x.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED))
                            errorMessage = Admin_Resources.ErrorUpdateApprovedReturnStatus;
                        else if ((string.Equals(returnViewModel.RMAOrderModel?.PaymentType, ZnodeConstant.CreditCard.ToString(), StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals(returnViewModel.RMAOrderModel?.PaymentType, ZnodeConstant.Amazon_Pay.ToString(), StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals(returnViewModel.RMAOrderModel?.PaymentType, ZnodeConstant.PAYPAL_EXPRESS.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            && string.Equals(returnViewModel.RMAOrderModel?.PaymentStatus, ZnodeConstant.AUTHORIZED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            errorMessage = Admin_Resources.ErrorCapturePaymentToApproveReturn;
                        else
                            UpdateReturnStatusInViewModel(returnStatusCode, returnViewModel, ref hasError, ref errorMessage);
                        break;

                    case (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED:
                        if (returnViewModel.ReturnLineItems.Any(x => x.RmaReturnStateId == (int)ZnodeReturnStateEnum.SUBMITTED || x.RmaReturnStateId == (int)ZnodeReturnStateEnum.RECEIVED) || !returnViewModel.ReturnLineItems.Any(x => x.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED))
                            errorMessage = Admin_Resources.ErrorUpdatePartiallyApprovedReturnStatus;
                        else if ((string.Equals(returnViewModel.RMAOrderModel?.PaymentType, ZnodeConstant.CreditCard.ToString(), StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals(returnViewModel.RMAOrderModel?.PaymentType, ZnodeConstant.Amazon_Pay.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            && string.Equals(returnViewModel.RMAOrderModel?.PaymentStatus, ZnodeConstant.AUTHORIZED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            errorMessage = Admin_Resources.ErrorCapturePaymentToApproveReturn;
                        else
                            UpdateReturnStatusInViewModel(returnStatusCode, returnViewModel, ref hasError, ref errorMessage);
                        break;

                    case (int)ZnodeReturnStateEnum.REJECTED:
                        if (returnViewModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || returnViewModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED)
                            errorMessage = Admin_Resources.ErrorUpdateReturnStatusToRefundProcessed;
                        else if (returnViewModel.ReturnLineItems.Any(x => x.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || x.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED))
                            errorMessage = Admin_Resources.ErrorUpdateRejectedReturnStatus;
                        else
                        {
                            UpdateReturnStatusInViewModel(returnStatusCode, returnViewModel, ref hasError, ref errorMessage);
                            returnViewModel.ReturnLineItems.ForEach(x => { x.RmaReturnStateId = returnStatusCode; x.ReturnedQuantity = 0; x.Total = 0; x.IsShippingReturn = false; x.ShippingCost = 0; x.RefundAmount = 0; x.TaxCost = 0; });
                            BindTotalSummaryData(returnViewModel);
                        }
                        break;

                    case (int)ZnodeReturnStateEnum.REFUND_PROCESSED:
                        if (returnViewModel.OldRmaReturnStateId != (int)ZnodeReturnStateEnum.APPROVED && returnViewModel.OldRmaReturnStateId != (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED)
                            errorMessage = Admin_Resources.ErrorUpdateRefundProcessedReturnStatus;
                        else if ((string.Equals(returnViewModel.RMAOrderModel?.PaymentType, ZnodeConstant.CreditCard.ToString(), StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals(returnViewModel.RMAOrderModel?.PaymentType, ZnodeConstant.Amazon_Pay.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            && string.Equals(returnViewModel.RMAOrderModel?.PaymentStatus, ZnodeConstant.AUTHORIZED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            errorMessage = Admin_Resources.ErrorCapturePaymentToProcessRefund;
                        else if(string.Equals(returnViewModel.RMAOrderModel?.PaymentType, ZnodeConstant.COD.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            errorMessage = errorMessage + " " + Admin_Resources.MessageRefundErrorForCOD;
                        else
                        {
                            UpdateReturnStatusInViewModel(returnStatusCode, returnViewModel, ref hasError, ref errorMessage);
                            errorMessage = errorMessage + " " + Admin_Resources.MessageSaveChangesToProcessRefund;
                        }
                        break;

                    default:
                        errorMessage = Admin_Resources.ErrorUpdateReturnStatus;
                        break;
                }
            }
        }

        //Update return status in return view model
        protected virtual void UpdateReturnStatusInViewModel(int returnStatusCode, RMAReturnViewModel returnViewModel, ref bool hasError, ref string errorMessage)
        {
            if (IsNotNull(returnViewModel))
            {
                hasError = false;
                errorMessage = Admin_Resources.SuccessUpdateReturnStatus;
                returnViewModel.RmaReturnStateId = returnStatusCode;
                returnViewModel.ReturnStatus = Enum.GetName(typeof(ZnodeReturnStateEnum), returnStatusCode)?.Replace("_", " ")?.ToLower()?.ToProperCase();
                BindVoucherAmountData(returnViewModel);
            }
        }

        //Update Order Line Item Details
        protected virtual void UpdateOrderLineItemDetails(OrderModel orderModel, RMAReturnViewModel returnViewModel)
        {
            _orderAgent = DependencyResolver.Current.GetService<IOrderAgent>();
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ShoppingCartModel cart = orderModel?.ShoppingCartModel;
            List<RMAReturnLineItemViewModel> returnLineItemList = returnViewModel?.ReturnLineItems?.Where(x => x.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || x.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED)?.ToList();
            if (IsNotNull(cart) && returnLineItemList?.Count > 0)
            {
                cart.ReturnCharges = orderModel.ReturnCharges.GetValueOrDefault() + returnViewModel.ReturnCharges;
                foreach (RMAReturnLineItemViewModel returnLineItem in returnLineItemList)
                {
                    ManageOrderDataModel orderDataModel = new ManageOrderDataModel()
                    {
                        CustomQuantity = (decimal)returnLineItem.ReturnedQuantity,
                        OrderLineItemStatusId = ZnodeConstant.ReturnOrderLineItemStatusId,
                        OrderLineItemStatus = ZnodeOrderStatusEnum.RETURNED.ToString(),
                        ReasonForReturnId = (int)returnLineItem.RmaReasonForReturnId,
                        ReasonForReturn = returnLineItem.RmaReasonForReturn,
                        OmsOrderLineItemsId = (int)returnLineItem.OmsOrderLineItemsId,
                        ProductId = (int)returnLineItem.ProductId,
                        IsShippingReturn = returnLineItem.IsShippingReturn,
                        ShippingCost = returnLineItem.IsShippingReturn ? (decimal)returnLineItem.ShippingCost : 0,
                    };

                    // Check if item exists.
                    ShoppingCartItemModel cartItem = cart?.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == orderDataModel.OmsOrderLineItemsId);
                    if (IsNull(cartItem))
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ShoppingCartNotNull);
                    cartItem.PersonaliseValuesDetail = orderModel?.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartItem.OmsOrderLineItemsId)?.PersonaliseValuesDetail;                   

                    _orderAgent.SetCreatedByUser(cart?.UserId);

                    decimal orderlineItemQuantity = GetOrderLineItemQuantity(orderModel, cartItem);
                    orderDataModel.Quantity = orderlineItemQuantity;
                    ZnodeLogging.LogMessage("OrderlineItemQuantity: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderlineItemQuantity = orderlineItemQuantity });

                    if (IsNotNull(cart))
                    {
                        GetUpdatedOrderLineItem(cart, cartItem, orderDataModel, orderModel);

                        cart.ShippingAddress = cart.ShippingAddress;
                        cart.Payment = new PaymentModel() { ShippingAddress = cart.ShippingAddress, PaymentSetting = new PaymentSettingModel() };

                        //Set Gift Card Number and CSR Discount Amount Data For Calculation
                        cart.GiftCardNumber = cart.GiftCardNumber;
                        cart.CSRDiscountAmount = cart.CSRDiscountAmount - returnLineItem.PerQuantityCSRDiscount * returnLineItem.ReturnedQuantity.GetValueOrDefault();
                        cart.ShippingCost = cart.ShippingCost - returnLineItem.PerQuantityShippingCost * returnLineItem.ReturnedQuantity.GetValueOrDefault();

                        if (!string.IsNullOrEmpty(orderDataModel.OrderLineItemStatus) && string.Equals(orderDataModel.OrderLineItemStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            ShoppingCartItemModel cartItemModel = GetCustomCartItem(cart.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == orderDataModel.OmsOrderLineItemsId));
                            GetReturnLineItemList(orderDataModel, orderModel, cart, cartItemModel);
                        }

                        //This code will Execute, if ship separately line item return with shipping cost
                        if (!IsNull(orderDataModel) && orderDataModel.IsShippingReturn)
                        {
                            UpdateReturnShippingHistory(orderModel, orderDataModel, returnLineItem, true);
                        }
                    }
                }
                if (cart.GiftCardAmount > 0)
                {
                    cart.GiftCardAmount = (orderModel.GiftCardAmount - returnViewModel.VoucherAmount) ?? 0;
                    orderModel.GiftCardAmount = cart.GiftCardAmount;
                    cart.IsCalculateVoucher = cart.IsQuoteOrder ? false : true;
                    GetUpdatedVoucherlistForApprovedReturn(orderModel, returnViewModel.VoucherAmount ?? 0);
                }
                orderModel.ShoppingCartModel = _orderAgent.GetCalculatedShoppingCartForEditOrder(cart);
                orderModel.ShoppingCartModel.GiftCardAmount = orderModel.GiftCardAmount;
            }
        }

        //The method is used to update the voucher amount of voucherlist of return when voucher is applied in order.
        protected virtual void GetUpdatedVoucherlistForApprovedReturn(OrderModel orderModel, decimal giftCardAmount)
        {        
            if (orderModel.ShoppingCartModel?.Vouchers?.Count > 0)
            {
                foreach (VoucherModel voucher in orderModel.ShoppingCartModel.Vouchers)
                {
                    decimal voucherUsed = voucher.VoucherAmountUsed;
                    if (voucher.VoucherAmountUsed > 0 && giftCardAmount > 0)
                    {
                        if (voucher.VoucherAmountUsed >= giftCardAmount)
                        {
                            voucher.VoucherAmountUsed = voucherUsed - giftCardAmount;
                            giftCardAmount = 0;
                            break;
                        }
                        else
                            giftCardAmount = giftCardAmount - voucherUsed;
                        voucher.VoucherAmountUsed = 0;
                    }
                }
            }
        }

        //Create return line items list
        protected virtual void GetReturnLineItemList(ManageOrderDataModel orderDataModel, OrderModel orderModel, ShoppingCartModel cart, ShoppingCartItemModel cartItemModel)
        {
            cart.ShoppingCartItems?.Remove(cart.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == orderDataModel.OmsOrderLineItemsId));

            if (orderModel.ReturnItemList?.ReturnItemList?.Count > 0)
                orderModel.ReturnItemList.ReturnItemList.Add(GetReturnLineItem(cartItemModel, orderDataModel));
            else
            {
                orderModel.ReturnItemList = new ReturnOrderLineItemListModel { ReturnItemList = new List<ReturnOrderLineItemModel>() };
                orderModel.ReturnItemList.ReturnItemList.Add(GetReturnLineItem(cartItemModel, orderDataModel));
            }
            orderModel.ReturnItemList.SubTotal = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.ExtendedPrice);

            if (!orderModel.IsTaxCostEdited)
                orderModel.ReturnItemList.TaxCost = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.TaxCost);

            cart.IsLineItemReturned = true;
            cart.ReturnItemList = orderModel.ReturnItemList.ReturnItemList;

            if (orderModel?.ShoppingCartModel.ShoppingCartItems?.Count < 1 && orderModel.ReturnItemList?.ReturnItemList?.Count > 0)
                orderModel.OrderHistory.Add(ZnodeConstant.OrderReturnAllAndCancelStatus, ZnodeOrderStatusEnum.CANCELED.ToString());
        }

        //Get quantity of ordered line item.
        protected virtual decimal GetOrderLineItemQuantity(OrderModel orderModel, ShoppingCartItemModel cartItemModel)
        {
            return (cartItemModel.GroupProducts?.Count > 0 ?
                    (orderModel.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartItemModel.OmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)?.Quantity).GetValueOrDefault()
                    : !string.IsNullOrEmpty(cartItemModel.ConfigurableProductSKUs) ? (orderModel.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartItemModel.OmsOrderLineItemsId)?.Quantity).GetValueOrDefault() :
                    (orderModel.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartItemModel.OmsOrderLineItemsId)?.Quantity).GetValueOrDefault());
        }

        //Update custom data for cart items.
        protected virtual void GetUpdatedOrderLineItem(ShoppingCartModel cartModel, ShoppingCartItemModel shoppingCartItem, ManageOrderDataModel orderDataModel, OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string sku = string.Empty;
            OrderLineItemHistoryModel orderLineItemHistoryModel = new OrderLineItemHistoryModel();

            if (IsNotNull(shoppingCartItem))
            {
                sku = orderDataModel.ProductId > 0 ? shoppingCartItem.ProductName + "-" + Convert.ToString(shoppingCartItem.GroupProducts?.Where(y => y.ProductId == orderDataModel.ProductId).Select(s => s.Sku).FirstOrDefault()) : sku = shoppingCartItem.SKU;

                //Set sku of the actual product irrespective of any product type
                orderLineItemHistoryModel.SKU = (orderDataModel.ProductId > 0)
                                    ? shoppingCartItem.GroupProducts?.FirstOrDefault(y => y.ProductId == orderDataModel.ProductId)?.Sku
                                    : !string.IsNullOrEmpty(shoppingCartItem.ConfigurableProductSKUs) ? shoppingCartItem.ConfigurableProductSKUs : shoppingCartItem.SKU;

                ZnodeLogging.LogMessage("SKU of the actual product: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { SKU = orderLineItemHistoryModel.SKU });

                DependencyResolver.Current.GetService<IOrderAgent>().RemoveKeyFromDictionary(orderModel, sku, true);

                if (orderDataModel.OrderLineItemStatusId > 0 && !Equals(shoppingCartItem.OmsOrderStatusId, orderDataModel.OrderLineItemStatusId) && !string.IsNullOrEmpty(orderDataModel.OrderLineItemStatus))
                {
                    orderLineItemHistoryModel.OrderUpdatedStatus = orderDataModel.OrderLineItemStatus;
                    shoppingCartItem.IsItemStateChanged = true;
                }

                if (HelperUtility.IsNotNull(orderDataModel.PartialRefundAmount) && orderDataModel.PartialRefundAmount > 0 && orderDataModel.PartialRefundAmount != shoppingCartItem.PartialRefundAmount)
                {
                    shoppingCartItem.PartialRefundAmount = orderDataModel.PartialRefundAmount;
                    orderLineItemHistoryModel.PartialRefundAmount = HelperMethods.FormatPriceWithCurrency(orderDataModel.PartialRefundAmount ?? 0, orderModel.CultureCode);
                }

                if (IsNotNull(orderLineItemHistoryModel))
                {
                    orderLineItemHistoryModel.ProductName = shoppingCartItem.ProductName;
                    orderLineItemHistoryModel.Quantity = orderDataModel.CustomQuantity > 0 ? orderDataModel.CustomQuantity.ToInventoryRoundOff() : (orderDataModel.Quantity > shoppingCartItem.Quantity || orderDataModel.Quantity < shoppingCartItem.Quantity) ? orderDataModel.Quantity.ToInventoryRoundOff() : shoppingCartItem.Quantity.ToInventoryRoundOff();
                    orderLineItemHistoryModel.OmsOrderLineItemsId = shoppingCartItem.OmsOrderLineItemsId;
                    orderLineItemHistoryModel.TaxCost = shoppingCartItem.TaxCost / orderDataModel?.Quantity * Convert.ToDecimal(orderLineItemHistoryModel.Quantity) ?? 0;
                    orderLineItemHistoryModel.ImportDuty = shoppingCartItem.ImportDuty / orderDataModel?.Quantity * Convert.ToDecimal(orderLineItemHistoryModel.Quantity) ?? 0;
                    orderLineItemHistoryModel.ReturnShippingAmount = orderDataModel.IsShippingReturn ? orderDataModel.ShippingCost.ToInventoryRoundOff() : string.Empty;
                    orderLineItemHistoryModel.SubTotal = Convert.ToDecimal(orderLineItemHistoryModel.Quantity) * shoppingCartItem.UnitPrice;
                    orderLineItemHistoryModel.SubTotal = orderLineItemHistoryModel.SubTotal - GetReturnLineItemDiscount(shoppingCartItem, Convert.ToInt32(orderLineItemHistoryModel.Quantity));
                    OrderLineItemHistory(orderModel, sku, orderLineItemHistoryModel);
                }

                if (orderDataModel.OrderLineItemStatusId > 0 && orderDataModel.CustomQuantity > 0 && orderDataModel.Quantity > orderDataModel.CustomQuantity)
                {
                    ShoppingCartItemModel customCartItem = GetCustomCartItem(shoppingCartItem, orderDataModel, cartModel.TaxCost);
                    customCartItem.GroupProducts = GetCustomGroupProductItems(shoppingCartItem, orderDataModel);

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

                cartModel.ShoppingCartItems.Insert(cartModel.ShoppingCartItems.FindIndex(x => x.OmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId), shoppingCartItem);

                if (orderModel.ReturnItemList?.ReturnItemList?.Count > 0)
                    cartModel.ShoppingCartItems.Remove(cartModel.ShoppingCartItems.LastOrDefault(x => x.OmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId));
                else
                    cartModel.ShoppingCartItems.Remove(cartModel.ShoppingCartItems.LastOrDefault(x => x.OmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId));

                ZnodeLogging.LogMessage("ShoppingCartItem model with CustomUnitPrice, OmsOrderStatusId and ExternalId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CustomUnitPrice = shoppingCartItem.CustomUnitPrice, OmsOrderStatusId = shoppingCartItem.OmsOrderStatusId, ExternalId = shoppingCartItem.ExternalId });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get Custom Cart Item
        protected virtual ShoppingCartItemModel GetCustomCartItem(ShoppingCartItemModel itemModel, ManageOrderDataModel orderDataModel = null, decimal? cartTaxCost = null)
       => IsNotNull(itemModel) ? new ShoppingCartItemModel
       {
           Description = itemModel.Description,
           ExtendedPrice = IsNotNull(orderDataModel?.UnitPrice) ? orderDataModel.UnitPrice.GetValueOrDefault() * orderDataModel.CustomQuantity : itemModel.ExtendedPrice,
           ExternalId = itemModel.ExternalId,
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
           TrackingNumber = itemModel.TrackingNumber,
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
           ParentOmsOrderLineItemsId = itemModel.ParentOmsOrderLineItemsId,
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
           AdditionalCost = itemModel.AdditionalCost,
           PerQuantityCSRDiscount = itemModel.PerQuantityCSRDiscount,
           PerQuantityLineItemDiscount = itemModel.PerQuantityLineItemDiscount,
           PerQuantityShippingCost = itemModel.PerQuantityShippingCost,
           PerQuantityShippingDiscount = itemModel.PerQuantityShippingDiscount,
           PerQuantityOrderLevelDiscountOnLineItem = itemModel.PerQuantityOrderLevelDiscountOnLineItem,
           PaymentStatusId = itemModel.PaymentStatusId,
           PerQuantityVoucherAmount = itemModel.PerQuantityVoucherAmount
       } : new ShoppingCartItemModel();

        //Get Return Line Item
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
                returnLineItem.PersonaliseValuesList = cartItemModel.PersonaliseValuesList;
                returnLineItem.PersonaliseValuesDetail = cartItemModel.PersonaliseValuesDetail;
                returnLineItem.ReasonForReturn = orderDataModel.ReasonForReturn;
                returnLineItem.ReasonForReturnId = orderDataModel.ReasonForReturnId;
                returnLineItem.GroupProducts = GetCustomGroupProductItems(cartItemModel, orderDataModel);
                returnLineItem.OmsOrderLineItemsId = cartItemModel.OmsOrderLineItemsId;
                returnLineItem.ParentOmsOrderLineItemsId = cartItemModel.ParentOmsOrderLineItemsId;
                returnLineItem.AddOnLineItemId = cartItemModel.AddOnLineItemId;
                returnLineItem.Custom1 = cartItemModel.Custom1;
                returnLineItem.Custom2 = cartItemModel.Custom2;
                returnLineItem.Custom3 = cartItemModel.Custom3;
                returnLineItem.Custom4 = cartItemModel.Custom4;
                returnLineItem.Custom5 = cartItemModel.Custom5;
                returnLineItem.IsShippingReturn = orderDataModel.IsShippingReturn;
                returnLineItem.PartialRefundAmount = orderDataModel.PartialRefundAmount;
                returnLineItem.AdditionalCost = cartItemModel.AdditionalCost;
                returnLineItem.PerQuantityCSRDiscount = cartItemModel.PerQuantityCSRDiscount;
                returnLineItem.PerQuantityLineItemDiscount = cartItemModel.PerQuantityLineItemDiscount;
                returnLineItem.PerQuantityShippingCost = cartItemModel.PerQuantityShippingCost;
                returnLineItem.PerQuantityShippingDiscount = cartItemModel.PerQuantityShippingDiscount;
                returnLineItem.PerQuantityOrderLevelDiscountOnLineItem = cartItemModel.PerQuantityOrderLevelDiscountOnLineItem;
                returnLineItem.PaymentStatusId = cartItemModel.PaymentStatusId.GetValueOrDefault();
                returnLineItem.PerQuantityVoucherAmount = cartItemModel.PerQuantityVoucherAmount;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return returnLineItem;
            }
            return null;
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
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrder.ToString());
            expands.Add(ExpandKeys.ZnodeShipping);
            expands.Add(ExpandKeys.ZnodeUser);
            ZnodeLogging.LogMessage("ExpandForOrderDetails: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands });
            return expands;
        }

        //Validate Order Update
        protected virtual bool ValidateOrderUpdate(OrderModel orderModel)
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

        //Map store filter values in view model
        protected virtual void BindStoreFilterValues(RMAReturnListViewModel returnListViewModel, int portalId, string portalName)
        {
            returnListViewModel.PortalName = string.IsNullOrEmpty(portalName) ? Admin_Resources.DefaultAllStores : portalName;
            returnListViewModel.PortalId = portalId;
        }

        //Set return list data in ReturnListViewModel.
        protected virtual void SetReturnListData(RMAReturnViewModel returns)
        {
            if (IsNotNull(returns))
            {
                returns.ReturnDateWithTime = returns.ReturnDate.ToTimeFormat();
                returns.ReturnTotalWithCurrency = HelperMethods.FormatPriceWithCurrency(returns.TotalReturnAmount, returns.CultureCode);
            }
        }

        // Remove key from dictionary.
        protected virtual void RemoveKeyFromDictionaryForReturn(RMAReturnViewModel returnViewModel, string key, bool isFromLineItem = false)
        {
            if (IsNotNull(returnViewModel?.ReturnHistory) && !isFromLineItem)
            {
                if (returnViewModel.ReturnHistory.ContainsKey(key))
                    returnViewModel.ReturnHistory.Remove(key);
            }
            else
            {
                if (IsNotNull(returnViewModel.ReturnLineItemHistory) && returnViewModel.ReturnLineItemHistory.ContainsKey(key))
                    returnViewModel.ReturnLineItemHistory.Remove(key);
            }
        }

        // Add key and value in dictionary for return.
        protected virtual void ReturnHistory(RMAReturnViewModel returnViewModel, string settingType, string newValue = "")
            => returnViewModel.ReturnHistory?.Add(settingType, newValue);

        // Add key and value in return line dictionary.
        protected virtual void ReturnLineItemHistory(RMAReturnViewModel returnViewModel, string key, RMAReturnLineItemHistoryModel lineItemHistory)
            => returnViewModel.ReturnLineItemHistory?.Add(key, lineItemHistory);

        // Add key and value in order line dictionary.
        protected virtual void OrderLineItemHistory(OrderModel orderModel, string key, OrderLineItemHistoryModel lineHistory) => orderModel.OrderLineItemHistory?.Add(key, lineHistory);

        // Update Return line item history
        protected virtual void UpdateReturnLineItemHistory(RMAReturnViewModel returnViewModel, RMAReturnLineItemViewModel returnLineItemModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(returnLineItemModel) && IsNotNull(returnViewModel))
            {
                string lineitemHistorykey = returnLineItemModel.Sku + "_" + returnLineItemModel.RmaReturnLineItemsId;

                RMAReturnLineItemViewModel oldReturnLineItemModel = returnViewModel.OldReturnLineItems?.FirstOrDefault(x => x.RmaReturnLineItemsId == returnLineItemModel.RmaReturnLineItemsId);
                if (IsNotNull(oldReturnLineItemModel))
                {
                    RemoveKeyFromDictionaryForReturn(returnViewModel, lineitemHistorykey, true);

                    RMAReturnLineItemHistoryModel returnLineItemHistoryModel = new RMAReturnLineItemHistoryModel();

                    returnLineItemHistoryModel.SKU = returnLineItemModel.Sku;
                    returnLineItemHistoryModel.RmaReturnLineItemsId = returnLineItemModel.RmaReturnLineItemsId;
                    returnLineItemHistoryModel.ProductName = returnLineItemModel.ProductName + "<br/>" + returnLineItemModel?.Description.TrimStart().Replace("<br />", " ");
                    returnLineItemHistoryModel.Total = returnLineItemModel.Total;
                    returnLineItemHistoryModel.ReturnShippingAmount = returnLineItemModel.ShippingCost ?? 0;

                    if (oldReturnLineItemModel.RmaReturnStateId != returnLineItemModel.RmaReturnStateId)
                        returnLineItemHistoryModel.ReturnUpdatedStatus = returnLineItemModel.ReturnStatus;

                    if (IsNull(oldReturnLineItemModel.ReturnedQuantity) && oldReturnLineItemModel.ExpectedReturnQuantity != returnLineItemModel.ReturnedQuantity && returnLineItemModel.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED)
                        returnLineItemHistoryModel.ReturnedQuantity = returnLineItemModel.ReturnedQuantity.ToString();
                    else if (IsNotNull(oldReturnLineItemModel.ReturnedQuantity) && oldReturnLineItemModel.ReturnedQuantity != returnLineItemModel.ReturnedQuantity && returnLineItemModel.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED)
                        returnLineItemHistoryModel.ReturnedQuantity = returnLineItemModel.ReturnedQuantity.ToString();

                    if (oldReturnLineItemModel.RefundAmount != returnLineItemModel.RefundAmount)
                        returnLineItemHistoryModel.PartialRefundAmount = HelperMethods.FormatPriceWithCurrency(returnLineItemModel.RefundAmount, returnViewModel.CultureCode);

                    if (oldReturnLineItemModel.IsShippingReturn != returnLineItemModel.IsShippingReturn)
                        returnLineItemHistoryModel.IsShippingReturn = returnLineItemModel.IsShippingReturn ? ZnodeConstant.True : ZnodeConstant.False;

                    ReturnLineItemHistory(returnViewModel, lineitemHistorykey, returnLineItemHistoryModel);
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }
        //Refund Process
        protected virtual bool RefundProcess(RMAReturnViewModel returnViewModel, decimal overDueAmount, out string errorMessage)
        {
            OrderModel orderModel = returnViewModel.RMAOrderModel;
            errorMessage = string.Empty;
            bool status = false;

            if (IsNotNull(orderModel))
            {
                _orderAgent = DependencyResolver.Current.GetService<IOrderAgent>();

                if (String.Equals(orderModel.PaymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.OrdinalIgnoreCase) && 
                    !(returnViewModel.ReturnCharges > 0) && !(orderModel.ReturnCharges > 0) && !(orderModel.ShippingHandlingCharges > 0))
                {
                    //Void or Refund transactions
                    status = _orderAgent.VoidRefundPayment(orderModel, false, out errorMessage, orderModel.OmsOrderId);
                }

                if (String.Equals(orderModel.PaymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase) && overDueAmount < 0 && 
                    (returnViewModel.ReturnCharges > 0 || orderModel.ReturnCharges > 0 || orderModel.ShippingHandlingCharges > 0 ||
                    String.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    //Refund order line item transactions
                    status = _orderAgent.RefundPaymentByAmount(orderModel.OmsOrderDetailsId, orderModel.PaymentTransactionToken, overDueAmount, out errorMessage,orderModel);
                    errorMessage = status ? Admin_Resources.PaymentRefundSuccessMessage : errorMessage;
                }

                //Amazon refund for cancel order.
                if (String.Equals(orderModel.PaymentType, ZnodeConstant.Amazon_Pay, StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    //Void or Refund transactions
                    status = _orderAgent.VoidRefundPayment(orderModel, false, out errorMessage, orderModel.OmsOrderId);
                }

                //Amazon refund.
                if (String.Equals(orderModel.PaymentType, ZnodeConstant.Amazon_Pay, StringComparison.OrdinalIgnoreCase) && overDueAmount < 0)
                {
                    //Refund order line item transactions
                    status = _orderAgent.RefundPaymentByAmount(orderModel.OmsOrderDetailsId, orderModel.PaymentTransactionToken, overDueAmount, out errorMessage,orderModel);
                    errorMessage = status ? Admin_Resources.PaymentRefundSuccessMessage : errorMessage;
                }

                //Purchase Order and InvoiceMe refund.
                if (String.Equals(orderModel.PaymentType, ZnodeConstant.PurchaseOrder, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(orderModel.PaymentType, ZnodeConstant.InvoiceMe, StringComparison.OrdinalIgnoreCase))
                {
                    //Void or Refund transactions
                    status = _orderAgent.VoidRefundPayment(orderModel, false, out errorMessage, orderModel.OmsOrderId);
                    errorMessage = status ? Admin_Resources.PaymentRefundSuccessMessage : errorMessage;
                }
                
                //PayPalExpress refund
                if (String.Equals(orderModel.PaymentType, ZnodeConstant.PAYPAL_EXPRESS, StringComparison.OrdinalIgnoreCase) && 
                    (String.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.OrdinalIgnoreCase) &&
                    !(returnViewModel.ReturnCharges > 0) && !(orderModel.ReturnCharges > 0) && !(orderModel.ShippingHandlingCharges > 0)))
                {
                    //Void or Refund transactions
                    status = _orderAgent.VoidRefundPayment(orderModel, false, out errorMessage, orderModel.OmsOrderId);
                }
                if (String.Equals(orderModel.PaymentType, ZnodeConstant.PAYPAL_EXPRESS, StringComparison.OrdinalIgnoreCase) && overDueAmount < 0 &&
                    (returnViewModel.ReturnCharges > 0 || orderModel.ReturnCharges > 0 || orderModel.ShippingHandlingCharges > 0 ||
                    String.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    //Refund order line item transactions
                    status = _orderAgent.RefundPaymentByAmount(orderModel.OmsOrderDetailsId, orderModel.PaymentTransactionToken, overDueAmount, out errorMessage, orderModel);
                    errorMessage = status ? Admin_Resources.PaymentRefundSuccessMessage : errorMessage;
                }
            }
            return status;
        }

        //Create Refund History
        protected virtual void CreateRefundHistory(RMAReturnViewModel returnViewModel, string errorMessage, string transactionId, decimal overDueAmount)
        {
            if (overDueAmount != 0)
            {
                List<RMAReturnHistoryModel> returnHistoryModelList = new List<RMAReturnHistoryModel>();
                string refundTransactionId = _paymentClient.GetRefundTransactionId(transactionId);
                returnHistoryModelList.Add(new RMAReturnHistoryModel
                {
                    RmaReturnDetailsId = returnViewModel.RmaReturnDetailsId,
                    Message = errorMessage,
                    TransactionId = string.IsNullOrEmpty(refundTransactionId) ? transactionId : refundTransactionId,
                    ReturnAmount = overDueAmount < 0 ? overDueAmount : -(overDueAmount)
                });
                _rmaReturnClient.CreateReturnHistory(returnHistoryModelList);
            }
        }

        //Set expand for Return Details.
        protected virtual ExpandCollection SetExpandForReturnDetails()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.ReturnItemList);
            expands.Add(ExpandKeys.ReturnShippingDetails);
            expands.Add(ExpandKeys.ReturnProductImages);
            expands.Add(ExpandKeys.ReturnBarcode);
            ZnodeLogging.LogMessage("Expand for return details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands });
            return expands;
        }

        //Save return notes
        protected virtual bool SaveReturnNotes(RMAReturnViewModel returnViewModel, string notes)
        {
            if (IsNotNull(returnViewModel) && !string.IsNullOrEmpty(notes))
            {
                int userId = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);
                return _rmaReturnClient.SaveReturnNotes(new RMAReturnNotesModel()
                {
                    RmaReturnDetailsId = returnViewModel.RmaReturnDetailsId,
                    Notes = notes,
                    CreatedBy = userId,
                    ModifiedBy = userId,
                });
            }
            else
                return false;
        }

        //Set portal details in return view model
        protected virtual void SetPortalDetails(RMAReturnViewModel returnViewModel)
        {
            if (IsNotNull(returnViewModel) && returnViewModel?.PortalId > 0)
            {
                PortalModel portalModel = GetClient<PortalClient>().GetPortal(returnViewModel.PortalId, null);
                returnViewModel.CustomerServiceEmail = portalModel?.CustomerServiceEmail;
                returnViewModel.CustomerServicePhoneNumber = portalModel?.CustomerServicePhoneNumber;
                returnViewModel.StoreName = portalModel?.StoreName;
            }
        }

        //Update Return Shipping
        protected virtual void UpdateReturnShippingHistory(OrderModel orderModel, ManageOrderDataModel orderDataModel, RMAReturnLineItemViewModel returnLineItem, bool isInsert = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderModel = orderModel, ManageOrderDataModel = orderDataModel, IsInsert = isInsert });

            List<OrderLineItemHistoryModel> orderLineHistoryList = orderModel.OrderLineItemHistory.Select(s => (OrderLineItemHistoryModel)s.Value).ToList();

            if (IsNotNull(orderModel) && IsNotNull(orderDataModel))
            {
                decimal? shippingCost = orderDataModel?.ShippingCost;

                OrderLineItemHistoryModel orderLineItemHistoryModel = new OrderLineItemHistoryModel()
                {
                    OmsOrderLineItemsId = orderDataModel.OmsOrderLineItemsId,
                    OrderLineQuantity = string.Empty,
                    OrderTrackingNumber = string.Empty,
                    Quantity = string.Empty,
                    PartialRefundAmount = string.Empty,
                    IsShippingReturn = true,
                    ProductName = (returnLineItem?.ProductName ?? string.Empty),
                    ReturnShippingAmount = (shippingCost ?? 0).ToString(),
                    SKU = (returnLineItem?.Sku ?? string.Empty).ToString(),
                };

                orderLineItemHistoryModel.ReturnShippingAmount = HelperMethods.FormatPriceWithCurrency((isInsert == true) ? (shippingCost ?? 0) : 0, orderModel.CultureCode);

                if (orderLineHistoryList?.Count() == 0 || IsNull(orderLineHistoryList))
                {
                    if (isInsert && shippingCost > 0)
                        OrderLineItemHistory(orderModel, returnLineItem.Sku, orderLineItemHistoryModel);
                }
                else
                {
                    DependencyResolver.Current.GetService<IOrderAgent>().RemoveKeyFromDictionary(orderModel, "Shipping_" + returnLineItem.Sku, true);
                    if (isInsert && shippingCost > 0)
                        OrderLineItemHistory(orderModel, "Shipping_" + returnLineItem.Sku, orderLineItemHistoryModel);
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Bind Default return status and line item status on the basis of return state and return line item state
        protected virtual List<SelectListItem> BindManageReturnStatus(FilterTuple filter = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            if (filter != null) { filters.Add(filter); }
            ZnodeLogging.LogMessage("Filters to get order states: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            return ToReturnStateList(_rmaReturnClient.GetReturnStatusList(null, filters, null, null, null)?.ReturnStates);
        }

        //Check is return process complete or not
        protected virtual bool IsReturnProcessComplete(int returnStatus) =>
            returnStatus == (int)ZnodeReturnStateEnum.APPROVED ||
            returnStatus == (int)ZnodeReturnStateEnum.REJECTED ||
            returnStatus == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED ||
            returnStatus == (int)ZnodeReturnStateEnum.REFUND_PROCESSED ? true : false;


        //Get the order details and return the view model
        protected virtual RMAReturnOrderDetailViewModel GetReturnOrderDetail(OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel = new RMAReturnOrderDetailViewModel();
            if (orderModel?.OmsOrderId > 0)
            {
                rmaReturnOrderDetailViewModel = orderModel.ToViewModel<RMAReturnOrderDetailViewModel>();

                if (IsNotNull(orderModel?.ShoppingCartModel))
                {
                    SetPersonaliseForShoppingCart(orderModel.ShoppingCartModel, orderModel?.OrderLineItems);
                    rmaReturnOrderDetailViewModel.ReturnCartViewModel = orderModel?.ShoppingCartModel?.ToViewModel<RMAReturnCartViewModel>();
                    rmaReturnOrderDetailViewModel.ReturnCalculateViewModel = new RMAReturnCalculateViewModel()
                    {
                        ReturnSubTotal = null,
                        ReturnTaxCost = null,
                        ReturnShippingCost = null,
                        ReturnTotal = null,
                        CultureCode = orderModel.CultureCode
                    };
                    //Bind Reason For Return line item
                    if (rmaReturnOrderDetailViewModel?.ReturnCartViewModel?.ShoppingCartItems.Count > 0)
                    {
                        List<SelectListItem> reasonForReturnItems = BindReasonForReturn();
                        if (reasonForReturnItems?.Count > 0)
                            rmaReturnOrderDetailViewModel.ReturnCartViewModel.ShoppingCartItems.ForEach(x => x.ReasonForReturnItems = reasonForReturnItems);
                    }
                }
            }
            return rmaReturnOrderDetailViewModel;
        }

        //To Set personalize attribute in ShoppingCartModel.
        protected virtual void SetPersonaliseForShoppingCart(ShoppingCartModel shoppingCartModel, List<OrderLineItemModel> orderLineItemModel)
        {
            List<OrderLineItemModel> orderLineItemList = orderLineItemModel?.Where(w => w.ParentOmsOrderLineItemsId == null && w.OrderLineItemState.ToUpper() != ZnodeOrderStatusEnum.RETURNED.ToString() && w.PersonaliseValueList?.Count > 0).ToList();
            if (IsNotNull(orderLineItemList))
                foreach (ShoppingCartItemModel shoppingCart in shoppingCartModel?.ShoppingCartItems)
                {
                    shoppingCart.PersonaliseValuesList = orderLineItemModel?.FirstOrDefault(x => x.OmsOrderLineItemsId == shoppingCart.OmsOrderLineItemsId)?.PersonaliseValueList;
                    shoppingCart.PersonaliseValuesDetail = orderLineItemModel?.FirstOrDefault(x => x.OmsOrderLineItemsId == shoppingCart.OmsOrderLineItemsId)?.PersonaliseValuesDetail;
                    shoppingCart.GroupId = orderLineItemModel?.FirstOrDefault(x => x.OmsOrderLineItemsId == shoppingCart.OmsOrderLineItemsId)?.GroupId;
                }
        }

        //Bind Reason For Return
        protected virtual List<SelectListItem> BindReasonForReturn()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, FilterKeys.ActiveTrueValue));

            SortCollection sort = new SortCollection();
            sort.Add(SortKeys.Name, SortDirections.Ascending);

            IEnumerable<RequestStatusModel> rmaReasonListModel = _rmaConfigurationClient.GetReasonForReturnList(null, filters, sort, null, null).RequestStatusList;
            List<SelectListItem> rmaReasonItems = new List<SelectListItem>();
            if (IsNotNull(rmaReasonListModel) && rmaReasonListModel?.Count() > 0)
            {
                rmaReasonItems = (from item in rmaReasonListModel
                                  select new SelectListItem
                                  {
                                      Text = item.Name,
                                      Value = item.RmaReasonForReturnId.ToString(),
                                  }).ToList();
            }
            return rmaReasonItems;
        }

        //Set expand for Return Details.
        protected virtual ExpandCollection SetExpandForReturnDetails(bool isReturnDetailsReceipt)
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.ReturnItemList);
            expands.Add(ExpandKeys.ReturnShippingDetails);
            expands.Add(ExpandKeys.ReturnProductImages);
            expands.Add(ExpandKeys.ReturnBarcode);

            if (isReturnDetailsReceipt)
                expands.Add(ExpandKeys.ReturnNotes);

            ZnodeLogging.LogMessage("Expand for return details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands });
            return expands;
        }

        //Bind return notes details
        protected virtual List<RMAReturnNotesViewModel> BindNotesDetails(List<RMAReturnNotesViewModel> returnNotesList)
        {
            if (IsNotNull(returnNotesList) && returnNotesList.Count > 0)
            {
                foreach (RMAReturnNotesViewModel notes in returnNotesList)
                {
                    notes.ModifiedTime = Convert.ToDateTime(notes.ModifiedDate).ToString(HelperMethods.GetStringTimeFormat());
                    notes.ModifiedDate = Convert.ToDateTime(notes.ModifiedDate).ToString(HelperMethods.GetStringDateTimeFormat());
                }
            }
            return returnNotesList;
        }


        //Get the line item discount.
        private decimal GetReturnLineItemDiscount(ShoppingCartItemModel cartItem, int returnQuantity)
        {
            return (cartItem.PerQuantityLineItemDiscount * returnQuantity) + (cartItem.PerQuantityOrderLevelDiscountOnLineItem * returnQuantity) + (cartItem.PerQuantityCSRDiscount * returnQuantity);
        }
        #endregion
    }
}