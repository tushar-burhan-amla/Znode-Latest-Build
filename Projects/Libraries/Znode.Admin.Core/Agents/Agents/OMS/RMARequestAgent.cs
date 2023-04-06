using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class RMARequestAgent : BaseAgent, IRMARequestAgent
    {
        #region Private Variable
        private readonly IRMAConfigurationClient _rmaConfigurationClient;
        private readonly IRMARequestClient _rmaRequestClient;
        private readonly IRMARequestItemClient _rmaRequestItemClient;
        private readonly IStoreAgent _storeAgent;
        #endregion

        #region Constant Variables
        private const string AppendFlag = "APPEND";
        private const string VoidMode = "void";
        private const string ViewFlag = "view";
        private const string EditFlag = "edit";
        private const string RMACreateFlag = "rmacreate";
        private const string RMA = "rma";
        private const string alphaNumericCharactersSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        #endregion

        #region Constructor
        public RMARequestAgent(IRMAConfigurationClient rmaConfigurationClient, IRMARequestClient rmaRequestClient, IRMARequestItemClient rmaRequestItemClient)
        {
            _rmaConfigurationClient = GetClient<IRMAConfigurationClient>(rmaConfigurationClient);
            _rmaRequestClient = GetClient<IRMARequestClient>(rmaRequestClient);
            _rmaRequestItemClient = GetClient<IRMARequestItemClient>(rmaRequestItemClient);
            _storeAgent = new StoreAgent(GetClient<PortalClient>(), GetClient<EcommerceCatalogClient>(), GetClient<ThemeClient>(), GetClient<DomainClient>(), GetClient<PriceClient>(), GetClient<OrderStateClient>(),
                GetClient<ProductReviewStateClient>(), GetClient<PortalProfileClient>(), GetClient<WarehouseClient>(), GetClient<CSSClient>(), GetClient<ManageMessageClient>(), GetClient<ContentPageClient>(), GetClient<TaxClassClient>(),
                GetClient<PaymentClient>(), GetClient<ShippingClient>(), GetClient<PortalCountryClient>(), GetClient<TagManagerClient>(), GetClient<GeneralSettingClient>());
        }
        #endregion

        #region Public Method

        //Check is RMA Enable or not.
        public virtual bool IsRMAEnable(int orderId, string orderState, string orderDate, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, orderState = orderState, orderDate = orderDate });
            bool enable = false;
            errorMessage = string.Empty;
            //To check order state
            if (Equals(orderState.ToLower(), Convert.ToString(ZnodeOrderStatusEnum.CANCELED).ToLower()) || Equals(orderState.ToLower(), Convert.ToString(ZnodeOrderStatusEnum.RETURNED).ToLower()))
                return false;

            //Check rma config
            RMAConfigurationModel rmaConfig = _rmaConfigurationClient.GetRMAConfiguration();
            int rmaPeriod = 0;
            if (IsNotNull(rmaConfig))
            {
                if (rmaConfig.MaxDays.HasValue)
                    rmaPeriod = rmaConfig.MaxDays.Value;

                DateTime orderdate = Convert.ToDateTime(orderDate).AddDays(rmaPeriod);
                if (orderdate.Date < DateTime.Now.Date)
                {
                    errorMessage = Admin_Resources.RmaDateExpiredErrorMessage;
                    return false;
                }
            }
            //To check rma orderflag
            if (!orderId.Equals(0))
            {
                enable = _rmaRequestClient.GetOrderRMAFlag(orderId);
                if (!enable)
                    errorMessage = Admin_Resources.RmaAlreadyAppliedErrorMessage;
            }
            return enable;
        }

        //Get RMA request list item list.
        public virtual RMARequestItemListViewModel GetRMARequestListItem(int omsOrderDetailsId, int rmaId, string flag, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsOrderDetailsId = omsOrderDetailsId, rmaId = rmaId, flag = flag, portalId = portalId });
            CreateFiltersForRMARequestItem(omsOrderDetailsId, rmaId, flag);

            RMARequestItemListModel rmaRequestItemList = _rmaRequestItemClient.GetRMARequestItemList(Expands, Filters, new SortCollection(), null, null);
            RMARequestItemListViewModel rmaRequestItemListViewModel = new RMARequestItemListViewModel { RMARequestItemList = rmaRequestItemList?.RMARequestItemList?.ToViewModel<RMARequestItemViewModel>()?.ToList() };

            rmaRequestItemListViewModel.ReasonForReturnItems = BindReasonForReturn();

            if (rmaRequestItemListViewModel?.RMARequestItemList?.Count > 0)
                return SetRMARequestItemData(omsOrderDetailsId, flag, rmaRequestItemListViewModel, portalId);

            return new RMARequestItemListViewModel();
        }

        //Generate Request Number.
        public virtual string GenerateRequestNumber(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //This method will generate RMA Request number in following format: [store id]-[year]-[alphaCharacters]-[alphaNumericCharacters]: . 
            int alphaCharactersLength = 3;
            int alphaNumericCharactersLength = 5;
            ZnodeLogging.LogMessage("PortalId to generate request number:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, portalId);

            Random random = new Random();
            string alphaCharacters = new string(
                Enumerable.Repeat(alphaNumericCharactersSet, alphaCharactersLength)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            string alphaNumericCharacters = new string(
                 Enumerable.Repeat(alphaNumericCharactersSet, alphaNumericCharactersLength)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return string.Concat(portalId.ToString(), "-", DateTime.Now.ToString("yy"), "-", alphaCharacters, "-", alphaNumericCharacters);
        }

        //Bind Reason For Return
        public virtual List<SelectListItem> BindReasonForReturn()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, FilterKeys.ActiveTrueValue));

            SortCollection sort = new SortCollection();
            sort.Add(SortKeys.Name, SortDirections.Ascending);

            return ToSelectListItems(_rmaConfigurationClient.GetReasonForReturnList(null, filters, sort, null, null).RequestStatusList);
        }

        //Creates a SelectListItem List for ReasonForReturn.
        private List<SelectListItem> ToSelectListItems(IEnumerable<RequestStatusModel> model)
        {
            List<SelectListItem> rmaReasonItems = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                rmaReasonItems = (from item in model
                                  select new SelectListItem
                                  {
                                      Text = item.Name,
                                      Value = item.RmaReasonForReturnId.ToString(),
                                  }).ToList();
            }
            ZnodeLogging.LogMessage("SelectListItems count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaReasonItems?.Count);
            return rmaReasonItems;
        }

        //Bind RMARequestList
        public virtual RMARequestListViewModel GetRMARequestList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(sortCollection) || sortCollection.Count < 1)
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodeRmaRequestEnum.RmaRequestId.ToString(), DynamicGridConstants.DESCKey);
            }

            RMARequestListModel rmaRequestListModel = _rmaRequestClient.GetRMARequest(expands, filters, sortCollection, pageIndex, pageSize);

            RMARequestListViewModel listViewModel = new RMARequestListViewModel { RMARequestList = rmaRequestListModel?.RMARequests.ToViewModel<RMARequestViewModel>().ToList() };
            SetListPagingData(listViewModel, rmaRequestListModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return rmaRequestListModel?.RMARequests?.Count > 0 ? listViewModel : new RMARequestListViewModel() { RMARequestList = new List<RMARequestViewModel>() };
        }

        //Get RMARequest Gift Card Details
        public virtual void RMARequestGiftCardDetails(RMARequestItemListViewModel rmaRequestItems, int rmaRequestId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            IssuedGiftCardListModel giftCardDetails = _rmaRequestClient.GetRMARequestGiftCardDetails(rmaRequestId);
            rmaRequestItems.GiftCardsIssued = IsNotNull(giftCardDetails) ? giftCardDetails.ToViewModel<IssuedGiftCardListViewModel>() : new IssuedGiftCardListViewModel();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get RMARequest by rmaRequestId
        public virtual RMARequestViewModel GetRMARequest(int rmaRequestId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (rmaRequestId > 0)
            {
                RMARequestModel rmaRequestModel = _rmaRequestClient.GetRMARequest(rmaRequestId);
                return IsNotNull(rmaRequestModel) ? rmaRequestModel.ToViewModel<RMARequestViewModel>() : new RMARequestViewModel();
            }
            return new RMARequestViewModel();
        }

        //Get RMA request item model.
        public virtual RMARequestItemListViewModel GetRMARequestItemListViewModel(RMARequestParameterViewModel rmaRequestParamModel, string flag)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            RMARequestItemListViewModel rmaItemList = new RMARequestItemListViewModel();
            RMARequestViewModel rmaRequest = GetRMARequest(rmaRequestParamModel.RMARequestID);

            //Get RMA Request List Items.
            rmaItemList = GetRMARequestListItem(rmaRequestParamModel.OmsOrderDetailsId, rmaRequestParamModel.RMARequestID, flag, rmaRequestParamModel.PortalId);

            if (rmaItemList.RMARequestItemList?.Count > 0)
            {
                rmaItemList.OmsOrderDetailsId = rmaRequestParamModel.OmsOrderDetailsId;
                rmaItemList.RequestDate = string.IsNullOrEmpty(flag) ? rmaItemList.RequestDate : rmaRequestParamModel.RequestDate;
                rmaItemList.RMARequestId = rmaRequestParamModel.RMARequestID;
                rmaItemList.Comments = rmaRequest?.Comments;
                rmaItemList.CustomerName = rmaRequestParamModel.customerName;
                rmaItemList.flag = flag;
                rmaItemList.RequestNumber = string.IsNullOrEmpty(flag) ? rmaItemList.RequestNumber : rmaRequestParamModel.requestNumber;
                rmaItemList.RequestCode = rmaRequest?.RequestCode;
                rmaItemList.PortalId = rmaRequestParamModel.PortalId;
                rmaItemList.OMSOrderId = rmaRequestParamModel.OMSOrderId;
                //Get RMA Request Gift Card Details.
                RMARequestGiftCardDetails(rmaItemList, rmaRequestParamModel.RMARequestID);

                // Generate View for RMA on flag based.
                rmaItemList.ViewHtml = GenerateView(rmaItemList, flag);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return rmaItemList;
        }

        //Create RMA Request.
        public virtual bool CreateRMARequest(RMARequestViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                model.RequestCode = ZnodeRmaRequestStatusEnum.Authorized.ToString();
                RMARequestModel request = _rmaRequestClient.CreateRMARequest(model.ToModel<RMARequestModel>());
                return request?.RmaRequestId > 0;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return false;
        }

        //Get RMA Request Items For Gift Card by orderLineItems
        public virtual RMARequestItemListViewModel GetRMARequestItemsForGiftCard(string orderLineItems)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(orderLineItems))
            {
                RMARequestItemListModel rmaRequestItemListModel = _rmaRequestItemClient.GetRMARequestItemsForGiftCard(orderLineItems);
                return IsNotNull(rmaRequestItemListModel) ? rmaRequestItemListModel.ToViewModel<RMARequestItemListViewModel>() : new RMARequestItemListViewModel();
            }
            return new RMARequestItemListViewModel();
        }

        //Update RMA Request
        public virtual bool UpdateRMARequest(int rmaRequestId, RMARequestViewModel rmaRequest)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(rmaRequest))
                rmaRequest = (_rmaRequestClient.UpdateRMARequest(rmaRequestId, rmaRequest.ToModel<RMARequestModel>()).ToViewModel<RMARequestViewModel>());

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return rmaRequest?.RMARequestID > 0;
        }

        #endregion

        #region Private Methods

        //Creates filter for RMA Request item.
        private void CreateFiltersForRMARequestItem(int omsOrderDetailsId, int rmaId, string flag)
        {
            Filters = new FilterCollection();
            Filters.Add(new FilterTuple(FilterKeys.OmsOrderDetailsId, FilterOperators.Equals, omsOrderDetailsId.ToString()));
            Filters.Add(new FilterTuple(FilterKeys.RMAId, FilterOperators.Equals, rmaId.ToString()));
            Filters.Add(new FilterTuple(FilterKeys.IsReturnable, FilterOperators.Equals, FilterKeys.ActiveFalse));
            Filters.Add(new FilterTuple(FilterKeys.Flag, FilterOperators.Equals, flag));
        }

        //Sets the max quantity of an RMA request item.
        private static void SetMaxQuantity(RMARequestItemViewModel rmaRequestItem, out decimal maxQuantity, out decimal rmaMaxQuantity, out decimal rmaQuantity)
        {
            maxQuantity = 0;
            rmaMaxQuantity = Convert.ToDecimal(rmaRequestItem.RMAMaxQuantity);
            rmaQuantity = rmaRequestItem.RMAQuantity;

            rmaRequestItem.IsReturnableCheckboxEnabled = true;
            rmaRequestItem.QuantityDropDownEnabled = true;
        }

        //Generate RMA View according to mode like edit,view  and create rma.
        private MvcHtmlString GenerateView(RMARequestItemListViewModel rmaRequestItemListViewModel, string flag)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            StringBuilder strHtml = new StringBuilder();
            int rowCount = 1;
            strHtml.Append("<div class='col-sm-12 list-container'>");
            strHtml.Append(GenerateRMAHeader(rmaRequestItemListViewModel));
            strHtml.Append("<table class='table table-bordered attribut-table' id='rmaGrid'> <thead><tr>");
            strHtml.Append("<th>" + Admin_Resources.LabelSr + "</th>" + (!flag.Equals(ViewFlag) ? "<th scope='col' class='grid-checkbox'><input name = 'chkall' id='chkall' type = 'Checkbox' onchange='RMAManager.prototype.SelectAll()' class='header-check-all' /> <span class='lbl padding-8'></span>" : null) + " </th><th>" + Admin_Resources.LabelLineItemID + " </th> <th>Product Name</th> <th>" + Admin_Resources.LabelDescription + "</th><th>" + Admin_Resources.LabelSKU + " </th><th>" + Admin_Resources.LabelQuantity + " </th> <th>" + Admin_Resources.LabelReasonForReturn + " </th> <th>" + Admin_Resources.LabelPrice + "</th> <th>" + Admin_Resources.LabelTotal + " </th>");
            strHtml.Append("</tr></thead>");
            if (rmaRequestItemListViewModel?.RMARequestItemList?.Count() > 0)
            {
                foreach (var item in rmaRequestItemListViewModel?.RMARequestItemList)
                {
                    strHtml.Append("<tr>");
                    strHtml.Append("<td>" + rowCount + "</td> ");
                    strHtml.Append(!flag.Equals(ViewFlag) ? "<td class='grid-checkbox' ><input class='grid-row-checkbox'  " + (item.IsReturnable.GetValueOrDefault() ? "checked='checked'" : "") + " id = " + item.RowCount + " name = 'IsReturnable' type = 'checkbox' value = 'true' class='chkboxstyle' onchange='RMAManager.prototype.CalculateSubTotal();' " + (item.IsReturnableCheckboxEnabled ? "" : "disabled=disabled") + "/> <span class='lbl padding-8'></span>" : null + "</td>");
                    strHtml.Append("<td class='OmsOrderLineItemsId'> " + item.OmsOrderLineItemsId + " </td>");
                    strHtml.Append("<td> " + item.ProductName + " </td>");
                    strHtml.Append("<td> " + item.Description + " </td>");
                    strHtml.Append("<td class='Sku'> " + item.SKU + " </td>");
                    strHtml.Append("<td> " + (flag.Equals(ViewFlag) ? Convert.ToString(item.RMAMaxQuantity?.ToInventoryRoundOff()) : GenerateQuantityDropdownList(item.RmaQuantitySelectList, item.Quantity)?.ToString()) + " </td>");
                    strHtml.Append("<td> " + (string.IsNullOrEmpty(flag) ? GenerateReasonForReturnDropdownList(rmaRequestItemListViewModel?.ReasonForReturnItems, item.RmaReasonForReturnId)?.ToString() : Convert.ToString(item.ReasonforReturn)) + " </td>");
                    strHtml.Append("<td class='BasePrice' style='display:none'> " + item.Price + " </td>");
                    strHtml.Append("<td class='UnitPrice'> " + item.UnitPriceWithCurrencySymbol + " </td>");
                    strHtml.Append("<td class='Total'> " + item.PriceWithCurrencySymbol + " </td>");
                    strHtml.Append("<td class='Tax' style='display:none'> " + item.SalesTax + " </td>");
                    strHtml.Append("<td class='CurrencyCode' style='display:none'> " + item.CurrencyCode + " </td>");
                    strHtml.Append("</tr>");
                    rowCount++;
                }
            }
            strHtml.Append("</table> ");
            strHtml.Append("</div>");
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return MvcHtmlString.Create(strHtml.ToString());
        }

        //Generate table header.
        private StringBuilder GenerateRMAHeader(RMARequestItemListViewModel rmaRequestItemListViewModel)
        {
            StringBuilder strHtml = new StringBuilder();
            strHtml.Append("<div class='col-sm-12 nopadding'>");
            strHtml.Append("<h3 class='section-heading'>Request Number: " + rmaRequestItemListViewModel?.RequestNumber + "</h1>");
            strHtml.Append("<p><div id='orderid'>Order Number: " + rmaRequestItemListViewModel?.OrderNumber + "</div></p>");
            strHtml.Append("<p> Request Date: " + rmaRequestItemListViewModel?.RequestDate.ToDateTimeFormat() + "</p>");
            strHtml.Append("</div>");
            return strHtml;
        }

        //Generate dropdown for quantity.
        private StringBuilder GenerateQuantityDropdownList(IEnumerable<SelectListItem> rmaRequestItems, decimal? quantity)
        {
            StringBuilder strHtml = new StringBuilder();
            strHtml.Append("<select name='ddlRmaQuantity'> ");
            foreach (var item in rmaRequestItems)
                strHtml.Append(quantity == Convert.ToDecimal(item.Value) ? "<option selected value=" + item.Value + ">" + item.Text + "</option>" : "<option selected value=" + item.Value + ">" + item.Text + "</option>");

            strHtml.Append("</select> ");
            return strHtml;
        }

        //Generate dropdown for reasonforreturn.
        private StringBuilder GenerateReasonForReturnDropdownList(List<SelectListItem> rmaRequestItems, int? reasonForReturnId)
        {
            StringBuilder strHtml = new StringBuilder();
            strHtml.Append("<select name='ddlReasonForReturn'> ");

            foreach (var item in rmaRequestItems)
                strHtml.Append(reasonForReturnId == Convert.ToInt32(item.Value) ? "<option value=" + item.Value + ">" + item.Text + "</option>" : "<option value=" + item.Value + ">" + item.Text + "</option>");

            strHtml.Append("</select>");
            return strHtml;
        }

        //Creates Quantity Drop down for RMA Request Items.
        private static List<SelectListItem> CreateQuantityDropDown(decimal maxQuantity)
        {
            List<SelectListItem> rmaQuantityList = new List<SelectListItem>();
            //Checks the number is completely divisible or not
            if ((maxQuantity % 1) == 0)
                for (int count = 0; count < maxQuantity; count++)
                    rmaQuantityList.Add(new SelectListItem { Text = (count + 1).ToString(), Value = (count + 1).ToString(), Selected = Equals(Convert.ToInt32(maxQuantity), (count + 1)) });
            else
                rmaQuantityList.Add(new SelectListItem { Text = Math.Round(Convert.ToDouble(maxQuantity), 6).ToString(), Value = Math.Round(Convert.ToDouble(maxQuantity), 6).ToString(), Selected = true });

            ZnodeLogging.LogMessage("rmaQuantityList count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaQuantityList?.Count);
            return rmaQuantityList;
        }

        //Set RMA Request Item Data.
        private RMARequestItemListViewModel SetRMARequestItemData(int omsOrderDetailsId, string flag, RMARequestItemListViewModel rmaRequestItemListViewModel, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int rowCount = 0;
            foreach (RMARequestItemViewModel rmaRequestItem in rmaRequestItemListViewModel.RMARequestItemList)
            {
                decimal maxQuantity, rmaMaxQuantity;
                decimal rmaQuantity;

                SetMaxQuantity(rmaRequestItem, out maxQuantity, out rmaMaxQuantity, out rmaQuantity);

                maxQuantity = (string.IsNullOrEmpty(flag)) ? Convert.ToDecimal((rmaRequestItem.Quantity.GetValueOrDefault() - rmaMaxQuantity)) : Convert.ToDecimal((rmaMaxQuantity - rmaQuantity));

                if (maxQuantity == 0 && flag.Equals(AppendFlag))
                {
                    maxQuantity = rmaMaxQuantity;
                    rmaRequestItem.QuantityDropDownEnabled = false;
                    rmaRequestItem.IsReturnable = true;
                    rmaRequestItem.IsReturnableCheckboxEnabled = false;
                }

                //Create Quantity DropDown.
                rmaRequestItem.RmaQuantitySelectList = CreateQuantityDropDown(maxQuantity);
                rmaRequestItem.RmaQuantitySelect = Convert.ToInt32(maxQuantity);
                rmaRequestItem.RowCount = rowCount;
                rowCount++;

                rmaRequestItem.ReasonForReturnList = rmaRequestItemListViewModel.ReasonForReturnItems;
                rmaRequestItem.UnitPriceWithCurrencySymbol = HelperMethods.FormatPriceWithCurrency(rmaRequestItem.Price, rmaRequestItem.CultureCode);
                rmaRequestItem.PriceWithCurrencySymbol = HelperMethods.FormatPriceWithCurrency(rmaRequestItem.Price * (flag.Equals(ViewFlag) ? rmaRequestItem.RMAMaxQuantity.GetValueOrDefault() : maxQuantity), rmaRequestItem.CultureCode);
            }
            rmaRequestItemListViewModel.Total = rmaRequestItemListViewModel.SubTotal + rmaRequestItemListViewModel.Tax;

            rmaRequestItemListViewModel.OmsOrderDetailsId = omsOrderDetailsId;
            rmaRequestItemListViewModel.RequestDate = Convert.ToString(DateTime.Now);

            //Generate Request Number.
            rmaRequestItemListViewModel.RequestNumber = GenerateRequestNumber(portalId);
            rmaRequestItemListViewModel.CurrencyCode = rmaRequestItemListViewModel?.RMARequestItemList?.FirstOrDefault()?.CurrencyCode;
            rmaRequestItemListViewModel.OrderNumber = rmaRequestItemListViewModel?.RMARequestItemList?.FirstOrDefault()?.OrderNumber;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return rmaRequestItemListViewModel;
        }
        #endregion
    }
}