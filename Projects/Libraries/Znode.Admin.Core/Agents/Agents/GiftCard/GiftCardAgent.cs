using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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
    public class GiftCardAgent : BaseAgent, IGiftCardAgent
    {
        #region Private Variables

        private readonly IGiftCardClient _giftCardClient;
        private readonly IPortalClient _portalClient;
        private readonly IUserClient _userClient;
        private readonly IRMARequestAgent _rmaRequestAgent;
        private readonly IRMARequestItemClient _rmaRequestItemClient;
        private readonly IRMARequestClient _rmaRequestClient;
        private readonly IAccountClient _accountClient;
        private readonly ICurrencyClient _currencyClient;
        private readonly IPortalUnitClient _portalUnitClient;
        #endregion

        #region Constructor

        public GiftCardAgent(IGiftCardClient giftCardClient, IPortalClient portalClient, IUserClient userClient, IRMARequestItemClient rmaRequestItemClient, IRMARequestClient rmaRequestClient,
            IAccountClient accountClient, ICurrencyClient currencyClient, IPortalUnitClient portalUnitClient)
        {
            _currencyClient = GetClient<ICurrencyClient>(currencyClient);
            _giftCardClient = GetClient<IGiftCardClient>(giftCardClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _userClient = GetClient<IUserClient>(userClient);
            _rmaRequestAgent = new RMARequestAgent(GetClient<RMAConfigurationClient>(), GetClient<RMARequestClient>(), GetClient<RMARequestItemClient>());
            _rmaRequestItemClient = GetClient<IRMARequestItemClient>(rmaRequestItemClient);
            _rmaRequestClient = GetClient<IRMARequestClient>(rmaRequestClient);
            _accountClient = GetClient<IAccountClient>(accountClient);
            _portalUnitClient = GetClient<IPortalUnitClient>(portalUnitClient);
        }

        #endregion

        #region public Methods
        //Gets the list of GiftCard.
        public virtual GiftCardListViewModel GetGiftCardList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isExcludeExpired, int userId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters, expands and sortCollection.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { expands, filters, sorts });
            //If exclude expired is true, add filter for expiration date.
            if (isExcludeExpired)
                SetFilterForExpiredGiftCard(filters);
            else
                filters.RemoveAll(x => string.Equals(x.FilterName, $"{ZnodeGiftCardEnum.ExpirationDate.ToString()}", StringComparison.CurrentCultureIgnoreCase));

            SetUserFilter(filters, userId);
            //Get the sort collection for gift card id desc.
            sorts = HelperMethods.SortDesc(ZnodeGiftCardEnum.GiftCardId.ToString(), sorts);

            GiftCardListModel giftCardList = _giftCardClient.GetGiftCardList(expands, filters, sorts, pageIndex, pageSize);
            GiftCardListViewModel listViewModel = new GiftCardListViewModel { GiftCardList = giftCardList?.GiftCardList?.ToViewModel<GiftCardViewModel>().ToList() };
            listViewModel.IsExcludeExpired = isExcludeExpired;
            if (userId > 0)
                listViewModel.UserId = userId;
            SetListPagingData(listViewModel, giftCardList);

            //Set the Tool Menus for Gift card List Grid View.
            SetGiftCardToolMenus(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return giftCardList?.GiftCardList?.Count > 0 ? listViewModel : new GiftCardListViewModel() { GiftCardList = new List<GiftCardViewModel>(), IsExcludeExpired = isExcludeExpired, UserId = userId };
        }

        //Create GiftCard.
        public virtual GiftCardViewModel Create(GiftCardViewModel giftCardViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                giftCardViewModel.RemainingAmount = giftCardViewModel?.GiftCardAmount.ToString();
                GiftCardModel giftCardModel = _giftCardClient.CreateGiftCard(giftCardViewModel?.ToModel<GiftCardModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return IsNotNull(giftCardModel) ? giftCardModel.ToViewModel<GiftCardViewModel>() : new GiftCardViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.InvalidData)
                    return (GiftCardViewModel)GetViewModelWithErrorMessage(giftCardViewModel, ex.Message);

                return (GiftCardViewModel)GetViewModelWithErrorMessage(giftCardViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (GiftCardViewModel)GetViewModelWithErrorMessage(giftCardViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get GiftCard by GiftCard id.
        public virtual GiftCardViewModel GetGiftCard(int giftCardId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                GiftCardViewModel giftCardViewModel = _giftCardClient.GetGiftCard(giftCardId).ToViewModel<GiftCardViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return giftCardViewModel;
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


        //Update GiftCard.
        public virtual GiftCardViewModel Update(GiftCardViewModel giftCardViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                GiftCardModel giftCardModel = _giftCardClient.UpdateGiftCard(giftCardViewModel.ToModel<GiftCardModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return IsNotNull(giftCardModel) ? giftCardModel.ToViewModel<GiftCardViewModel>() : (GiftCardViewModel)GetViewModelWithErrorMessage(new GiftCardViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (GiftCardViewModel)GetViewModelWithErrorMessage(new GiftCardViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete GiftCard.
        public virtual bool DeleteGiftCard(string giftCardId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return _giftCardClient.DeleteGiftCard(new ParameterModel { Ids = giftCardId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual string GetRandomGiftCardNumber() => _giftCardClient.GetRandomGiftCardNumber();

        //Check Check whether entered Customer id already exists or not.
        public virtual bool CheckIsUserIdExist(int userId, int portalId = 0)
        {
            if (userId > 0)
                return (_userClient.GetUserAccountData(userId, new ExpandCollection() { ZnodeConstant.IsGuestUserForGiftCard }, portalId)?.UserId).GetValueOrDefault() > 0;

            return false;
        }

        //Set RMA Data
        public virtual GiftCardViewModel SetRMAData(RMARequestViewModel rmaRequestViewModel)
        {
            GiftCardViewModel model = new GiftCardViewModel();
            model.RmaRequestModel = new RMARequestViewModel() { RMARequestID = rmaRequestViewModel.RMARequestID, OrderLineItems = rmaRequestViewModel.OrderLineItems, Quantities = rmaRequestViewModel.Quantities, OmsOrderDetailsId = rmaRequestViewModel.OmsOrderDetailsId, Total = rmaRequestViewModel.Total };
            model.GiftCardAmount = Convert.ToDecimal(rmaRequestViewModel.Total);
            model.PortalId = rmaRequestViewModel.PortalId;
            model.CurrencyCode = rmaRequestViewModel.CurrencyCode;
            model.SendMail = true;
            RMARequestItemListViewModel rmaRequestItemList = _rmaRequestAgent.GetRMARequestItemsForGiftCard(model.RmaRequestModel.OrderLineItems);

            BindRMAData(rmaRequestItemList, model);

            return model;
        }

        //Update RMA Request
        public virtual bool UpdateRMA(GiftCardViewModel model, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            message = string.Empty;
            RMARequestViewModel rmaRequestViewModel = _rmaRequestAgent.GetRMARequest(model.RmaRequestModel.RMARequestID);
            rmaRequestViewModel.RequestCode = ZnodeRmaRequestStatusEnum.Returned_Refunded.ToString();

            //Update RMA Request
            _rmaRequestAgent.UpdateRMARequest(model.RmaRequestModel.RMARequestID, rmaRequestViewModel);

            string[] lineitems = model.RmaRequestModel.OrderLineItems.Split(',');
            string[] qtyItems = model.RmaRequestModel.Quantities.Split(',');
            bool isRmaRequestItemAdded = true;

            for (int itemCount = 0; itemCount < lineitems.Length; itemCount++)
            {
                int orderLineItemId = Convert.ToInt32(lineitems[itemCount]);
                RMARequestItemViewModel rmaRequestitem = new RMARequestItemViewModel()
                {
                    RMArequestId = model.RmaRequestModel.RMARequestID,
                    Quantity = Convert.ToDecimal(qtyItems[itemCount]),
                    OmsOrderLineItemsId = orderLineItemId,
                    GiftCardId = model.GiftCardId,
                    IsReturnable = true,
                    IsReceived = true,
                    Price = null
                };
                isRmaRequestItemAdded &= this.AddRMARequestItem(rmaRequestitem);
            }
            try
            {
                _rmaRequestClient.IsStatusEmailSent(model.RmaRequestModel.RMARequestID);
                if (isRmaRequestItemAdded && model.SendMail)
                    _rmaRequestClient.IsGiftCardMailSent(model.ToModel<GiftCardModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                message = ex.ErrorMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                message = Admin_Resources.UpdateErrorMessage;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return isRmaRequestItemAdded;
        }

        //Add RMA Request Item
        public virtual bool AddRMARequestItem(RMARequestItemViewModel rmaRequestItem)
        {
            if (IsNotNull(rmaRequestItem))
            {
                rmaRequestItem = (_rmaRequestItemClient.CreateRMARequestItem((rmaRequestItem).ToModel<RMARequestItemModel>()).ToViewModel<RMARequestItemViewModel>());
                if (rmaRequestItem?.RMARequestItemID > 0)
                    return true;
            }
            return false;
        }

        //Get active currency.
        public virtual CurrencyViewModel GetActiveCurrency(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            if (portalId > 0)
            {
                PortalUnitViewModel portalUnitViewModel = _portalUnitClient.GetPortalUnit(portalId)?.ToViewModel<PortalUnitViewModel>();
                if (IsNotNull(portalUnitViewModel))
                    filters.Add(new FilterTuple(FilterKeys.CurrencyId, FilterOperators.Equals, portalUnitViewModel.CurrencyTypeID.ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeCurrencyEnum.IsDefault.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));

            //Get currency list.
            CurrencyViewModel currency = _currencyClient.GetCurrencyCultureList(null, filters, null, null, null)?.Currencies?.FirstOrDefault()?.ToViewModel<CurrencyViewModel>();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return IsNull(currency) ? new CurrencyViewModel() : currency;
        }

        //Get currency details by code.
        public virtual CurrencyViewModel GetCurrencyDetailsByCode(string currencyCode)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.CurrencyCode, FilterOperators.Contains, currencyCode));
            CurrencyViewModel currencyList = _currencyClient.GetCurrencyList(null, filters, null, null, null).Currencies.FirstOrDefault().ToViewModel<CurrencyViewModel>();
            return currencyList;
        }

        // Activate deactivate vouchers.
        public virtual bool ActivateDeactivateVouchers(string voucherIds, bool isActive)
        {
            bool status;
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                status = _giftCardClient.ActivateDeactivateVouchers(new ParameterModel { Ids = voucherIds }, isActive);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return false;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }

        // Get voucher history list. 
        public VoucherHistoryListViewModel GetVoucherHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int pageIndex, int pageSize, int voucherId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters, expands and sortCollection.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { expands, filters, sortCollection });
            //If exclude expired is true, add filter for expiration date.

            if (voucherId > 0 && portalId > 0)
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, $"{ZnodeGiftCardEnum.PortalId.ToString()}", StringComparison.CurrentCultureIgnoreCase));
                filters.Add($"{ZnodeGiftCardEnum.PortalId.ToString()}", FilterOperators.Equals, portalId.ToString());
                filters.RemoveAll(x => string.Equals(x.FilterName, $"{ZnodeGiftCardEnum.GiftCardId.ToString()}", StringComparison.CurrentCultureIgnoreCase));
                filters.Add($"{ZnodeGiftCardEnum.GiftCardId.ToString()}", FilterOperators.Equals, voucherId.ToString());
            }
            else
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, $"{ZnodeGiftCardEnum.GiftCardId.ToString()}", StringComparison.CurrentCultureIgnoreCase));
                filters.RemoveAll(x => string.Equals(x.FilterName, $"{ZnodeGiftCardEnum.PortalId.ToString()}", StringComparison.CurrentCultureIgnoreCase));
            }
                

            GiftCardHistoryListModel giftCardHistoryList = _giftCardClient.GetGiftCardHistoryList(expands, filters, sortCollection, pageIndex, pageSize);
            VoucherHistoryListViewModel listViewModel = new VoucherHistoryListViewModel { GiftCardHistoryList = giftCardHistoryList?.GiftCardHistoryList?.ToViewModel<VoucherHistoryViewModel>().ToList(), GiftCardId = voucherId, PortalId = portalId };
            
            SetListPagingData(listViewModel, giftCardHistoryList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listViewModel?.GiftCardHistoryList?.Count > 0 ? listViewModel : new VoucherHistoryListViewModel() { GiftCardHistoryList = new List<VoucherHistoryViewModel>() };

        }
        #endregion

        #region Private Methods.
        //Set the Tool Menus for Gift card List Grid View.
        private void SetGiftCardToolMenus(GiftCardListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                if (model.UserId <= 0)
                    model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('GiftCardDeletePopup')", ControllerName = "GiftCard", ActionName = "Delete" });

                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "EditableText.prototype.DialogDelete('ActivateVoucher')", ControllerName = "GiftCard", ActionName = "ActivateDeactivateVouchers" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "EditableText.prototype.DialogDelete('DeactivateVoucher')", ControllerName = "GiftCard", ActionName = "ActivateDeactivateVouchers" });
            }
        }

        //If exclude expired dropdown is selected on UI, add filter for expiration date to show only unexpired gift cards.
        private void SetFilterForExpiredGiftCard(FilterCollection filters)
        {
            filters.RemoveAll(x => string.Equals(x.FilterName, $"{ZnodeGiftCardEnum.ExpirationDate.ToString()}", System.StringComparison.CurrentCultureIgnoreCase));
            filters.Add($"{ZnodeGiftCardEnum.ExpirationDate.ToString()}", FilterOperators.GreaterThanOrEqual, System.DateTime.Today.ToShortDateString());
        }

        //Bind RMA Data for gift card
        private void BindRMAData(RMARequestItemListViewModel rmaRequestItemList, GiftCardViewModel giftCardModel)
        {
            if (rmaRequestItemList?.RMARequestItemList?.Count > 0)
            {
                int? accountId = rmaRequestItemList.RMARequestItemList[0].UserId;
                int giftCardExpirationPeriod = rmaRequestItemList.RMARequestItemList[0].GCExpirationPeriod.GetValueOrDefault();
                giftCardModel.ExpirationDate = DateTime.Now.AddDays(giftCardExpirationPeriod);
                giftCardModel.RestrictToCustomerAccount = true;
                giftCardModel.AccountId = accountId;
                giftCardModel.UserId = accountId;
                //generate gift card number
                giftCardModel.CardNumber = GetRandomGiftCardNumber();
            }
        }

        // Set User Id filter.
        protected virtual void SetUserFilter(FilterCollection filters, int userId)
        {
            filters.RemoveAll(x => string.Equals(x.FilterName, $"{ZnodeGiftCardEnum.UserId.ToString()}", StringComparison.CurrentCultureIgnoreCase));
            if (userId > 0)
            {               
                filters.Add($"{ZnodeGiftCardEnum.UserId.ToString()}", FilterOperators.Equals, userId.ToString());
            }
            
        }
        #endregion
    }
}