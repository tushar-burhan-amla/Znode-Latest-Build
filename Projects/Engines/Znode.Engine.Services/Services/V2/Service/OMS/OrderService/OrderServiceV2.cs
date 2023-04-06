using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Engine.Services.Maps.V2;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class OrderServiceV2 : OrderService, IOrderServiceV2
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailsRepository;
        private readonly IZnodeRepository<ZnodeOmsOrder> _omsOrderRepository;
        private readonly IUserServiceV2 _userService = new UserServiceV2();
        private readonly IShippingService _shippingService;
        private readonly IAccountService _accountService = new AccountService();     
        #endregion Private Variables

        #region Constructor

        public OrderServiceV2() : base()
        {
            _shippingService = ZnodeDependencyResolver.GetService<IShippingService>();
            _orderDetailsRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            _omsOrderRepository = new ZnodeRepository<ZnodeOmsOrder>();
            _userService = ZnodeDependencyResolver.GetService<IUserServiceV2>();
            _accountService = ZnodeDependencyResolver.GetService<IAccountService>();
        }

        #endregion Constructor

        public OrdersListModelV2 GetOrderListV2(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection Page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);           
            //Replace sort key name.
            if (IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            int userId = 0;
            GetUserIdFromFilters(filters, ref userId);

            int fromAdmin = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, Znode.Libraries.ECommerce.Utilities.FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase))?.Item3);
            filters?.RemoveAll(x => string.Equals(x.FilterName, Znode.Libraries.ECommerce.Utilities.FilterKeys.IsFromAdmin, StringComparison.CurrentCultureIgnoreCase));

            PageListModel pageListModel = new PageListModel(filters, sorts, Page);
            ZnodeLogging.LogMessage("pageListModel generated to get order list: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IList<OrderModelV2> list = GetOrderHistory(pageListModel, userId, fromAdmin);
            ZnodeLogging.LogMessage("Order list count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, list?.Count);

            OrdersListModelV2 orderListModel = new OrdersListModelV2() { Orders = list?.ToList() };

            orderListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return orderListModel;
        }

        //Get order list by sp.
        public virtual IList<OrderModelV2> GetOrderHistory(PageListModel pageListModel, int userId, int fromAdmin)
        {
            ZnodeLogging.LogMessage("Input parameters pageListModel, userId and fromAdmin: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), userId, fromAdmin });

            IZnodeViewRepository<OrderModelV2> objStoredProc = new ZnodeViewRepository<OrderModelV2>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsFromAdmin", fromAdmin, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@SalesRepUserId", HelperMethods.GetSalesRepUserId(), ParameterDirection.Input, DbType.Int32);

            return objStoredProc.ExecuteStoredProcedureList("Znode_GetOmsOrderDetail @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT,@UserId,@IsFromAdmin,@SalesRepUserId", 4, out pageListModel.TotalRowCount);
        }


        public virtual OrderResponseModelV2 CreateOrderV2(CreateOrderModelV2 model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            try
            {
                //Get shopping cart
                ShoppingCartModel _shoppingCart = GetShoppingCart(model);

                if (IsNull(_shoppingCart))
                    throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ShoppingCartNotNull);

                if (_shoppingCart.ShoppingCartItems.Count.Equals(0))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.NoItemPresentInShoppingCart);

                ZnodeLogging.LogMessage("ShoppingCartItems count: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, _shoppingCart?.ShoppingCartItems?.Count);

                //Mapping OrderNumber from CreateOrderModelV2 to ShoppingCartModel.
                _shoppingCart.OrderNumber = model.OrderNumber;

                //Get User details                
                _shoppingCart.UserDetails = (model.UserId == 0 && model.IsGuest) ? new UserModel() : GetUserDetails(Convert.ToInt32(model.UserId));
                ZnodeLogging.LogMessage("UserId property of _shoppingCart.UserDetails: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { _shoppingCart?.UserDetails?.UserId });

                //Get Shipping details
                _shoppingCart.Shipping = GetShipping(model.ShippingOptionId);

                //if there is no proper shipping allotted to the order.
                if (HelperUtility.IsNull(_shoppingCart.Shipping))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidShippingId);

                //Get billing address
                _shoppingCart.BillingAddress = GetAddress(Convert.ToInt32(model.UserId), model.BillingAddressId, true);

                //Get Shipping Address
                _shoppingCart.ShippingAddress = GetAddress(Convert.ToInt32(model.UserId), model.ShippingAddressId, false);
                ZnodeLogging.LogMessage("BillingAddress and ShippingAddress generated: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddress = _shoppingCart?.BillingAddress, ShippingAddress = _shoppingCart?.ShippingAddress });

                //prepare shopping cart
                ShoppingCartMapV2.CreateOrderToShoppingCartModel(model, _shoppingCart);

                //Map payment details
                ShoppingCartMapV2.MapPayment(model, _shoppingCart);

                //if there is no proper payment allotted to the order.
                if (HelperUtility.IsNull(_shoppingCart.Payment?.PaymentSetting?.ProfileId))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidPaymentOptionId);

                if (model.IsGuest || string.IsNullOrEmpty(_shoppingCart.UserDetails?.AspNetUserId))
                    _shoppingCart.UserId = 0;

                //create order details
                var _order = CreateOrder(_shoppingCart);

                if (IsNull(_order))
                    throw new ZnodeException(null, $"Unable to place order. Please check product inventory/pricing.", System.Net.HttpStatusCode.BadRequest);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return new OrderResponseModelV2
                {
                    OrderNumber = _order.OrderNumber,
                    OmsOrderId = _order.OmsOrderId,
                    UserEmailId = _shoppingCart.UserDetails.Email,
                    Custom1 = _order.Custom1,
                    Custom2 = _order.Custom2,
                    Custom3 = _order.Custom3,
                    Custom4 = _order.Custom4,
                    Custom5 = _order.Custom5
                };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.ToString(), "V2OMS");
                throw;
            }
        }

        public virtual UserModel GetUserDetails(int userId) => _userService.GetUserById(Convert.ToInt32(userId), null);

        public virtual ShoppingCartModel GetShoppingCart(CreateOrderModelV2 model)
        {
            IShoppingCartServiceV2 _shoppingCartService = new ShoppingCartServiceV2();
            return _shoppingCartService.GetShoppingCart(ShoppingCartMapV2.ToCartParameterModel(model));
        }

        public virtual OrderShippingModel GetShipping(int shippingOptionId) => _shippingService.GetShipping(shippingOptionId).ToEntity<OrderShippingModel>();

        public virtual AddressModel GetAddress(int userId, bool isBilling = true)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter userId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { userId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            NameValueCollection _expand = new NameValueCollection();
            _expand.Add(ZnodeAccountAddressEnum.ZnodeAddress.ToString(), ZnodeAccountAddressEnum.ZnodeAddress.ToString());
            AddressListModel address = _accountService.GetUserAddressList(_expand, filters, null, null);

            ZnodeLogging.LogMessage("CustomerName property and AddressList count of AddressListModel: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CustomerName = address?.CustomerName, AddressListCount = address?.AddressList?.Count });

            if (isBilling)
                return address.AddressList?.Where(x => x.IsDefaultBilling)?.FirstOrDefault();
            else
                return address.AddressList?.Where(x => x.IsDefaultShipping)?.FirstOrDefault();
        }

        public virtual AddressModel GetAddress(int userId, int addressId, bool isBilling)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters userId, addressId and isBilling : ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { userId, addressId, isBilling });

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            NameValueCollection _expand = new NameValueCollection();

            _expand.Add(ZnodeAccountAddressEnum.ZnodeAddress.ToString(), ZnodeAccountAddressEnum.ZnodeAddress.ToString());
            AddressListModel address = _accountService.GetUserAddressList(_expand, filters, null, null);

            ZnodeLogging.LogMessage("CustomerName property and AddressList count of AddressListModel: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { CustomerName = address?.CustomerName, AddressListCount = address?.AddressList?.Count });

            if (isBilling)
                return (addressId > 0) ? address.AddressList?.Where(x => x.AddressId == addressId)?.FirstOrDefault() :
                                         address.AddressList?.Where(x => x.IsDefaultBilling)?.FirstOrDefault();
            else
                return (addressId > 0) ? address.AddressList?.Where(x => x.AddressId == addressId)?.FirstOrDefault() :
                                         address.AddressList?.Where(x => x.IsDefaultShipping)?.FirstOrDefault();
        }

        public virtual bool UpdatePaymentStatusV2(int omsOrderId, int paymentStatusId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters omsOrderId and paymentStatusId : ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { omsOrderId, paymentStatusId });

            if (omsOrderId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.OrderIdLessThanOne);

            if (paymentStatusId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PaymentStatusIdLessThanOne);

            int? paymentStateId = null;
            if (IsNotNull(paymentStatusId))
            {
                paymentStateId = new ZnodeRepository<ZnodeOmsPaymentState>().Table.Where(x => x.OmsPaymentStateId == paymentStatusId)?.FirstOrDefault()?.OmsPaymentStateId;
                ZnodeLogging.LogMessage("paymentStateId generated : ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { paymentStateId });
            }

            if (IsNull(paymentStateId) || paymentStateId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.InvalidPaymentState);

            ZnodeOmsOrderDetail orderDetails = _orderDetailsRepository.Table.Where(x => x.OmsOrderId == omsOrderId && x.IsActive)?.FirstOrDefault();

            if (IsNotNull(orderDetails))
            {
                orderDetails.OmsPaymentStateId = paymentStateId;
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return _orderDetailsRepository.Update(orderDetails);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return false;

        }

        /// <summary>     
        /// </summary>
        /// <param name="orderModel"></param>
        /// <returns></returns>
        public virtual string GetOrderBillingAddressDSG(OrderModel orderModel)
        {
            ZnodeLogging.LogMessage("BillingAddress property of orderModel : ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderModel?.BillingAddress });

            if (IsNotNull(orderModel))
            {
                AddressModel address = orderModel.BillingAddress;
                string address2 = string.IsNullOrEmpty(address.Address2) ? string.Empty : $", {address.Address2}";
                return $"{address.FirstName}{" "}{address.LastName}{"<br />"}{address.Address1}{address2}{"<br />"}{ address.CityName}{", "}{address.StateName}{", "}{address.PostalCode}{"<br />"}{address.CountryName}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{address.PhoneNumber}";
            }
            return string.Empty;
        }

        /// <summary>
       
        /// </summary>
        /// <param name="orderShipment"></param>
        /// <returns></returns>
        public virtual string GetOrderShipmentAddressDSG(OrderShipmentModel orderShipment)
        {
            ZnodeLogging.LogMessage("Input parameter orderShipment: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderShipment);

            if (IsNotNull(orderShipment))
            {
                string shipToStreet2 = string.IsNullOrEmpty(orderShipment.ShipToStreet2) ? string.Empty : $", {orderShipment.ShipToStreet2}";
                orderShipment.ShipToCompanyName = IsNotNull(orderShipment?.ShipToCompanyName) ? $"<br />{orderShipment?.ShipToCompanyName}<br />" : "<br />";
                return $"{orderShipment?.ShipToFirstName}{" "}{ orderShipment?.ShipToLastName}{ orderShipment.ShipToCompanyName}{orderShipment.ShipToStreet1}{shipToStreet2}{"<br />"}{ orderShipment.ShipToCity}{", "}{orderShipment.ShipToStateCode}{", "}{orderShipment.ShipToPostalCode}{"<br />"}{orderShipment.ShipToCountry}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{orderShipment.ShipToPhoneNumber}{"<br />"}{WebStore_Resources.TitleEmail}{" : "}{orderShipment.ShipToEmailId}";
            }
            return string.Empty;
        }

        //Get order details by order id.
        public virtual OrderModel GetOrderByOrderNumber(string orderNumber, FilterCollection filters, NameValueCollection expands)
        {
            return GetOrderByOrderDetails(0, orderNumber, filters, expands);
        }

        //get the order details by order id or order number.
        protected virtual OrderModel GetOrderByOrderDetails(int orderId, string orderNumber = "", FilterCollection filters = null, NameValueCollection expands = null)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters orderId and orderNumber: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderId, orderNumber });

            ZnodeOmsOrder order = null;

            //Variable to check method call from receipt or from other resource.
            bool isFromOrderReceipt = String.IsNullOrEmpty(expands.Get(ExpandKeys.IsFromOrderReceipt));

            bool isOrderHistory = !String.IsNullOrEmpty(expands.Get(ZnodeOmsOrderDetailEnum.ZnodeOmsHistories.ToString()));

            if (IsNull(filters))
                filters = new FilterCollection();

            orderNumber = string.IsNullOrEmpty(orderNumber) ? filters.Find(x => string.Equals(x.FilterName, ZnodeOmsOrderEnum.OrderNumber.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3 : orderNumber;
            string emailAddress = filters.Find(x => string.Equals(x.FilterName, ZnodeUserEnum.Email.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3;

            int portalId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodePortalEnum.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);

            ZnodeLogging.LogMessage("orderNumber, emailAddress and portalId generated from filters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { orderNumber, emailAddress, portalId });

            //check if OrderNumber and EmailAddress available and based on it we will show the order history
            if (!string.IsNullOrEmpty(orderNumber) && !string.IsNullOrEmpty(emailAddress))
            {
                IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();

                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeOmsOrderEnum.OrderNumber.ToString(), StringComparison.CurrentCultureIgnoreCase));
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeUserEnum.Email.ToString(), StringComparison.CurrentCultureIgnoreCase));
                order = (from znodeorder in _omsOrderRepository.Table
                         join znodeOrderDetail in _orderDetailsRepository.Table on znodeorder.OmsOrderId equals znodeOrderDetail.OmsOrderId
                         join znodeUser in _userRepository.Table on znodeOrderDetail.UserId equals znodeUser.UserId
                         where znodeOrderDetail.PortalId == portalId && znodeorder.OrderNumber == orderNumber && znodeUser.Email == emailAddress
                         select znodeorder)?.FirstOrDefault();

            }
            else if (!string.IsNullOrEmpty(orderNumber))
                //Get active order by order number.
                order = _omsOrderRepository.Table.FirstOrDefault(x => x.OrderNumber == orderNumber);
            else
                //Get active order by order id.
                order = _omsOrderRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId);

            if (IsNull(order) && !string.IsNullOrEmpty(orderNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidOrderNo);
            else if (IsNull(order) && orderId > 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidOrderId);

            filters.Add(new FilterTuple(Constants.FilterKeys.OmsOrderId, FilterOperators.Equals, orderId > 0 ? orderId.ToString() : order?.OmsOrderId.ToString()));
            filters.Add(new FilterTuple(Constants.FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.TrueValue));

            ZnodeOmsOrderDetail orderDetails = _orderDetailsRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause, GetExpands(expands));
            return GetOrderDetails(order, orderDetails, isFromOrderReceipt, isOrderHistory,false, expands,false);
        }
    }
}
