using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class RMAReturnService : BaseService, IRMAReturnService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeRmaReturnDetail> _returnDetailsRepository;
        private readonly IZnodeRepository<ZnodeRmaReturnLineItem> _returnLineItemsRepository;
        private readonly IZnodeRepository<ZnodeRmaReturnState> _returnStateRepository;
        private readonly IZnodeRepository<ZnodeRmaReturnNote> _returnNoteRepository;
        private readonly IEmailTemplateSharedService _emailTemplateSharedService;
        private readonly IZnodeRepository<ZnodeTaxRule> _taxRule;
        #endregion

        #region Constructor
        public RMAReturnService()
        {
            _returnDetailsRepository = new ZnodeRepository<ZnodeRmaReturnDetail>();
            _returnLineItemsRepository = new ZnodeRepository<ZnodeRmaReturnLineItem>();
            _returnStateRepository = new ZnodeRepository<ZnodeRmaReturnState>();
            _returnNoteRepository = new ZnodeRepository<ZnodeRmaReturnNote>();
            _emailTemplateSharedService = GetService<IEmailTemplateSharedService>();
            _taxRule = new ZnodeRepository<ZnodeTaxRule>();

        }
        #endregion

        #region Public Methods
        //Get Return Eligible Order Number List.
        public virtual RMAReturnOrderNumberListModel GetEligibleOrderNumberListForReturn(int userId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            RMAReturnOrderNumberListModel rmaReturnOrderNumberList = new RMAReturnOrderNumberListModel();
            rmaReturnOrderNumberList.ReturnEligibleOrderNumberList = GetReturnEligibleOrderNumberList(userId, portalId, string.Empty)?.ToList();
            return rmaReturnOrderNumberList;
        }

        //Check Is Order Eligible for Return Items
        public virtual bool IsOrderEligibleForReturn(int userId, int portalId, string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
                return false;

            return GetReturnEligibleOrderNumberList(userId, portalId, orderNumber)?.ToList()?.Count > 0 ? true : false;
        }

        //Get order details by order number for return.
        public virtual OrderModel GetOrderDetailsForReturn(int userId, string orderNumber, bool isFromAdmin = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(orderNumber) || Equals(userId, 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorOrderNumberNull);

            OrderModel orderModel = GetOrderDetails(userId, orderNumber);
            orderModel.IsFromAdminOrder = isFromAdmin;

            if (orderModel == null)
                return null;

            if (IsNotNull(orderModel?.OrderLineItems) && orderModel.OrderLineItems.Count > 0)
                orderModel.OrderLineItems = GetService<IZnodeOrderHelper>()?.FormalizeOrderLineItems(orderModel);

            ValidOrderLineItemsForReturn(orderModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderModel;
        }

        //Get return list.
        public virtual RMAReturnListModel GetReturnList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Replace sort key name.
            ReplaceSortKeys(ref sorts);

            //Add date time value in filter collection against filter column name Order date.
            filters = ServiceHelper.AddDateTimeValueInFilterByName(filters, ZnodeRmaReturnDetailEnum.ReturnDate.ToString().ToLower());

            bool isAdminRequest = Convert.ToBoolean(filters?.Find(x => string.Equals(x.FilterName, Libraries.ECommerce.Utilities.FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase))?.Item3);
            string returnDateRange = string.Empty;

            if (isAdminRequest)
            {
                BindUserPortalFilter(ref filters);
                returnDateRange = GetReturnDateRangeFromFilters(filters);
            }
            RemoveFilter(filters, Libraries.ECommerce.Utilities.FilterKeys.IsFromAdmin);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IList<RMAReturnModel> returnList = GetReturnList(pageListModel, isAdminRequest, returnDateRange);
            ZnodeLogging.LogMessage("Return list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, returnList?.Count);
            RMAReturnListModel returnListModel = new RMAReturnListModel() { Returns = returnList?.ToList() };
            returnListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return returnListModel;
        }

        //Gets order return details by return number
        public virtual RMAReturnModel GetReturnDetails(string rmaReturnNumber, NameValueCollection expands = null)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(rmaReturnNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorReturnNumberNull);

            RMAReturnModel returnModel = _returnDetailsRepository.Table.FirstOrDefault(x => x.ReturnNumber.ToLower() == rmaReturnNumber.ToLower())?.ToModel<RMAReturnModel>();
            //Get expand details
            bool isReturnItemList = !string.IsNullOrEmpty(expands.Get(ExpandKeys.ReturnItemList));
            bool isReturnShippingDetails = !string.IsNullOrEmpty(expands.Get(ExpandKeys.ReturnShippingDetails));
            bool isReturnNotes = !string.IsNullOrEmpty(expands.Get(ExpandKeys.ReturnNotes));
            bool isReturnProductImages = !string.IsNullOrEmpty(expands.Get(ExpandKeys.ReturnProductImages));
            bool isReturnBarcode = !string.IsNullOrEmpty(expands.Get(ExpandKeys.ReturnBarcode));

            if (IsNotNull(returnModel))
            {
                returnModel.ReturnStatus = Enum.GetName(typeof(ZnodeReturnStateEnum), returnModel.RmaReturnStateId)?.Replace('_', ' ')?.ToLower()?.ToProperCase();
                returnModel.StoreLogo = GetStoreLogoPath(returnModel.PortalId);
                returnModel.StoreName = (from portalRepository in new ZnodeRepository<ZnodePortal>().Table.Where(x => x.PortalId == returnModel.PortalId) select portalRepository.StoreName)?.FirstOrDefault();
                if (isReturnItemList)
                {
                    returnModel.ReturnLineItems = GetReturnLineItems(returnModel.RmaReturnDetailsId, true, IsReturnProcessComplete(returnModel.RmaReturnStateId));

                    if (returnModel?.ReturnLineItems?.Count > 0 && isReturnProductImages)
                        returnModel.ReturnLineItems = GetProductImagePath(returnModel.ReturnLineItems, returnModel.PortalId);

                    ZnodeLogging.LogMessage("Return line item list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, returnModel.ReturnLineItems?.Count);
                }
                if (isReturnShippingDetails)
                {
                    WarehouseModel warehouseDetails = GetReturnWarehouseDetails(returnModel.PortalId);
                    returnModel.ShippingToAddressHtml = GetReturnShippingAddressHTML(warehouseDetails?.Address, warehouseDetails?.WarehouseName);

                    AddressModel customerAddress = GetCustomerAddressDetails(returnModel.AddressId);
                    returnModel.ShippingFromAddressHtml = GetReturnShippingAddressHTML(customerAddress, customerAddress?.FirstName + " " + customerAddress?.LastName);
                }
                if (isReturnNotes)
                    returnModel.ReturnNotes = GetReturnNotes(returnModel);

                if (isReturnBarcode)
                    returnModel.BarcodeImage = GetBarcodeImage(returnModel.ReturnNumber);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return returnModel;
        }

        //Insert or update order return details.
        public virtual RMAReturnModel SaveOrderReturn(RMAReturnModel returnModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNull(returnModel) || IsNull(returnModel?.ReturnLineItems) || returnModel?.ReturnLineItems?.Count == 0)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);
            if (returnModel.PortalId < 1)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.PortalIdNotLessThanOne);
            if (returnModel.UserId < 1)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.ErrorUserIdLessThanOne);
            if (string.IsNullOrEmpty(returnModel?.OrderNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorOrderNumberEmptyOrNull);

            if (returnModel.IsAdminRequest)
            {
                if (returnModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.REFUND_PROCESSED)
                    return UpdateOrderReturnForRefundProcessed(returnModel);
                else
                    return UpdateOrderReturnForAdmin(returnModel);
            }

            if (!IsOrderEligibleForReturn(returnModel.UserId, returnModel.PortalId, returnModel.OrderNumber))
                throw new ZnodeException(ErrorCodes.NotOrderEligibleForReturn, Admin_Resources.ErrorOrderNotEligibleForReturn);

            ZnodeLogging.LogMessage("Input parameter RmaReturnDetailsId of returnModel: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { RmaReturnDetailsId = returnModel?.RmaReturnDetailsId });

            returnModel.ActionMode = string.IsNullOrEmpty(returnModel.ReturnNumber) ? ZnodeConstant.Create : ZnodeConstant.Edit;

            if (ValidateOrderReturn(returnModel?.ReturnLineItems, returnModel.ActionMode))
            {
                return string.IsNullOrEmpty(returnModel.ReturnNumber) ? InsertOrderReturn(returnModel) : UpdateOrderReturn(returnModel);
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.InvalidData, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);
            }
        }

        //To generate unique return number on basis of current date.
        public virtual string GenerateReturnNumber(RMAReturnModel rmaReturnModel, ParameterModel portalId = null)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            var _erpInc = new ERPInitializer<RMAReturnModel>(rmaReturnModel, "GetReturnNumber");
            if (string.IsNullOrEmpty(Convert.ToString(_erpInc.Result)))
            {
                string returnNumber = "RO";
                string storeName = ZnodeConfigManager.SiteConfig.StoreName;
                if (!string.IsNullOrEmpty(storeName))
                    returnNumber += storeName.Trim().Length > 2 ? storeName.Substring(0, 2) : storeName.Substring(0, 1);

                DateTime date = GetDateWithTime();
                // we have removed '-fff' from the date string as order number field length not exceeds the limit.
                // This change in made for the ticket ZPD-13806
                String strDate = date.ToString("yyMMdd-HHmmss");
                string randomSuffix = GetRandomCharacters();
                returnNumber += $"-{strDate}-{randomSuffix}";
                return returnNumber.ToUpper();
            }
            else
                return Convert.ToString(_erpInc?.Result);
        }

        //Delete order return
        public virtual bool DeleteOrderReturn(int returnDetailsId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (Equals(returnDetailsId, 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorFailedToDelete);

            FilterCollection filters = new FilterCollection { new FilterTuple("RmaReturnDetailsId", ProcedureFilterOperators.Equals, returnDetailsId.ToString()) };

            EntityWhereClauseModel whereClauseForDelete = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated for delete: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, whereClauseForDelete?.WhereClause);
            _returnNoteRepository.Delete(whereClauseForDelete.WhereClause, whereClauseForDelete.FilterValues);
            _returnLineItemsRepository.Delete(whereClauseForDelete.WhereClause, whereClauseForDelete.FilterValues);
            return _returnDetailsRepository.Delete(whereClauseForDelete.WhereClause, whereClauseForDelete.FilterValues);
        }

        //Delete order return on the basis of return number.
        public virtual bool DeleteOrderReturnByReturnNumber(string rmaReturnNumber, int userId)
        {
            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(rmaReturnNumber) || Equals(userId, 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorFailedToDelete);

            int rmaReturnDetailsId = _returnDetailsRepository.Table.FirstOrDefault(x => x.ReturnNumber.ToLower() == rmaReturnNumber.ToLower() && x.UserId == userId)?.RmaReturnDetailsId ?? 0;

            if (rmaReturnDetailsId > 0)
                return DeleteOrderReturn(rmaReturnDetailsId);

            return false;
        }

        //Perform calculations for a order return line item.
        public virtual RMAReturnCalculateModel CalculateOrderReturn(RMAReturnCalculateModel returnCalculateModel)
        {
            if (IsNull(returnCalculateModel) || IsNull(returnCalculateModel?.ReturnCalculateLineItemList) || returnCalculateModel?.ReturnCalculateLineItemList?.Count == 0)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);
            if (returnCalculateModel.PortalId < 1)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.PortalIdNotLessThanOne);
            if (returnCalculateModel.UserId < 1)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.ErrorUserIdLessThanOne);
            if (string.IsNullOrEmpty(returnCalculateModel?.OrderNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorReturnNumberNull);


            if (ValidateCalculationOrderReturn(returnCalculateModel))
            {

                int omsOrderId = GetOrderDetailsByOrderNumber(returnCalculateModel.OrderNumber, returnCalculateModel.UserId)?.OmsOrderId ?? 0;
                PerformCalculationForOrderReturnLineItem(returnCalculateModel, GetShoppingCartDetails(returnCalculateModel.UserId, returnCalculateModel.PortalId, omsOrderId));
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.InvalidData, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            return returnCalculateModel;
        }

        //Get List of Return States
        public virtual RMAReturnStateListModel GetReturnStatusList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IList<ZnodeRmaReturnState> returnStatusList = _returnStateRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("returnStatusList count", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { returnStatusList?.Count });

            RMAReturnStateListModel rmaReturnStateListModel = new RMAReturnStateListModel();
            rmaReturnStateListModel.ReturnStates = returnStatusList?.Count > 0 ? returnStatusList.ToModel<RMAReturnStateModel>().ToList() : new List<RMAReturnStateModel>();

            rmaReturnStateListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return rmaReturnStateListModel;
        }

        //Gets order return details from admin by return number
        public virtual RMAReturnModel GetReturnDetailsForAdmin(string returnNumber)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //This method check the access of manage screen for sales rep user
            ValidateAccessForSalesRepUser(ZnodeConstant.Return, 0, returnNumber);
            if (string.IsNullOrEmpty(returnNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorReturnNumberNull);

            RMAReturnModel returnModel = _returnDetailsRepository.Table.FirstOrDefault(x => x.ReturnNumber.ToLower() == returnNumber.ToLower())?.ToModel<RMAReturnModel>();

            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.ErrorReturnNumberNotFound);

            bool isReturnProcessComplete = IsReturnProcessComplete(returnModel.RmaReturnStateId);
            returnModel.ReturnStatus = _returnStateRepository.GetById(returnModel.RmaReturnStateId)?.ReturnStateName;
            if (returnModel.CreatedBy == 0)
                returnModel.CreatedByName = returnModel.EmailId;
            else
                returnModel.CreatedByName = returnModel.CreatedBy == returnModel.UserId ? returnModel.EmailId : new ZnodeRepository<ZnodeUser>().Table.FirstOrDefault(x => x.UserId == returnModel.CreatedBy)?.Email;
            returnModel.StoreName = (from portalRepository in new ZnodeRepository<ZnodePortal>().Table.Where(x => x.PortalId == returnModel.PortalId) select portalRepository.StoreName)?.FirstOrDefault();
            returnModel.ReturnLineItems = GetReturnLineItems(returnModel.RmaReturnDetailsId, false, isReturnProcessComplete);
            if (isReturnProcessComplete)
                returnModel.ReturnLineItems = GetProductImagePath(returnModel.ReturnLineItems, returnModel.PortalId);
            else
                returnModel.OldReturnLineItems = returnModel.ReturnLineItems;
            returnModel.ReturnHistoryAndNotesList = GetReturnHistoryAndNotes(returnModel.RmaReturnDetailsId);
            returnModel.RMAOrderModel = GetOrderDetails(returnModel.UserId, returnModel.OrderNumber, true, isReturnProcessComplete, returnModel.RmaReturnStateId);
            returnModel.BarcodeImage = GetBarcodeImage(returnModel.ReturnNumber);
            WarehouseModel warehouseDetails = GetReturnWarehouseDetails(returnModel.PortalId);
            returnModel.ShippingToAddressHtml = GetReturnShippingAddressHTML(warehouseDetails?.Address, warehouseDetails?.WarehouseName);
            AddressModel customerAddress = GetCustomerAddressDetails(returnModel.AddressId);
            returnModel.ShippingFromAddressHtml = GetReturnShippingAddressHTML(customerAddress, customerAddress?.FirstName + " " + customerAddress?.LastName);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return returnModel;
        }

        //Save return history
        public virtual bool CreateReturnHistory(List<RMAReturnHistoryModel> returnHistoryModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNull(returnHistoryModel) && returnHistoryModel?.Count < 1)
            {
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelInvalid);
            }
            else
            {
                IEnumerable<ZnodeRmaReturnHistory> returnHistory = new ZnodeRepository<ZnodeRmaReturnHistory>().Insert(returnHistoryModel?.ToEntity<ZnodeRmaReturnHistory>()?.ToList());
                ZnodeLogging.LogMessage(IsNotNull(returnHistory) && returnHistory?.Count() > 0 ? Admin_Resources.SuccessCreateReturnHistory : Admin_Resources.ErrorCreateReturnHistory, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return returnHistory?.Count() > 0;
            }
        }

        //Save additional notes for return.
        public virtual bool SaveReturnNotes(RMAReturnNotesModel rmaReturnNotesModel)
        {
            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNull(rmaReturnNotesModel) && string.IsNullOrEmpty(rmaReturnNotesModel.Notes) && Equals(rmaReturnNotesModel.RmaReturnDetailsId, 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { rmaReturnNotesModel = rmaReturnNotesModel });

            ZnodeRmaReturnNote returnNotes = _returnNoteRepository.Insert(rmaReturnNotesModel?.ToEntity<ZnodeRmaReturnNote>());

            return returnNotes?.RmaReturnNotesId > 0 ? true : false;
        }


        // To check line-item eligibility for create return
        public virtual RMAReturnModel IsValidReturnItems(RMAReturnModel returnModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<ZnodeOmsOrderLineItem> lineItems = GetService<IZnodeOrderHelper>().GetOrderLineItemByOrderId(returnModel.OmsOrderDetailsId).ToList();
            List<OrderLineItemModel> orderLineItems = lineItems.ToModel<OrderLineItemModel>().ToList();

            foreach (var item in orderLineItems)
            {
                foreach (var returnItem in returnModel.ReturnLineItems)
                {
                    if (returnItem.OmsOrderLineItemsId == item.OmsOrderLineItemsId)
                    {
                        if(!string.Equals(item.OrderLineItemState, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                            && !string.Equals(item.OrderLineItemState, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                            && item.Quantity < 1)
                        {
                            returnModel.HasError = true;
                            returnItem.HasError = true;
                            returnItem.ErrorMessage = Admin_Resources.ErrorLineItemNotEligibleForReturn;
                        }
                    }
                }

            }
            return returnModel;
        }
        #endregion

        #region Protected Methods
        //Get Return Eligible Order Number List by using store procedure.
        protected virtual IList<string> GetReturnEligibleOrderNumberList(int userId, int portalId, string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber) || Equals(userId, 0) || Equals(portalId, 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            //Get RMA Configuration
            RMAConfigurationModel rmaConfiguration = GetService<IRMAConfigurationService>().GetRMAConfiguration();

            IList<string> eligibleOrderNumberList = null;

            if (IsNull(rmaConfiguration) || rmaConfiguration?.MaxDays == 0)
                return eligibleOrderNumberList;

            ZnodeLogging.LogMessage("Input parameters to Get Eligible OrderNumber List For Return:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, portalId = portalId, orderNumber = orderNumber });
            IZnodeViewRepository<string> objStoredProc = new ZnodeViewRepository<string>();
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@MaxDays", rmaConfiguration.MaxDays, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@OrderNumber", orderNumber, ParameterDirection.Input, DbType.String);
            eligibleOrderNumberList = objStoredProc.ExecuteStoredProcedureList("Znode_GetEligibleOrderNumberListForReturn @UserId, @PortalId,@MaxDays,@OrderNumber");
            return eligibleOrderNumberList;
        }

        //get the order details by order number.
        protected virtual OrderModel GetOrderDetails(int userId, string orderNumber, bool IsAdminRequest = false, bool IsReturnProcessComplete = false, int RmaReturnStateId = 0)
        {
            if (string.IsNullOrEmpty(orderNumber) || Equals(userId, 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            ZnodeOmsOrderDetail orderDetail = GetOrderDetailsByOrderNumber(orderNumber, userId);

            if (IsNull(orderDetail))
                return null;

            OrderModel orderModel = new OrderModel();
            //null check for order detail object.

            ZnodeLogging.LogMessage("OmsOrderDetailsId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderDetailsId = orderDetail?.OmsOrderDetailsId });
            //Map order detail object to OrderMOdel object.
            orderModel = orderDetail.ToModel<OrderModel>();
            orderModel.BillingAddress.PhoneNumber = orderDetail.BillingPhoneNumber;

            if (IsAdminRequest)
            {
                orderModel.PaymentType = (from paymentRepository in new ZnodeRepository<ZnodePaymentType>().Table.Where(x => x.PaymentTypeId == orderDetail.PaymentTypeId) select paymentRepository.Name)?.FirstOrDefault();
                orderModel.PaymentStatus = (from paymentStateRepository in new ZnodeRepository<ZnodeOmsPaymentState>().Table.Where(x => x.OmsPaymentStateId == orderDetail.OmsPaymentStateId) select paymentStateRepository.Name)?.FirstOrDefault();
                orderModel.OrderState = new ZnodeRepository<ZnodeOmsOrderState>().GetById(orderModel.OmsOrderStateId)?.OrderStateName;
            }

            //check for omsOrderDetailsId greater than 0.
            if (orderModel?.OmsOrderDetailsId > 0)
            {
                //Map orderNumber.
                orderModel.OrderNumber = orderNumber;

                if (!IsReturnProcessComplete)
                {
                    BindOrderLineItemData(orderDetail, orderModel);

                    //Get ShoppingCart By OrderId.
                    orderModel.ShoppingCartModel = GetShoppingCartByOrderId(orderModel.OmsOrderId, orderModel.PortalId, orderModel.UserId);

                    orderModel?.ShoppingCartModel?.ShoppingCartItems?.ForEach(X => { X.PersonaliseValuesDetail = orderModel?.OrderLineItems?.FirstOrDefault(y => y.OmsOrderLineItemsId == X.OmsOrderLineItemsId)?.PersonaliseValuesDetail; });

                    //set the amount of dissenter discount applied during order creation.
                    GetService<IOrderService>().SetOrderDiscount(orderModel);
                }
                else if (IsReturnProcessComplete && (RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED))
                {
                    //set the amount of discount applied during order creation.
                    GetService<IOrderService>().SetOrderDiscount(orderModel);
                }
            }
            return orderModel;
        }

        //Bind order line item data
        protected virtual void BindOrderLineItemData(ZnodeOmsOrderDetail orderDetail, OrderModel orderModel)
        {
            if (IsNull(orderDetail) || IsNull(orderModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            List<ZnodeOmsOrderLineItem> orderLineItems = GetService<IZnodeOrderHelper>()?.GetOrderLineItemByOrderId(orderDetail.OmsOrderDetailsId)?.ToList();
            orderModel.OrderLineItems = orderLineItems.Where(m => m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles))?
                                            .ToModel<OrderLineItemModel>()?.ToList();
            if (orderModel.OrderLineItems?.Count > 0)
            {
                orderModel.OrderLineItems.ForEach(x =>
                {
                    x.Description = orderLineItems.FirstOrDefault(m => m.OrderLineItemRelationshipTypeId == Convert.ToInt16(ZnodeCartItemRelationshipTypeEnum.AddOns)
                                                   && m.ParentOmsOrderLineItemsId == x.OmsOrderLineItemsId)?.Description ?? x.Description;
                    x.Quantity = Convert.ToDecimal(ServiceHelper.ToInventoryRoundOff(x.Quantity));
                    ZnodeOmsOrderLineItem lineItem = orderLineItems.FirstOrDefault(y => y.OmsOrderLineItemsId == x.OmsOrderLineItemsId);
                    x.Price += orderLineItems
                            .Where(z => z.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns
                            && z.ParentOmsOrderLineItemsId == x.OmsOrderLineItemsId
                            ).Sum(z => z.Price);
                    x.OrderWarehouse = orderLineItems.FirstOrDefault(y => y.OmsOrderLineItemsId == x.OmsOrderLineItemsId)?.ZnodeOmsOrderWarehouses.ToModel<OrderWarehouseModel>()?.ToList();
                    if (lineItem.ZnodeOmsOrderAttributes?.Count > 0)
                    {
                        x.Attributes = new List<OrderAttributeModel>();
                        foreach (ZnodeOmsOrderAttribute item in lineItem.ZnodeOmsOrderAttributes)
                            x.Attributes.Add(new OrderAttributeModel { AttributeCode = item.AttributeCode, AttributeValue = item.AttributeValue, AttributeValueCode = item.AttributeValueCode });
                    }
                    ZnodeOmsOrderLineItem parentLineItem = orderLineItems.FirstOrDefault(y => y.OmsOrderLineItemsId == x.ParentOmsOrderLineItemsId);
                    if (IsNotNull(parentLineItem))
                        x.GroupId = parentLineItem.GroupId;
                });

                //Bind Personalized Data
                BindPersonalizedData(orderModel);
            }
        }

        //Get order details as shopping cart by order id.
        protected virtual ShoppingCartModel GetShoppingCartByOrderId(int orderId, int portalId, int userId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (Equals(orderId, 0) || Equals(portalId, 0) || Equals(userId, 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            ZnodeLogging.LogMessage(" Input parameters OrderId, portalId, userId, portalCatalogId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderId, portalId, userId });
            //Get catalog list by portal id.
            int portalCatalogId = GetPortalCatalogId(portalId).GetValueOrDefault();

            IShoppingCartService _shoppingCartService = GetService<IShoppingCartService>();

            //Get shopping cart model by using orderId.
            ShoppingCartModel shoppingCart = _shoppingCartService.GetShoppingCart(new CartParameterModel
            {
                LocaleId = GetLocaleIdFromHeader(),
                PortalId = portalId,
                UserId = userId,
                PublishedCatalogId = portalCatalogId > 0 ? portalCatalogId : 0,
                OmsOrderId = orderId
            });
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return shoppingCart;
        }



        protected virtual bool IsValidLineItemForReturn(DateTime orderDate, DateTime? shipDate, string orderLineItemStatus, RMAConfigurationModel rmaConfiguration)
        {
            if (IsNotNull(rmaConfiguration))
            {
                int rmaPeriod = 0;
                if (rmaConfiguration.MaxDays.HasValue)
                    rmaPeriod = rmaConfiguration.MaxDays.Value;

                if (string.Equals(orderLineItemStatus, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    orderDate = IsNotNull(shipDate) ? shipDate.GetValueOrDefault() : orderDate;
                }
                DateTime validRmaConfigOrderDate = Convert.ToDateTime(orderDate).AddDays(rmaPeriod);
                return !(validRmaConfigOrderDate.Date < DateTime.Now.Date);
            }
            return false;
        }

        //Valid order line items for return
        protected virtual void ValidOrderLineItemsForReturn(OrderModel orderModel)
        {
            //Set only shipped line items
            RMAConfigurationModel rmaConfiguration = GetService<IRMAConfigurationService>().GetRMAConfiguration();
            if (IsNotNull(orderModel) && orderModel?.OrderLineItems?.Count > 0)
            {
                orderModel.OrderLineItems = orderModel?.OrderLineItems?.Where(x => (string.Equals(x.OrderLineItemState, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                                             || string.Equals(x.OrderLineItemState, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                             && x.Quantity > 0
                                             && IsValidLineItemForReturn(orderModel.OrderDate,x.ShipDate , x.OrderLineItemState, rmaConfiguration)
                                             )?.ToList();
            }

            if (IsNotNull(orderModel) && orderModel?.ShoppingCartModel?.ShoppingCartItems?.Count > 0)
            {
                orderModel.ShoppingCartModel.ShoppingCartItems = orderModel?.ShoppingCartModel?.ShoppingCartItems?.Where(x => (string.Equals(x.OrderLineItemStatus, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                                                                  || string.Equals(x.OrderLineItemStatus, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.InvariantCultureIgnoreCase)) &&
                                                                  x.Quantity > 0
                                                                  && IsValidLineItemForReturn(orderModel.OrderDate, x.ShipDate, x.OrderLineItemStatus, rmaConfiguration)
                                                                  )?.ToList();
                if (orderModel?.ShoppingCartModel?.ShoppingCartItems?.Count > 0)
                {
                    List<RMAReturnLineItemModel> returnedQuantityList = GetReturnedQuantityForOrderReturn(orderModel.OrderNumber, orderModel.IsFromAdminOrder);
                    if (returnedQuantityList?.Count > 0)
                    {
                        foreach (ShoppingCartItemModel shoppingCartItem in orderModel.ShoppingCartModel.ShoppingCartItems)
                        {
                            decimal returnedQuantity = returnedQuantityList?.FirstOrDefault(x => x.OmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId)?.ReturnedQuantity ?? 0;
                            if (returnedQuantity > 0)
                            {
                                shoppingCartItem.Quantity = shoppingCartItem.Quantity - returnedQuantity;
                                foreach (AssociatedProductModel associatedProduct in shoppingCartItem?.GroupProducts)
                                {
                                    associatedProduct.Quantity = associatedProduct.Quantity - returnedQuantity;
                                }
                            }
                        }
                    }
                }
            }
        }

        //Get return list by SP.
        protected virtual IList<RMAReturnModel> GetReturnList(PageListModel pageListModel, bool isAdminRequest, string returnDateRange)
        {
            if (IsNull(pageListModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("pagelist model to get return list:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { pageListModel = pageListModel?.ToDebugString() });
            IZnodeViewRepository<RMAReturnModel> objStoredProc = new ZnodeViewRepository<RMAReturnModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@IsAdmin", isAdminRequest, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@ReturnDate", returnDateRange, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@SalesRepUserId", HelperMethods.GetSalesRepUserId(), ParameterDirection.Input, DbType.Int32);

            return objStoredProc.ExecuteStoredProcedureList("Znode_GetRmaReturnHistoryList @WhereClause, @Rows,@PageNo,@Order_By,@IsAdmin,@RowCount OUT,@ReturnDate,@SalesRepUserId", 4, out pageListModel.TotalRowCount);
        }

        //Get warehouse details by portal id
        protected virtual WarehouseModel GetReturnWarehouseDetails(int portalId)
        {
            WarehouseModel warehouseDetails = new WarehouseModel();
            if (portalId > 0)
            {
                int warehouseId = new ZnodeRepository<ZnodePortalWarehouse>().Table.FirstOrDefault(x => x.PortalId == portalId)?.WarehouseId ?? 0;
                ZnodeLogging.LogMessage("warehouseId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { warehouseId = warehouseId });
                if (warehouseId > 0)
                {
                    IWarehouseService _warehouseService = GetService<IWarehouseService>();
                    warehouseDetails = _warehouseService.GetWarehouse(warehouseId);
                }
            }
            return warehouseDetails;
        }

        //Get customer address details
        protected virtual AddressModel GetCustomerAddressDetails(int? addressId)
        {
            if (IsNotNull(addressId) && addressId > 0)
                return new ZnodeRepository<ZnodeAddress>().Table.FirstOrDefault(x => x.AddressId == addressId)?.ToModel<AddressModel>();
            else
                return null;
        }

        //Get shipping address HTML
        protected virtual string GetReturnShippingAddressHTML(AddressModel address, string addressName)
        {
            if (IsNotNull(address))
            {
                string street1 = string.IsNullOrEmpty(address.Address2) ? string.Empty : "<br />" + address.Address2;
                return $"{addressName}{"<br />"}{address.Address1}{street1}{"<br />"}{ address.CityName}{", "}{(string.IsNullOrEmpty(address.StateCode) ? address.StateName : address.StateCode)}{", "}{address.CountryName}{" "}{address.PostalCode}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{address.PhoneNumber}";
            }
            return string.Empty;
        }

        //Get return line items list
        protected virtual List<RMAReturnLineItemModel> GetReturnLineItems(int rmaReturnDetailsId, bool isBindPersonaliseData = false, bool isReturnProcessComplete = false)
        {
            List<RMAReturnLineItemModel> returnLineItemsList = new List<RMAReturnLineItemModel>();
            if (rmaReturnDetailsId > 0)
            {
                List<ZnodeRmaReturnState> returnStates = _returnStateRepository.Table.ToList();
                returnLineItemsList = _returnLineItemsRepository.Table.Where(x => x.RmaReturnDetailsId == rmaReturnDetailsId)?.ToModel<RMAReturnLineItemModel>()?.ToList();
                List<ZnodeOmsPersonalizeItem> personalizeList = null;
                List<OrderLineItemModel> orderLineItemsList = null;
                if (isBindPersonaliseData || isReturnProcessComplete)
                {
                    List<int> orderLineItemsIds = returnLineItemsList?.Select(x => isReturnProcessComplete ? x.OmsReturnOrderLineItemsId.GetValueOrDefault() : x.OmsOrderLineItemsId)?.ToList();
                    if (orderLineItemsIds?.Count > 0)
                    {
                        orderLineItemsList = new ZnodeRepository<ZnodeOmsOrderLineItem>().Table.Where(x => orderLineItemsIds.Contains(x.OmsOrderLineItemsId))?.ToModel<OrderLineItemModel>()?.ToList();
                        personalizeList = GetPersonalizedValueOrderLineItemList(orderLineItemsList);
                    }
                }
                IZnodeOrderHelper orderHelper = GetService<IZnodeOrderHelper>();
                List<int> returnLineItemsIds = returnLineItemsList?.Select(x => x.OmsOrderLineItemsId)?.ToList();
                List<ZnodeOmsOrderLineItem> returnPaymentStatusIds = new ZnodeRepository<ZnodeOmsOrderLineItem>().Table.Where(x => returnLineItemsIds.Contains(x.OmsOrderLineItemsId)).ToList();
                List<OrderDiscountModel> returnItemDiscountList = orderHelper.GetReturnItemsDiscount(rmaReturnDetailsId);
                returnLineItemsList?.ForEach(returnLineItem =>
                {
                    returnLineItem.ReturnStatus = returnStates?.FirstOrDefault(y => y.RmaReturnStateId == returnLineItem?.RmaReturnStateId)?.ReturnStateName;
                    if (personalizeList?.Count > 0)
                    {
                        //get personalize attributes by omsorderlineitemid
                        returnLineItem.PersonaliseValueList = GetPersonalizedValueOrderLineItem(orderLineItemsList?.FirstOrDefault(x => x.OmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId)?.ParentOmsOrderLineItemsId ?? returnLineItem.OmsOrderLineItemsId, personalizeList);
                        returnLineItem.PersonaliseValuesDetail = orderHelper.GetPersonalizedAttributeLineItemDetails(returnLineItem.PersonaliseValueList, string.Empty);
                    }
                    returnLineItem.PaymentStatusId = returnPaymentStatusIds.Where(x => x.OmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId)?.FirstOrDefault()?.PaymentStatusId;
                    orderHelper.SetPerQuantityDiscountForReturnItem(returnItemDiscountList, returnLineItem);

                });
            }
            return returnLineItemsList;
        }

        //Get return notes list
        protected virtual List<RMAReturnNotesModel> GetReturnNotes(RMAReturnModel returnModel)
        {
            List<RMAReturnNotesModel> returnNotesList = new List<RMAReturnNotesModel>();
            if (IsNotNull(returnModel))
            {
                returnNotesList = _returnNoteRepository.Table.Where(x => x.RmaReturnDetailsId == returnModel.RmaReturnDetailsId)?.ToModel<RMAReturnNotesModel>()?.ToList();
                if (IsNotNull(returnNotesList) && returnNotesList?.Count > 0)
                {
                    List<int> userIds = returnNotesList?.Where(x => x.CreatedBy != returnModel.UserId)?.Select(x => x.CreatedBy)?.ToList();
                    List<ZnodeUser> znodeUserList = null;
                    if (userIds?.Count > 0)
                        znodeUserList = new ZnodeRepository<ZnodeUser>().Table.Where(x => userIds.Contains(x.UserId))?.ToList();
                    foreach (RMAReturnNotesModel returnNotes in returnNotesList)
                    {
                        if (returnNotes.CreatedBy == returnModel.UserId)
                            returnNotes.UserEmailId = returnModel.EmailId;
                        else
                            returnNotes.UserEmailId = znodeUserList?.FirstOrDefault(x => x.UserId == returnNotes.CreatedBy)?.Email;
                    }

                }
            }
            return returnNotesList;
        }

        //Save data for order return.
        protected virtual RMAReturnModel InsertOrderReturn(RMAReturnModel returnModel)
        {
            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            //Get generated unique order number on basis of current date.           
            ParameterModel portalId = new ParameterModel() { Ids = Convert.ToString(returnModel.PortalId) };

            returnModel.ReturnNumber = !string.IsNullOrEmpty(returnModel.ReturnNumber) ? returnModel.ReturnNumber : GenerateReturnNumber(returnModel, portalId);

            ZnodeLogging.LogMessage("Generated return number: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { GenerateReturnNumber = returnModel?.ReturnNumber });

            //Bind Return Details Data
            BindReturnDetailsData(returnModel);

            //start transaction
            using (SqlConnection connection = new SqlConnection(HelperMethods.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();// Start a local transaction.

                try
                {
                    //Save return details
                    ZnodeRmaReturnDetail znodeRmaReturnDetail = _returnDetailsRepository.Insert(returnModel?.ToEntity<ZnodeRmaReturnDetail>());
                    ZnodeLogging.LogMessage(znodeRmaReturnDetail?.RmaReturnDetailsId > 0 ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.ErrorFailedToCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                    if (znodeRmaReturnDetail?.RmaReturnDetailsId > 0)
                    {
                        returnModel.RmaReturnDetailsId = znodeRmaReturnDetail.RmaReturnDetailsId;

                        //Save return line item data
                        SaveOrderReturnLineItem(returnModel.RmaReturnDetailsId, returnModel.ReturnLineItems);

                        //Save additional notes for return.
                        if (!string.IsNullOrEmpty(returnModel?.Notes))
                        {
                            SaveReturnNotes(new RMAReturnNotesModel()
                            {
                                RmaReturnDetailsId = returnModel.RmaReturnDetailsId,
                                Notes = returnModel.Notes,
                                CreatedBy = returnModel.CreatedBy,
                                CreatedDate = returnModel.CreatedDate,
                                ModifiedBy = returnModel.ModifiedBy,
                                ModifiedDate = returnModel.ModifiedDate
                            });
                        }
                    }
                    transaction.Commit();
                    //Send return request notification email to customer and admin
                    if (returnModel.IsSubmitReturn)
                        SendReturnRequestEmailNotificationAsync(returnModel);

                    ZnodeLogging.LogMessage("Execution done", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return new RMAReturnModel() { ReturnNumber = returnModel.ReturnNumber, RmaReturnDetailsId = returnModel.RmaReturnDetailsId };
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    transaction.Rollback();
                    if (returnModel.RmaReturnDetailsId > 0)
                        DeleteOrderReturn(returnModel.RmaReturnDetailsId);
                    throw new ZnodeException(ErrorCodes.ExceptionalError, ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        //Bind user data
        protected virtual void BindUserData(RMAReturnModel returnModel)
        {
            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeUser userModel = GetUser(returnModel.UserId);
            returnModel.FirstName = userModel?.FirstName;
            returnModel.LastName = userModel?.LastName;
            returnModel.EmailId = userModel?.Email;
            returnModel.PhoneNumber = userModel?.PhoneNumber;
        }

        //Save return line item data
        protected virtual void SaveOrderReturnLineItem(int rmaReturnDetailsId, List<RMAReturnLineItemModel> returnLineItems)
        {
            if (IsNotNull(returnLineItems) && returnLineItems.Count > 0)
            {
                returnLineItems.ForEach(x => x.RmaReturnDetailsId = rmaReturnDetailsId);
                _returnLineItemsRepository.Insert(returnLineItems?.ToEntity<ZnodeRmaReturnLineItem>()?.ToList());
            }
        }

        //Update return order
        protected virtual RMAReturnModel UpdateOrderReturn(RMAReturnModel returnModel)
        {
            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<int> deleteReturnLineItemIds = returnModel.ReturnLineItems.Where(x => x.RmaReturnLineItemsId > 0 && x.ExpectedReturnQuantity == 0)?.Select(x => x.RmaReturnLineItemsId)?.ToList();
            returnModel.ReturnLineItems.RemoveAll(x => x.ExpectedReturnQuantity == 0);

            RMAReturnModel updateReturnModel = BindReturnDetailsDataForUpdate(returnModel);

            //start transaction
            using (SqlConnection connection = new SqlConnection(HelperMethods.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();// Start a local transaction.

                try
                {
                    //Save return line item data
                    List<RMAReturnLineItemModel> insertReturnLineItemsList = updateReturnModel.ReturnLineItems.Where(x => x.RmaReturnLineItemsId == 0 && x.ExpectedReturnQuantity > 0)?.ToList();
                    SaveOrderReturnLineItem(updateReturnModel.RmaReturnDetailsId, insertReturnLineItemsList);

                    //Update return line item data
                    List<RMAReturnLineItemModel> updateReturnLineItemsList = updateReturnModel.ReturnLineItems.Where(x => x.RmaReturnLineItemsId > 0 && x.ExpectedReturnQuantity > 0)?.ToList();
                    updateReturnLineItemsList.ForEach(x => x.RmaReturnDetailsId = updateReturnModel.RmaReturnDetailsId);
                    UpdateReturnLineItems(updateReturnLineItemsList, returnModel.IsSubmitReturn);

                    //Delete return line item data
                    if (deleteReturnLineItemIds.Count > 0)
                        DeleteReturnLineItems(new ParameterModel() { Ids = String.Join(",", deleteReturnLineItemIds) });

                    bool isUpdateReturnDetails = _returnDetailsRepository.Update(updateReturnModel?.ToEntity<ZnodeRmaReturnDetail>());
                    ZnodeLogging.LogMessage("Return details updated:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { isUpdateReturnDetails = isUpdateReturnDetails });

                    InsertUpdateReturnNotes(updateReturnModel.Notes, updateReturnModel.RmaReturnDetailsId, updateReturnModel.UserId);

                    transaction.Commit();

                    //Send return request notification email to customer and admin
                    if (returnModel.IsSubmitReturn)
                        SendReturnRequestEmailNotificationAsync(returnModel);

                    ZnodeLogging.LogMessage("Execution done", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return new RMAReturnModel() { ReturnNumber = updateReturnModel.ReturnNumber, RmaReturnDetailsId = updateReturnModel.RmaReturnDetailsId };
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    transaction.Rollback();
                    throw new ZnodeException(ErrorCodes.ExceptionalError, ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        //Bind return details data for update
        protected virtual RMAReturnModel BindReturnDetailsDataForUpdate(RMAReturnModel returnModel)
        {
            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeRmaReturnDetail znodeReturnDetails = _returnDetailsRepository.Table.FirstOrDefault(x => x.ReturnNumber.ToLower() == returnModel.ReturnNumber.ToLower() && x.UserId == returnModel.UserId);
            if (IsNotNull(znodeReturnDetails))
            {
                RMAReturnModel updateReturnDetails = znodeReturnDetails.ToModel<RMAReturnModel>();

                updateReturnDetails.TotalExpectedReturnQuantity = returnModel.ReturnLineItems.Sum(x => x.ExpectedReturnQuantity);
                DateTime returnDateTime = GetDateWithTime();
                if (returnModel.IsSubmitReturn)
                    updateReturnDetails.ReturnDate = returnDateTime;
                else
                    updateReturnDetails.ReturnDate = null;

                updateReturnDetails.RmaReturnStateId = returnModel.IsSubmitReturn ? (int)ZnodeReturnStateEnum.SUBMITTED : (int)ZnodeReturnStateEnum.NOT_SUBMITTED;
                updateReturnDetails.ReturnStatus = returnModel.IsSubmitReturn ? ZnodeConstant.ReturnStateSubmitted : ZnodeConstant.ReturnStateNotSubmitted;
                updateReturnDetails.ModifiedDate = returnDateTime;
                updateReturnDetails.ModifiedBy = returnModel.UserId;
                updateReturnDetails.Notes = returnModel.Notes;

                //Bind Order Return Line Item Data
                BindOrderReturnLineItemData(returnModel, GetShoppingCartDetails(updateReturnDetails.UserId, updateReturnDetails.PortalId, updateReturnDetails.OmsOrderId));
                updateReturnDetails.ReturnLineItems = returnModel.ReturnLineItems;

                //Bind Order Summary
                BindOrderReturnSummary(updateReturnDetails);
                return updateReturnDetails;
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);
        }

        //Update return line items
        protected virtual bool UpdateReturnLineItems(List<RMAReturnLineItemModel> updateReturnLineItems, bool isSubmit)
        {
            if (IsNotNull(updateReturnLineItems) && updateReturnLineItems.Count > 0)
            {
                if (isSubmit)
                    return _returnLineItemsRepository.BatchUpdate(updateReturnLineItems.ToEntity<ZnodeRmaReturnLineItem>()?.ToList());
                else
                {
                    List<RMAReturnLineItemModel> updateReturnLineItemsList = new List<RMAReturnLineItemModel>();
                    List<int> updateReturnLineItemIds = updateReturnLineItems.Where(y => IsNotNull(y)).Select(x => x.RmaReturnLineItemsId)?.ToList();
                    List<ZnodeRmaReturnLineItem> znodeReturnLineItemList = _returnLineItemsRepository.Table.Where(x => updateReturnLineItemIds.Contains(x.RmaReturnLineItemsId))?.ToList();

                    foreach (RMAReturnLineItemModel returnlineitem in updateReturnLineItems)
                    {
                        ZnodeRmaReturnLineItem znodeReturnLineItem = znodeReturnLineItemList.FirstOrDefault(x => x.RmaReturnLineItemsId == returnlineitem.RmaReturnLineItemsId);
                        if (returnlineitem.ExpectedReturnQuantity != znodeReturnLineItem.ExpectedReturnQuantity || returnlineitem.RmaReasonForReturnId != znodeReturnLineItem.RmaReasonForReturnId)
                            updateReturnLineItemsList.Add(returnlineitem);
                    }

                    if (updateReturnLineItemsList.Count > 0)
                        return _returnLineItemsRepository.BatchUpdate(updateReturnLineItemsList.ToEntity<ZnodeRmaReturnLineItem>()?.ToList());
                }
            }
            return false;
        }

        //Delete return line items
        protected virtual bool DeleteReturnLineItems(ParameterModel returnLineItemIds)
        {
            if (IsNotNull(returnLineItemIds) || !string.IsNullOrEmpty(returnLineItemIds.Ids))
            {
                FilterCollection filters = new FilterCollection { new FilterTuple(ZnodeRmaReturnLineItemEnum.RmaReturnLineItemsId.ToString(), ProcedureFilterOperators.In, returnLineItemIds.Ids) };
                EntityWhereClauseModel whereClauseForDelete = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                return _returnLineItemsRepository.Delete(whereClauseForDelete.WhereClause);
            }
            return false;
        }

        //Get order details by ordernumber and userid
        protected virtual ZnodeOmsOrderDetail GetOrderDetailsByOrderNumber(string orderNumber, int userId)
        {
            if (!string.IsNullOrEmpty(orderNumber) && userId > 0)
            {
                IZnodeRepository<ZnodeOmsOrder> _omsOrderRepository = new ZnodeRepository<ZnodeOmsOrder>();
                IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailsRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
                return (from znodeorderDetails in _orderDetailsRepository.Table
                        join znodeOrder in _omsOrderRepository.Table on znodeorderDetails.OmsOrderId equals znodeOrder.OmsOrderId
                        where znodeOrder.OrderNumber.ToLower() == orderNumber.ToLower() && znodeorderDetails.IsActive && znodeorderDetails.UserId == userId
                        select znodeorderDetails)?.FirstOrDefault();
            }
            else
                return null;
        }

        //Validate calculation order return data
        protected virtual bool ValidateCalculationOrderReturn(RMAReturnCalculateModel returnCalculateModel)
        {
            bool status = true;
            foreach (RMAReturnCalculateLineItemModel returnCalculateLineItemModel in returnCalculateModel?.ReturnCalculateLineItemList)
            {
                if (returnCalculateModel.IsAdminRequest)
                {
                    if (returnCalculateLineItemModel.ReturnedQuantity > returnCalculateLineItemModel.ExpectedReturnQuantity)
                    {
                        status = false;
                        returnCalculateLineItemModel.HasError = true;
                        returnCalculateLineItemModel.ErrorMessage = Admin_Resources.ErrorReturnQuantityLessThanMessage;
                        break;
                    }
                }
                else
                {
                    if (returnCalculateLineItemModel.ExpectedReturnQuantity < 1)
                    {
                        status = false;
                        returnCalculateLineItemModel.HasError = true;
                        returnCalculateLineItemModel.ErrorMessage = Admin_Resources.ErrorReturnQuantity;
                        break;
                    }
                    else if (returnCalculateLineItemModel.ExpectedReturnQuantity > returnCalculateLineItemModel.ShippedQuantity)
                    {
                        status = false;
                        returnCalculateLineItemModel.HasError = true;
                        returnCalculateLineItemModel.ErrorMessage = Admin_Resources.ErrorReturnQuantityLessThanMessage;
                        break;
                    }
                }
            }
            foreach (RMAReturnCalculateLineItemModel returnCalculateLineItemModel in returnCalculateModel?.ReturnCalculateLineItemList)
            {
                if (!returnCalculateLineItemModel.HasError && returnCalculateLineItemModel.ErrorMessage == null)
                {
                    status = true;
                    break;
                }
            }
            ZnodeLogging.LogMessage("Validate calculation order return data status", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { status = status });
            return status;
        }

        //Perform calculation for an order return line item
        protected virtual void PerformCalculationForOrderReturnLineItem(RMAReturnCalculateModel returnCalculateModel, ShoppingCartModel shoppingCartModel)
        {
            if (IsNotNull(shoppingCartModel) && shoppingCartModel?.ShoppingCartItems?.Count > 0)
            {
                //Get tax rule against order
                TaxRuleModel taxRule = GetTaxRate(shoppingCartModel.OmsOrderId);
                foreach (RMAReturnCalculateLineItemModel returnCalculateLineItem in returnCalculateModel.ReturnCalculateLineItemList)
                {
                    if (returnCalculateLineItem.ErrorMessage == null && !returnCalculateLineItem.HasError)
                    {
                        ShoppingCartItemModel shoppingCartItem = shoppingCartModel?.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == returnCalculateLineItem.OmsOrderLineItemsId);
                        if (IsNotNull(shoppingCartItem))
                        {
                            decimal quantity = returnCalculateLineItem.ReturnedQuantity ?? returnCalculateLineItem.ExpectedReturnQuantity;
                            returnCalculateLineItem.TotalLineItemPrice = shoppingCartItem.UnitPrice * quantity;
                            returnCalculateLineItem.UnitPrice = shoppingCartItem.UnitPrice;

                            returnCalculateLineItem.TaxCost = GetTaxCost(taxRule, shoppingCartItem, quantity, shoppingCartModel.IsCalculateTaxAfterDiscount.GetValueOrDefault());

                            returnCalculateLineItem.PerQuantityLineItemDiscount = quantity > 0 ? shoppingCartItem.PerQuantityLineItemDiscount : 0;
                            returnCalculateLineItem.PerQuantityCSRDiscount = quantity > 0 ? shoppingCartItem.PerQuantityCSRDiscount : 0;
                            returnCalculateLineItem.PerQuantityShippingCost = quantity > 0 ? shoppingCartItem.PerQuantityShippingCost : 0;
                            returnCalculateLineItem.PerQuantityShippingDiscount = quantity > 0 ? shoppingCartItem.PerQuantityShippingDiscount : 0;
                            returnCalculateLineItem.ShippingCost = quantity > 0 ? shoppingCartItem.PerQuantityShippingCost * quantity : 0;
                            returnCalculateLineItem.PerQuantityOrderLevelDiscountOnLineItem = quantity > 0 ? shoppingCartItem.PerQuantityOrderLevelDiscountOnLineItem : 0;
                            returnCalculateLineItem.PaymentStatusId = shoppingCartItem.PaymentStatusId;
                            returnCalculateLineItem.PerQuantityVoucherAmount = shoppingCartItem.PerQuantityVoucherAmount;
                            returnCalculateLineItem.ParentOmsOrderLineItemsId = shoppingCartItem.ParentOmsOrderLineItemsId;
                            returnCalculateLineItem.ImportDuty = shoppingCartItem.ImportDuty > 0 ? (shoppingCartItem.ImportDuty / shoppingCartItem.Quantity) * returnCalculateLineItem.ExpectedReturnQuantity : shoppingCartItem.ImportDuty;
                        }
                    }
                }
                returnCalculateModel.ReturnSubTotal = returnCalculateModel.ReturnCalculateLineItemList.Sum(x => x.TotalLineItemPrice);
                returnCalculateModel.ReturnTaxCost = returnCalculateModel.ReturnCalculateLineItemList.Sum(x => x.TaxCost);
                returnCalculateModel.ReturnShippingCost = GetShippingCost(returnCalculateLineItemList: returnCalculateModel.ReturnCalculateLineItemList);
                returnCalculateModel.ReturnShippingDiscount = GetShippingDiscount(returnCalculateLineItemList: returnCalculateModel.ReturnCalculateLineItemList);
                returnCalculateModel.Discount = GetTotalDiscountOnLineItem(returnCalculateLineItemList: returnCalculateModel.ReturnCalculateLineItemList);
                returnCalculateModel.CSRDiscount = GetCSRDiscount(returnCalculateLineItemList: returnCalculateModel.ReturnCalculateLineItemList);
                decimal taxOnShipping = GetTaxOnShipping(taxRule, returnCalculateModel.ReturnTaxCost, shoppingCartModel.IsCalculateTaxAfterDiscount.GetValueOrDefault(), returnCalculateLineItemList: returnCalculateModel.ReturnCalculateLineItemList);
                returnCalculateModel.ReturnCharges = GetReturnCharges(taxOnShipping, returnCalculateLineItemList: returnCalculateModel.ReturnCalculateLineItemList);
                returnCalculateModel.ReturnImportDuty = returnCalculateModel.ReturnCalculateLineItemList.Sum(x => x.ImportDuty);
            }
        }

        
        //Get Tax rate for an order
        protected virtual TaxRuleModel GetTaxRate(int? orderId)
        {
            if (orderId > 0)
            {
                IZnodeViewRepository<TaxRuleModel> objStoredProc = new ZnodeViewRepository<TaxRuleModel>();
                objStoredProc.SetParameter("@OrderId", Convert.ToInt32(orderId) > 0 ? orderId : -1, ParameterDirection.Input, DbType.Int32);
                IList<TaxRuleModel> taxRuleList = objStoredProc.ExecuteStoredProcedureList("Znode_GetOrderTaxDetails @OrderId");
                return taxRuleList?.ToList()?.FirstOrDefault() ?? new TaxRuleModel();
            }
            return null;
        }

        //Validate order return data
        protected virtual bool ValidateOrderReturn(List<RMAReturnLineItemModel> returnLineItemList, string actionMode)
        {
            bool status = true;
            if (returnLineItemList?.Count > 0)
            {
                foreach (RMAReturnLineItemModel returnLineItem in returnLineItemList)
                {
                    if (returnLineItem.ExpectedReturnQuantity > returnLineItem.ShippedQuantity)
                    {
                        status = false;
                        returnLineItem.HasError = true;
                        returnLineItem.ErrorMessage = Admin_Resources.ErrorReturnQuantityLessThanMessage;
                    }
                    else if (actionMode == ZnodeConstant.Create && returnLineItem.ExpectedReturnQuantity < 1)
                    {
                        status = false;
                        returnLineItem.HasError = true;
                        returnLineItem.ErrorMessage = Admin_Resources.ErrorReturnQuantity;

                    }
                    else if (actionMode == ZnodeConstant.Edit && returnLineItem.RmaReturnLineItemsId < 1 && returnLineItem.ExpectedReturnQuantity < 1)
                    {
                        status = false;
                        returnLineItem.HasError = true;
                        returnLineItem.ErrorMessage = Admin_Resources.ErrorReturnQuantity;
                    }
                }
            }
            else
                status = false;

            ZnodeLogging.LogMessage("Validate order return data status", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { status = status });
            return status;
        }

        //Bind Return Details Data
        protected virtual void BindReturnDetailsData(RMAReturnModel returnModel)
        {
            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeOmsOrderDetail orderDetail = GetOrderDetailsByOrderNumber(returnModel.OrderNumber, returnModel.UserId);
            if (IsNotNull(orderDetail))
            {
                returnModel.OmsOrderId = orderDetail.OmsOrderId;
                returnModel.OmsOrderDetailsId = orderDetail.OmsOrderDetailsId;

                DateTime returnDateTime = GetDateWithTime();

                if (returnModel.IsSubmitReturn)
                    returnModel.ReturnDate = returnDateTime;
                else
                    returnModel.ReturnDate = null;

                returnModel.RmaReturnStateId = returnModel.IsSubmitReturn ? (int)ZnodeReturnStateEnum.SUBMITTED : (int)ZnodeReturnStateEnum.NOT_SUBMITTED;
                returnModel.ReturnStatus = returnModel.IsSubmitReturn ? ZnodeConstant.ReturnStateSubmitted : ZnodeConstant.ReturnStateNotSubmitted;
                returnModel.TotalExpectedReturnQuantity = returnModel.ReturnLineItems.Sum(x => x.ExpectedReturnQuantity);
                returnModel.AddressId = orderDetail.AddressId;
                returnModel.ShippingId = orderDetail.ShippingId;
                returnModel.ShippingNumber = orderDetail.ShippingNumber;
                returnModel.IsTaxCostEdited = (bool)orderDetail.IsTaxCostEdited;
                returnModel.IsActive = true;
                returnModel.CurrencyCode = orderDetail.CurrencyCode;
                returnModel.CultureCode = orderDetail.CultureCode;
                returnModel.ModifiedDate = returnDateTime;
                returnModel.ModifiedBy = returnModel.UserId;
                returnModel.VoucherAmount = returnModel.VoucherAmount;
                returnModel.IsCalculateTaxAfterDiscount = orderDetail.IsCalculateTaxAfterDiscount.GetValueOrDefault();
                returnModel.IsPricesInclusiveOfTaxes = orderDetail.IsPricesInclusiveOfTaxes.GetValueOrDefault();

                //Bind User Data
                BindUserData(returnModel);
                //BindOrderReturnLineItemData
                BindOrderReturnLineItemData(returnModel, GetShoppingCartDetails(returnModel.UserId, returnModel.PortalId, returnModel.OmsOrderId));
                //Bind Order Return Summary
                BindOrderReturnSummary(returnModel);
            }
        }    

        //Get shopping cart details
        protected virtual ShoppingCartModel GetShoppingCartDetails(int userId, int portalId, int orderId)
        {
            if (Equals(userId, 0) || Equals(portalId, 0) || Equals(orderId, 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            //Get catalog list by portal id.
            int portalCatalogId = (new ZnodeRepository<ZnodePortalCatalog>().Table.FirstOrDefault(x => x.PortalId == portalId)?.PublishCatalogId).GetValueOrDefault();
            CartParameterModel cartParameterModel = new CartParameterModel
            {
                LocaleId = GetLocaleIdFromHeader(),
                PortalId = portalId,
                UserId = userId,
                PublishedCatalogId = portalCatalogId > 0 ? portalCatalogId : 0,
                OmsOrderId = orderId
            };
            return GetService<IZnodeShoppingCart>()?.LoadCartFromOrder(cartParameterModel, GetCatalogVersionId(cartParameterModel.PublishedCatalogId));
        }

        //Bind Order Return Line Item Data
        protected virtual void BindOrderReturnLineItemData(RMAReturnModel returnModel, ShoppingCartModel shoppingCartModel)
        {
            if (IsNotNull(returnModel) && IsNotNull(shoppingCartModel) && shoppingCartModel?.ShoppingCartItems?.Count > 0)
            {
                TaxRuleModel taxRuleModel = GetTaxRate(shoppingCartModel.OmsOrderId);
                foreach (RMAReturnLineItemModel returnLineItem in returnModel?.ReturnLineItems)
                {
                    ShoppingCartItemModel shoppingCartItem = shoppingCartModel?.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId);
                    if (IsNotNull(shoppingCartItem))
                    {
                        returnLineItem.OmsOrderLineItemsId = shoppingCartItem.OmsOrderLineItemsId;
                        if (IsNull(shoppingCartItem.OrderLineItemRelationshipTypeId) && IsNotNull(shoppingCartItem.BundleProductSKUs))
                            returnLineItem.OrderLineItemRelationshipTypeId = (int)ZnodeCartItemRelationshipTypeEnum.Bundles;
                        else
                            returnLineItem.OrderLineItemRelationshipTypeId = (int)shoppingCartItem.OrderLineItemRelationshipTypeId;

                        returnLineItem.Sku = shoppingCartItem.SKU;
                        returnLineItem.Description = shoppingCartItem.Description;

                        if (IsNotNull(shoppingCartItem.OrderLineItemRelationshipTypeId))
                        {
                            if (shoppingCartItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)
                            {
                                returnLineItem.Sku = shoppingCartItem?.GroupProducts.FirstOrDefault().Sku;
                                returnLineItem.Description = shoppingCartItem?.GroupProducts?.FirstOrDefault().ProductName;
                            }
                            else if (shoppingCartItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)
                            {
                                returnLineItem.Sku = shoppingCartItem.ConfigurableProductSKUs;
                            }
                        }
                        
                        returnLineItem.ProductName = shoppingCartItem.ProductName;
                        returnLineItem.Price = shoppingCartItem.UnitPrice;

                        returnLineItem.ShipSeparately = shoppingCartItem.ShipSeperately;
                        returnLineItem.ReturnDate = GetDateWithTime();
                        returnLineItem.RmaReturnStateId = returnModel.IsSubmitReturn ? (int)ZnodeReturnStateEnum.SUBMITTED : (int)ZnodeReturnStateEnum.NOT_SUBMITTED;
                        returnModel.ModifiedDate = GetDateWithTime();
                        returnModel.ModifiedBy = returnModel.UserId;
                        returnLineItem.IsActive = true;


                        returnLineItem.TaxCost = GetTaxCost(taxRuleModel, shoppingCartItem, returnLineItem.ExpectedReturnQuantity, shoppingCartModel.IsCalculateTaxAfterDiscount.GetValueOrDefault());
                        returnLineItem.PerQuantityLineItemDiscount = shoppingCartItem.PerQuantityLineItemDiscount;
                        returnLineItem.PerQuantityCSRDiscount = shoppingCartItem.PerQuantityCSRDiscount;
                        returnLineItem.PerQuantityShippingCost = shoppingCartItem.PerQuantityShippingCost;
                        returnLineItem.DiscountAmount = GetTotalLineItemDiscount(returnLineItem.ExpectedReturnQuantity, returnLineItem.PerQuantityLineItemDiscount, returnLineItem.PerQuantityCSRDiscount, returnLineItem.RefundAmount.GetValueOrDefault());
                        returnLineItem.ShippingCost = shoppingCartItem.PerQuantityShippingCost * returnLineItem.ExpectedReturnQuantity;
                        returnLineItem.PerQuantityShippingDiscount = shoppingCartItem.PerQuantityShippingDiscount;
                        returnLineItem.PerQuantityOrderLevelDiscountOnLineItem = shoppingCartItem.PerQuantityOrderLevelDiscountOnLineItem;
                        returnLineItem.PaymentStatusId = shoppingCartItem.PaymentStatusId;
                        returnLineItem.PerQuantityVoucherAmount = shoppingCartItem.PerQuantityVoucherAmount;
                        returnLineItem.ParentOmsOrderLineItemsId = shoppingCartItem.ParentOmsOrderLineItemsId;
                        returnLineItem.ImportDuty = shoppingCartItem.ImportDuty > 0 ? (shoppingCartItem.ImportDuty / shoppingCartItem.Quantity) * returnLineItem.ExpectedReturnQuantity : shoppingCartItem.ImportDuty;
                    }
                }
            }
        }

        //Calculate and return tax rate
        protected virtual decimal GetTaxCost(TaxRuleModel taxRate, ShoppingCartItemModel shoppingCartItem, decimal quantity, bool isCalculateTaxAfterDiscount)
        {
            decimal shippingCost = GetLineItemShippingCost(shoppingCartItem, quantity, isCalculateTaxAfterDiscount);
            decimal shippingTaxCost = HelperUtility.IsNotNull(taxRate) && taxRate.TaxShipping
                                      && shippingCost > 0 && quantity > 0 ?
                                      (shippingCost * (taxRate.TaxRate / 100)) : 0;
            return shoppingCartItem.TaxCost > 0 ? ((shoppingCartItem.TaxCost / shoppingCartItem.Quantity) * quantity) + shippingTaxCost : shoppingCartItem.TaxCost;
        }

        // Get line item shipping cost.
        protected virtual decimal GetLineItemShippingCost (ShoppingCartItemModel shoppingCartItem, decimal quantity, bool isCalculateTaxAfterDiscount)
        {
            return isCalculateTaxAfterDiscount ? (shoppingCartItem.PerQuantityShippingCost * quantity) - (shoppingCartItem.PerQuantityShippingDiscount * quantity) : (shoppingCartItem.PerQuantityShippingCost * quantity);
        }

        //Bind Order Return Summary
        protected virtual void BindOrderReturnSummary(RMAReturnModel returnModel, bool isAdminRequest = false)
        {
            //Get tax rule against order
            TaxRuleModel taxRule = GetTaxRate(returnModel.OmsOrderId);
            if (IsNotNull(returnModel) && IsNotNull(returnModel?.ReturnLineItems))
            {
                returnModel.SubTotal = !isAdminRequest ? returnModel.ReturnLineItems.Sum(x => x.ExpectedReturnQuantity * x.Price) : (decimal)returnModel.ReturnLineItems.Sum(x => x.ReturnedQuantity * x.Price);
                returnModel.ReturnShippingCost = GetShippingCost(returnModel.ReturnLineItems);
                returnModel.DiscountAmount = GetTotalDiscountOnLineItem(returnModel.ReturnLineItems);
                returnModel.ReturnTaxCost = returnModel.ReturnLineItems.Sum(x => x.TaxCost);
                returnModel.CSRDiscount = GetCSRDiscount(returnModel.ReturnLineItems);
                returnModel.ReturnShippingDiscount = GetShippingDiscount(returnModel.ReturnLineItems);                
                decimal taxOnShipping = GetTaxOnShipping(taxRule, returnModel.ReturnTaxCost, returnModel.IsCalculateTaxAfterDiscount, returnModel.ReturnLineItems);
                returnModel.ReturnCharges = GetReturnCharges(taxOnShipping, returnModel.ReturnLineItems);
                returnModel.VoucherAmount = returnModel.VoucherAmount;
                returnModel.ReturnImportDuty = returnModel.ReturnLineItems.Sum(x => x.ImportDuty);
            }
        }

        //Get Returned Quantity For Order Return
        protected virtual List<RMAReturnLineItemModel> GetReturnedQuantityForOrderReturn(string orderNumber, bool isFromAdminOrder)
        {
            List<RMAReturnLineItemModel> returnedQuantityList = null;
            if (isFromAdminOrder)
            {
                returnedQuantityList = (from returnDetail in _returnDetailsRepository.Table
                                        join returnLineItem in _returnLineItemsRepository.Table
                                        on returnDetail.RmaReturnDetailsId equals returnLineItem.RmaReturnDetailsId
                                        where returnDetail.OrderNumber == orderNumber
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.APPROVED
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.REFUND_PROCESSED
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.NOT_SUBMITTED
                                        group returnLineItem by returnLineItem.OmsOrderLineItemsId into returnLineItemGroup
                                        select new RMAReturnLineItemModel
                                        {
                                            OmsOrderLineItemsId = returnLineItemGroup.Key,
                                            ReturnedQuantity = returnLineItemGroup.Sum(x => x.ExpectedReturnQuantity),
                                        })?.ToList();
            }
            else
            {
                returnedQuantityList = (from returnDetail in _returnDetailsRepository.Table
                                        join returnLineItem in _returnLineItemsRepository.Table
                                        on returnDetail.RmaReturnDetailsId equals returnLineItem.RmaReturnDetailsId
                                        where returnDetail.OrderNumber == orderNumber
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.APPROVED
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.REFUND_PROCESSED
                                        && returnDetail.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED
                                        group returnLineItem by returnLineItem.OmsOrderLineItemsId into returnLineItemGroup
                                        select new RMAReturnLineItemModel
                                        {
                                            OmsOrderLineItemsId = returnLineItemGroup.Key,
                                            ReturnedQuantity = returnLineItemGroup.Sum(x => x.ExpectedReturnQuantity),
                                        })?.ToList();
            }
            return returnedQuantityList;
        }

        //Get product image path for return line items
        protected virtual List<RMAReturnLineItemModel> GetProductImagePath(List<RMAReturnLineItemModel> returnLineItems, int portalId)
        {
            if (IsNotNull(returnLineItems) && returnLineItems?.Count > 0 && portalId > 0)
            {
                int publishCatalogId = GetPortalCatalogId(portalId).GetValueOrDefault();
                int localeId = GetDefaultLocaleId();
                List<PublishedProductEntityModel> productList = GetService<IPublishedProductDataService>().GetPublishProductBySKUs(returnLineItems?.Select(x => x.Sku)?.ToList(), publishCatalogId, localeId, GetCatalogVersionId(publishCatalogId, localeId, portalId))?.ToModel<PublishedProductEntityModel>()?.ToList();

                ZnodeLogging.LogMessage("Product list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { productList = productList?.Count });

                IImageHelper objImage = GetService<IImageHelper>();
                if (IsNotNull(productList) && productList.Count > 0)
                {
                    foreach (RMAReturnLineItemModel returnItem in returnLineItems)
                    {
                        string imagePath = productList.FirstOrDefault(x => x.SKU.ToLower() == returnItem.Sku.ToLower())?.Attributes?.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;
                        returnItem.ImagePath = BindImagePath(imagePath, portalId, objImage);
                    }
                }
            }
            return returnLineItems ?? new List<RMAReturnLineItemModel>();
        }

        //Bind product image full path
        protected virtual string BindImagePath(string imagePath, int portalId, IImageHelper objImage)
        {
            if (IsNull(objImage))
                objImage = GetService<IImageHelper>();

            return GetService<IShoppingCartItemMap>()?.GetImagePath(imagePath, portalId, objImage);
        }

        //Generate barcode image
        protected virtual string GetBarcodeImage(string returnNumber)
        {
            if (IsNotNull(returnNumber))
            {
                BarcodeModel barcodeModel = new BarcodeModel()
                {
                    BarcodeText = returnNumber,
                    FontName = ZnodeConstant.IDAutomationHC39M,
                    FontSize = ZnodeConstant.TTFFontSize12,
                    Length = 20,
                    Height = 80,
                    PointX = 2f,
                    PointY = 2f,
                    BarcodeLineColor = Color.Black,
                    BarcodeBackgroundColor = Color.White,
                    BarcodeImageFormat = ImageFormat.Png,
                };
                return GetService<IBarcodeHelper>().GenerateBarcode(barcodeModel);
            }
            else
            {
                return null;
            }
        }

        //Insert Update Return Notes
        protected virtual void InsertUpdateReturnNotes(string notes, int returnDetailsId, int userId)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { notes = notes, returnDetailsId = returnDetailsId, userId = userId });
            if (returnDetailsId > 0 && userId > 0)
            {
                int returnNotesId = _returnNoteRepository.Table.FirstOrDefault(x => x.RmaReturnDetailsId == returnDetailsId && x.CreatedBy == userId)?.RmaReturnNotesId ?? 0;

                RMAReturnNotesModel returnNotesModel = new RMAReturnNotesModel()
                {
                    RmaReturnDetailsId = returnDetailsId,
                    Notes = notes,
                    ModifiedBy = userId,
                    ModifiedDate = GetDateWithTime()
                };

                if (returnNotesId > 0 && !string.IsNullOrEmpty(notes))
                {
                    returnNotesModel.RmaReturnNotesId = returnNotesId;
                    UpdateReturnNotes(returnNotesModel);
                }
                else if (returnNotesId < 1 && !string.IsNullOrEmpty(notes))
                    SaveReturnNotes(returnNotesModel);
                else if (returnNotesId > 0 && string.IsNullOrEmpty(notes))
                    DeleteReturnNotes(new ParameterModel() { Ids = returnNotesId.ToString() });
            }
        }

        //Update return notes
        protected virtual bool UpdateReturnNotes(RMAReturnNotesModel rmaReturnNotesModel)
        {
            if (!string.IsNullOrEmpty(rmaReturnNotesModel?.Notes) && rmaReturnNotesModel?.RmaReturnDetailsId > 0)
                return _returnNoteRepository.Update(rmaReturnNotesModel?.ToEntity<ZnodeRmaReturnNote>());
            return false;
        }

        //Delete return notes
        protected virtual bool DeleteReturnNotes(ParameterModel deleteReturnNoteIds)
        {
            if (IsNotNull(deleteReturnNoteIds) || !string.IsNullOrEmpty(deleteReturnNoteIds.Ids))
            {
                FilterCollection filters = new FilterCollection { new FilterTuple(ZnodeRmaReturnNoteEnum.RmaReturnNotesId.ToString(), ProcedureFilterOperators.In, deleteReturnNoteIds.Ids) };
                EntityWhereClauseModel whereClauseForDelete = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                return _returnNoteRepository.Delete(whereClauseForDelete.WhereClause);
            }
            return false;
        }

        //This method will send email notification to customer       
        protected virtual void SendReturnRequestNotificationEmailToCustomer(RMAEmailDetailsModel emailDetails, RMAReturnModel returnModel)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { emailDetails = emailDetails });
            if (IsNotNull(emailDetails))
            {
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(emailDetails.EmailTemplateCode, emailDetails.PortalId);
                PortalModel portalModel = GetCustomPortalDetails(emailDetails.PortalId);
                emailDetails.CustomerServiceEmail = portalModel?.CustomerServiceEmail;
                emailDetails.CustomerServicePhoneNumber = portalModel?.CustomerServicePhoneNumber;

                if (IsNotNull(emailTemplateMapperModel))
                {
                    emailDetails.UserFullName = string.IsNullOrEmpty(emailDetails.UserFullName) ? emailDetails.EmailId : emailDetails.UserFullName;
                    if (!string.IsNullOrEmpty(emailDetails.ReturnStatus))
                    {
                        string webstoreDomain = GetWebstoreDomainByPortalId(emailDetails.PortalId, ZnodeConstant.Webstore);
                        emailDetails.ReturnDetailsUrl = $"{HttpContext.Current.Request.Url.Scheme}://{webstoreDomain}/RMAReturn/GetReturnDetails?returnNumber={emailDetails.ReturnNumber}";
                    }
                    string messageText = GetEmailMessageText(emailDetails, emailTemplateMapperModel.Descriptions, returnModel, portalModel?.StoreName);

                    emailTemplateMapperModel.Subject = GetEmailMessageText(emailDetails, emailTemplateMapperModel.Subject, returnModel);

                    ZnodeLogging.LogMessage("Message text to send email notification to customer :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { messageText = messageText });

                    string bcc = string.Empty;
                    try
                    {
                        if (IsNotNull(portalModel) && emailDetails.PortalId > 0)
                            ZnodeEmail.SendEmail(emailDetails.PortalId, emailDetails.EmailId, portalModel.AdministratorEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, emailDetails.PortalId, bcc), $"{portalModel.StoreName} - {emailTemplateMapperModel.Subject}", messageText, true,null,string.Empty,returnModel.BarcodeImage);
                        else
                            ZnodeEmail.SendEmail(emailDetails.EmailId, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, emailDetails.PortalId, bcc), $"{ZnodeConfigManager.SiteConfig.StoreName} - {emailTemplateMapperModel.Subject}", messageText, true, returnModel.BarcodeImage);
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginCreateSuccess, emailDetails.UserFullName, string.Empty, null, ex.Message, null);
                    }
                }
            }
        }

        //Get email message text
        protected virtual string GetEmailMessageText(RMAEmailDetailsModel emailDetails, string emailDescription, RMAReturnModel returnModel, string storeName = null)
        {
            string messageText = string.Empty;
            if (IsNotNull(emailDetails) && !string.IsNullOrEmpty(emailDescription))
            {
                messageText = ReplaceTokenWithMessageText("#UserFullName#", emailDetails.UserFullName, emailDescription);
                messageText = ReplaceTokenWithMessageText("#ReturnNumber#", emailDetails.ReturnNumber, messageText);
                messageText = ReplaceTokenWithMessageText("#ReturnStatus#", returnModel.ReturnStatus, messageText);
                messageText = ReplaceTokenWithMessageText("#StoreName#", storeName, messageText);
                messageText = ReplaceTokenWithMessageText("#Url#", emailDetails.ReturnDetailsUrl, messageText);
                messageText = ReplaceTokenWithMessageText("#ReturnDate#", returnModel.ReturnDate?.ToTimeZoneDateTimeFormat(), messageText);
                messageText = ReplaceTokenWithMessageText("#ReturnTotalExpectedQty#", (Convert.ToInt32(returnModel.TotalExpectedReturnQuantity)).ToString(), messageText);
                messageText = ReplaceTokenWithMessageText("#ShippingTo#", returnModel.ShippingToAddressHtml, messageText);
                messageText = ReplaceTokenWithMessageText("#ShippingFrom#", returnModel.ShippingFromAddressHtml, messageText);
                messageText = ReplaceTokenWithMessageText("#OrderNumber#", returnModel.OrderNumber, messageText);
                messageText = ReplaceTokenWithMessageText("#CustomerName#", emailDetails.UserFullName, messageText);
                messageText = ReplaceTokenWithMessageText("#CustomerServiceEmail#", emailDetails.CustomerServiceEmail, messageText);
                messageText = ReplaceTokenWithMessageText("#CustomerServicePhoneNumber#", emailDetails.CustomerServicePhoneNumber, messageText);
                messageText = ReplaceTokenWithMessageText("#ReturnBarcode#", ZnodeConstant.ContentId, messageText);
                messageText = ReplaceTokenWithMessageText("#TotalCost#", GetFormatPriceWithCurrency(returnModel.TotalReturnAmount, returnModel.CultureCode), messageText);

                if (string.Equals(returnModel?.ReturnStatus, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    messageText = GenerateHtmlReceiptWithParser(messageText, returnModel);
                messageText = _emailTemplateSharedService.ReplaceTemplateTokens(messageText, new Dictionary<string, string>());
            }
            return messageText;
        }

        //Get domain name on the basis of portal id.
        protected virtual string GetWebstoreDomainByPortalId(int portalId, string applicationType)
        {
            string domainName = string.Empty;
            if (portalId > 0 && !string.IsNullOrEmpty(applicationType))
                domainName = (from znodeDomainRepository in new ZnodeRepository<ZnodeDomain>().Table.Where(x => x.PortalId == portalId && x.ApplicationType.ToLower() == applicationType.ToLower() && x.IsActive && x.IsDefault) select znodeDomainRepository.DomainName)?.FirstOrDefault();
            return domainName;
        }

        //This method will send return order request notification email to admin.        
        protected virtual void SendReturnRequestNotificationEmailToAdmin(RMAReturnModel returnModel, string emailTemplateCode, RMAEmailDetailsModel emailDetails)
        {
            ZnodeLogging.LogMessage("Input Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { returnNumber = returnModel.ReturnNumber, portalId = returnModel.PortalId });

            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(emailTemplateCode, returnModel.PortalId);
            PortalModel portalModel = GetCustomPortalDetails(returnModel.PortalId);
            emailDetails.CustomerServiceEmail = portalModel?.CustomerServiceEmail;
            emailDetails.CustomerServicePhoneNumber = portalModel?.CustomerServicePhoneNumber;
            
            if (IsNotNull(emailTemplateMapperModel))
            {
                string messageText = GetEmailMessageText(emailDetails, emailTemplateMapperModel.Descriptions, returnModel, portalModel?.StoreName);

                ZnodeLogging.LogMessage("Message text for return request notification email to admin:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { messageText = messageText });

                string bcc = string.Empty;
                try
                {
                    if (IsNotNull(portalModel) && returnModel.PortalId > 0)
                        ZnodeEmail.SendEmail(returnModel.PortalId, portalModel?.AdministratorEmail, portalModel?.AdministratorEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, returnModel.PortalId, bcc), $"{portalModel?.StoreName} - {emailTemplateMapperModel?.Subject}", messageText, true);
                    else
                        ZnodeEmail.SendEmail(portalModel?.AdministratorEmail, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, returnModel.PortalId, bcc), $"{ZnodeConfigManager.SiteConfig.StoreName} - {emailTemplateMapperModel?.Subject}", messageText, true);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginCreateSuccess, returnModel.ReturnNumber, string.Empty, null, ex.Message, null);
                }
            }
        }

        //Send return email notification
        protected virtual bool SendReturnRequestEmailNotificationAsync(RMAReturnModel returnModel, bool isAdminRequest = false)
        {
            bool emailNotificationStatus = false;
            if (IsNotNull(returnModel))
            {
                System.Web.HttpContext current = System.Web.HttpContext.Current;

                Thread thread = new Thread(new ThreadStart(() =>
                {
                    System.Web.HttpContext.Current = current;
                    try
                    {
                        RMAEmailDetailsModel emailDetails = new RMAEmailDetailsModel()
                        {
                            EmailId = returnModel.EmailId,
                            PortalId = returnModel.PortalId,
                            ReturnNumber = returnModel.ReturnNumber,
                            UserFullName = string.Concat(returnModel.FirstName, " ", returnModel.LastName)
                        };

                        if (isAdminRequest)
                        {
                            emailDetails.ReturnStatus = returnModel.ReturnStatus;
                            if (returnModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.REFUND_PROCESSED)
                            {
                                //refund email notification
                                emailDetails.EmailTemplateCode = ZnodeConstant.EmailTemplateRefundProcessedNotificationForCustomer;
                                SendReturnRequestNotificationEmailToCustomer(emailDetails, returnModel);
                                var OnRefundProcessedInit = new ZnodeEventNotifier<RMAReturnModel>(returnModel, EventConstant.OnRefundProcessedNotificationForCustomer);
                            }
                            else
                            {
                                //Updated return status email notification
                                emailDetails.EmailTemplateCode = ZnodeConstant.EmailTemplateReturnStatusNotificationForCustomer;
                                SendReturnRequestNotificationEmailToCustomer(emailDetails, returnModel);
                                var OnReturnStatusInit = new ZnodeEventNotifier<RMAReturnModel>(returnModel, EventConstant.OnReturnStatusNotificationForCustomer);
                            }
                        }
                        else
                        {
                            //The phoneNumber was getting override so need to reassign.
                            string phoneNumber = returnModel.PhoneNumber;
                            returnModel = GetReturnDetails(returnModel.ReturnNumber, GetReturnDetailsExpands());
                            returnModel.PhoneNumber = phoneNumber;
                            emailDetails.EmailTemplateCode = ZnodeConstant.EmailTemplateReturnRequestNotificationForCustomer;
                            SendReturnRequestNotificationEmailToCustomer(emailDetails, returnModel);
                            var OnReturnRequestInit = new ZnodeEventNotifier<RMAReturnModel>(returnModel, EventConstant.OnReturnRequestNotificationForCustomer);
                            SendReturnRequestNotificationEmailToAdmin(returnModel, ZnodeConstant.EmailTemplateReturnRequestNotificationForAdmin, emailDetails);
                            emailNotificationStatus = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        emailNotificationStatus = false;
                    }
                }));
                thread.Start();
            }
            ZnodeLogging.LogMessage("Return request email notification status", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { emailNotificationStatus = emailNotificationStatus });
            return emailNotificationStatus;
        }
        
        //Map return history from history as well as notes.
        protected virtual List<RMAReturnHistoryModel> GetReturnHistoryAndNotes(int rmaReturnDetailsId)
        {
            if (rmaReturnDetailsId > 0)
            {
                IZnodeViewRepository<RMAReturnHistoryModel> objStoredProc = new ZnodeViewRepository<RMAReturnHistoryModel>();
                objStoredProc.SetParameter("@RmaReturnDetailsId", rmaReturnDetailsId, ParameterDirection.Input, DbType.Int32);
                IList<RMAReturnHistoryModel> returnHistoryAndNotesList = objStoredProc.ExecuteStoredProcedureList("Znode_GetReturnHistory @RmaReturnDetailsId");
                ZnodeLogging.LogMessage("Return history list count and RmaReturnDetailsId to get Return history: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ReturnHistoryListCount = returnHistoryAndNotesList?.Count, RmaReturnDetailsId = rmaReturnDetailsId });
                return returnHistoryAndNotesList?.ToList();
            }
            else
            {
                return new List<RMAReturnHistoryModel>();
            }
        }

        //Get Personalized Value Order Line Item List
        protected virtual List<ZnodeOmsPersonalizeItem> GetPersonalizedValueOrderLineItemList(List<OrderLineItemModel> lineItems)
        {
            List<ZnodeOmsPersonalizeItem> orderPersonalizeItemList = null;
            if (lineItems?.Count > 0)
            {
                List<int?> lineItemIds = lineItems.Select(x => Convert.ToInt32(x.ParentOmsOrderLineItemsId) > 0 ? x.ParentOmsOrderLineItemsId : x.OmsOrderLineItemsId)?.Distinct()?.ToList();
                if (lineItemIds?.Count > 0)
                    orderPersonalizeItemList = new ZnodeRepository<ZnodeOmsPersonalizeItem>().Table.Where(x => lineItemIds.Contains(x.OmsOrderLineItemsId))?.ToList();
            }
            return orderPersonalizeItemList;
        }

        //Get Personalized Value Order Line Item
        protected virtual Dictionary<string, object> GetPersonalizedValueOrderLineItem(int orderLineItemId, List<ZnodeOmsPersonalizeItem> personalizeItems)
        {
            Dictionary<string, object> personalizeItem = new Dictionary<string, object>();
            if (orderLineItemId > 0 && personalizeItems?.Count > 0)
            {
                foreach (KeyValuePair<string, string> personalizeAttr in personalizeItems.Where(x => x.OmsOrderLineItemsId == orderLineItemId)?.ToDictionary(x => x.PersonalizeCode, x => x.PersonalizeValue))
                    personalizeItem.Add(personalizeAttr.Key, (object)personalizeAttr.Value);
            }

            return personalizeItem;
        }

        //Bind Personalized Data
        protected virtual void BindPersonalizedData(OrderModel orderModel)
        {
            //check for omsOrderDetailsId greater than 0.
            if (orderModel?.OmsOrderDetailsId > 0 && IsNotNull(orderModel?.OrderLineItems))
            {
                List<ZnodeOmsPersonalizeItem> personalizeList = GetPersonalizedValueOrderLineItemList(orderModel.OrderLineItems);
                IZnodeOrderHelper orderHelper = GetService<IZnodeOrderHelper>();
                foreach (OrderLineItemModel lineItem in orderModel.OrderLineItems)
                {
                    //get personalize attributes by omsorderlineitemid
                    lineItem.PersonaliseValueList = GetPersonalizedValueOrderLineItem(Convert.ToInt32(lineItem.ParentOmsOrderLineItemsId) > 0 ? Convert.ToInt32(lineItem.ParentOmsOrderLineItemsId) : lineItem.OmsOrderLineItemsId, personalizeList);
                    lineItem.PersonaliseValuesDetail = orderHelper.GetPersonalizedAttributeLineItemDetails(lineItem.PersonaliseValueList, string.Empty);
                }
            }
        }

        //Replace sort key name
        protected virtual void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            if (IsNotNull(sorts))
            {
                foreach (string key in sorts.Keys)
                {
                    if (string.Equals(key, ZnodeConstant.ReturnTotalWithCurrency, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, ZnodeConstant.ReturnTotalWithCurrency.ToLower(), ZnodeRmaReturnDetailEnum.TotalReturnAmount.ToString()); }
                    if (string.Equals(key, ZnodeConstant.ReturnDateWithTime, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, ZnodeConstant.ReturnDateWithTime.ToLower(), ZnodeRmaReturnDetailEnum.ReturnDate.ToString()); }
                }
            }
        }

        // Get User Detail for Guest and Registered users.
        protected virtual ZnodeUser GetUser(int userId)
        {
            IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();
            return _userRepository?.Table.FirstOrDefault(x => x.UserId == userId);
        }

        #region Admin

    //Update return order for admin
    protected virtual RMAReturnModel UpdateOrderReturnForAdmin(RMAReturnModel returnModel)
        {
            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            RMAReturnModel updateReturnModel = BindReturnDetailsDataForAdmin(returnModel);

            //start transaction
            using (SqlConnection connection = new SqlConnection(HelperMethods.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();// Start a local transaction.

                try
                {
                    if (updateReturnModel?.ReturnLineItems?.Count > 0)
                    {
                        _returnLineItemsRepository.BatchUpdate(updateReturnModel.ReturnLineItems.ToEntity<ZnodeRmaReturnLineItem>()?.ToList());

                        if (returnModel.ReturnLineItems.All(x => x.RmaReturnStateId == (int)ZnodeReturnStateEnum.REJECTED))
                            updateReturnModel.RmaReturnStateId = (int)ZnodeReturnStateEnum.REJECTED;

                        bool isUpdateReturnDetails = _returnDetailsRepository.Update(updateReturnModel?.ToEntity<ZnodeRmaReturnDetail>());
                        ZnodeLogging.LogMessage("Return details updated:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { isUpdateReturnDetails = isUpdateReturnDetails });

                        //Save additional notes for return.
                        if (isUpdateReturnDetails && !string.IsNullOrEmpty(returnModel?.Notes))
                        {
                            int userId = HelperMethods.GetLoginAdminUserId();
                            SaveReturnNotes(new RMAReturnNotesModel()
                            {
                                RmaReturnDetailsId = returnModel.RmaReturnDetailsId,
                                Notes = returnModel.Notes,
                                CreatedBy = userId,
                                CreatedDate = returnModel.ModifiedDate,
                                ModifiedBy = userId,
                                ModifiedDate = returnModel.ModifiedDate
                            });
                        }
                        SaveReturnHistory(returnModel);
                        transaction.Commit();

                        if (!string.Equals(returnModel.OldReturnStatus, returnModel.ReturnStatus, StringComparison.InvariantCultureIgnoreCase))
                            SendReturnRequestEmailNotificationAsync(returnModel, true);

                        ZnodeLogging.LogMessage("Execution done", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                        return new RMAReturnModel() { ReturnNumber = updateReturnModel.ReturnNumber, RmaReturnDetailsId = updateReturnModel.RmaReturnDetailsId };
                    }
                    else
                    {
                        ZnodeLogging.LogMessage("Line Item not found", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        throw new ZnodeException(ErrorCodes.InvalidData, "Line Item not found");
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    transaction.Rollback();
                    throw new ZnodeException(ErrorCodes.ExceptionalError, ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        //Bind return details data for admin
        protected virtual RMAReturnModel BindReturnDetailsDataForAdmin(RMAReturnModel returnModel)
        {
            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeRmaReturnDetail znodeReturnDetails = _returnDetailsRepository.Table.FirstOrDefault(x => x.ReturnNumber.ToLower() == returnModel.ReturnNumber.ToLower() && x.UserId == returnModel.UserId && x.OrderNumber == returnModel.OrderNumber);
            if (IsNotNull(znodeReturnDetails))
            {
                ZnodeOmsOrderDetail orderDetail = GetOrderDetailsByOrderNumber(returnModel.OrderNumber, returnModel.UserId);                
                returnModel.RmaReturnDetailsId = znodeReturnDetails.RmaReturnDetailsId;
                RMAReturnModel updateReturnDetails = znodeReturnDetails.ToModel<RMAReturnModel>();
                updateReturnDetails.IsCalculateTaxAfterDiscount = (orderDetail?.IsCalculateTaxAfterDiscount).GetValueOrDefault();
                updateReturnDetails.IsPricesInclusiveOfTaxes = (orderDetail?.IsPricesInclusiveOfTaxes).GetValueOrDefault();
                updateReturnDetails.RmaReturnStateId = returnModel.RmaReturnStateId;
                if (returnModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || returnModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED)
                    updateReturnDetails.OverDueAmount = returnModel.OverDueAmount;
                updateReturnDetails.ModifiedDate = GetDateTime();
                updateReturnDetails.ModifiedBy = HelperMethods.GetLoginAdminUserId();
                updateReturnDetails.VoucherAmount = returnModel.VoucherAmount;

                //Bind Order Return Line Item Data for admin
                updateReturnDetails.ReturnLineItems = BindOrderReturnLineItemDataForAdmin(returnModel);

                //Bind Order Summary
                BindOrderReturnSummary(updateReturnDetails, true);
                return updateReturnDetails;
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);
        }

        //Bind Order Return Line Item Data for admin
        protected virtual List<RMAReturnLineItemModel> BindOrderReturnLineItemDataForAdmin(RMAReturnModel returnModel)
        {
            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            List<RMAReturnLineItemModel> returnLineItemsList = _returnLineItemsRepository.Table.Where(x => x.RmaReturnDetailsId == returnModel.RmaReturnDetailsId)?.ToModel<RMAReturnLineItemModel>()?.ToList();

            if (IsNotNull(returnLineItemsList) && returnLineItemsList?.Count > 0)
            {
                foreach (RMAReturnLineItemModel returnLineItem in returnLineItemsList)
                {
                    RMAReturnLineItemModel lineItem = returnModel?.ReturnLineItems?.FirstOrDefault(x => x.RmaReturnLineItemsId == returnLineItem.RmaReturnLineItemsId);
                    if (IsNotNull(lineItem))
                    {
                        returnLineItem.ReturnedQuantity = lineItem.ReturnedQuantity;
                        returnLineItem.ShipSeparately = lineItem.ShipSeparately;
                        returnLineItem.RefundAmount = lineItem.RefundAmount;
                        returnLineItem.IsShippingReturn = lineItem.IsShippingReturn;
                        returnLineItem.ShippingCost = lineItem.ShippingCost;
                        returnLineItem.PerQuantityCSRDiscount = lineItem.PerQuantityCSRDiscount;
                        returnLineItem.PerQuantityLineItemDiscount = lineItem.PerQuantityLineItemDiscount;
                        returnLineItem.PerQuantityShippingDiscount = lineItem.PerQuantityShippingDiscount;
                        returnLineItem.PerQuantityShippingCost = lineItem.ReturnedQuantity >0 ?(lineItem.ShippingCost/lineItem.ReturnedQuantity).GetValueOrDefault():0;
                        returnLineItem.TaxCost = lineItem.TaxCost;
                        returnLineItem.ImportDuty = lineItem.ImportDuty;
                        returnLineItem.RmaReturnStateId = lineItem.RmaReturnStateId;
                        returnLineItem.ModifiedDate = GetDateTime();
                        returnLineItem.ModifiedBy = HelperMethods.GetLoginAdminUserId();
                        returnLineItem.DiscountAmount = lineItem.DiscountAmount;
                        returnLineItem.PerQuantityOrderLevelDiscountOnLineItem = lineItem.PerQuantityOrderLevelDiscountOnLineItem;
                        returnLineItem.PerQuantityVoucherAmount = lineItem.PerQuantityVoucherAmount;
                        if (returnLineItem?.OmsReturnOrderLineItemsId != 0 && (returnModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.APPROVED || returnModel.RmaReturnStateId == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED))
                            returnLineItem.OmsReturnOrderLineItemsId = lineItem.OmsOrderLineItemsId;
                        returnLineItem.ParentOmsOrderLineItemsId = lineItem.ParentOmsOrderLineItemsId;
                    }
                    else
                    {
                        return new List<RMAReturnLineItemModel>();
                    }
                }
            }
            return returnLineItemsList;
        }

        //To save return history
        protected virtual void SaveReturnHistory(RMAReturnModel returnModel)
        {
            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(returnModel))
            {
                List<RMAReturnHistoryModel> returnHistoryModel = new List<RMAReturnHistoryModel>();
                //to check return status is updated
                if (ExistUpdateHistory(ZnodeConstant.ReturnUpdatedStatus, returnModel))
                {
                    string returnHistoryMessage = GetHistoryMessageByKey(ZnodeConstant.ReturnUpdatedStatus, returnModel);
                    returnHistoryModel.Add(new RMAReturnHistoryModel { Message = returnHistoryMessage });
                }

                //to refund processed status is updated 
                if (ExistUpdateHistory(ZnodeConstant.ReturnRefundProcessed, returnModel))
                {
                    string returnHistoryMessage = GetHistoryMessageByKey(ZnodeConstant.ReturnRefundProcessed, returnModel);
                    decimal amount = returnModel.TotalReturnAmount;
                    returnHistoryModel.Add(new RMAReturnHistoryModel { Message = returnHistoryMessage, ReturnAmount = amount });
                }

                if (HelperUtility.IsNotNull(returnModel.ReturnLineItemHistory) && returnModel.ReturnLineItemHistory?.Count > 0)
                {
                    string returnHistoryMessage = string.Empty;
                    string returnShippingMessage = string.Empty;
                    decimal amount = 0;
                    decimal shippingAmount = 0;
                    foreach (var returnLineItemHistory in returnModel.ReturnLineItemHistory)
                    {
                        if (IsNotNull(returnLineItemHistory.Value))
                        {
                            returnHistoryMessage = GenerateReturnLineItemHistory(returnHistoryMessage, returnLineItemHistory.Value, returnModel.OldReturnLineItems, returnModel.CultureCode, ref returnShippingMessage);

                            if (!string.IsNullOrEmpty(returnLineItemHistory.Value.ReturnedQuantity) || !Equals(returnLineItemHistory.Value?.ReturnedQuantity, "0") || !string.IsNullOrEmpty(returnLineItemHistory.Value.ReturnUpdatedStatus))
                                amount = amount + returnLineItemHistory.Value.Total;

                            string oldIsShippingReturn = returnModel.OldReturnLineItems.FirstOrDefault(w => w.RmaReturnLineItemsId == returnLineItemHistory.Value.RmaReturnLineItemsId).IsShippingReturn ? ZnodeConstant.True : ZnodeConstant.False;
                            string updatedShippingReturn = returnLineItemHistory.Value.IsShippingReturn;

                            if (!string.Equals(updatedShippingReturn, oldIsShippingReturn, StringComparison.InvariantCultureIgnoreCase))
                                shippingAmount = string.Equals(updatedShippingReturn, ZnodeConstant.True, StringComparison.InvariantCultureIgnoreCase) ? shippingAmount + returnLineItemHistory.Value.ReturnShippingAmount : shippingAmount;

                            ZnodeLogging.LogMessage("return history amount and shipping amount value: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { amount = amount, shippingAmount = shippingAmount });
                        }
                    }
                    if (!string.IsNullOrEmpty(returnHistoryMessage))
                        returnHistoryModel.Add(new RMAReturnHistoryModel { Message = returnHistoryMessage, ReturnAmount = returnModel.TotalReturnAmount });

                    if (!string.IsNullOrEmpty(returnShippingMessage))
                        returnHistoryModel.Add(new RMAReturnHistoryModel { Message = returnShippingMessage, ReturnAmount = shippingAmount });
                }

                ZnodeLogging.LogMessage("returnHistoryModel count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { returnHistoryModel = returnHistoryModel?.Count });

                if (IsNotNull(returnHistoryModel) && returnHistoryModel?.Count > 0)
                {
                    returnHistoryModel.ForEach(x => { x.RmaReturnDetailsId = returnModel.RmaReturnDetailsId; x.CreatedBy = returnModel.CreatedBy; x.ModifiedBy = returnModel.ModifiedBy; });
                    CreateReturnHistory(returnHistoryModel);
                }
            }
            ZnodeLogging.LogMessage("Execution done", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //to save return line item changes
        protected virtual string GenerateReturnLineItemHistory(string returnHistoryMessage, RMAReturnLineItemHistoryModel skuHistory, List<RMAReturnLineItemModel> oldReturnLineItems, string cultureCode, ref string returnShippingMessage)
        {
            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(skuHistory) && IsNotNull(oldReturnLineItems))
            {
                string sku = skuHistory.SKU;
                int returnLineItemId = skuHistory.RmaReturnLineItemsId;
                string oldIsShippingReturn = oldReturnLineItems.FirstOrDefault(w => w.RmaReturnLineItemsId == returnLineItemId).IsShippingReturn ? ZnodeConstant.True : ZnodeConstant.False;
                string productName = skuHistory.ProductName;
                string oldQuantity = Convert.ToString(oldReturnLineItems.FirstOrDefault(w => w.RmaReturnLineItemsId == returnLineItemId)?.ReturnedQuantity);
                oldQuantity = string.IsNullOrEmpty(oldQuantity) ? Convert.ToString(oldReturnLineItems.FirstOrDefault(w => w.RmaReturnLineItemsId == returnLineItemId)?.ExpectedReturnQuantity) : oldQuantity;
                oldQuantity = oldQuantity?.Split('.')?.FirstOrDefault();
                string confirmedQty = skuHistory.ReturnedQuantity;

                if (!string.IsNullOrEmpty(skuHistory.ReturnUpdatedStatus))
                {
                    string oldStatus = Convert.ToString(oldReturnLineItems.FirstOrDefault(w => w.RmaReturnLineItemsId == returnLineItemId)?.ReturnStatus);
                    confirmedQty = !string.IsNullOrEmpty(confirmedQty) && !Equals(confirmedQty, "0") ? confirmedQty : oldQuantity;
                    returnHistoryMessage = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.ReturnHistoryUpdatedLineItemStatus, sku, productName, confirmedQty, oldStatus, skuHistory.ReturnUpdatedStatus), returnHistoryMessage);
                }

                if (!string.IsNullOrEmpty(skuHistory.ReturnedQuantity) && !Equals(skuHistory.ReturnedQuantity, "0"))
                    returnHistoryMessage = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.ReturnHistoryUpdatedLineItemQuantity, sku, productName, oldQuantity, confirmedQty), returnHistoryMessage);

                ZnodeLogging.LogMessage("returnHistoryMessage: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { returnHistoryMessage = returnHistoryMessage });

                if (!string.IsNullOrEmpty(skuHistory.IsShippingReturn) && !string.Equals(skuHistory.IsShippingReturn, oldIsShippingReturn, StringComparison.InvariantCultureIgnoreCase))
                    returnShippingMessage = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.ReturnHistoryUpdatedLineItemShipping, sku, productName, skuHistory.IsShippingReturn?.ToLower()), returnShippingMessage);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return returnHistoryMessage;
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
        protected virtual string GetHistoryMessageByKey(string key, RMAReturnModel returnModel)
        {
            string value = string.Empty;
            if (IsNotNull(returnModel) && !string.IsNullOrEmpty(key))
            {
                returnModel.ReturnHistory?.TryGetValue(key, out value);

                switch (key)
                {
                    case ZnodeConstant.ReturnUpdatedStatus:
                        return string.Format(Admin_Resources.ReturnHistoryUpdatedStatus, returnModel.OldReturnStatus, value);

                    case ZnodeConstant.ReturnRefundProcessed:
                        return Admin_Resources.ReturnHistoryRefundProcessed;

                    default:
                        return string.Format(Admin_Resources.ReturnHistory, key, value);
                }
            }
            return value;
        }

        //to check update history for provided key
        protected virtual bool ExistUpdateHistory(string key, RMAReturnModel returnModel)
            => returnModel?.ReturnHistory?.Keys?.Contains(key) ?? false;

        //Get return date filter value from filters
        protected virtual string GetReturnDateRangeFromFilters(FilterCollection filters)
        {
            string returnDateRange = string.Empty;
            FilterCollection returnDateFilter = new FilterCollection();
            FilterTuple returnDateFilterTuple = filters?.FirstOrDefault(x => string.Equals(x.FilterName, ZnodeRmaReturnDetailEnum.ReturnDate.ToString(), StringComparison.CurrentCultureIgnoreCase));
            if (IsNotNull(returnDateFilterTuple))
            {
                returnDateFilter.Add(returnDateFilterTuple);
                RemoveFilter(filters, ZnodeRmaReturnDetailEnum.ReturnDate.ToString());
                returnDateRange = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(returnDateFilter.ToFilterDataCollection());
            }
            return returnDateRange;
        }

        //Remove filter from FilterCollection by name
        protected virtual void RemoveFilter(FilterCollection filters, string filterName)
        {
            if (IsNotNull(filters) && !string.IsNullOrEmpty(filterName))
                filters?.RemoveAll(x => string.Equals(x.FilterName, filterName, StringComparison.CurrentCultureIgnoreCase));
        }

        //Check RMA configuration for order
        protected virtual bool IsRMAConfigurationValidForOrder(string orderNumber, int userId)
        {
            if (string.IsNullOrEmpty(orderNumber) || userId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            RMAConfigurationModel rmaConfiguration = GetService<IRMAConfigurationService>().GetRMAConfiguration();
            DateTime? orderDate = GetOrderDetailsByOrderNumber(orderNumber, userId)?.OrderDate;
            if (IsNotNull(rmaConfiguration) && IsNotNull(orderDate))
            {
                int rmaPeriod = 0;
                if (rmaConfiguration.MaxDays.HasValue)
                    rmaPeriod = rmaConfiguration.MaxDays.Value;

                DateTime validRmaConfigOrderDate = Convert.ToDateTime(orderDate).AddDays(rmaPeriod);
                return !(validRmaConfigOrderDate.Date < DateTime.Now.Date);
            }
            return false;
        }

        //Check is return process complete or not
        protected virtual bool IsReturnProcessComplete(int returnStatus) =>
             returnStatus == (int)ZnodeReturnStateEnum.APPROVED ||
             returnStatus == (int)ZnodeReturnStateEnum.REJECTED ||
             returnStatus == (int)ZnodeReturnStateEnum.PARTIALLY_APPROVED ||
             returnStatus == (int)ZnodeReturnStateEnum.REFUND_PROCESSED ? true : false;

        //Update return order for Refund Processed
        protected virtual RMAReturnModel UpdateOrderReturnForRefundProcessed(RMAReturnModel returnModel)
        {
            if (IsNull(returnModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            ZnodeRmaReturnDetail znodeReturnDetails = _returnDetailsRepository.Table.FirstOrDefault(x => x.ReturnNumber.ToLower() == returnModel.ReturnNumber.ToLower() && x.UserId == returnModel.UserId && x.OrderNumber == returnModel.OrderNumber);

            //start transaction
            using (SqlConnection connection = new SqlConnection(HelperMethods.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();// Start a local transaction.

                try
                {
                    if (IsNotNull(znodeReturnDetails))
                    {
                        znodeReturnDetails.RmaReturnStateId = returnModel.RmaReturnStateId;
                        znodeReturnDetails.IsRefundProcess = true;
                        znodeReturnDetails.ModifiedDate = GetDateTime();
                        znodeReturnDetails.ModifiedBy = HelperMethods.GetLoginAdminUserId();
                        bool isUpdateReturnDetails = _returnDetailsRepository.Update(znodeReturnDetails);
                        ZnodeLogging.LogMessage("Return details updated:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { isUpdateReturnDetails = isUpdateReturnDetails });

                        //Save additional notes for return.
                        if (isUpdateReturnDetails && !string.IsNullOrEmpty(returnModel?.Notes))
                        {
                            int userId = HelperMethods.GetLoginAdminUserId();
                            SaveReturnNotes(new RMAReturnNotesModel()
                            {
                                RmaReturnDetailsId = returnModel.RmaReturnDetailsId,
                                Notes = returnModel.Notes,
                                CreatedBy = userId,
                                CreatedDate = returnModel.ModifiedDate,
                                ModifiedBy = userId,
                                ModifiedDate = returnModel.ModifiedDate
                            });
                        }
                        SaveReturnHistory(returnModel);
                        transaction.Commit();

                        if (!string.Equals(returnModel.OldReturnStatus, returnModel.ReturnStatus, StringComparison.InvariantCultureIgnoreCase))
                            SendReturnRequestEmailNotificationAsync(returnModel, true);

                        ZnodeLogging.LogMessage("Execution done", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                        return new RMAReturnModel() { ReturnNumber = znodeReturnDetails.ReturnNumber, RmaReturnDetailsId = znodeReturnDetails.RmaReturnDetailsId };
                    }
                    else
                    {
                        ZnodeLogging.LogMessage("Line Item not found", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        throw new ZnodeException(ErrorCodes.InvalidData, "Line Item not found");
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    transaction.Rollback();
                    throw new ZnodeException(ErrorCodes.ExceptionalError, ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        protected virtual decimal GetCSRDiscount(List<RMAReturnLineItemModel> returnLineItems = null, List<RMAReturnCalculateLineItemModel> returnCalculateLineItemList = null)
        {
            decimal csrDiscount = 0;
            if (HelperUtility.IsNotNull(returnLineItems) && returnLineItems.Count > 0)
                csrDiscount = returnLineItems.Where(x => x.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED).Sum(x => x.PerQuantityCSRDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault();
            else if (HelperUtility.IsNotNull(returnCalculateLineItemList) && returnCalculateLineItemList.Count > 0)
                csrDiscount = returnCalculateLineItemList.Sum(x => x.PerQuantityCSRDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault();
            return csrDiscount;
        }

        protected virtual decimal GetReturnCharges(decimal taxOnShipping = 0, List<RMAReturnLineItemModel> returnLineItems = null, List<RMAReturnCalculateLineItemModel> returnCalculateLineItemList = null)
        {
            decimal returnCharges = 0;
            if (HelperUtility.IsNotNull(returnLineItems) && returnLineItems.Count > 0)
                returnCharges = returnLineItems.Where(x => x.IsShippingReturn == false).Sum(x => x.ShippingCost - (x.PerQuantityShippingDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity))).GetValueOrDefault() + taxOnShipping;
            else if (HelperUtility.IsNotNull(returnCalculateLineItemList) && returnCalculateLineItemList.Count > 0)
                returnCharges = returnCalculateLineItemList.Where(x => x.IsShippingReturn == false).Sum(x => x.ShippingCost - (x.PerQuantityShippingDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity))).GetValueOrDefault() + taxOnShipping;
            return returnCharges;
        }
       
        protected virtual decimal GetShippingDiscount(List<RMAReturnLineItemModel> returnLineItems = null, List<RMAReturnCalculateLineItemModel> returnCalculateLineItemList = null)
        {
            decimal shippingDiscount = 0;
            if (HelperUtility.IsNotNull(returnLineItems) && returnLineItems.Count > 0)
                shippingDiscount = returnLineItems.Where(x => x.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED).Sum(x => x.PerQuantityShippingDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault();
            else if (HelperUtility.IsNotNull(returnCalculateLineItemList) && returnCalculateLineItemList.Count > 0)
                shippingDiscount = returnCalculateLineItemList.Sum(x => x.PerQuantityShippingDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault();
            return shippingDiscount;
        }

        protected virtual decimal GetShippingCost(List<RMAReturnLineItemModel> returnLineItems = null, List<RMAReturnCalculateLineItemModel> returnCalculateLineItemList = null)
        {
            decimal shippingCost = 0;
            if (HelperUtility.IsNotNull(returnLineItems) && returnLineItems.Count > 0)
                shippingCost = returnLineItems.Where(x => x.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED).Sum(x => x.PerQuantityShippingCost * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault();
            else if (HelperUtility.IsNotNull(returnCalculateLineItemList) && returnCalculateLineItemList.Count > 0)
                shippingCost = returnCalculateLineItemList.Sum(x => x.PerQuantityShippingCost * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault();
            return shippingCost;
        }

        protected virtual decimal GetTotalDiscountOnLineItem(List<RMAReturnLineItemModel> returnLineItems = null, List<RMAReturnCalculateLineItemModel> returnCalculateLineItemList = null)
        {
            decimal discountAmount = 0;
            if (HelperUtility.IsNotNull(returnLineItems) && returnLineItems.Count > 0)
                discountAmount = returnLineItems.Where(x => x.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED).Sum(x => x.PerQuantityLineItemDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault() + returnLineItems.Sum(x => x.RefundAmount).GetValueOrDefault();
            else if (HelperUtility.IsNotNull(returnCalculateLineItemList) && returnCalculateLineItemList.Count > 0)
                discountAmount = returnCalculateLineItemList.Sum(x => x.PerQuantityLineItemDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault();
            return discountAmount + GetLineItemOrderDiscountAmount(returnLineItems, returnCalculateLineItemList);
        }

        protected virtual decimal GetLineItemOrderDiscountAmount(List<RMAReturnLineItemModel> returnLineItems = null, List<RMAReturnCalculateLineItemModel> returnCalculateLineItemList = null)
        {
            decimal discountAmount = 0;
            if (HelperUtility.IsNotNull(returnLineItems) && returnLineItems.Count > 0)
                discountAmount = returnLineItems.Where(x => x.RmaReturnStateId != (int)ZnodeReturnStateEnum.REJECTED).Sum(x => x.PerQuantityOrderLevelDiscountOnLineItem * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault() + returnLineItems.Sum(x => x.RefundAmount).GetValueOrDefault();
            else if (HelperUtility.IsNotNull(returnCalculateLineItemList) && returnCalculateLineItemList.Count > 0)
                discountAmount = returnCalculateLineItemList.Sum(x => x.PerQuantityOrderLevelDiscountOnLineItem * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)).GetValueOrDefault();
            return discountAmount;
        }

        protected virtual decimal GetTotalLineItemDiscount(decimal quantity, decimal perQuantityLineItemDiscount = 0, decimal perQuantityCSRDiscount = 0, decimal partialRefund = 0)
        {
            decimal lineItemTotalDiscountAmount = 0;
            if (quantity > 0)
            {
                lineItemTotalDiscountAmount = perQuantityLineItemDiscount * quantity;
            }
            return lineItemTotalDiscountAmount;
        }

        // Get tax on shipping for return shipping.
        protected virtual decimal GetTaxOnShipping(TaxRuleModel taxRate, decimal taxcost, bool isCalculateTaxAfterDiscount, List<RMAReturnLineItemModel> returnLineItems = null, List<RMAReturnCalculateLineItemModel> returnCalculateLineItemList = null)
        {
            decimal shippingCost = 0;
            decimal totalTaxOnShipping = 0;
            if(taxcost > 0)
            {
                if (HelperUtility.IsNotNull(returnLineItems) && returnLineItems.Count > 0)
                    shippingCost = returnLineItems.Where(x => !x.IsShippingReturn).Sum(x => x.ShippingCost - (isCalculateTaxAfterDiscount ? (x.PerQuantityShippingDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)) : 0)).GetValueOrDefault();
                else if (HelperUtility.IsNotNull(returnCalculateLineItemList) && returnCalculateLineItemList.Count > 0)
                    shippingCost = returnCalculateLineItemList.Where(x => !x.IsShippingReturn).Sum(x => x.ShippingCost - (isCalculateTaxAfterDiscount ? (x.PerQuantityShippingDiscount * (x.ReturnedQuantity != null ? x.ReturnedQuantity : x.ExpectedReturnQuantity)) : 0)).GetValueOrDefault();

                totalTaxOnShipping = IsNotNull(taxRate) && taxRate.TaxShipping && shippingCost > 0 ? (shippingCost * (taxRate.TaxRate / 100)) : 0;
            }           
            return totalTaxOnShipping;
        }      

        //Expand for return details.
        protected virtual NameValueCollection GetReturnDetailsExpands()
        {
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ExpandKeys.ReturnItemList, ExpandKeys.ReturnItemList);
            expands.Add(ExpandKeys.ReturnShippingDetails, ExpandKeys.ReturnShippingDetails);
            expands.Add(ExpandKeys.ReturnBarcode, ExpandKeys.ReturnBarcode);
            return expands;
        }

        // Set Returned Order Line Item Table.
        protected virtual DataRow SetReturnedOrderLineItemTable(DataTable returnedOrderLineItemTable, RMAReturnLineItemModel lineitem, decimal extendedPrice, string cultureCode)
        {
            ZnodeOrderReceipt orderReceipt = new ZnodeOrderReceipt();
            DataRow orderlineItemDbRow = returnedOrderLineItemTable.NewRow();
            if (IsNotNull(lineitem))
            {
                orderlineItemDbRow["ReturnedName"] = lineitem.ProductName;
                orderlineItemDbRow["ReturnedSKU"] = lineitem.Sku;
                orderlineItemDbRow["ReturnedDescription"] = lineitem.Description;
                orderlineItemDbRow["ReturnedQuantity"] = Convert.ToString(double.Parse(Convert.ToString(lineitem.ExpectedReturnQuantity)));
                orderlineItemDbRow["ReturnedPrice"] = GetFormatPriceWithCurrency(lineitem.Price, cultureCode);
            }
            orderlineItemDbRow["ReturnedExtendedPrice"] = GetFormatPriceWithCurrency(extendedPrice, cultureCode);
            orderlineItemDbRow["ReturnedShortDescription"] = string.Empty;
            orderlineItemDbRow["ReturnedUOMDescription"] = string.Empty;
            return orderlineItemDbRow;
        }

        protected virtual string GenerateHtmlReceiptWithParser(string receiptHtml, RMAReturnModel returnModel)
        {
            // create returned order line Item
            ZnodeOrderReceipt orderReceipt = new ZnodeOrderReceipt();

            // create returned order line Item
            DataTable returnedOrderlineItemTable = orderReceipt.CreateReturnedOrderLineItemTable();

            // order to bind returned order amount details in data table
            DataTable returnedOrderAmountTable = SetReturnedOrderAmountData(returnModel, orderReceipt);

            //bind line item data
            BuildOrderLineItem(returnedOrderlineItemTable, returnModel);

            ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(receiptHtml);

            // Parse returned ReturnOrderLineItem
            var returnFilterData = returnedOrderlineItemTable.DefaultView;
            if (returnFilterData.Count > 0 && IsNotNull(returnFilterData))
                receiptHelper.Parse("ReturnLineItems", returnFilterData.ToTable().CreateDataReader());

            // Parse returned order amount table
            if (returnedOrderAmountTable.Rows.Count > 0 && IsNotNull(returnedOrderAmountTable))
                receiptHelper.Parse("ReturnedGrandAmountLineItems", returnedOrderAmountTable.CreateDataReader());
            //Replace the Email Template Keys, based on the passed email template parameters.

            // Return the HTML output
            return receiptHelper.Output;
        }

        // To set returned order amount data
        public virtual DataTable SetReturnedOrderAmountData(RMAReturnModel returnModel, ZnodeOrderReceipt orderReceipt)
        {
            // Create order amount table
            DataTable orderAmountTable = orderReceipt.CreateReturnedOrderAmountTable();
            orderReceipt.BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleSubTotal")) ? Admin_Resources.LabelSubTotal : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleSubTotal").ToString(), returnModel.SubTotal, orderAmountTable, returnModel.CultureCode);
            orderReceipt.BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalShippingCost")) ? $"{Admin_Resources.LabelShipping}" : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalShippingCost").ToString(), returnModel.ReturnShippingCost ?? 0, orderAmountTable, returnModel.CultureCode);
            orderReceipt.BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalTaxCost")) ? Admin_Resources.LabelTax : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalTaxCost").ToString(), returnModel.ReturnTaxCost, orderAmountTable, returnModel.CultureCode);
            orderReceipt.BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleDiscountAmount")) ? Admin_Resources.LabelDiscountAmount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleDiscountAmount").ToString(), returnModel.DiscountAmount, orderAmountTable, returnModel.CultureCode);
            orderReceipt.BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleCSRDiscountAmount")) ? Admin_Resources.LabelCSRDiscount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleCSRDiscountAmount").ToString(), returnModel.CSRDiscount, orderAmountTable, returnModel.CultureCode);
            orderReceipt.BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShippingDiscountAmount")) ? Admin_Resources.LabelShippingDiscount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShippingDiscountAmount").ToString(), returnModel.ReturnShippingDiscount, orderAmountTable, returnModel.CultureCode);
            orderReceipt.BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnReturnCharges")) ? Admin_Resources.LabelReturnCharges : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnReturnCharges").ToString(), returnModel.ReturnCharges, orderAmountTable, returnModel.CultureCode);
            return orderAmountTable;
        }

        // Builds the order line item table.
        protected virtual void BuildOrderLineItem(DataTable returnedOrderlineItemTable, RMAReturnModel returnModel)
        {
            // Returned line item
            foreach (RMAReturnLineItemModel lineitem in returnModel?.ReturnLineItems ?? new List<RMAReturnLineItemModel>())
            {
                if (returnedOrderlineItemTable != null)
                    returnedOrderlineItemTable.Rows.Add(SetReturnedOrderLineItemTable(returnedOrderlineItemTable, lineitem, lineitem.Price * lineitem.ExpectedReturnQuantity, returnModel.CultureCode));
            }
        }

        //to get amount with currency symbol
        protected virtual string GetFormatPriceWithCurrency(decimal priceValue, string cultureCode, string uom = "")
        {
            return ZnodeCurrencyManager.FormatPriceWithCurrency(priceValue, cultureCode, uom);
        }

        //Get Store Logo Path.
        protected virtual string GetStoreLogoPath(int portalId)
        {
            ZnodeLogging.LogMessage("portalId to generate query: ", string.Empty, TraceLevel.Verbose, portalId);
            IZnodeRepository<ZnodePublishWebstoreEntity> _webstoreVersionEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);
            string storeLogo = (from webstoreVersionEntity in _webstoreVersionEntity.Table
                                .Where(x => x.PortalId == portalId)
                                select webstoreVersionEntity.WebsiteLogo).FirstOrDefault();

            MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configurationModel);
            string thumbnailPath = $"{serverPath}{configurationModel.ThumbnailFolderName}";
            return $"{thumbnailPath}/{storeLogo}";
        }
        #endregion

        #endregion
    }
}
