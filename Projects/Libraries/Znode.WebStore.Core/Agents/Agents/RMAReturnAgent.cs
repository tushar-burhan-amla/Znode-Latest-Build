using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.WebStore.Agents
{
    public class RMAReturnAgent : BaseAgent, IRMAReturnAgent
    {
        #region Private Variables
        private readonly IRMAReturnClient _rmaReturnClient;
        private readonly IUserAgent _userAgent;
        private readonly IOrderClient _orderClient;
        private readonly IRMAConfigurationClient _rmaConfigurationClient;
        #endregion

        #region Constructor
        public RMAReturnAgent(IRMAReturnClient rmaReturnClient, IOrderClient orderClient, IRMAConfigurationClient rmaConfigurationClient)
        {
            _rmaReturnClient = GetClient<IRMAReturnClient>(rmaReturnClient);
            _userAgent = DependencyResolver.Current.GetService<IUserAgent>();
            _orderClient = GetClient<IOrderClient>(orderClient);
            _rmaConfigurationClient = GetClient<IRMAConfigurationClient>(rmaConfigurationClient);
        }
        #endregion

        #region Public Methods

        //Get order details by order number for return.
        public virtual RMAReturnOrderDetailViewModel GetOrderDetailsForReturn(string orderNumber = "")
        {
            if(!string.IsNullOrEmpty(orderNumber))
            {
                int userId = GetUserID(orderNumber);
                if (userId > 0)
                {
                    // Get order details by order number for return.
                    OrderModel orderModel = _rmaReturnClient.GetOrderDetailsForReturn(userId, orderNumber);
                    if (IsNotNull(orderModel))
                    {
                        RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel = new RMAReturnOrderDetailViewModel();
                        rmaReturnOrderDetailViewModel = GetReturnOrderDetail(orderModel);
                        rmaReturnOrderDetailViewModel.OrderNumber = orderNumber;
                        return rmaReturnOrderDetailViewModel;
                    }
                }
            }
            return new RMAReturnOrderDetailViewModel();
        }

        //Get returns list for user.
        public virtual RMAReturnListViewModel GetReturnList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();
            filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());
            filters.RemoveAll(x => x.Item1 == ZnodePortalEnum.PortalId.ToString());
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, _userAgent.GetUserViewModelFromSession()?.UserId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString()));

            RMAReturnListModel returnList = _rmaReturnClient.GetReturnList(null, filters, sortCollection, pageIndex, recordPerPage);
            RMAReturnListViewModel returnListViewModel = new RMAReturnListViewModel { ReturnList = returnList?.Returns?.ToViewModel<RMAReturnViewModel>()?.ToList() };
            SetListPagingData(returnListViewModel, returnList);
            return returnListViewModel?.ReturnList?.Count > 0 ? returnListViewModel : new RMAReturnListViewModel();
        }

        //Check if the order is eligible for return
        public virtual bool IsOrderEligibleForReturn(string orderNumber)
        {
            if (!string.IsNullOrEmpty(orderNumber))
            {
                int userId = GetUserID(orderNumber);
                if (userId > 0)
                    return _rmaReturnClient.IsOrderEligibleForReturn(userId, PortalAgent.CurrentPortal.PortalId, orderNumber);
                else
                    return false;
            }
            return false;
        }

        //Get order return details by return number
        public virtual RMAReturnViewModel GetReturnDetails(string returnNumber, bool isReturnDetailsReceipt = true)
        {
            RMAReturnViewModel returnModel = _rmaReturnClient.GetReturnDetails(returnNumber, SetExpandForReturnDetails(isReturnDetailsReceipt))?.ToViewModel<RMAReturnViewModel>();
            if (IsNotNull(returnModel))
            {
                returnModel.IsReturnDetailsReceipt = isReturnDetailsReceipt;
                if (isReturnDetailsReceipt)
                    returnModel.ReturnNotes = BindNotesDetails(returnModel.ReturnNotes);

                returnModel.StoreName = PortalAgent.CurrentPortal.Name;
            }
            return IsNotNull(returnModel) ? returnModel : new RMAReturnViewModel();
        }

        //Insert or update order return details.
        public virtual RMAReturnViewModel SaveOrderReturn(RMAReturnViewModel returnViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(returnViewModel) && IsNotNull(returnViewModel.OrderNumber) && returnViewModel?.ReturnLineItems?.Count > 0)
            {
                int userId = GetUserID(returnViewModel.OrderNumber);
                if (userId > 0)
                {
                    try
                    {
                        string actionMode = string.IsNullOrEmpty(returnViewModel.ReturnNumber) ? ZnodeConstant.Create : ZnodeConstant.Edit;
                        returnViewModel.UserId = userId;
                        returnViewModel.PortalId = PortalAgent.CurrentPortal.PortalId;
                        returnViewModel = _rmaReturnClient.SaveOrderReturn(returnViewModel.ToModel<RMAReturnModel>()).ToViewModel<RMAReturnViewModel>();
                        return new RMAReturnViewModel() { ErrorMessage = returnViewModel.HasError ? WebStore_Resources.ErrorFailedToCreate : actionMode == ZnodeConstant.Create ? WebStore_Resources.ReturnSavedSuccessfully : WebStore_Resources.UpdateMessage, ReturnNumber = returnViewModel.ReturnNumber };
                    }
                    catch (ZnodeException exception)
                    {
                        ZnodeLogging.LogMessage(exception, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                        switch (exception.ErrorCode)
                        {
                            case ErrorCodes.NotOrderEligibleForReturn:
                                return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorOrderNotEligibleForReturn };
                            case ErrorCodes.NullModel:
                                return new RMAReturnViewModel() { HasError = true, ErrorMessage = ZnodeConstant.NullModelError };
                            default:
                                return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailedToCreate };
                        }
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailedToCreate };
                    }
                }
            }
            return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailedToCreate };
        }

        //Delete order return on the basis of return number
        public virtual bool DeleteOrderReturn(string returnNumber)
        {
            int userId = _userAgent.GetUserViewModelFromSession()?.UserId ?? 0;
            if (userId > 0)
            {
                return _rmaReturnClient.DeleteOrderReturn(returnNumber, userId);
            }
            else
            {
                return false;
            }
        }

        //Submit order return
        public virtual RMAReturnViewModel SubmitOrderReturn(RMAReturnViewModel returnViewModel)
        {

            if (IsNotNull(returnViewModel) && IsNotNull(returnViewModel.OrderNumber) && returnViewModel?.ReturnLineItems?.Count > 0 )
            {
                int userId = GetUserID(returnViewModel.OrderNumber);
                if (userId > 0)
                {
                    try
                    {
                        //Check the value of store level global attribute order return request is enable or disable for store
                        if (!GlobalAttributeHelper.EnableReturnRequest())
                            throw new ZnodeException(ErrorCodes.NotOrderEligibleForReturn, string.Empty);

                        returnViewModel.UserId = userId;
                        returnViewModel.PortalId = PortalAgent.CurrentPortal.PortalId;
                        returnViewModel = _rmaReturnClient.SubmitOrderReturn(returnViewModel.ToModel<RMAReturnModel>()).ToViewModel<RMAReturnViewModel>();
                        return new RMAReturnViewModel() { ErrorMessage = returnViewModel.HasError ? WebStore_Resources.ErrorSubmitReturn :WebStore_Resources.SuccessSubmitReturn, ReturnNumber = returnViewModel.ReturnNumber };
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
            }
            return new RMAReturnViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorSubmitReturn };
        }

        //Perform calculations for an order return line item.
        public virtual RMAReturnCalculateViewModel CalculateOrderReturn(RMAReturnCalculateViewModel returnCalculateViewModel)
        {
            int userId = GetUserID(returnCalculateViewModel.OrderNumber);
            if (IsNotNull(returnCalculateViewModel) && IsNotNull(returnCalculateViewModel.OrderNumber) && returnCalculateViewModel?.ReturnCalculateLineItemList?.Count > 0 && userId > 0)
            {
                returnCalculateViewModel.UserId = userId;
                returnCalculateViewModel.PortalId = PortalAgent.CurrentPortal.PortalId;
                try
                {
                    returnCalculateViewModel = _rmaReturnClient.CalculateOrderReturn(returnCalculateViewModel.ToModel<RMAReturnCalculateModel>()).ToViewModel<RMAReturnCalculateViewModel>();
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

        //Manage order return
        public virtual RMAReturnOrderDetailViewModel ManageOrderReturn(string returnNumber)
        {
            int userId = _userAgent.GetUserViewModelFromSession().UserId;
            if (userId > 0 && !string.IsNullOrEmpty(returnNumber))
            {
                RMAReturnViewModel returnModel = _rmaReturnClient.GetReturnDetails(returnNumber, SetExpandForManageReturn())?.ToViewModel<RMAReturnViewModel>();
                if (IsNotNull(returnModel))
                {
                    // Get order details by order number for return.
                    OrderModel orderModel = _rmaReturnClient.GetOrderDetailsForReturn(userId, returnModel.OrderNumber);
                    if (IsNotNull(orderModel))
                    {
                        RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel = new RMAReturnOrderDetailViewModel();
                        rmaReturnOrderDetailViewModel = GetReturnOrderDetail(orderModel);
                        rmaReturnOrderDetailViewModel.OrderNumber = returnModel.OrderNumber;
                        rmaReturnOrderDetailViewModel.ReturnNumber = returnNumber;
                        rmaReturnOrderDetailViewModel.ReturnNote = returnModel?.ReturnNotes?.FirstOrDefault()?.Notes;
                        if (rmaReturnOrderDetailViewModel?.ReturnCartViewModel?.ShoppingCartItems?.Count > 0)
                        {
                            BindOrderReturnDataToShoppingCart(returnModel, rmaReturnOrderDetailViewModel);
                            rmaReturnOrderDetailViewModel.ReturnCalculateViewModel = new RMAReturnCalculateViewModel()
                            {
                                ReturnSubTotal = returnModel.SubTotal,
                                ReturnTaxCost = returnModel.ReturnTaxCost,
                                ReturnShippingCost = 0,
                                ReturnTotal = returnModel.TotalReturnAmount,
                                CultureCode = returnModel.CultureCode,
                                ReturnImportDuty = returnModel.ReturnImportDuty
                            };
                        }
                        return rmaReturnOrderDetailViewModel;
                    }
                }
            }
            return new RMAReturnOrderDetailViewModel();
        }

        // method to validate the User Return.
        public virtual UserViewModel ValidateGuestUserReturn(string orderNumber)
        {
            if (!string.IsNullOrEmpty(orderNumber))
            {
                int userId = GetOrderUserIdFromCookie(orderNumber);
                int registeredUserId = _userAgent.GetUserViewModelFromSession()?.UserId ?? 0;
                if (userId > 0)
                {
                    IUserClient userClient = new UserClient();
                    UserModel userModel = userClient.GetUserAccountData(userId);
                    if(IsNotNull (userModel))
                    {
                        if (registeredUserId > 0 && userModel.UserId != registeredUserId)
                        {
                            return new UserViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorCreateGuestReturn };
                        }
                        else if (registeredUserId == 0 && !userModel.IsGuestUser)
                        {
                            return new UserViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorLoginToCreateReturn };
                        }
                        return userModel.ToViewModel<UserViewModel>();
                    }                   
                }
            }
            return new UserViewModel() { HasError = false };
        }

        #endregion

        #region Protected Methods

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

        //Set expand for Return Details.
        protected virtual ExpandCollection SetExpandForManageReturn()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.ReturnItemList);
            expands.Add(ExpandKeys.ReturnNotes);
            ZnodeLogging.LogMessage("Expand for return details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands });
            return expands;
        }

        //Bind order return data to shoppingCart
        protected virtual void BindOrderReturnDataToShoppingCart(RMAReturnViewModel returnModel, RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel)
        {
            foreach (RMAReturnCartItemViewModel cartItem in rmaReturnOrderDetailViewModel?.ReturnCartViewModel?.ShoppingCartItems)
            {
                RMAReturnLineItemViewModel returnLineItem = returnModel?.ReturnLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == cartItem.OmsOrderLineItemsId);
                if (IsNotNull(returnLineItem))
                {
                    cartItem.Quantity = cartItem.Quantity + returnLineItem.ExpectedReturnQuantity;
                    cartItem.ExpectedReturnQuantity = returnLineItem.ExpectedReturnQuantity;
                    cartItem.TotalLineItemPriceWithCurrency = HelperMethods.FormatPriceWithCurrency(returnLineItem.ExpectedReturnQuantity * returnLineItem.Price, cartItem.CultureCode);
                    cartItem.ReasonForReturnId = (int)returnLineItem.RmaReasonForReturnId;
                    cartItem.RmaReturnLineItemsId = returnLineItem.RmaReturnLineItemsId;
                    foreach (AssociatedProductModel associatedProduct in cartItem?.GroupProducts)
                    {
                        associatedProduct.Quantity = associatedProduct.Quantity + returnLineItem.ExpectedReturnQuantity;
                    }
                }
            }
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

        // Get Order User Id from cookie.
        public virtual int GetOrderUserIdFromCookie(string orderNumber)
        {
            int OrderUserId = 0;
            Int32.TryParse(CookieHelper.GetCookieValue<string>("OrderUserId_" + orderNumber), out OrderUserId);
            return OrderUserId;
        }

        // Get User Id.
        private int GetUserID(string orderNumber)
        {
            int userId = _userAgent.GetUserViewModelFromSession()?.UserId ?? 0;
            int orderUserId = 0;
            if (IsNotNull(orderNumber) && userId == 0)
                orderUserId = GetOrderUserIdFromCookie(orderNumber);
            userId = orderUserId > 0 ? orderUserId : userId;
            return userId;
        }
        #endregion
    }
}