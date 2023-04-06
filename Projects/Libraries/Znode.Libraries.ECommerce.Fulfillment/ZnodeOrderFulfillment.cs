using System;
using System.Collections.Generic;

using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.Linq;
using System.Diagnostics;
using System.Data;
using Znode.Engine.Exceptions;
using Znode.Libraries.Resources;

namespace Znode.Libraries.ECommerce.Fulfillment
{
    // Provides methods to manage Orders and OrderLine Items during Checkout
    [Serializable()]
    public class ZnodeOrderFulfillment : ZnodeOrder
    {
        #region Private Variables
        private IZnodeShoppingCartEntities _cart;
        private readonly IZnodeOrderHelper orderHelper = ZnodeDependencyResolver.GetService<IZnodeOrderHelper>();
        private readonly IZnodeRepository<ZnodeOmsCustomerShipping> _omsCustomerShippingRepository;
        private readonly IZnodeRepository<Data.DataModel.ZnodeShipping> _znodeShippingRepository;
        private readonly IZnodeRepository<ZnodeShippingType> _znodeShippingTypeRepository;
        private readonly IZnodeRepository<ZnodeOmsTaxOrderSummary> _omsTaxOrderSummary;
        #endregion

        #region Public Properties
        public IZnodeShoppingCartEntities Cart { get { return _cart; } }
        #endregion

        #region Constructor

        // Initializes a new instance of the ZNodeOrderFulfillment class  
        //ZTodo - need to find way to pass type rather than creating new object
        public ZnodeOrderFulfillment() : base(new ZnodeShoppingCart())
        {
            _omsCustomerShippingRepository = new ZnodeRepository<ZnodeOmsCustomerShipping>();
            _znodeShippingRepository = new ZnodeRepository<Data.DataModel.ZnodeShipping>();
            _znodeShippingTypeRepository = new ZnodeRepository<ZnodeShippingType>();
            _omsTaxOrderSummary = new ZnodeRepository<ZnodeOmsTaxOrderSummary>();
        }

        public ZnodeOrderFulfillment(IZnodeShoppingCartEntities shoppingcart) : base(shoppingcart)
        {
            if (IsNull(_cart))
                _cart = shoppingcart;

            if (IsNull(shoppingcart))
                this.shoppingCart = shoppingcart;

            _omsCustomerShippingRepository = new ZnodeRepository<ZnodeOmsCustomerShipping>();
            _znodeShippingRepository = new ZnodeRepository<Data.DataModel.ZnodeShipping>();
            _znodeShippingTypeRepository = new ZnodeRepository<ZnodeShippingType>();
        }

        #endregion

        #region Public Method

        // Submits order to database       
        public virtual void AddOrderToDatabase(ZnodeOrderFulfillment znodeOrderFulfillment, ShoppingCartModel shoppingCartModel)
        {
            try
            {
                //Set Order data
                SetOrderFulfillment(znodeOrderFulfillment);
                GetOrderAdditionalDetails(znodeOrderFulfillment, shoppingCartModel);

                // Loop through the order line items
                List<OrderDiscountModel> orderDiscounts = GetOrderDiscount(znodeOrderFulfillment.Order);

                //Set order level discount to existing OrderDiscountModel list.
                UpdateOrderLevelDiscountDetails(znodeOrderFulfillment.shoppingCart.OrderLevelDiscountDetails, orderDiscounts);

                //GEt line Item collection
                List<PlaceOrderLineItemModel> lineItemcollection = GetLineItemCollection(znodeOrderFulfillment, shoppingCartModel, ref orderDiscounts);

                //Bind line Item details
                PlaceOrderModel orderModel = BindSubmitOrderModel(znodeOrderFulfillment, shoppingCartModel, orderDiscounts);

                if(IsNotNull(orderModel))
                    orderModel.LineItems = lineItemcollection;

                //Add order details to database
                InsertOrderData(znodeOrderFulfillment.UserID, orderModel);

                if (IsTransactionMade(shoppingCartModel.Payment.PaymentName, shoppingCartModel.TransactionId))
                {
                    OrderPaymentDataModel orderPaymentModel = MapOrderPaymentDetails(shoppingCartModel);

                    //Add order payment details
                    InsertOrderPaymentData(orderPaymentModel, shoppingCartModel.TransactionDate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in the method AddOrderToDatabase for order number {shoppingCartModel.OrderNumber} with exception " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Save order data to database
        public virtual void InsertOrderData(int userId, PlaceOrderModel orderModel)
        {
            try
            {
                if(HelperUtility.IsNotNull(orderModel))
                {
                    IZnodeViewRepository<PlaceOrderStatusModel> objStoredProc = new ZnodeViewRepository<PlaceOrderStatusModel>();
                    objStoredProc.SetParameter("OrderXML", GetCompressedXml(orderModel), ParameterDirection.Input, DbType.Xml);
                    objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), userId > 0 ? userId : HelperMethods.GetLoginAdminUserId(), ParameterDirection.Input, DbType.Int32);
                    PlaceOrderStatusModel placeOrderStatusModel = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateOmsOrder @OrderXML, @UserId").FirstOrDefault();
                    if (placeOrderStatusModel.Status)
                    {
                        this.Order.OmsOrderId = placeOrderStatusModel.OmsOrderId;
                        this.Order.OmsOrderDetailsId = placeOrderStatusModel.OmsOrderDetailsId;
                    }
                    else
                    {
                        ZnodeLogging.LogMessage(placeOrderStatusModel.ErrorMessage, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.Error);
                    }
                }
                else
                {
                    ZnodeLogging.LogMessage($"Error in InsertOrderData method for order number {orderModel.OrderNumber}." +"Model should not be null.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.Error);
                    }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in SP Znode_InsertUpdateOmsOrder for order number {orderModel.OrderNumber}" + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }


        public virtual void UpdateBundleQuantity(OrderLineItemModel orderLineItem, ShoppingCartModel shoppingCartModel)
        {
            foreach (OrderLineItemModel childLineItems in orderLineItem.OrderLineItemCollection)
            {
                foreach (ShoppingCartItemModel shoppingCartItem in shoppingCartModel.ShoppingCartItems)
                {
                    var associatedProduct = shoppingCartItem.BundleProducts?.FirstOrDefault(d => d.SKU == childLineItems.Sku);
                    if (IsNotNull(associatedProduct))
                        childLineItems.BundleQuantity = Convert.ToInt32(associatedProduct.AssociatedQuantity != null ? associatedProduct.AssociatedQuantity.Value : associatedProduct.Quantity);
                }
            }
        }

        public virtual void UpdateOldOrderFlag(int orderId)
        {
            IZnodeRepository<ZnodeOmsOrder> _znodeOmsOrderRepository = new ZnodeRepository<ZnodeOmsOrder>();
            ZnodeOmsOrder omsOrder = _znodeOmsOrderRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId);
            omsOrder.IsOldOrder = false;
            _znodeOmsOrderRepository.Update(omsOrder);
        }

        // Add an entry to gift card history.  
        [Obsolete("This method is marked as obsolete from Znode version 9.6.1, instead of this AddToVoucherHistory method should be used.")]
        public bool AddToGiftCardHistory(ZnodeOrderFulfillment order, int? userId = 0)
        {
            bool isAdded = true;
            if (!string.IsNullOrEmpty(order?.GiftCardNumber) && order?.GiftCardAmount > 0 && order?.Cart?.Vouchers?.Count == 0)
            {
                return Convert.ToBoolean(orderHelper.AddToGiftCardHistory(order.Order.OmsOrderDetailsId, order.GiftCardAmount, order.GiftCardNumber, order.OrderDate, userId));
            }
            return isAdded;
        }

        // Add an entry to voucher history.       
        public virtual bool AddToVoucherHistory(ZnodeOrderFulfillment order, int? userId = 0)
        {
            bool isAdded = true;
            if (order?.Cart?.Vouchers?.Count > 0)
            {
                foreach (ZnodeVoucher voucher in order.Cart.Vouchers)
                {
                    if(voucher.IsVoucherApplied)
                    {
                        isAdded = Convert.ToBoolean(orderHelper.AddToGiftCardHistory(order.Order.OmsOrderDetailsId, voucher.VoucherAmountUsed, voucher.VoucherNumber, order.OrderDate, userId));
                    }
                }
            }
            return isAdded;
        }

        //Save order payment data to database
        public virtual bool InsertOrderPaymentData(OrderPaymentDataModel orderPaymentModel, DateTime? transactionDate)
        {
            try
            {
                if (HelperUtility.IsNotNull(orderPaymentModel) && orderPaymentModel.OmsOrderId > 0)
                {
                    IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                    objStoredProc.SetParameter("OrderPaymentXML", GetCompressedXml(orderPaymentModel), ParameterDirection.Input, DbType.Xml);
                    objStoredProc.SetParameter("OrderId", orderPaymentModel.OmsOrderId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter("TransactionDate", transactionDate, ParameterDirection.Input, DbType.DateTime);
                    objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), orderPaymentModel.CreatedBy > 0 ? orderPaymentModel.CreatedBy : HelperMethods.GetLoginAdminUserId(), ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter(View_ReturnBooleanEnum.Status.ToString(), null, ParameterDirection.Output, DbType.Int32);
                    int status = 0;
                    objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateOmsOrderPayment @OrderPaymentXML, @OrderId, @TransactionDate, @UserId, @Status OUT", 4, out status);

                    if (status == 1)
                        return status == 1;
                    else
                    {
                        ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorOrderModelNull, orderPaymentModel.OmsOrderId), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.Error);
                    }
                }
                else
                {
                    ZnodeLogging.LogMessage($"Error in InsertOrderPaymentData method for order id {orderPaymentModel.OmsOrderId}" + "Model should not be null.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.Error);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in SP Znode_InsertUpdateOmsOrderPayment for order id {orderPaymentModel.OmsOrderId}" + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }


        #endregion

        //Get child item details for order line items
        protected virtual List<PlaceOrderLineItemModel> GetLineItemCollection(ZnodeOrderFulfillment order, ShoppingCartModel shoppingCartModel, ref List<OrderDiscountModel> orderDiscounts)
        {
            List<PlaceOrderLineItemModel> lineItemcollection = new List<PlaceOrderLineItemModel>();
            int groupIdentifier = 0;

            foreach (OrderLineItemModel orderLineItem in this.OrderLineItems)
            {
                //bind Order state ID
                orderLineItem.OrderLineItemStateId = orderLineItem?.OrderLineItemStateId > 0 ? orderLineItem?.OrderLineItemStateId : order.OrderStateID;

                //Gets the transaction number based on 
                GetOrderTransactionNumber(orderLineItem);

                //manage quantity for bundle product
                UpdateBundleQuantity(orderLineItem, shoppingCartModel);

                //This call is for manage order to track incase any line Item status has been changed
                GetModifiedLineItems(orderLineItem);

                //Get line Item Details
                PlaceOrderLineItemModel lineItem = orderHelper.BindLineItemOrderModel(orderLineItem);

                if(HelperUtility.IsNotNull(lineItem))
                {
                    lineItem.GroupIdentifier = groupIdentifier;

                    //Get additional cost
                    lineItem.AdditionalCostList = BindAdditionalCost(orderLineItem?.AdditionalCost);

                    //Get Personalisable Details
                    lineItem.PersonaliseDetails = BindPersonalisableModel(orderLineItem, groupIdentifier);

                    //Get Line item discount
                    orderDiscounts = GetOrderDiscountDetails(orderDiscounts, orderLineItem);

                    //Get child line Item collection
                    lineItem.OrderLineItem = GetChildItemCollection(orderLineItem, groupIdentifier);

                    //Add line Item
                    lineItemcollection.Add(lineItem);
                }

                groupIdentifier++;
            }
            return lineItemcollection;
        }

        //Get child item details
        protected virtual List<PlaceOrderlineItemCollection> GetChildItemCollection(OrderLineItemModel orderLineItem, int groupIdentifier)
        {
            List<PlaceOrderlineItemCollection> lineItemCollection = new List<PlaceOrderlineItemCollection>();
            foreach (OrderLineItemModel childLineItems in orderLineItem.OrderLineItemCollection)
            {
                if (childLineItems?.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns))
                {
                    childLineItems.ParentProductSKU = orderLineItem.Sku;
                }
                PlaceOrderlineItemCollection collection = orderHelper.BindChildItemOrderModel(childLineItems);
                if(HelperUtility.IsNotNull(collection))
                {
                    collection.GroupIdentifier = groupIdentifier;
                    collection.OrderLineItemStateId = orderLineItem.OrderLineItemStateId;
                    childLineItems.OmsOrderDetailsId = orderLineItem.OmsOrderDetailsId;
                    List<PlaceOrderAttributeModel> orderAttribute = new List<PlaceOrderAttributeModel>();
                    if (orderLineItem?.Attributes?.Count > 0)
                    {
                        orderLineItem.Attributes.ToList().ForEach(item => { orderAttribute.Add(new PlaceOrderAttributeModel { AttributeCode = item.AttributeCode, AttributeValue = item.AttributeValue, AttributeValueCode = item.AttributeValueCode, Sku = childLineItems.Sku, GroupId = childLineItems.GroupId, GroupIdentifier = groupIdentifier }); });
                    }
                    collection.OrderAttribute = orderAttribute;
                    lineItemCollection.Add(collection);
                }
            }
            return lineItemCollection;
        }

        //Get order discount
        protected virtual List<PlaceOrderDiscountModel> GetOrderDiscount(List<OrderDiscountModel> orderDiscounts)
        {
            IDiscountHelper discountHelper = ZnodeDependencyResolver.GetService<IDiscountHelper>();
            discountHelper.SplitDiscount(orderDiscounts, OrderLineItems);
            return orderHelper.BindDiscountModel(orderDiscounts);
        }

        //Get modified line item details
        protected virtual void GetModifiedLineItems(OrderLineItemModel orderLineItem)
        {
            if (orderLineItem.IsItemStateChanged)
                this.Order.ModifiedLineItemSkus += orderLineItem.Sku + ",";
        }

        //get transaction number
        protected virtual void GetOrderTransactionNumber(OrderLineItemModel orderLineItem)
        {
            if (Convert.ToBoolean(orderLineItem.IsRecurringBilling))
            {
                orderLineItem.TransactionNumber = _cart.Token;
            }
        }

        //this method will check if payment is made or updated
        protected bool IsTransactionMade(string paymentType, string transactionId)
        {
            if (!string.IsNullOrEmpty(paymentType))
            {
                if (!string.IsNullOrEmpty(paymentType))
                {
                    return !string.Equals(paymentType, ZnodeConstant.COD, StringComparison.InvariantCultureIgnoreCase) &&
                           !string.Equals(paymentType, ZnodeConstant.PurchaseOrder, StringComparison.InvariantCultureIgnoreCase) &&
                           !string.Equals(paymentType, ZnodeConstant.InvoiceMe, StringComparison.InvariantCultureIgnoreCase) &&
                           this.Order.OmsOrderId > 0 && !string.IsNullOrEmpty(transactionId);
                }
            }
            return false;
        }

        // Map order payment details.
        protected virtual OrderPaymentDataModel MapOrderPaymentDetails(ShoppingCartModel shoppingCartModel)
        {
            OrderPaymentDataModel orderPaymentModel = new OrderPaymentDataModel();
            if (HelperUtility.IsNotNull(orderPaymentModel))
            {
                orderPaymentModel.OmsOrderId = this.Order.OmsOrderId;
                orderPaymentModel.TransactionReference = shoppingCartModel.TransactionId;
                orderPaymentModel.Total = this.Order.Total;
                orderPaymentModel.TransactionStatus = GetPaymentStatus(shoppingCartModel.Payment.PaymentStatusId);
                orderPaymentModel.PaymentSettingId = Convert.ToInt32(shoppingCartModel.Payment.PaymentSetting.PaymentSettingId);
                orderPaymentModel.CreatedBy = this.Order.UserId;
            }
            return orderPaymentModel;
        }

        //Get Payment Status
        protected virtual string GetPaymentStatus(int PaymentStatusId)
        {
            string paymentState = string.Empty;
            IZnodeRepository<ZnodeOmsPaymentState> _paymentStateRepository = new ZnodeRepository<ZnodeOmsPaymentState>();
            paymentState = _paymentStateRepository.Table.Where(x => x.OmsPaymentStateId == PaymentStatusId)?.Select(x => x.Name)?.FirstOrDefault();
            return paymentState;
        }

        #region Private Method

        //to set order fulfillment
        private void SetOrderFulfillment(ZnodeOrderFulfillment order)
        {
            // Set order date and status
            this.OrderDate = OrderDate;
            this.DiscountAmount = DiscountAmount;
            this.CSRDiscountAmount = CSRDiscountAmount;
            this.CouponCode = CouponCode;
            this.AdditionalInstructions = AdditionalInstructions;
            this.Order.AddressId = order.BillingAddress.AddressId;
            this.Order.ReferralUserId = order.ReferralUserId;
            this.Order = order.Order;
            this.Order.TotalAdditionalCost = order.shoppingCart.TotalAdditionalCost;
            this.GiftCardNumber = GiftCardNumber;
            this.GiftCardAmount = GiftCardAmount;
        }

        //Get order additional details from shopping cart
        private void GetOrderAdditionalDetails(ZnodeOrderFulfillment orderFulfillment, ShoppingCartModel shoppingCartModel)
        {
            this.Order.FirstName = shoppingCartModel?.UserDetails?.FirstName ?? shoppingCartModel?.BillingAddress?.FirstName;
            this.Order.LastName = shoppingCartModel?.UserDetails?.LastName ?? shoppingCartModel?.BillingAddress?.LastName;
            this.Order.Email = shoppingCartModel?.UserDetails?.Email ?? shoppingCartModel?.ShippingAddress?.EmailAddress;
            this.Order.PhoneNumber = shoppingCartModel?.UserDetails?.PhoneNumber ?? shoppingCartModel?.BillingAddress?.PhoneNumber;
            this.Order.IsCalculateTaxAfterDiscount = shoppingCartModel.IsCalculateTaxAfterDiscount;
            this.Order.Email = shoppingCartModel?.UserDetails?.Email ?? shoppingCartModel?.ShippingAddress?.EmailAddress;
            this.Order.PhoneNumber = shoppingCartModel?.UserDetails?.PhoneNumber ?? shoppingCartModel?.BillingAddress?.PhoneNumber;
            int? defaultOrderStateId = shoppingCartModel.OmsOrderStatusId;
            if (!(defaultOrderStateId > 0) || shoppingCartModel.IsQuoteOrder)
            {
                defaultOrderStateId = orderFulfillment.PortalId > 0 ? orderHelper.GetorderDefaultStateId(orderFulfillment.PortalId) : ZnodeConfigManager.SiteConfig.DefaultOrderStateID.GetValueOrDefault(1);
            }

            // Set order state if order is edited and status is updated then set updated status to new order else default status of order will be set
            if (orderFulfillment.OrderID > 0)
                this.OrderStateID = orderFulfillment.Order.OmsOrderStateId;
            else
                this.OrderStateID = (int)defaultOrderStateId;
        }

        //to get all line items discount 
        private List<OrderDiscountModel> GetOrderDiscount(OrderModel model)
        {
            if (IsNull(model.OrdersDiscount) || model.OrdersDiscount?.Count == 0)
                model.OrdersDiscount = new List<OrderDiscountModel>();
            else
                model.OrdersDiscount?.ForEach(item => { item.OmsOrderDetailsId = model.OmsOrderDetailsId;});
            return model.OrdersDiscount;
        }

        //to get all line items discount 
        private List<OrderDiscountModel> GetOrderDiscountDetails(List<OrderDiscountModel> orderDiscount, OrderLineItemModel orderLineItem)
        {
            //If Quantity of OrderLineItem is Zero then we will take Quantity from OrderLineItemCollection
            orderLineItem.Quantity = orderLineItem.Quantity == 0 ? orderLineItem.OrderLineItemCollection.FirstOrDefault().Quantity : orderLineItem.Quantity;

            orderDiscount = AddDiscountItem(orderLineItem, orderDiscount, orderLineItem.OmsOrderDetailsId);

            if (IsNotNull(orderLineItem.OrderLineItemCollection) && orderLineItem.OrderLineItemCollection.Count > 0)
            {
                foreach (OrderLineItemModel lineItem in orderLineItem.OrderLineItemCollection)
                {
                    orderDiscount = AddDiscountItem(lineItem, orderDiscount, orderLineItem.OmsOrderDetailsId);
                }
            }
            return orderDiscount;
        }

        // Add discount details.
        protected virtual List<OrderDiscountModel> AddDiscountItem(OrderLineItemModel lineItem, List<OrderDiscountModel> orderDiscount, int omsOrderDetailsId)
        {
            if (IsNotNull(lineItem.OrdersDiscount) && lineItem.OrdersDiscount.Count > 0)
            {
                foreach (OrderDiscountModel lineItemDiscount in lineItem.OrdersDiscount)
                {
                    lineItemDiscount.OmsOrderLineItemId = lineItem.OmsOrderLineItemsId;
                    lineItemDiscount.OmsOrderDetailsId = omsOrderDetailsId;
                    lineItemDiscount.DiscountAmount = (lineItem.Quantity * lineItemDiscount.OriginalDiscount);
                    lineItemDiscount.PerQuantityDiscount = lineItemDiscount.OriginalDiscount.GetValueOrDefault();
                    lineItemDiscount.ParentOmsOrderLineItemsId = lineItem.ParentOmsOrderLineItemsId;
                    lineItemDiscount.DiscountMultiplier = (lineItem.Quantity * lineItemDiscount.DiscountMultiplier);
                    lineItemDiscount.Sku = lineItem.Sku;
                    lineItemDiscount.GroupId = lineItem.GroupId;
                    orderDiscount.Add(lineItemDiscount);
                }
            }
            return orderDiscount;
        }

        //to add single line items discount
        private List<OrderDiscountModel> AddDiscountItem(int lineItemId, decimal quantity, List<OrderDiscountModel> orderDiscount, List<OrderDiscountModel> orderLineItem, int omsOrderDetailsId, int? parentOmsOrderLineItemsId, string SKU, string groupId)
        {
            if (IsNotNull(orderLineItem) && orderLineItem.Count > 0)
            {
                foreach (OrderDiscountModel lineItemDiscount in orderLineItem)
                {
                    lineItemDiscount.OmsOrderLineItemId = lineItemId;
                    lineItemDiscount.OmsOrderDetailsId = omsOrderDetailsId;
                    lineItemDiscount.DiscountAmount = (quantity * lineItemDiscount.OriginalDiscount);
                    lineItemDiscount.PerQuantityDiscount =  lineItemDiscount.OriginalDiscount.GetValueOrDefault();
                    lineItemDiscount.ParentOmsOrderLineItemsId = parentOmsOrderLineItemsId;
                    lineItemDiscount.DiscountMultiplier = (quantity * lineItemDiscount.DiscountMultiplier);
                    lineItemDiscount.Sku = SKU;
                    lineItemDiscount.GroupId = groupId;
                    orderDiscount.Add(lineItemDiscount);
                }
            }
            return orderDiscount;
        }

        //Bind order model for saving order details
        protected virtual PlaceOrderModel BindSubmitOrderModel(ZnodeOrderFulfillment order, ShoppingCartModel shoppingCartModel, List<OrderDiscountModel> orderDiscounts)
        {
            PlaceOrderModel ordermodel = orderHelper.BindOrderModel(order.Order);
            if(HelperUtility.IsNotNull(ordermodel))
            {
                ordermodel.DiscountAmount = order.DiscountAmount += order.CSRDiscountAmount;
                ordermodel.BillingAddressId = order.BillingAddress.AddressId;
                ordermodel.ShippingAddressId = order.ShippingAddress.AddressId;
                if(HelperUtility.IsNotNull(shoppingCartModel))
                {
                    ordermodel.PaymentStatusId = shoppingCartModel.Payment?.PaymentStatusId ?? 0;
                    ordermodel.PaymentCode = shoppingCartModel.Payment?.PaymentSetting?.PaymentCode;
                    ordermodel.PaymentDisplayName = shoppingCartModel.Payment?.PaymentDisplayName;
                    ordermodel.ShippingTypeId = shoppingCartModel.Shipping?.ShippingTypeId;
                    ordermodel.ShippingId = shoppingCartModel.Shipping?.ShippingId ?? 0;
                    ordermodel.ShippingMethod = shoppingCartModel.Shipping?.ShippingMethod;
                    ordermodel.AccountNumber = shoppingCartModel.Shipping?.AccountNumber;
                    ordermodel.IsOldOrder = shoppingCartModel.IsOldOrder;
                    ordermodel.AccountId = shoppingCartModel?.QuotesAccountId > 0 ? shoppingCartModel?.QuotesAccountId : (shoppingCartModel?.UserDetails?.AccountId).GetValueOrDefault();
                    if (HelperUtility.IsNull(shoppingCartModel.Payment?.PaymentName) && shoppingCartModel?.Payment?.PaymentSetting?.PaymentTypeId > 0)
                    {
                        IZnodeRepository<ZnodePaymentType> _paymentTypeRepository = new ZnodeRepository<ZnodePaymentType>();
                        shoppingCartModel.Payment.PaymentName = _paymentTypeRepository.Table.FirstOrDefault(x=>x.PaymentTypeId == shoppingCartModel.Payment.PaymentSetting.PaymentTypeId).Name;
                    }
                    if (string.Equals(shoppingCartModel.Payment?.PaymentName, ZnodeConstant.PurchaseOrder, StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals(shoppingCartModel.Payment?.PaymentName, ZnodeConstant.InvoiceMe, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ordermodel.RemainingOrderAmount = order.Total;
                    }
                }
                ordermodel.TaxRate = shoppingCartModel.TaxRate;
                ordermodel.HST = OrderLineItems.Sum(x => x.HST);
                ordermodel.VAT = OrderLineItems.Sum(x => x.VAT);
                ordermodel.PST = OrderLineItems.Sum(x => x.PST);
                ordermodel.GST = OrderLineItems.Sum(x => x.GST);
                ordermodel.SalesTax = OrderLineItems.Sum(x => x.SalesTax);
                ordermodel.OrderDiscounts = GetOrderDiscount(orderDiscounts);
                ordermodel.BillingAddressModel = MapBillingAddressDetails(ordermodel, order.BillingAddress.AddressId);
            }
            return ordermodel;
        }

        //Bind personalisable data
        public virtual List<PlaceOrderPersonaliseModel> BindPersonalisableModel(OrderLineItemModel orderLineItem, int groupIdentifier)
        {
            List<PersonaliseValueModel> personaliseValuesDetail = orderLineItem?.PersonaliseValuesDetail;
            List<PlaceOrderPersonaliseModel> list = new List<PlaceOrderPersonaliseModel>();
            if (HelperUtility.IsNotNull(personaliseValuesDetail))
            {
                personaliseValuesDetail?.ForEach(item => {
                    list.Add(new PlaceOrderPersonaliseModel
                    {
                        PersonalizeCode = item.PersonalizeCode,
                        PersonalizeValue = item.PersonalizeValue,
                        DesignId = item.DesignId,
                        ThumbnailURL = item.ThumbnailURL,
                        Sku = orderLineItem.Sku,
                        GroupId = orderLineItem.GroupId,
                        GroupIdentifier = groupIdentifier
                    });
                });
            }
            return list;
        }

        //Get additional cost
        public virtual List<AdditionalCost> BindAdditionalCost(Dictionary<string, decimal> additionalCost)
        {
            List<AdditionalCost> list = new List<AdditionalCost>();
            if (HelperUtility.IsNotNull(additionalCost) && additionalCost?.Count > 0)
            {
                foreach (KeyValuePair<string, decimal> values in additionalCost)
                {
                    AdditionalCost cost = new AdditionalCost();
                    cost.KeyName = values.Key;
                    cost.KeyValue = values.Value;
                    list.Add(cost);

                }
            }
            return list;
        }

        //to check email notification
        private bool SendEmailNotification(int? stateId)
            => IsNull(stateId) ? false : orderHelper.IsSendEmail(stateId.GetValueOrDefault());

        protected virtual void UpdateOrderLevelDiscountDetails(List<OrderDiscountModel> orderLevelDiscountDetails, List<OrderDiscountModel> orderDiscounts)
        {
            if (orderLevelDiscountDetails?.Count > 0)
            {
                foreach (OrderDiscountModel item in orderLevelDiscountDetails)
                {
                    item.OmsOrderDetailsId = this.Order.OmsOrderDetailsId;
                    item.OmsOrderLineItemId = null;
                    orderDiscounts.Add(item);
                }
            }
        }

        //Create Return Table
        protected virtual DataTable CreateReturnTable()
        {
            DataTable returnTable = new DataTable();
            returnTable.Columns.Add(ZnodeOmsOrderLineItemEnum.OmsOrderLineItemsId.ToString(), typeof(int));
            returnTable.Columns.Add("UpdatedOmsOrderLineItemsId", typeof(int));
            return returnTable;
        }

        //Update RMA Return Details
        protected virtual void UpdateRMAReturnDetails(DataTable returnLineItemTable, List<OrderLineItemModel> OrderLineItem, int orderId, int oldOrderDetailId)
        {
            if (orderId > 0)
            {
                ZnodeRepository<ZnodeRmaReturnDetail> _znodeRmaReturnDetailRepository = new ZnodeRepository<ZnodeRmaReturnDetail>();
                List<ZnodeRmaReturnDetail> returnDetailsList = _znodeRmaReturnDetailRepository.Table.Where(x => x.OmsOrderId == orderId)?.ToList();
                if (returnDetailsList?.Count > 0)
                {
                    returnDetailsList?.Where(x => x.OmsOrderDetailsId == oldOrderDetailId)?.ToList()?.ForEach(x => x.OmsOrderDetailsId = this.Order.OmsOrderDetailsId);
                    bool isReturnDetailsUpdated = _znodeRmaReturnDetailRepository.BatchUpdate(returnDetailsList);

                    int[] rmaReturnDetailsIds = returnDetailsList?.Select(x => x.RmaReturnDetailsId)?.ToArray();
                    if (isReturnDetailsUpdated && rmaReturnDetailsIds?.Length > 0 && IsNotNull(rmaReturnDetailsIds))
                    {
                        ZnodeRepository<ZnodeRmaReturnLineItem> _returnLineItemsRepository = new ZnodeRepository<ZnodeRmaReturnLineItem>();
                        List<ZnodeRmaReturnLineItem> returnLineItemList = _returnLineItemsRepository.Table.Where(x => rmaReturnDetailsIds.Contains(x.RmaReturnDetailsId))?.ToList();
                        if (returnLineItemList?.Count > 0 && IsNotNull(returnLineItemTable))
                        {
                            foreach (ZnodeRmaReturnLineItem returnLineItem in returnLineItemList)
                            {
                                int? updatedOmsOrderLineItemsId = Convert.ToInt32(returnLineItemTable.Select(ZnodeOmsOrderLineItemEnum.OmsOrderLineItemsId.ToString() + "=" + returnLineItem.OmsOrderLineItemsId)?.FirstOrDefault()?.ItemArray[1]?.ToString());
                                if (IsNotNull(updatedOmsOrderLineItemsId) && updatedOmsOrderLineItemsId > 0)
                                {
                                    updatedOmsOrderLineItemsId = returnLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles ? updatedOmsOrderLineItemsId :
                                                                 OrderLineItem?.FirstOrDefault(x => x.OmsOrderLineItemsId == updatedOmsOrderLineItemsId)?.OrderLineItemCollection?.FirstOrDefault(x => x.ParentOmsOrderLineItemsId == updatedOmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId == returnLineItem?.OrderLineItemRelationshipTypeId)?.OmsOrderLineItemsId;
                                    if (IsNotNull(updatedOmsOrderLineItemsId))
                                        returnLineItem.OmsOrderLineItemsId = (int)updatedOmsOrderLineItemsId;
                                }
                            }
                            _returnLineItemsRepository.BatchUpdate(returnLineItemList);
                        }
                    }
                }
            }
        }

        //Save tax summary details for order.
        protected virtual void SaveTaxOrderSummaryDetails(int omsOrderDetailsId, List<TaxSummaryModel> taxSummaryListModel)
        {
            try
            {
                if (HelperUtility.IsNotNull(taxSummaryListModel) && taxSummaryListModel?.Count > 0)
                {
                    List<ZnodeOmsTaxOrderSummary> omsTaxOrderSummaryList = new List<ZnodeOmsTaxOrderSummary>();
                    IZnodeRepository<ZnodeOmsTaxOrderSummary> _omsTaxOrderSummaryRepository = new ZnodeRepository<ZnodeOmsTaxOrderSummary>();
                    List<ZnodeOmsTaxOrderSummary> omsTaxOrderSummaries = _omsTaxOrderSummaryRepository.Table.Where(x => x.OmsOrderDetailsId == omsOrderDetailsId).ToList();

                    // From manage order screen, if address will get changed the tax values may vary so it is safe to delete the existing records.                
                    if (omsTaxOrderSummaries?.Count > 0)
                        _omsTaxOrderSummaryRepository.Delete(omsTaxOrderSummaries);

                    foreach (TaxSummaryModel transactionSummary in taxSummaryListModel)
                    {
                        omsTaxOrderSummaryList.Add(new ZnodeOmsTaxOrderSummary()
                        {
                            Tax = transactionSummary.Tax,
                            Rate = transactionSummary.Rate,
                            TaxName = transactionSummary.TaxName,
                            TaxTypeName = transactionSummary.TaxTypeName,
                            OmsOrderDetailsId = omsOrderDetailsId
                        });
                    }
                    _omsTaxOrderSummaryRepository.Insert(omsTaxOrderSummaryList);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            #endregion
        }

        // Map billing address details.
        protected virtual PlaceOrderAddressModel MapBillingAddressDetails(PlaceOrderModel ordermodel, int addressId)
        {
            AddressModel model = this.Order?.BillingAddress;
            PlaceOrderAddressModel addressModel = new PlaceOrderAddressModel();
            if (HelperUtility.IsNotNull(model))
            {
                addressModel.AddressId = model.AddressId > 0 ? model.AddressId : addressId;
                addressModel.FirstName = model.FirstName;
                addressModel.LastName = model.LastName;
                addressModel.Country = model.CountryName;
                addressModel.StateCode = model.StateCode;
                addressModel.PostalCode = model.PostalCode;
                addressModel.PhoneNumber = model.PhoneNumber;
                addressModel.EmailId = model.EmailAddress;
                addressModel.Street1 = model.Address1;
                addressModel.Street2 = model.Address2;
                addressModel.City = model.CityName;
                addressModel.CompanyName = model.CompanyName;
            }
            return addressModel;
        }
    }
}
