using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Engine.Taxes;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Fulfillment;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Entities = Znode.Libraries.ECommerce.Entities;
using System.Threading.Tasks;
using System.Web;

namespace Znode.Engine.Services
{
    public class OrderService : BaseService, IOrderService
    {
        #region Private Variables

        protected readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        protected readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailsRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderLineItem> _orderLineItemRepository;
        private readonly IZnodeRepository<ZnodeOmsPaymentRefund> _omsPaymentRefundRepository;
        private readonly IZnodeRepository<ZnodeOmsNote> _omsNoteRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderShipment> _orderShipmentRepository;
        private readonly IZnodeRepository<ZnodeOmsOrder> _omsOrderRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        protected readonly IZnodeOrderHelper orderHelper;
        private readonly IZnodeRepository<ZnodeOmsOrderState> _omsOrderStateRepository;
        private readonly IZnodeRepository<ZnodeOmsHistory> _orderHistoryRepository;
        private readonly IZnodeRepository<ZnodeOmsTaxOrderLineDetail> _omsTaxOrderLineDetailRepository;
        private readonly IZnodeRepository<ZnodeShipping> _shippingRepository;
        private readonly IZnodeRepository<ZnodeRmaConfiguration> _rmaConfigurationRepository;
        private readonly IZnodeRepository<ZnodeShippingType> _shippingTypeRepository;
        private readonly IZnodeRepository<ZnodeOmsCustomerShipping> _omsCustomerShippingRepository;
        private readonly IZnodeRepository<ZnodeOmsDownloadableProductKey> _omsDownloadableProductKey;
        private readonly IZnodeRepository<ZnodePimDownloadableProductKey> _pimDownloadableProductKey;
        private readonly IZnodeRepository<ZnodePimDownloadableProduct> _pimDownloadableProduct;
        private readonly IZnodeRepository<ZnodePortalPaymentSetting> _portalPaymentSettingRepository;
        private readonly IZnodeRepository<ZnodeOmsQuote> _znodeOmsQuote;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository;
        private int _previousOrderStateId = 0;
        private readonly string InProgressOrderState = "IN PROGRESS";
        protected readonly IUserService _userService;
        protected readonly IPaymentSettingService _paymentSettingService;
        private readonly IZnodeRepository<ZnodeOmsCookieMapping> _cookieMappingRepository;
        private readonly IShoppingCartMap _shoppingCartMap;
        private readonly IZnodeRepository<ZnodeOmsQuoteComment> _omsQuoteComment;
        private readonly IZnodeRepository<ZnodeOMSQuoteApproval> _omsQuoteApproval;
        private readonly IOrderInventoryManageHelper _orderInventoryManageHelper;
        private readonly IZnodeRepository<ZnodeUserAddress> _userAddress;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<ZnodeOmsTaxOrderDetail> _omsTaxOrderDetails;
        private readonly IZnodeRepository<ZnodeOmsTaxOrderSummary> _omsTaxOrderSummary;
        protected readonly IZnodeRepository<ZnodeUserApprover> _userApprovers;
        #endregion Private Variables

        #region Constructor

        public OrderService()
        {
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _orderDetailsRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            _omsPaymentRefundRepository = new ZnodeRepository<ZnodeOmsPaymentRefund>();
            _orderLineItemRepository = new ZnodeRepository<ZnodeOmsOrderLineItem>();
            _omsNoteRepository = new ZnodeRepository<ZnodeOmsNote>();
            orderHelper = ZnodeDependencyResolver.GetService<IZnodeOrderHelper>();
            _orderShipmentRepository = new ZnodeRepository<ZnodeOmsOrderShipment>();
            _omsOrderRepository = new ZnodeRepository<ZnodeOmsOrder>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _omsOrderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            _orderHistoryRepository = new ZnodeRepository<ZnodeOmsHistory>();
            _omsTaxOrderLineDetailRepository = new ZnodeRepository<ZnodeOmsTaxOrderLineDetail>();
            _shippingRepository = new ZnodeRepository<ZnodeShipping>();
            _rmaConfigurationRepository = new ZnodeRepository<ZnodeRmaConfiguration>();
            _shippingTypeRepository = new ZnodeRepository<ZnodeShippingType>();
            _omsCustomerShippingRepository = new ZnodeRepository<ZnodeOmsCustomerShipping>();
            _omsDownloadableProductKey = new ZnodeRepository<ZnodeOmsDownloadableProductKey>();
            _pimDownloadableProductKey = new ZnodeRepository<ZnodePimDownloadableProductKey>();
            _pimDownloadableProduct = new ZnodeRepository<ZnodePimDownloadableProduct>();
            _portalPaymentSettingRepository = new ZnodeRepository<ZnodePortalPaymentSetting>();
            _znodeOmsQuote = new ZnodeRepository<ZnodeOmsQuote>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _userService = GetService<IUserService>();
            _paymentSettingService = GetService<IPaymentSettingService>();
            _cookieMappingRepository = new ZnodeRepository<ZnodeOmsCookieMapping>();
            _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            _shoppingCartMap = GetService<IShoppingCartMap>();
            _omsQuoteComment = new ZnodeRepository<ZnodeOmsQuoteComment>();
            _omsQuoteApproval = new ZnodeRepository<ZnodeOMSQuoteApproval>();
            _orderInventoryManageHelper = GetService<IOrderInventoryManageHelper>();
            _userAddress = new ZnodeRepository<ZnodeUserAddress>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _omsTaxOrderDetails = new ZnodeRepository<ZnodeOmsTaxOrderDetail>();
            _omsTaxOrderSummary =  new ZnodeRepository<ZnodeOmsTaxOrderSummary>();
            _userApprovers = new ZnodeRepository<ZnodeUserApprover>();
        }

        #endregion Constructor

        #region Public Methods

        //Get order list.
        public virtual OrdersListModel GetOrderList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);
            //Replace sort key name.
            if (IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            int userId = 0;
            GetUserIdFromFilters(filters, ref userId);
            //Add date time value in filter collection against filter column name Order date.
            filters = ServiceHelper.AddDateTimeValueInFilterByName(filters, Constants.FilterKeys.OrderDate);

            int fromAdmin = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, Znode.Libraries.ECommerce.Utilities.FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase))?.Item3);
            filters?.RemoveAll(x => string.Equals(x.FilterName, Znode.Libraries.ECommerce.Utilities.FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase));
            ReplaceFilterKeys(ref filters);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            IList<OrderModel> list = GetOrderList(pageListModel, userId, fromAdmin);
            ZnodeLogging.LogMessage("Order list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, list?.Count);
            OrdersListModel orderListModel = new OrdersListModel() { Orders = list?.ToList() };

            GetCustomerName(userId, orderListModel);
            orderListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderListModel;
        }

        //Get order list by sp.
        public virtual IList<OrderModel> GetOrderList(PageListModel pageListModel, int userId, int fromAdmin)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters to get order list:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { pageListModel = pageListModel?.ToDebugString(), userId = userId, fromAdmin = fromAdmin });
            IZnodeViewRepository<OrderModel> objStoredProc = new ZnodeViewRepository<OrderModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsFromAdmin", fromAdmin, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@SalesRepUserId", Libraries.Data.Helpers.HelperMethods.GetSalesRepUserId(), ParameterDirection.Input, DbType.Int32);

            return objStoredProc.ExecuteStoredProcedureList("Znode_GetOmsOrderDetail @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT,@UserId,@IsFromAdmin,@SalesRepUserId", 4, out pageListModel.TotalRowCount);
        }

        //Get Order Details based on the Expand parameter
        public virtual IList<OrderModel> GetOrderListWithExpands(NameValueCollection expands, PageListModel pageListModel, int userId, int fromAdmin, string storedProcedureName)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters to get order list: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { pageListModel = pageListModel, userId = userId, fromAdmin = fromAdmin, storedProcedureName = storedProcedureName });
            string ExpandParameter = IsNull(expands) ? string.Empty : string.Join(",", GetExpands(expands).ToArray());
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@UserId", userId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@IsFromAdmin", fromAdmin, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Expands", ExpandParameter, ParameterDirection.Input, SqlDbType.NVarChar);
            DateTime datetimeStart = DateTime.UtcNow;
            DataSet dataset = objStoredProc.GetSPResultInDataSet(storedProcedureName);
            DateTime datetimeStop = DateTime.UtcNow;
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.ExecutionTimeStoredProcedure, Convert.ToString((datetimeStop - datetimeStart).TotalSeconds)), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<OrderModel> entities = new List<OrderModel>();
            SetOrderListWithExpands(dataset, entities, pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return entities;
        }

        //Get group order list.
        public virtual OrdersListModel GetGroupOrderList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);
            //Replace sort key name.
            if (HelperUtility.IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            int userId = 0;
            GetUserIdFromFilters(filters, ref userId);

            int fromAdmin = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, Znode.Libraries.ECommerce.Utilities.FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase))?.Item3);
            filters?.RemoveAll(x => string.Equals(x.FilterName, Znode.Libraries.ECommerce.Utilities.FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase));
            ReplaceFilterKeys(ref filters);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("Where condition:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<OrderModel> list = GetOrderListWithExpands(expands, pageListModel, userId, fromAdmin, "Znode_GetOmsGroupOrderListExpandDetail");
            OrdersListModel orderListModel = new OrdersListModel() { Orders = list?.ToList() };
            ZnodeLogging.LogMessage("Order list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, list?.Count());
            GetCustomerName(userId, orderListModel);

            orderListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderListModel;
        }

        //Get Order Note List.
        public virtual OrderNotesListModel GetOrderNoteList(int omsOrderId, int omsQuoteId)
            => new OrderNotesListModel() { OrderNotes = GetOrderNoteDetails(omsOrderId, omsQuoteId) };

        //Create new order.
        public virtual OrderModel CreateOrder(ShoppingCartModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                //Get the IPAddress only when the order is from webstore.
                if (model.IsOrderFromWebstore)
                {
                    model.IpAddress = GetIpAddress();
                }

                bool isCardNumber = checkCardNumber(model);

                if (!isCardNumber)
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCardNumber);
                }

                if(!string.IsNullOrEmpty(model.CreditCardNumber))
                {
                    bool CCNumberValidation = model.CreditCardNumber.All(Char.IsDigit);
                    if(!CCNumberValidation)
                    {
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCardNumberMsg);
                    }
                }

                if (!string.IsNullOrEmpty(model.CreditCardNumber))
                {
                    if (model.CreditCardNumber.Length > 4 || model.CreditCardNumber.Length < 4)
                    {
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCardNumberMsg);
                    }
                }

                bool isCardType = CheckIsCardType(model);

                if (!isCardType)
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.CardTypeErrorMessage);
                }

                bool isCardExpDate = checkCardExpDate(model);

                if (!isCardExpDate)
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ValidErrorCardExpDate);
                }

                if (IsNull(model))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorShoppingCartModelNull);

                if (IsAllowedTerritories(model))
                    throw new ZnodeException(ErrorCodes.AllowedTerritories, Admin_Resources.AllowedTerritoriesError);

                for (int Item = 0; Item < model.ShoppingCartItems.Count; Item++)
                {
                    if(model.ShoppingCartItems[Item].Quantity > model.ShoppingCartItems[Item].MaxQuantity || model.ShoppingCartItems[Item].Quantity < model.ShoppingCartItems[Item].MinQuantity)
                    {
                        throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.WarningSelectedQuantityWithProductName, Convert.ToInt32(model.ShoppingCartItems[Item].MinQuantity), Convert.ToInt32(model.ShoppingCartItems[Item].MaxQuantity), model.ShoppingCartItems[Item].ProductName));
                    }

                }

                if (model.Payment.PaymentName == ZnodeConstant.PurchaseOrder)
                {
                    if (string.IsNullOrEmpty(model.PurchaseOrderNumber))
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.rfvPurchaseOrderNumber);
                }
                SubmitOrderModel submitOrderModel = new SubmitOrderModel();

                ParameterModel portalId = new ParameterModel() { Ids = Convert.ToString(model.PortalId) };

                //Get generated unique order number on basis of current date.           
                submitOrderModel.OrderNumber = !string.IsNullOrEmpty(model.OrderNumber) ? model.OrderNumber : GenerateOrderNumber(submitOrderModel, portalId);
                ZnodeLogging.LogMessage("Generated order number: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { GeneratedOrderNumber = submitOrderModel?.OrderNumber });

                int omsOrderId = _omsOrderRepository.Table.Where(x => x.OrderNumber == submitOrderModel.OrderNumber).Select(x => x.OmsOrderId).FirstOrDefault();

                if (omsOrderId>0)
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorDuplicateOrderNumber);
                }

                return SaveOrder(model, submitOrderModel);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in CreateOrder method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get the ip address when user is not placing the order through impersonation.
        public virtual string GetIpAddress()
        {
            string ipAddress = string.Empty;
            string impersonationPortalId = GetHeaderValue("Znode-ImpersonationPortalId");
            string impersonationCSRId = GetHeaderValue("Znode-ImpersonationCSRId");
            if (string.IsNullOrEmpty(impersonationPortalId) && string.IsNullOrEmpty(impersonationCSRId))
            {
                ipAddress = GetHeaderValue("User-IpAddress");
            }
            ZnodeLogging.LogMessage($"The IPAddress of the user is - {ipAddress}", "IPAddress", TraceLevel.Info, null);
            return ipAddress;
        }

        //To generate unique order number on basis of current date.
        public virtual string GenerateOrderNumber(SubmitOrderModel submitOrderModel, ParameterModel portalId = null)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                var _erpInc = new ERPInitializer<SubmitOrderModel>(submitOrderModel, "GetOrderNumber");
                if (string.IsNullOrEmpty(Convert.ToString(_erpInc.Result)))
                {
                    string orderNumber = string.Empty;
                    if (!string.IsNullOrEmpty(ZnodeConfigManager.SiteConfig.StoreName))
                        orderNumber = ZnodeConfigManager.SiteConfig.StoreName.Trim().Length > 2 ? ZnodeConfigManager.SiteConfig.StoreName.Substring(0, 2) : ZnodeConfigManager.SiteConfig.StoreName.Substring(0, 1);

                    string randomSuffix = GetRandomCharacters();

                    DateTime date = DateTime.Now;
                    // we have removed '-fff' from the date string as order number field length not exceeds the limit.
                    // This change in made for the ticket ZPD-13806
                    String strDate = date.ToString("yyMMdd-HHmmss");
                    orderNumber += $"-{strDate}-{randomSuffix}";
                    return orderNumber.ToUpper();
                }
                else
                    return Convert.ToString(_erpInc?.Result);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GenerateOrderNumber method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Update existing order.
        public virtual OrderModel UpdateOrder(OrderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorOrderModelNull);

            if (IsNull(model.OmsOrderId) || model?.OmsOrderId == 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorOrderIdNullOrZero);

            if (IsNull(model?.ShoppingCartModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorShoppingCartModelNull);

            if (IsAllowedTerritories(model.ShoppingCartModel))
                throw new ZnodeException(ErrorCodes.AllowedTerritories, Admin_Resources.AllowedTerritoriesError);

            ZnodeLogging.LogMessage("OrderModel with OmsOrderDetailsId and OmsOrderId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderDetailsId = model?.OmsOrderDetailsId, OmsOrderId = model?.OmsOrderId });
            model.ModifiedBy = HelperMethods.GetLoginUserId();
            //if there is no change in order data then no need to update order
            if (!IsOrderDataUpdated(model))
                return model;

            bool isTaxCostUpdated = CheckIsTaxUpdated(model);

            //to check order state isEditable if order is not editable then update order state
            if (!IsOrderEditable(model) && !IsReturnAllItems(model))
            {
                SaveHistoryAndUpdateOrderState(model, true);
                CancelOrderAmount(model);

                if (!string.IsNullOrEmpty(model.ShoppingCartModel.ShoppingCartItems?.FirstOrDefault()?.TaxTransactionNumber))
                    ZnodeLogging.LogMessage("Order State while updating order:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderState = model?.OrderState });
                if (string.Equals(model.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    model.ShoppingCartModel.OrderStatus = model.OrderState;
                    IZnodeCheckout checkout = GetZnodeCheeckoutModel(model);
                    checkout.ShoppingCart.CancelTaxOrderRequest(model.ShoppingCartModel);
                }
                return model;
            }

            if (IsExistingOrderUpdated(model))
            {
                SaveHistoryAndUpdateOrderState(model, true);
                if (string.Equals(model.OrderState, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    IZnodeCheckout checkout = GetZnodeCheeckoutModel(model);
                    checkout.ShoppingCart.SubmitTax(isTaxCostUpdated);
                }
                return model;
            }

            if (IsReturnShipping(model))
            {
                SaveHistoryAndUpdateOrderState(model, true);
                ReturnShippingAmount(model);
                return model;
            }
            if (IsReturnAllItems(model))
            {
                string omsOrderLineItemsIds = GetOrderLineItemsIds(model);
                //This flag is set to 1 if all the items are returned.
                int isRevertAll = 1;
                if (!RevertOrderInventory(model.OmsOrderId, model.UserId, omsOrderLineItemsIds.TrimStart(','), isRevertAll))
                    throw new ZnodeException(ErrorCodes.InternalItemNotUpdated, Admin_Resources.ErrorRevertOrderInventory);
                //Update downloadable product key status
                UpdatePimDownloadableReturnProductKey(model);
                SetOverDueAmountForReturnAllItem(model);
                SaveHistoryAndUpdateOrderState(model);
                //Bind the ReturnItemLines in shopping cart model.
                model.ShoppingCartModel.ReturnItemList = model.ReturnItemList?.ReturnItemList ?? new List<ReturnOrderLineItemModel>();

                ZnodeLogging.LogMessage("Return Item List count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ReturnItemListCount = model?.ShoppingCartModel?.ReturnItemList?.Count });

                //To preserve the order status in ShoppingCartModel.
                string oldOrderState = model.ShoppingCartModel.OrderStatus;

                //Call to ReturnOrderLineItem method of respective tax class.
                //Updated OrderStatus property of ShoppingCartModel with value CANCELLED to use in case of Vertex tax class.
                model.ShoppingCartModel.OrderStatus = ZnodeOrderStatusEnum.CANCELED.ToString();
                IZnodeTaxManager taxManager = GetService<IZnodeTaxManager>(new ZnodeTypedParameter(typeof(ShoppingCartModel), model.ShoppingCartModel));
                taxManager.ReturnOrderLineItem(model.ShoppingCartModel);

                //Assigned back the old value of OrderStatus property after tax operations.
                model.ShoppingCartModel.OrderStatus = oldOrderState;

                return model;
            }

            ZnodeLogging.LogMessage(string.Format(Admin_Resources.UpdateOrderForOrderId, model.OmsOrderId), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Manage those items in the order which are not got modified its quantity greater than its previous quantity.
            ManageExistingOrderLineItems(model);

            if (!RevertOrderInventory(model.OmsOrderId, model.UserId))
                throw new ZnodeException(ErrorCodes.InternalItemNotUpdated, Admin_Resources.ErrorRevertOrderInventory);

            //Update downloadable product key status
            UpdatePimDownloadableReturnProductKey(model);

            MapOrderModelToShoppingCart(model);

            SubmitOrderModel updateModel = SetUpdateOrderData(model);

            if (!model.IsTaxCostEdited)
                CalculateReturnItemTax(model.PortalId, model, updateModel.ReturnOrderLineItems?.ReturnItemList);

            SetCustomerShipping(model);

            MapOrderDataToShoppingCartModel(model);
            model.ShoppingCartModel.IsEmailSend = model.IsEmailSend;
            model.ShoppingCartModel.IsCallLiveShipping = true;
            OrderModel updatedmodel = SaveOrder(model.ShoppingCartModel, updateModel, isTaxCostUpdated);

            UpdateProductKey(model.ShoppingCartModel, updatedmodel);

            model.OverDueAmount = updatedmodel.OverDueAmount;

            SaveHistoryAndUpdateOrderState(model);
            return updatedmodel;
        }

        //This method is used to set the data of order model to shoppingcart model
        protected virtual void MapOrderDataToShoppingCartModel(OrderModel model)
        {
            model.ShoppingCartModel.IsEmailSend = model.IsEmailSend;
            model.ShoppingCartModel.IsFromManageOrder = model.IsFromManageOrder;
            model.ShoppingCartModel.IsFromAdminOrder = model.IsFromAdminOrder;
            model.ShoppingCartModel.IpAddress = model.IpAddress;
            model.ShoppingCartModel.Custom1 = model.Custom1;
            model.ShoppingCartModel.Custom2 = model.Custom2;
            model.ShoppingCartModel.Custom3 = model.Custom3;
            model.ShoppingCartModel.Custom4 = model.Custom4;
            model.ShoppingCartModel.Custom5 = model.Custom5;
        }
        //To get the order details for payment
        public virtual OrderPaymentModel GetOrderDetailsForPayment(OrderPaymentCreateModel orderPaymentCreateModel)
        {
            try
            {
                OrderModel model = GetOrderModelInstance(orderPaymentCreateModel);

                if (IsNull(model))
                    throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorOrderModelNull);

                if (IsNull(model.OmsOrderId) || model?.OmsOrderId == 0)
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorOrderIdNullOrZero);

                if (IsNull(model?.ShoppingCartModel))
                    throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorShoppingCartModelNull);

                if (orderPaymentCreateModel.IsAllowedTerritories)
                    throw new ZnodeException(ErrorCodes.AllowedTerritories, Admin_Resources.AllowedTerritoriesError);

                OrderPaymentModel orderPaymentModel = new OrderPaymentModel();

                bool isReturnAllItems = IsReturnAllItems(model, false);

                //to check order state isEditable if order is not editable then update order state
                if (!IsOrderEditable(model) && !isReturnAllItems)
                {
                    CancelOrderAmount(model);
                    orderPaymentModel.Total = model.Total;
                    if (ExistUpdateHistory(ZnodeConstant.OrderUpdatedStatus, model))
                        orderPaymentModel.OrderState = model?.OrderHistory[ZnodeConstant.OrderUpdatedStatus];
                    return orderPaymentModel;
                }

                if (IsExistingOrderUpdated(model))
                    return orderPaymentModel;

                if (IsReturnShipping(model))
                {
                    ReturnShippingAmount(model, false);
                    orderPaymentModel.OverDueAmount = model.OverDueAmount;
                    return orderPaymentModel;
                }

                if (isReturnAllItems)
                {
                    SetOverDueAmountForReturnAllItem(model);
                    orderPaymentModel.OverDueAmount = model.OverDueAmount;
                    return orderPaymentModel;
                }

                orderPaymentModel.OverDueAmount = model.ShoppingCartModel.OverDueAmount.GetValueOrDefault();
                return orderPaymentModel;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetOrderDetailsForPayment method for OrderId " + orderPaymentCreateModel?.OmsOrderId + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //To get the OrderModel instance using OrderPaymentCreateModel.
        protected virtual OrderModel GetOrderModelInstance(OrderPaymentCreateModel orderPaymentCreateModel)
        {
            OrderModel orderModel = orderPaymentCreateModel.ToModel<OrderModel, OrderPaymentCreateModel>();

            if (IsNotNull(orderModel))
            {
                orderModel.BillingAddress = new AddressModel() { AddressId = orderPaymentCreateModel.BillingAddressId };
                orderModel.ShippingAddress = new AddressModel() { AddressId = orderPaymentCreateModel.ShippingAddressId };
                orderModel.ShoppingCartModel = new ShoppingCartModel() { OverDueAmount = orderPaymentCreateModel.ShoppingCartOverDueAmount };

                for (int i = 0; i < orderPaymentCreateModel.ShoppingCartItemsCount; i++)
                    orderModel.ShoppingCartModel.ShoppingCartItems.Add(new ShoppingCartItemModel());
            }

            return orderModel;
        }

        /// <summary>
        /// This method will set backordering true for those items in the order which are not got modified its quantity greater than its previous quantity.
        /// </summary>
        /// <param name="model">OrderModel</param>
        protected virtual void ManageExistingOrderLineItems(OrderModel model)
        {
            //Get the list of items which was already in order
            List<ShoppingCartItemModel> existingShoppingCartItems = model.ShoppingCartModel.ShoppingCartItems.Where(x => x.OmsOrderLineItemsId > 0).ToList();

            //Get list of items which are existing items and not updated its quantity greater than previous quantity
            List<ShoppingCartItemModel> backOrderedItems = new List<ShoppingCartItemModel>();

            foreach (ShoppingCartItemModel item in existingShoppingCartItems)
            {
                int? relationType = item.OrderLineItemRelationshipTypeId; //Get relationship type Id
                decimal? prevQuantity = GetPreviousQuantity(item, relationType); //Get the previous quantity of line item(sku)

                if (item.Quantity <= prevQuantity)
                {
                    backOrderedItems.Add(item);
                }
            }

            if (backOrderedItems.Count > 0)
            {
                _orderInventoryManageHelper.SetBackOrderingForShoppingCart(backOrderedItems, model);
            }
        }

        /// <summary>
        /// Get Previous quantity of an item in before editing it.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="relationType"></param>
        /// <returns></returns>
        protected decimal? GetPreviousQuantity(ShoppingCartItemModel item, int? relationType)
        {
            decimal? prevQuantity;
            if (relationType == (int)ZnodeCartItemRelationshipTypeEnum.Bundles)
                prevQuantity = _orderLineItemRepository.Table.Where(w => w.ParentOmsOrderLineItemsId == item.OmsOrderLineItemsId)?.FirstOrDefault()?.Quantity;
            else
                prevQuantity = _orderLineItemRepository.Table.Where(w => w.OmsOrderLineItemsId == item.OmsOrderLineItemsId)?.FirstOrDefault()?.Quantity;

            return prevQuantity;
        }

        public virtual string GetOrderLineItemsIds(OrderModel model)
        {
            string omsOrderLineItemsIds = "";
            model.ReturnItemList.ReturnItemList.ForEach(returnItem =>
            {
                string omsOrderLineItemsId;
                omsOrderLineItemsId = model.OrderLineItems.Where(orderLineItem => orderLineItem.OmsOrderLineItemsId == returnItem.OmsOrderLineItemsId
                                                           && !string.Equals(orderLineItem.OrderLineItemState, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase) && returnItem.RmaReturnLineItemStatus == null)
                                                            .Select(x => x.OmsOrderLineItemsId).FirstOrDefault().ToString();

                omsOrderLineItemsIds = omsOrderLineItemsIds + "," + omsOrderLineItemsId == "0" ? "" : omsOrderLineItemsId;
            });
            ZnodeLogging.LogMessage("Order line item Ids:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsOrderLineItemsIds = omsOrderLineItemsIds });
            return omsOrderLineItemsIds;
        }

        //Get order details by order id.
        public virtual OrderModel GetOrderById(int orderId, FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //This method check the access of manage screen for sales rep user
            ValidateAccessForSalesRepUser(ZnodeConstant.Order, orderId);
            OrderModel orderModel = GetOrderByOrderDetails(orderId, string.Empty, filters, expands);

            if (orderModel == null)
                return null;

            if (IsNotNull(orderModel?.OrderLineItems) && orderModel.OrderLineItems.Count > 0)
                orderModel.OrderLineItems = orderHelper.FormalizeOrderLineItems(orderModel);

            //Set Import Duty Value
            SetImportDuty(orderModel);
            IAccountQuoteService _accountQuoteService = GetService<IAccountQuoteService>();
            orderModel.QuoteApproverComments = _accountQuoteService.GetApproverComments(orderModel.OmsQuoteId);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderModel;
        }


        //Get order details by order id.
        public virtual OrderModel GetOrderByOrderNumber(string orderNumber, FilterCollection filters, NameValueCollection expands)
        {
            return GetOrderByOrderDetails(0, orderNumber, filters, expands);
        }

        //to create new customer
        public virtual UserAddressModel CreateNewCustomer(UserAddressModel userAddressModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(userAddressModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorUserAddressModelNull);

            if (IsNull(userAddressModel.BillingAddress) && IsNull(userAddressModel.ShippingAddress))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorBillingAndShippingAddressNull);

            ZnodeUser user = new ZnodeUser { Email = userAddressModel.Email };

            //If user Id is less than 1 then create new guest user otherwise add shipping/billing address for user.
            if (userAddressModel.UserId < 1)
                user = CreateNewRegisteredUser(user, userAddressModel);
            else
                AssignShippingBillingAddressUserId(userAddressModel, userAddressModel.UserId);

            //Insert/Update shippin/billing address of user.
            List<ZnodeAddress> userAddress = InsertUpdateUserAddress(userAddressModel);

            //Set Default Shipping/Billing Address of user.
            userAddressModel.BillingAddress = userAddress.Where(x => x.IsDefaultBilling)?.FirstOrDefault()?.ToModel<AddressModel>();
            userAddressModel.ShippingAddress = userAddress.Where(x => x.IsDefaultShipping)?.FirstOrDefault()?.ToModel<AddressModel>();
            ZnodeLogging.LogMessage("BillingAddressId and ShippingAddressId of UserAddressModel: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddressId = userAddressModel?.BillingAddress?.AddressId, ShippingAddressId = userAddressModel?.ShippingAddress?.AddressId });
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddressModel;
        }

        //Get order invoice details.
        public virtual OrdersListModel GetOrderDetailsForInvoice(ParameterModel orderIds, NameValueCollection expands, FilterCollection filters = null)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                if (string.IsNullOrEmpty(orderIds?.Ids))
                    return null;

                if (IsNull(filters))
                    filters = new FilterCollection();

                filters.Add(new FilterTuple(ZnodeOmsOrderDetailEnum.OmsOrderId.ToString(), FilterOperators.In, orderIds.Ids));
                filters.Add(new FilterTuple(ZnodeOmsOrderDetailEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));

                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("WhereClause to get orderList:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { WhereClause = whereClauseModel?.WhereClause });
                List<ZnodeOmsOrderDetail> orderList = _orderDetailsRepository.GetEntityList(whereClauseModel?.WhereClause, new List<string>() { ZnodeOmsOrderDetailEnum.ZnodeOmsOrder.ToString(), ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString(), "ZnodeOmsTaxOrderSummaries" }).ToList();
                ZnodeLogging.LogMessage("orderList Count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderListCount = orderList?.Count });

                OrdersListModel listModel = new OrdersListModel() { Orders = new List<OrderModel>() };

                IPortalService _portalService = GetService<IPortalService>();
                foreach (ZnodeOmsOrderDetail order in orderList)
                {
                    OrderModel orderModel = new OrderModel();

                    //Map order detail object to OrderModel object.
                    orderModel = order.ToModel<OrderModel>();

                    orderModel.BillingAddress = order?.ToModel<AddressModel>();
                    string BillingcompanyName = _addressRepository.Table.Where(x => x.AddressId == orderModel.BillingAddress.AddressId)?.FirstOrDefault()?.CompanyName;
                    orderModel.BillingAddress.CompanyName = BillingcompanyName;
                    //Get order details by using expands.
                    GetExpands(expands, orderModel);

                //Get ordered shipping address.
                foreach (OrderLineItemModel lineItem in orderModel.OrderLineItems)
                {
                    lineItem.ShippingAddressHtml = GetOrderShipmentAddress(lineItem.ZnodeOmsOrderShipment);
                        //get personalize attributes by omsorderlineitemid
                        lineItem.PersonaliseValueList = orderHelper.GetPersonalizedValueOrderLineItem(lineItem.OmsOrderLineItemsId, false, 0);
                        lineItem.PersonaliseValuesDetail = orderHelper.GetPersonalizedAttributeLineItemDetails(lineItem.PersonaliseValueList, string.Empty);
                    }
                orderModel.OrderNumber = orderList?.Select(x => x.ZnodeOmsOrder).Where(x => x.OmsOrderId == order.OmsOrderId)?.Select(x => x.OrderNumber)?.FirstOrDefault();

                    List<OrderLineItemModel> addonOrderLineItems = new List<OrderLineItemModel>();

                    addonOrderLineItems = orderModel.OrderLineItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns).ToList();
                    //Create Single Order Line Item if order is having group product.
                    orderModel.OrderLineItems = CreateSingleOrderLineItem(orderModel, true).Where(x => x.ParentOmsOrderLineItemsId != null).ToList();


                    if (addonOrderLineItems.Count > 0)
                        AddonPrice(orderModel, addonOrderLineItems);

                    //Set order related discounts.
                    SetOrderDiscount(orderModel);
                    //Get Ordered billing address.

                    orderModel.BillingAddressHtml = GetOrderBillingAddress(orderModel);

                    //Get portal information
                    PortalModel portal = _portalService.GetPortal(order.PortalId, null);
                    orderModel.CustomerServiceEmail = portal?.CustomerServiceEmail;
                    orderModel.CustomerServicePhoneNumber = portal?.CustomerServicePhoneNumber;
                    orderModel.TaxSummaryList = MapTaxSummaryDetails(order.ZnodeOmsTaxOrderSummaries.ToList());

                    listModel.Orders.Add(orderModel);
                }

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return listModel;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetOrderDetailsForInvoice method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //This method use to add the price of addon to main product.
        public virtual void AddonPrice(OrderModel orderModel, List<OrderLineItemModel> addonOrderLineItems)
        {
            foreach (var item in orderModel.OrderLineItems)
            {
                foreach (var addonItem in addonOrderLineItems)
                {
                    if (item.OmsOrderLineItemsId == addonItem.ParentOmsOrderLineItemsId && item.OmsOrderDetailsId == addonItem.OmsOrderDetailsId)
                        item.Price += addonItem.Price;
                }
            }
        }

        //Update Order Payment Status
        public virtual bool UpdateOrderPaymentStatus(int orderId, string paymentStatus, int? paymentStateId = null, int createdBy = 0, int modifiedBy = 0)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (orderId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.OrderIdLessThanOne);

            if (string.IsNullOrEmpty(paymentStatus))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorPaymentStatusNull);

            if (IsNull(paymentStateId))
                paymentStateId = new ZnodeRepository<ZnodeOmsPaymentState>().Table.Where(x => x.Name == paymentStatus)?.FirstOrDefault()?.OmsPaymentStateId;

            if (IsNull(paymentStateId) || paymentStateId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidPaymentState);

            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, paymentStatus = paymentStatus });
            ZnodeOmsOrderDetail orderDetails = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == orderId && x.IsActive)?.FirstOrDefault();

            if (IsNotNull(orderDetails))
            {
                orderDetails.OmsPaymentStateId = paymentStateId;
                orderDetails.CreatedBy = createdBy;
                orderDetails.ModifiedBy = modifiedBy;
                return _orderDetailsRepository.Update(orderDetails);
            }
            return false;
        }

        //Update Order Tracking Number         
        public virtual bool UpdateTrackingNumber(int orderId, string trackingNumber)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (orderId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.OrderIdLessThanOne);

            if (string.IsNullOrEmpty(trackingNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorTrackingNumberNull);

            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, trackingNumber = trackingNumber });
            ZnodeOmsOrderDetail orderDetails = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == orderId && x.IsActive)?.FirstOrDefault();

            if (IsNotNull(orderDetails))
            {
                orderDetails.TrackingNumber = trackingNumber;
                return _orderDetailsRepository.Update(orderDetails);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return false;
        }

        //Update Order Tracking Number         
        public virtual bool UpdateTrackingNumberByOrderNumber(string orderNumber, string trackingNumber)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(orderNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorOrderNumberNull);

            if (string.IsNullOrEmpty(trackingNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorTrackingNumberNull);

            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderNumber = orderNumber, trackingNumber = trackingNumber });
            ZnodeOmsOrderDetail orderDetails = (from znodeOrderDetails in _orderDetailsRepository.Table
                                                join znodeOrder in _omsOrderRepository.Table on znodeOrderDetails.OmsOrderId equals znodeOrder.OmsOrderId
                                                where znodeOrder.OrderNumber == orderNumber && znodeOrderDetails.IsActive
                                                select znodeOrderDetails)?.FirstOrDefault();

            if (IsNotNull(orderDetails))
            {
                orderDetails.TrackingNumber = trackingNumber;
                return _orderDetailsRepository.Update(orderDetails);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return false;
        }

        //Update Order Tracking Number
        public virtual bool UpdateBillingAddress(int orderId, AddressModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (orderId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.OrderIdLessThanOne);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorBillingAddressNull);

            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, AddressModelWithAddressId = model?.AddressId });
            ZnodeOmsOrderDetail orderDetails = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == orderId && x.IsActive)?.FirstOrDefault();

            if (IsNotNull(orderDetails))
            {
                orderDetails.BillingFirstName = model.FirstName;
                orderDetails.BillingLastName = model.LastName;
                orderDetails.BillingCountry = model.CountryName;
                orderDetails.BillingStateCode = string.IsNullOrEmpty(model.StateCode) ? model.StateName : model.StateCode;
                orderDetails.BillingPostalCode = model.PostalCode;
                orderDetails.BillingPhoneNumber = model.PhoneNumber;
                orderDetails.BillingStreet1 = model.Address1;
                orderDetails.BillingStreet2 = model.Address2;
                orderDetails.BillingCity = model.CityName;
                orderDetails.AddressId = model.AddressId;
                return _orderDetailsRepository.Update(orderDetails);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return false;
        }

        //Update Order Payment Status
        public virtual bool AddOrderNote(OrderNotesModel orderNotesModel)
        {
            if (!string.IsNullOrEmpty(orderNotesModel?.Notes))
            {
                ZnodeOmsNote notes = _omsNoteRepository.Insert(orderNotesModel.ToEntity<ZnodeOmsNote>());
                ZnodeLogging.LogMessage("OrderNotesModel inserted having OmsNotesId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsNotesId = orderNotesModel?.OmsNotesId });
                orderNotesModel.OmsNotesId = notes.OmsNotesId;
                return orderNotesModel.OmsNotesId > 0;
            }
            return false;
        }

        //Add Refund Payment details
        public virtual bool AddRefundPaymentDetails(OrderItemsRefundModel refundPaymentListModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(refundPaymentListModel?.RefundOrderLineitems?.Count > 0))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRefundPaymentListModelNull);

            ZnodeLogging.LogMessage("Refund Order Line items count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { RefundOrderLineitemsCount = refundPaymentListModel?.RefundOrderLineitems?.Count });
            List<RefundPaymentModel> refundPayments = _omsPaymentRefundRepository.Insert(refundPaymentListModel?.RefundOrderLineitems.ToEntity<ZnodeOmsPaymentRefund>().ToList())?.ToModel<RefundPaymentModel>()?.ToList();

            ZnodeLogging.LogMessage(IsNotNull(refundPayments?.Count > 0) ? Admin_Resources.SuccessRefundPaymentCreate : Admin_Resources.ErrorRefundPaymentCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return refundPayments?.Count > 0;
        }

        //Get OrderLine Items With Refund payment left
        public virtual OrderItemsRefundModel GetOrderLineItemsWithRefund(int orderDetailsId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Order DetailsId for getting order Line Items With Refund:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderDetailsId = orderDetailsId });
            //Get order Details
            ZnodeOmsOrderDetail orderDetails = _orderDetailsRepository.Table?.Include(x => x.ZnodeOmsOrder)?.SingleOrDefault(x => x.OmsOrderDetailsId == orderDetailsId && x.IsActive);

            //Initialize OrderItemsRefundModel and Map ZnodeOmsOrderDetail
            OrderItemsRefundModel orderItemsRefundModel = MapToOrderItemsRefundModel(orderDetails);

            ZnodeLogging.LogMessage("Order number:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderNumber = orderDetails?.ZnodeOmsOrder?.OrderNumber });
            orderItemsRefundModel.OrderNumber = orderDetails?.ZnodeOmsOrder?.OrderNumber;
            //Get all Refund Types in Dictionary
            Dictionary<string, int> refundTypes = GetRefundPaymentType();

            //Get Order Line Items with Refund Details
            orderItemsRefundModel.RefundOrderLineitems.AddRange(GetRefundOrderLineItems(orderDetailsId, refundTypes[ZnodeConstant.PartialRefund.ToUpper()]));

            // Get shipping details if shipping cost greater than Zero
            orderItemsRefundModel.ShippingRefundDetails = (orderDetails?.ShippingCost > 0) ? MapRefundShippingDetails(refundTypes[ZnodeConstant.ShippingRefund.ToUpper()], orderDetails) : new RefundPaymentModel();

            //Map Order total refund details
            orderItemsRefundModel.TotalRefundDetails = MapRefundTotalDetails(refundTypes[ZnodeConstant.TotalRefund.ToUpper()], orderItemsRefundModel, orderDetails);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderItemsRefundModel;
        }

        //to resend order confirmation email
        public virtual bool ResendOrderConfirmationEmail(int orderId, FilterCollection filters, NameValueCollection expands)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Order Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderId);
                bool isEnableBcc = false;
                FilterCollection returnedOrderFilter = filters;
                filters = new FilterCollection();
                OrderModel orderModel = GetOrderById(orderId, filters, expands);
                //Create Single Order Line Item if order is having group product.
                orderModel.OrderLineItems = CreateSingleOrderLineItem(orderModel, true).Where(x => x.ParentOmsOrderLineItemsId != null).ToList();

                returnedOrderFilter.Add(new FilterTuple(Constants.FilterKeys.OmsOrderId, FilterOperators.Equals, orderId.ToString()));
                returnedOrderFilter.Add(new FilterTuple(Constants.FilterKeys.IsActive, FilterOperators.Equals, Convert.ToString(true)));

                OrderModel returnedOrderModel = GetOrderByIdForReturn(orderId, expands, returnedOrderFilter);

                MapReturnedTotal(orderModel, returnedOrderModel);

                //Create Single Order Line Item if returned order is having group product.
                orderModel.ReturnedOrderLineItems = CreateSingleOrderLineItem(returnedOrderModel);
                ZnodeLogging.LogMessage("ReturnedOrderLineItems count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ReturnedOrderLineItemsCount = orderModel.ReturnedOrderLineItems?.Count });
                orderModel.ShoppingCartModel.Shipping.ShippingName = GetShippingName(orderModel.ShoppingCartModel.Shipping.ShippingName, orderModel.ShoppingCartModel.Shipping.ShippingId);
                // And finally attach the receipt HTML to the order and return
                orderModel.ReceiptHtml = GetHtmlResendReceiptForEmail(orderModel, false, out isEnableBcc);
                var onPendingOrderStatusInit = new ZnodeEventNotifier<OrderModel>(orderModel, EventConstant.OnPendingOrderStatusNotification);
                return SendOrderReceipt(orderModel.PortalId, string.IsNullOrEmpty(orderModel.BillingAddress.EmailAddress) ? orderModel.Email : orderModel.BillingAddress.EmailAddress, $"{Admin_Resources.TitleOrderReceipt} - {orderModel.OrderNumber}", ZnodeConfigManager.SiteConfig.AdminEmail, string.Empty, orderModel.ReceiptHtml, isEnableBcc);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in ResendOrderConfirmationEmail method for OrderId " + orderId + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to resend order confirmation email for cart items.
        public virtual bool ResendOrderLineItemConfirmationEmail(int orderId, string omsOrderLineId, NameValueCollection expands, bool isEnableBcc = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (!expands.HasKeys())
                expands = GetOrderExpandForResendMail();

            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, omsOrderLineId = omsOrderLineId, expands = expands });
            OrderModel orderModel = GetOrderById(orderId, null, expands);

            //Create Single Order Line Item if order is having group product.
            CalculateCartItemForResendMail(orderModel, omsOrderLineId);

            // And finally attach the receipt HTML to the order and return
            orderModel.ReceiptHtml = GetHtmlResendReceiptForEmail(orderModel, true, out isEnableBcc);

            return SendOrderReceipt(orderModel.PortalId, orderModel.BillingAddress.EmailAddress, $"{Admin_Resources.TitleOrderReceipt} - {orderModel.OrderNumber}", ZnodeConfigManager.SiteConfig.AdminEmail, string.Empty, orderModel.ReceiptHtml, isEnableBcc);
        }

        //For Manage Order send line item status email notification
        protected virtual bool SendLineItemConfirmationEmail(ZnodeOrderFulfillment znodeOrderFulfillment, NameValueCollection expands, bool isEnableBcc = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<int> omsOrderLineIds = new List<int>() ;
            if (!expands.HasKeys())
                expands = GetOrderExpandForResendMail();

            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = znodeOrderFulfillment.OrderID, expands = expands });
            OrderModel orderModel = GetOrderById(znodeOrderFulfillment.OrderID, null, expands);
            foreach (OrderLineItemModel orderLineItem in znodeOrderFulfillment.OrderLineItems)
            {
                if (orderLineItem.IsItemStateChanged && SendOrderLineItemEmailNotification(orderLineItem.OrderLineItemStateId))
                {
                    omsOrderLineIds.Add(orderModel.OrderLineItems.FirstOrDefault(x => x.GroupId == orderLineItem.GroupId && x.Sku == orderLineItem.Sku).OmsOrderLineItemsId);
                }
            }
            if(omsOrderLineIds.Count == 0)
                return false;

            string modifiedLineItems = String.Join(",", omsOrderLineIds);
            //Create Single Order Line Item if order is having group product.
            CalculateCartItemForResendMail(orderModel, modifiedLineItems);

            // And finally attach the receipt HTML to the order and return
            orderModel.ReceiptHtml = GetHtmlResendReceiptForEmail(orderModel, true, out isEnableBcc);

            return SendOrderReceipt(orderModel.PortalId, orderModel.BillingAddress.EmailAddress, $"{Admin_Resources.TitleOrderReceipt} - {orderModel.OrderNumber}", ZnodeConfigManager.SiteConfig.AdminEmail, string.Empty, orderModel.ReceiptHtml, isEnableBcc);
        }

        //Get Html Resend Receipt For Email.
        public virtual string GetHtmlResendReceiptForEmail(OrderModel orderModel, bool isFromReturnedReceipt, out bool isEnableBcc)
        {
            try
            {
                string onResendReceipt = EventConstant.OnResendOrderReceipt;
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                foreach (OrderLineItemModel item in orderModel.OrderLineItems)
                {
                    item.PersonaliseValueList?.Remove("AllocatedLineItems");
                }

                IZnodeOrderReceipt receipt = GetOrderReceiptInstance(orderModel);
                orderModel.IsFromReturnedReceipt = isFromReturnedReceipt;
                string templateCode = isFromReturnedReceipt ? ZnodeConstant.OrderReceipt : ZnodeConstant.ResendOrderReceipt;
                if (Equals(orderModel.ReturnedOrderLineItems?.Count, 0))
                {
                    templateCode = ZnodeConstant.OrderReceipt;
                    onResendReceipt = EventConstant.OnOrderPlaced;
                }
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(templateCode, (orderModel.PortalId > 0) ? orderModel.PortalId : PortalId);
                isEnableBcc = false;
                UpdateDateTimeByGlobalSetting(orderModel);
                if (HelperUtility.IsNotNull(emailTemplateMapperModel))
                {
                    string receiptContent = ShowOrderAdditionalDetails(emailTemplateMapperModel.Descriptions, orderModel.AccountId);
                    isEnableBcc = emailTemplateMapperModel.IsEnableBcc;
                    return EmailTemplateHelper.ReplaceTemplateTokens(receipt.GetOrderResendReceiptHtml(receiptContent));
                }
                var OnResendOrderReceiptInit = new ZnodeEventNotifier<OrderModel>(orderModel, onResendReceipt);
                return string.Empty;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetHtmlResendReceiptForEmail method for OrderNumber " + orderModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get payment status list.
        public virtual List<OrderPaymentStateModel> GetOrderPaymentState()
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodeOmsPaymentState> _omsPaymentStateRepository = new ZnodeRepository<ZnodeOmsPaymentState>();
            return _omsPaymentStateRepository.GetEntityList(string.Empty).ToModel<OrderPaymentStateModel>().ToList();
        }

        /// <summary>
        /// This method is used to update the date-time as per global setting for emails, receipt.
        /// </summary>
        /// <param name="orderModel"></param>
        public virtual void UpdateDateTimeByGlobalSetting(OrderModel orderModel)
        {
            orderModel.OrderDate= Convert.ToDateTime(orderModel.OrderDate.ToTimeZoneDateTimeFormat());
            orderModel.OrderDateWithTime = Convert.ToDateTime(orderModel.OrderDateWithTime).ToTimeZoneDateTimeFormat();
            orderModel.InHandDate = Convert.ToDateTime(orderModel.InHandDate?.ToTimeZoneDateTimeFormat());
        }

        //Get order details by order id.
        public virtual OrderModel GetOrderByOrderLineItemId(int orderLineItemId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderModel orderModel = new OrderModel();
            ZnodeLogging.LogMessage("Order Line Item Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderLineItemId);
            //Get active order list for an order line.
            ZnodeOmsOrderLineItem orderLineItemDetails = _orderLineItemRepository.Table.Where(w => w.OmsOrderLineItemsId == orderLineItemId && w.IsActive)?.FirstOrDefault();

            //Get order line item data along with addon, configurable, bundle or group data.
            List<ZnodeOmsOrderLineItem> orderData = _orderLineItemRepository.Table.Where(w => w.OmsOrderLineItemsId == orderLineItemId || (w.ParentOmsOrderLineItemsId == orderLineItemDetails.OmsOrderLineItemsId)).ToList();
            List<OrderLineItemModel> orderLineItemModel = orderData?.ToModel<OrderLineItemModel>().ToList();
            orderModel.OrderLineItems = orderLineItemModel;
            ZnodeLogging.LogMessage("OrderLineItems Count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderLineItemsCount = orderModel?.OrderLineItems?.Count });
            foreach (OrderLineItemModel lineItem in orderModel.OrderLineItems)
            {
                //get personalise attributes by omsorderlineitemid
                orderHelper.SetPersonalizeDetails(lineItem);
            }

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderModel;
        }

        //to update order status.
        public virtual bool UpdateOrderStatus(OrderStateParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("OrderStateParameterModel with Id: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderId = model?.OmsOrderId });
            bool updated = false;

            ZnodeOmsOrderDetail order = IsValidOrder(model);

            if (IsNotNull(order))
            {
                int previousOrderStateId = order.OmsOrderStateId;
                order.OmsOrderStateId = model.OmsOrderStateId;
                order.TrackingNumber = model.TrackingNumber;
                order.ModifiedDate = GetDateTime();
                List<ZnodeOmsOrderState> orderStateList = _omsOrderStateRepository.GetEntityList(string.Empty)?.ToList();
                ZnodeLogging.LogMessage("orderStateList Count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderStateListCount = orderStateList?.Count });

                if (orderStateList?.Count > 0)
                {
                    if (order.OmsOrderStateId.Equals(orderStateList?.FirstOrDefault(x => x.OrderStateName == ZnodeOrderStatusEnum.SHIPPED.ToString())?.OmsOrderStateId))
                        order.ShipDate = DateTime.Now;
                    else if (order.OmsOrderStateId.Equals(orderStateList?.FirstOrDefault(x => x.OrderStateName == ZnodeOrderStatusEnum.RETURNED.ToString())?.OmsOrderStateId))
                        order.ReturnDate = GetDateTime().Date + GetDateTime().TimeOfDay;
                }

                updated = _orderDetailsRepository.Update(order);
                if (updated)
                    UpdateLineItemState(model.OmsOrderId, previousOrderStateId, model.OmsOrderStateId, order.ShipDate);
                //Cancel tax transaction.
                CancelTaxTransaction(updated, order, orderStateList);

                if (updated && orderHelper.IsSendEmail(order.OmsOrderStateId))
                {
                    OrderModel orderModel = GetOrderById(order.OmsOrderId, null, GetOrderExpands());
                    SendOrderStatusEmail(orderModel);
                    return true;
                }
            }
            return updated;
        }

        /// <summary>
        /// Check quantity with in-stock inventory
        /// </summary>
        /// <param name="checkout"></param>
        /// <param name="inventoryList"></param>
        /// <returns></returns>
        public virtual bool CheckQuantityWithInventory(IZnodeCheckout checkout, List<InventorySKUModel> inventoryList)
        {
            //Check quantity with in-stock inventory
            return checkout.ShoppingCart.CheckWithInStockInventory(inventoryList);
        }

        protected virtual ZnodeOmsOrderDetail IsValidOrder(OrderStateParameterModel model)
        {
            try
            {
                if (IsNull(model))
                    throw new Exception("Order model cannot be null.");

                if (model.OmsOrderId < 1)
                    throw new Exception("Order ID cannot be less than 1.");

                if (IsNull(model.OmsOrderStateId) || model.OmsOrderStateId < 1)
                    throw new Exception("Invalid order status.");

                ZnodeOmsOrderDetail order = _orderDetailsRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(CreateFiltersForOrder(model).ToFilterDataCollection())?.WhereClause, GetExpandsForOrderLineItem(GetOrderLineItemExpands()));
                return order;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in IsValidOrder method for OmsOrderId " + model?.OmsOrderId + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        protected virtual bool UpdateReturnedOrderStatus(OrderStateParameterModel model)
        {
            bool updated = false;

            ZnodeOmsOrderDetail order = IsValidOrder(model);

            if (IsNotNull(order))
            {
                order.OmsOrderStateId = model.OmsOrderStateId;
                order.TrackingNumber = model.TrackingNumber;
                order.ModifiedDate = GetDateTime();
                order.SubTotal = 0;
                order.Total = 0;
                order.OrderTotalWithoutVoucher = model.ShippingHandlingCharges + model.ReturnCharges;
                updated = _orderDetailsRepository.Update(order);
            }
            return updated;
        }


        //to update order status.
        public virtual bool UpdateOrderDetailsByOrderNumber(OrderDetailsModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool orderUpdated = false;

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorOrderModelNull);

            if (string.IsNullOrEmpty(model.OmsOrderNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorOrderNumberEmptyOrNull);
            ZnodeLogging.LogMessage("OrderDetailsModel with: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderNumber = model?.OmsOrderNumber, ExternalId = model?.ExternalId, OrderStateName = model?.OrderStateName });

            int omsOrderId = _omsOrderRepository.Table.Where(x => x.OrderNumber == model.OmsOrderNumber).Select(x => x.OmsOrderId).FirstOrDefault();
            if (omsOrderId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidOrderNo);

            //Update external id.
            if (!string.IsNullOrEmpty(model.ExternalId))
                orderUpdated = UpdateOMSExternalId(model.OmsOrderNumber, model.ExternalId);

            //Insert order notes.
            if (!string.IsNullOrEmpty(model.OrderNotes))
            {
                _omsNoteRepository.Insert(new ZnodeOmsNote { Notes = model.OrderNotes, OmsOrderDetailsId = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == omsOrderId && x.IsActive).Select(x => x.OmsOrderDetailsId).FirstOrDefault() });
                orderUpdated = true;
            }

            //Update order status.
            if (!string.IsNullOrEmpty(model.OrderStateName) || model.OmsOrderStateId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeOmsOrderStateEnum.IsOrderState.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));
                string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;

                List<ZnodeOmsOrderState> orderStateList = _omsOrderStateRepository.GetEntityList(whereClause)?.ToList();
                //Get the order state id on the basis of entered state code.
                int? omsOrderStateId = !string.IsNullOrEmpty(model.OrderStateName)
                    ? orderStateList?.FirstOrDefault(x => x.OrderStateName.ToLower() == model.OrderStateName.ToLower())?.OmsOrderStateId
                    : IsNull(model.OmsOrderStateId) ? 0 : model.OmsOrderStateId;

                if (IsNotNull(omsOrderStateId))
                    orderUpdated = UpdateOrderStatus(new OrderStateParameterModel { OmsOrderStateId = omsOrderStateId.Value, OmsOrderId = omsOrderId });
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderUpdated;
        }

        public virtual void SendOrderStatusEmail(OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            foreach (OrderLineItemModel item in orderModel.OrderLineItems)
                item.OrderLineItemCollection.AddRange(orderModel.OrderLineItems.Where(x => x.ParentOmsOrderLineItemsId == item.OmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.AddOns)?.ToList());

            orderModel.OrderLineItems.RemoveAll(x => x.ParentOmsOrderLineItemsId == null);

            string subject = string.Empty;
            bool isEnableBcc = false;
            string onOrderStatusUpdate = string.Empty;
            if(string.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                subject = $"{Admin_Resources.CancelledOrderStatusSubject} - {orderModel.OrderNumber}";
                orderModel.ReceiptHtml = GetCancelledOrderReceiptForEmail(orderModel, out isEnableBcc);
                onOrderStatusUpdate = EventConstant.OnOrderCancelled;
            }
            else
            {
                // And finally attach the receipt HTML to the order and return
                subject = $"{Admin_Resources.ShippedOrderStatusSubject} - {orderModel.OrderNumber}";
                orderModel.ReceiptHtml = GetShippingReceiptForEmail(orderModel, out isEnableBcc);
                onOrderStatusUpdate = EventConstant.OnOrderShipped;
            }
            if (!string.IsNullOrEmpty(orderModel.ReceiptHtml))
                SendOrderReceipt(orderModel.PortalId, orderModel.BillingAddress.EmailAddress ?? orderModel.Email, subject, ZnodeConfigManager.SiteConfig.AdminEmail, string.Empty, orderModel.ReceiptHtml, isEnableBcc);
            if (!string.IsNullOrEmpty(onOrderStatusUpdate))
            {
                var onOrderStatusUpdateInit = new ZnodeEventNotifier<OrderModel>(orderModel, onOrderStatusUpdate);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        public virtual OrderModel CheckInventoryAndMinMaxQuantity(ShoppingCartModel shoppingCartModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                if (IsNull(shoppingCartModel))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorShoppingCartModelNull);

                UserAddressModel userDetails = SetUserDetails(shoppingCartModel);

                var znodeShoppingCart = _shoppingCartMap.ToZnodeShoppingCart(shoppingCartModel, userDetails);

                // Create the checkout object
                IZnodeCheckout checkout = CheckoutMap.ToZnodeCheckout(userDetails, znodeShoppingCart);
                string isInventoryInStockMessage = string.Empty;
                Dictionary<int, string> minMaxSelectableQuantity;
                checkout.ShoppingCart.CheckInventoryAndMinMaxQuantity(out isInventoryInStockMessage, out minMaxSelectableQuantity);

                if (!string.IsNullOrEmpty(isInventoryInStockMessage))
                    throw new ZnodeException(ErrorCodes.OutOfStockException, Admin_Resources.OutOfStockException);

                if (IsNotNull(minMaxSelectableQuantity) && minMaxSelectableQuantity.Count > 0 && minMaxSelectableQuantity.ContainsKey(ErrorCodes.MinAndMaxSelectedQuantityError))
                    throw new ZnodeException(ErrorCodes.MinAndMaxSelectedQuantityError, minMaxSelectableQuantity[ErrorCodes.MinAndMaxSelectedQuantityError]);

                ZnodeLogging.LogMessage("Is available inventory message and MinMaxQuantity flag:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { isInventoryInStockMessage = isInventoryInStockMessage, minMaxSelectableQuantity = minMaxSelectableQuantity });

                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return new OrderModel() { IsAvailabelInventoryAndMinMaxQuantity = true };
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in CheckInventoryAndMinMaxQuantity method for OrderNumber " + shoppingCartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to save order history in database
        public virtual OrderHistoryModel CreateOrderHistory(OrderHistoryModel orderHistoryModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(orderHistoryModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorOrderHistoryModelNull);

            if (orderHistoryModel.OmsNotesId == 0)
                orderHistoryModel.OmsNotesId = null;

            if (orderHistoryModel.OrderAmount == 0)
                orderHistoryModel.OrderAmount = null;

            ZnodeLogging.LogMessage("OrderHistoryModel with OmsHistoryId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsHistoryId = orderHistoryModel?.OmsHistoryId });
            ZnodeOmsHistory orderHistory = _orderHistoryRepository.Insert(orderHistoryModel.ToEntity<ZnodeOmsHistory>());

            ZnodeLogging.LogMessage(IsNotNull(orderHistory) ? Admin_Resources.SuccessOrderHistoryCreate : Admin_Resources.ErrorOrderHistoryCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderHistory?.ToModel<OrderHistoryModel>();
        }

        //Get order state by id
        public virtual OrderStateModel GetOrderStateValueById(int omsOrderStateId)
        {
            ZnodeLogging.LogMessage("Oms order state Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, omsOrderStateId);
            ZnodeOmsOrderState model = _omsOrderStateRepository.Table.FirstOrDefault(x => x.OmsOrderStateId == omsOrderStateId);
            return model.ToModel<OrderStateModel>();
        }

        // Send returned order email.
        public virtual bool SendReturnedOrderEmail(int orderId, NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, expands = expands, filters = filters });
            OrderModel orderModel = GetOrderByIdForReturn(orderId, expands, filters);
            bool isEnableBcc = false;
            //Create Single Order Line Item if order is having group product.
            orderModel.OrderLineItems = CreateSingleOrderLineItem(orderModel);
            //Set shipping of returned order
            orderModel.SubTotal = Convert.ToDecimal(orderModel?.ReturnItemList?.SubTotal);
            orderModel.ShippingCost = Convert.ToDecimal(orderModel?.ReturnItemList?.ShippingCost);
            orderModel.TaxCost = Convert.ToDecimal(orderModel?.ReturnItemList?.TaxCost);
            orderModel.DiscountAmount = Convert.ToDecimal(orderModel?.ReturnItemList?.DiscountAmount);
            orderModel.CSRDiscountAmount = Convert.ToDecimal(orderModel?.ReturnItemList?.CSRDiscount);
            orderModel.ShippingDiscount = Convert.ToDecimal(orderModel?.ReturnItemList?.ShippingDiscount);
            orderModel.ReturnCharges = Convert.ToDecimal(orderModel?.ReturnItemList?.ReturnCharges);
            orderModel.OrderTotalWithoutVoucher = Convert.ToDecimal(orderModel?.ReturnItemList?.Total);

            orderModel.ShippingHandlingCharges = 0m;
            orderModel.ShippingDifference = 0m;
            orderModel.GiftCardAmount = 0m;
            orderModel.OrderLineItems.ForEach(x => x.OrderLineItemState = ZnodeOrderStatusEnum.RETURNED.ToString());
            // And finally attach the receipt HTML to the order and return
            orderModel.ReceiptHtml = GetHtmlResendReceiptForEmail(orderModel, true, out isEnableBcc);

            return SendOrderReceipt(orderModel.PortalId, orderModel.BillingAddress.EmailAddress, $"{Admin_Resources.TitleReturnedOrderReceipt} - {orderModel.OrderNumber}", ZnodeConfigManager.SiteConfig.AdminEmail, string.Empty, orderModel.ReceiptHtml, isEnableBcc);
        }

        // Get order by order id for returned order.
        public virtual OrderModel GetOrderByIdForReturn(int orderId, NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, expands = expands, filters = filters });
            ZnodeOmsOrder order = _omsOrderRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId);

            ZnodeOmsOrderDetail orderDetails = GetOrderDetailsForLineItem(orderId, expands, filters);

            return GetOrderDetails(order, orderDetails, true, false, false, null, true);
        }

        public virtual bool SendPOEmail(SendInvoiceModel sendInvoiceModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeEmail.SendEmail(sendInvoiceModel.PortalId, sendInvoiceModel.ReceiverEmail, ZnodeConfigManager.SiteConfig.AdminEmail, null, $"{Admin_Resources.TitleOrderReceipt} - {sendInvoiceModel.OrderNumber}", sendInvoiceModel.ReceiptHtml, true, "");
            return true;
        }


        //Calculate tax cost for partially return items.
        public virtual void CalculateReturnItemTax(int portalId, OrderModel model, List<ReturnOrderLineItemModel> returnItems)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { portalId = portalId, returnItemsCount = returnItems?.Count });
            IZnodeRepository<ZnodePortalTaxClass> _portalTaxClassRepository = new ZnodeRepository<ZnodePortalTaxClass>();
            IZnodeRepository<ZnodeTaxRule> _taxRuleRepository = new ZnodeRepository<ZnodeTaxRule>();
            IZnodeRepository<ZnodeTaxClass> _taxClassRepository = new ZnodeRepository<ZnodeTaxClass>();
            IZnodeRepository<ZnodeTaxClassSKU> _taxClassSKURepository = new ZnodeRepository<ZnodeTaxClassSKU>();

            List<ReturnOrderLineItemModel> addOnReturnLineItem = GetAddOnReturnItem(model.OrderLineItems, returnItems);
            returnItems.AddRange(addOnReturnLineItem);

            foreach (var item in returnItems)
            {
                bool isSKUTaxExists = (from portalTaxClass in _portalTaxClassRepository.Table
                                       join taxClassSku in _taxClassSKURepository.Table on portalTaxClass.TaxClassId equals taxClassSku.TaxClassId
                                       where portalTaxClass.PortalId == portalId && taxClassSku.SKU == item.SKU
                                       select taxClassSku)?.Count() > 0;

                ZnodeTaxRule taxRuleItem = null;
                if (isSKUTaxExists)
                {
                    taxRuleItem = (from taxRule in _taxRuleRepository.Table
                                   join taxClass in _taxClassRepository.Table on taxRule.TaxClassId equals taxClass.TaxClassId
                                   join portalTaxClass in _portalTaxClassRepository.Table on taxRule.TaxClassId equals portalTaxClass.TaxClassId
                                   join taxClassSku in _taxClassSKURepository.Table on taxClass.TaxClassId equals taxClassSku.TaxClassId
                                   where taxClassSku.SKU == item.SKU && portalTaxClass.PortalId == portalId
                                   select taxRule)?.FirstOrDefault();
                }
                else
                {
                    taxRuleItem = (from taxRule in _taxRuleRepository.Table
                                   join portalTaxClass in _portalTaxClassRepository.Table on taxRule.TaxClassId equals portalTaxClass.TaxClassId
                                   where portalTaxClass.PortalId == portalId && taxRule.Precedence > 0
                                   select taxRule)?.FirstOrDefault();
                }

                if (IsNotNull(taxRuleItem))
                {
                    item.Gst = IsNotNull(taxRuleItem.GST) ? item.ExtendedPrice * (taxRuleItem.GST / 100) : 0;
                    item.Hst = IsNotNull(taxRuleItem.HST) ? item.ExtendedPrice * (taxRuleItem.HST / 100) : 0;
                    item.Pst = IsNotNull(taxRuleItem.PST) ? item.ExtendedPrice * (taxRuleItem.PST / 100) : 0;
                    item.Vat = IsNotNull(taxRuleItem.VAT) ? item.ExtendedPrice * (taxRuleItem.VAT / 100) : 0;
                    item.SalesTax = IsNotNull(taxRuleItem.SalesTax) ? item.ExtendedPrice * (taxRuleItem.SalesTax / 100) : 0;
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get AddOns return line item for calculate tax cost.
        public virtual List<ReturnOrderLineItemModel> GetAddOnReturnItem(List<OrderLineItemModel> orderLineItemList, ReturnOrderLineItemModel returnItem)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<ReturnOrderLineItemModel> addOnReturnLineItem = new List<ReturnOrderLineItemModel>();
            List<OrderLineItemModel> addOnLineItem = orderLineItemList
                                .Where(orderLineItem => returnItem.AddOnProductSKUs.Contains(orderLineItem.Sku) && orderLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns)?.ToList();

            returnItem.ExtendedPrice = (orderLineItemList.FirstOrDefault(orderLineItem => orderLineItem.OmsOrderLineItemsId == returnItem.OmsOrderLineItemsId)?.Price).GetValueOrDefault();

            if (addOnLineItem?.Count() > 0)
            {
                foreach (var addOnItem in addOnLineItem)
                    addOnReturnLineItem.Add(GetAddOnReturnLineItem(addOnItem, returnItem));
            }
            ZnodeLogging.LogMessage("AddOnReturnLineItem count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, addOnReturnLineItem?.Count);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return addOnReturnLineItem;
        }

        //Get list of AddOns return line item for calculate tax cost.
        public virtual List<ReturnOrderLineItemModel> GetAddOnReturnItem(List<OrderLineItemModel> orderLineItemList, List<ReturnOrderLineItemModel> returnItemList)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<ReturnOrderLineItemModel> addOnReturnLineItem = new List<ReturnOrderLineItemModel>();
            foreach (var returnItem in returnItemList)
            {
                List<OrderLineItemModel> addOnLineItem = orderLineItemList
                                                .Where(orderLineItem => returnItem.AddOnProductSKUs.Contains(orderLineItem.Sku) && orderLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns)?.ToList();

                if (addOnLineItem?.Count() > 0)
                {
                    foreach (var addOnItem in addOnLineItem)
                        addOnReturnLineItem.Add(GetAddOnReturnLineItem(addOnItem, returnItem));
                }
            }
            ZnodeLogging.LogMessage("AddOn ReturnLineItem count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, addOnReturnLineItem?.Count);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return addOnReturnLineItem;
        }

        //Map Add ons return line items.
        public virtual ReturnOrderLineItemModel GetAddOnReturnLineItem(OrderLineItemModel orderLineItemModel, ReturnOrderLineItemModel returnItem)
             => new ReturnOrderLineItemModel()
             {
                 Description = orderLineItemModel.Description,
                 Quantity = returnItem.Quantity,
                 ShippingCost = orderLineItemModel.ShippingCost.GetValueOrDefault(),
                 SKU = orderLineItemModel.Sku,
                 ProductName = orderLineItemModel.ProductName,
                 IsActive = orderLineItemModel.IsActive,
                 ShipSeperately = orderLineItemModel.ShipSeparately.GetValueOrDefault(),
                 Vat = orderLineItemModel.VAT,
                 Gst = orderLineItemModel.GST,
                 Hst = orderLineItemModel.HST,
                 Pst = orderLineItemModel.PST,
                 ParentOmsOrderLineItemsId = returnItem.ParentOmsOrderLineItemsId,
                 OrderLineItemRelationshipTypeId = (int)ZnodeCartItemRelationshipTypeEnum.AddOns,
                 SalesTax = orderLineItemModel.SalesTax,
                 OmsOrderStatusId = returnItem.OmsOrderStatusId,
                 OmsOrderLineItemsId = orderLineItemModel.OmsOrderLineItemsId,
                 OmsOrderShipmentId = orderLineItemModel.OmsOrderShipmentId,
                 ReasonForReturnId = returnItem.ReasonForReturnId,
                 OrderLineItemStatus = orderLineItemModel.OrderLineItemState,
                 OrderDetailId = orderLineItemModel.OmsOrderDetailsId,
                 ExtendedPrice = orderLineItemModel.Price
             };

        /// <summary>
        /// Check if all the order line items need to return and if yes then update the status of all return items.
        /// </summary>
        /// <param name="model">OrderModel</param>
        /// <param name="isReturnItemsUpdateRequired">If set to false then return items should not get updated in database.</param>
        /// <returns></returns>
        public virtual bool IsReturnAllItems(OrderModel model, bool isReturnItemsUpdateRequired = true)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("OrderModel with OmsOrderDetailsId and OmsOrderId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderDetailsId = model?.OmsOrderDetailsId, OmsOrderId = model?.OmsOrderId });
            if (model.ShoppingCartModel.ShoppingCartItems?.Count < 1 && model.ReturnItemList?.ReturnItemList?.Count > 0)
            {
                List<ReturnOrderLineItemModel> addOnReturnLineItem = new List<ReturnOrderLineItemModel>();
                model.ReturnItemList.ReturnItemList.ForEach(returnItem =>
                {
                    var lineItem = model.OrderLineItems
                                   .FirstOrDefault(orderLineItem => orderLineItem.OmsOrderLineItemsId == returnItem.OmsOrderLineItemsId);
                    returnItem.OrderDetailId = lineItem.OmsOrderDetailsId;
                    returnItem.OmsOrderShipmentId = lineItem.OmsOrderShipmentId;
                    returnItem.IsActive = lineItem.IsActive;
                    if (returnItem?.GroupProducts?.Count > 0)
                    {
                        returnItem.ExtendedPrice = 0;
                        returnItem.Quantity = 0;
                    }

                    if (returnItem.AddOnLineItemId > 0 || (!string.IsNullOrEmpty(returnItem.AddOnProductSKUs)))
                        addOnReturnLineItem = GetAddOnReturnItem(model.OrderLineItems, returnItem);
                });

                if (addOnReturnLineItem.Count > 0)
                    model.ReturnItemList.ReturnItemList.AddRange(addOnReturnLineItem);

                IsShippingCostReturned(model.OmsOrderId, model.ReturnItemList.ReturnItemList);

                //if value of flag is false the records should not get updated in database.
                if (!isReturnItemsUpdateRequired)
                    return true;

                if (model.ReturnItemList.ReturnItemList.Count > 1)
                    return model.ReturnItemList.ReturnItemList.TrueForAll(item => orderHelper.ReturnOrderLineItems(item));
                else
                    return orderHelper.ReturnOrderLineItems((model.ReturnItemList.ReturnItemList.FirstOrDefault()));
            }
            ZnodeLogging.LogMessage("ReturnOrderLineItems count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, model?.ReturnItemList?.ReturnItemList?.Count);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return false;
        }

        //Expands necessary to get OrderDetails.
        public virtual NameValueCollection GetOrderExpands()
        {

            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsPaymentState.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsPaymentState.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrder.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsOrder.ToString().ToLower());
            expands.Add(ExpandKeys.ZnodeShipping, ExpandKeys.ZnodeShipping);
            expands.Add(ExpandKeys.ZnodeUser, ExpandKeys.ZnodeUser);
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsHistories.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsHistories.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString().ToLower());
            ZnodeLogging.LogMessage("Get order expands parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, expands);
            return expands;
        }

        //Get portal pixel tracking details.
        public virtual void GetPortalPixelTracking(OrderModel orderModel)
        {
            IZnodeRepository<ZnodePortalPixelTracking> _pixelTrackingRepository = new ZnodeRepository<ZnodePortalPixelTracking>();
            ZnodeLogging.LogMessage("PortalId to get PortalTrackingPixel: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = orderModel?.PortalId });
            orderModel.PortalTrackingPixel = _pixelTrackingRepository.Table.FirstOrDefault(x => x.PortalId == orderModel.PortalId)?.ToModel<PortalTrackingPixelModel>();
        }

        public virtual OrderModel GetOrderDetails(ZnodeOmsOrder order, ZnodeOmsOrderDetail orderDetail, bool isFromOrderReceipt, bool isOrderHistory, bool isFromReorder, NameValueCollection expands = null, bool isFromReturnLineItem = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderModel orderModel = new OrderModel();
            //null check for order detail object.
            if (IsNotNull(orderDetail))
            {
                ZnodeLogging.LogMessage("OmsOrderDetailsId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsOrderDetailsId = orderDetail?.OmsOrderDetailsId });
                //Map order detail object to OrderMOdel object.
                orderModel = orderDetail.ToModel<OrderModel>();

                //If expand key contains PortalTrackingPixel key then get portal tracking pixel details.
                if (!string.IsNullOrEmpty(expands?[ExpandKeys.PortalTrackingPixel]))
                    GetPortalPixelTracking(orderModel);

                if (!isFromReturnLineItem)
                {
                    List<ZnodeOmsOrderLineItem> orderLineItems = orderHelper.GetOrderLineItemByOrderId(orderDetail.OmsOrderDetailsId).ToList();
                    if (isFromReorder)
                        orderModel.OrderLineItems = orderLineItems.Where(m => m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns) && m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles))?
                                                        .ToModel<OrderLineItemModel>()?.ToList();
                    else
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
                    }
                }
                else
                {
                    orderModel.OrderLineItems = orderDetail.ZnodeOmsOrderLineItems.ToModel<OrderLineItemModel>().ToList() ?? new List<OrderLineItemModel>();
                    orderModel.ReturnedOrderLineItems = IsNotNull(orderModel.ReturnedOrderLineItems) ? orderModel.ReturnedOrderLineItems : new List<OrderLineItemModel>();
                }

                //Map order data from ZnodeOmsOrder.
                orderModel.IsQuoteOrder = order.IsQuoteOrder;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OmsQuoteId = order.OMSQuoteId.GetValueOrDefault();
                orderModel.IsOldOrder= order.IsOldOrder;
                if (orderModel.CreatedBy == 0)
                {
                    orderModel.CreatedBy = order.CreatedBy;
                }
                ZnodeLogging.LogMessage("OrderNumber:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderNumber = orderModel.OrderNumber });

                //check for omsOrderDetailsId greater than 0.
                if (orderModel?.OmsOrderDetailsId > 0)
                {
                    //Map order data from ZnodeOmsOrder.
                    orderModel.IsQuoteOrder = order.IsQuoteOrder;
                    orderModel.OrderNumber = order.OrderNumber;
                    orderModel.PaymentType = GetPaymentType(orderModel);

                    //Map Order details data to OrderModel.
                    MapOrderData(orderModel, orderDetail, isOrderHistory);

                    //Get Ordered billing address.
                    orderModel.BillingAddressHtml = GetOrderBillingAddress(orderModel);

                    //set the amount of different discount applied during order creation.
                    SetOrderDiscount(orderModel);

                    //Set Rma details for RMA validation checks
                    GetAndMapRmaDetails(orderModel);

                    // Map Customer Shipping
                    CustomerShipping(orderModel);

                    //Check whether current order is old order or not
                    CheckOrderIsOld(orderModel, order.OmsOrderId);

                    if (IsNotNull(orderModel?.OrderLineItems))
                    {
                        List<ZnodeOmsOrderStateShowToCustomer> orderStatusList = GetOrderStatusForCustomerList(orderModel.OrderLineItems);

                        List<ZnodeOmsOrderLineItemsAdditionalCost> additionalCostList = GetAdditionalCostList(orderModel.OrderLineItems);

                        List<ZnodeOmsOrderShipment> orderShipmentList = GetOrderShipmentList(orderModel.OrderLineItems);

                        List<ZnodeAddress> addressList = GetOrderShipmentAddressList(orderModel.OrderLineItems.Select(x => x.ZnodeOmsOrderShipment)?.ToList());

                        List<ZnodeOmsPersonalizeItem> personalizeList = GetPersonalizedValueOrderLineItemList(orderModel.OrderLineItems);

                        List<string> downloadableProductkeys = GetDownloadableProductKeyList(orderModel.OrderLineItems.Select(x => x.Sku)?.Distinct()?.ToList());

                        foreach (OrderLineItemModel lineItem in orderModel.OrderLineItems)
                        {
                            //If expands constains IsWebStoreOrderReceipt key and line item contains IsShowToCustomer false, get order status that will be shown to customer.
                            if (!string.IsNullOrEmpty(expands?[ExpandKeys.IsWebStoreOrderReceipt]) && !lineItem.IsShowToCustomer)
                                lineItem.OrderLineItemState = GetOrderStatusForCustomer(lineItem.OrderLineItemStateId, orderStatusList);

                            lineItem.ZnodeOmsOrderShipment = orderShipmentList?.FirstOrDefault(x => x.OmsOrderShipmentId == lineItem.OmsOrderShipmentId)?.ToModel<OrderShipmentModel>();

                            lineItem.ShippingAddressHtml = GetOrderShipmentAddress(lineItem.ZnodeOmsOrderShipment, addressList);
                            //get personalize attributes by omsorderlineitemid
                            lineItem.PersonaliseValueList = GetPersonalizedValueOrderLineItem(Convert.ToInt32(lineItem.ParentOmsOrderLineItemsId) > 0 ? Convert.ToInt32(lineItem.ParentOmsOrderLineItemsId) : lineItem.OmsOrderLineItemsId, personalizeList);

                            lineItem.PersonaliseValuesDetail = orderHelper.GetPersonalizedAttributeLineItemDetails(lineItem.PersonaliseValueList, string.Empty);

                            lineItem.DownloadableProductKey = GetProductKey(lineItem.Sku, lineItem.OmsOrderLineItemsId, downloadableProductkeys);

                            lineItem.AdditionalCost = additionalCostList?.Where(x => x.OmsOrderLineItemsId == lineItem.OmsOrderLineItemsId)?.ToDictionary(x => x.KeyName, y => y.KeyValue);
                        }
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderModel;
        }

        //Get Order Receipt Datails.
        public virtual OrderModel GetOrderReceiptByOrderId(int orderId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (orderId <= 0)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorPlaceOrder);

            DataSet dataset =  GetDataSetByOrderId(orderId);

            if (IsNull(dataset))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorPlaceOrder);

            SetDataTableNames(dataset);

            ConvertDataTableToList dataTable = new ConvertDataTableToList();
            ZnodeOmsOrder order = dataTable.ConvertDataTable<ZnodeOmsOrder>(dataset.Tables[ZnodeConstant.Order])?.FirstOrDefault();

            OrderModel orderModel = new OrderModel();

            if (IsNotNull(order))
            {
                ZnodeOmsOrderDetail orderDetail = dataTable.ConvertDataTable<ZnodeOmsOrderDetail>(dataset.Tables[ZnodeConstant.OrderDetail])?.FirstOrDefault();
                orderDetail.ZnodeShipping = dataTable.ConvertDataTable<ZnodeShipping>(dataset.Tables[ZnodeConstant.ZnodeShipping])?.FirstOrDefault();
                orderId = order.OmsOrderId;

                //null check for order detail object.
                if (IsNotNull(orderDetail))
                {
                    orderModel = orderDetail.ToModel<OrderModel>();
                    //Map order detail object to OrderMOdel object.
                    List<ZnodeOmsOrderLineItem> orderLineItems = dataTable.ConvertDataTable<ZnodeOmsOrderLineItem>(dataset.Tables[ZnodeConstant.OrderLineItems]);
                    List<ZnodeOmsOrderState> znodeOmsOrderState = dataTable.ConvertDataTable<ZnodeOmsOrderState>(dataset.Tables[ZnodeConstant.ZnodeOmsOrderState]);

                    foreach (ZnodeOmsOrderLineItem orderLineItem in orderLineItems)
                    {
                        orderLineItem.ZnodeOmsOrderState = znodeOmsOrderState.FirstOrDefault(x => x.OmsOrderStateId == orderLineItem.OrderLineItemStateId);
                    }

                    orderModel.OrderLineItems = orderLineItems?.Where(m => m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns) && m.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles))?
                                                        .ToModel<OrderLineItemModel>()?.ToList();
                    if (orderModel.OrderLineItems?.Count > 0)
                    {
                        orderModel.OrderLineItems.ForEach(x =>
                        {
                            x.Description = orderLineItems.FirstOrDefault(m => m.OrderLineItemRelationshipTypeId == Convert.ToInt16(ZnodeCartItemRelationshipTypeEnum.AddOns)
                                                           && m.ParentOmsOrderLineItemsId == x.OmsOrderLineItemsId)?.Description ?? x.Description;
                            x.Quantity = Convert.ToDecimal(ServiceHelper.ToInventoryRoundOff(x.Quantity));
                            ZnodeOmsOrderLineItem lineItem = orderLineItems.FirstOrDefault(y => y.OmsOrderLineItemsId == x.OmsOrderLineItemsId);
                            x.OrderLineItemState = lineItem?.ZnodeOmsOrderState?.OrderStateName;
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
                            if (x.ParentOmsOrderLineItemsId == null)
                                x.ProductType = GetLineItemProductType(x, orderLineItems);
                        });
                    }
                    //Map order data from ZnodeOmsOrder.
                    orderModel.IsQuoteOrder = order.IsQuoteOrder;
                    orderModel.OrderNumber = order.OrderNumber;
                    orderModel.OmsQuoteId = order.OMSQuoteId.GetValueOrDefault();
                    orderModel.OrderState = GetOrderStateValueById(orderModel.OmsOrderStateId)?.OrderStateName?.ToLower()?.ToProperCase();

                    //check for omsOrderDetailsId greater than 0.
                    if (orderModel.OmsOrderDetailsId > 0)
                    {
                        orderModel.CustomerPaymentGUID = orderDetail.ZnodeUser?.CustomerPaymentGUID;
                        orderModel.ShippingId = (orderDetail.ZnodeShipping?.ShippingId).GetValueOrDefault();
                        orderModel.TrackingUrl = orderDetail.ZnodeShipping?.TrackingUrl;
                        int orderShipmentId = (orderModel.OrderLineItems.FirstOrDefault()?.OmsOrderShipmentId).GetValueOrDefault();

                        orderModel.BillingAddress = orderDetail.ToModel<AddressModel>();

                        SetOrderBillingAddress(orderModel);
                        //Get Ordered billing address.
                        orderModel.BillingAddressHtml = GetOrderBillingAddress(orderModel);

                        //set the amount of different discount applied during order creation.
                        SetOrderDiscount(orderModel);

                        //set the tax importDuty.
                        SetImportDuty(orderModel);

                        //Set Rma details for RMA validation checks
                        GetAndMapRmaDetails(orderModel);

                        // Map Customer Shipping
                        CustomerShipping(orderModel);

                        if (IsNotNull(orderModel.OrderLineItems) && orderModel.OrderLineItems.Count() > 0)
                        {
                            List<ZnodeAddress> addressList = GetOrderShipmentAddressList(orderModel.OrderLineItems.Select(x => x.ZnodeOmsOrderShipment).ToList());
                            List<ZnodeOmsOrderStateShowToCustomer> orderStatusList = GetOrderStatusForCustomerList(orderModel.OrderLineItems);
                            List<ZnodeOmsOrderShipment> orderShipmentList = GetOrderShipmentList(orderModel.OrderLineItems);
                            List<ZnodeOmsPersonalizeItem> personalizeList = GetPersonalizedValueOrderLineItemList(orderModel.OrderLineItems);
                            List<string> downloadableProductkeys = GetDownloadableProductKeyList(orderModel.OrderLineItems?.Select(x => x.Sku)?.Distinct()?.ToList());
                            List<ZnodeOmsOrderLineItemsAdditionalCost> additionalCostList = GetAdditionalCostList(orderModel.OrderLineItems);

                            foreach (OrderLineItemModel lineItem in orderModel.OrderLineItems)
                            {
                                if (!lineItem.IsShowToCustomer)
                                    lineItem.OrderLineItemState = GetOrderStatusForCustomer(lineItem.OrderLineItemStateId, orderStatusList);

                                lineItem.ZnodeOmsOrderShipment = orderShipmentList?.FirstOrDefault(x => x.OmsOrderShipmentId == lineItem.OmsOrderShipmentId)?.ToModel<OrderShipmentModel>();

                                lineItem.ShippingAddressHtml = GetOrderShipmentAddress(lineItem.ZnodeOmsOrderShipment, addressList);
                                //get personalize attributes by omsorderlineitemid
                                lineItem.PersonaliseValueList = GetPersonalizedValueOrderLineItem(Convert.ToInt32(lineItem.ParentOmsOrderLineItemsId) > 0 ? Convert.ToInt32(lineItem.ParentOmsOrderLineItemsId) : lineItem.OmsOrderLineItemsId, personalizeList);
                                lineItem.PersonaliseValuesDetail = orderHelper.GetPersonalizedAttributeLineItemDetails(lineItem.PersonaliseValueList, string.Empty);

                                lineItem.DownloadableProductKey = GetProductKey(lineItem.Sku, lineItem.OmsOrderLineItemsId, downloadableProductkeys);

                                lineItem.AdditionalCost = additionalCostList?.Where(x => x.OmsOrderLineItemsId == lineItem.OmsOrderLineItemsId)?.ToDictionary(x => x.KeyName, y => y.KeyValue);
                            }
                        }
                    }
                }
            }
            if (orderModel?.OrderLineItems?.Count > 0)
                orderModel.OrderLineItems = orderHelper.FormalizeOrderLineItems(orderModel);

            GetOrderTaxSummaryDetails(orderModel.OmsOrderDetailsId, orderModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderModel;
        }

        //Get Download product key of product.
        public virtual string GetProductKey(string sku, decimal quantity, int omsOrderLineItemsId)
        {
            try
            {
                ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { sku = sku, quantity = quantity, omsOrderLineItemsId = omsOrderLineItemsId });
                string productKey = string.Empty;
                bool IsDownloadableSKU = _pimDownloadableProduct.Table.Any(x => x.SKU == sku);

                if (IsDownloadableSKU)
                {
                    var productKeyDetails =
                        from omsDownloadableProductKey in _omsDownloadableProductKey.Table
                        join pimDownloadableProductKey in _pimDownloadableProductKey.Table on omsDownloadableProductKey.PimDownloadableProductKeyId equals pimDownloadableProductKey.PimDownloadableProductKeyId
                        join pimDownloadableProduct in _pimDownloadableProduct.Table on pimDownloadableProductKey.PimDownloadableProductId equals pimDownloadableProduct.PimDownloadableProductId
                        where pimDownloadableProduct.SKU == sku && pimDownloadableProductKey.IsUsed && omsDownloadableProductKey.OmsOrderLineItemsId == omsOrderLineItemsId
                        select new { keys = pimDownloadableProductKey.DownloadableProductKey }.keys;

                    productKey = string.Join(",", productKeyDetails);
                }
                ZnodeLogging.LogMessage("productKey:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, productKey);

                return productKey;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetProductKey method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get Downloadable product key of product
        public virtual string GetProductKey(string sku, int omsOrderLineItemsId, List<string> downloadableProductkeys)
        {
            try
            {
                string productKey = string.Empty;
                if (omsOrderLineItemsId > 0 && string.IsNullOrEmpty(sku) && downloadableProductkeys?.Count > 0)
                {
                    bool IsDownloadableSKU = downloadableProductkeys.Any(x => x == sku);

                    if (IsDownloadableSKU)
                    {
                        var productKeyDetails =
                            from omsDownloadableProductKey in _omsDownloadableProductKey.Table
                            join pimDownloadableProductKey in _pimDownloadableProductKey.Table on omsDownloadableProductKey.PimDownloadableProductKeyId equals pimDownloadableProductKey.PimDownloadableProductKeyId
                            join pimDownloadableProduct in _pimDownloadableProduct.Table on pimDownloadableProductKey.PimDownloadableProductId equals pimDownloadableProduct.PimDownloadableProductId
                            where pimDownloadableProduct.SKU == sku && pimDownloadableProductKey.IsUsed && omsDownloadableProductKey.OmsOrderLineItemsId == omsOrderLineItemsId
                            select new { keys = pimDownloadableProductKey.DownloadableProductKey }.keys;

                        productKey = string.Join(",", productKeyDetails);
                    }
                }
                return productKey;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetProductKey method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get order status that will be shown to customer.
        public virtual string GetOrderStatusForCustomer(OrderLineItemModel lineItem)
        {
            ZnodeLogging.LogMessage("OrderLineItemStateId to get order status for customer: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderLineItemStateId = lineItem?.OrderLineItemStateId });
            IZnodeRepository<ZnodeOmsOrderStateShowToCustomer> _orderStateShowToCustomerRepository = new ZnodeRepository<ZnodeOmsOrderStateShowToCustomer>();
            return _orderStateShowToCustomerRepository.Table.FirstOrDefault(x => x.OmsOrderStateId == lineItem.OrderLineItemStateId)?.OrderStateName;
        }

        //Get order status that will be shown to customer.
        public virtual string GetOrderStatusForCustomer(int? orderLineItemStateId, List<ZnodeOmsOrderStateShowToCustomer> orderStatusList)
        {
            if (orderStatusList?.Count > 0 && orderLineItemStateId > 0)
                return orderStatusList.FirstOrDefault(x => x.OmsOrderStateId == orderLineItemStateId)?.Description;

            return string.Empty;
        }

        //Map Order details data to OrderModel.
        public virtual void MapOrderData(OrderModel orderModel, ZnodeOmsOrderDetail orderDetail, bool isOrderHistory)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                orderModel.CustomerPaymentGUID = orderDetail?.ZnodeUser?.CustomerPaymentGUID;
                orderModel.ShippingId = (orderDetail?.ZnodeShipping?.ShippingId).GetValueOrDefault();
                orderModel.TrackingUrl = orderDetail?.ZnodeShipping?.TrackingUrl;
                int orderShipmentId = (orderModel.OrderLineItems.FirstOrDefault()?.OmsOrderShipmentId).GetValueOrDefault();

                orderModel = GetBillingShippingAddress(orderModel, orderDetail, orderShipmentId);

                if (!string.IsNullOrEmpty(orderModel?.BillingAddress?.StateName))
                    orderModel.BillingAddress.StateCode = new ZnodeRepository<ZnodeState>().Table.FirstOrDefault(x => x.StateCode == orderModel.BillingAddress.StateName)?.StateCode;

                if (IsNotNull(orderModel.BillingAddress))
                {
                    ZnodeAddress billing = _addressRepository.Table.FirstOrDefault(x => x.AddressId == orderModel.BillingAddress.AddressId);
                    //Sets the external id for billing address.
                    orderModel.BillingAddress.ExternalId = billing?.ExternalId;
                    // Set IsDefaultBilling & IsDefaultShipping flags for Billing Address
                    orderModel.BillingAddress.IsDefaultBilling = (billing?.IsDefaultBilling).GetValueOrDefault();
                    orderModel.BillingAddress.IsDefaultShipping = (billing?.IsDefaultShipping).GetValueOrDefault();
                }
                orderModel.ShippingAddress = _orderShipmentRepository.Table.FirstOrDefault(x => x.OmsOrderShipmentId == orderShipmentId)?.ToModel<AddressModel>();

                if (IsNotNull(orderModel.ShippingAddress))
                {
                    ZnodeAddress shipping = _addressRepository.Table.FirstOrDefault(x => x.AddressId == orderModel.ShippingAddress.AddressId);
                    //Sets the external id for shipping address.
                    orderModel.ShippingAddress.ExternalId = shipping?.ExternalId;
                    // Set IsDefaultBilling & IsDefaultShipping flags for Billing Address
                    orderModel.ShippingAddress.IsDefaultBilling = (shipping?.IsDefaultBilling).GetValueOrDefault();
                    orderModel.ShippingAddress.IsDefaultShipping = (shipping?.IsDefaultShipping).GetValueOrDefault();
                }
                //Check UserExpand
                GetUserDetails(orderModel.UserId, orderModel, orderDetail);

                if (isOrderHistory)
                    MapOrderHistory(orderModel);
                GetPaymentHistory(orderModel);
                MapPortalData(orderModel);
                MapShoppingCartData(orderModel);
                if (IsNotNull(orderModel?.ShoppingCartModel?.Shipping) && IsNotNull(orderDetail?.ZnodeShipping))
                {
                    orderModel.ShoppingCartModel.Shipping.ShippingCode = orderDetail.ZnodeShipping.ShippingCode;
                    orderModel.ShoppingCartModel.Shipping.ShippingDiscountDescription = orderDetail.ZnodeShipping.Description;
                }
                GetOrderTaxSummaryDetails(orderDetail.OmsOrderDetailsId, orderModel);

                MapReturnItems(orderModel);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapOrderData method for OrderNumber " + orderModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Sets the external id in address model.
        public virtual void SetExternalId(AddressModel address)
        {
            if (IsNotNull(address))
            {
                address.ExternalId = GetExternalId(address.AddressId);
                ZnodeLogging.LogMessage("AddressModel with:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = address?.AddressId, ExternalId = address?.ExternalId });
            }
        }

        //Gets the external id.
        public virtual string GetExternalId(int addressId)
            => addressId > 0 ? _addressRepository.Table.FirstOrDefault(x => x.AddressId == addressId)?.ExternalId : string.Empty;

        //Map returned order line item.
        public virtual void MapReturnItems(OrderModel orderModel)
        {
            if (IsNotNull(orderModel))
            {
                List<ShoppingCartItemModel> returnCartItems = orderModel.ShoppingCartModel?.ShoppingCartItems.Where(x => x.OrderLineItemStatus.ToUpper() == ZnodeOrderStatusEnum.RETURNED.ToString()).ToList();
                List<OrderDiscountModel> orderDiscountsList = null;
                if (returnCartItems?.Count > 0)
                    orderDiscountsList = orderHelper.GetReturnItemsDiscountList(orderModel.OmsOrderId);
                MapReturnLineItemDetails(returnCartItems, orderDiscountsList, orderModel);
                MapProductNameForGroupProduct(orderModel, returnCartItems);
                orderModel.ShoppingCartModel?.ShoppingCartItems.RemoveAll(x => x.OrderLineItemStatus.ToUpper() == ZnodeOrderStatusEnum.RETURNED.ToString());
                orderModel.ReturnItemList = new ReturnOrderLineItemListModel() { ReturnItemList = GetReturnItemData(returnCartItems, orderModel) };
                ZnodeLogging.LogMessage("ReturnItemList count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ReturnItemListCount = orderModel?.ReturnItemList?.ReturnItemList?.Count });
                if (orderModel.ReturnItemList.ReturnItemList?.Count > 0)
                {
                    orderModel.ReturnItemList.SubTotal = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.ExtendedPrice);
                    orderModel.ReturnItemList.ShippingCost = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.ShippingCost);
                    orderModel.ReturnItemList.TaxCost = orderModel.ReturnTaxCost;
                    orderModel.ReturnItemList.DiscountAmount = orderModel.ReturnItemList.ReturnItemList.Sum(x => (x.PerQuantityLineItemDiscount * x.Quantity) + (x.PerQuantityOrderLevelDiscountOnLineItem * x.Quantity));
                    orderModel.ReturnItemList.CSRDiscount = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.PerQuantityCSRDiscount * x.Quantity);
                    orderModel.ReturnItemList.ShippingDiscount = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.PerQuantityShippingDiscount * x.Quantity);
                    orderModel.ReturnItemList.ReturnCharges = orderModel.ReturnCharges.GetValueOrDefault();
                    orderModel.ReturnItemList.VoucherAmount = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.PerQuantityVoucherAmount * x.Quantity);
                    orderModel.ReturnItemList.ImportDuty = orderModel.ReturnItemList.ReturnItemList.Sum(x => x.ImportDuty);
                }
            }
        }

        protected virtual decimal GetReturnCharges(List<ReturnOrderLineItemModel> returnLineItems)
        {
            decimal returnCharges = 0;
            if (HelperUtility.IsNotNull(returnLineItems) && returnLineItems.Count > 0)
                returnCharges = returnLineItems.Where(x => x.IsShippingReturn == false).Sum(x => x.ShippingCost - (x.PerQuantityShippingDiscount * x.Quantity));
            return returnCharges;
        }

        protected virtual void MapReturnLineItemDetails(List<ShoppingCartItemModel> returnCartItems, List<OrderDiscountModel> orderDiscountsList, OrderModel orderModel)
        {
            if (returnCartItems?.Count > 0)
            {
                List<int> returnLineItemIds = returnCartItems?.Select(x => x.OmsOrderLineItemsId)?.ToList();
                if (returnLineItemIds?.Count > 0)
                {
                    var rmaReturnLineItemList = (from a in new ZnodeRepository<ZnodeRmaReturnLineItem>().Table
                                                 join b in new ZnodeRepository<ZnodeRmaReturnDetail>().Table on a.RmaReturnDetailsId equals b.RmaReturnDetailsId
                                                 where returnLineItemIds.Any(x => x == (int)a.OmsReturnOrderLineItemsId)
                                                 select new
                                                 {
                                                     a.RmaReturnStateId,
                                                     a.OmsOrderLineItemsId,
                                                     b.ReturnNumber,
                                                     b.RmaReturnDetailsId,
                                                     a.OmsReturnOrderLineItemsId,
                                                     b.ReturnTaxCost,
                                                     b.ReturnCharges,
                                                     b.VoucherAmount
                                                 })?.ToList();

                    orderModel.ReturnTaxCost = Convert.ToDecimal(rmaReturnLineItemList?.GroupBy(z => z?.RmaReturnDetailsId)?.Select(x => x.FirstOrDefault()?.ReturnTaxCost)?.Sum());
                    orderModel.ReturnCharges = Convert.ToDecimal(rmaReturnLineItemList?.GroupBy(z => z?.RmaReturnDetailsId)?.Select(x => x.FirstOrDefault()?.ReturnCharges)?.Sum());
                    orderModel.ReturnVoucherAmount = Convert.ToDecimal(rmaReturnLineItemList?.GroupBy(z => z?.RmaReturnDetailsId)?.Select(x => x.FirstOrDefault()?.VoucherAmount)?.Sum());

                    if (rmaReturnLineItemList?.Count > 0)
                    {
                        foreach (ShoppingCartItemModel returnCartItem in returnCartItems)
                        {
                            var rmaReturnLineItem = rmaReturnLineItemList?.FirstOrDefault(x => x.OmsReturnOrderLineItemsId == returnCartItem.OmsOrderLineItemsId);
                            if (IsNotNull(rmaReturnLineItem))
                            {
                                returnCartItem.RmaReturnLineItemStatus = Enum.GetName(typeof(ZnodeReturnStateEnum), rmaReturnLineItem.RmaReturnStateId).Replace('_', ' ').ToLower().ToProperCase();
                                returnCartItem.ReturnNumber = rmaReturnLineItemList?.FirstOrDefault(x => x.RmaReturnDetailsId == rmaReturnLineItem.RmaReturnDetailsId)?.ReturnNumber;
                                orderHelper.SetPerQuantityDiscount(orderDiscountsList, returnCartItem);
                            }
                        }
                    }
                }
            }
        }


        private void MapProductNameForGroupProduct(OrderModel orderModel, List<ShoppingCartItemModel> returnCartItems)
        {
            string productName = orderModel.ShoppingCartModel?.ShoppingCartItems
                                    .Where(p => p.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group)
                                     && p.OrderLineItemStatus != ZnodeOrderStatusEnum.RETURNED.ToString())
                                    .Select(n => n.ProductName)
                                    .FirstOrDefault();
            returnCartItems?.ForEach(m =>
            {
                if (m.GroupProducts?.Count > 0)
                    m.ProductName = productName;
            });
            ZnodeLogging.LogMessage("Return Cart Items details list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { returnCartItemsCount = returnCartItems?.Count });
        }

        //Get returned order line item.
        public virtual List<ReturnOrderLineItemModel> GetReturnItemData(List<ShoppingCartItemModel> returnCartItems, OrderModel orderModel = null)
        {
            List<ReturnOrderLineItemModel> returnItemList = new List<ReturnOrderLineItemModel>();
            if (returnCartItems?.Count > 0)
            {
                returnItemList.AddRange(returnCartItems.Select(x => new ReturnOrderLineItemModel
                {
                    Description = x.Description,
                    ExtendedPrice = x.ExtendedPrice,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    ShippingCost = x.ShippingCost,
                    ProductDiscountAmount = x.ProductDiscountAmount,
                    ShippingOptionId = x.ShippingOptionId,
                    SKU = x.SKU,
                    UnitPrice = x.UnitPrice,
                    CartDescription = x.CartDescription,
                    CurrencyCode = x.CurrencyCode,
                    CultureCode = x.CultureCode,
                    ImagePath = x.ImagePath,
                    MediaConfigurationId = x.MediaConfigurationId,
                    ProductName = x.ProductName,
                    ProductType = x.ProductType,
                    ImageMediumPath = x.ImageMediumPath,
                    AddOnProductSKUs = x.AddOnProductSKUs,
                    BundleProductSKUs = x.BundleProductSKUs,
                    ConfigurableProductSKUs = x.ConfigurableProductSKUs,
                    GroupProducts = x.GroupProducts,
                    ProductCode = x.ProductCode,
                    TrackingNumber = x.TrackingNumber,
                    UOM = x.UOM,
                    IsEditStatus = x.IsEditStatus,
                    ShipSeperately = x.ShipSeperately,
                    OmsOrderStatusId = x.OmsOrderStatusId,
                    OmsOrderLineItemsId = x.OmsOrderLineItemsId,
                    OrderLineItemStatus = x.OrderLineItemStatus,
                    CustomText = x.CustomText,
                    TaxCost = x.TaxCost,
                    ReasonForReturnId = x.RmaReasonForReturnId,
                    ReasonForReturn = x.RmaReasonForReturn,
                    PersonaliseValuesList = x.PersonaliseValuesList,
                    IsShippingReturn = x.IsShippingReturn,
                    RmaReturnLineItemStatus = x.RmaReturnLineItemStatus,
                    ReturnNumber = x.ReturnNumber,
                    PerQuantityLineItemDiscount = x.PerQuantityLineItemDiscount,
                    ParentOmsOrderLineItemsId = x.ParentOmsOrderLineItemsId,
                    PerQuantityCSRDiscount = x.PerQuantityCSRDiscount,
                    PerQuantityShippingCost = x.ShippingCost / x.Quantity,
                    PerQuantityShippingDiscount = x.PerQuantityShippingDiscount,
                    PerQuantityOrderLevelDiscountOnLineItem = x.PerQuantityOrderLevelDiscountOnLineItem,
                    ShipDate = orderModel?.OrderLineItems.FirstOrDefault(y => y.OmsOrderLineItemsId == x.OmsOrderLineItemsId)?.ShipDate,
                    PaymentStatusId = x.PaymentStatusId.GetValueOrDefault(),
                    PerQuantityVoucherAmount = x.PerQuantityVoucherAmount,
                    ImportDuty = x.ImportDuty
                }).ToList());
            }
            ZnodeLogging.LogMessage("returnItemList count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { returnItemListCount = returnItemList?.Count });
            return returnItemList;
        }

        //Map order history from histroy as well as notes.
        public virtual void MapOrderHistory(OrderModel orderModel)
        {
            //SP call to revert order inventory, update this code once dba provide the sp.
            IZnodeViewRepository<OrderHistoryModel> objStoredProc = new ZnodeViewRepository<OrderHistoryModel>();
            objStoredProc.SetParameter("@OrderId", orderModel.OmsOrderId, ParameterDirection.Input, DbType.Int32);
            IList<OrderHistoryModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetOrderHistory @OrderId");
            ZnodeLogging.LogMessage("Order history list count and OmsOrderId to get order history: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderHistoryListCount = list?.Count, OmsOrderId = orderModel?.OmsOrderId });
            orderModel.OrderHistoryList.OrderHistoryList = list?.ToList();
        }

        //Map Portal related data.
        public virtual void MapPortalData(OrderModel orderModel)
        {
            try
            {
                UserModel userInfo = _userService.GetUserById(orderModel.UserId, null);
                ZnodeLogging.LogMessage("orderModel with:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = orderModel?.UserId, PortalId = orderModel?.PortalId });

                if (IsNotNull(userInfo?.PublishCatalogId))
                    orderModel.PortalCatalogId = userInfo.PublishCatalogId.Value;
                else
                    orderModel.PortalCatalogId = (_portalCatalogRepository.Table.FirstOrDefault(x => x.PortalId == orderModel.PortalId)?.PublishCatalogId).GetValueOrDefault();

                orderModel.AccountId = IsNotNull(userInfo) ? userInfo.AccountId.GetValueOrDefault() : 0;
                orderModel.StoreName = _portalRepository.Table?.FirstOrDefault(x => x.PortalId == orderModel.PortalId)?.StoreName;
                orderModel.IsTradeCentricUser = userInfo.IsTradeCentricUser;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapPortalData method for OrderNumber " + orderModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map ShoppingCart related data.
        public virtual void MapShoppingCartData(OrderModel orderModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                orderModel.ShoppingCartModel = GetShoppingCartByOrderId(orderModel.OmsOrderId, orderModel.PortalId, orderModel.UserId, orderModel.PortalCatalogId, orderModel.IsOldOrder);

                if (IsNotNull(orderModel.ShoppingCartModel))
                {
                    orderModel.ShoppingCartModel.ShippingId = orderModel.ShippingId;
                    orderModel.ShoppingCartModel.ShippingAddress = orderModel.ShippingAddress;
                    orderModel.ShoppingCartModel.BillingAddress = orderModel.BillingAddress;
                    orderModel.ShoppingCartModel.Shipping.ShippingId = orderModel.ShippingId;
                    orderModel.ShoppingCartModel.Shipping.ShippingCountryCode = orderModel.ShippingAddress?.CountryName;
                    orderModel.PortalCatalogId = orderModel.ShoppingCartModel.PublishedCatalogId;
                    if (orderModel.IsTaxCostEdited)
                        orderModel.ShoppingCartModel.CustomTaxCost = orderModel.TaxCost;
                    if (orderModel.IsShippingCostEdited)
                        orderModel.ShoppingCartModel.CustomShippingCost = orderModel.ShippingCost;
                    orderModel.GiftCardNumber = orderModel.ShoppingCartModel.GiftCardNumber;
                }

                List<string> downloadableProductkeys = GetDownloadableProductKeyList(orderModel.ShoppingCartModel.ShoppingCartItems?.Select(x => x.SKU)?.Distinct()?.ToList());
                if (downloadableProductkeys?.Count > 0)
                {
                    foreach (ShoppingCartItemModel lineItem in orderModel.ShoppingCartModel.ShoppingCartItems)
                    {
                        bool IsDownloadableSKU = downloadableProductkeys.Any(x => x == lineItem.SKU);
                        if (IsDownloadableSKU)
                        {
                            int? parentOmsOrderLineItemsId = orderModel.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == lineItem.OmsOrderLineItemsId).ParentOmsOrderLineItemsId;
                            List<OrderLineItemModel> downloadableItemlist = orderModel.OrderLineItems.Where(x => x.OmsOrderLineItemsId == parentOmsOrderLineItemsId)?.ToList();
                            if (downloadableItemlist?.Count > 0)
                                foreach (OrderLineItemModel item in downloadableItemlist)
                                    lineItem.DownloadableProductKey = GetProductKey(item.Sku, item.OmsOrderLineItemsId, downloadableProductkeys);
                        }
                    }
                }

                foreach (ShoppingCartItemModel lineItem in orderModel.ShoppingCartModel.ShoppingCartItems)
                {
                    int? parentOmsOrderLineItemsId = orderModel.OrderLineItems.FirstOrDefault(x => x.OmsOrderLineItemsId == lineItem.OmsOrderLineItemsId)?.ParentOmsOrderLineItemsId;

                    lineItem.PaymentStatusId = Convert.ToInt32(orderModel.OrderLineItems.Where(w => w.Sku == lineItem.ConfigurableProductSKUs && w.ParentOmsOrderLineItemsId == parentOmsOrderLineItemsId).Select(s => s.PaymentStatusId)?.FirstOrDefault());
                    lineItem.InventoryTracking = orderModel.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == lineItem.OmsOrderLineItemsId)?.Attributes?.FirstOrDefault(x => x.AttributeCode == "OutOfStockOptions")?.AttributeValueCode;
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapShoppingCartData method for OrderNumber " + orderModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to save order after performing validation and calculation in database
        public virtual OrderModel SaveOrder(ShoppingCartModel model, SubmitOrderModel updateOrderModel = null, bool isTaxCostUpdated = true)
        {
            string orderNumber = string.IsNullOrEmpty(updateOrderModel?.OrderNumber) ? model?.OrderNumber : updateOrderModel?.OrderNumber;
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                ZnodeLogging log = new ZnodeLogging();

                bool isUpdateAction = model.OmsOrderId > 0;
                bool isEnableBcc = false;

                model.OrderAttribute = DefaultGlobalConfigSettingHelper.DefaultOrderAttribute;
                if (model?.PublishStateId < 1)
                    model.PublishStateId = PublishStateId;

                if (string.IsNullOrEmpty(model?.ShippingAddress?.StateCode))
                    SetStateCode(model);

                UserAddressModel userDetails = SetUserDetails(model);
                OrderModel orderModel = null;
                string isInventoryInStockMessage = string.Empty;
                Dictionary<int, string> minMaxSelectableQuantity = new Dictionary<int, string>();

                IZnodeCheckout checkout = SetCheckoutData(userDetails, model, log);

                //this method updates shopping cart data according to checkout calculation
                model = UpdateShoppingCartData(checkout, model);

                // Instantiate the order fulfillment
                ZnodeOrderFulfillment order;
                try
                {

                    //Get refunded line item for CCH tax.
                    GetRefundedLineItemForCCH(model, updateOrderModel);

                    // Perform validation and start the timer
                    ValidateCheckout(checkout);

                    //This block of code prevalidates order processing. This validation is necessary for API worklow
                    //This has been skipped from Create order workflow since all the prevalidations have already been taken care in the checkout process
                    bool isPreOrderValidationSucceeded = PreValidateOrderProcess(checkout, model.SkipPreprocessing, out isInventoryInStockMessage, out minMaxSelectableQuantity);

                    if (isPreOrderValidationSucceeded)
                    {
                        //this block is required for return workflow
                        if (isUpdateAction)
                        {
                            checkout.ShoppingCart.ReturnOrderLineItem(model);
                        }

                        order = checkout.SubmitOrder(updateOrderModel, model, isTaxCostUpdated);

                        //To add order data in orderhistory table if payment status is PendingForReview
                        if (model.Payment.PaymentStatusId == (int)Znode.Engine.Api.Models.Enum.ZnodePaymentStatus.PENDINGFORREVIEW)
                            CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = order.Order.OmsOrderDetailsId, Message = Admin_Resources.OrderPaymentPendingForReviewHistoryMessage, CreatedBy = order.CreatedBy, ModifiedBy = order.ModifiedBy });

                        //Save in Quote if it is quote to order.                  
                        SaveInQuote(model, order);

                        //get order details
                        orderModel = BindOrderData(order, model);

                    }
                    else
                    {
                        log.LogActivityTimerEnd((int)ZnodeLogging.ErrorNum.OrderSubmissionFailed, null);
                        throw new ZnodeException(ErrorCodes.ProcessingFailed, Admin_Resources.ErrorPresubmitOrderProcessing);
                    }
                }
                catch (Exception ex)
                {



                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    if (!string.IsNullOrEmpty(isInventoryInStockMessage))
                        throw new ZnodeException(ErrorCodes.OutOfStockException, Admin_Resources.ErrorPlaceOrder);

                    if (IsNotNull(minMaxSelectableQuantity) && minMaxSelectableQuantity.Count > 0 && minMaxSelectableQuantity.ContainsKey(ErrorCodes.MinAndMaxSelectedQuantityError))
                        throw new ZnodeException(ErrorCodes.MinAndMaxSelectedQuantityError, minMaxSelectableQuantity[ErrorCodes.MinAndMaxSelectedQuantityError]);

                    throw;
                }

                //This block of code has been segregated from the above so as to make sure the none of the block below should cause order failures
                try
                {
                    //Send Order Emails
                    SendOrderEmails(model, checkout, order, orderModel, isUpdateAction, isEnableBcc);

                    InitializeERPConnectorForCreateUpdateOrder(orderModel);

                    //Save downloadable product data
                    SaveDownloadableData(model, isUpdateAction, isEnableBcc, checkout, order, orderModel);

                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                    return orderModel;

                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage($"Error in post order data with order number {orderNumber} with message {ex.Message} ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    return orderModel;

                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SaveOrder method for OrderNumber " + orderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Pre-Validate order process before submission 
        protected virtual bool PreValidateOrderProcess(IZnodeCheckout znodeCheckout, bool skipPreprocessing, out string isInventoryInStockMessage, out Dictionary<int, string> minMaxSelectableQuantity)
        {
            isInventoryInStockMessage = string.Empty;
            minMaxSelectableQuantity = new Dictionary<int, string>();
            if (!skipPreprocessing)
            {
                bool status = znodeCheckout.ShoppingCart.PreSubmitOrderProcess(out isInventoryInStockMessage, out minMaxSelectableQuantity);
                return status;
            }
            return true;
        }

        protected virtual void SaveDownloadableData(ShoppingCartModel shoppingCartModel, bool isUpdateAction, bool isEnableBcc, IZnodeCheckout znodeCheckout, ZnodeOrderFulfillment order, OrderModel orderModel)
        {
            if (!isUpdateAction)
            {
                if (orderModel?.OrderLineItems.Count > 0)
                {
                    List<OrderLineItemModel> downLoadablelineItems = new List<OrderLineItemModel>();
                    foreach (OrderLineItemModel item in orderModel.OrderLineItems)
                    {
                        var status = (item.OrderLineItemCollection?.Where(x => x.IsDownloadableSKU).ToList());
                        if (status?.Count() > 0)
                            downLoadablelineItems.AddRange(status);
                    }
                    if (downLoadablelineItems.Any())
                        //Save Downloadable product key to database
                        SaveDownloadableProductKey(shoppingCartModel, isUpdateAction, isEnableBcc, znodeCheckout, order, orderModel, downLoadablelineItems);

                }
            }
        }

        //Send emails for order
        protected virtual void SendOrderEmails(ShoppingCartModel shoppingCartModel, IZnodeCheckout checkout, ZnodeOrderFulfillment order, OrderModel orderModel, bool isUpdateAction, bool isEnableBcc)
        {
            try
            {
                //Gets comma separated list of required email template codes
                string emailTemplates = GetRequiredOrderEmailTemplates();

                //This is used to fetch all the required email templates based on email template codes
                List<EmailTemplateMapperModel> emailTemplateMapperListModel = GetOrderEmailTemplateListByCodes(emailTemplates, (order.PortalId > 0) ? order.PortalId : PortalId, shoppingCartModel.LocaleId);

                checkout.ShoppingCart.Shipping.ShippingName = GetShippingName(checkout.ShoppingCart.Shipping.ShippingName, checkout.ShoppingCart.Shipping.ShippingID);

                if (!isUpdateAction)
                {
                    // And finally attach the receipt HTML to the order and return.

                    orderModel.ReceiptHtml = GetOrderReceipt(order, checkout, shoppingCartModel.FeedbackUrl, shoppingCartModel.LocaleId, isUpdateAction, out isEnableBcc, shoppingCartModel.UserDetails, emailTemplateMapperListModel);

                    if (!string.IsNullOrEmpty(orderModel.ReceiptHtml) && shoppingCartModel.QuoteTypeCode != ZnodeConstant.Quote)
                    {
                        orderModel.IsEmailSend = SendOrderReceipt(orderModel.PortalId, orderModel.BillingAddress.EmailAddress, $"{Admin_Resources.TitleOrderReceipt} - {orderModel.OrderNumber}", ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeConfigManager.SiteConfig.AdminEmail, orderModel.ReceiptHtml, isEnableBcc);
                        var onOrderPlaceInit = new ZnodeEventNotifier<OrderModel>(orderModel, EventConstant.OnOrderPlaced);
                    }
                    if (checkout.ShoppingCart.LowInventoryProducts?.Count > 0)
                    {
                        SendEmailNotificationForLowInventory(checkout, order, orderModel, shoppingCartModel.LocaleId, emailTemplateMapperListModel);
                        var onOrderPlaceInit = new ZnodeEventNotifier<OrderModel>(orderModel, EventConstant.OnNotificationForLowInventory);
                    }
                }

                //Send purchased product details Email To Vendor.
                SendEmailToVendor(order, checkout, shoppingCartModel.FeedbackUrl, shoppingCartModel.LocaleId, isEnableBcc, emailTemplateMapperListModel);

                //Send order alert email for store notification
                SendEmailNotification(shoppingCartModel, isUpdateAction, checkout, order, orderModel, isEnableBcc, emailTemplateMapperListModel);

                //To send line item state change receipt to user.
                //Only consumed for manage order
                if (isUpdateAction)
                    SendLineItemStateChangeEmail(order, isEnableBcc);

                //to send voucher usage email to shopper.
                if (!isUpdateAction)
                {
                    SendEmailForVoucherUsages(order?.Order, shoppingCartModel.Vouchers, shoppingCartModel.LocaleId, emailTemplateMapperListModel);
                    var onRemainingVoucherBalanceInit = new ZnodeEventNotifier<OrderModel>(orderModel, EventConstant.OnRemainingVoucherBalance);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in SendOrderEmails with order number {order?.Order.OrderNumber} with message {ex.Message} ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);

            }
        }

        // To check tax cost and order status updated to shipped or not.
        protected virtual bool CheckIsTaxUpdated(OrderModel orderModel)
        {
            bool isTaxCostUpdated = orderModel.TaxCost != orderModel.ShoppingCartModel.TaxCost;
            if (!isTaxCostUpdated && orderModel.OrderHistory.ContainsKey(ZnodeConstant.OrderUpdatedStatus)
                && string.Equals(orderModel.OrderHistory[ZnodeConstant.OrderUpdatedStatus], ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                && !string.Equals(orderModel.OrderState, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                isTaxCostUpdated = true;
            }
            return isTaxCostUpdated;
        }

        public void SaveDownloadableProductKey(ShoppingCartModel model, bool isUpdateAction, bool isEnableBcc, IZnodeCheckout checkout, ZnodeOrderFulfillment order, OrderModel orderModel, List<OrderLineItemModel> downLoadablelineItems)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<Tuple<int, string, decimal>> productDetails = SetDownloadableProductKeyDetails(downLoadablelineItems.ToList());
            DataTable orderData = ConvertOrderDataToDataTable(productDetails);

            DataTable keysData = DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled ?
                orderHelper.SaveDownloadableProductKeyWithJSON(orderData, model.UserId.GetValueOrDefault()) :
                orderHelper.SaveDownloadableProductKey(orderData, model.UserId.GetValueOrDefault(), orderModel.OmsOrderDetailsId);

            if (keysData?.Rows?.Count > 0)
            {
                downLoadablelineItems.ForEach(m =>
                {
                    // Get downloadable product keys from SKU.
                    if (string.Equals(m.Sku, Convert.ToString(keysData.Rows[0]["SKU"]), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m.DownloadableProductKey = Convert.ToString(keysData.Rows[0]["DownloadableProductKey"]);
                    }
                });
            }

            //Send order receipt for downloadable product keys.
            SendOrderReceiptForProductKeys(model, isUpdateAction, checkout, order, orderModel, keysData, isEnableBcc);
        }

        //Keep Product key visible during order update
        private void UpdateProductKey(ShoppingCartModel model, OrderModel orderModel)
        {
            int oldOmsOrderLineItemsId = model.ShoppingCartItems.FirstOrDefault().OmsOrderLineItemsId;
            var productKeyDetail = _omsDownloadableProductKey.Table.Where(x => x.OmsOrderLineItemsId == oldOmsOrderLineItemsId)?.ToList();
            int newOmsOrderLineItemsId = orderModel.OrderLineItems.FirstOrDefault().OmsOrderLineItemsId;
            if (IsNotNull(productKeyDetail))
            {
                productKeyDetail.ForEach(x => x.OmsOrderLineItemsId = newOmsOrderLineItemsId);
                productKeyDetail.ForEach(x => _omsDownloadableProductKey.Update(x));
            }
        }

        //Send order receipt for downloadable product keys.
        public void SendOrderReceiptForProductKeys(ShoppingCartModel model, bool isUpdateAction, IZnodeCheckout checkout, ZnodeOrderFulfillment order, OrderModel orderModel, DataTable keysData, bool isEnableBcc = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            DownloadableProductKeyListModel keys;
            keys = ConvertToParamList(keysData);
            if (keys?.DownloadableProductKeys?.Count > 0)
            {
                // And finally attach the receipt HTML to the order and return.
                orderModel.KeyReceiptHtml = GetDownloadableProductOrderReceipt(order, checkout, model.FeedbackUrl, model.LocaleId, keys, isUpdateAction, out isEnableBcc);

                if (!string.IsNullOrEmpty(orderModel.KeyReceiptHtml))
                    SendOrderReceipt(orderModel.PortalId, orderModel.BillingAddress.EmailAddress, $"{Admin_Resources.TitleOrderReceipt} - {orderModel.OrderNumber}", ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeConfigManager.SiteConfig.AdminEmail, orderModel.KeyReceiptHtml, isEnableBcc);
                var OnProductKeyOrderReceiptInit = new ZnodeEventNotifier<OrderModel>(orderModel, EventConstant.OnProductKeyOrderReceipt);
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //This method will convert the datable to List model.
        public DownloadableProductKeyListModel ConvertToParamList(DataTable dataTable)
        {
            DownloadableProductKeyListModel model = new DownloadableProductKeyListModel();
            model.DownloadableProductKeys = new List<DownloadableProductKeyModel>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                    model.DownloadableProductKeys.Add(new DownloadableProductKeyModel { SKU = dr["SKU"].ToString(), DownloadableProductKey = dr["DownloadableProductKey"].ToString(), OmsOrderLineItemsId = Convert.ToInt32(dr["OmsOrderLineItemsId"].ToString()), DownloadableProductURL = dr["DownloadableProductURL"].ToString(), ProductName = dr["ProductName"].ToString() });
            }
            ZnodeLogging.LogMessage("DownloadableProductKeys list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { DownloadableProductKeysListCount = model?.DownloadableProductKeys?.Count });
            return model;
        }

        //Set Downloadable Product Key Details to tuples
        public List<Tuple<int, string, decimal>> SetDownloadableProductKeyDetails(List<OrderLineItemModel> OrderLineItem)
        {
            ZnodeLogging.LogMessage("OrderLineItem list count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderLineItemListCount = OrderLineItem?.Count });
            List<Tuple<int, string, decimal>> productKeyDetails = new List<Tuple<int, string, decimal>>();

            foreach (OrderLineItemModel product in OrderLineItem)
            {
                int orderLineItemId = product.OmsOrderLineItemsId;
                string sku = product.Sku;
                decimal quantity = product.Quantity;

                productKeyDetails.Add(new Tuple<int, string, decimal>(orderLineItemId, sku, quantity));
            }
            ZnodeLogging.LogMessage("productKeyDetails list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { productKeyDetailsListCount = productKeyDetails?.Count });
            return productKeyDetails;
        }

        // te data table  and item into it.
        public DataTable ConvertOrderDataToDataTable(List<Tuple<int, string, decimal>> productDetails)
        {
            DataTable table = new DataTable("OMSDownloadableProduct");
            table.Columns.Add("OmsOrderLineItemsId", typeof(int));
            table.Columns.Add("SKU", typeof(string));
            table.Columns.Add("Quantity", typeof(decimal));

            foreach (Tuple<int, string, decimal> item in productDetails)
                table.Rows.Add(item.Item1, item.Item2, item.Item3);

            return table;
        }

        //Get refunded line item for cch tax.
        public virtual void GetRefundedLineItemForCCH(ShoppingCartModel model, SubmitOrderModel updateordermodel)
        {
            try
            {
                if (updateordermodel?.ReturnOrderLineItems?.ReturnItemList?.Count > 0)
                {
                    string[] refundedSkusList = updateordermodel.RefundedSkus.Split(',');
                    model.ReturnItemList = IsNotNull(refundedSkusList)
                        ? updateordermodel?.ReturnOrderLineItems?.ReturnItemList.Where(c => refundedSkusList.Contains(Convert.ToString(c.OmsOrderLineItemsId))).ToList()
                        : updateordermodel.ReturnOrderLineItems.ReturnItemList;
                    model.IsCchCalculate = true;
                    ZnodeLogging.LogMessage("ReturnItemList Count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { ReturnItemListCount = model?.ReturnItemList });
                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetRefundedLineItemForCCH method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // Get shipping name.
        public virtual string GetShippingName(string shippingName, int shippingId) => !string.IsNullOrEmpty(shippingName) ? shippingName : Convert.ToString((_shippingRepository.Table.Where(w => w.ShippingId == shippingId).Select(s => s.Description).FirstOrDefault()));

        //to get shipping address
        public virtual string GetOrderShipmentAddress(OrderShipmentModel orderShipment)
        {
            try
            {
                if (IsNotNull(orderShipment))
                {
                    ZnodeLogging.LogMessage("AddressId to get shipping company name", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = orderShipment.AddressId });
                    string ShippingcompanyName = _addressRepository.Table.FirstOrDefault(x => x.AddressId == orderShipment.AddressId)?.CompanyName;

                    string street1 = string.IsNullOrEmpty(orderShipment.ShipToStreet2) ? string.Empty : "<br />" + orderShipment.ShipToStreet2;
                    orderShipment.ShipToCompanyName = IsNotNull(orderShipment?.ShipToCompanyName) ? $"{orderShipment?.ShipToCompanyName}{"<br />"}" : ShippingcompanyName;
                    return $"{orderShipment?.ShipToFirstName}{" "}{ orderShipment?.ShipToLastName}{"<br />"}{ orderShipment.ShipToCompanyName}{"<br />"}{orderShipment.ShipToStreet1}{street1}{"<br />"}{ orderShipment.ShipToCity}{"<br />"}{orderShipment.ShipToStateCode}{"<br />"}{orderShipment.ShipToPostalCode}{"<br />"}{orderShipment.ShipToCountry}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{orderShipment.ShipToPhoneNumber}{"<br />"}{WebStore_Resources.TitleEmail}{" : "}{orderShipment.ShipToEmailId}";
                }
                return string.Empty;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetOrderShipmentAddress method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //To get shipping address
        public virtual string GetOrderShipmentAddress(OrderShipmentModel orderShipment, List<ZnodeAddress> addressList)
        {
            try
            {
                string shippingCompanyName = string.Empty;
                if (IsNotNull(orderShipment))
                {
                    if (addressList?.Count > 0)
                    {
                        ZnodeLogging.LogMessage("AddressId to get shipping company name", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = orderShipment.AddressId });
                        shippingCompanyName = addressList.FirstOrDefault(x => x.AddressId == orderShipment.AddressId)?.CompanyName;
                    }

                    string street1 = string.IsNullOrEmpty(orderShipment.ShipToStreet2) ? string.Empty : "<br />" + orderShipment.ShipToStreet2;
                    orderShipment.ShipToCompanyName = IsNotNull(orderShipment?.ShipToCompanyName) ? $"{orderShipment?.ShipToCompanyName}" : shippingCompanyName;
                    string CompanyName = string.IsNullOrEmpty(orderShipment.ShipToCompanyName) ? string.Empty : orderShipment.ShipToCompanyName + "<br />";
                    return $"{orderShipment?.ShipToFirstName}{" "}{ orderShipment?.ShipToLastName}{"<br />"}{CompanyName}{orderShipment.ShipToStreet1}{street1}{"<br />"}{ orderShipment.ShipToCity}{", "}{orderShipment.ShipToStateCode}{", "}{orderShipment.ShipToCountry}{" "}{orderShipment.ShipToPostalCode}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{orderShipment.ShipToPhoneNumber}{"<br />"}{WebStore_Resources.TitleEmail}{" : "}{orderShipment.ShipToEmailId}";
                }
                return string.Empty;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetOrderShipmentAddress method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to get shipping address
        public virtual string GetOrderBillingAddress(OrderModel orderBilling)
        {
            try
            {
                if (IsNotNull(orderBilling))
                {
                    string street1 = string.IsNullOrEmpty(orderBilling.BillingAddress.Address2) ? string.Empty : "<br />" + orderBilling.BillingAddress.Address2;
                    string CompanyName = string.IsNullOrEmpty(orderBilling.BillingAddress.CompanyName) ? string.Empty : orderBilling.BillingAddress.CompanyName + "<br />";
                    return $"{orderBilling?.BillingAddress.FirstName}{" "}{orderBilling?.BillingAddress.LastName}{"<br />"}{CompanyName}{orderBilling.BillingAddress.Address1}{street1}{"<br />"}{ orderBilling.BillingAddress.CityName}{", "}{(string.IsNullOrEmpty(orderBilling.BillingAddress.StateCode) ? orderBilling.BillingAddress.StateName : orderBilling.BillingAddress.StateCode)}{", "}{orderBilling.BillingAddress.CountryName}{" "}{orderBilling.BillingAddress.PostalCode}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{orderBilling.BillingAddress.PhoneNumber}";
                }
                return string.Empty;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetOrderBillingAddress method for OrderNumber " + orderBilling?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get expands and add them to navigation properties
        public virtual List<string> GetExpands(NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<string> navigationProperties = new List<string>();
            ZnodeLogging.LogMessage("Expands details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, expands);
            if (expands?.HasKeys() ?? false)
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    if (Equals(key.ToLower(), ZnodeOmsOrderEnum.ZnodeOmsOrderDetails.ToString().ToLower())) SetExpands(ZnodeOmsOrderEnum.ZnodeOmsOrderDetails.ToString(), navigationProperties);
                    if (Equals(key.ToLower(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderShipment.ToString().ToLower())) SetExpands(ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderShipment.ToString(), navigationProperties);
                    if (Equals(key.ToLower(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderAttributes.ToString().ToLower())) SetExpands(ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderAttributes.ToString(), navigationProperties);
                    if (Equals(key, ZnodeOmsOrderDetailEnum.ZnodeOmsPaymentState.ToString().ToLower())) SetExpands(ZnodeOmsOrderDetailEnum.ZnodeOmsPaymentState.ToString(), navigationProperties);
                    if (Equals(key, ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString().ToLower())) SetExpands(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString(), navigationProperties);
                    if (Equals(key, ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString().ToLower())) SetExpands(ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString(), navigationProperties);
                    if (Equals(key, ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower())) SetExpands(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.ZnodeUser)) SetExpands(ExpandKeys.ZnodeUser, navigationProperties);
                    if (Equals(key, ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString().ToLower())) SetExpands(ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.ZnodeShipping)) SetExpands(ExpandKeys.ZnodeShipping, navigationProperties);
                    if (Equals(key, ExpandKeys.ZnodeOmsOrderLinePersonalize)) SetExpands(ExpandKeys.ZnodeOmsOrderLinePersonalize, navigationProperties);
                    if (Equals(key, ExpandKeys.ZnodeOmsNotes)) SetExpands(ExpandKeys.ZnodeOmsNotes, navigationProperties);
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return navigationProperties;
        }

        //Updates the Shopping cart data accordingly to checkout data.
        public virtual ShoppingCartModel UpdateShoppingCartData(IZnodeCheckout checkout, ShoppingCartModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                List<Entities.ZnodeVoucher> vouchers = checkout.ShoppingCart.Vouchers;

                if (IsNotNull(checkout?.ShoppingCart?.Vouchers))
                {
                    model.Vouchers = new List<VoucherModel>();
                    foreach (Entities.ZnodeVoucher voucher in vouchers)
                        model.Vouchers.Add(VoucherMap.ToVoucherModel(voucher));
                }
                return model;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in UpdateShoppingCartData method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw ;
            }
        }

        //To set shopping cart data to checkout object.
        public virtual IZnodeCheckout SetShoppingCartDataToCheckout(IZnodeCheckout checkout, ShoppingCartModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                checkout.ShoppingCart.LocalId = model.LocaleId;
                checkout.ShoppingCart.PublishedCatalogId = model.PublishedCatalogId;
                checkout.ShoppingCart.OrderDate = model.OrderDate;
                checkout.ShoppingCart.GiftCardAmount = model.GiftCardAmount;
                checkout.ShoppingCart.GiftCardMessage = model.GiftCardMessage;
                checkout.ShoppingCart.GiftCardNumber = model.GiftCardNumber;
                checkout.ShoppingCart.IsGiftCardApplied = model.GiftCardApplied;
                checkout.ShoppingCart.IsGiftCardValid = model.GiftCardValid;
                checkout.ShoppingCart.CreditCardNumber = model.CreditCardNumber;
                checkout.ShoppingCart.CSRDiscountAmount = model.CSRDiscountAmount;
                checkout.ShoppingCart.CSRDiscountDescription = model.CSRDiscountDescription;
                checkout.ShoppingCart.CSRDiscountApplied = model.CSRDiscountApplied;
                checkout.ShoppingCart.CSRDiscountMessage = model.CSRDiscountMessage;
                checkout.ShoppingCart.CustomShippingCost = model.CustomShippingCost;
                checkout.ShoppingCart.CustomTaxCost = model.CustomTaxCost;
                checkout.ShoppingCart.OrderAttribute = model.OrderAttribute;
                checkout.ShoppingCart.CurrencyCode = model.CurrencyCode;
                checkout.ShoppingCart.CultureCode = model.CultureCode;
                checkout.ShoppingCart.UserId = model.UserId;
                checkout.ShoppingCart.ExternalId = model.ExternalId;
                checkout.ShoppingCart.CardType = model.CardType;
                checkout.ShoppingCart.CreditCardExpMonth = model.CreditCardExpMonth;
                checkout.ShoppingCart.CreditCardExpYear = model.CreditCardExpYear;
                checkout.ShoppingCart.LoginUserName = model.UserDetails?.LoginName ?? string.Empty;
                checkout.ShoppingCart.IsLineItemReturned = model.IsLineItemReturned;
                if (IsNotNull(model?.Coupons))
                {
                    foreach (CouponModel coupon in model.Coupons)
                        checkout.ShoppingCart.Coupons.Add(CouponMap.ToZnodeCoupon(coupon));
                }

                if (IsNotNull(model?.Vouchers))
                {
                    foreach (VoucherModel voucher in model.Vouchers)
                        checkout.ShoppingCart.Vouchers.Add(VoucherMap.ToZnodeVoucher(voucher));
                }
                checkout.ShoppingCart.IsCalculateVoucher = model.IsCalculateVoucher;
                checkout.ShoppingCart.IsAllVoucherRemoved = model.IsAllVoucherRemoved;
                checkout.ShoppingCart.PortalID = model.PortalId;
                checkout.ShoppingCart.VAT = model.Vat.GetValueOrDefault();
                checkout.ShoppingCart.HST = model.Hst.GetValueOrDefault();
                checkout.ShoppingCart.GST = model.Gst.GetValueOrDefault();
                checkout.ShoppingCart.PST = model.Pst.GetValueOrDefault();
                checkout.ShoppingCart.ImportDuty = model.ImportDuty.GetValueOrDefault();
                checkout.ShoppingCart.TaxSummaryList = model.TaxSummaryList;
                checkout.ShoppingCart.TaxMessageList = model.TaxMessageList;
                checkout.AdditionalInstructions = model.AdditionalInstructions;
                checkout.PurchaseOrderNumber = model.PurchaseOrderNumber;
                checkout.PoDocument = model.PODocumentName;
                checkout.PortalID = model.PortalId;
                checkout.ShoppingCart.Payment = PaymentMap.ToZnodePayment(model.Payment);
                checkout.ShoppingCart.Shipping = ShippingMap.ToZnodeShipping(model.Shipping);
                checkout.ShippingID = checkout.ShoppingCart.Shipping.ShippingID;
                checkout.ShoppingCart.Payment.PaymentSettingId = model.Payment?.PaymentSetting == null || model.Payment?.PaymentSetting.PaymentSettingId == 0 ? null : model.Payment?.PaymentSetting.PaymentSettingId;
                checkout.ShoppingCart.ReturnItemList = model.ReturnItemList;
                checkout.ShoppingCart.IsCchCalculate = model.IsCchCalculate;
                checkout.ShoppingCart.IsAllowWithOtherPromotionsAndCoupons = model.IsAllowWithOtherPromotionsAndCoupons;
                checkout.ShoppingCart.EstimateShippingCost = model.EstimateShippingCost;
                checkout.ShoppingCart.IpAddress = model.IpAddress;
                checkout.ShoppingCart.InHandDate = model.InHandDate;
                checkout.ShoppingCart.JobName = model.JobName;
                checkout.ShoppingCart.ShippingConstraintCode = model.ShippingConstraintCode;
                checkout.ShoppingCart.Custom1 = model.Custom1;
                checkout.ShoppingCart.Custom2 = model.Custom2;
                checkout.ShoppingCart.Custom3 = model.Custom3;
                checkout.ShoppingCart.Custom4 = model.Custom4;
                checkout.ShoppingCart.Custom5 = model.Custom5;
                checkout.ShoppingCart.IsShippingRecalculate = model.IsShippingRecalculate;
                checkout.ShoppingCart.ShippingDiscount = IsNotNull(model.Shipping) ? model.Shipping.ShippingDiscount : 0;
                checkout.ShoppingCart.ShippingHandlingCharges = IsNotNull(model.Shipping.ShippingHandlingCharge == 0) ? model.ShippingHandlingCharges : model.Shipping.ShippingHandlingCharge;
                checkout.ShoppingCart.ReturnCharges = model.ReturnCharges;
                checkout.ShoppingCart.IsFromAdminOrder = model.IsFromAdminOrder;
                checkout.ShoppingCart.IsQuoteToOrder = model.IsQuoteToOrder;
                checkout.ShoppingCart.IsOldOrder = model.IsOldOrder;
                checkout.ShoppingCart.IsRemoveShippingDiscount = model.IsRemoveShippingDiscount;
                checkout.ShoppingCart.IsPendingOrderRequest = model.IsPendingOrderRequest;
                //Set the flag for webstore order validations.
                checkout.ShoppingCart.SkipPreprocessing = model.SkipPreprocessing;
                if (IsNotNull(model.OrderShipment))
                {
                    // Do the cart calculation
                    checkout.ShoppingCart.AddressCarts.ForEach(x =>
                    {
                        x.Shipping = string.IsNullOrEmpty(x.Shipping.ShippingName) ? new Libraries.ECommerce.Entities.ZnodeShipping
                        {
                            ShippingID = checkout.ShoppingCart.Shipping.ShippingID,
                            ShippingName = checkout.ShoppingCart.Shipping.ShippingName,
                            ShippingCountryCode = checkout?.ShoppingCart?.Shipping.ShippingCountryCode
                        } : x.Shipping;
                        var address = _addressRepository.GetById(x.AddressID);
                        checkout.ShoppingCart.Payment = PaymentMap.ToZnodePayment(model.Payment, address.ToModel<AddressModel>());
                        x.Payment = checkout.ShoppingCart.Payment;
                        x.PortalId = checkout.PortalID;
                        x.UserId = checkout.ShoppingCart.UserId;
                        x.CurrencyCode = checkout.ShoppingCart.CurrencyCode;
                        x.CultureCode = checkout.ShoppingCart.CultureCode;
                        x.Coupons = checkout.ShoppingCart.Coupons;
                        x.PublishStateId = checkout.ShoppingCart.PublishStateId;
                        x.IsAllowWithOtherPromotionsAndCoupons = checkout.ShoppingCart.IsAllowWithOtherPromotionsAndCoupons;
                        x.InvalidOrderLevelShippingDiscount = checkout.ShoppingCart?.InvalidOrderLevelShippingDiscount;
                        x.InvalidOrderLevelShippingPromotion = checkout.ShoppingCart?.InvalidOrderLevelShippingPromotion;
                        x.IsShippingDiscountRecalculate = checkout.ShoppingCart.IsShippingDiscountRecalculate;
                        x.OrderLevelShipping = checkout.ShoppingCart.OrderLevelShipping;
                        x.Calculate();
                    });
                }
                else
                {
                    // Do the cart calculation
                    checkout.ShoppingCart.AddressCarts.ForEach(x =>
                    {
                        x.Shipping = string.IsNullOrEmpty(x.Shipping.ShippingName) ? new Libraries.ECommerce.Entities.ZnodeShipping
                        {
                            ShippingID = checkout.ShoppingCart.Shipping.ShippingID,
                            ShippingName = checkout.ShoppingCart.Shipping.ShippingName,
                            ShippingCountryCode = checkout?.ShoppingCart?.Shipping.ShippingCountryCode
                        } : x.Shipping;
                        x.Payment = checkout.ShoppingCart.Payment;
                        x.PortalId = checkout.PortalID;
                        x.UserId = checkout.ShoppingCart.UserId;
                        x.CurrencyCode = checkout.ShoppingCart.CurrencyCode;
                        x.CultureCode = checkout.ShoppingCart.CultureCode;
                        x.OrderId = checkout.ShoppingCart.OrderId;
                        x.IsCchCalculate = checkout.ShoppingCart.IsCchCalculate;
                        x.ReturnItemList = checkout.ShoppingCart.ReturnItemList;
                        x.OrderDate = checkout.ShoppingCart.OrderDate;
                        x.Coupons = checkout.ShoppingCart.Coupons;
                        x.PublishStateId = checkout.ShoppingCart.PublishStateId;
                        x.IsAllowWithOtherPromotionsAndCoupons = checkout.ShoppingCart.IsAllowWithOtherPromotionsAndCoupons;
                        x.InvalidOrderLevelShippingDiscount = checkout.ShoppingCart?.InvalidOrderLevelShippingDiscount;
                        x.InvalidOrderLevelShippingPromotion = checkout.ShoppingCart?.InvalidOrderLevelShippingPromotion;
                        x.IsShippingDiscountRecalculate = checkout.ShoppingCart.IsShippingDiscountRecalculate;
                        x.OrderLevelShipping = checkout.ShoppingCart.OrderLevelShipping;
                    });
                }

                CalculateCheckoutCart(checkout, model);

                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return checkout;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetShoppingCartDataToCheckout method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //To revert product inventory for updating existion order .
        public virtual bool RevertOrderInventory(int orderId, int? userId, string omsOrderLineitemIds = "", int isRevertAll = 0)
        {
            try
            {
                if (orderId > 0)
                {
                    ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, userId = userId, omsOrderLineitemIds = omsOrderLineitemIds, isRevertAll = isRevertAll });
                    int? omsOrderdetailId = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == orderId && x.IsActive)?.FirstOrDefault().OmsOrderDetailsId;

                    //SP call to revert order inventory, update this code once dba provide the sp.
                    IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                    objStoredProc.SetParameter(ZnodeOmsOrderDetailEnum.OmsOrderDetailsId.ToString(), omsOrderdetailId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter("OmsOrderLineItemIds", omsOrderLineitemIds, ParameterDirection.Input, DbType.String);
                    objStoredProc.SetParameter(View_ReturnBooleanEnum.Status.ToString(), null, ParameterDirection.Output, DbType.Int32);
                    objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), userId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter("IsRevertAll", isRevertAll, ParameterDirection.Input, DbType.Int32);
                    int status = 0;
                    objStoredProc.ExecuteStoredProcedureList("Znode_RevertOrderInventory @OmsOrderDetailsId, @OmsOrderLineItemIds, @Status OUT, @UserId,@IsRevertAll", 2, out status);

                    if (status == 1)
                        ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessOrderInventoryRevert, orderId), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    else
                        ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorOrderInventoryRevert, orderId), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return status == 1;
                }
                return false;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in RevertOrderInventory method for orderId " + orderId + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get order details by using expands.
        public virtual void GetExpands(NameValueCollection expands, OrderModel order)
        {
            if (expands.HasKeys())
            {
                ExpandOrderLineItems(expands, order);
                ExpandStore(expands, order);
                ExpandPaymentType(expands, order);
                ExpandOmsOrderState(expands, order);
                ExpandShoppingCart(expands, order);
                ExpandPaymentState(expands, order);
                ExpandShippingType(expands, order);
                ExpandOmsOrderNote(expands, order);
                ExpandUserDetails(expands, order);
                ExpandPaymentStatusList(expands, order);
            }
        }

        //Set shopping cart items by order line item details.
        public virtual void ExpandShoppingCart(NameValueCollection expands, OrderModel order)
        {
            ZnodeLogging.LogMessage("expands details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, expands);
            if (!String.IsNullOrEmpty(expands.Get(ExpandKeys.ShoppingCart)))
            {
                //Get order details as shopping cart by order id.
                ShoppingCartModel shoppingCart = GetShoppingCartByOrderId(order.OmsOrderId, order.PortalId, order.UserId, order.PortalCatalogId, order.IsOldOrder);

                if (IsNotNull(shoppingCart))
                    order.ShoppingCartModel = shoppingCart;
            }
        }

        //Get order details as shopping cart by order id.
        public virtual ShoppingCartModel GetShoppingCartByOrderId(int orderId, int portalId, int userId, int portalCatalogId, bool isOldOrder = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage(" Input parameters OrderId, portalId, userId, portalCatalogId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderId, portalId, userId, portalCatalogId });
            //Get catalog list by portal id.
            if (portalCatalogId <= 0)
                portalCatalogId = (_portalCatalogRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.PublishCatalogId).GetValueOrDefault();

            IShoppingCartService _shoppingCartService = GetService<IShoppingCartService>();

            //Get shopping cart model by using orderId.
            ShoppingCartModel shoppingCart = _shoppingCartService.GetShoppingCart(new CartParameterModel
            {
                LocaleId = GetLocaleIdFromHeader(),
                PortalId = portalId,
                UserId = userId,
                PublishedCatalogId = portalCatalogId > 0 ? portalCatalogId : 0,
                OmsOrderId = orderId,
                IsOldOrder = isOldOrder
            });
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return shoppingCart;
        }

        //Get order line item by order id.
        public virtual void ExpandOrderLineItems(NameValueCollection expands, OrderModel order)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (!String.IsNullOrEmpty(expands.Get(ExpandKeys.OrderLineItems)))
            {
                //Expand for oms order shipment.
                expands.Add(ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderShipment.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderShipment.ToString());
                //Expand for oms order attributes.
                expands.Add(ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderAttributes.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderAttributes.ToString());

                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeOmsOrderLineItemEnum.OmsOrderDetailsId.ToString(), FilterOperators.In, order.OmsOrderDetailsId.ToString()));
                string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
                ZnodeLogging.LogMessage("WhereClause for GetEntityList:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, whereClause);
                List<ZnodeOmsOrderLineItem> orderLineItemList = _orderLineItemRepository.GetEntityList(whereClause, GetExpands(expands))?.ToList();
                //Map the Order Line Item to Order Model.
                if (IsNotNull(orderLineItemList))
                {
                    if (IsNull(order.OrderLineItems))
                        order.OrderLineItems = new List<OrderLineItemModel>();

                    foreach (ZnodeOmsOrderLineItem orderLineItem in orderLineItemList)
                        order.OrderLineItems.Add(orderLineItem.ToModel<OrderLineItemModel>());
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get order shipment details by OmsOrderShipmentId.
        public virtual void ExpandOrderShipment(NameValueCollection expands, OrderLineItemModel orderLineItemModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (!string.IsNullOrEmpty(expands.Get(ExpandKeys.OrderShipment)))
            {
                List<ZnodeOmsOrderShipment> orderShipmentList = _orderShipmentRepository.Table.Where(w => w.OmsOrderShipmentId == orderLineItemModel.OmsOrderShipmentId).ToList();
                ZnodeLogging.LogMessage("orderShipmentList count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderShipmentList?.Count);

                //Map the Order Line Item to Order Model.
                if (IsNotNull(orderShipmentList))
                {
                    foreach (ZnodeOmsOrderShipment orderShipment in orderShipmentList)
                        orderLineItemModel.ZnodeOmsOrderShipment = orderShipment.ToModel<OrderShipmentModel>();
                }
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get store name by portal id.
        public virtual void ExpandStore(NameValueCollection expands, OrderModel order)
        {
            ZnodeLogging.LogMessage("OrderModel with PortalId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, order?.PortalId);

            if (!String.IsNullOrEmpty(expands.Get(ExpandKeys.Store)))
            {
                ZnodePortal portal = _portalRepository.GetById(order.PortalId);

                //Map the Store name to order model.
                if (IsNotNull(portal))
                    order.StoreName = portal.StoreName;
            }
        }

        //Get Payment state name by Payment Status Id.
        public virtual void ExpandPaymentState(NameValueCollection expands, OrderModel order)
        {

            if (!string.IsNullOrEmpty(expands.Get(ExpandKeys.OmsPaymentState)))
            {
                IZnodeRepository<ZnodeOmsPaymentState> _paymentStateRepository = new ZnodeRepository<ZnodeOmsPaymentState>();
                ZnodeOmsPaymentState portal = _paymentStateRepository.Table.Where(x => x.OmsPaymentStateId == order.OmsPaymentStateId)?.FirstOrDefault();

                //Map the Payment status name to order model.
                if (IsNotNull(portal))
                    order.PaymentStatus = portal.Name;
            }
        }

        //Get payment type name by payment type id.
        public virtual void ExpandPaymentType(NameValueCollection expands, OrderModel order)
        {
            if (!String.IsNullOrEmpty(expands.Get(ExpandKeys.PaymentType)))
            {
                IZnodeRepository<ZnodePaymentType> _paymentTypeRepository = new ZnodeRepository<ZnodePaymentType>();
                ZnodePaymentType paymentType = _paymentTypeRepository.Table.Where(w => w.PaymentTypeId == order.PaymentTypeId)?.FirstOrDefault();

                //Map the payment type name to order model.
                if (IsNotNull(paymentType))
                    order.PaymentType = paymentType.Name;
            }
        }

        //Get username, user first and last name by payment type id.
        public virtual void ExpandUserDetails(NameValueCollection expands, OrderModel order)
        {
            if (!String.IsNullOrEmpty(expands.Get(ExpandKeys.UserDetails)))
                GetUserDetails(order.UserId, order);
        }

        //Get user details by id.
        public virtual void GetUserDetails(int userId, OrderModel order)
        {
            try
            {
                ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId });

                IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();

                ZnodeLogging.LogMessage("UserId to get user details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = order?.UserId });
                UserModel userDetails = GetUserNameByUserId(order.UserId);
                if (IsNotNull(order) && IsNotNull(userDetails))
                {
                    order.FirstName = userDetails.FirstName;
                    order.LastName = userDetails.LastName;
                    order.UserName = userDetails.UserName;
                    order.PhoneNumber = userDetails.PhoneNumber;
                    order.CreatedByName = GetUserNameByUserId(order.CreatedBy)?.UserName ?? order.UserName;
                }
                else
                {

                    string email = _userRepository.Table.FirstOrDefault(x => x.UserId == order.UserId)?.Email;

                    if (string.IsNullOrEmpty(email))
                        order.UserName = GetUserNameByUserId(order.CreatedBy)?.UserName ?? email;

                    if (!string.IsNullOrEmpty(email))
                        order.CreatedByName = email;
                    else
                        order.CreatedByName = order.UserName;
                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetUserDetails method for OrderNumber " + order?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get payment status list.
        public virtual void ExpandPaymentStatusList(NameValueCollection expands, OrderModel order)
        {
            if (!string.IsNullOrEmpty(expands.Get(ExpandKeys.PaymentStateList)))
            {
                IZnodeRepository<ZnodeOmsPaymentState> _omsPaymentStateRepository = new ZnodeRepository<ZnodeOmsPaymentState>();
                order.OrderPaymentStateModelList = _omsPaymentStateRepository.GetEntityList(string.Empty).ToModel<OrderPaymentStateModel>().ToList();
            }
        }

        //Get shipping type by shipping type id.
        public virtual void ExpandShippingType(NameValueCollection expands, OrderModel order)
        {
            if (!String.IsNullOrEmpty(expands.Get(ExpandKeys.ShippingType)) && order.ShippingId > 0)
                order.ShippingTypeName = _shippingRepository.Table.FirstOrDefault(w => w.ShippingId == order.ShippingId)?.Description;
        }

        public virtual void ExpandOmsOrderState(NameValueCollection expands, OrderModel order)
        {
            if (!String.IsNullOrEmpty(expands.Get(ExpandKeys.OmsOrderState)))
            {
                IZnodeRepository<ZnodeOmsOrderState> _omsOrderStateTypeRepository = new ZnodeRepository<ZnodeOmsOrderState>();
                ZnodeOmsOrderState omsOrderState = _omsOrderStateTypeRepository.Table.FirstOrDefault(w => w.OmsOrderStateId == order.OmsOrderStateId);

                //Map the order state name to order model.
                if (IsNotNull(omsOrderState))
                    order.OrderState = omsOrderState.OrderStateName;
            }
        }

        //Get Oms Order Note details.
        public virtual void ExpandOmsOrderNote(NameValueCollection expands, OrderModel order)
           => order.OrderNotes = !String.IsNullOrEmpty(expands.Get(ExpandKeys.OrderNotes)) ?
                GetOrderNoteDetails(order.OmsOrderId, 0) : new List<OrderNotesModel>();

        //Assign UserId for Shipping Billing Address.
        public virtual void AssignShippingBillingAddressUserId(UserAddressModel userAddressModel, int userId)
        {
            userAddressModel.BillingAddress.UserId = userId;
            userAddressModel.ShippingAddress.UserId = userId;
        }

        //Set Shipping Billing Address of User.
        public virtual AddressListModel SetShippingBillingAddress(UserAddressModel userAddressModel)
        {
            try
            {
                AddressListModel addressList = new AddressListModel { AddressList = new List<AddressModel>() };

                if (userAddressModel.UseSameAsBillingAddress)
                    addressList.AddressList.Add(userAddressModel.ShippingAddress);
                else
                {
                    //add shipping/billing address to address list.
                    addressList.AddressList.Add(userAddressModel.BillingAddress);
                    addressList.AddressList.Add(userAddressModel.ShippingAddress);
                }

                foreach (var address in addressList.AddressList)
                {
                    //Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Customer.
                    AddressHelper.SetAddressFlagsToFalse(address);
                }
                return addressList;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetShippingBillingAddress method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Create new user.
        public virtual ZnodeUser CreateNewRegisteredUser(ZnodeUser user, UserAddressModel userAddressModel)
        {
            ZnodeLogging.LogMessage("New User with id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, user?.UserId);

            IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();
            user = _userRepository.Insert(user);
            ZnodeLogging.LogMessage(Admin_Resources.SuccessNewRegisteredUserCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            AssignShippingBillingAddressUserId(userAddressModel, user.UserId);
            return user;
        }

        //Insert/Update shippin/billing address of user.
        public virtual List<ZnodeAddress> InsertUpdateUserAddress(UserAddressModel userAddressModel)
        {
            //Create a list of addresses of user.
            AddressListModel addressList = SetShippingBillingAddress(userAddressModel);

            ZnodeLogging.LogMessage("AddressList count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, addressList?.AddressList?.Count);

            if (addressList?.AddressList?.Count > 0)
            {
                //Create the object of userAddress and store the shipping/billing address of user
                List<ZnodeAddress> userAddress = new List<ZnodeAddress>();

                if (IsNotNull(userAddressModel))
                {
                    // update Shipping address only.
                    if (userAddressModel.ShippingAddress?.AddressId > 0)
                        userAddress = UpdateUserAddress(userAddressModel.ShippingAddress, addressList, userAddress);

                    //Update shipping address only.
                    if (userAddressModel.BillingAddress?.AddressId > 0 && !Convert.ToBoolean(userAddressModel?.UseSameAsBillingAddress))
                        userAddress = UpdateUserAddress(userAddressModel.BillingAddress, addressList, userAddress);

                    //insert shipping or billing address.
                    if (userAddressModel.ShippingAddress?.AddressId == 0 || userAddressModel?.BillingAddress?.AddressId == 0)
                        userAddress = InsertUserAddress(userAddressModel, addressList);
                }
                return userAddress;
            }
            else

                return new List<ZnodeAddress>();
        }

        //Insert new address for user.
        public virtual List<ZnodeAddress> InsertUserAddress(UserAddressModel userAddressModel, AddressListModel addressList)
        {
            List<ZnodeAddress> userAddress = _addressRepository.Insert(addressList.AddressList.Where(w => w.AddressId == 0).ToEntity<ZnodeAddress>().ToList())?.ToList();
            //Set mapping of User and its address.
            InsertUserAddressMapping(userAddress, userAddressModel);
            ZnodeLogging.LogMessage(Admin_Resources.SuccessNewAddressCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddress;
        }

        //Update address for user.
        public virtual List<ZnodeAddress> UpdateUserAddress(AddressModel addressModel, AddressListModel addressList, List<ZnodeAddress> userAddress)
        {
            // update selected address to address table.
            bool status = false;
            if (addressList.AddressList.Any(w => w.AddressId == addressModel.AddressId))
                status = _addressRepository.Update(addressModel.ToEntity<ZnodeAddress>());
            userAddress.Add(addressModel.ToEntity<ZnodeAddress>());
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessUpdateAccountAddress : Admin_Resources.ErrorUpdateAccountAddress, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return userAddress;
        }

        //Set mapping of User and its address.
        public virtual void InsertUserAddressMapping(List<ZnodeAddress> userAddress, UserAddressModel user)
        {
            List<ZnodeUserAddress> userAddressList = new List<ZnodeUserAddress>();

            //Get newly created shipping/billing address of user.
            if (IsNotNull(userAddress))
            {
                foreach (ZnodeAddress item in userAddress)
                    userAddressList.Add(new ZnodeUserAddress { UserId = user.UserId, AddressId = item.AddressId });
            }

            IZnodeRepository<ZnodeUserAddress> _userAddressRepository = new ZnodeRepository<ZnodeUserAddress>();
            userAddressList = _userAddressRepository.Insert(userAddressList)?.ToList();
            ZnodeLogging.LogMessage(Admin_Resources.SuccessNewAddressCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        // To set user details from model with the login or anonymous users
        public virtual UserAddressModel SetUserDetails(ShoppingCartModel model)
        {
            if (IsNull(model.UserDetails))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.UserModelNotNull);

            try
            {
                UserAddressModel userAddress = model.UserDetails.ToModel<UserAddressModel, UserModel>();

                //Set billing address in user details.
                if (model.Payment?.BillingAddress?.AddressId > 0)
                    userAddress.BillingAddress = model.Payment?.BillingAddress;
                else if (model.BillingAddress?.AddressId > 0)
                    userAddress.BillingAddress = model.BillingAddress;
                else
                    userAddress.BillingAddress = model.Payment?.BillingAddress ?? model.BillingAddress;

                userAddress.ShippingAddress = model?.ShippingAddress;

                if (IsNotNull(model?.Payment?.ShippingAddress))
                    model.Payment.ShippingAddress.StateCode = model.ShippingAddress.StateCode;


                ZnodeLogging.LogMessage("BillingAddressId and ShippingAddressId of UserAddressModel: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddressId = userAddress?.BillingAddress?.AddressId, ShippingAddressId = userAddress?.ShippingAddress?.AddressId });
                return userAddress;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetUserDetails method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return new UserAddressModel();
            }
        }

        //to set ShoppingCart to ZNodeCheckout
        public virtual IZnodeCheckout SetCheckoutData(UserAddressModel userAddress, ShoppingCartModel model, ZnodeLogging log)
        {
            try
            {
                ZnodeLogging.LogMessage("UserAddressModel and ShoppingCartModel with Ids: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserAddressModelWithId = userAddress?.UserId, ShoppingCartModelWithOmsOrderStatusId = model?.OmsOrderStatusId });
                // Create the checkout object

                IZnodeCheckout checkout = GetService<IShoppingCartService>().GetCartAndMapToCheckout(userAddress, model);
                if (IsNull(checkout?.ShoppingCart))
                {
                    log.LogActivityTimerEnd((int)ZnodeLogging.ErrorNum.OrderSubmissionFailed, null);
                    throw new ZnodeException(ErrorCodes.ProcessingFailed, Admin_Resources.UnableToProcessOrder);
                }

                return SetShoppingCartDataToCheckout(checkout, model);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetCheckoutData method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to Validate Checkout object
        public virtual void ValidateCheckout(IZnodeCheckout checkout)
        {
            try
            {
                if (IsNotNull(checkout))
                {
                    if (IsNull(checkout?.ShoppingCart))
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ShoppingCartNotNull);

                    if (checkout?.ShoppingCart?.ShoppingCartItems?.Count < 1)
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorShoppingCartEmpty);

                    //Validates the payment option for order by user id, portal id, and profile.
                    if (!ValidatePaymentMethod(checkout))
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAssociatePaymentMethod);

                    if (IsNull(checkout.ShoppingCart.Payment))
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorShoppingCartPaymentNull);
                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in ValidateCheckout method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Validates the payment method selected for order by user id, portal id and profile.
        /// </summary>
        /// <param name="checkout"></param>
        /// <returns></returns>
        public virtual bool ValidatePaymentMethod(IZnodeCheckout checkout)
        {
            try
            {
                if (IsNotNull(checkout?.UserAccount))
                {
                    //Set filters for payment list.
                    FilterCollection filters = new FilterCollection();
                    filters.Add(Constants.FilterKeys.IsActive.ToString(), FilterOperators.In, "1");
                    if (IsNotNull(checkout))
                    {
                        filters.Add(ZnodeUserEnum.UserId.ToString(), FilterOperators.In, checkout.UserAccount.UserId.ToString());
                        filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.In, checkout.PortalID.ToString());
                        filters.Add(ZnodeProfileEnum.ProfileId.ToString(), FilterOperators.In, checkout.UserAccount.ProfileId.ToString());
                        filters.Add(new FilterTuple(Constants.FilterKeys.IsAssociated.ToString(), FilterOperators.Equals, "true"));
                    }

                    //Get payment list
                    PaymentSettingListModel paymentList = _paymentSettingService.GetPaymentSettingList(null, filters, null, null);

                    //Checks whether the order is placed from webstore and payment is made through vouchers/giftcards.
                    if (checkout.ShoppingCart.Vouchers.Count > 0 || checkout.ShoppingCart.Coupons.Count > 0)
                    {
                        if (checkout.ShoppingCart.Total == 0)
                            return true;
                    }
                    //If the order is placed from webstore and the order is of zero order value.
                    if (checkout.ShoppingCart.SkipPreprocessing && IsNull(checkout.ShoppingCart.Payment.PaymentSettingId))
                    {
                        return true;
                    }

                    return paymentList?.PaymentSettings?.Where(x => x.PaymentSettingId == checkout?.ShoppingCart?.Payment?.PaymentSettingId).ToList().Count !=0;
                }
                return false;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in ValidatePaymentMethod method for OrderId " + checkout?.ShoppingCart?.OrderId + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // to do PostSubmitOrder
        public virtual void PostSubmitOrder(ZnodeOrderFulfillment order, IZnodeCheckout checkout, string feedbackUrl)
          // Remove all saved cart items.
          => GetService<IShoppingCartService>().RemoveSavedCartItems(order.UserID, checkout.ShoppingCart.CookieMappingId,order.PortalId);

        //to generate order receipt
        public virtual string GetOrderReceipt(ZnodeOrderFulfillment order, IZnodeCheckout checkout, string feedbackUrl, int localeId, bool isUpdate, out bool isEnableBcc, int accountId = 0)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                foreach (OrderLineItemModel item in order.OrderLineItems)
                {
                    if (item.PersonaliseValueList != null)
                        item.PersonaliseValueList.Remove("AllocatedLineItems");

                    if (item.PersonaliseValuesDetail != null)
                        item.PersonaliseValuesDetail.RemoveAll(pv => pv.PersonalizeCode == "AllocatedLineItems");
                }

                IZnodeOrderReceipt receipt = GetOrderReceiptInstance(order, checkout.ShoppingCart);

                //Method to get Email Template.
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.OrderReceipt, (order.PortalId > 0) ? order.PortalId : PortalId, localeId);
                if (HelperUtility.IsNotNull(emailTemplateMapperModel))
                {
                    string receiptContent = ShowOrderAdditionalDetails(emailTemplateMapperModel.Descriptions, accountId);
                    isEnableBcc = emailTemplateMapperModel.IsEnableBcc;
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return EmailTemplateHelper.ReplaceTemplateTokens(receipt.GetOrderReceiptHtml(receiptContent));
                }
                isEnableBcc = false;

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return string.Empty;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetOrderReceipt method for OrderId " + order?.OrderID + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get order receipt for submit order
        public virtual string GetOrderReceipt(ZnodeOrderFulfillment znodeOrderFulfillment, IZnodeCheckout znodeCheckout, string feedbackUrl, int localeId, bool isUpdate, out bool isEnableBcc, UserModel userDetails = null, List<EmailTemplateMapperModel> emailTemplateMapperModelList = null)
        {
            isEnableBcc = false;
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                foreach (OrderLineItemModel item in znodeOrderFulfillment.OrderLineItems)
                {
                    if (item.PersonaliseValueList != null)
                        item.PersonaliseValueList.Remove("AllocatedLineItems");

                    if (item.PersonaliseValuesDetail != null)
                        item.PersonaliseValuesDetail.RemoveAll(pv => pv.PersonalizeCode == "AllocatedLineItems");
                }

                IZnodeOrderReceipt receipt = GetOrderReceiptInstance(znodeOrderFulfillment, znodeCheckout.ShoppingCart);

                //Method to get Email Template.
                EmailTemplateMapperModel emailTemplateMapperModel = GetOrderEmailTemplateModel(emailTemplateMapperModelList, ZnodeConstant.OrderReceipt, znodeOrderFulfillment.PortalId, localeId);

                if (HelperUtility.IsNotNull(emailTemplateMapperModel))
                {
                    string receiptContent = ShowOrderAdditionalDetailsForReceipt(emailTemplateMapperModel.Descriptions, userDetails);
                    isEnableBcc = emailTemplateMapperModel.IsEnableBcc;
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return EmailTemplateHelper.ReplaceTemplateTokens(receipt.GetOrderReceiptHtml(receiptContent));
                }

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return string.Empty;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetOrderReceipt method for OrderId " + znodeOrderFulfillment?.OrderID + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return string.Empty;
            }
        }


        //to generate order receipt
        public virtual string GetDownloadableProductOrderReceipt(ZnodeOrderFulfillment order, IZnodeCheckout checkout, string feedbackUrl, int localeId, DownloadableProductKeyListModel key, bool isUpdate, out bool isEnableBcc)
        {
            IZnodeOrderReceipt receipt = GetOrderReceiptInstance(order, checkout.ShoppingCart);

            //Method to get Email Template.
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.ProductKeyOrderReceipt, (order.PortalId > 0) ? order.PortalId : PortalId, localeId);
            if (IsNotNull(emailTemplateMapperModel))
            {
                isEnableBcc = emailTemplateMapperModel.IsEnableBcc;
                return EmailTemplateHelper.ReplaceTemplateTokens(receipt.GetProductKeysOrderReceiptHtml(emailTemplateMapperModel.Descriptions, key));
            }
            isEnableBcc = false;
            return string.Empty;
        }

        //Get purchased product order Receipt html For Email.
        public virtual string GetHtmlVendorForEmail(ZnodeOrderFulfillment order, IZnodeCheckout checkout, string feedbackUrl, string vendorCode, int localeId, List<EmailTemplateMapperModel> emailTemplateMapperModelList = null)
        {
            try
            {
                IZnodeOrderReceipt receipt = GetOrderReceiptInstance(order, checkout.ShoppingCart);
                //Method to get Email Template.
                EmailTemplateMapperModel emailTemplateMapperModel = GetOrderEmailTemplateModel(emailTemplateMapperModelList, ZnodeConstant.VendorReceipt, order.PortalId, localeId);

                if (IsNotNull(emailTemplateMapperModel))
                    return EmailTemplateHelper.ReplaceTemplateTokens(receipt.GetVendorProductOrderReceiptHtml(emailTemplateMapperModel.Descriptions, vendorCode));

                return string.Empty;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetHtmlVendorForEmail method for OrderId " + order?.OrderID + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to generate shipping status receipt
        //Get Html Resend Receipt For Email.
        public virtual string GetShippingReceiptForEmail(OrderModel orderModel, out bool isEnableBcc)
        {
            try
            {
                UpdateDateTimeByGlobalSetting(orderModel);
                IZnodeOrderReceipt receipt = GetOrderReceiptInstance(orderModel);
                //Method to get Email Template.
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.ShippingReceipt, (orderModel.PortalId > 0) ? orderModel.PortalId : PortalId);
                if (IsNotNull(emailTemplateMapperModel))
                {
                    isEnableBcc = emailTemplateMapperModel.IsEnableBcc;
                    return EmailTemplateHelper.ReplaceTemplateTokens(receipt.GetOrderResendReceiptHtml(emailTemplateMapperModel.Descriptions));
                }
                isEnableBcc = false;
                return string.Empty;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetShippingReceiptForEmail method for OrderNumber " + orderModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }
        public virtual string GetCancelledOrderReceiptForEmail(OrderModel orderModel, out bool isEnableBcc)
        {
            UpdateDateTimeByGlobalSetting(orderModel);
            IZnodeOrderReceipt receipt = GetOrderReceiptInstance(orderModel);
            //Method to get Email Template.
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.CancelledOrderReceipt, (orderModel.PortalId > 0) ? orderModel.PortalId : PortalId);
            if (IsNotNull(emailTemplateMapperModel))
            {
                isEnableBcc = emailTemplateMapperModel.IsEnableBcc;
                return EmailTemplateHelper.ReplaceTemplateTokens(receipt.GetOrderResendReceiptHtml(emailTemplateMapperModel.Descriptions));
            }
            isEnableBcc = false;
            return string.Empty;
        }
        //to send order receipt to customer
        public virtual bool SendOrderReceipt(string userEmailId, string subject, string senderEmail, string bccEmailId, string receiptHtml)
        {
            try
            {
                bool isSuccess = false;

                //This method is used to send an email.
                ZnodeEmail.SendEmail(userEmailId, senderEmail, bccEmailId, subject, receiptHtml, true);
                isSuccess = true;
                return isSuccess;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SendOrderReceipt method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to send order receipt to customer
        public virtual bool SendOrderReceipt(int portalId, string userEmailId, string subject, string senderEmail, string bccEmailId, string receiptHtml, bool isEnableBcc = false)
        {
            try
            {
                bool isSuccess = false;
                ZnodePortalSmtpSetting smtpConfigurations = ZnodeEmail.GetSMTPSetting(portalId);
                string userName = GetDecryptSMTPUserName(smtpConfigurations.UserName);
                smtpConfigurations.UserName = userName;
                HttpContext httpContext = HttpContext.Current;
                //This method is used to send an email.    
                Task.Run(() =>
                {
                    HttpContext.Current = httpContext;
                    ZnodeEmail.SendEmail(portalId, userEmailId, senderEmail, ZnodeEmail.GetBccEmail(isEnableBcc, portalId, bccEmailId), subject, receiptHtml, true, smtpConfigurations, userName);
                });
                isSuccess = true;
                return isSuccess;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SendOrderReceipt method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get user details by id,order and orderDetail.
        public virtual void GetUserDetails(int userId, OrderModel order, ZnodeOmsOrderDetail orderDetail)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId });
            UserModel userDetails = orderDetail.ZnodeUser.ToModel<UserModel>(); ;
            IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();

            ZnodeLogging.LogMessage("UserId to get user details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = order?.UserId });
            if (IsNull(userDetails))
                userDetails = GetUserNameByUserId(order.UserId);
            if (IsNotNull(order) && IsNotNull(userDetails))
            {
                if (IsNull(userDetails.UserName))
                {
                    userDetails.UserName = userDetails.Email;
                }
                order.FirstName = userDetails.FirstName;
                order.LastName = userDetails.LastName;
                order.UserName = userDetails.UserName;
                order.CreatedByName = order.CreatedBy == order.UserId ? order.UserName : GetUserNameByUserId(order.CreatedBy)?.UserName;
            }
            else
            {
                order.UserName = GetUserNameByUserId(order.CreatedBy)?.UserName ?? _userRepository.Table.FirstOrDefault(x => x.UserId == order.UserId)?.Email;
                order.CreatedByName = order.UserName;
            }
        }

        //to send notification to customer service if any order contains low inventory products
        private bool SendEmailNotificationForLowInventory(IZnodeCheckout checkout, ZnodeOrderFulfillment order, OrderModel orderModel, int localeId, List<EmailTemplateMapperModel> emailTemplateMapperModelList = null)
        {
            IZnodeOrderReceipt receipt = GetOrderReceiptInstance(order, checkout.ShoppingCart);

            string receiptHtml = string.Empty;

            //Method to get Email Template.
            EmailTemplateMapperModel emailTemplateMapperModel = GetOrderEmailTemplateModel(emailTemplateMapperModelList, ZnodeConstant.LowInventoryOrderNotification, order.PortalId, localeId);
            if (HelperUtility.IsNotNull(emailTemplateMapperModel))
            {
                string receiptContent = emailTemplateMapperModel.Descriptions;
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                receiptHtml = EmailTemplateHelper.ReplaceTemplateTokens(receipt.CreateLowInventoryNotification(receiptContent));
            }

            if (!string.IsNullOrEmpty(receiptHtml) && !string.IsNullOrEmpty(ZnodeConfigManager.SiteConfig.AdminEmail) && !string.IsNullOrEmpty(ZnodeConfigManager.SiteConfig.CustomerServiceEmail))
                orderModel.IsEmailSend = SendOrderReceipt(orderModel.PortalId, ZnodeConfigManager.SiteConfig.CustomerServiceEmail, $"{Admin_Resources.TitleLowInventoryOrderNotification} - {orderModel.OrderNumber}", ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeConfigManager.SiteConfig.AdminEmail, receiptHtml);
            return true;
        }


        //Get Refund types from database and map into dictionary
        public virtual Dictionary<string, int> GetRefundPaymentType()
        {
            List<ZnodeOmsRefundType> refundPaymentTypes = new ZnodeRepository<ZnodeOmsRefundType>().Table.ToList();
            ZnodeLogging.LogMessage("refundPaymentTypes count.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { refundPaymentTypesCount = refundPaymentTypes?.Count });

            Dictionary<string, int> refundTypeDictionary = new Dictionary<string, int>();

            //Map Refundtype into dictionary
            foreach (ZnodeOmsRefundType refundtype in refundPaymentTypes)
                refundTypeDictionary.Add(refundtype.RefundType.ToUpper(), refundtype.OmsRefundTypeId);

            return refundTypeDictionary;
        }

        //Map Refund Total order Details
        public virtual RefundPaymentModel MapRefundTotalDetails(int OmsRefundTypeId, OrderItemsRefundModel orderItemsRefundModel, ZnodeOmsOrderDetail orderDetails)
        {
            return new RefundPaymentModel
            {
                OmsOrderDetailsId = orderDetails.OmsOrderDetailsId,
                OmsRefundTypeId = OmsRefundTypeId,
                RefundableAmountLeft = (orderItemsRefundModel.RefundOrderLineitems.Sum(x => x.RefundableAmountLeft)
                                       + (orderItemsRefundModel?.ShippingRefundDetails?.RefundableAmountLeft ?? 0.00m))
                                       - (_omsPaymentRefundRepository.Table.Where(x => x.OmsOrderDetailsId == orderDetails.OmsOrderDetailsId
                                       && x.OmsRefundTypeId == OmsRefundTypeId)?.ToList()?.Sum(x => x == null ? 0.00m : x.RefundAmount) ?? 0.00m),
                TotalAmount = Convert.ToDecimal(orderDetails.Total),
                RefundType = ZnodeConstant.TotalRefund,
            };
        }

        //Map Shipping Refund  Details
        public virtual RefundPaymentModel MapRefundShippingDetails(int OmsRefundTypeId, ZnodeOmsOrderDetail orderDetails)
         => new RefundPaymentModel
         {
             OmsOrderDetailsId = orderDetails.OmsOrderDetailsId,
             OmsRefundTypeId = OmsRefundTypeId,
             RefundableAmountLeft = (orderDetails.ShippingCost ?? 0.00m) - (_omsPaymentRefundRepository.Table.Where(x => x.OmsOrderDetailsId == orderDetails.OmsOrderDetailsId && x.OmsRefundTypeId == OmsRefundTypeId)?.ToList()?.Sum(x => x == null ? 0.00m : x.RefundAmount) ?? 0.00m),
             TotalAmount = Convert.ToDecimal(orderDetails.ShippingCost),
             RefundType = ZnodeConstant.ShippingRefund,
         };

        // Map ZnodeOmsOrderDetail to OrderItemsRefundModel
        public virtual OrderItemsRefundModel MapToOrderItemsRefundModel(ZnodeOmsOrderDetail orderDetail)
        => new OrderItemsRefundModel
        {
            TransactionId = orderDetail.PaymentTransactionToken,
            OmsOrderDetailsId = orderDetail.OmsOrderDetailsId,
            OmsOrderId = orderDetail.OmsOrderId,
        };

        //Get Order Line Items with Refund Details
        private List<RefundPaymentModel> GetRefundOrderLineItems(int orderDetailsId, int OmsRefundTypeId)
        //Join OrderLineItemRepository and OmsPaymentRefundRepository and get RefundableAmountLeft
        => (from orderlineItems in _orderLineItemRepository.Table
            join OmsPaymentRefund in _omsPaymentRefundRepository.Table on orderlineItems.OmsOrderLineItemsId equals OmsPaymentRefund.OmsOrderLineItemsId into j1
            from j2 in j1.DefaultIfEmpty()
            where orderlineItems.OmsOrderDetailsId == orderDetailsId
            group j2 by new
            {
                OmsOrderDetailsId = orderlineItems.OmsOrderDetailsId,
                OmsOrderLineItemsId = orderlineItems.OmsOrderLineItemsId,
                ProductName = orderlineItems.ProductName,
                Unitprice = orderlineItems.Price,
                DiscountAmount = orderlineItems.DiscountAmount ?? 0.00m,
                Quantity = orderlineItems.Quantity ?? 0.00m,
            }
                         into grouped
            select new RefundPaymentModel
            {
                OmsRefundTypeId = OmsRefundTypeId,
                RefundType = ZnodeConstant.PartialRefund,
                ProductName = grouped.Key.ProductName,
                OmsOrderDetailsId = grouped.Key.OmsOrderDetailsId,
                OmsOrderLineItemsId = grouped.Key.OmsOrderLineItemsId,
                RefundableAmountLeft = ((grouped.Key.Unitprice * grouped.Key.Quantity) - (grouped.Key.DiscountAmount + grouped.Sum(d => d == null ? 0 : d.RefundAmount)))
            })?.ToList();

        #region Update order status

        //Send the order status email to customer.
        public virtual string SendShippingStatusEmailReceipt(OrderModel model, string receiptHtml)
        {
            if (string.IsNullOrEmpty(receiptHtml))
                return receiptHtml;

            //order to bind shipping details in data table.
            DataTable shippingStatusTable = SetShippingStatusData(model);
            ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(receiptHtml);

            // Parse order table
            receiptHelper.Parse(shippingStatusTable.CreateDataReader());

            // Return the HTML output
            return receiptHelper.Output;
        }

        //to set order details
        public virtual DataTable SetShippingStatusData(OrderModel Order)
        {
            // Create new row
            DataTable orderTable = CreateShippingTable();
            DataRow orderRow = orderTable.NewRow();

            // Additional info
            orderRow["BillingFirstName"] = Order.BillingAddress.FirstName;
            orderRow["BillingLastName"] = Order.BillingAddress.LastName;

            if (!string.IsNullOrEmpty(Order.TrackingNumber))
            {
                orderRow["TrackingMessage"] = Equals(Order.OrderState, ZnodeOrderStatusEnum.SHIPPED.ToString()) ? Admin_Resources.ShippingTrackingNoMessage + SetTrackingUrl(Order.TrackingNumber, Order.TrackingUrl) : string.Empty;
                orderRow["Message"] = string.Format(Admin_Resources.ShippingStatusMessage, Order.OrderState.ToLower()) + (Equals(Order.OrderState, ZnodeOrderStatusEnum.SHIPPED.ToString()) ? Admin_Resources.TrackingPackageMessage : string.Empty);
            }
            else
            {
                orderRow["TrackingMessage"] = string.Empty;
                orderRow["Message"] = string.Format(Admin_Resources.ShippingStatusMessage, Order.OrderState.ToLower());
            }

            // Add rows to order table
            orderTable.Rows.Add(orderRow);
            return orderTable;
        }

        public virtual DataTable CreateShippingTable()
        {
            DataTable shippingTable = new DataTable();
            // Additional info
            shippingTable.Columns.Add("BillingFirstName");
            shippingTable.Columns.Add("BillingLastName");
            shippingTable.Columns.Add("TrackingMessage");
            shippingTable.Columns.Add("Message");

            return shippingTable;
        }

        #endregion Update order status private

        //Get customer name by userId.
        public virtual void GetCustomerName(int? userId, OrdersListModel orderListModel)
        {
            if (IsNotNull(userId))
            {
                ZnodeLogging.LogMessage("UserId to get customer name", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId });
                IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();
                orderListModel.CustomerName = _userRepository.Table.Where(x => x.UserId == userId).Select(x => x.FirstName + " " + x.LastName)?.FirstOrDefault();
            }
        }

        //Get Order Note Details.
        public virtual List<OrderNotesModel> GetOrderNoteDetails(int omsOrderId, int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Input parameters : ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsOrderId = omsOrderId, omsQuoteId = omsQuoteId });

            IZnodeRepository<View_GetOmsOrderNotes> _viewOmsOrderNoteList = new ZnodeRepository<View_GetOmsOrderNotes>();
            FilterCollection filters = new FilterCollection();
            if (omsOrderId > 0)
                filters.Add(new FilterTuple(ZnodeOmsOrderDetailEnum.OmsOrderId.ToString(), FilterOperators.Equals, omsOrderId.ToString()));

            if (omsQuoteId > 0)
                filters.Add(new FilterTuple(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString()));

            if (filters.Count > 0)
            {
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("Where clause in GetOrderNoteDetails method get data", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause);
                return _viewOmsOrderNoteList.GetEntityList(whereClauseModel.WhereClause)?.ToList().ToModel<OrderNotesModel>()?.ToList();
            }
            return new List<OrderNotesModel>();
        }

        //Create Single Order Line Item if order is having group product.
        public virtual List<OrderLineItemModel> CreateSingleOrderLineItem(OrderModel orderModel, bool isResendEmail = false)
        {
            if (orderModel?.OrderLineItems.Count > 0 && isResendEmail)
                orderModel.OrderLineItems = orderModel.OrderLineItems.Where(q => q.OrderLineItemStateId != 30).ToList();

            return orderHelper.FormalizeOrderLineItems(orderModel);
        }

        //For Getting personalize attribute.       
        public virtual string GetPersonalizeAttributes(Dictionary<string, object> personalizeValueList)
        {
            personalizeValueList.Remove("AllocatedLineItems");
            string personaliseAttributeHtml = string.Empty;
            if (IsNotNull(personalizeValueList))
                personalizeValueList.Remove("AllocatedLineItems");
            string personalizeAttributeHtml = string.Empty;
            if (IsNotNull(personalizeValueList))
            {
                foreach (var personalizeAttribute in personalizeValueList)
                    personalizeAttributeHtml += $"{"<p>"} { personalizeAttribute.Key}{" : "}{personalizeAttribute.Value}{"</p>"}";

                return personalizeAttributeHtml;
            }
            return string.Empty;
        }

        // To set order discount amount
        public virtual void SetOrderDiscount(OrderModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                List<OrderDiscountModel> discountList = orderHelper.GetOrderDiscountAmount(model.OmsOrderDetailsId);
                if (discountList?.Count > 0)
                {
                    OrderDiscountModel CSRDiscountModel = discountList.FirstOrDefault(x => x.DiscountCode == OrderDiscountTypeEnum.CSRDISCOUNT.ToString() && x.OmsOrderLineItemId == null);
                    //decimal csrDiscount = (discountList.FirstOrDefault(x => x.DiscountType == OrderDiscountTypeEnum.CSRDISCOUNT.ToString())?.DiscountAmount).GetValueOrDefault();
                    if (HelperUtility.IsNotNull(CSRDiscountModel) && HelperUtility.IsNotNull(model.ShoppingCartModel))
                    {
                        model.ShoppingCartModel.CSRDiscountAmount = CSRDiscountModel.DiscountAmount.GetValueOrDefault();
                        model.ShoppingCartModel.Discount = (model?.ShoppingCartModel.Discount - model?.ShoppingCartModel.CSRDiscountAmount).GetValueOrDefault();
                        model.CSRDiscountAmount = model.ShoppingCartModel.CSRDiscountAmount;
                        model.DiscountAmount = model.DiscountAmount - model.ShoppingCartModel.CSRDiscountAmount;
                    }
                    else if(HelperUtility.IsNotNull(CSRDiscountModel))
                    {
                        model.CSRDiscountAmount = (CSRDiscountModel?.DiscountAmount).GetValueOrDefault();
                        model.DiscountAmount = (model.DiscountAmount - CSRDiscountModel?.DiscountAmount).GetValueOrDefault();

                    }
                    decimal giftCardDiscount = (discountList.FirstOrDefault(x => x.DiscountType == OrderDiscountTypeEnum.GIFTCARD.ToString())?.DiscountAmount).GetValueOrDefault();
                    if (giftCardDiscount > 0)
                    {
                        model.GiftCardAmount = giftCardDiscount;
                        model.GiftCardNumber = (discountList.FirstOrDefault(x => x.DiscountType == OrderDiscountTypeEnum.GIFTCARD.ToString())?.DiscountCode);
                    }


                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetOrderDiscount method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Save in Quote.
        public virtual void SaveInQuote(ShoppingCartModel model, ZnodeOrderFulfillment order)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                //If the order is from quote.
                ZnodeLogging.LogMessage("Order ID while saving order as a quote:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, order?.OrderID);

                if (order.OrderID > 0 && model.OmsQuoteId > 0)
                {
                    ZnodeOmsOrder createdOrder = new ZnodeRepository<ZnodeOmsOrder>().Table.Where(x => x.OmsOrderId == order.OrderID).FirstOrDefault();
                    ZnodeLogging.LogMessage("Created order in SaveInQuote method with OmsOrderId", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, createdOrder?.OmsOrderId);

                    if (IsNotNull(createdOrder))
                    {
                        //Set IsQuoteOrder to true.
                        createdOrder.IsQuoteOrder = true;
                        createdOrder.OMSQuoteId = model.OmsQuoteId;
                        ZnodeLogging.LogMessage(_omsOrderRepository.Update(createdOrder) ? Admin_Resources.SuccessOrderUpdate : Admin_Resources.ErrorOrderUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                        //Convert quote additional notes to order notes.
                        ToOrderNotes(model, order);
                        order.Order.IsQuoteOrder = true;
                        ZnodeOmsQuote quote = new ZnodeRepository<ZnodeOmsQuote>().Table.Where(x => x.OmsQuoteId == model.OmsQuoteId).FirstOrDefault();
                        if (IsNotNull(quote))
                        {
                            //Set IsQuoteOrder to true.
                            quote.IsConvertedToOrder = true;
                            ZnodeLogging.LogMessage(_znodeOmsQuote.Update(quote) ? Admin_Resources.SuccessQuotePendingOrderUpdate : Admin_Resources.ErrorQuotePendingOrderUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                        }
                    }
                }
                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SaveInQuote method for OrderId " + model.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to set shipping state code
        public virtual void SetStateCode(ShoppingCartModel model)
        {
            IShoppingCartService _shoppingCartService = GetService<IShoppingCartService>();
            _shoppingCartService.SetShippingStateCode(model);
        }

        //Convert quote additional notes to order notes.
        public virtual void ToOrderNotes(ShoppingCartModel model, ZnodeOrderFulfillment order)
        {
            try
            {
                //Get oms additional notes for given quote id.
                List<ZnodeOmsNote> quoteNotes = _omsNoteRepository.Table.Where(x => x.OmsQuoteId == model.OmsQuoteId)?.ToList();
                ZnodeLogging.LogMessage("Quote notes list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, quoteNotes?.Count());
                if (quoteNotes?.Count() > 0)
                {
                    //Convert quote additional notes to order additional notes.
                    foreach (ZnodeOmsNote note in quoteNotes)
                    {
                        note.OmsOrderDetailsId = order?.Order?.OmsOrderDetailsId;
                        _omsNoteRepository.Update(note);

                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in ToOrderNotes method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Send purchased product details Email To Vendor.
        public virtual void SendEmailToVendor(ZnodeOrderFulfillment order, IZnodeCheckout checkout, string feedbackUrl, int localeId, bool isEnableBcc, List<EmailTemplateMapperModel> emailTemplateMapperModelList = null)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                //Get order line item which having vendors.
                List<OrderLineItemModel> vendorOrderLineItems = order.OrderLineItems?.Where(w => w.Vendor != string.Empty && w.Vendor != null).ToList();
                ZnodeLogging.LogMessage("Vendor orderLineItems list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, vendorOrderLineItems?.Count());
                if (vendorOrderLineItems?.Count > 0)
                {
                    IZnodeRepository<ZnodePimVendor> _pimVendorRepository = new ZnodeRepository<ZnodePimVendor>();

                    //Get distinct vendor codes from order line item.
                    List<string> distinctVendorCodes = vendorOrderLineItems.Select(x => x.Vendor).Distinct().ToList();
                    ZnodeLogging.LogMessage("distinctVendorCodes count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, distinctVendorCodes?.Count());

                    //Get Email id and Vendor code together by using vendor code.
                    var vendorDetails = _pimVendorRepository.Table.Where(x => distinctVendorCodes.Contains(x.VendorCode) && x.IsActive == true)?.Select(s => new { Email = s.Email, Code = s.VendorCode }).ToList();

                    if (IsNotNull(vendorDetails))
                    {
                        //Send purchased product details to respective vendor.
                        foreach (var vendorInfo in vendorDetails)
                        {
                            //Create purchased product receipt HTML to the order.
                            string VendorReceiptHtml = GetHtmlVendorForEmail(order, checkout, feedbackUrl, vendorInfo.Code, localeId, emailTemplateMapperModelList);

                            //Send order receipt to respective vendor.
                            if (!string.IsNullOrEmpty(VendorReceiptHtml))
                                SendOrderReceipt(order.PortalId, vendorInfo.Email, Admin_Resources.TitleOrderReceipt, ZnodeConfigManager.SiteConfig.AdminEmail, string.Empty, VendorReceiptHtml, isEnableBcc);
                        }
                    }
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SendEmailToVendor method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //to send line item state change email to user
        public virtual void SendLineItemStateChangeEmail(ZnodeOrderFulfillment order, bool isEnableBcc)
        {
            if (IsNotNull(order?.Order) && !string.IsNullOrEmpty(order?.Order.ModifiedLineItemSkus))
            {
                SendLineItemConfirmationEmail(order, new NameValueCollection(), isEnableBcc);
            }
        }

        //Replace sort key name
        public virtual void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, Constants.FilterKeys.OrderTotalWithCurrency, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Constants.FilterKeys.OrderTotalWithCurrency.ToLower(), Constants.FilterKeys.Total); }
                if (string.Equals(key, Constants.FilterKeys.OrderDateWithTime, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Constants.FilterKeys.OrderDateWithTime.ToLower(), Constants.FilterKeys.OrderDate); }
            }
        }

        protected virtual void ReplaceFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (tuple.Item1 == Constants.FilterKeys.OrderDateWithTime) { ReplaceFilterKeyName(ref filters, Constants.FilterKeys.OrderDateWithTime, Constants.FilterKeys.OrderDate); }
            }
        }

        //Get user id from filters.
        public virtual void GetUserIdFromFilters(FilterCollection filters, ref int userId)
        {
            userId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            filters?.RemoveAll(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase));
        }

        //to check order is allowed to edit
        public virtual bool IsOrderEditable(OrderModel model)
        => _omsOrderStateRepository.GetById(model.OmsOrderStateId).IsEdit;

        //to update order state
        public virtual bool UpdateOrderState(OrderModel model)
        {
            OrderStateParameterModel orderStateModel = new OrderStateParameterModel();
            orderStateModel.OmsOrderId = model.OmsOrderId;
            orderStateModel.OmsOrderDetailsId = model.OmsOrderDetailsId;
            orderStateModel.TrackingNumber = model.TrackingNumber;
            orderStateModel.OmsOrderStateId = model.OmsOrderStateId;
            orderStateModel.CreatedBy = model.CreatedBy;
            orderStateModel.ModifiedBy = model.ModifiedBy;
            return UpdateOrderStatus(orderStateModel);
        }

        //to update order state
        public virtual bool UpdateReturnedOrderState(OrderModel model)
        {
            //Get tax rule against order
            TaxRuleModel taxRule = GetTaxRate(model.OmsOrderId);
            decimal totalTaxOnShipping = IsNotNull(taxRule) && taxRule.TaxShipping && model.ShippingCost > 0 ? (model.ShippingCost * (taxRule.TaxRate / 100)) : 0;
            decimal returnCharges = 0;
            if (HelperUtility.IsNotNull(model.ReturnItemList) && model.ReturnItemList.ReturnItemList.Count > 0)
                returnCharges = model.ReturnItemList.ReturnItemList.Where(x => x.IsShippingReturn == false).Sum(x => x.ShippingCost - (x.PerQuantityShippingDiscount * x.Quantity)) + totalTaxOnShipping;
            OrderStateParameterModel orderStateModel = new OrderStateParameterModel();
            orderStateModel.OmsOrderId = model.OmsOrderId;
            orderStateModel.OmsOrderDetailsId = model.OmsOrderDetailsId;
            orderStateModel.TrackingNumber = model.TrackingNumber;
            orderStateModel.OmsOrderStateId = Convert.ToInt32(_orderStateRepository.Table.FirstOrDefault(x => x.OrderStateName == ZnodeOrderStatusEnum.CANCELED.ToString())?.OmsOrderStateId); ;
            orderStateModel.CreatedBy = model.CreatedBy;
            orderStateModel.ModifiedBy = model.ModifiedBy;
            orderStateModel.ShippingHandlingCharges = model.ShippingHandlingCharges;
            orderStateModel.ReturnCharges = returnCharges;
            return UpdateReturnedOrderStatus(orderStateModel);
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

        //to update order status.
        public virtual bool UpdateOrderTrackingNumber(OrderModel model)
        {

            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.Where(w => w.OmsOrderId == model.OmsOrderId && w.IsActive).FirstOrDefault();
            ZnodeLogging.LogMessage("OmsOrderDetailsId for ZnodeOmsOrderDetail model :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, order?.OmsOrderDetailsId);
            if (IsNotNull(order))
            {
                order.TrackingNumber = model.TrackingNumber;
                order.ModifiedDate = GetDateTime();
                order.CreatedBy = model.CreatedBy;
                order.ModifiedBy = model.ModifiedBy;
                return _orderDetailsRepository.Update(order);
            }

            return false;
        }

        //Update the inHand date when user changes the date from mange Order.
        public virtual bool UpdateOrderInHandsDate(OrderModel model)
        {

            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.FirstOrDefault(w => w.OmsOrderId == model.OmsOrderId && w.IsActive);
            ZnodeLogging.LogMessage("OmsOrderDetailsId for ZnodeOmsOrderDetail model :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, order?.OmsOrderDetailsId);
            if (IsNotNull(order))
            {
                order.InHandDate = model.InHandDate;
                order.ModifiedDate = GetDateTime();
                order.CreatedBy = model.CreatedBy;
                order.ModifiedBy = model.ModifiedBy;
                return _orderDetailsRepository.Update(order);
            }

            return false;
        }

        /// <summary>
        /// Update the Job Name when user changes the Job Name from Manage Order and saves the order
        /// </summary>
        /// <param name="model">OrderModel</param>
        /// <returns>Update success</returns>
        public virtual bool UpdateOrderJobName(OrderModel model)
        {
            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.FirstOrDefault(w => w.OmsOrderId == model.OmsOrderId && w.IsActive);
            ZnodeLogging.LogMessage("UpdateOrderJobName for ZnodeOmsOrderDetail model :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, order?.OmsOrderDetailsId);

            if (IsNotNull(order))
            {
                order.JobName = model.JobName;
                order.ModifiedDate = GetDateTime();
                order.CreatedBy = model.CreatedBy;
                order.ModifiedBy = model.ModifiedBy;
                return _orderDetailsRepository.Update(order);
            }

            return false;
        }

        /// <summary>
        /// Update the Shipping Constraint when user changes the Shipping Constraint from Manage Order and saves the order
        /// </summary>
        /// <param name="model">OrderModel</param>
        /// <returns>Update success</returns>
        public virtual bool UpdateOrderShippingConstraint(OrderModel model)
        {
            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.FirstOrDefault(w => w.OmsOrderId == model.OmsOrderId && w.IsActive);
            ZnodeLogging.LogMessage("UpdateOrderShippingConstraint for ZnodeOmsOrderDetail model :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, order?.OmsOrderDetailsId);

            if (IsNotNull(order))
            {
                order.ShippingConstraintCode = model.ShippingConstraintCode;
                order.ModifiedDate = GetDateTime();
                order.CreatedBy = model.CreatedBy;
                order.ModifiedBy = model.ModifiedBy;
                return _orderDetailsRepository.Update(order);
            }

            return false;
        }

        //to update order status.
        private void UpdatePurchaseOrderNumber(OrderModel model)
        {
            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.Where(w => w.OmsOrderId == model.OmsOrderId && w.IsActive).FirstOrDefault();
            if (IsNotNull(order))
            {
                order.PurchaseOrderNumber = model.PurchaseOrderNumber;
                order.ModifiedDate = GetDateTime();
                order.CreatedBy = model.CreatedBy;
                order.ModifiedBy = model.ModifiedBy;
                _orderDetailsRepository.Update(order);
            }
        }

        // Update Order Shipping Billing Address
        public virtual bool UpdateOrderAddress(AddressModel model)
        {
            bool updated = false;

            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.Where(w => w.OmsOrderId == model.omsOrderId && w.IsActive).FirstOrDefault();
            ZnodeOmsOrderShipment shipment = _orderShipmentRepository.Table.Where(w => w.OmsOrderShipmentId == model.omsOrderShipmentId).FirstOrDefault();

            if (IsNotNull(order) && model.FromBillingShipping == "billing")
            {
                BillingMapping(order, model);
                updated = _orderDetailsRepository.Update(order);
            }
            if (IsNotNull(shipment) && model.FromBillingShipping == "shipping")
            {
                ShippingMapping(shipment, model);
                updated = _orderShipmentRepository.Update(shipment);
            }
            return updated;
        }

        //to check update history for provided key
        public virtual bool ExistUpdateHistory(string key, OrderModel model)
            => model?.OrderHistory?.Keys?.Contains(key) ?? false;

        //to check get history for provided key
        public virtual string GetHistoryMessageByKey(string key, OrderModel model)
        {
            string val = string.Empty;
            model.OrderHistory.TryGetValue(key, out val);

            switch (key)
            {
                case ZnodeConstant.OrderTax:
                    return string.Format(Admin_Resources.OrderTaxExempted, val);

                case ZnodeConstant.OrderBillingAddress:
                    return $" {val}";

                case ZnodeConstant.OrderShippingAddress:
                    return $" {val}";

                case ZnodeConstant.OrderShippingCost:
                    return string.Format(Admin_Resources.OrderHistoryShippingAmount, model.OrderOldValue.ShippingAmount, val);

                case ZnodeConstant.OrderShippingType:
                    return string.Format(Admin_Resources.OrderHistoryShippingType, _shippingRepository.Table.Where(w => w.ShippingId == model.OrderOldValue.ShippingId)?.Select(s => s.Description)?.FirstOrDefault(), val);

                case ZnodeConstant.OrderCSRDiscount:
                    return string.Format(Admin_Resources.CSRDiscountApplied, val);

                case ZnodeConstant.OrderGiftCard:
                    {
                        decimal giftcardAmount = model?.ShoppingCartModel?.GiftCardAmount ?? 0;
                        string formatedAmount = ZnodeCurrencyManager.FormatPriceWithCurrency(giftcardAmount, model.CultureCode, string.Empty);
                        return string.Format(Admin_Resources.GiftCardHistory, val, formatedAmount);
                    }

                case ZnodeConstant.OrderCoupon:
                    return string.Format(Admin_Resources.CouponCodeApplied, val);

                case ZnodeConstant.OrderUpdatedStatus:
                    return string.Format(Admin_Resources.OrderHistoryUpdatedStatus, model.OrderOldValue.OrderState, val);

                case ZnodeConstant.OrderRemoveVoucher:
                    return string.Format(Admin_Resources.VoucherRemoved, val);

                default:
                    return string.Format(Admin_Resources.OrderHistory, key, val);
            }
        }

        //to save order and payment state
        public virtual void SaveHistoryAndUpdateOrderState(OrderModel orderModel, bool updateExistingOrder = false)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string orderHistory = string.Empty;
            int notesId = 0;
            //to check payment status is updated then update in existing order
            if (ExistUpdateHistory(ZnodeConstant.OrderPaymentState, orderModel))
            {
                UpdateOrderPaymentStatus(orderModel.OmsOrderId, orderModel.PaymentStatus, orderModel.OmsPaymentStateId, orderModel.CreatedBy, orderModel.ModifiedBy);
                orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(ZnodeConstant.OrderPaymentState, orderModel), orderHistory);
            }

            //to check payment status is updated then update in existing order
            if (ExistUpdateHistory(ZnodeConstant.OrderBillingAddress, orderModel))
            {
                UpdateBillingAddress(orderModel.OmsOrderId, orderModel.BillingAddress);
                orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(ZnodeConstant.OrderBillingAddress, orderModel), orderHistory);
            }

            //to check order status is updated then update in existing order
            if (ExistUpdateHistory(ZnodeConstant.OrderTrackingNumber, orderModel))
            {
                UpdateOrderTrackingNumber(orderModel);
                orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(ZnodeConstant.OrderTrackingNumber, orderModel), orderHistory);
            }

            ////to check order in hand date is updated then update in existing order
            if (ExistUpdateHistory(ZnodeConstant.OrderInHandsDate, orderModel))
            {
                UpdateOrderInHandsDate(orderModel);
                orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(ZnodeConstant.OrderInHandsDate, orderModel), orderHistory);
            }

            //Check whether order Job Name is updated, then update in existing order
            if (ExistUpdateHistory(ZnodeConstant.OrderJobName, orderModel))
            {
                UpdateOrderJobName(orderModel);
                orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(ZnodeConstant.OrderJobName, orderModel), orderHistory);
            }

            //Check whether order Shipping Constraint is updated, then update in existing order
            if (ExistUpdateHistory(ZnodeConstant.OrderShippingConstraintCode, orderModel))
            {
                UpdateOrderShippingConstraint(orderModel);
                orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(ZnodeConstant.OrderShippingConstraintCode, orderModel), orderHistory);
            }

            //to check order status is updated then update in existing order
            if (ExistUpdateHistory("PurchaseOrderNumber", orderModel))
            {
                UpdatePurchaseOrderNumber(orderModel);
                orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey("PurchaseOrderNumber", orderModel), orderHistory);
            }

            if (!string.IsNullOrEmpty(orderModel.AdditionalInstructions))
            {
                OrderNotesModel notesModel = new OrderNotesModel() { Notes = orderModel.AdditionalInstructions, OmsOrderDetailsId = orderModel.OmsOrderDetailsId, CreatedBy = orderModel.CreatedBy, ModifiedBy = orderModel.ModifiedBy };
                AddOrderNote(notesModel);
                notesId = notesModel.OmsNotesId;
            }

            UpdateExternalId(orderModel);

            //to check order status is updated then update in existing order
            if (ExistUpdateHistory(ZnodeConstant.OrderUpdatedStatus, orderModel))
            {
                bool isOrderStateUpdated = UpdateOrderState(orderModel);

                if (isOrderStateUpdated && orderModel.OmsOrderStateId == _omsOrderStateRepository.Table.FirstOrDefault(x => x.OrderStateName == ZnodeOrderStatusEnum.CANCELED.ToString())?.OmsOrderStateId)
                    RevertOrderInventory(orderModel.OmsOrderId, orderModel.UserId);

                orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(ZnodeConstant.OrderUpdatedStatus, orderModel), orderHistory);
                //if update Existing Order then no need to save other history of order.
                if (updateExistingOrder)
                {
                    if (IsNotNull(orderModel.OmsOrderStateId))
                        orderModel.OrderState = _omsOrderStateRepository.GetById(orderModel.OmsOrderStateId).OrderStateName;

                    CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = orderHistory, OmsNotesId = notesId, CreatedBy = orderModel.CreatedBy, ModifiedBy = orderModel.ModifiedBy });
                    return;
                }
            }
            if (ExistUpdateHistory(ZnodeConstant.OrderReturnAllAndCancelStatus, orderModel))
            {
                bool isOrderStateUpdated = UpdateReturnedOrderState(orderModel);
                if (isOrderStateUpdated && orderModel.OrderHistory.ContainsKey(ZnodeConstant.OrderReturnAllAndCancelStatus)) orderModel.OrderHistory.Remove(ZnodeConstant.OrderReturnAllAndCancelStatus);
            }

            if (IsNotNull(orderModel?.OrderHistory))
            {
                foreach (var history in orderModel?.OrderHistory)
                {
                    if (!string.IsNullOrEmpty(history.Key) &&
                        history.Key != ZnodeConstant.OrderPaymentState &&
                        history.Key != ZnodeConstant.OrderTrackingNumber &&
                        history.Key != ZnodeConstant.OrderUpdatedStatus &&
                        history.Key != ZnodeConstant.OrderBillingAddress &&
                        history.Key != ZnodeConstant.OrderInHandsDate &&
                        history.Key != ZnodeConstant.OrderJobName &&
                        history.Key != ZnodeConstant.OrderShippingConstraintCode
                        && history.Key != "PurchaseOrderNumber")
                    {
                        orderHistory = GetConsolidatedHistoryMessage(GetHistoryMessageByKey(history.Key, orderModel), orderHistory);
                    }
                }
            }

            decimal orderAmount = orderModel.OverDueAmount;
            decimal overAllOrderAmount = 0;
            decimal returnAmount = 0;

            string orderLineShippingHistory = string.Empty;

            if (HelperUtility.IsNotNull(orderModel?.OrderLineItemHistory) && orderModel?.OrderLineItemHistory.Count > 0)
            {
                RemoveShippingHistoryForReturn(orderModel?.OrderLineItemHistory);

                foreach (var item in orderModel?.OrderLineItemHistory)
                {
                    orderAmount = 0;
                    if (string.Equals(item.Value.OrderUpdatedStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        returnAmount = BindReturnAmount(returnAmount, item);

                    //Log history for edit shipping of order line item
                    if (IsNotNull(item.Value) && item.Value.IsOrderLineItemShippingUpdate)
                    {
                        orderLineShippingHistory = GenerateOrderLineItemShippingHistory(orderLineShippingHistory, item.Value, orderModel.OrderOldValue.OrderLineItems, orderModel.CultureCode);
                    }
                    else
                    {
                        orderHistory = GenerateOrderLineItemHistory(orderHistory, item.Key, item.Value, orderModel.OrderOldValue.OrderLineItems, orderModel.CultureCode);

                        orderAmount = BindOrderAmount(orderAmount, item);
                        overAllOrderAmount += orderAmount;
                    }
                }
                orderAmount = overAllOrderAmount; ;

                if (orderModel.OrderLineItemHistory.All(x => string.Equals(x.Value.OrderUpdatedStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                    orderAmount = returnAmount;
            }

            if (!string.IsNullOrEmpty(orderHistory))
                CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = orderHistory, OmsNotesId = notesId, OrderAmount = orderAmount, CreatedBy = orderModel.CreatedBy, ModifiedBy = orderModel.ModifiedBy });

            //To save edit shipping line item history in database
            CreateEditShippingOrderHistory(orderModel, orderLineShippingHistory);
        }

        //Bind order amount 
        protected virtual decimal BindOrderAmount(decimal orderAmount, KeyValuePair<string, OrderLineItemHistoryModel> item)
        {
            //To save Amount in Order History If Order Status Updated From submitted To Shipped
            if (string.Equals(item.Value.OrderUpdatedStatus, ZnodeOrderStatusEnum.SHIPPED.ToString(),StringComparison.InvariantCultureIgnoreCase))
                orderAmount = item.Value.SubTotal;
            //if return shipping amount contain value
            if (string.IsNullOrEmpty(item.Value.ReturnShippingAmount))
                orderAmount = string.Equals(item.Value.OrderUpdatedStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    ? (item.Value.SubTotal + item.Value.TaxCost + item.Value.ImportDuty)
                    : orderAmount;
            else
                orderAmount = string.Equals(item.Value.OrderUpdatedStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    ? (item.Value.SubTotal + item.Value.TaxCost + item.Value.ImportDuty + Convert.ToDecimal(item.Value.ReturnShippingAmount))
                    : orderAmount;
            return orderAmount;
        }

        //to save order line item changes to history table
        public virtual string GenerateOrderLineItemHistory(string history, string sku, OrderLineItemHistoryModel skuHistory, List<OrderLineItemModel> oldValue, string cultureCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(skuHistory))
            {
                if (oldValue?.Where(w => w.Sku == sku)?.Count() == 0 || oldValue?.FirstOrDefault(w => w.Sku == sku)?.Quantity == 0)
                    sku = skuHistory.SKU;

                string oldQuantity = Convert.ToString(oldValue.FirstOrDefault(w => w.Sku == sku)?.Quantity);

                string oldStatus = Convert.ToString(oldValue.Where(w => w.Sku == sku).Select(s => s.OrderLineItemState).FirstOrDefault());
                string oldUnitPrice = ZnodeCurrencyManager.FormatPriceWithCurrency(oldValue.Where(w => w.Sku == sku).Select(s => s.Price).FirstOrDefault(), cultureCode, string.Empty);

                string productName = skuHistory.ProductName;
                string qty = Convert.ToString(skuHistory.Quantity);

                if (!string.IsNullOrEmpty(skuHistory.OrderLineAdd))
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemAdded, skuHistory.SKU, skuHistory.OrderLineAdd, qty), history);

                if (!string.IsNullOrEmpty(skuHistory.OrderLineDelete))
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemDeleted, skuHistory.SKU, skuHistory.OrderLineDelete, qty), history);

                if (!string.IsNullOrEmpty(skuHistory.OrderUpdatedStatus) && string.Equals(skuHistory.OrderUpdatedStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemStateChangedToReturnApproved, !string.IsNullOrEmpty(skuHistory.SKU) ? skuHistory.SKU : sku, productName, qty), history);
                    skuHistory.ReturnShippingAmount = string.Empty;
                }
                else if (!string.IsNullOrEmpty(skuHistory.OrderUpdatedStatus) && !(string.Equals(skuHistory.OrderUpdatedStatus, OrderDiscountTypeEnum.PARTIALREFUND.ToString(), StringComparison.OrdinalIgnoreCase)))
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemStateChanged, !string.IsNullOrEmpty(skuHistory.SKU) ? skuHistory.SKU : sku, productName, qty, oldStatus, skuHistory.OrderUpdatedStatus), history);

                if (!string.IsNullOrEmpty(skuHistory.OrderLineQuantity) && !Equals(skuHistory.OrderLineQuantity, "0"))
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemQuantityChanged, sku, productName, oldQuantity, skuHistory.OrderLineQuantity), history);

                if (!string.IsNullOrEmpty(skuHistory.OrderLineUnitPrice))
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemPriceChanged, sku, productName, oldUnitPrice, skuHistory.OrderLineUnitPrice), history);

                if (!string.IsNullOrEmpty(skuHistory.OrderTrackingNumber))
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemTrackingNumberChanged, sku, productName, qty, skuHistory.OrderTrackingNumber), history);

                if (!string.IsNullOrEmpty(skuHistory.ReturnShippingAmount))
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemRetrunShipping, sku, productName, skuHistory.ReturnShippingAmount), history);

                if (!string.IsNullOrEmpty(skuHistory.PartialRefundAmount))
                    history = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemPartialRefund, sku, productName, skuHistory.PartialRefundAmount), history);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return history;
        }

        //to check that do we need to update existing order data return true else return false (in case submit new order with same order number)
        public virtual bool IsExistingOrderUpdated(OrderModel model)
        {
            bool isExistingOrderUpdated = false;
            if (IsNotNull(model?.OrderHistory) || IsNotNull(model?.OrderLineItemHistory)
                && model.OrderHistory.Count > 0 || model.OrderLineItemHistory.Count > 0)
            {
                isExistingOrderUpdated = !IsOrderHistoryExceptPaymentAndOrderState(model);
            }
            return isExistingOrderUpdated;
        }

        //to check return shipping amount only
        public virtual bool IsReturnShipping(OrderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool isReturnShipping = false;
            int historyCount = 0;
            if (IsNotNull(model?.OrderHistory) || IsNotNull(model?.OrderLineItemHistory)
                && model.OrderHistory.Count == 0 || model.OrderLineItemHistory.Count > 0)
            {
                foreach (var item in model?.OrderLineItemHistory)
                {
                    if (!string.IsNullOrEmpty(item.Value.ReturnShippingAmount) && (model.ReturnItemList?.ReturnItemList?.Count == 0 || item.Value.IsShippingReturn))
                    {
                        isReturnShipping = true;
                    }
                    else if (!string.IsNullOrEmpty(item.Value.OrderLineAdd) ||
                        !string.IsNullOrEmpty(item.Value.OrderLineDelete) ||
                        !string.IsNullOrEmpty(item.Value.OrderUpdatedStatus) ||
                        !(!string.IsNullOrEmpty(item.Value.OrderLineQuantity) || !Equals(item.Value.OrderLineQuantity, "0")) ||
                        !string.IsNullOrEmpty(item.Value.OrderLineUnitPrice) ||
                        !string.IsNullOrEmpty(item.Value.OrderTrackingNumber))
                    {
                        historyCount += 1;
                    }
                }

                if (isReturnShipping && historyCount == 0)
                    return true;
                else
                    return false;
            }
            return isReturnShipping;
        }

        //to check order data is updated
        public virtual bool IsOrderDataUpdated(OrderModel model)
        {
            bool isOrderDataUpdated = true;
            if (IsNotNull(model))
            {
                if (IsNull(model.OrderHistory) && IsNull(model.OrderLineItemHistory)
                || (model.OrderHistory.Count == 0 && model.OrderLineItemHistory.Count == 0))
                    isOrderDataUpdated = false;

                if (!string.IsNullOrEmpty(model.AdditionalInstructions))
                    isOrderDataUpdated = true;

                if (!string.IsNullOrEmpty(model.ExternalId))
                    isOrderDataUpdated = true;
            }
            return isOrderDataUpdated;
        }

        //to check order history except payment and order state exist return true/false
        public virtual bool IsOrderHistoryExceptPaymentAndOrderState(OrderModel model)
        {
            int count = 0;
            foreach (var history in model.OrderHistory)
            {
                if (history.Key != ZnodeConstant.OrderUpdatedStatus
                    && history.Key != ZnodeConstant.OrderPaymentState
                    && history.Key != ZnodeConstant.OrderTrackingNumber
                    && history.Key != ZnodeConstant.OrderBillingAddress
                    && history.Key != ZnodeConstant.OrderInHandsDate
                    && history.Key != ZnodeConstant.OrderJobName
                    && history.Key != ZnodeConstant.OrderShippingConstraintCode
                    && history.Key != "PurchaseOrderNumber")
                {
                    count++;
                }

                if (history.Key == ZnodeConstant.OrderBillingAddress && Equals(model?.BillingAddress?.AddressId, model?.ShippingAddress?.AddressId))
                    count++;
            }

            if (count == 0)
            {
                return model?.OrderLineItemHistory.Count > 0;
            }

            return count > 0;
        }

        //to get consolidated history message
        public virtual string GetConsolidatedHistoryMessage(string message, string mergedMessage)
        {
            if (!string.IsNullOrEmpty(mergedMessage))
                mergedMessage += $"<br/>{ message}";
            else
                mergedMessage = message;

            return mergedMessage;
        }

        // Get order details for returned line items.
        public virtual ZnodeOmsOrderDetail GetOrderDetailsForLineItem(int orderId, NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            string stateName = filters.Where(s => s.FilterName.ToLower() == Constants.FilterKeys.OmsOrderStateName.ToLower()).Select(s => s.FilterValue).FirstOrDefault();

            ZnodeLogging.LogMessage("Input parameter OrderId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderId);

            int orderStateId = 0;
            if (!string.IsNullOrEmpty(stateName))
            {
                orderStateId = _omsOrderStateRepository.Table.Where(w => w.OrderStateName.ToLower() == stateName.ToLower()).Select(s => s.OmsOrderStateId).FirstOrDefault();
                filters.RemoveAll(x => x.Item1.Equals(Constants.FilterKeys.OmsOrderStateName, StringComparison.InvariantCultureIgnoreCase));
            }

            ZnodeLogging.LogMessage("orderStateId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderStateId);
            if (orderStateId > 0)
            {
                ZnodeOmsOrderDetail orderDetails = _orderDetailsRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause, GetExpands(expands));

                orderDetails.ZnodeOmsOrderLineItems = orderDetails.ZnodeOmsOrderLineItems.Where(w => w.OrderLineItemStateId == orderStateId && w.IsActive).ToList();

                decimal lineItemTax = 0m;
                decimal lineItemShippingCost = 0m;
                decimal subTotal = 0m;
                decimal discountAmount = 0m;
                decimal lineItemImportDuty = 0m;

                foreach (ZnodeOmsOrderLineItem omsOrderLineItem in orderDetails?.ZnodeOmsOrderLineItems)
                {
                    ZnodeOmsTaxOrderLineDetail omsTaxOrderLineDetail = _omsTaxOrderLineDetailRepository.Table.FirstOrDefault(w => w.OmsOrderLineItemsId == omsOrderLineItem.OmsOrderLineItemsId);
                    ZnodeOmsOrderLineItem znodeOmsOrderLineItem = _orderLineItemRepository.Table.FirstOrDefault(w => w.OmsOrderLineItemsId == omsOrderLineItem.OmsOrderLineItemsId);
                    lineItemTax = lineItemTax + Convert.ToDecimal(omsTaxOrderLineDetail?.GST) + Convert.ToDecimal(omsTaxOrderLineDetail?.HST) + Convert.ToDecimal(omsTaxOrderLineDetail?.PST) + Convert.ToDecimal(omsTaxOrderLineDetail?.SalesTax) + Convert.ToDecimal(omsTaxOrderLineDetail?.VAT);
                    lineItemImportDuty = lineItemImportDuty + Convert.ToDecimal(omsTaxOrderLineDetail?.ImportDuty);
                    subTotal = subTotal + (omsOrderLineItem.Price * Convert.ToDecimal(omsOrderLineItem.Quantity));

                    if (IsNull(orderDetails?.ZnodeOmsOrderLineItems.FirstOrDefault(x=> x.OmsOrderLineItemsId == omsOrderLineItem.ParentOmsOrderLineItemsId)))
                        discountAmount = discountAmount + Convert.ToDecimal(znodeOmsOrderLineItem.DiscountAmount);
                }

                orderDetails.ShippingCost = 0m;
                orderDetails.TaxCost = lineItemTax;
                orderDetails.ImportDuty = lineItemImportDuty;
                orderDetails.Total = subTotal + lineItemShippingCost + lineItemTax + lineItemImportDuty - discountAmount;
                orderDetails.SubTotal = subTotal;
                orderDetails.DiscountAmount = discountAmount;

                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return orderDetails;
            }
            else
            {
                return new ZnodeOmsOrderDetail();
            }
        }

        //Calculate cartItem to send mail
        public virtual void CalculateCartItemForResendMail(OrderModel orderModel, string omsOrderLineId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter omsOrderLineId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, omsOrderLineId);
            string[] omsOrderLineArray = omsOrderLineId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            SetOrderLineItems(orderModel, omsOrderLineArray);

            decimal lineItemTax = 0m;
            decimal lineItemImportDuty = 0m;
            decimal lineItemShippingCost = 0m;
            decimal subTotal = 0m;
            decimal lineItemOrderDiscountAmount = 0m;
            decimal lineItemCSRDiscount = 0m;
            decimal lineItemShippingDiscount = 0m;
            foreach (OrderLineItemModel omsOrderLineItem in orderModel.OrderLineItems)
            {
                ZnodeOmsTaxOrderLineDetail omsTaxOrderLineDetail = _omsTaxOrderLineDetailRepository.Table.FirstOrDefault(w => w.OmsOrderLineItemsId == omsOrderLineItem.OmsOrderLineItemsId);
                lineItemTax = lineItemTax + Convert.ToDecimal(omsTaxOrderLineDetail?.GST) + Convert.ToDecimal(omsTaxOrderLineDetail?.HST) + Convert.ToDecimal(omsTaxOrderLineDetail?.PST) + Convert.ToDecimal(omsTaxOrderLineDetail?.SalesTax) + Convert.ToDecimal(omsTaxOrderLineDetail?.VAT);
                lineItemImportDuty = lineItemImportDuty + Convert.ToDecimal(omsTaxOrderLineDetail?.ImportDuty);
                lineItemShippingCost = lineItemShippingCost + Convert.ToDecimal(omsOrderLineItem.ShippingCost);

                ShoppingCartItemModel shoppingCartItem = orderModel.ShoppingCartModel?.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == omsOrderLineItem.OmsOrderLineItemsId);
                lineItemOrderDiscountAmount = lineItemOrderDiscountAmount + GetLineItemDiscountAmount(omsOrderLineItem, shoppingCartItem);
                lineItemCSRDiscount = lineItemCSRDiscount + GetLineItemCSRDiscount(omsOrderLineItem, shoppingCartItem);
                lineItemShippingDiscount = lineItemShippingDiscount + GetLineItemShippingDiscount(omsOrderLineItem, shoppingCartItem);
                subTotal = subTotal + (omsOrderLineItem.Price * Convert.ToDecimal(omsOrderLineItem.Quantity));
            }
            orderModel.GiftCardAmount = 0;
            orderModel.ShippingHandlingCharges = 0;
            orderModel.ReturnCharges = 0;
            orderModel.CSRDiscountAmount = lineItemCSRDiscount;
            orderModel.DiscountAmount = lineItemOrderDiscountAmount;
            orderModel.ShippingCost = lineItemShippingCost;
            orderModel.ShippingDiscount = lineItemShippingDiscount;
            orderModel.TaxCost = lineItemTax;
            orderModel.ImportDuty = lineItemImportDuty;
            orderModel.SubTotal = subTotal;
            orderModel.Total = subTotal + lineItemShippingCost + lineItemTax + lineItemImportDuty - lineItemOrderDiscountAmount - lineItemCSRDiscount - lineItemShippingDiscount;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //to set update order data
        public virtual SubmitOrderModel SetUpdateOrderData(OrderModel model)
        {
            SubmitOrderModel updateModel = new SubmitOrderModel
            {
                OrderId = model.OmsOrderId,
                OmsOrderDetailsId = model.OmsOrderDetailsId,
                OrderStateId = model.OmsOrderStateId,
                PaymentStateId = model.OmsPaymentStateId,
                TrackingNumber = model.TrackingNumber,
                OrderNumber = model.OrderNumber,
                ReturnOrderLineItems = model.ReturnItemList,
                IsLineItemReturned = model.IsLineItemReturned,
                LineItemReturnAmount = model.ShoppingCartModel?.OverDueAmount,
                CreatedBy = model.CreatedBy,
                ModifiedBy = model.ModifiedBy,
                RefundedSkus = string.Join(",", model?.OrderLineItemHistory?.Where(w => w.Value.OrderUpdatedStatus == ZnodeOrderStatusEnum.RETURNED.ToString())?.Select(p => p.Value.OmsOrderLineItemsId.ToString()))
            };
            return updateModel;
        }

        //to bind order details that need to return
        public virtual OrderModel BindOrderData(ZnodeOrderFulfillment order, ShoppingCartModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                OrderModel orderModel = new OrderModel();
                if (IsNotNull(order))
                {
                    MapBillingAddressInOrderModel(order, orderModel);

                    MapShippingAddressInOrderModel(order, orderModel);

                    MapOrderDetailsInOrderModel(order, model, orderModel);

                    List<ZnodeAddress> addressList = GetAddressList(model);

                    List<ZnodeOmsOrderStateShowToCustomer> orderStatusList = GetOrderStatusForCustomerList(orderModel.OrderLineItems);

                    PrepareOrderLineItemData(order, orderModel, addressList, orderStatusList);

                    if (IsNotNull(order.Order))
                    {
                        string street1 = string.IsNullOrEmpty(order.Order.BillingAddress.Address2) ? string.Empty : "<br />" + order.Order.BillingAddress.Address2;
                        orderModel.BillingAddressHtml = $"{order.Order?.BillingAddress.FirstName}{" "}{order.Order?.BillingAddress.LastName}{"<br />"}{order.BillingAddress.CompanyName}{"<br />"}{order.Order.BillingAddress.Address1}{street1}{"<br />"}{ order.Order.BillingAddress.CityName}{"<br />"}{(string.IsNullOrEmpty(order.Order.BillingAddress.StateName) ? order.Order.BillingAddress.StateCode : order.Order.BillingAddress.StateName)}{"<br />"}{order.Order.BillingAddress.PostalCode}{"<br />"}{order.Order.BillingAddress.CountryName}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{order.Order.BillingAddress.PhoneNumber}";
                    }
                }

                if (IsNotNull(model))
                {
                    orderModel.PortalId = model.PortalId;
                    orderModel.PaymentDisplayName = model.Payment?.PaymentDisplayName;
                    orderModel.IsQuoteOrder = model.IsQuoteOrder;
                }

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return orderModel;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in BindOrderData method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get Order Address List
        protected virtual List<ZnodeAddress> GetAddressList(ShoppingCartModel model)
        {
            List<ZnodeAddress> addressList = new List<ZnodeAddress>();
            addressList.Add(model.ShippingAddress.ToEntity<ZnodeAddress>());
            return addressList;
        }

        //Prepare order line item details like formating price, quantity etc.
        protected virtual void PrepareOrderLineItemData(ZnodeOrderFulfillment order, OrderModel orderModel, List<ZnodeAddress> addressList, List<ZnodeOmsOrderStateShowToCustomer> orderStatusList)
        {

            List<string> downloadabeSKUs = GetDownloadableSkus(orderModel);
            foreach (OrderLineItemModel item in orderModel.OrderLineItems)
            {
                item.ShippingAddressHtml = GetOrderShipmentAddress(item.ZnodeOmsOrderShipment, addressList);
                item.OrderLineItemState = GetOrderStatusForCustomer(item.OrderLineItemStateId, orderStatusList);
                item.Price = (item.Price == 0)
                     ? (order?.OrderLineItems?.FirstOrDefault(x => x.OmsOrderLineItemsId == item.OmsOrderLineItemsId)?.OrderLineItemCollection?.Count > 0) ? order.OrderLineItems.FirstOrDefault(x => x.OmsOrderLineItemsId == item.OmsOrderLineItemsId).OrderLineItemCollection.FirstOrDefault().Price : item.Price
                     : item.Price; item.Quantity = Convert.ToDecimal(ServiceHelper.ToInventoryRoundOff(item.Quantity));

                item.Quantity = Convert.ToDecimal(ServiceHelper.ToInventoryRoundOff(item.Quantity));
                item.OrderLineItemCollection?.ForEach(x =>
                {
                    x.Quantity = Convert.ToDecimal(ServiceHelper.ToInventoryRoundOff(x.Quantity));
                    x.IsDownloadableSKU = downloadabeSKUs.Contains(item.Sku);
                });
                item.IsDownloadableSKU = downloadabeSKUs.Contains(item.Sku);
            }
        }

        //Fetch the products which are downloadable from the line items
        protected virtual List<string> GetDownloadableSkus(OrderModel orderModel)
        {
            List<string> skus = orderModel?.OrderLineItems?.SelectMany(x => new string[] { x.Sku, String.Join(",", x?.OrderLineItemCollection?.Select(y => y.Sku)) }).ToList();
            List<string> associatedSKUs = new List<string>();

            skus?.ForEach(x => { associatedSKUs.AddRange(x.Split(',')); });
            DataTable dtAssociatedSKUs = SetSkuData(associatedSKUs);
            IZnodeViewRepository<string> objStoredProc = new ZnodeViewRepository<string>();
            objStoredProc.SetTableValueParameter("@SKU", dtAssociatedSKUs, ParameterDirection.Input, SqlDbType.Structured, "dbo.AssociatedSkus");
            List<string> validAssociatedSKUs = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimDownloadableProduct @SKU")?.ToList();
            return validAssociatedSKUs;
        }

        // Set sku Data for SP.
        protected virtual DataTable SetSkuData(List<string> associatedSkus)
        {
            ZnodeLogging.LogMessage("Execution Started: Method Name: SetSkuDataForSP ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose);

            // Bind associatedSKUs Values in Data Table.
            DataTable table = BindOrderSkusToDataTable(associatedSkus);

            ZnodeLogging.LogMessage("Execution Done", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose);

            return table;
        }

        // Binding number of sku with DataTable columns.
        protected virtual DataTable CreateSkuDataTable()
        {
            ZnodeLogging.LogMessage("Execution Started: Method Name: CreateDataTableForOrderL ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            DataTable table = new DataTable("ZnodePimDownloadableProduct");
            table.Columns.Add("Sku", typeof(string));

            ZnodeLogging.LogMessage("Execution Done", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return table;
        }

        // Bind DataTable Skus.
        protected virtual DataTable BindOrderSkusToDataTable(List<string> associatedSkus)
        {
            ZnodeLogging.LogMessage("Execution Started: Method Name: BindOrderLineItemsWithDataTable ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Create Data Table For associatedSKUs.
            DataTable table = CreateSkuDataTable();

            foreach (string sku in associatedSkus)
            {
                table.Rows.Add(sku);
            }

            ZnodeLogging.LogMessage("Execution Done", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return table;
        }
        //Map other order details in order model.
        protected virtual void MapOrderDetailsInOrderModel(ZnodeOrderFulfillment order, ShoppingCartModel model, OrderModel orderModel)
        {
            try
            {
                orderModel.OmsOrderId = order.OrderID;
                orderModel.OrderNumber = order.Order?.OrderNumber;
                orderModel.PortalId = model.PortalId;
                order.OrderDateWithTime = Convert.ToString(order.OrderDate.ToTimeZoneDateTimeFormat());
                order.InHandDate = Convert.ToDateTime(order.InHandDate?.ToTimeZoneDateTimeFormat());
                orderModel.OverDueAmount = order.OrderOverDueAmount;
                orderModel.OrderLineItems = order.OrderLineItems;
                orderModel.PaymentDisplayName = model.Payment.PaymentDisplayName;
                orderModel.CreditCardNumber = order?.CreditCardNumber;
                orderModel.ShippingCost = order.ShippingCost;
                orderModel.TaxCost = order.TaxCost;
                orderModel.ImportDuty = order.ImportDuty;
                orderModel.DiscountAmount = order.DiscountAmount;
                orderModel.GiftCardAmount = order.GiftCardAmount;
                orderModel.GiftCardNumber = order.GiftCardNumber;
                orderModel.Total = order.Total;
                orderModel.OrderTotalWithoutVoucher = order.OrderTotalWithoutVoucher;
                orderModel.SubTotal = order.SubTotal;
                orderModel.OrderDate =Convert.ToDateTime(order.OrderDate.ToTimeZoneDateTimeFormat());
                orderModel.InHandDate = Convert.ToDateTime(order.InHandDate?.ToTimeZoneDateTimeFormat());
                orderModel.ShippingTypeName = order.ShippingName;
                orderModel.TransactionId = order.Order.TransactionId;
                orderModel.OmsOrderDetailsId = order.Order.OmsOrderDetailsId;
                orderModel.CreatedBy = order.Order.CreatedBy;
                orderModel.ModifiedBy = order.Order.ModifiedBy;
                orderModel.Custom1 = order.Custom1;
                orderModel.Custom2 = order.Custom2;
                orderModel.Custom3 = order.Custom3;
                orderModel.Custom4 = order.Custom4;
                orderModel.Custom5 = order.Custom5;
                orderModel.AccountNumber = model?.Shipping?.AccountNumber;
                orderModel.ShippingMethod = model?.Shipping?.ShippingMethod;
                orderModel.IsFromAdminOrder = model.IsFromAdminOrder;
                orderModel.IsFromManageOrder = model.IsFromManageOrder;
                orderModel.UserId = model.UserId.GetValueOrDefault();
                orderModel.ExternalId = model.ExternalId;
                orderModel.OrderState = model.OrderStatus;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapOrderDetailsInOrderModel method for OrderId " + orderModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map user shipping address in order model.
        protected virtual void MapShippingAddressInOrderModel(ZnodeOrderFulfillment order, OrderModel orderModel)
        {
            string orderNumber = string.IsNullOrEmpty(orderModel?.OrderNumber) ? order?.Order?.OrderNumber : orderModel?.OrderNumber;
            try
            {
                //Map Shipping Address.
                orderModel.ShippingAddress.EmailAddress = string.IsNullOrEmpty(order.Email) ? order.ShippingAddress.EmailAddress : order.Email;
                orderModel.ShippingAddress.Address1 = order.ShippingAddress.Address1;
                orderModel.ShippingAddress.Address2 = order.ShippingAddress.Address2;
                orderModel.ShippingAddress.StateCode = order.ShippingAddress.StateCode;
                orderModel.ShippingAddress.CompanyName = order.ShippingAddress.CompanyName;
                orderModel.ShippingAddress.CityName = order.ShippingAddress.CityName;
                orderModel.ShippingAddress.CountryCode = string.IsNullOrEmpty(order?.ShippingAddress?.CountryCode) ? order?.ShippingAddress?.CountryName : order?.ShippingAddress?.CountryCode;
                orderModel.ShippingAddress.PostalCode = order.ShippingAddress.PostalCode;
                orderModel.ShippingAddress.FirstName = order.ShippingAddress.FirstName;
                orderModel.ShippingAddress.LastName = order.ShippingAddress.LastName;
                orderModel.ShippingAddress.DisplayName = order.ShippingAddress.DisplayName;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapShippingAddressInOrderModel method for OrderId " + orderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map user billing address details in order model.
        protected virtual void MapBillingAddressInOrderModel(ZnodeOrderFulfillment order, OrderModel orderModel)
        {
            string orderNumber = string.IsNullOrEmpty(orderModel?.OrderNumber) ? order?.Order?.OrderNumber : orderModel?.OrderNumber;
            try
            {
                //Map Billing Address
                orderModel.BillingAddress.EmailAddress = string.IsNullOrEmpty(order.Email) ? order.BillingAddress.EmailAddress : order.Email;
                orderModel.BillingAddress.Address1 = order.BillingAddress.Address1;
                orderModel.BillingAddress.Address2 = order.BillingAddress.Address2;
                orderModel.BillingAddress.StateCode = order.BillingAddress.StateCode;
                orderModel.BillingAddress.StateName = order.BillingAddress.StateName;
                orderModel.BillingAddress.CompanyName = order.BillingAddress.CompanyName;
                orderModel.BillingAddress.CityName = order.BillingAddress.CityName;
                orderModel.BillingAddress.CountryCode = string.IsNullOrEmpty(order?.BillingAddress?.CountryCode) ? order?.BillingAddress?.CountryName : order?.BillingAddress?.CountryCode;
                orderModel.BillingAddress.PostalCode = order.BillingAddress.PostalCode;
                orderModel.BillingAddress.FirstName = order.BillingAddress.FirstName;
                orderModel.BillingAddress.LastName = order.BillingAddress.LastName;
                orderModel.BillingAddress.DisplayName = order.BillingAddress.DisplayName;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapBillingAddressInOrderModel method for OrderId " + orderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Expands necessary to get Order for Resend Mail.
        public virtual NameValueCollection GetOrderExpandForResendMail()
        {
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrder.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsOrder.ToString().ToLower());
            return expands;
        }

        // Map returned order amount.
        public virtual void MapReturnedTotal(OrderModel orderModel, OrderModel returnedOrderModel)
        {
            orderModel.ReturnTaxCost = returnedOrderModel.TaxCost;
            orderModel.ReturnShippingCost = returnedOrderModel.ShippingCost;
            orderModel.ReturnSubTotal = returnedOrderModel.SubTotal;
            orderModel.ReturnTotal = returnedOrderModel.Total;
            orderModel.ReturnImportDuty = returnedOrderModel.ImportDuty;
        }

        //Map Order data to Shopping cart.
        public virtual void MapOrderModelToShoppingCart(OrderModel orderModel)
        {
            orderModel.ShoppingCartModel.AdditionalInstructions = !string.IsNullOrEmpty(orderModel.AdditionalInstructions) ? orderModel.AdditionalInstructions : orderModel.ShoppingCartModel.AdditionalInstructions;
            orderModel.ShoppingCartModel.PurchaseOrderNumber = orderModel.PurchaseOrderNumber;
            orderModel.ShoppingCartModel.PODocumentName = orderModel.PoDocument;
            orderModel.ShoppingCartModel.Token = orderModel.ShoppingCartModel.Token ?? orderModel.PaymentTransactionToken;
            orderModel.ShoppingCartModel.CreditCardNumber = orderModel.ShoppingCartModel.CreditCardNumber ?? orderModel.CreditCardNumber;
            orderModel.ShoppingCartModel.CardType = orderModel.ShoppingCartModel.CardType ?? orderModel.CardType;
            orderModel.ShoppingCartModel.CreditCardExpMonth = orderModel.ShoppingCartModel.CreditCardExpMonth ?? orderModel.CreditCardExpMonth;
            orderModel.ShoppingCartModel.CreditCardExpYear = orderModel.ShoppingCartModel.CreditCardExpYear ?? orderModel.CreditCardExpYear;
            orderModel.ShoppingCartModel.ExternalId = orderModel.ShoppingCartModel.ExternalId ?? orderModel.ExternalId;
            orderModel.ShoppingCartModel.JobName = orderModel.JobName;
            orderModel.ShoppingCartModel.ShippingConstraintCode = orderModel.ShippingConstraintCode;
            orderModel.ShoppingCartModel.InHandDate = orderModel.InHandDate;
            SetOrderState(orderModel);
        }

        //Get Tracking Url by ShippingId.
        public virtual string GetTrackingUrlByShippingId(int shippingId)
         => _shippingRepository.GetById(shippingId)?.TrackingUrl;

        //Set Tracking Url.
        public virtual string SetTrackingUrl(string trackingNo, string trackingUrl)
        {
            ZnodeLogging.LogMessage("Input parameter trackingNo and trackingUrl:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { trackingNo, trackingUrl });
            return IsNotNull(trackingUrl) ? $"<a target=_blank href={ trackingUrl + trackingNo }>{trackingNo} </ a >" : trackingNo;
        }

        // Check for allowed territories.
        public virtual bool IsAllowedTerritories(ShoppingCartModel model) => model.ShoppingCartItems.Where(w => w.IsAllowedTerritories == false).ToList().Count > 0;

        //Get expands and add them to navigation properties
        public virtual List<string> GetExpandsForOrderLineItem(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands?.HasKeys() ?? false)
            {
                foreach (string key in expands.Keys)
                    //Add expand keys
                    if (Equals(key, ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower())) SetExpands(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString(), navigationProperties);
            }
            return navigationProperties;
        }

        //Cancel tax transaction.
        private static void CancelTaxTransaction(bool updated, ZnodeOmsOrderDetail order, List<ZnodeOmsOrderState> orderStateList)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool isCancelledOrder = order.OmsOrderStateId.Equals(orderStateList?.FirstOrDefault(x => x.OrderStateName == ZnodeOrderStatusEnum.CANCELED.ToString())?.OmsOrderStateId);
            if (updated && isCancelledOrder)
            {
                IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
                int? omsLineItemId = order?.ZnodeOmsOrderLineItems?.FirstOrDefault()?.OmsOrderLineItemsId;

            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Filters to get order data.
        protected virtual FilterCollection CreateFiltersForOrder(OrderStateParameterModel model)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(Constants.FilterKeys.OmsOrderId, FilterOperators.Equals, model.OmsOrderId.ToString()));
            filter.Add(new FilterTuple(Constants.FilterKeys.IsActive, FilterOperators.Equals, Convert.ToString(true)));
            ZnodeLogging.LogMessage("Filter details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, filter);
            return filter;
        }

        //Expand for order line item.
        protected virtual NameValueCollection GetOrderLineItemExpands()
        {
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower());
            return expands;
        }

        //Method for subtracting days from date.
        public virtual void GetAndMapRmaDetails(OrderModel orderModel)
        {
            ZnodeRmaConfiguration rmaConfiguration = _rmaConfigurationRepository.GetEntity(string.Empty);
            DateTime lastDateOfReturn = orderModel.OrderDate.Subtract(TimeSpan.FromDays(Convert.ToDouble(-rmaConfiguration?.MaxDays ?? 90)));

            if (IsNotNull(rmaConfiguration))
                orderModel.IsEmailNotificationForRma = rmaConfiguration.IsEmailNotification;
            orderModel.IsValidForRma = CompareTwoDates(DateTime.UtcNow, lastDateOfReturn);
            orderModel.IsAnyPendingReturn = IsAnyPendingReturn(orderModel.OmsOrderId);
        }

        //For Comparing two dates and get result.
        public virtual bool CompareTwoDates(DateTime orderCreatedDate, DateTime lastDateOfReturn)
        {
            int result = DateTime.Compare(orderCreatedDate, lastDateOfReturn);
            return (result <= 0);
        }

        //Method for check any pending return is active.
        protected virtual bool IsAnyPendingReturn(int orderId)
        {
            bool isAnyPendingReturn = false;
            if (orderId > 0)
            {
                IZnodeRepository<ZnodeRmaReturnDetail> _rmaReturnDetailRepository = new ZnodeRepository<ZnodeRmaReturnDetail>();

                isAnyPendingReturn = (from _rmaReturnDetail in _rmaReturnDetailRepository.Table
                                      where _rmaReturnDetail.OmsOrderId == orderId && _rmaReturnDetail.IsActive == true &&
                                     (_rmaReturnDetail.RmaReturnStateId == (int)ZnodeReturnStateEnum.SUBMITTED ||
                                     _rmaReturnDetail.RmaReturnStateId == (int)ZnodeReturnStateEnum.IN_REVIEW ||
                                     _rmaReturnDetail.RmaReturnStateId == (int)ZnodeReturnStateEnum.RECEIVED)
                                      select _rmaReturnDetail.RmaReturnDetailsId).Any();
            }
            return isAnyPendingReturn;

        }

        //to set over due amount for return all items in the cart
        public virtual void SetOverDueAmountForReturnAllItem(OrderModel model)
        {
            if (IsNotNull(model))
                model.OverDueAmount = (model.Total - (model.ShippingCost + model.ShippingDifference)) * -1;

            decimal returnShippingCost = 0;
            int returnCount = model?.ReturnItemList?.ReturnItemList?.Where(w => w.IsAlreadyReturned == true).Count() ?? 0;

            returnShippingCost = returnCount > 0 ? model?.ReturnItemList?.ReturnItemList?.Where(x => x.IsShippingReturn == true && x.IsAlreadyReturned == true)?.Sum(x => x.ShippingCost) ?? 0 : model?.ReturnItemList?.ReturnItemList?.Where(x => x.IsShippingReturn == true)?.Sum(x => x.ShippingCost) ?? 0;

            model.OverDueAmount += (returnShippingCost * -1);
        }

        //to update line item state Id as per order stateId
        public virtual void UpdateLineItemState(int orderId, int previousOrderStateId, int currentOrderStateId, DateTime? shipDate)
        {
            ZnodeLogging.LogMessage("Input parameters orderId, previousOrderStateId and currentOrderStateId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderId, previousOrderStateId, currentOrderStateId });
            if (previousOrderStateId == currentOrderStateId)
            {
                previousOrderStateId = _previousOrderStateId;
            }

            List<ZnodeOmsOrderLineItem> lineItemToUpdate = (from _details in _orderDetailsRepository.Table
                                                            join _lineitem in _orderLineItemRepository.Table on _details.OmsOrderDetailsId equals _lineitem.OmsOrderDetailsId
                                                            where _details.OmsOrderId == orderId &&
                                                           _details.IsActive == true &&
                                                           _lineitem.OrderLineItemStateId == previousOrderStateId
                                                            select _lineitem).ToList();

            ZnodeLogging.LogMessage("lineItemToUpdate count :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { lineItemToUpdate?.Count });

            foreach (ZnodeOmsOrderLineItem item in lineItemToUpdate)
            {
                item.OrderLineItemStateId = currentOrderStateId;
                item.ShipDate = currentOrderStateId == 20 ? shipDate : null;
                _orderLineItemRepository.Update(item);
            }
        }

        //to set order state
        public virtual void SetOrderState(OrderModel model)
        {
            if (ExistUpdateHistory(ZnodeConstant.OrderUpdatedStatus, model))
            {
                _previousOrderStateId = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == model.OmsOrderId && x.IsActive == true).FirstOrDefault().OmsOrderStateId;
            }
        }

        //to set over due amount for return all items in the cart
        public virtual void CancelOrderAmount(OrderModel model)
        {
            if (IsNotNull(model) && !string.IsNullOrEmpty(model.OrderState) &&
                String.Equals(model.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.OrdinalIgnoreCase) &&
                model?.ShoppingCartModel?.ShoppingCartItems?.Count < 1 &&
                model.ReturnItemList?.ReturnItemList?.Count > 0)
            {
                model.Total = (model.Total - model.ReturnItemList.Total);
            }
            if (IsNotNull(model) && !string.IsNullOrEmpty(model.OrderState) &&
                String.Equals(model.OrderState, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.OrdinalIgnoreCase) &&
                model.ReturnItemList?.ReturnItemList?.Count > 0)
            {
                model.Total = (model.Total - Convert.ToDecimal(model.ReturnCharges) - Convert.ToDecimal(model.ShippingHandlingCharges));
                model.OverDueAmount = model.Total  > 0 ? - (model.Total): 0;
            }
        }

        //If the external id is set for an order then update the external id in order detail table.
        public virtual void UpdateExternalId(OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("ExternalId for order:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderModel?.ExternalId });

            //If the external id is set for an order then update the external id in order detail table.
            if (!string.IsNullOrEmpty(orderModel?.ExternalId))
            {
                ZnodeOmsOrderDetail orderDetail = _orderDetailsRepository.Table.FirstOrDefault(x => x.OmsOrderDetailsId == orderModel.OmsOrderDetailsId);
                if (IsNotNull(orderDetail) && !Equals(orderDetail.ExternalId, orderModel.ExternalId))
                {
                    orderDetail.ExternalId = orderModel.ExternalId;
                    ZnodeLogging.LogMessage(_orderDetailsRepository.Update(orderDetail) ? string.Format(Admin_Resources.UpdateExternalId, orderModel.OmsOrderId) : string.Format(Admin_Resources.FailToUpdateExternalId, orderModel.OmsOrderId), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                }
            }
        }

        /// <summary>
        /// To return shipping amount of already returned item but but shipping is not returned at the time of return line item.
        /// </summary>
        /// <param name="model">OrderModel</param>
        /// <param name="isShippingCostUpdateRequired">If set to false then shipping cost should not get updated in database.</param>
        /// <returns>bool</returns>
        public virtual bool ReturnShippingAmount(OrderModel model, bool isShippingCostUpdateRequired = true)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool isSuccess = false;
            List<int> lineitemIds = new List<int>();
            foreach (var item in model?.OrderLineItemHistory)
            {
                if (!string.IsNullOrEmpty(item.Value.ReturnShippingAmount))
                {
                    lineitemIds.Add(item.Value.OmsOrderLineItemsId);
                }
            }
            decimal retrunShippingCost = GetShippingCostForReturnedItem(lineitemIds, isShippingCostUpdateRequired);
            ZnodeLogging.LogMessage("retrunShippingCost returned from GetShippingCostForReturnedItem:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { retrunShippingCost });

            if (HelperUtility.IsNotNull(retrunShippingCost) && retrunShippingCost > 0)
            {
                //Shipping cost should not get updated if this flag is set to false.
                if (isShippingCostUpdateRequired)
                    UpdateOrderShippingCost(model.OmsOrderId, retrunShippingCost);
                model.OverDueAmount = retrunShippingCost * -1;
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isSuccess;
        }

        //to update shipping cost of existing order by OrderId
        public virtual void UpdateOrderShippingCost(int orderId, decimal returnShipping)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters orderId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderId });

            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId && x.IsActive == true);
            if (HelperUtility.IsNotNull(order) && returnShipping > 0)
            {
                if (HelperUtility.IsNotNull(order.ShippingDifference) && order.ShippingDifference > 0 && order.ShippingDifference >= returnShipping)
                {
                    order.ShippingDifference = Convert.ToDecimal(order.ShippingDifference - returnShipping);
                }
                else if (HelperUtility.IsNotNull(order.ShippingCost) && order.ShippingCost > 0)
                {
                    order.ShippingCost = Convert.ToDecimal(order.ShippingCost - returnShipping);
                }
                order.Total = Convert.ToDecimal(order.Total - returnShipping) > 0 ? Convert.ToDecimal(order.Total - returnShipping) : 0;

                _orderDetailsRepository.Update(order);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        /// <summary>
        /// To get shipping cost for returned item by line item IDs.
        /// </summary>
        /// <param name="lineitemIds"></param>
        /// <param name="isLineItemUpdateRequired">If set to false then order cart line item should not get updated in database.</param>
        /// <returns></returns>
        public virtual decimal GetShippingCostForReturnedItem(List<int> lineitemIds, bool isLineItemUpdateRequired = true)
        {
            decimal totalRetrunShippingCost = 0;
            ZnodeLogging.LogMessage("lineitemIds count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, lineitemIds?.Count);
            List<ZnodeOmsOrderLineItem> items = _orderLineItemRepository.Table.Where(x => lineitemIds.Contains(x.OmsOrderLineItemsId)).ToList();
            foreach (ZnodeOmsOrderLineItem item in items)
            {
                totalRetrunShippingCost += HelperUtility.IsNotNull(item.ShippingCost) ? item.ShippingCost.GetValueOrDefault() : 0;
            }

            if (isLineItemUpdateRequired)
            {
                foreach (ZnodeOmsOrderLineItem item in items)
                {
                    item.IsShippingReturn = true;
                    _orderLineItemRepository.Update(item);
                }
            }
            return totalRetrunShippingCost;
        }

        // Map Customer Shipping.
        public virtual void CustomerShipping(OrderModel orderModel)
        {
            var shipping = (from customerShipping in _omsCustomerShippingRepository.Table
                            join shippingType in _shippingTypeRepository.Table on customerShipping.ShippingTypeId equals shippingType.ShippingTypeId
                            where customerShipping.OmsOrderDetailsId == orderModel.OmsOrderDetailsId
                            select new { AccountNumber = customerShipping.AccountNumber, ShippingMethod = customerShipping.ShippingMethod, ShippingTypeClassName = shippingType.ClassName })?.FirstOrDefault();

            if (IsNotNull(shipping))
            {
                orderModel.AccountNumber = shipping.AccountNumber;
                orderModel.ShippingMethod = shipping.ShippingMethod;
                orderModel.ShippingTypeClassName = shipping.ShippingTypeClassName;
            }
        }

        //Get portal payment display name
        public virtual string GetPortalPaymentDisplayName(int paymentSettingId, int portalId)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { paymentSettingId = paymentSettingId, portalId = portalId });
            return _portalPaymentSettingRepository.Table.Where(x => x.PaymentSettingId == paymentSettingId && x.PortalId == portalId).FirstOrDefault()?.PaymentDisplayName ?? string.Empty;
        }

        private static void SetCustomerShipping(OrderModel model)
        {
            model.ShoppingCartModel.Shipping.AccountNumber = model?.AccountNumber;
            ZnodeLogging.LogMessage("AccountNumber in CustomerShipping:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, model.ShoppingCartModel.Shipping.AccountNumber);
            model.ShoppingCartModel.Shipping.ShippingMethod = model?.ShippingMethod;
            ZnodeLogging.LogMessage("CustomerShipping details in CustomerShipping:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, model.ShoppingCartModel.Shipping.ShippingMethod);
        }

        //to check shipping cost for the line item that has been already returned
        public virtual void IsShippingCostReturned(int orderId, List<ReturnOrderLineItemModel> itemstoreturn)
        {
            ZnodeLogging.LogMessage("orderId and itemstoreturn count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, itemstoreturnCount = itemstoreturn?.Count });

            if (itemstoreturn?.Count > 0)
            {
                string ids = string.Join(",", itemstoreturn.Select(x => x.OmsOrderLineItemsId));
                var order = (from dtl in _orderDetailsRepository.Table
                             join itm in _orderLineItemRepository.Table on dtl.OmsOrderDetailsId equals itm.OmsOrderDetailsId
                             where dtl.OmsOrderDetailsId == itm.OmsOrderDetailsId &&
                             dtl.OmsOrderId == orderId &&
                             dtl.IsActive == true &&
                             itm.IsActive == true &&
                             itm.IsShippingReturn == true &&
                             ids.Contains(itm.OmsOrderLineItemsId.ToString())
                             select itm).ToList();

                if (IsNotNull(order))
                {
                    itemstoreturn.ForEach(d =>
                    {
                        var si = order
                                    .FirstOrDefault(s => s.OmsOrderLineItemsId == d.OmsOrderLineItemsId);
                        d.IsAlreadyReturned = si != null ? si.IsShippingReturn.GetValueOrDefault() : false;
                    });
                }
            }
        }

        //Get the ZnodeCheckout model.
        public virtual IZnodeCheckout GetZnodeCheeckoutModel(OrderModel model)
        {
            ZnodeLogging log = new ZnodeLogging();
            UserAddressModel userDetails = SetUserDetails(model.ShoppingCartModel);
            IZnodeCheckout checkout = SetCheckoutData(userDetails, model.ShoppingCartModel, log);
            return checkout;
        }

        //Update order line item details.
        public virtual OrderLineItemStatusListModel UpdateOrderLineItems(OrderLineItemDataListModel orderDetailListModel)
        {

            if (IsNull(orderDetailListModel) || IsNull(orderDetailListModel?.OrderLineItemDetails))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (string.IsNullOrEmpty(orderDetailListModel?.OrderNumber))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.ErrorOrderNumberRequired);

            return UpdateOrderLineItemsDetails(orderDetailListModel);
        }

        //Update order line item details.
        public virtual OrderLineItemStatusListModel UpdateOrderLineItemsDetails(OrderLineItemDataListModel orderDataListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            OrderLineItemStatusListModel listResponse = new OrderLineItemStatusListModel();

            //Get Order Id based on the Order Number
            int? orderId = _omsOrderRepository.Table.Where(x => x.OrderNumber == orderDataListModel.OrderNumber)?.FirstOrDefault()?.OmsOrderId;
            ZnodeLogging.LogMessage("orderId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId });

            if (IsNull(orderId) || orderId == 0)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.OrderNotFound);

            int? orderDetailsId = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == orderId.Value && x.IsActive == true)?.FirstOrDefault()?.OmsOrderDetailsId;

            if (IsNull(orderDetailsId) || orderDetailsId == 0)
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.OrderDetailsNotFound);

            bool status = true;

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeOmsOrderStateEnum.IsOrderLineItemState.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("Where condition in UpdateOrderLineItemsDetails method:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, whereClause);
            List<ZnodeOmsOrderState> orderStateList = _omsOrderStateRepository.GetEntityList(whereClause)?.ToList();

            //get the current state of line item with their ids
            List<Tuple<int, int, int>> ordLineIdStateAndDisplayOrder = GetOrderLineCurrentStateWithDisplayOrder(orderDataListModel.OrderLineItemDetails);

            //Update all the order line item under the specified order number.And returns list of failed line item ids if any.
            foreach (OrderLineItemDataModel orderDetailsModel in orderDataListModel.OrderLineItemDetails)
            {
                OrderLineItemStatusModel itemStatus = new OrderLineItemStatusModel();
                try
                {
                    status = orderId > 0 && orderDetailsModel.OmsOrderLineItemsId > 0 ?
                         UpdateLineItemStatusAndTrackingNumber(orderDetailsId, orderStateList, orderDetailsModel) : false;

                    if (status)
                    {
                        itemStatus.OrderLineItemsId = orderDetailsModel.OmsOrderLineItemsId;
                        itemStatus.Message = Admin_Resources.SuccessUpdate;
                        itemStatus.Status = true;
                        listResponse.OrderLineItemStatusList.Add(itemStatus);

                        AddOrderLineNotes(orderDetailsModel.OmsOrderLineItemsId, orderDetailsId.GetValueOrDefault());
                    }
                }
                catch (Exception ex)
                {
                    itemStatus.OrderLineItemsId = orderDetailsModel.OmsOrderLineItemsId;
                    itemStatus.Message = ex.Message;
                    itemStatus.Status = false;
                    listResponse.OrderLineItemStatusList.Add(itemStatus);
                }
            }

            orderStateList = null;

            if (status)
                UpdateOrderByLineItemState(orderId.Value, orderDataListModel, ordLineIdStateAndDisplayOrder);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listResponse;
        }

        //Update line item status and tracking number
        public virtual bool UpdateLineItemStatusAndTrackingNumber(int? orderDetailsId, List<ZnodeOmsOrderState> orderStateList, OrderLineItemDataModel orderDetailsModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("orderDetailsId while updating line item:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderDetailsId);
            bool status;
            //Get the order state id on the basis of entered state code.
            orderDetailsModel.OrderLineItemStateId = !string.IsNullOrEmpty(orderDetailsModel.OrderLineItemState) ?
                orderStateList?.FirstOrDefault(x => x.OrderStateName.ToLower() == orderDetailsModel.OrderLineItemState.ToLower())?.OmsOrderStateId : orderStateList?.FirstOrDefault(x => x.OmsOrderStateId == orderDetailsModel.OrderLineItemStateId)?.OmsOrderStateId;

            if (IsNull(orderDetailsModel.OrderLineItemStateId) || orderDetailsModel.OrderLineItemStateId == 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidOrderStatus);

            // Update line item status & tracking number.
            status = orderDetailsModel.OrderLineItemStateId > 0 ? UpdateOrderLineItem(orderDetailsModel, orderDetailsId.Value) : false;
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }


        //Get order line item and update.
        public virtual bool UpdateOrderLineItem(OrderLineItemDataModel orderLineItemModel, int omsOrderDetailsId)
        {
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsOrderDetailsId = omsOrderDetailsId });

            bool status = true;

            //Get the Order Line Item Details based on the OrderLineItem & OrderDetailsId
            ZnodeOmsOrderLineItem model = _orderLineItemRepository.Table.FirstOrDefault(x => x.OmsOrderLineItemsId == orderLineItemModel.OmsOrderLineItemsId && x.OmsOrderDetailsId == omsOrderDetailsId);
            if (IsNotNull(model))
            {
                if (!IsPartialShipping(orderLineItemModel))
                {
                    UpdateExistingItem(model, orderLineItemModel);
                }
                else
                {
                    status = ShipItemPartially(model, orderLineItemModel);
                }
            }
            else
            {
                status = false;
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidOrderNo);
            }

            return status;
        }

        //Update order line items.
        public virtual void UpdateOrderLineItem(ZnodeOmsOrderLineItem model, OrderLineItemDataModel orderLineItemDetailModel)
        {
            model.TrackingNumber = (!string.IsNullOrEmpty(Convert.ToString(orderLineItemDetailModel.TrackingNumber))) ? orderLineItemDetailModel.TrackingNumber : model.TrackingNumber;
            model.OrderLineItemStateId = (orderLineItemDetailModel.OrderLineItemStateId > 0) ? orderLineItemDetailModel.OrderLineItemStateId : model.OrderLineItemStateId;
            _orderLineItemRepository.Update(model);
        }

        public virtual void SendEmailNotification(ShoppingCartModel model, bool isUpdateAction, IZnodeCheckout checkout, ZnodeOrderFulfillment order, OrderModel orderModel, bool isEnableBcc, List<EmailTemplateMapperModel> emailTemplateMapperModelList = null)
        {
            string storeReceiptHtml = string.Empty;
            //Attach the store receipt HTML to the order and return.
            if (!string.IsNullOrEmpty(orderModel.ReceiptHtml))
                storeReceiptHtml = orderModel.ReceiptHtml;
            else
                storeReceiptHtml = GetOrderReceipt(order, checkout, model.FeedbackUrl, model.LocaleId, isUpdateAction, out isEnableBcc, model.UserDetails, emailTemplateMapperModelList);
            //Send order amount notification email.
            SendOrderAlertStoreNotification(order.Total, orderModel, storeReceiptHtml, isEnableBcc);
        }

        //Send order alert notification email.
        public virtual void SendOrderAlertStoreNotification(decimal orderAmount, OrderModel orderModel, string storeReceiptHtml, bool isEnableBcc)
        {
            if (IsNotNull(orderModel.PortalId))
            {
                //Get portal information by portalId
                ZnodePortal portal = _portalRepository.GetById(orderModel.PortalId);
                if (IsNotNull(portal?.OrderAmount))
                    if (orderAmount >= portal.OrderAmount)
                        if (!string.IsNullOrEmpty(storeReceiptHtml))
                            SendOrderReceipt(orderModel.PortalId, portal.Email, $"{Admin_Resources.TitleOrderReceipt} - {orderModel.OrderNumber}", null, null, storeReceiptHtml, isEnableBcc);
            }
        }

        //to generate order receipt
        public virtual string GetOrderReceipt(ZnodeOrderFulfillment order, IZnodeCheckout checkout, string feedbackUrl, int localeId, bool isUpdate = false, string emailTemplate = "OrderReceipt")
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            foreach (OrderLineItemModel item in order.OrderLineItems)
            {
                if (item.PersonaliseValueList != null)
                    item.PersonaliseValueList.Remove("AllocatedLineItems");

                if (item.PersonaliseValuesDetail != null)
                    item.PersonaliseValuesDetail.RemoveAll(pv => pv.PersonalizeCode == "AllocatedLineItems");
            }

            IZnodeOrderReceipt receipt = GetOrderReceiptInstance(order, checkout.ShoppingCart);

            //Method to get Email Template.
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.OrderReceipt, (order.PortalId > 0) ? order.PortalId : PortalId, localeId);
            if (HelperUtility.IsNotNull(emailTemplateMapperModel))
            {
                string receiptContent = ShowOrderAdditionalDetails(emailTemplateMapperModel.Descriptions);
                return EmailTemplateHelper.ReplaceTemplateTokens(receipt.GetOrderReceiptHtml(receiptContent));
            }
            return string.Empty;
        }

        //to check Order State by stateId and state name
        public virtual bool IsOrderStateExist(int stateId, string stateName)
        {
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { stateId = stateId, stateName = stateName });

            ZnodeOmsOrderState state = _omsOrderStateRepository.Table.Where(x => x.OmsOrderStateId == stateId && x.OrderStateName.ToLower() == stateName.ToLower())?.FirstOrDefault() ?? null;
            return IsNull(state) ? false : state?.OmsOrderStateId > 0;
        }
        //to check whether the line item process for partial shipping
        public virtual bool IsPartialShipping(OrderLineItemDataModel model)
        => IsOrderStateExist(model.OrderLineItemStateId.GetValueOrDefault(), ZnodeOrderStatusEnum.SHIPPED.ToString()) && IsNotNull(model.Quantity) && (model?.Quantity > 0);

        //to ship item partially by Line item id and quantity
        public virtual bool ShipItemPartially(ZnodeOmsOrderLineItem model, OrderLineItemDataModel lineItemModel)
        {
            bool isSuccess = true;
            if (IsNotNull(model))
            {
                //if total quantity greater than partial shipping quantity
                if (model.Quantity > lineItemModel.Quantity)
                {
                    isSuccess = AddPartialShippedItems(lineItemModel);
                }
                else
                {
                    UpdateExistingItem(model, lineItemModel);
                }
            }
            return isSuccess;
        }

        //to update order state by line item state
        public virtual void UpdateOrderByLineItemState(int orderId, OrderLineItemDataListModel model, List<Tuple<int, int, int>> ordLineIdStateAndDisplayOrder)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId && x.IsActive);
            if (IsOrderStateUpdate(orderId))
            {
                int lineItemStatusId = model?.OrderLineItemDetails?.Select(x => x.OrderLineItemStateId)?.FirstOrDefault() ?? 0;
                if (lineItemStatusId > 0)
                {
                    bool orderStatus = _omsOrderStateRepository.Table.FirstOrDefault(x => x.OmsOrderStateId == lineItemStatusId)?.IsOrderState ?? false;
                    if (orderStatus)
                    {
                        if (IsNotNull(order) && order.OmsOrderStateId != lineItemStatusId)
                        {
                            order.OmsOrderStateId = lineItemStatusId;
                            _orderDetailsRepository.Update(order);

                            AddOrderNotes(orderId, order?.OmsOrderDetailsId);
                        }
                    }
                    else
                    {
                        //Update order state to InProgress
                        UpdateOrderStateToInProgress(orderId);
                        AddOrderNotes(orderId, order?.OmsOrderDetailsId);
                    }
                }
            }
            else
            {
                foreach (var lstItem in ordLineIdStateAndDisplayOrder)
                {
                    var lineItm = _orderLineItemRepository.GetById(lstItem.Item1);
                    var stateDisplayOrder = _omsOrderStateRepository.GetById(lineItm.OrderLineItemStateId.GetValueOrDefault())?.DisplayOrder;

                    if (stateDisplayOrder < lstItem.Item3)
                    {
                        int orderLineStateId = lineItm.OrderLineItemStateId.GetValueOrDefault();
                        bool orderStatus = _omsOrderStateRepository.Table.FirstOrDefault(x => x.OmsOrderStateId == orderLineStateId)?.IsOrderState ?? false;
                        if (orderStatus)
                        {
                            if (IsNotNull(order))
                            {
                                order.OmsOrderStateId = lineItm.OrderLineItemStateId.GetValueOrDefault();
                                _orderDetailsRepository.Update(order);
                                AddOrderNotes(orderId, order?.OmsOrderDetailsId);
                            }
                        }
                        else
                        {
                            UpdateOrderStateToInProgress(orderId);
                            AddOrderNotes(orderId, order?.OmsOrderDetailsId);
                        }
                    }
                    else if (stateDisplayOrder > lstItem.Item3)
                    {
                        UpdateOrderStateToInProgress(orderId);
                        AddOrderNotes(orderId, order?.OmsOrderDetailsId);
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //to check all items in order are updated 
        public virtual bool IsOrderStateUpdate(int orderId)
        {
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId });

            bool isSuccess = false;

            List<int?> lineStateId = (from orderDetails in _orderDetailsRepository.Table
                                      join lineItems in _orderLineItemRepository.Table on orderDetails.OmsOrderDetailsId equals lineItems.OmsOrderDetailsId
                                      where orderDetails.IsActive && orderDetails.OmsOrderId == orderId
                                      select lineItems.OrderLineItemStateId)?.Distinct()?.ToList() ?? null;

            if (lineStateId?.Count > 0)
            {
                return lineStateId?.Count == 1;
            }
            return isSuccess;
        }

        //to update existing item details
        public virtual void UpdateExistingItem(ZnodeOmsOrderLineItem model, OrderLineItemDataModel orderLineItemModel)
        {
            UpdateOrderLineItem(model, orderLineItemModel);
            if (IsNotNull(model.ParentOmsOrderLineItemsId))
            {
                //Get the Parent Order Line Item Details.
                model = _orderLineItemRepository.Table.FirstOrDefault(x => x.OmsOrderLineItemsId == model.ParentOmsOrderLineItemsId);
                if (IsNotNull(model))
                    UpdateOrderLineItem(model, orderLineItemModel);
            }
        }

        //to add partial shipped items to database
        public virtual bool AddPartialShippedItems(OrderLineItemDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int status = 0;
            IZnodeViewRepository<SEODetailsModel> objStoredProc = new ZnodeViewRepository<SEODetailsModel>();
            objStoredProc.SetParameter("@LineItemId", model.OmsOrderLineItemsId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LineItemStateId", model.OrderLineItemStateId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Quantity", model.Quantity, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@TrackingNumber", string.IsNullOrEmpty(model.TrackingNumber) ? string.Empty : model.TrackingNumber, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.ExecuteStoredProcedureList("Znode_InsertPartialShippedItems @LineItemId,@LineItemStateId,@Quantity,@TrackingNumber,@status OUT", 4, out status);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status == 1;
        }

        //Update Order Paypal Payment TransactionId
        public virtual bool UpdateOrderTransactionId(int orderId, string transactionId, int createdBy = 0, int modifiedBy = 0)
        {
            ZnodeLogging.LogMessage("Input parameters orderId and transactionId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, transactionId = transactionId });

            if (orderId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.OrderIdLessThanOne);

            if (string.IsNullOrEmpty(transactionId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorTransactionIdNull);

            ZnodeOmsOrderDetail orderDetails = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == orderId && x.IsActive)?.FirstOrDefault();
            ZnodeLogging.LogMessage("Order details in UpdateOrderTransactionId method:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderDetails);
            if (IsNotNull(orderDetails))
            {
                orderDetails.TransactionId = transactionId;
                orderDetails.CreatedBy = createdBy;
                orderDetails.ModifiedBy = modifiedBy;
                return _orderDetailsRepository.Update(orderDetails);
            }
            return false;
        }


        public virtual bool ReorderCompleteOrder(int orderId, int portalId, int userId = 0, int omsOrderLineItemsId = 0)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, portalId = portalId, omsOrderLineItemsId = omsOrderLineItemsId });

            if (orderId < 1 && omsOrderLineItemsId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.OrderIdLessThanOne);

            int savedCartId = GetSavedCartId(portalId, userId);

            int status = 0;
            IZnodeViewRepository<SEODetailsModel> objStoredProc = new ZnodeViewRepository<SEODetailsModel>();
            objStoredProc.SetParameter("@OmsOrderId", orderId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@OmsSavedCartId", savedCartId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@OmsOrderLineItemsId", omsOrderLineItemsId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.ExecuteStoredProcedureList("Znode_InsertSaveCartLineItemsForReOrder @OmsOrderId,@OmsSavedCartId,@UserId,@OmsOrderLineItemsId,@status OUT", 4, out status);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status == 1;
        }

        public virtual AddressListModel GetAddressListWithShipment(int orderId, int userId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId});

            if (orderId < 1 && userId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.OrderIdLessThanOne);

            IZnodeViewRepository<AddressModel> objStoredProc = new ZnodeViewRepository<AddressModel>();
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@OrderId", orderId, ParameterDirection.Input, DbType.Int32);

            IList<AddressModel> addressList = objStoredProc.ExecuteStoredProcedureList("Znode_GetAddressListWithShipment @UserId,@OrderId");
            AddressListModel addressListModel = new AddressListModel() { AddressList = addressList?.ToList() };

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return addressListModel;
        }

        //to get savedcartId by cookieMappingId
        private int GetSavedCartId(int portalId = 0, int? userId = 0)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int cookieMappingId = Convert.ToInt32(_cookieMappingRepository.Table.FirstOrDefault(x => x.UserId == userId)?.OmsCookieMappingId);
            ZnodeLogging.LogMessage("Input parameters PortalId and UserId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, portalId = portalId });

            if (cookieMappingId > 0)
            {
                IZnodeRepository<ZnodeOmsSavedCart> _savedCartRepository = new ZnodeRepository<ZnodeOmsSavedCart>();
                ZnodeOmsSavedCart savedCart = _savedCartRepository.Table.FirstOrDefault(x => x.OmsCookieMappingId == cookieMappingId);
                if (HelperUtility.IsNull(savedCart))
                {
                    ZnodeOmsCookieMapping cookieMapping = _cookieMappingRepository.Table.FirstOrDefault(x => x.OmsCookieMappingId == cookieMappingId);
                    if (HelperUtility.IsNull(cookieMapping) || cookieMapping.OmsCookieMappingId <= 0)
                    {
                        cookieMappingId = CreateCookieMappingId(userId, portalId);
                    }
                    //savedcartId not exist for cookieMappingId then create new.
                    ZnodeOmsSavedCart newCart = _savedCartRepository.Insert(new ZnodeOmsSavedCart()
                    {
                        OmsCookieMappingId = cookieMappingId,
                        CreatedDate = HelperUtility.GetDateTime(),
                        ModifiedDate = HelperUtility.GetDateTime()
                    });
                    return newCart.OmsSavedCartId;
                }
                return Convert.ToInt32(savedCart.OmsSavedCartId);
            }
            return 0;
        }

        //create new cookiemappingid for cart
        protected virtual int CreateCookieMappingId(int? userId, int portalId)
        {
            ZnodeLogging.LogMessage("Input parameters userId and portalId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, portalId = portalId });
            ZnodeOmsCookieMapping cookieMapping = _cookieMappingRepository.Insert(new ZnodeOmsCookieMapping()
            {
                UserId = userId == 0 ? null : userId,
                CreatedDate = HelperUtility.GetDateTime(),
                ModifiedDate = HelperUtility.GetDateTime(),
                PortalId = portalId
            });
            return Convert.ToInt32(cookieMapping?.OmsCookieMappingId);
        }

        // Convert the quote to order and send email to user
        public virtual OrderModel ConvertToOrder(AccountQuoteModel accountQuoteModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int isUpdated = 0;
            if (IsNull(accountQuoteModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (accountQuoteModel.OmsQuoteId <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorQuoteIdGreaterThanZero);
            int approvedOrderId = _omsOrderStateRepository.Table.FirstOrDefault(x => x.OrderStateName == ZnodeOrderStatusEnum.APPROVED.ToString()).OmsOrderStateId;
            IAccountQuoteService _accountQuoteService = GetService<IAccountQuoteService>();
            IList<AccountQuoteModel> accountQuoteList = _accountQuoteService.UpdateQuoteStatus(approvedOrderId, accountQuoteModel.OmsQuoteId.ToString(), "Ordered,Draft", out isUpdated);

            string quoteStatus = accountQuoteList?.Select(x => x.Status)?.FirstOrDefault();

            accountQuoteModel.IsUpdated = isUpdated == 1;

            InsertApproverComments(accountQuoteModel);

            if (accountQuoteModel.IsUpdated && (string.Equals(quoteStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(quoteStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(quoteStatus, ZnodeConstant.IN_REVIEW, StringComparison.CurrentCultureIgnoreCase)))
            {
                _accountQuoteService.SendQuoteStatusEmailToUser(quoteStatus, Convert.ToInt32(accountQuoteList?.Select(x => x.PortalId).FirstOrDefault()), accountQuoteList?.ToList(), accountQuoteModel.LocaleId);
            }

            ZnodeLogging.LogMessage("Input parameter OmsQuoteId for getting quote details :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { accountQuoteModel?.OmsQuoteId });
            ZnodeOmsQuote quoteDetails = _znodeOmsQuote.GetById(accountQuoteModel.OmsQuoteId);

            if (IsNull(quoteDetails))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.DetailsNotFound);

            ShoppingCartModel model = GetShoppingCartModel(quoteDetails, accountQuoteModel);

            //In case of pending order live calculations needs to done.
            RecalculatePendingOrderDetails(model);

            if (IsAllowedTerritories(model))
                throw new ZnodeException(ErrorCodes.AllowedTerritories, Admin_Resources.AllowedTerritoriesError);

            SubmitOrderModel submitOrderModel = new SubmitOrderModel();

            ParameterModel portalId = new ParameterModel() { Ids = Convert.ToString(model.PortalId) };

            //Get generated unique order number on basis of current date.
            submitOrderModel.OrderNumber = GenerateOrderNumber(submitOrderModel, portalId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if(model.ShoppingCartItems.Count > 0)
            {
                return SaveOrder(model, submitOrderModel);
            }
            else
            {
                return new OrderModel();
            }

        }

        public virtual void InitializeERPConnectorForCreateUpdateOrder(OrderModel orderModel)
        {
            ERPInitializer<OrderModel> _erpInc = new ERPInitializer<OrderModel>(orderModel, "CreateOrder");
        }

        //Map Calculate Properties Or Calculate.
        protected virtual void CalculateCheckoutCart(IZnodeCheckout checkout, ShoppingCartModel model)
        {

            try
            {
                if (!model.SkipCalculations || model.OmsOrderId > 0)
                {
                    checkout.ShoppingCart.Calculate(model.ProfileId, model.IsCalculateTaxAndShipping, model.IsCalculatePromotionAndCoupon);
                }
                else
                {
                    checkout.ShoppingCart.Shipping.ShippingDiscountType = model.Shipping.ShippingDiscountType.GetValueOrDefault();
                    checkout.ShoppingCart.CSRDiscountAmount  = model.CSRDiscountAmount;
                    checkout.ShoppingCart.CSRDiscountApplied  = model.CSRDiscountApplied;
                    checkout.ShoppingCart.CSRDiscountDescription  = model.CSRDiscountDescription;
                    checkout.ShoppingCart.CSRDiscountMessage  = model.CSRDiscountMessage;
                    checkout.ShoppingCart.CustomShippingCost = model.CustomShippingCost;
                    checkout.ShoppingCart.Discount = model.Discount;
                    checkout.ShoppingCart.CustomTaxCost = model.CustomTaxCost;
                    checkout.ShoppingCart.GiftCardBalance = model.GiftCardBalance;
                    checkout.ShoppingCart.SalesTax = model.SalesTax;
                    checkout.ShoppingCart.ShippingCost = model.ShippingCost;
                    checkout.ShoppingCart.ShippingDifference = model.ShippingDifference;
                    checkout.ShoppingCart.TaxCost = model.TaxCost;
                    checkout.ShoppingCart.SubTotal = model.SubTotal.GetValueOrDefault();
                    checkout.ShoppingCart.TaxRate = model.TaxRate;
                    checkout.ShoppingCart.Token = model.Token;
                    checkout.ShoppingCart.Total = model.Total.GetValueOrDefault();
                    checkout.ShoppingCart.OrderTotalWithoutVoucher = model.OrderTotalWithoutVoucher.GetValueOrDefault();
                    checkout.ShoppingCart.TotalAdditionalCost = model.TotalAdditionalCost.GetValueOrDefault();
                    checkout.ShoppingCart.ImportDuty = model.ImportDuty.GetValueOrDefault();
                    checkout.ShoppingCart.Shipping.ShippingHandlingCharge = model.Shipping.ShippingHandlingCharge;
                    checkout.ShoppingCart.Shipping.ShippingDiscount = model.Shipping.ShippingDiscount;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in CalculateCheckoutCart method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get failed order transaction list.
        public virtual FailedOrderTransactionListModel FailedOrderTransactionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Replace sort key name.
            if (IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            //Add date time value in filter collection against filter column name Order date.
            filters = ServiceHelper.AddDateTimeValueInFilterByName(filters, Constants.FilterKeys.OrderDate);

            ReplaceFilterKeys(ref filters);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<FailedOrderTransactionModel> objStoredProc = new ZnodeViewRepository<FailedOrderTransactionModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<FailedOrderTransactionModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_OmsFailedOrderPaymentsList @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);

            ZnodeLogging.LogMessage("Failed Order Payment list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, list?.Count);
            FailedOrderTransactionListModel orderListModel = new FailedOrderTransactionListModel() { FailedOrderTransactionModel = list.Count() > 0 ? list?.ToList() : new List<FailedOrderTransactionModel>() };

            orderListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderListModel;
        }

        //Save Order Payment Detail
        public virtual bool SaveOrderPaymentDetail(OrderPaymentDataModel orderPaymentModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                if (IsNull(orderPaymentModel))
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorOrderPaymentModelNull);

                ZnodeOrderFulfillment order = new ZnodeOrderFulfillment();
                return order.InsertOrderPaymentData(orderPaymentModel, orderPaymentModel.TransactionDate);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SaveOrderPaymentDetail method for order " + orderPaymentModel?.OmsOrderId + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map payment history.
        public virtual void GetPaymentHistory(OrderModel orderModel)
        {
            try
            {
                //SP call to revert order inventory, update this code once dba provide the sp.
                IZnodeViewRepository<PaymentHistoryModel> objStoredProc = new ZnodeViewRepository<PaymentHistoryModel>();
                objStoredProc.SetParameter("@OrderId", orderModel.OmsOrderId, ParameterDirection.Input, DbType.Int32);
                IList<PaymentHistoryModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPaymentHistory @OrderId");
                ZnodeLogging.LogMessage("Payment history list count and OmsOrderId to get payment history: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PaymentHistoryListCount = list?.Count, OmsOrderId = orderModel?.OmsOrderId });
                orderModel.PaymentHistoryList.PaymentHistoryList = list?.ToList();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetPaymentHistory method for order " + orderModel?.OmsOrderId + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        //Get ProfileId by userId
        protected int? GetProfileIdByUserId(int userId)
        {
            IZnodeRepository<ZnodeUserProfile> _userProfile = new ZnodeRepository<ZnodeUserProfile>();

            int? profileId = _userProfile.Table.Where(x => x.UserId == userId && x.IsDefault == true)?.FirstOrDefault()?.ProfileId;
            return profileId;
        }

        //Add the order notes in case of order status updated by API.
        protected virtual void AddOrderNotes(int orderId, int? omsOrderDetailsId)
        {
            ZnodeLogging.LogMessage("Input parameters OrderId and omsOrderDetailsId :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderId, omsOrderDetailsId });
            if (omsOrderDetailsId.GetValueOrDefault() > 0)
            {
                ZnodeOmsNote ordNote = new ZnodeOmsNote();
                ordNote.OmsOrderDetailsId = omsOrderDetailsId;
                ordNote.Notes = string.Format(Admin_Resources.SuccessOrderStatusUpdate, orderId);
                _omsNoteRepository.Insert(ordNote);
            }
        }

        //Add the order notes in case of line item updated by API.
        protected virtual void AddOrderLineNotes(int omsOrderLineItemsId, int orderDetailsId)
        {
            ZnodeLogging.LogMessage("Input parameters omsOrderLineItemsId and orderDetailsId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { omsOrderLineItemsId, orderDetailsId });
            ZnodeOmsNote lineNote = new ZnodeOmsNote();
            lineNote.OmsOrderDetailsId = orderDetailsId;
            lineNote.Notes = string.Format(Admin_Resources.SuccessOrderLineItemStatusUpdate, omsOrderLineItemsId);
            _omsNoteRepository.Insert(lineNote);
        }

        //Updates the order status to In Progress if available else keep the status as is
        protected virtual void UpdateOrderStateToInProgress(int orderId)
        {
            ZnodeLogging.LogMessage("OrderId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderId);
            ZnodeOmsOrderDetail order = _orderDetailsRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId && x.IsActive);
            if (IsNotNull(order))
            {
                int? stateId = _omsOrderStateRepository.Table.FirstOrDefault(x => x.OrderStateName.ToUpper() == InProgressOrderState)?.OmsOrderStateId;
                if (IsNotNull(stateId) && stateId.GetValueOrDefault() > 0)
                {
                    order.OmsOrderStateId = stateId.GetValueOrDefault();
                    _orderDetailsRepository.Update(order);
                }
            }
        }

        //Add the current order state and other things in tuple.
        protected virtual List<Tuple<int, int, int>> GetOrderLineCurrentStateWithDisplayOrder(List<OrderLineItemDataModel> orderLineItemDetails)
        {
            List<Tuple<int, int, int>> ordLineIdStateAndDisplayOrderList = new List<Tuple<int, int, int>>();

            foreach (OrderLineItemDataModel lineItem in orderLineItemDetails)
            {
                var lineItm = _orderLineItemRepository.GetById(lineItem.OmsOrderLineItemsId);
                if (IsNull(lineItm))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorInvalidOrderLineItemId);

                var stateDisplayOrder = _omsOrderStateRepository.GetById(lineItm.OrderLineItemStateId.GetValueOrDefault())?.DisplayOrder;
                ordLineIdStateAndDisplayOrderList.Add(new Tuple<int, int, int>(lineItm.OmsOrderLineItemsId, lineItm.OrderLineItemStateId.GetValueOrDefault(), stateDisplayOrder.GetValueOrDefault()));
            }
            return ordLineIdStateAndDisplayOrderList;
        }

        //Get order receipt instance.
        protected virtual IZnodeOrderReceipt GetOrderReceiptInstance(OrderModel order)
        {
            var objZnodeOrderReceipt = GetService<IZnodeOrderReceipt>();
            objZnodeOrderReceipt.FromApi = true;
            objZnodeOrderReceipt.OrderModel = order;
            return objZnodeOrderReceipt;
        }

        //Get order receipt instance.
        protected virtual IZnodeOrderReceipt GetOrderReceiptInstance(ZnodeOrderFulfillment order, IZnodePortalCart shoppingCart)
        {
            return GetService<IZnodeOrderReceipt>(new ZnodeNamedParameter("order", order), new ZnodeNamedParameter("shoppingCart", shoppingCart));
        }

        //Set Order Line Items for all the product types
        protected virtual void SetOrderLineItems(OrderModel orderModel, string[] omsOrderLineArray)
        {
            orderModel.OrderLineItems = CreateSingleOrderLineItem(orderModel, true).Where(lineItemId => omsOrderLineArray.Contains(lineItemId.OmsOrderLineItemsId.ToString()) ||
                                          omsOrderLineArray.Contains(lineItemId.ParentOmsOrderLineItemsId.ToString())).ToList();

            int? itemsCount = orderModel.OrderLineItems.Where(e => e.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group)
                              || e.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Configurable)
                              || e.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Simple))
                              .ToList()?.Count;
            if (itemsCount > 0)
            {
                orderModel.OrderLineItems = orderModel.OrderLineItems.Where(e => e.ParentOmsOrderLineItemsId != null).ToList();
            }
        }

        //Remove shipping history for return
        protected virtual void RemoveShippingHistoryForReturn(Dictionary<string, OrderLineItemHistoryModel> orderLineItemHistory)
        {
            if (IsNotNull(orderLineItemHistory) && orderLineItemHistory.Count > 0)
            {
                List<string> returnShippingHistoryListKey = new List<string>();
                foreach (var returnItem in orderLineItemHistory.Where(x => string.Equals(x.Value.OrderUpdatedStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    string returnShippingHistoryKey = orderLineItemHistory.FirstOrDefault(x => string.Equals(x.Key, "Shipping_" + returnItem.Key, StringComparison.InvariantCultureIgnoreCase)).Key;
                    if (!string.IsNullOrEmpty(returnShippingHistoryKey))
                        returnShippingHistoryListKey.Add(returnShippingHistoryKey);
                }
                if (IsNotNull(returnShippingHistoryListKey) && returnShippingHistoryListKey.Count > 0)
                    foreach (var key in returnShippingHistoryListKey)
                        orderLineItemHistory.Remove(key);
            }
        }
       
        //Send Voucher usages detail email to customer.
        public virtual void SendEmailForVoucherUsages(OrderModel order, List<VoucherModel> vouchers, int localeId, List<EmailTemplateMapperModel> emailTemplateMapperModelList = null)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNull(order.StoreName))
                order.StoreName = _portalRepository.Table?.FirstOrDefault(x => x.PortalId == order.PortalId)?.StoreName;

            if (vouchers?.Count > 0)
            {
                EmailTemplateMapperModel emailTemplateMapperModel = GetOrderEmailTemplateModel(emailTemplateMapperModelList, ZnodeConstant.RemainingVoucherBalance, order.PortalId, localeId);

                IGiftCardService giftCardService = GetService<IGiftCardService>();
                foreach (VoucherModel voucher in vouchers)
                {
                    if (voucher.IsVoucherApplied && voucher.IsVoucherValid)
                    {
                        if (voucher.UserId > 0)
                            SendVoucherUsagesEmail(voucher, order, localeId, emailTemplateMapperModel);
                        else
                        {
                            GiftCardModel giftCard = orderHelper.GetVoucherByCardNumber(voucher.VoucherNumber);
                            if (giftCard?.GiftCardId > 0)
                            {
                                giftCard.ModifiedDate = GetDate();
                                giftCard.TransactionAmount = voucher.VoucherAmountUsed;
                                giftCard.SendMail = true;
                                giftCardService?.UpdateGiftCard(giftCard);
                                voucher.UserId = order.UserId;
                                SendVoucherUsagesEmail(voucher, order, localeId, emailTemplateMapperModel);
                            }
                        }
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //gets comma separated list of required email template codes
        protected virtual string GetRequiredOrderEmailTemplates()
        {
            List<string> emailCode = new List<string>(new string[] {
                ZnodeConstant.OrderReceipt, ZnodeConstant.LowInventoryOrderNotification, ZnodeConstant.VendorReceipt, ZnodeConstant.ResendOrderReceipt, ZnodeConstant.RemainingVoucherBalance});

            return string.Join(",", emailCode);
        }

        //get required email template
        protected virtual EmailTemplateMapperModel GetOrderEmailTemplateModel(List<EmailTemplateMapperModel> emailTemplateListModel, string code, int portalID, int localeId)
        {
            EmailTemplateMapperModel emailTemplateMapperModel = new EmailTemplateMapperModel();
            if (IsNotNull(emailTemplateListModel) && emailTemplateListModel?.Count > 0)
            {
                emailTemplateMapperModel = emailTemplateListModel.FirstOrDefault(x => string.Equals(x.Code, code, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                emailTemplateMapperModel = GetEmailTemplateByCode(code, (portalID > 0) ? portalID : PortalId, localeId);
            }
            return emailTemplateMapperModel;
        }

        //Send Voucher usages Email.
        protected virtual void SendVoucherUsagesEmail(VoucherModel voucher, OrderModel order, int localeId, EmailTemplateMapperModel emailTemplateMapperModel)
        {
            string messageText = string.Empty;
            if (IsNotNull(emailTemplateMapperModel) && IsNotNull(voucher))
            {
                ZnodeUser userDetails = GetUser(voucher.UserId);
                string subject = $"{emailTemplateMapperModel?.Subject}";
                subject = ReplaceTokenWithMessageText(ZnodeConstant.VoucherName, $" {voucher.VoucherName.ToString()}", subject);
                messageText = emailTemplateMapperModel.Descriptions;
                int portalId = Convert.ToInt32(voucher?.PortalId);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerName, $" {userDetails?.FirstName} {userDetails?.LastName}", messageText);
                messageText = ReplaceTokenWithCurrencyText(ZnodeConstant.VoucherAmountUsed, $"{ ZnodeCurrencyManager.FormatPriceWithCurrency(voucher.VoucherAmountUsed, voucher.CultureCode)}", messageText, voucher.CultureCode);
                messageText = ReplaceTokenWithCurrencyText(ZnodeConstant.RemainingAmount, $"{ ZnodeCurrencyManager.FormatPriceWithCurrency(voucher.VoucherBalance, voucher.CultureCode)}", messageText, voucher.CultureCode);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.VoucherName, $" {voucher.VoucherName.ToString()}", messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.GiftCardNumber, voucher.VoucherNumber.ToString(), messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.OrderNumber, order?.OrderNumber.ToString(), messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.StoreName, $" {order?.StoreName}", messageText);
                ZnodeEmail.SendEmail(portalId, userDetails?.Email, ZnodeConfigManager.SiteConfig.AdminEmail, string.Empty, subject, messageText, true);
            }
        }

        //Bind return amount 
        protected virtual decimal BindReturnAmount(decimal returnAmount, KeyValuePair<string, OrderLineItemHistoryModel> returnItem) =>
             string.IsNullOrEmpty(returnItem.Value.ReturnShippingAmount) ? returnAmount + returnItem.Value.SubTotal + returnItem.Value.TaxCost + returnItem.Value.ImportDuty : returnAmount + returnItem.Value.SubTotal + returnItem.Value.TaxCost + returnItem.Value.ImportDuty + Convert.ToDecimal(returnItem.Value.ReturnShippingAmount);

        //Generate history for order line item history who's shipping is updated
        protected virtual string GenerateOrderLineItemShippingHistory(string orderLineShippingHistory, OrderLineItemHistoryModel orderLineItemHistory, List<OrderLineItemModel> oldValue, string cultureCode)
        {
            if (IsNotNull(orderLineItemHistory))
            {
                string formattedOriginalOrderLineItemShippingCost = ZnodeCurrencyManager.FormatPriceWithCurrency(orderLineItemHistory.OriginalOrderLineItemShippingCost, cultureCode, string.Empty);
                string formattedNewOrderLineItemShippingCost = ZnodeCurrencyManager.FormatPriceWithCurrency(orderLineItemHistory.NewOrderLineItemShippingCost, cultureCode, string.Empty);

                orderLineShippingHistory = GetConsolidatedHistoryMessage(string.Format(Admin_Resources.LineItemShippingChange, orderLineItemHistory.SKU, orderLineItemHistory.ProductName, formattedOriginalOrderLineItemShippingCost, formattedNewOrderLineItemShippingCost), orderLineShippingHistory);
            }
            return orderLineShippingHistory;
        }

        //To save edit shipping line item history in database
        protected virtual void CreateEditShippingOrderHistory(OrderModel orderModel, string orderLineShippingHistory)
        {
            if (!string.IsNullOrEmpty(orderLineShippingHistory))
            {
                decimal orderShippingCost = Convert.ToDecimal(orderModel?.ShoppingCartModel?.ShippingCost);
                decimal orderOldShippingCost = Convert.ToDecimal(orderModel?.ShippingCost);
                decimal shippingAmountDifference = orderOldShippingCost - orderShippingCost;

                CreateOrderHistory(new OrderHistoryModel { OmsOrderDetailsId = orderModel.OmsOrderDetailsId, Message = orderLineShippingHistory, OmsNotesId = 0, OrderAmount = shippingAmountDifference, CreatedBy = orderModel.CreatedBy, ModifiedBy = orderModel.ModifiedBy });
            }
        }

        //Returns subtotal discount for order line item
        protected virtual decimal GetLineItemDiscountAmount(OrderLineItemModel omsOrderLineItem, ShoppingCartItemModel shoppingCartItem)
        {
            return omsOrderLineItem.Quantity * Convert.ToDecimal(shoppingCartItem?.PerQuantityOrderLevelDiscountOnLineItem + shoppingCartItem?.PerQuantityLineItemDiscount);
        }

        //Returns CSR discount for order line item
        protected virtual decimal GetLineItemCSRDiscount(OrderLineItemModel omsOrderLineItem, ShoppingCartItemModel shoppingCartItem)
        {
            return omsOrderLineItem.Quantity * Convert.ToDecimal(shoppingCartItem?.PerQuantityCSRDiscount);
        }

        //Returns shipping discount for order line item
        protected virtual decimal GetLineItemShippingDiscount(OrderLineItemModel omsOrderLineItem, ShoppingCartItemModel shoppingCartItem)
        {
            return omsOrderLineItem.Quantity * Convert.ToDecimal(shoppingCartItem?.PerQuantityShippingDiscount);
        }
        #endregion

        #region Private Methods

        //to Sett billing address
        private void SetOrderBillingAddress(OrderModel orderModel)
        {
            if (IsNotNull(orderModel))
            {

                if (IsNotNull(orderModel.BillingAddress))
                {
                    orderModel.BillingAddress.StateCode = new ZnodeRepository<ZnodeState>().Table.FirstOrDefault(x => x.StateName == orderModel.BillingAddress.StateName)?.StateCode;
                    ZnodeAddress billing = _addressRepository.Table.FirstOrDefault(x => x.AddressId == orderModel.BillingAddress.AddressId);
                    orderModel.BillingAddress.CompanyName = billing?.CompanyName;
                    //Sets the external id for billing address.
                    orderModel.BillingAddress.ExternalId = billing?.ExternalId;
                }
            }
        }

        //Get shopping cart model using quote details
        protected virtual ShoppingCartModel GetShoppingCartModel(ZnodeOmsQuote quoteDetails, AccountQuoteModel accountQuoteModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Bind cart parameter model.
            CartParameterModel cartParameterModel = BindCartParameterModel(quoteDetails, accountQuoteModel);
            ShoppingCartModel model = GetShoppingCartDetails(quoteDetails, accountQuoteModel, cartParameterModel);
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Get Shopping Cart Details
        public virtual ShoppingCartModel GetShoppingCartDetails(ZnodeOmsQuote quoteDetails, AccountQuoteModel accountQuoteModel, CartParameterModel cartParameterModel)
        {
            IShoppingCartService _shoppingCartService = GetService<IShoppingCartService>();
            ShoppingCartModel model = _shoppingCartService.GetShoppingCartDetails(cartParameterModel);
            model.UserDetails = _userService.GetUserById(quoteDetails.UserId, null);
            model.ProfileId = string.IsNullOrEmpty(quoteDetails.QuoteTypeCode) ? GetProfileIdByUserId(quoteDetails.UserId) ?? 0 : GetProfileId();

            AddressModel shippingAddress;
            AddressModel billingAddress;

            //Bind shipping billing address.
            BindShippingBillingAddress(quoteDetails, out shippingAddress, out billingAddress);

            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodePaymentSettingEnum.ZnodePaymentType.ToString().ToLower(), ZnodePaymentSettingEnum.ZnodePaymentType.ToString());

            //Bind shopping cart model.
            BindShoppingCartModel(quoteDetails, accountQuoteModel, model, shippingAddress, billingAddress);
            PaymentSettingModel paymentSetting = _paymentSettingService.GetPaymentSetting(quoteDetails.PaymentSettingId.GetValueOrDefault(), expands, model.PortalId);
            model.Payment = new PaymentModel { BillingAddress = billingAddress, ShippingAddress = shippingAddress, PaymentSetting = paymentSetting, PaymentDisplayName = paymentSetting.PaymentDisplayName };
            return model;
        }

        protected virtual int GetOrderStateIdByName(string orderStateName = "")
            => _omsOrderStateRepository.Table.FirstOrDefault(x => x.OrderStateName.ToUpper() == orderStateName.ToUpper())?.OmsOrderStateId ?? 0;

        //get publish catalog id
        private int GetPublishCatalogId(int portalId)
        {
            ZnodeLogging.LogMessage("Get publish catalogId by using portal Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, portalId);
            int? portalCatalogId = _portalCatalogRepository.Table.Where(x => x.PortalId == portalId)?.FirstOrDefault()?.PublishCatalogId;
            if (portalCatalogId > 0)
                portalCatalogId = portalCatalogId.GetValueOrDefault();

            return portalCatalogId.GetValueOrDefault();
        }

        //Set Expand parameters for Order List.
        private void SetOrderListWithExpands(DataSet dataSet, List<OrderModel> entities, PageListModel pageListModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (!HelperUtility.IsNull(dataSet) && dataSet.Tables.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];

                DateTime datetimeStart = DateTime.UtcNow;
                //Convert Dataset to entity
                foreach (DataRow row in dataTable.Rows)
                    entities.Add(JsonConvert.DeserializeObject<OrderModel>(Convert.ToString(row["OrderJSON"])));
                ZnodeLogging.LogMessage("Order entities list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderEntitiesCount = entities });
                foreach (OrderModel orderModel in entities)
                {
                    if (IsNotNull(orderModel?.OrderLineItems) && orderModel.OrderLineItems.Count > 0)
                    {
                        orderModel.OrderItem = orderModel.OrderLineItems.FirstOrDefault().ProductName;
                        orderModel.ItemCount = orderModel.OrderLineItems.Where(x => x.ParentOmsOrderLineItemsId == null).ToList().Count;
                    }
                }
                DateTime datetimeStop = DateTime.UtcNow;

                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ExecutionTimeDataBinding, Convert.ToString((datetimeStop - datetimeStart).TotalSeconds)), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                pageListModel.TotalRowCount = dataSet.Tables[0].Rows.Count > 0 ? Convert.ToInt32(dataSet.Tables[0].Rows[0]["RowsCount"]) : 0;
            }
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }
        //Update external Id against Order number.
        private bool UpdateOMSExternalId(string orderNumber, string externalId)
        {
            ZnodeOmsOrderDetail orderDetail = GetOrderDetailsByOMSOrderNumber(orderNumber);
            return UpdateOMSOrderDetailsExternalId(orderDetail, externalId);
        }

        //Get Order Details by Order Number.
        private ZnodeOmsOrderDetail GetOrderDetailsByOMSOrderNumber(string orderNumber)
        {
            ZnodeLogging.LogMessage("Order Number:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderNumber);
            ZnodeOmsOrderDetail orderDetail = (from znodeOrder in _omsOrderRepository.Table
                                               join znodeOrderDetail in _orderDetailsRepository.Table on znodeOrder.OmsOrderId equals znodeOrderDetail.OmsOrderId
                                               where znodeOrder.OrderNumber == orderNumber && znodeOrderDetail.IsActive
                                               select znodeOrderDetail)?.FirstOrDefault();
            return orderDetail;
        }

        //Update external Id against Order Details.
        private bool UpdateOMSOrderDetailsExternalId(ZnodeOmsOrderDetail orderDetail, string externalId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(orderDetail) && !string.IsNullOrEmpty(externalId))
                orderDetail.ExternalId = externalId;

            OrderModel orderModel = orderDetail.ToModel<OrderModel>();

            ZnodeLogging.LogMessage("External Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, externalId);
            bool isSavedExternalId = false;
            if (IsNotNull(orderDetail))
            {
                isSavedExternalId = _orderDetailsRepository.Update(orderDetail);
                ZnodeLogging.LogMessage(isSavedExternalId ? string.Format(Admin_Resources.UpdateExternalId, orderModel.OmsOrderId) : string.Format(Admin_Resources.FailToUpdateExternalId, orderModel.OmsOrderId), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isSavedExternalId;
        }

        protected virtual string ShowOrderAdditionalDetails(string receiptContent, int accountId = 0)
        {
            receiptContent = BindAccountNameToReceipt(receiptContent, accountId);

            return receiptContent;
        }

        //To get the additional details for receipt
        protected virtual string ShowOrderAdditionalDetailsForReceipt(string receiptContent, UserModel userDetails = null)
        {
            if(!string.IsNullOrEmpty(receiptContent))
                return BindAccountNameToReceipt(receiptContent, userDetails);

            return string.Empty;
        }

        //get the order details by order id or order number.
        protected virtual OrderModel GetOrderByOrderDetails(int orderId, string orderNumber = "", FilterCollection filters = null, NameValueCollection expands = null)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { orderId = orderId, orderNumber = orderNumber });
            ZnodeOmsOrder order = null;

            //Variable to check method call from receipt or from other resource.
            bool isFromOrderReceipt = String.IsNullOrEmpty(expands.Get(ExpandKeys.IsFromOrderReceipt));
            bool isFromReOrder = string.IsNullOrEmpty(expands.Get(ExpandKeys.IsFromReOrder));
            bool isOrderHistory = !String.IsNullOrEmpty(expands.Get(ZnodeOmsOrderDetailEnum.ZnodeOmsHistories.ToString()));

            if (IsNull(filters))
                filters = new FilterCollection();

            orderNumber = string.IsNullOrEmpty(orderNumber) ? filters.Find(x => string.Equals(x.FilterName, ZnodeOmsOrderEnum.OrderNumber.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3 : orderNumber;
            string emailAddress = filters.Find(x => string.Equals(x.FilterName, ZnodeUserEnum.Email.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3;

            int portalId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodePortalEnum.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);

            int userId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeUserEnum.UserId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out userId);

            bool isApprover;
            bool.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeConstant.IsApprover, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out isApprover);

            string roleName = filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeConstant.RoleName.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue;

            //check if OrderNumber and EmailAddress available and based on it we will show the order history
            if (!string.IsNullOrEmpty(orderNumber) && !string.IsNullOrEmpty(emailAddress))
            {
                ZnodeLogging.LogMessage("OrderNumber and EmailAddress to show order history: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OrderNumber = orderNumber, EmailAddress = emailAddress });
                IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();

                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeOmsOrderEnum.OrderNumber.ToString(), StringComparison.CurrentCultureIgnoreCase));
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeUserEnum.Email.ToString(), StringComparison.CurrentCultureIgnoreCase));
                order = (from znodeOrder in _omsOrderRepository.Table
                         join znodeOrderDetail in _orderDetailsRepository.Table on znodeOrder.OmsOrderId equals znodeOrderDetail.OmsOrderId
                         join znodeUser in _userRepository.Table on znodeOrderDetail.UserId equals znodeUser.UserId
                         where znodeOrderDetail.PortalId == portalId && znodeOrder.OrderNumber == orderNumber && znodeUser.Email == emailAddress
                         select znodeOrder)?.FirstOrDefault();

            }
            //Manage order receipt for account order on the basis of administrator role.
            else if(!string.IsNullOrEmpty(roleName) && roleName.Equals(ZnodeConstant.AdministratorRole, StringComparison.InvariantCultureIgnoreCase))
            {
                order = (from znodeOrder in _omsOrderRepository.Table
                         join znodeOrderDetail in _orderDetailsRepository.Table on znodeOrder.OmsOrderId equals znodeOrderDetail.OmsOrderId
                         join znodeUser in _userRepository.Table on znodeOrderDetail.UserId equals znodeUser.UserId
                         where znodeOrder.OmsOrderId == orderId
                         select znodeOrder)?.FirstOrDefault();
                filters.Remove(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeUserEnum.UserId.ToString(), StringComparison.InvariantCultureIgnoreCase)));
                filters.Remove(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeConstant.RoleName.ToString(), StringComparison.InvariantCultureIgnoreCase)));
                filters.Remove(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeConstant.IsApprover.ToString(), StringComparison.InvariantCultureIgnoreCase)));
            }

            else if (!string.IsNullOrEmpty(orderNumber))
                //Get active order by order number.
                order = _omsOrderRepository.Table.FirstOrDefault(x => x.OrderNumber == orderNumber);
            else if (userId > 0)
            {
                //Get active order by order id.
                if (isApprover)
                {
                    order = (from znodeOrder in _omsOrderRepository.Table
                             join znodeOrderDetail in _orderDetailsRepository.Table on znodeOrder.OmsOrderId equals znodeOrderDetail.OmsOrderId
                             join znodeUserApprover in _userApprovers.Table on znodeOrderDetail.UserId equals znodeUserApprover.ApproverUserId into oq
                             from z in oq.DefaultIfEmpty()
                             where znodeOrder.OmsOrderId == orderId
                             select znodeOrder)?.FirstOrDefault();

                    filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.IsApprover, StringComparison.CurrentCultureIgnoreCase));
                    filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    order = (from znodeOrder in _omsOrderRepository.Table
                             join znodeOrderDetail in _orderDetailsRepository.Table on znodeOrder.OmsOrderId equals znodeOrderDetail.OmsOrderId
                             join znodeUser in _userRepository.Table on znodeOrderDetail.UserId equals znodeUser.UserId
                             where znodeOrder.OmsOrderId == orderId && znodeUser.UserId == userId
                             select znodeOrder)?.FirstOrDefault();
                }
            }
            else
                //Get active order by order id.
                order = _omsOrderRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId);

            if (order == null)
                return null;

            filters.Add(new FilterTuple(Constants.FilterKeys.OmsOrderId, FilterOperators.Equals, orderId > 0 ? orderId.ToString() : order?.OmsOrderId.ToString()));
            filters.Add(new FilterTuple(Constants.FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.TrueValue));

            ZnodeOmsOrderDetail orderDetails = null;

            ZnodeLogging.LogMessage("Filters to get order details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
            if (orderId > 0 || (IsNotNull(order) && order?.OmsOrderId > 0))
                orderDetails = _orderDetailsRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause, GetExpands(expands));

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return GetOrderDetails(order, orderDetails, isFromOrderReceipt, isOrderHistory, isFromReOrder, expands);
        }

        /* This method will check whether the order is old order or not & set the flag accordingly to show the notification on edit screen
         * to notify that order should be managed carefully as a lot of new calculation changes are released after this order was placed
           so if any modification done, it will work as per new calculation flow and may impact the calculation in case of absent data in old order */
        protected virtual void CheckOrderIsOld(OrderModel orderModel, int omsOrderId)
        {
            string oldOrderIdentifierOrderId = DefaultGlobalConfigSettingHelper.OldOrderIdentifierOrderId;
            orderModel.IsAnOldOrder = !string.IsNullOrEmpty(oldOrderIdentifierOrderId) ?
                                      omsOrderId <= Convert.ToInt32(oldOrderIdentifierOrderId) ? true : false : false;
        }

        //get billing and shipping address
        protected virtual OrderModel GetBillingShippingAddress(OrderModel orderModel, ZnodeOmsOrderDetail orderDetail, int orderShipmentId)
        {
            try
            {
                IZnodeViewRepository<AddressModel> objStoredProc = new ZnodeViewRepository<AddressModel>();
                objStoredProc.SetParameter("@BillingaddressId", orderDetail.AddressId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@orderShipmentId", orderShipmentId, ParameterDirection.Input, DbType.Int32);

                IList<AddressModel> addressList = objStoredProc.ExecuteStoredProcedureList("Znode_GetBillingShippingAddress @BillingaddressId,@orderShipmentId");
                orderModel.BillingAddress = orderDetail?.ToModel<AddressModel>();
                orderModel.ShippingAddress = addressList.FirstOrDefault(x => x.omsOrderShipmentId.Equals(orderShipmentId));

                if (IsNull(orderModel.BillingAddress))
                {
                    orderModel.BillingAddress = addressList.FirstOrDefault(x => x.AddressId == orderDetail.AddressId);
                    orderModel.BillingAddress.EmailAddress = orderDetail.Email;
                }

                if (IsNull(orderModel.ShippingAddress))
                    orderModel.ShippingAddress = orderModel.BillingAddress;

                return orderModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetBillingShippingAddress method for OrderNumber " + orderModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Bind shipping billing address.
        private void BindShippingBillingAddress(ZnodeOmsQuote quoteDetails, out AddressModel shippingAddress, out AddressModel billingAddress)
        {
            try
            {
                shippingAddress = _addressRepository.GetById(quoteDetails.ShippingAddressId.GetValueOrDefault())?.ToModel<AddressModel>();
                billingAddress = _addressRepository.GetById(quoteDetails.BillingAddressId.GetValueOrDefault())?.ToModel<AddressModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in BindShippingBillingAddress method for QuoteNumber " + quoteDetails?.QuoteNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Bind shopping cart model.
        protected virtual void BindShoppingCartModel(ZnodeOmsQuote quoteDetails, AccountQuoteModel accountQuoteModel, ShoppingCartModel model, AddressModel shippingAddress, AddressModel billingAddress)
        {
            try
            {
                model.ShippingAddress = shippingAddress;
                model.BillingAddress = billingAddress;
                model.IsQuoteOrder = HelperUtility.IsNotNull(quoteDetails.QuoteTypeCode) ? true : false;
                model.UserId = quoteDetails.UserId;
                model.PortalId = quoteDetails.PortalId;
                model.OmsQuoteId = quoteDetails.OmsQuoteId;
                model.CurrencyCode = accountQuoteModel.CurrencyCode;
                model.CultureCode = accountQuoteModel.CultureCode;
                model.CardType = quoteDetails.CardType;
                model.CreditCardNumber = quoteDetails.CreditCardNumber;
                model.CreditCardExpMonth = quoteDetails.CreditCardExpMonth;
                model.CreditCardExpYear = quoteDetails.CreditCardExpYear;
                model.Token = quoteDetails.PaymentTransactionToken;
                model.PODocumentName = quoteDetails.PoDocument;
                model.PurchaseOrderNumber = quoteDetails.PurchaseOrderNumber;
                model.OrderDate = DateTime.Now;
                model.OmsOrderStatusId = GetOrderStateIdByName(ZnodeOrderStatusEnum.SUBMITTED.ToString());
                model.CustomTaxCost = quoteDetails.QuoteTypeCode == ZnodeConstant.Quote ? quoteDetails.IsTaxExempt.GetValueOrDefault() ? quoteDetails.TaxCost : null : null;
                // model.CustomShippingCost = quoteDetails.ShippingCost;
                model.ShippingConstraintCode = quoteDetails.ShippingConstraintCode;
                model.JobName = quoteDetails.JobName;
                model.QuoteTypeCode = quoteDetails.QuoteTypeCode;
                model.InHandDate = quoteDetails.InHandDate;
                model.Total = quoteDetails.QuoteOrderTotal;
                model.ShippingCost = Convert.ToDecimal(quoteDetails.ShippingCost);
                model.TaxCost = Convert.ToDecimal(quoteDetails.TaxCost);
                model.ShippingHandlingCharges = Convert.ToDecimal(quoteDetails.ShippingHandlingCharges);
                model.Shipping.ShippingHandlingCharge = Convert.ToDecimal(quoteDetails.ShippingHandlingCharges);
                model.ImportDuty = Convert.ToDecimal(quoteDetails.ImportDuty);
                model.IsPendingOrderRequest = string.IsNullOrEmpty(quoteDetails.QuoteTypeCode) ? true : false;
                model.Shipping.AccountNumber = quoteDetails?.AccountNumber;
                model.Shipping.ShippingMethod = quoteDetails?.ShippingMethod;
                model.Shipping.ShippingCountryCode = shippingAddress.CountryName;
                model.QuotesAccountId = quoteDetails?.AccountId.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in BindShoppingCartModel method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Bind cart parameter model.
        private CartParameterModel BindCartParameterModel(ZnodeOmsQuote quoteDetails, AccountQuoteModel accountQuoteModel)
        {
            return new CartParameterModel
            {
                OmsQuoteId = quoteDetails.OmsQuoteId,
                ShippingId = quoteDetails.ShippingId,
                PublishedCatalogId = GetPublishCatalogId(quoteDetails.PortalId),
                LocaleId = accountQuoteModel.LocaleId.Equals(0) ? GetDefaultLocaleId() : accountQuoteModel.LocaleId,
                UserId = quoteDetails.UserId,
                PortalId = quoteDetails.PortalId,
                ShippingCountryCode = accountQuoteModel.ShippingCountryCode
            };
        }
        //Insert approver comments.
        private void InsertApproverComments(AccountQuoteModel accountQuoteModel)
        {
            //Update comments for the approver.
            if (accountQuoteModel.IsUpdated && IsNotNull(accountQuoteModel.Comments))
            {
                int quoteId = Convert.ToInt32(accountQuoteModel.OmsQuoteId);
                ZnodeOmsQuoteComment quoteComment = _omsQuoteComment.Insert(new ZnodeOmsQuoteComment() { OmsQuoteId = quoteId, Comments = accountQuoteModel.Comments });
                if (quoteComment?.OmsQuoteCommentId > 0)
                {
                    int approverUserId = GetLoginUserId();
                    ZnodeOMSQuoteApproval quoteApproval = _omsQuoteApproval.Table.FirstOrDefault(x => x.OmsQuoteId == quoteId && x.ApproverUserId == approverUserId);
                    if (IsNotNull(quoteApproval))
                    {
                        quoteApproval.OmsQuoteCommentId = quoteComment?.OmsQuoteCommentId;
                        bool quoteApprovalCommentUpdated = _omsQuoteApproval.Update(quoteApproval);
                        ZnodeLogging.LogMessage(quoteApprovalCommentUpdated ? "Comment has been successfully updated against this approver." : "Failed to update comments for the current user.", string.Empty, TraceLevel.Info);
                    }
                }
            }
        }
        private void BillingMapping(ZnodeOmsOrderDetail order, AddressModel model)
        {
            order.DisplayName = model.DisplayName;
            order.BillingCity = model.CityName;
            order.BillingCompanyName = model.CompanyName;
            order.BillingFirstName = model.FirstName;
            order.BillingLastName = model.LastName;
            order.BillingPhoneNumber = model.PhoneNumber;
            order.BillingPostalCode = model.PostalCode;
            order.BillingStreet1 = model.Address1;
            order.BillingStreet2 = model.Address2;
            order.BillingCountry = model.CountryName;
            order.BillingStateCode = model.StateName;
            order.AddressId = model.AddressId;
        }
        private void ShippingMapping(ZnodeOmsOrderShipment shipment, AddressModel model)
        {
            shipment.ShipToCity = model.CityName;
            shipment.ShipToCompanyName = model.CompanyName;
            shipment.ShipToFirstName = model.FirstName;
            shipment.ShipToLastName = model.LastName;
            shipment.ShipToPhoneNumber = model.PhoneNumber;
            shipment.ShipToPostalCode = model.PostalCode;
            shipment.ShipToStreet1 = model.Address1;
            shipment.ShipToStreet2 = model.Address2;
            shipment.ShipToCountry = model.CountryName;
            shipment.ShipToStateCode = model.StateName;
            shipment.DisplayName = model.DisplayName;
            shipment.AddressId = model.AddressId;
        }
        //Set is IsBilling, IsShipping as per data in addressViewModel and addressType
        private void SetBillingShippingFlags(int addressId)
        {
            try
            {
                if (addressId > 0)
                {
                    //Get entity from address repository where AddressId equals with addressModel.AddressId
                    ZnodeAddress addressEntity = _addressRepository.Table?.FirstOrDefault(x => x.AddressId == addressId);
                    addressEntity.IsShipping = true;
                    addressEntity.IsBilling = true;
                    _addressRepository.Update(addressEntity);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        //Get the Address List based on Order Shipment Address Ids.
        private List<ZnodeAddress> GetOrderShipmentAddressList(List<OrderShipmentModel> orderShipments)
        {
            try
            {
                List<ZnodeAddress> addressList = null;
                if (orderShipments?.Count > 0)
                {
                    List<int> addressIds = orderShipments.Where(y => IsNotNull(y)).Select(x => x.AddressId)?.Distinct()?.ToList();
                    if (addressIds?.Count > 0)
                        addressList = _addressRepository.Table.Where(x => addressIds.Contains(x.AddressId))?.ToList();
                }
                return addressList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetOrderShipmentAddressList method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get the Order status list for Customer List based on the Order Line Status Ids.
        private List<ZnodeOmsOrderStateShowToCustomer> GetOrderStatusForCustomerList(List<OrderLineItemModel> lineItems)
        {
            List<ZnodeOmsOrderStateShowToCustomer> orderStatusList = null;
            if (lineItems?.Count > 0)
            {
                List<int?> statusList = lineItems.Select(x => x.OrderLineItemStateId)?.Distinct()?.ToList();
                if (statusList?.Count > 0)
                {
                    IZnodeRepository<ZnodeOmsOrderStateShowToCustomer> _orderStateShowToCustomerRepository = new ZnodeRepository<ZnodeOmsOrderStateShowToCustomer>();
                    orderStatusList = _orderStateShowToCustomerRepository.Table.Where(x => statusList.Contains(x.OmsOrderStateId))?.ToList();
                }

            }
            return orderStatusList;
        }

        //Get the Order State to Customer List based on the Order Line Status Ids.
        private List<ZnodeOmsOrderShipment> GetOrderShipmentList(List<OrderLineItemModel> lineItems)
        {
            List<ZnodeOmsOrderShipment> orderShipmentList = null;
            if (lineItems?.Count > 0)
            {
                List<int> orderShipmentIds = lineItems.Select(x => x.OmsOrderShipmentId)?.Distinct()?.ToList();
                if (orderShipmentIds?.Count > 0)
                    orderShipmentList = _orderShipmentRepository.Table.Where(x => orderShipmentIds.Contains(x.OmsOrderShipmentId))?.ToList();
            }
            return orderShipmentList;
        }

        private List<ZnodeOmsPersonalizeItem> GetPersonalizedValueOrderLineItemList(List<OrderLineItemModel> lineItems)
        {
            List<ZnodeOmsPersonalizeItem> orderPersonalizeItemList = null;
            if (lineItems?.Count > 0)
            {
                List<int?> lineItemIds = lineItems.Select(x => Convert.ToInt32(x.ParentOmsOrderLineItemsId) > 0 ? x.ParentOmsOrderLineItemsId : x.OmsOrderLineItemsId)?.Distinct()?.ToList();
                if (lineItemIds?.Count > 0)
                    orderPersonalizeItemList = new ZnodeRepository<ZnodeOmsPersonalizeItem>().Table.Where(x => lineItemIds.Contains(x.OmsOrderLineItemsId)).ToList();
            }
            return orderPersonalizeItemList;
        }

        private Dictionary<string, object> GetPersonalizedValueOrderLineItem(int orderLineItemId, List<ZnodeOmsPersonalizeItem> personalizeItems)
        {
            Dictionary<string, object> personalizeItem = new Dictionary<string, object>();
            if (orderLineItemId > 0 && personalizeItems?.Count > 0)
            {
                foreach (KeyValuePair<string, string> personalizeAttr in personalizeItems.Where(x => x.OmsOrderLineItemsId == orderLineItemId)?.ToDictionary(x => x.PersonalizeCode, x => x.PersonalizeValue))
                    personalizeItem.Add(personalizeAttr.Key, (object)personalizeAttr.Value);
            }

            return personalizeItem;
        }

        private List<string> GetDownloadableProductKeyList(List<string> lineItemSKU)
        {
            List<string> orderProductKeySKUList = null;
            if (lineItemSKU?.Count > 0)
                orderProductKeySKUList = _pimDownloadableProduct.Table.Where(x => lineItemSKU.Contains(x.SKU)).Select(x => x.SKU)?.ToList();

            return orderProductKeySKUList;
        }

        private List<ZnodeOmsOrderLineItemsAdditionalCost> GetAdditionalCostList(List<OrderLineItemModel> orderLineItems)
        {
            List<ZnodeOmsOrderLineItemsAdditionalCost> additionalCostList = null;
            if (orderLineItems?.Count > 0)
            {
                List<int> parentLineItemIds = orderLineItems?.Where(x => x.ParentOmsOrderLineItemsId == null).Select(x => x.OmsOrderLineItemsId)?.ToList();
                if (parentLineItemIds?.Count > 0)
                    additionalCostList = new ZnodeRepository<ZnodeOmsOrderLineItemsAdditionalCost>().Table.Where(x => parentLineItemIds.Contains(x.OmsOrderLineItemsId.HasValue ? x.OmsOrderLineItemsId.Value : 0))?.ToList();
            }
            return additionalCostList;
        }

        //returns the product type of the order line item by the relationship id of its child.
        private string GetLineItemProductType(OrderLineItemModel orderLineItem, List<ZnodeOmsOrderLineItem> orderLineItems)
        {
            switch (orderLineItems.FirstOrDefault(x => x.ParentOmsOrderLineItemsId == orderLineItem.OmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.AddOns)?.OrderLineItemRelationshipTypeId)
            {
                case (int)ZnodeCartItemRelationshipTypeEnum.Bundles: return ZnodeConstant.BundleProduct;
                case (int)ZnodeCartItemRelationshipTypeEnum.Group: return ZnodeConstant.GroupedProduct;
                case (int)ZnodeCartItemRelationshipTypeEnum.Simple: return ZnodeConstant.SimpleProduct;
                case (int)ZnodeCartItemRelationshipTypeEnum.Configurable: return ZnodeConstant.ConfigurableProduct;
                default: return null;
            }
        }

        //Set the user details if not in user profile it will get billing details
        private void UpdateUserDetailsOnOrder(ShoppingCartModel userModel)
        {
            try
            {
                if (IsNotNull(userModel) && IsNotNull(userModel.UserDetails))
                {
                    ZnodeUser znodeUser = _userRepository.Table.FirstOrDefault(u => u.AspNetUserId == userModel.UserDetails.AspNetUserId);

                    if (IsNotNull(znodeUser))
                    {
                        znodeUser.FirstName = znodeUser.FirstName ?? userModel.BillingAddress?.FirstName;
                        znodeUser.LastName = znodeUser.LastName ?? userModel.BillingAddress?.LastName;
                        znodeUser.PhoneNumber = znodeUser.PhoneNumber ?? userModel.BillingAddress?.PhoneNumber;
                        _userRepository.Update(znodeUser);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in UpdateUserDetailsOnOrder method for OrderNumber " + userModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get and Bind account name to receipt.
        protected virtual string BindAccountNameToReceipt(string receiptContent, int accountId)
        {
            if (accountId > 0)
            {
                IZnodeRepository<ZnodeAccount> _accountRepository = new ZnodeRepository<ZnodeAccount>();
                string accountName = _accountRepository.Table.Where(x => x.AccountId == accountId)?.Select(x => x.Name).FirstOrDefault();

                if (!string.IsNullOrEmpty(accountName))
                    receiptContent = receiptContent.Replace("#AccountName#", accountName);
            }
            else
            {
                receiptContent = receiptContent.Replace("#AccountName#", "&nbsp;");
                receiptContent = receiptContent.Replace("Account Name:", "&nbsp;");
            }
            return receiptContent;
        }

        //Bind Account details
        protected virtual string BindAccountNameToReceipt(string receiptContent, UserModel userDetails = null)
        {
            int accountId = IsNotNull(userDetails) ? userDetails.AccountId.GetValueOrDefault() : 0;

            if (HelperUtility.IsNotNull(userDetails) && !string.IsNullOrEmpty(userDetails.Accountname))
            {
                receiptContent = receiptContent.Replace("#AccountName#", userDetails.Accountname);

            }
            else if (accountId > 0)
            {
                IZnodeRepository<ZnodeAccount> _accountRepository = new ZnodeRepository<ZnodeAccount>();

                string accountName = _accountRepository.Table.Where(x => x.AccountId == accountId)?.Select(x => x.Name).FirstOrDefault();

                if (!string.IsNullOrEmpty(accountName))
                    receiptContent = receiptContent.Replace("#AccountName#", accountName);
            }
            else
            {
                receiptContent = receiptContent.Replace("#AccountName#", "&nbsp;");
                receiptContent = receiptContent.Replace("Account Name:", "&nbsp;");
            }
            return receiptContent;
        }

        //returns the dataset consisting of Order,OrderDetail,ZnodeShipping,OrderLineItems,ZnodeOmsOrderState.
        protected virtual DataSet GetDataSetByOrderId(int orderId)
        {
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

            objStoredProc.GetParameter("@OmsOrderId", orderId, ParameterDirection.Input, SqlDbType.Int);
            return objStoredProc.GetSPResultInDataSet("Znode_GetOrderDetailsByOrderId");
        }

        //Set DataSet table names.
        protected virtual void SetDataTableNames(DataSet dataset)
        {
            dataset.Tables[0].TableName = ZnodeConstant.Order;
            dataset.Tables[1].TableName = ZnodeConstant.OrderDetail;
            dataset.Tables[2].TableName = ZnodeConstant.ZnodeShipping;
            dataset.Tables[3].TableName = ZnodeConstant.OrderLineItems;
            dataset.Tables[4].TableName = ZnodeConstant.ZnodeOmsOrderState;
        }

        private ZnodeUser GetUser(int? userId)
        => _userRepository.Table.FirstOrDefault(x => x.UserId == userId);

        //Send email notifications for line items
        private bool SendOrderLineItemEmailNotification(int? stateId)
            => IsNull(stateId) ? false : orderHelper.IsSendEmail(stateId.GetValueOrDefault());

        private string GetDecryptSMTPUserName(string userName)
        {
            string sMTPUserName = string.Empty;

            ZnodeEncryption encrypt = new ZnodeEncryption();
            sMTPUserName = encrypt.DecryptData(userName);

            return sMTPUserName;
        }

        // To set order Import Duty Tax.
        protected virtual void SetImportDuty(OrderModel model)
        {
            model.ImportDuty = (_omsTaxOrderDetails?.Table?.FirstOrDefault(x => x.OmsOrderDetailsId == model.OmsOrderDetailsId)?.ImportDuty).GetValueOrDefault();
        }

        //Get Tax Summary
        protected virtual void GetOrderTaxSummaryDetails(int omsOrderDetailsId, OrderModel orderModel)
        {
            try
            {
                List<ZnodeOmsTaxOrderSummary> omsTaxOrderSummaries = _omsTaxOrderSummary.Table.Where(x => x.OmsOrderDetailsId == omsOrderDetailsId).ToList();

                orderModel.TaxSummaryList = MapTaxSummaryDetails(omsTaxOrderSummaries);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        // To map tax summary details in list of TaxSummaryModel.
        protected virtual List<TaxSummaryModel> MapTaxSummaryDetails(List<ZnodeOmsTaxOrderSummary> omsTaxOrderSummaries)
        {
            List<TaxSummaryModel> taxSummaryList = new List<TaxSummaryModel>();

            foreach (ZnodeOmsTaxOrderSummary omsTaxOrderSummary in omsTaxOrderSummaries)
            {
                taxSummaryList.Add(new TaxSummaryModel()
                {
                    OmsOrderTaxSummaryId = omsTaxOrderSummary.OmsTaxOrderSummaryId,
                    OmsOrderDetailsId = omsTaxOrderSummary.OmsOrderDetailsId,
                    Tax = omsTaxOrderSummary.Tax,
                    Rate = omsTaxOrderSummary.Rate,
                    TaxName = omsTaxOrderSummary.TaxName,
                    TaxTypeName = omsTaxOrderSummary.TaxTypeName,
                });
            }

            return taxSummaryList;
        }

        protected virtual void UpdatePimDownloadableReturnProductKey(OrderModel model)
        {
            List<OrderLineItemDataModel> itemList = new List<OrderLineItemDataModel>();
            model.ReturnItemList?.ReturnItemList.ForEach(returnItemSku =>
            {
                OrderLineItemDataModel itemModel = new OrderLineItemDataModel();
                OrderLineItemModel orderLineItemDetail = model.OrderLineItems.FirstOrDefault(orderLineItem => orderLineItem.OmsOrderLineItemsId == returnItemSku.OmsOrderLineItemsId
                                                            && orderLineItem.OrderLineItemState != ZnodeOrderStatusEnum.RETURNED.ToString());
                if (HelperUtility.IsNotNull(orderLineItemDetail))
                {
                    itemModel.Sku = orderLineItemDetail.Sku;
                    itemModel.Quantity = orderLineItemDetail.Quantity;
                    itemModel.OmsOrderLineItemsId = orderLineItemDetail.OmsOrderLineItemsId;
                    itemList.Add(itemModel);
                }
            });
            if (itemList.Count > 0)
            {
                //SP call to Update downloadable product key status.
                IZnodeViewRepository<OrderHistoryModel> objStoredProc = new ZnodeViewRepository<OrderHistoryModel>();
                objStoredProc.SetParameter("OrderLineItemDataModel", HelperUtility.ToXML(itemList), ParameterDirection.Input, DbType.String);
                objStoredProc.ExecuteStoredProcedureList("Znode_UpdatePimDownloadableProductKey @OrderLineItemDataModel");
            }
        }

        //Map calculation related properties from calculated shopping cart to shoppingcart which pass to create order
        //Manual mapping is done as we only want caluclated values to be mapped.
        protected virtual ShoppingCartModel MapCalculatedCartToShoppingCart(ShoppingCartModel cartModel, ShoppingCartModel calculatedCartModel)
        {
            try
            {
                //Gift card related properties
                cartModel.GiftCardAmount = calculatedCartModel.GiftCardAmount;
                cartModel.GiftCardBalance = calculatedCartModel.GiftCardBalance;

                //Discount related properties
                cartModel.Discount = calculatedCartModel.Discount;
                cartModel.OrderLevelDiscount = calculatedCartModel.OrderLevelDiscount;
                cartModel.OrderLevelShipping = calculatedCartModel.OrderLevelShipping;
                cartModel.OrderLevelTaxes = calculatedCartModel.OrderLevelTaxes;
                cartModel.CSRDiscountAmount = calculatedCartModel.CSRDiscountAmount;

                //Shipping related properties
                cartModel.ShippingCost = calculatedCartModel.ShippingCost;
                cartModel.ShippingDiscount = calculatedCartModel.ShippingDiscount;
                cartModel.ShippingHandlingCharges = calculatedCartModel.ShippingHandlingCharges;
                cartModel.ShippingDifference = calculatedCartModel.ShippingDifference;
                cartModel.CustomShippingCost = calculatedCartModel?.CustomShippingCost;
                cartModel.Shipping = IsNotNull(calculatedCartModel?.Shipping) ? calculatedCartModel.Shipping : null;

                //Tax related properties
                cartModel.TaxCost = calculatedCartModel.TaxCost;
                cartModel.TaxRate = calculatedCartModel.TaxRate;
                cartModel.SalesTax = calculatedCartModel.SalesTax;
                cartModel.Vat = calculatedCartModel?.Vat;
                cartModel.Gst = calculatedCartModel?.Gst;
                cartModel.Hst = calculatedCartModel?.Hst;
                cartModel.Pst = calculatedCartModel?.Pst;
                cartModel.CustomTaxCost = calculatedCartModel?.CustomTaxCost;
                cartModel.ImportDuty = calculatedCartModel?.ImportDuty;
                cartModel.TaxSummaryList = calculatedCartModel?.TaxSummaryList;
                cartModel.IsCalculateTaxAfterDiscount = calculatedCartModel?.IsCalculateTaxAfterDiscount;

                //Vouchers properties mapping
                cartModel.Vouchers = IsNotNull(calculatedCartModel?.Vouchers) ? calculatedCartModel.Vouchers : new List<VoucherModel>();

                //Cart line item wise mapping
                MapShoppingLineItemPricingProperties(cartModel, calculatedCartModel);

                //Other calculation properties
                cartModel.TotalAdditionalCost = calculatedCartModel?.TotalAdditionalCost;
                cartModel.SubTotal = calculatedCartModel?.SubTotal;
                cartModel.Total = calculatedCartModel?.Total;

                return cartModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapCalculatedCartToShoppingCart method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //In case of pending order live calculations needs to done.
        protected virtual void RecalculatePendingOrderDetails(ShoppingCartModel model)
        {
            try
            {
                if (model.IsPendingOrderRequest)
                {
                    ShoppingCartModel calculatedModel = GetService<IShoppingCartService>().Calculate(model);

                    if (IsNotNull(calculatedModel))
                    {
                        //Map calculation related properties from calculated shopping cart to shoppingcart which pass to create order
                        model = MapCalculatedCartToShoppingCart(model, calculatedModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapCalculatedCartToShoppingCart method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }


        //Map line item calculation related properties  from calculated shippingcart to cart  model
        protected virtual void MapShoppingLineItemPricingProperties(ShoppingCartModel cartModel, ShoppingCartModel calculatedCartModel)
        {
            if (IsNotNull(calculatedCartModel?.ShoppingCartItems))
            {
                //Map calculated shoppingcart line item properties to cartmodel line item properties
                foreach (ShoppingCartItemModel calculatedCartLineItem in calculatedCartModel?.ShoppingCartItems)
                {
                    if (IsNotNull(calculatedCartLineItem))
                    {
                        ShoppingCartItemModel shoppingCartItem = cartModel?.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == calculatedCartLineItem.ExternalId);

                        if (IsNotNull(shoppingCartItem))
                        {
                            //Shipping related properties
                            shoppingCartItem.CustomShippingCost = calculatedCartLineItem.CustomShippingCost;

                            //Discount related properties
                            shoppingCartItem.ProductDiscountAmount = calculatedCartLineItem.ProductDiscountAmount;

                            //Tax related properties
                            shoppingCartItem.TaxCost = calculatedCartLineItem.TaxCost;

                            //Per quantity discount
                            shoppingCartItem.PerQuantityLineItemDiscount = calculatedCartLineItem.PerQuantityLineItemDiscount;
                            shoppingCartItem.PerQuantityCSRDiscount = calculatedCartLineItem.PerQuantityCSRDiscount;
                            shoppingCartItem.PerQuantityShippingCost = calculatedCartLineItem.PerQuantityShippingCost;
                            shoppingCartItem.PerQuantityShippingDiscount = calculatedCartLineItem.PerQuantityShippingDiscount;
                            shoppingCartItem.PerQuantityOrderLevelDiscountOnLineItem = calculatedCartLineItem.PerQuantityOrderLevelDiscountOnLineItem;
                            shoppingCartItem.PerQuantityVoucherAmount = calculatedCartLineItem.PerQuantityVoucherAmount;

                            //Price related properties
                            shoppingCartItem.AdditionalCost = IsNotNull(calculatedCartLineItem?.AdditionalCost) ? calculatedCartLineItem.AdditionalCost : null;
                            shoppingCartItem.ExtendedPrice = calculatedCartLineItem.ExtendedPrice;
                            shoppingCartItem.UnitPrice = calculatedCartLineItem.UnitPrice;

                            if (IsNotNull(shoppingCartItem.Product) && IsNotNull(calculatedCartLineItem.Product))
                            {
                                //Shipping related properties
                                shoppingCartItem.Product.ShippingCost = calculatedCartLineItem.Product?.ShippingCost > 0 ? calculatedCartLineItem.Product?.ShippingCost : calculatedCartLineItem.ShippingCost;

                                //Price related properties
                                shoppingCartItem.Product.PromotionalPrice = calculatedCartLineItem.Product?.PromotionalPrice;
                            }
                        }
                    }
                }
            }
        }

        protected virtual string GetPaymentType(OrderModel orderModel)
        {
            IZnodeRepository<ZnodePaymentType> _paymentType = new ZnodeRepository<ZnodePaymentType>();
            string paymentType = _paymentType.Table?.FirstOrDefault(x => x.PaymentTypeId == orderModel.PaymentTypeId)?.Name;
            return paymentType;
        }

        protected virtual bool CheckIsCardType(ShoppingCartModel model)
        {
            bool isCardType = true;

            if (model.CardType == ZnodeConstant.AmericanExpress)
            {
                model.CardType = ZnodeConstant.TAmericanExpress;
            }
            if (model.Payment.PaymentName == ZnodeConstant.CreditCard)
            {
                if (model.IsOrderFromWebstore)
                {
                    if (model.Payment.PaymentSetting.GatewayCode == ZnodeConstant.CyberSourceVal || model.Payment.PaymentSetting.GatewayCode == ZnodeConstant.Braintree)
                    {
                        if (string.IsNullOrEmpty(model.CardType))
                        {
                            isCardType = false;
                        }
                        else
                        {
                            CheckIsValidCardType(model.CardType);
                        }
                    }
                    return isCardType;
                }
                else
                {
                    if (string.IsNullOrEmpty(model.CardType))
                    {
                        isCardType = false;
                    }
                    else
                    {
                        CheckIsValidCardType(model.CardType);
                    }
                }
            }
            return isCardType;
        }

        protected virtual bool CheckIsValidCardType(string CardType)
        {
            bool isCardType = true;
            switch (CardType.ToUpper())
            {
                case ZnodeConstant.TMastercard:
                    return isCardType = true;
                case ZnodeConstant.TAmericanExpress:
                    return isCardType = true;
                case ZnodeConstant.TDiscover:
                    return isCardType = true;
                case ZnodeConstant.TVisa:
                    return isCardType = true;
                case ZnodeConstant.CreditCard:
                    return isCardType = true;
                default:
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorValidCardType);
                    break;
            }
            return isCardType;
        }

        protected virtual bool checkCardNumber(ShoppingCartModel model)
        {
            bool isCardNumber = true;
            if (model.Payment.PaymentName == ZnodeConstant.CreditCard)
            {
                if (string.IsNullOrEmpty(model.CreditCardNumber))
                {
                    isCardNumber = false;
                }
                else
                {
                    isCardNumber = true;
                }
            }
            return isCardNumber;
        }

        protected virtual bool checkCardExpDate(ShoppingCartModel model)
        {
            bool isCardExpDate = true;
            if (model.Payment.PaymentName == ZnodeConstant.CreditCard)
            {
                if (!model.IsOrderFromWebstore)
                {
                    if (model.Payment.PaymentSetting.GatewayCode != ZnodeConstant.AuthorizeNet)
                    {
                        if (string.IsNullOrEmpty(model.CcCardExpiration))
                        {
                            isCardExpDate = false;
                        }
                        else
                        {
                            isCardExpDate = true;
                        }
                    }
                }
            }
            return isCardExpDate;
        }
        #endregion
    }
}
