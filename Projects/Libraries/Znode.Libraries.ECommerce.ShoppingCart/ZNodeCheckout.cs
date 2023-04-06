using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Enum;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Fulfillment;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using ZNode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;


namespace Znode.Libraries.ECommerce.ShoppingCart
{
    // Order Checkout Class - Order the checkout process
    public class ZnodeCheckout : ZnodeBusinessBase, IZnodeCheckout
    {
        #region Member Variables
        private readonly int _ShippingID = 0;
        private readonly IZnodeOrderHelper orderHelper;
        private readonly IPublishProductHelper publishProductHelper;
        private readonly Dictionary<int, string> publishCategory = new Dictionary<int, string>();
        #endregion

        #region Constructor
        public ZnodeCheckout()
        {
            orderHelper = GetService<IZnodeOrderHelper>();
            publishProductHelper = GetService<IPublishProductHelper>();
        }

        // Initializes a new instance of the ZNodeCheckout class.
        public ZnodeCheckout(UserAddressModel userAccount, IZnodePortalCart shoppingCart)
        {

            this.UserAccount = userAccount;
            this.ShoppingCart = shoppingCart;
            orderHelper = GetService<IZnodeOrderHelper>();
            publishProductHelper = GetService<IPublishProductHelper>();
        }

        #endregion

        #region Public Properties
        // Gets or sets a value indicating whether it is success or not
        public bool IsSuccess { get; set; }

        // Gets or sets the PaymentResponse Text
        public string PaymentResponseText { get; set; }

        // Gets or sets the user account 
        public UserAddressModel UserAccount { get; set; }

        // Gets or sets the shopping cart 
        public IZnodePortalCart ShoppingCart { get; set; }

        // Gets or sets the Customer Additional instructions for this order
        public string AdditionalInstructions { get; set; }

        // Gets or sets the purchase order number applied by customer,
        public string PurchaseOrderNumber { get; set; }

        // Gets or sets the purchase order document name uploaded by customer,
        public string PoDocument { get; set; }

        // Gets or sets the shipping id
        public int ShippingID { get; set; }

        // Gets or sets the portal id
        public int PortalID { get; set; }

        #endregion

        #region  public virtual Methods

        // to submits order
        public virtual ZnodeOrderFulfillment SubmitOrder(SubmitOrderModel model, ShoppingCartModel shoppingCartModel, bool isTaxCostUpdated = true)
        {
            string orderNumber = string.IsNullOrEmpty(model?.OrderNumber) ? shoppingCartModel?.OrderNumber : model?.OrderNumber;
            int portalID = ShoppingCart.PortalID;
            int orderId = model?.OrderId.GetValueOrDefault() ?? 0;
            List<OrderShipmentModel> orderShipmentModel = new List<OrderShipmentModel>();

            //Bind ZnodeOrderFulfillment
            ZnodeOrderFulfillment order = BindOrderDetails(model, shoppingCartModel, orderId, portalID, out orderShipmentModel);

            try
            {
                if (orderId > 0 && !CancelExistingOrder(order, orderId))
                    return order;

                SetOrderAdditionalDetails(order, model);

                bool paymentIsSuccess = SetPaymentDetails(order);
                order.Order.OrderNumber = model?.OrderNumber;
                if (paymentIsSuccess)
                {
                    order.AddOrderToDatabase(order, shoppingCartModel);
                    if (orderId > 0)
                    {
                        //to save return items in data base for selected order
                        this.IsSuccess = SaveReturnItems(order.Order.OmsOrderDetailsId, model?.ReturnOrderLineItems);
                        ZnodeLogging.LogMessage($"Updated existing order for Order number:{orderNumber}", ZnodeLogging.Components.OMS.ToString());
                    }

                    //Set Order Shipment Details to order Line Item.
                    SetOrderShipmentDetails(order, orderShipmentModel);

                    //set orderId
                    SetOrderDetailsToShoppingCart(order);

                    //This method is responsible for carrying out the post order submit process
                    PostOrderSubmitProcess(shoppingCartModel, order, isTaxCostUpdated);

                    this.IsSuccess = true;
                }
                else
                {
                    this.IsSuccess = false;
                    VerifySubmitOrderProcess(order.Order.OmsOrderDetailsId);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Failed in order processing for order number {orderNumber} with exception {ex.Message}", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);

                if (IsNotNull(ex.InnerException))
                    ZnodeLogging.LogMessage(ex.InnerException.ToString(), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error); // log exception

                this.IsSuccess = false;
                throw;
            }
            return order;
        }

        protected virtual bool CheckIfNewOrderPaymentCC(ShoppingCartModel shoppingCartModel, bool isCreateOrder)
        {
            bool IsSaveFailedOrder = false;
            if (isCreateOrder && shoppingCartModel.Payment.PaymentSetting.PaymentTypeName == Enum.GetName(typeof(ZNodePaymentTypeEnum), 0)
                        && (Equals((int)ZnodePaymentStatus.AUTHORIZED, shoppingCartModel.Payment.PaymentStatusId)
                        || Equals((int)ZnodePaymentStatus.CAPTURED, shoppingCartModel.Payment.PaymentStatusId)))
            {
                IsSaveFailedOrder = true;
            }
            return IsSaveFailedOrder;
        }

        //This constructor initializes the order and OrderLineItem entity objects.
        public virtual ZnodeOrderFulfillment GetOrderFulfillment(UserAddressModel userAccount, IZnodePortalCart shoppingCart, int portalId, out List<OrderShipmentModel> model)
        {
            ZnodeOrderFulfillment order = new ZnodeOrderFulfillment(shoppingCart);

            order.PortalId = portalId;          

            SetOrderDetails(order, shoppingCart, userAccount);
            model = new List<OrderShipmentModel>();
            try
            {
                foreach (IZnodeMultipleAddressCart addressCart in shoppingCart.AddressCarts)
                {
                    int? shippingId = addressCart.Shipping.ShippingID != 0 ? addressCart.Shipping.ShippingID : this._ShippingID;
                    AddressModel address = userAccount?.ShippingAddress;
                    address = IsNull(address) || Equals(address.AddressId, 0) ? shoppingCart.Payment?.ShippingAddress : address;
                    OrderShipmentModel shipment = CreateOrderShipment(address, shippingId, userAccount?.Email);
                    if(HelperUtility.IsNotNull(shipment))
                    {
                        model.Add(shipment);
                        addressCart.OrderShipmentID = shipment.OmsOrderShipmentId;
                    }
         
                    SetOrderLineItems(order, addressCart);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in CreateOrderShipment with shippingId:{shoppingCart?.Shipping?.ShippingID}  with exception {ex.Message}", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return order;
        }
        
        //Get order details
        protected virtual ZnodeOrderFulfillment BindOrderDetails(SubmitOrderModel model, ShoppingCartModel shoppingCartModel, int orderId, int portalID, out List<OrderShipmentModel> orderShipmentModel)
        {
            string orderNumber = string.IsNullOrEmpty(shoppingCartModel?.OrderNumber) ? model?.OrderNumber : shoppingCartModel?.OrderNumber;
            orderShipmentModel = new List<OrderShipmentModel>();
            try
            {
                ZnodeOrderFulfillment order = this.GetOrderFulfillment(this.UserAccount, this.ShoppingCart, portalID, out orderShipmentModel);
                order.Order.OmsOrderDetailsId = model?.OmsOrderDetailsId == null ? 0 : Convert.ToInt32(model?.OmsOrderDetailsId);
                order.AccountNumber = shoppingCartModel?.Shipping?.AccountNumber;
                order.ShippingMethod = shoppingCartModel?.Shipping?.ShippingMethod;

                if (IsNotNull(shoppingCartModel?.Payment?.PaymentStatusId))
                    order.PaymentStatusID = shoppingCartModel.Payment.PaymentStatusId;

                if (shoppingCartModel?.ReturnItemList?.Count > 0)
                    order.GiftCardAmount = shoppingCartModel.GiftCardAmount;

                if (orderId > 0)
                    SetOrderStateTrackingNumber(order, model);

                return order;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in BindOrderDetails with order number: {orderNumber} with exception {ex.Message}", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //Get post order submit model
        protected virtual PostOrderSubmitModel GetPostOrderSubmitModel(ShoppingCartModel shoppingCartModel, ZnodeOrderFulfillment order)
        {
            PostOrderSubmitModel postOrderModel = new PostOrderSubmitModel();
            postOrderModel.CookieMappingId = !string.IsNullOrEmpty(shoppingCartModel.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(shoppingCartModel.CookieMappingId)) : 0;
            postOrderModel.GiftCardAmount = shoppingCartModel.GiftCardAmount;
            postOrderModel.IsCalculateVoucher = shoppingCartModel.IsCalculateVoucher;
            postOrderModel.Coupons = shoppingCartModel.Coupons;
            postOrderModel.Vouchers = shoppingCartModel.Vouchers;
            postOrderModel.BillingAddressId = order.BillingAddress.AddressId;
            postOrderModel.OrderID = order?.Order.OmsOrderId ?? 0;
            postOrderModel.IsGuest = shoppingCartModel.ShippingAddress.IsGuest;
            postOrderModel.IsReferralCommission = IsNotNull(order.ReferralUserId) && order.ReferralUserId > 0;
            postOrderModel.CommissionAmount = (order.SubTotal - (order.DiscountAmount + order.GiftCardAmount));
            postOrderModel.SetBillingShippingFlags = shoppingCartModel.BillingAddress.AddressId == shoppingCartModel.ShippingAddress.AddressId && (!shoppingCartModel.ShippingAddress.IsShipping || !shoppingCartModel.ShippingAddress.IsBilling);
            postOrderModel.TaxSummaryList = shoppingCartModel.TaxSummaryList;
            return postOrderModel;
        }

        //This method is responsible for carrying out the post order submit process
        protected virtual void PostOrderSubmitProcess(ShoppingCartModel shoppingCartModel, ZnodeOrderFulfillment order, bool isTaxCostUpdated = true)
        {
            try
            {
                if(HelperUtility.IsNotNull(order))
                {
                    OrderWarehouseModel orderWarehouseModel = GetOrderInventory(order, this.ShoppingCart);
                    string xmldata = GetCompressedXml(GetPostOrderSubmitModel(shoppingCartModel, order));
                    int status = 0;
                    IZnodeViewRepository<OrderWarehouseLineItemsModel> objStoredProc = new ZnodeViewRepository<OrderWarehouseLineItemsModel>();
                    objStoredProc.SetParameter("PostOrderXml", xmldata, ParameterDirection.Input, DbType.Xml);
                    objStoredProc.SetParameter("InventoryData", GetCompressedXml(orderWarehouseModel.LineItems), ParameterDirection.Input, DbType.Xml);
                    objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), ShoppingCart.GetUserId(), ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter("@PortalId".ToString(), order.Order.PortalId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter(View_ReturnBooleanEnum.Status.ToString(), null, ParameterDirection.Output, DbType.Int32);
                    List<OrderWarehouseLineItemsModel> productInventoryList = objStoredProc.ExecuteStoredProcedureList("Znode_PostSubmitOrderProcess @PostOrderXml, @InventoryData,@UserId,@PortalId,@Status OUT", 4, out status)?.ToList();

                    if (status == 1)
                    {
                        ShoppingCart.LowInventoryProducts = new List<OrderWarehouseLineItemsModel>();
                        ShoppingCart.LowInventoryProducts = productInventoryList?.Where(x => x.Quantity <= 0 && !x.AllowBackOrder)?.ToList();
                        ShoppingCart.PostSubmitOrderProcess(order?.Order.OmsOrderId ?? 0, shoppingCartModel.ShippingAddress.IsGuest, isTaxCostUpdated);
                    }
                    else
                    {
                        ZnodeLogging.LogMessage($"Error in Post Order Submit Process for the Znode_PostSubmitOrderProcess for and order number {order?.Order.OrderNumber}", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);

                    }
                }
                else
                {
                    ZnodeLogging.LogMessage($"Error in Post Order Submit Process for order number {order?.Order.OrderNumber}. Model cannot be null", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in Post Order Submit Process for the Znode_PostSubmitOrderProcess for and order number {order?.Order.OrderNumber} with exception {ex.Message} ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        #endregion

        #region  public virtual Methods

        //to add order level discount to discount model
        public virtual void SetOrderDiscount(OrderModel model, decimal discountAmount, string discountCode, int discountType, DiscountLevelTypeIdEnum discountApplicable, decimal discountMultiplier)
        {
            if (IsNotNull(model))
            {
                if (IsNull(model.OrdersDiscount) || model.OrdersDiscount?.Count == 0)
                {
                    model.OrdersDiscount = new System.Collections.Generic.List<OrderDiscountModel>();
                }
                model.OrdersDiscount.Add(new OrderDiscountModel
                {
                    OmsDiscountTypeId = discountType,
                    DiscountCode = discountCode,
                    DiscountAmount = discountAmount,
                    OriginalDiscount = discountAmount,
                    DiscountMultiplier = discountMultiplier,
                    DiscountLevelTypeId = (int)discountApplicable
                });
            }
        }


        //to save OrderShipment data and return OrderShipmentId
        public virtual OrderShipmentModel CreateOrderShipment(AddressModel shippingAddress, int? shippingId, string emailId)
        {
            return orderHelper.SaveShippingAddressData(shippingAddress, shippingId, emailId);
        }

        //to set order details
        public virtual void SetOrderDetails(ZnodeOrderFulfillment order, IZnodePortalCart shoppingCart, UserAddressModel userAccount)
        {
            order.UserID = userAccount.UserId;
            order.Created = GetDateTime();
            order.Modified = GetDateTime();
            order.CreatedBy = userAccount.UserId;
            order.ModifiedBy = userAccount.UserId;
            order.TaxCost = shoppingCart.TaxCost;
            order.VAT = shoppingCart.VAT;
            order.TaxSummaryList = shoppingCart.TaxSummaryList;
            order.SalesTax = shoppingCart.SalesTax;
            order.HST = shoppingCart.HST;
            order.PST = shoppingCart.PST;
            order.GST = shoppingCart.GST;
            order.ImportDuty = shoppingCart.ImportDuty;
            order.Email = userAccount.Email;
            order.CurrencyCode = shoppingCart.CurrencyCode;
            order.CultureCode = shoppingCart.CultureCode;
            order.ShippingCost = shoppingCart.ShippingCost;
            order.ShippingDifference = shoppingCart.ShippingDifference;
            order.SubTotal = shoppingCart.SubTotal;
            order.Total = shoppingCart.Total;
            order.ExternalId = shoppingCart.ExternalId;
            order.DiscountAmount = shoppingCart.Discount;
            order.OrderTotalWithoutVoucher = shoppingCart.OrderTotalWithoutVoucher;
            order.BillingAddress = userAccount.BillingAddress;
            order.ShippingAddress = userAccount.ShippingAddress;
            order.OrderDate = shoppingCart.OrderDate.GetValueOrDefault();
            order.PaymentTransactionToken = shoppingCart.Token;
            order.CreditCardNumber = shoppingCart.CreditCardNumber;
            order.CardType = shoppingCart.CardType;
            order.CreditCardExpMonth = shoppingCart.CreditCardExpMonth;
            order.CreditCardExpYear = shoppingCart.CreditCardExpYear;
            order.IsShippingCostEdited = IsNotNull(shoppingCart.CustomShippingCost);
            order.IsTaxCostEdited = IsNotNull(shoppingCart.CustomTaxCost);
            order.Custom1 = shoppingCart.Custom1;
            order.Custom2 = shoppingCart.Custom2;
            order.Custom3 = shoppingCart.Custom3;
            order.Custom4 = shoppingCart.Custom4;
            order.Custom5 = shoppingCart.Custom5;
            order.PublishStateId = shoppingCart.PublishStateId;
            order.EstimateShippingCost = shoppingCart.EstimateShippingCost;
            order.IpAddress = shoppingCart.IpAddress;
            order.InHandDate = shoppingCart.InHandDate;
            order.JobName = shoppingCart.JobName;
            order.ShippingConstraintCode = shoppingCart.ShippingConstraintCode;
            order.TaxMessageList = order.TaxMessageList ?? shoppingCart?.AddressCarts?.FirstOrDefault()?.TaxMessageList;
            order.TaxSummaryList = order.TaxSummaryList ?? shoppingCart?.AddressCarts?.FirstOrDefault()?.TaxSummaryList;
            order.PaymentDisplayName = shoppingCart?.Payment?.PaymentDisplayName;
            order.PaymentExternalId = shoppingCart?.Payment?.PaymentExternalId;
            order.ReturnCharges = shoppingCart.ReturnCharges;
            order.IsCalculateTaxAfterDiscount = shoppingCart.IsCalculateTaxAfterDiscount;
            order.IsPricesInclusiveOfTaxes = shoppingCart.IsPricesInclusiveOfTaxes;
            foreach (ZnodeCoupon coupon in shoppingCart.Coupons)
            {
                if (coupon.CouponApplied && coupon.CouponValid)
                {
                    order.CouponCode = (!string.IsNullOrEmpty(order.CouponCode)) ? order.CouponCode += ZnodeConstant.CouponCodeSeparator + coupon.Coupon : coupon.Coupon;
                }
            }

            if(shoppingCart?.Vouchers?.Count > 0)
            {
                order.GiftCardNumber = string.Empty;
                foreach (ZnodeVoucher voucher in shoppingCart.Vouchers)
                {
                    if (voucher.IsVoucherApplied && voucher.IsVoucherValid || (!string.IsNullOrEmpty(voucher.VoucherNumber)))
                    {
                        order.GiftCardNumber = (!string.IsNullOrEmpty(order.GiftCardNumber)) ? order.GiftCardNumber += ZnodeConstant.CouponCodeSeparator + voucher.VoucherNumber : voucher.VoucherNumber;
                    }
                }
            }
            SetOrderModel(order, shoppingCart);
        }

        //to set order model to be insert into database
        public virtual void SetOrderModel(ZnodeOrderFulfillment order, IZnodePortalCart shoppingCart)
        {
            OrderModel model = new OrderModel();
            model.UserId = order.UserID;
            model.CreatedDate = GetDateTime();
            model.ModifiedDate = GetDateTime();
            model.OrderDate = order.OrderDate;
            model.CreatedBy = order.UserID;
            model.ModifiedBy = order.UserID;
            model.TaxCost = order.TaxCost;
            model.BillingAddress.EmailAddress = order.Email;
            model.ShippingCost = order.ShippingCost;
            model.ShippingDifference = order.ShippingDifference;
            model.SubTotal = order.SubTotal;
            model.Total = order.Total;
            model.OrderTotalWithoutVoucher = order.OrderTotalWithoutVoucher;
            model.OverDueAmount = order.OrderOverDueAmount;
            model.DiscountAmount = order.DiscountAmount;
            model.BillingAddress.CityName = order.BillingAddress?.CityName;
            model.BillingAddress.DisplayName = order.BillingAddress?.DisplayName;
            model.BillingAddress.CountryName = order.BillingAddress?.CountryName;
            model.BillingAddress.FirstName = order.BillingAddress?.FirstName;
            model.BillingAddress.LastName = order.BillingAddress?.LastName;
            model.BillingAddress.PhoneNumber = order.BillingAddress?.PhoneNumber;
            model.BillingAddress.PostalCode = order.BillingAddress?.PostalCode;
            model.BillingAddress.StateCode = order.BillingAddress?.StateName;
            model.BillingAddress.Address1 = order.BillingAddress?.Address1;
            model.BillingAddress.Address2 = order.BillingAddress?.Address2;
            model.BillingAddress.CompanyName = order.BillingAddress.CompanyName;            
            model.AdditionalInstructions = order.AdditionalInstructions;
            model.CouponCode = order.CouponCode;
            model.PortalId = order.PortalId;
            model.IsActive = true;
            model.PaymentTransactionToken = order.PaymentTransactionToken;
            model.CurrencyCode = order.CurrencyCode;
            model.CultureCode = order.CultureCode;
            model.PublishStateId = order.PublishStateId;
            //Set order tax to order.
            model.SalesTax = order.SalesTax;
            model.GST = order.GST;
            model.HST = order.HST;
            model.VAT = order.VAT;
            model.PST = order.PST;
            model.ImportDuty = order.ImportDuty;
            model.TaxSummaryList = order.TaxSummaryList;
            model.CreditCardNumber = order.CreditCardNumber;
            model.CardType = order.CardType;
            model.CreditCardExpMonth = order.CreditCardExpMonth;
            model.CreditCardExpYear = order.CreditCardExpYear;
            model.IsShippingCostEdited = order.IsShippingCostEdited;
            model.IsTaxCostEdited = order.IsTaxCostEdited;
            model.ExternalId = order.ExternalId;
            model.Custom1 = order.Custom1;
            model.Custom2 = order.Custom2;
            model.Custom3 = order.Custom3;
            model.Custom4 = order.Custom4;
            model.Custom5 = order.Custom5;
            model.EstimateShippingCost = order.EstimateShippingCost;

            model.PaymentDisplayName = order.PaymentDisplayName;
            model.PaymentExternalId = order.PaymentExternalId;
            model.IpAddress = order.IpAddress;
            model.InHandDate = order.InHandDate;
            model.JobName = order.JobName;
            model.ShippingConstraintCode = order.ShippingConstraintCode;
            model.AvataxIsSellerImporterOfRecord = shoppingCart.AvataxIsSellerImporterOfRecord;
            //to set gift card discount to order model
            decimal discountMultiplier = 0;
            if (!string.IsNullOrEmpty(order.GiftCardNumber))
            {
                SetOrderDiscount(model, order.GiftCardAmount, order.GiftCardNumber, (int)OrderDiscountTypeEnum.GIFTCARD, DiscountLevelTypeIdEnum.VoucherLevel, discountMultiplier);
            }

            //to set gift csr discount to order model
            if (order.CSRDiscountAmount > 0)
            {
                discountMultiplier = shoppingCart.GetDiscountMultiplier(order.CSRDiscountAmount); ;
                SetOrderDiscount(model, order.CSRDiscountAmount, OrderDiscountTypeEnum.CSRDISCOUNT.ToString(), (int)OrderDiscountTypeEnum.CSRDISCOUNT, DiscountLevelTypeIdEnum.CSRLevel, discountMultiplier);
            }

            
            order.Order.TotalAdditionalCost = order.Cart.TotalAdditionalCost;
            model.GiftCardNumber = order.GiftCardNumber;
            model.ShippingDiscount = order.ShippingDiscount;
            model.ShippingHandlingCharges = order.ShippingHandlingCharges;
            model.ReturnCharges = order.ReturnCharges;
            model.IsCalculateTaxAfterDiscount = order.IsCalculateTaxAfterDiscount;           
            order.Order = model;
        }

        //to set order lineitems 
        public virtual void SetOrderLineItems(ZnodeOrderFulfillment order, IZnodeMultipleAddressCart addressCart)
        {
            GetDistinctCategoryIdsforCartItem(addressCart);
            List<Data.DataModel.ZnodeOmsOrderLineItem> lineItemShippingDateList = orderHelper.GetLineItemShippingDate(addressCart?.ShoppingCartItems?.Cast<ZnodeShoppingCartItem>()?.Select(x => x.ParentOmsOrderLineItemsId)?.ToList());
            DateTime? ShipDate = DateTime.Now;
            // loop through cart and add line items
            List<ZnodeOmsOrderLineItem> omslineItems = null;
            if (addressCart?.OrderId > 0)
            {
                omslineItems = orderHelper.GetBundleOrderLineItemByOmsOrderId(addressCart.OrderId.Value);
            }
            foreach (ZnodeShoppingCartItem shoppingCartItem in addressCart.ShoppingCartItems)
            {
                OrderLineItemModel orderLineItem;

                if (string.IsNullOrEmpty(shoppingCartItem.GroupId))
                {
                    orderLineItem = order.OrderLineItems.FirstOrDefault(oli => oli.GroupId == shoppingCartItem.GroupId && oli.Sku == shoppingCartItem.SKU && shoppingCartItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Simple);
                }
                else
                {
                    orderLineItem = order.OrderLineItems.FirstOrDefault(oli => oli.GroupId == shoppingCartItem.GroupId && oli.Sku == shoppingCartItem.Product.SKU && shoppingCartItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Simple);
                }

                bool addNewOrderItem = false;
                if (IsNull(orderLineItem))
                {
                    addNewOrderItem = true;
                    orderLineItem = new OrderLineItemModel();
                    orderLineItem.OmsOrderShipmentId = addressCart.OrderShipmentID;
                    if (string.IsNullOrEmpty(shoppingCartItem.Product.ShoppingCartDescription) || string.IsNullOrWhiteSpace(shoppingCartItem.Product.ShoppingCartDescription))
                        if (string.IsNullOrEmpty(shoppingCartItem.Product.Description) || string.IsNullOrWhiteSpace(shoppingCartItem.Product.Description))
                            orderLineItem.Description = shoppingCartItem.Description;
                        else
                            orderLineItem.Description = shoppingCartItem.Product.Description;
                    else
                        orderLineItem.Description = shoppingCartItem.Description;
                    orderLineItem.ProductName = shoppingCartItem.Product.Name;
                    orderLineItem.Sku = shoppingCartItem.Product.SKU;
                    orderLineItem.Quantity = ((shoppingCartItem?.Product?.ZNodeConfigurableProductCollection.Count > 0) || (shoppingCartItem?.Product?.ZNodeGroupProductCollection.Count > 0)) ? 0 : shoppingCartItem.Quantity;
                    orderLineItem.Price = (shoppingCartItem?.Product?.ZNodeConfigurableProductCollection.Count > 0) ? 0 : GetParentProductPrice(shoppingCartItem);
                    orderLineItem.DiscountAmount = GetLineItemDiscountAmount(shoppingCartItem.Product.DiscountAmount, shoppingCartItem.Quantity);
                    orderLineItem.ShipSeparately = shoppingCartItem.Product.ShipSeparately;
                    orderLineItem.ParentOmsOrderLineItemsId = null;
                    orderLineItem.DownloadLink = shoppingCartItem.Product.DownloadLink;
                    orderLineItem.GroupId = shoppingCartItem.GroupId;
                    orderLineItem.IsActive = true;
                    orderLineItem.Vendor = shoppingCartItem.Product.VendorCode;
                    orderLineItem.OrderLineItemStateId = shoppingCartItem.OrderStatusId;
                    orderLineItem.IsItemStateChanged = shoppingCartItem.IsItemStateChanged;
                    if (string.Equals(shoppingCartItem?.OrderStatus, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.OrdinalIgnoreCase) && shoppingCartItem.IsItemStateChanged)
                        orderLineItem.ShipDate = ShipDate;
                    else if (string.Equals(shoppingCartItem.OrderStatus, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.OrdinalIgnoreCase))
                        orderLineItem.ShipDate = lineItemShippingDateList?.Count > 0 ? lineItemShippingDateList.Find(x => x.OmsOrderLineItemsId == shoppingCartItem.ParentOmsOrderLineItemsId)?.ShipDate : null;
                    if (!string.IsNullOrEmpty(shoppingCartItem.TrackingNumber))
                        orderLineItem.TrackingNumber = shoppingCartItem.TrackingNumber;
                    //to apply custom tax/shipping cost
                    orderLineItem.IsLineItemShippingCostEdited = order.IsShippingCostEdited;
                    orderLineItem.IsLineItemTaxCostEdited = order.IsTaxCostEdited;
                    orderLineItem.PartialRefundAmount = shoppingCartItem.PartialRefundAmount;
                    orderLineItem.ProductLevelTax = shoppingCartItem.ProductLevelTax;
                    //Assign Auto-add-on SKUs.
                    orderLineItem.AutoAddonSku = string.IsNullOrEmpty(shoppingCartItem.AutoAddonSKUs) ? null : shoppingCartItem.AutoAddonSKUs;

                    //to set order line item attributes
                    orderLineItem.Attributes = SetLineItemAttributes(shoppingCartItem?.Product?.Attributes, shoppingCartItem?.Product.ProductCategoryIds, shoppingCartItem);

                    // then make a shipping cost entry in orderlineItem table.             
                    orderLineItem.ShippingCost = orderLineItem.IsLineItemShippingCostEdited ? 0 : shoppingCartItem.ShippingCost;

                    //Set order tax to order line item.
                    orderLineItem.HST = orderLineItem.IsLineItemTaxCostEdited ? 0 : shoppingCartItem.Product.HST;
                    orderLineItem.PST = orderLineItem.IsLineItemTaxCostEdited ? 0 : shoppingCartItem.Product.PST;
                    orderLineItem.GST = orderLineItem.IsLineItemTaxCostEdited ? 0 : shoppingCartItem.Product.GST;
                    orderLineItem.VAT = orderLineItem.IsLineItemTaxCostEdited ? 0 : shoppingCartItem.Product.VAT;
                    orderLineItem.SalesTax = orderLineItem.IsLineItemTaxCostEdited ? 0 : shoppingCartItem.Product.SalesTax;
                    orderLineItem.ImportDuty = orderLineItem.IsLineItemTaxCostEdited ? 0 : shoppingCartItem.Product.ImportDuty;
                    orderLineItem.TaxTransactionNumber = shoppingCartItem.TaxTransactionNumber;
                    orderLineItem.TaxRuleId = shoppingCartItem.TaxRuleId;
                    orderLineItem.IsConfigurableProduct = shoppingCartItem.Product.ZNodeConfigurableProductCollection.Count > 0 ? true : false;
                    orderLineItem.Custom1 = shoppingCartItem.Custom1;
                    orderLineItem.Custom2 = shoppingCartItem.Custom2;
                    orderLineItem.Custom3 = shoppingCartItem.Custom3;
                    orderLineItem.Custom4 = shoppingCartItem.Custom4;
                    orderLineItem.Custom5 = shoppingCartItem.Custom5;
                    if (shoppingCartItem?.Product?.ZNodeBundleProductCollection.Count > 0)
                    {
                        orderLineItem.ProductType = ZnodeConstant.BundleProduct;
                    }
                    if (shoppingCartItem.Product.RecurringBillingInd)
                    {
                        orderLineItem.RecurringBillingAmount = shoppingCartItem.Product.RecurringBillingInitialAmount;
                        orderLineItem.RecurringBillingCycles = shoppingCartItem.Product.RecurringBillingTotalCycles;
                        orderLineItem.RecurringBillingFrequency = shoppingCartItem.Product.RecurringBillingFrequency;
                        orderLineItem.RecurringBillingPeriod = shoppingCartItem.Product.RecurringBillingPeriod;
                        orderLineItem.IsRecurringBilling = true;
                    }

                    orderLineItem.OrdersDiscount = shoppingCartItem.Product.OrdersDiscount;
                    //To add personalize attribute list
                    orderLineItem.PersonaliseValueList = shoppingCartItem.PersonaliseValuesList;
                    orderLineItem.PersonaliseValuesDetail = shoppingCartItem.PersonaliseValuesDetail;
                    orderLineItem.AdditionalCost = shoppingCartItem.AdditionalCost;
                    orderLineItem.OmsOrderLineItemsId = shoppingCartItem.OmsOrderLineItemId;
                }

                AddSimpleItemInOrderLineItem(orderLineItem, shoppingCartItem);

                //to add add-on items in order line item
                AddAddOnsItemsInOrderLineItem(orderLineItem, shoppingCartItem);

                //to add bundle items in order line item
                AddBundleItemsInOrderLineItem(orderLineItem, shoppingCartItem, omslineItems);

                //to add Configurable item in order line item                
                AddConfigurableItemsInOrderLineItem(orderLineItem, shoppingCartItem);

                //to add Group item in order line item
                AddGroupItemsInOrderLineItem(orderLineItem, shoppingCartItem);

                //To add personalise attribute list
                orderLineItem.PersonaliseValueList = shoppingCartItem.PersonaliseValuesList;

                if (addNewOrderItem)
                    order.OrderLineItems.Add(orderLineItem);
            }
        }

        //to add add-on items in order line item
        public virtual void AddAddOnsItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem)
        {
            foreach (ZnodeProductBaseEntity addonItems in shoppingCartItem.Product.ZNodeAddonsProductCollection)
            {
                AddLineItemsInOrderLineItem(orderLineItem, addonItems, shoppingCartItem, ZnodeCartItemRelationshipTypeEnum.AddOns);
            }
        }

        //to add bundle items in order line item
        public virtual void AddBundleItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem, List<ZnodeOmsOrderLineItem> omslineItems=null)
        {
            foreach (ZnodeProductBaseEntity bundleItems in shoppingCartItem.Product.ZNodeBundleProductCollection)
            {
                AddLineItemsInOrderLineItem(orderLineItem, bundleItems, shoppingCartItem, ZnodeCartItemRelationshipTypeEnum.Bundles, omslineItems);
            }
        }

        //to add configurable items in order line item
        public virtual void AddConfigurableItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem)
        {
            foreach (ZnodeProductBaseEntity configurableItems in shoppingCartItem.Product.ZNodeConfigurableProductCollection)
            {
                AddLineItemsInOrderLineItem(orderLineItem, configurableItems, shoppingCartItem, ZnodeCartItemRelationshipTypeEnum.Configurable);
            }
        }

        public virtual void AddSimpleItemInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Simple)
            {
                AddSimpleProductLineItemsInOrderLineItem(orderLineItem, ZnodeCartItemRelationshipTypeEnum.Simple);
            }
        }

        //to add Group items in order line item
        public virtual void AddGroupItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem)
        {
            foreach (ZnodeProductBaseEntity groupItems in shoppingCartItem.Product.ZNodeGroupProductCollection)
            {
                AddLineItemsInOrderLineItem(orderLineItem, groupItems, shoppingCartItem, ZnodeCartItemRelationshipTypeEnum.Group);
            }
        }

        //to save the referral commission only if order is placed using affiliate account.
        public virtual bool SaveReferralCommission(ZnodeOrderFulfillment order)
        {
            bool isSaved = false;
            if (IsNotNull(order.ReferralUserId) && order.ReferralUserId > 0)
            {
                decimal commission = (order.SubTotal - (order.DiscountAmount + order.GiftCardAmount));
                return orderHelper.SaveReferralCommission(order.ReferralUserId, order?.Order?.OmsOrderDetailsId ?? 0, string.Empty, commission);
            }
            return isSaved;
        }

        // To manage order inventory         
        public virtual bool ManageOrderInventory(ZnodeOrderFulfillment order, IZnodePortalCart shoppingCart)
        {
            bool isSuccess = false;
            if (IsNotNull(order) && order?.OrderLineItems.Count > 0)
            {
                OrderWarehouseModel orderWarehouse = new OrderWarehouseModel();
                orderWarehouse.OrderId = order.Order.OmsOrderDetailsId;
                orderWarehouse.UserId = order.UserID;
                orderWarehouse.PortalId = order.PortalId;

                List<OrderWarehouseLineItemsModel> skusInventory = SetSKUInventorySetting(shoppingCart);
                foreach (OrderLineItemModel item in order.OrderLineItems)
                {
                    bool allowBackOrder = false;
                    string inventoryTracking = GetInventoryTrackingBySKU(item.Sku, skusInventory, out allowBackOrder);
                    if (item.OrderLineItemCollection.FirstOrDefault()?.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Bundles)
                        orderWarehouse.LineItems.Add(new OrderWarehouseLineItemsModel { OrderLineItemId = item.OmsOrderLineItemsId, SKU = item.Sku, Quantity = item.Quantity, InventoryTracking = inventoryTracking, AllowBackOrder = allowBackOrder });

                    foreach (OrderLineItemModel childitem in item.OrderLineItemCollection)
                    {
                        if (item?.ProductType == ZnodeConstant.BundleProduct && shoppingCart.OrderId > 0)
                        {
                            inventoryTracking = childitem?.Attributes?.FirstOrDefault(d => d.AttributeCode == ZnodeConstant.OutOfStockOptions)?.AttributeValue;
                        }
                        else
                        {
                            inventoryTracking = GetInventoryTrackingBySKU(childitem.Sku, skusInventory, out allowBackOrder);
                        }
                        orderWarehouse.LineItems.Add(new OrderWarehouseLineItemsModel { OrderLineItemId = childitem.OmsOrderLineItemsId, SKU = childitem.Sku, Quantity = childitem.Quantity, InventoryTracking = inventoryTracking, AllowBackOrder = allowBackOrder });
                    }
                }
                List<OrderWarehouseLineItemsModel> productInventoryList = new List<OrderWarehouseLineItemsModel>();

                isSuccess = orderHelper.ManageOrderInventory(orderWarehouse, out productInventoryList);

                GetLowInventoryProducts(shoppingCart, productInventoryList);
            }
            return isSuccess;
        }

        // To manage order inventory       
        public virtual OrderWarehouseModel GetOrderInventory(ZnodeOrderFulfillment order, IZnodePortalCart shoppingCart)
        {
            OrderWarehouseModel orderWarehouse = new OrderWarehouseModel();

            if (IsNotNull(order) && order?.OrderLineItems.Count > 0)
            {
                orderWarehouse.OrderId = order.Order.OmsOrderDetailsId;
                orderWarehouse.UserId = order.UserID;
                orderWarehouse.PortalId = order.PortalId;

                List<OrderWarehouseLineItemsModel> skusInventory = SetSKUInventorySetting(shoppingCart);
                foreach (OrderLineItemModel item in order.OrderLineItems)
                {
                    bool allowBackOrder = false;
                    string inventoryTracking = GetInventoryTrackingBySKU(item.Sku, skusInventory, out allowBackOrder);

                    foreach (OrderLineItemModel childitem in item.OrderLineItemCollection)
                    {
                        if (item?.ProductType == ZnodeConstant.BundleProduct && shoppingCart.OrderId > 0)
                        {
                            inventoryTracking = childitem?.Attributes?.FirstOrDefault(d => d.AttributeCode == ZnodeConstant.OutOfStockOptions)?.AttributeValue;
                        }
                        else
                        {
                            inventoryTracking = GetInventoryTrackingBySKU(childitem.Sku, skusInventory, out allowBackOrder);
                        }
                        orderWarehouse.LineItems.Add(new OrderWarehouseLineItemsModel { OrderLineItemId = childitem.OmsOrderLineItemsId, SKU = childitem.Sku, Quantity = childitem.Quantity, InventoryTracking = inventoryTracking, AllowBackOrder = allowBackOrder });
                    }
                }
            }
            return orderWarehouse;
        }

        protected void GetLowInventoryProducts(IZnodePortalCart shoppingCart, List<OrderWarehouseLineItemsModel> productInventoryList)
        {
            shoppingCart.LowInventoryProducts = new List<OrderWarehouseLineItemsModel>();

            shoppingCart.LowInventoryProducts = productInventoryList?.Where(x => x.Quantity < 0 && x.AllowBackOrder == false)?.ToList();
        }

        //to add child line items in order line item model as per product type
        public virtual void AddLineItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeProductBaseEntity product, ZnodeShoppingCartItem shoppingCartItem, ZnodeCartItemRelationshipTypeEnum cartItemProductType, List<ZnodeOmsOrderLineItem> omslineItems=null)
        {
            OrderLineItemModel childLineItem = new OrderLineItemModel();
            childLineItem.OmsOrderShipmentId = orderLineItem.OmsOrderShipmentId;
            childLineItem.Description = orderLineItem.Description;
            childLineItem.ProductName = product.Name;
            childLineItem.Sku = product.SKU;
            childLineItem.Quantity = GetLineItemQuantity(cartItemProductType, product.SelectedQuantity, shoppingCartItem.Quantity);
            childLineItem.Price = IsNotNull(shoppingCartItem?.CustomUnitPrice) && !Equals(cartItemProductType, ZnodeCartItemRelationshipTypeEnum.Bundles) && !Equals(cartItemProductType, ZnodeCartItemRelationshipTypeEnum.AddOns) && !Equals(cartItemProductType, ZnodeCartItemRelationshipTypeEnum.Group) ? shoppingCartItem.CustomUnitPrice.GetValueOrDefault() : GetOrderLineItemPrice(cartItemProductType, product, shoppingCartItem);
            childLineItem.DiscountAmount = GetLineItemDiscountAmount(product.DiscountAmount, childLineItem.Quantity);
            childLineItem.ShipSeparately = product.ShipSeparately;
            childLineItem.ParentOmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId;
            childLineItem.OrderLineItemRelationshipTypeId = (int)cartItemProductType;
            childLineItem.DownloadLink = product.DownloadLink;
            childLineItem.IsActive = true;
            childLineItem.OrdersDiscount = product.OrdersDiscount;
            childLineItem.OrderLineItemStateId = orderLineItem.OrderLineItemStateId;
            childLineItem.TrackingNumber = orderLineItem.TrackingNumber;
            //Set order tax to order line item.
            childLineItem.HST = orderLineItem.IsLineItemTaxCostEdited ? 0 : product.HST;
            childLineItem.PST = orderLineItem.IsLineItemTaxCostEdited ? 0 : product.PST;
            childLineItem.GST = orderLineItem.IsLineItemTaxCostEdited ? 0 : product.GST;
            childLineItem.VAT = orderLineItem.IsLineItemTaxCostEdited ? 0 : product.VAT;
            childLineItem.SalesTax = orderLineItem.IsLineItemTaxCostEdited ? 0 : product.SalesTax;
            childLineItem.ImportDuty = orderLineItem.IsLineItemTaxCostEdited ? 0 : product.ImportDuty;
            childLineItem.TaxTransactionNumber = orderLineItem.TaxTransactionNumber;
            childLineItem.TaxRuleId = orderLineItem.TaxRuleId;
            childLineItem.PartialRefundAmount = orderLineItem.PartialRefundAmount;
            childLineItem.Custom1 = shoppingCartItem.Custom1;
            childLineItem.Custom2 = shoppingCartItem.Custom2;
            childLineItem.Custom3 = shoppingCartItem.Custom3;
            childLineItem.Custom4 = shoppingCartItem.Custom4;
            childLineItem.Custom5 = shoppingCartItem.Custom5;
            childLineItem.ParentProductSKU = shoppingCartItem.ParentProductSKU;
            SetChildItemAttributes(product, shoppingCartItem, cartItemProductType, childLineItem, omslineItems);
            childLineItem.PaymentStatusId = shoppingCartItem.PaymentStatusId;
            if (product.ShipSeparately)
                childLineItem.ShippingCost = orderLineItem.IsLineItemShippingCostEdited ? 0 : product.ShippingCost;

            if (product.RecurringBillingInd)
            {
                childLineItem.RecurringBillingAmount = product.RecurringBillingInitialAmount;
                childLineItem.RecurringBillingCycles = product.RecurringBillingTotalCycles;
                childLineItem.RecurringBillingFrequency = product.RecurringBillingFrequency;
                childLineItem.RecurringBillingPeriod = product.RecurringBillingPeriod;
                childLineItem.IsRecurringBilling = true;
            }
            orderLineItem.OrderLineItemCollection.Add(childLineItem);
        }

        protected virtual void SetChildItemAttributes(ZnodeProductBaseEntity product, ZnodeShoppingCartItem shoppingCartItem, ZnodeCartItemRelationshipTypeEnum cartItemProductType, OrderLineItemModel childLineItem, List<ZnodeOmsOrderLineItem> omslineItems)
        {
            if (cartItemProductType == ZnodeCartItemRelationshipTypeEnum.Bundles)
            {
                string inventoryTracking = string.Empty;
                if (shoppingCartItem.OmsOrderId > 0)
                {
                    inventoryTracking = omslineItems?.FirstOrDefault(d => d.Sku == product.SKU)?.ZnodeOmsOrderAttributes?.FirstOrDefault(d => d.AttributeCode == "OutOfStockOptions")?.AttributeValue;
                }
                childLineItem.Attributes = SetBundleChildLineItemAttributes(product?.Attributes, product?.ProductCategoryIds, shoppingCartItem, inventoryTracking);
            }
            else
            {
                childLineItem.Attributes = SetLineItemAttributes(product?.Attributes, product?.ProductCategoryIds);
            }
        }

        //to add child line items in order line item model as per product type
        public virtual void AddSimpleProductLineItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeCartItemRelationshipTypeEnum cartItemProductType)
        {
            OrderLineItemModel childLineItem = new OrderLineItemModel();
            childLineItem.OmsOrderShipmentId = orderLineItem.OmsOrderShipmentId;
            childLineItem.Description = orderLineItem.Description;
            childLineItem.ProductName = orderLineItem.ProductName;
            childLineItem.Sku = orderLineItem.Sku;
            childLineItem.Quantity = orderLineItem.Quantity;
            childLineItem.Price = orderLineItem.Price;
            childLineItem.DiscountAmount = orderLineItem.DiscountAmount;
            childLineItem.ShipSeparately = orderLineItem.ShipSeparately;
            childLineItem.ParentOmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId;
            childLineItem.OrderLineItemRelationshipTypeId = (int)cartItemProductType;
            childLineItem.DownloadLink = orderLineItem.DownloadLink;
            childLineItem.IsActive = true;
            childLineItem.OrdersDiscount = orderLineItem.OrdersDiscount;
            childLineItem.OrderLineItemStateId = orderLineItem.OrderLineItemStateId;
            childLineItem.TrackingNumber = orderLineItem.TrackingNumber;
            //Set order tax to order line item.
            childLineItem.HST = orderLineItem.IsLineItemTaxCostEdited ? 0 : orderLineItem.HST;
            childLineItem.PST = orderLineItem.IsLineItemTaxCostEdited ? 0 : orderLineItem.PST;
            childLineItem.GST = orderLineItem.IsLineItemTaxCostEdited ? 0 : orderLineItem.GST;
            childLineItem.VAT = orderLineItem.IsLineItemTaxCostEdited ? 0 : orderLineItem.VAT;
            childLineItem.ImportDuty = orderLineItem.IsLineItemTaxCostEdited ? 0 : orderLineItem.ImportDuty;
            childLineItem.SalesTax = orderLineItem.IsLineItemTaxCostEdited ? 0 : orderLineItem.SalesTax;
            childLineItem.TaxTransactionNumber = orderLineItem.TaxTransactionNumber;
            childLineItem.TaxRuleId = orderLineItem.TaxRuleId;
            childLineItem.PartialRefundAmount = orderLineItem.PartialRefundAmount;
            childLineItem.Custom1 = orderLineItem.Custom1;
            childLineItem.Custom2 = orderLineItem.Custom2;
            childLineItem.Custom3 = orderLineItem.Custom3;
            childLineItem.Custom4 = orderLineItem.Custom4;
            childLineItem.Custom5 = orderLineItem.Custom5;

            childLineItem.Attributes = orderLineItem.Attributes;

            childLineItem.ShippingCost = orderLineItem.ShippingCost;


            childLineItem.RecurringBillingAmount = orderLineItem.RecurringBillingAmount;
            childLineItem.RecurringBillingCycles = orderLineItem.RecurringBillingCycles;
            childLineItem.RecurringBillingFrequency = orderLineItem.RecurringBillingFrequency;
            childLineItem.RecurringBillingPeriod = orderLineItem.RecurringBillingPeriod;
            childLineItem.IsRecurringBilling = orderLineItem.IsRecurringBilling;

            childLineItem.PersonaliseValueList = orderLineItem.PersonaliseValueList;
            childLineItem.PersonaliseValuesDetail = orderLineItem.PersonaliseValuesDetail;
            childLineItem.ProductImagePath = orderLineItem.ProductImagePath;
            childLineItem.PaymentStatusId = orderLineItem.PaymentStatusId;
            orderLineItem.OrderLineItemCollection.Add(childLineItem);
        }

        //to get main product price depends upon product type
        public virtual decimal GetParentProductPrice(ZnodeShoppingCartItem shoppingCartItem)
        {
            decimal price = shoppingCartItem.UnitPrice - (shoppingCartItem?.Product?.AddOnPrice ?? 0);

            //if parent product price set to null or zero then associate configurable product price else  parent product price 
            if (GetProductPrice(shoppingCartItem.Product).Equals(0))
                price = price - (shoppingCartItem?.Product?.ConfigurableProductPrice ?? 0);

            //if product type is group then price will be zero 
            if (shoppingCartItem?.Product?.GroupProductPrice > 0)
                return 0;

            return price;
        }

        //to set line item price as per cartitem producttype
        public virtual decimal GetOrderLineItemPrice(ZnodeCartItemRelationshipTypeEnum cartItemProductType, ZnodeProductBaseEntity product, ZnodeShoppingCartItem shoppingCartItem)
        {
            decimal? price = 0;
            switch (cartItemProductType)
            {
                case ZnodeCartItemRelationshipTypeEnum.AddOns:
                    return product.FinalPrice;
                case ZnodeCartItemRelationshipTypeEnum.Bundles:
                    return 0;
                case ZnodeCartItemRelationshipTypeEnum.Configurable:
                    price = GetParentProductPrice(shoppingCartItem);
                    price = price > 0 ? price : shoppingCartItem?.Product?.ConfigurableProductPrice;
                    return price.GetValueOrDefault();
                case ZnodeCartItemRelationshipTypeEnum.Group:
                    return product.FinalPrice;
                default:
                    return product.FinalPrice;
            }
        }

        //to product price if SalePrice is present else RetailPrice
        public virtual decimal GetProductPrice(ZnodeProductBaseEntity product)
       => product.SalePrice > 0 ? product.SalePrice.GetValueOrDefault() : product.RetailPrice;

        //to get line item quantity
        public virtual decimal GetLineItemQuantity(ZnodeCartItemRelationshipTypeEnum productType, decimal groupProductQuantity, decimal cartQuantity)
       => productType.Equals(ZnodeCartItemRelationshipTypeEnum.Group) ? groupProductQuantity : productType.Equals(ZnodeCartItemRelationshipTypeEnum.Bundles) ? (groupProductQuantity * cartQuantity) : cartQuantity;

        //to get inventory tracking of sku by supplied sku
        public virtual string GetInventoryTrackingBySKU(string sku, List<OrderWarehouseLineItemsModel> skusInventory, out bool allowBackOrder)
        {
            allowBackOrder = skusInventory.FirstOrDefault(c => c.SKU == sku).AllowBackOrder;
            return skusInventory.FirstOrDefault(c => c.SKU == sku).InventoryTracking;
        }

        //to set all items in shopping cart to dictionary
        public virtual List<OrderWarehouseLineItemsModel> SetSKUInventorySetting(IZnodePortalCart shoppingCart)
        {
            List<OrderWarehouseLineItemsModel> SKUInventorySetting = new List<OrderWarehouseLineItemsModel>();
            foreach (ZnodeShoppingCartItem item in shoppingCart.ShoppingCartItems)
            {
                AddSkuInventoryTracking(item.Product, SKUInventorySetting);

                foreach (ZnodeProductBaseEntity addon in item.Product.ZNodeAddonsProductCollection)
                {
                    AddSkuInventoryTracking(addon, SKUInventorySetting);
                }

                foreach (ZnodeProductBaseEntity bundle in item.Product.ZNodeBundleProductCollection)
                {
                    AddSkuInventoryTracking(bundle, SKUInventorySetting);
                }

                foreach (ZnodeProductBaseEntity config in item.Product.ZNodeConfigurableProductCollection)
                {
                    AddSkuInventoryTracking(config, SKUInventorySetting, item.Product.AllowBackOrder);
                }

                foreach (ZnodeProductBaseEntity group in item.Product.ZNodeGroupProductCollection)
                {
                    AddSkuInventoryTracking(group, SKUInventorySetting);
                }
            }
            return SKUInventorySetting;
        }

        //to add Sku and its inventorytracking in dictionary
        public virtual void AddSkuInventoryTracking(ZnodeProductBaseEntity product, List<OrderWarehouseLineItemsModel> skuInventoryTracking, bool allowBackOrder = false)
        {
            if (skuInventoryTracking.Where(x => x.SKU == product.SKU).Count() == 0)
            {
                skuInventoryTracking.Add(new OrderWarehouseLineItemsModel { SKU = product.SKU, InventoryTracking = product.InventoryTracking, AllowBackOrder = allowBackOrder ? allowBackOrder : product.AllowBackOrder });
            }
        }

        //to get line item discount amount based on quantity
        public virtual decimal GetLineItemDiscountAmount(decimal discountAmount, decimal quantity)
           => (discountAmount * quantity);

        //to set payment details 
        public virtual bool SetPaymentDetails(ZnodeOrderFulfillment order)
        {
            ZnodeLogging log = new ZnodeLogging();
            //bool isPreAuthorize = _ShoppingCart.Payment.IsPreAuthorize;
            log.LogActivityTimerStart();

            string paymentStatus = Enum.GetName(typeof(ZnodePaymentStatus), order.PaymentStatusID);

            if (!string.Equals(ShoppingCart?.Payment?.PaymentTypeName, ZnodeConstant.PAYPAL_EXPRESS, StringComparison.OrdinalIgnoreCase))
                paymentStatus = GetPaymentType(ShoppingCart?.Payment?.PaymentTypeName);

            // update transaction id and status
            order.PaymentTransactionToken = ShoppingCart.Token;
            if (order.OrderID == 0 || order.OrderOverDueAmount > 0)
                order.PaymentStatusID = orderHelper.GetPaymentStatusId(paymentStatus);

            order.PaymentTypeId = ShoppingCart.Payment.PaymentTypeId;
            order.PaymentSettingID = ShoppingCart.Payment.PaymentSettingId;

            return IsNotNull(order.PaymentStatusID);
        }

        //to Save giftcardhistory &  the referral commission
        public virtual void SaveReferralCommissionAndGiftCardHistory(ZnodeOrderFulfillment order, int? userId = 0)
        {
            // to apply voucher history 
            order.AddToVoucherHistory(order, userId);

            // Save the referral commission
            this.SaveReferralCommission(order);
        }

        //Set Order Shipment Details to order Line Item.
        public virtual void SetOrderShipmentDetails(ZnodeOrderFulfillment order, List<OrderShipmentModel> orderShipmentModel)
        {
            if(HelperUtility.IsNotNull(orderShipmentModel))
            {
                List<int> orderShipmentIds = ShoppingCart?.AddressCarts?.Select(x => x.OrderShipmentID)?.Distinct()?.ToList();
                // Check to get the order shipment for the item  
                if (orderShipmentIds?.Count > 0)
                {
                    List<OrderShipmentModel> orderShipments = orderShipmentModel?.Where(x => orderShipmentIds.Contains(x.OmsOrderShipmentId)).ToList();
                    if (IsNotNull(orderShipments) && orderShipments?.Count > 0)
                    {
                        List<OrderLineItemModel> orderLineItems = order.OrderLineItems?.Where(x => orderShipments.Select(y => y.OmsOrderShipmentId).Contains(x.OmsOrderShipmentId)).ToList();
                        if (orderLineItems?.Count > 0)
                        {
                            foreach (OrderLineItemModel orderLineItemModel in orderLineItems)
                                orderLineItemModel.ZnodeOmsOrderShipment = orderShipments.FirstOrDefault(x => x.OmsOrderShipmentId == orderLineItemModel.OmsOrderShipmentId);
                        }
                    }
                }
            }
        }

        // to set order additional details like shippingid,purchaseordernumber & referraluserid etc
        public virtual void SetOrderAdditionalDetails(ZnodeOrderFulfillment order, SubmitOrderModel model)
        {
            order.ShippingId = this.ShoppingCart.Shipping.ShippingID != 0 ? this.ShoppingCart.Shipping.ShippingID : this._ShippingID;
            order.AdditionalInstructions = this.AdditionalInstructions;
            order.PurchaseOrderNumber = this.PurchaseOrderNumber;
            order.PODocument = this.PoDocument;
            order.ReferralUserId = this.UserAccount.ReferralUserId;
            if (IsNotNull(order.Order) && model?.OrderId > 0)
            {
                order.Order.CreatedBy = model.CreatedBy;
                order.Order.ModifiedBy = model.ModifiedBy;
            }
        }

        //to cancel existing order
        public virtual bool CancelExistingOrder(ZnodeOrderFulfillment order, int orderId)
        {
            ZnodeLogging.LogMessage($"Cancel Order process is initiated for Order Id:{orderId }", ZnodeLogging.Components.OMS.ToString());

            //to get existing order total amount       
            decimal orderOldTotal = orderHelper.GetOrderTotalById(orderId);
            ZnodeLogging.LogMessage($"Order Cancel successfully :{orderId }", ZnodeLogging.Components.OMS.ToString());
            order.OrderOverDueAmount = (order.Total - orderOldTotal);
            order.OrderID = orderId;
            return true;
        }

        //to set line item attribute by code specified in order attribute to store in ZnodeOmsOrderAttribute
        public virtual List<OrderAttributeModel> SetBundleChildLineItemAttributes(List<OrderAttributeModel> allAttributes, int[] productCategoryIds, ZnodeShoppingCartItem shoppingCartItem, string IteminventoryTracking)
        {
            string attributeCodes = ShoppingCart.OrderAttribute;
            List<OrderAttributeModel> selectedAttributes = new List<OrderAttributeModel>();
            if (!string.IsNullOrEmpty(attributeCodes) && allAttributes?.Count > 0)
            {
                //to get all attributeCodes in string array 
                string[] attributeList = attributeCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                //to distinct attribute Codes & its value 
                var filteredAttributes = allAttributes.Where(x => attributeList.Contains(x.AttributeCode))?.Distinct()?.ToList();
                if (filteredAttributes?.Count > 0)
                    selectedAttributes.AddRange(filteredAttributes);
            }

            //to add category associated with product to line item
            if (productCategoryIds.Length > 0 && publishCategory.Count > 0)
            {
                foreach (int categoryId in productCategoryIds)
                {
                    selectedAttributes.Add(new OrderAttributeModel { AttributeCode = ZnodeConstant.CategoryName, AttributeValue = GetCategoryNameByCategoryId(categoryId), AttributeValueCode = ZnodeConstant.CategoryCode });
                }
            }
            //Set the Inventory Tracking.
            string inventoryTracking = SetInventoryTracking(allAttributes, shoppingCartItem, IteminventoryTracking);
            selectedAttributes.Add(new OrderAttributeModel { AttributeCode = ZnodeConstant.OutOfStockOptions, AttributeValue = inventoryTracking, AttributeValueCode = inventoryTracking });
            return selectedAttributes;
        }

        protected virtual string SetInventoryTracking(List<OrderAttributeModel> allAttributes, ZnodeShoppingCartItem shoppingCartItem, string IteminventoryTracking)
        {
            string inventoryTracking = allAttributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.AttributeValueCode;
            if (shoppingCartItem?.OmsOrderId > 0)
            {
                inventoryTracking = string.IsNullOrEmpty(IteminventoryTracking) ? inventoryTracking : IteminventoryTracking;
            }
            else
            {
                inventoryTracking = string.IsNullOrEmpty(inventoryTracking) ? ZnodeConstant.DontTrackInventory : inventoryTracking;
            }
            return inventoryTracking;
        }

        //to set line item attribute by code specified in order attribute to store in ZnodeOmsOrderAttribute
        public virtual List<OrderAttributeModel> SetLineItemAttributes(List<OrderAttributeModel> allAttributes, int[] productCategoryIds, ZnodeShoppingCartItem shoppingCartItem = null)
        {
            string attributeCodes = ShoppingCart.OrderAttribute;
            List<OrderAttributeModel> selectedAttributes = new List<OrderAttributeModel>();
            if (!string.IsNullOrEmpty(attributeCodes) && allAttributes?.Count > 0)
            {
                //to get all attributeCodes in string array 
                string[] attributeList = attributeCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                //to distinct attribute Codes & its value 
                var filteredAttributes = allAttributes.Where(x => attributeList.Contains(x.AttributeCode)).Distinct().ToList();
                if (filteredAttributes?.Count > 0)
                    selectedAttributes.AddRange(filteredAttributes);
            }

            //to add category associated with product to line item
            if (productCategoryIds.Length > 0 && publishCategory.Count > 0)
            {
                foreach (int categoryId in productCategoryIds)
                {
                    selectedAttributes.Add(new OrderAttributeModel { AttributeCode = ZnodeConstant.CategoryName, AttributeValue = GetCategoryNameByCategoryId(categoryId), AttributeValueCode = ZnodeConstant.CategoryCode });
                }
            }
            string inventoryTracking = allAttributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.AttributeValueCode;
            if (shoppingCartItem?.OmsOrderId > 0)
            {
                inventoryTracking = string.IsNullOrEmpty(shoppingCartItem.InventoryTracking) ? inventoryTracking : shoppingCartItem.InventoryTracking;
            }
            else
            {
                inventoryTracking = string.IsNullOrEmpty(inventoryTracking) ? ZnodeConstant.DontTrackInventory : inventoryTracking;
            }
            selectedAttributes.Add(new OrderAttributeModel { AttributeCode = ZnodeConstant.OutOfStockOptions, AttributeValue = inventoryTracking, AttributeValueCode = inventoryTracking });
            return selectedAttributes;
        }

        //to get all product and its child item category ids
        public virtual void GetDistinctCategoryIdsforCartItem(IZnodeMultipleAddressCart addressCart)
        {
            List<int> _productCategoryIds = new List<int>();
            // loop through cart and add line items
            foreach (ZnodeShoppingCartItem shoppingCartItem in addressCart.ShoppingCartItems)
            {
                GetProductCategoryIds(shoppingCartItem.Product.ProductCategoryIds, _productCategoryIds);

                foreach (ZnodeProductBaseEntity addon in shoppingCartItem.Product.ZNodeAddonsProductCollection)
                {
                    GetProductCategoryIds(addon.ProductCategoryIds, _productCategoryIds);
                }

                foreach (ZnodeProductBaseEntity bundle in shoppingCartItem.Product.ZNodeBundleProductCollection)
                {
                    GetProductCategoryIds(bundle.ProductCategoryIds, _productCategoryIds);
                }

                foreach (ZnodeProductBaseEntity config in shoppingCartItem.Product.ZNodeConfigurableProductCollection)
                {
                    GetProductCategoryIds(config.ProductCategoryIds, _productCategoryIds);
                }

                foreach (ZnodeProductBaseEntity group in shoppingCartItem.Product.ZNodeGroupProductCollection)
                {
                    GetProductCategoryIds(group.ProductCategoryIds, _productCategoryIds);
                }
            }

            //to get category name by category Id
            if (IsNotNull(_productCategoryIds) || _productCategoryIds?.Count > 0)
                GetCategoryNameByCategoryIds(_productCategoryIds);
        }

        //to get all distinct CategoryIds from shopping cart product
        public virtual void GetProductCategoryIds(int[] productCategoryIds, List<int> categoryIds)
        {
            foreach (int categoryId in productCategoryIds)
            {
                if (!categoryIds.Contains(categoryId))
                    categoryIds.Add(categoryId);
            }
        }

        //to get category name by category ids
        public virtual void GetCategoryNameByCategoryIds(List<int> categoryIds)
        {
            int localeId = ShoppingCart.LocalId;
            int publishCatalogId = ShoppingCart.PublishedCatalogId;
            if (IsNotNull(categoryIds) || categoryIds?.Count > 0)
            {
                int? versionId = publishProductHelper.GetCatalogVersionId(publishCatalogId);
                IZnodeRepository<ZnodePublishCategoryEntity> _publishCategoryEntity = new ZnodeRepository<ZnodePublishCategoryEntity>(HelperMethods.Context);

                List<CategoryModel> categories = _publishCategoryEntity.Table.Where(x => x.LocaleId == localeId && x.VersionId == versionId && categoryIds.Contains(x.ZnodeCategoryId))?.ToModel<CategoryModel>()?.ToList();

                if (categories?.Count > 0)
                {
                    categories.ForEach(category =>
                    {
                        if (!string.IsNullOrEmpty(category.CategoryName) && !publishCategory.ContainsKey(category.PublishCategoryId))
                            publishCategory.Add(category.PublishCategoryId, category.CategoryName);
                    });
                }
            }
        }

        //to get category name by Category id
        public virtual string GetCategoryNameByCategoryId(int categoryId)
        {
            return publishCategory.Where(c => c.Key == categoryId).Select(c => c.Value).FirstOrDefault();
        }

        //to rollback order from database if submitting order is failed
        public virtual void VerifySubmitOrderProcess(int orderDetailId)
        {
            if (!this.IsSuccess)
            {
                orderHelper.RollbackFailedOrder(orderDetailId);
            }
        }

        // to set order state and tracking number
        public virtual void SetOrderStateTrackingNumber(ZnodeOrderFulfillment order, SubmitOrderModel model)
        {
            if (IsNotNull(model?.OrderId))
            {
                order.OrderStateID = model.OrderStateId.GetValueOrDefault();
                order.Order.TrackingNumber = model.TrackingNumber;
                if (IsNotNull(model?.PaymentStateId))
                    order.Order.OmsPaymentStateId = model.PaymentStateId.GetValueOrDefault();
            }
        }

        //to save return items in database
        public virtual bool SaveReturnItems(int orderDetailId, ReturnOrderLineItemListModel model)
        {
            bool isSuccess = true;
            if (IsNotNull(model))
            {
                if (model?.ReturnItemList?.Count > 0)
                {
                    foreach (ReturnOrderLineItemModel item in model?.ReturnItemList)
                    {
                        item.OrderDetailId = orderDetailId;
                        isSuccess = orderHelper.ReturnOrderLineItems(item);
                    }
                }
            }
            return isSuccess;
        }

        //Set Order Details To Shopping Cart. b
        public virtual void SetOrderDetailsToShoppingCart(ZnodeOrderFulfillment order)
        {
            this.ShoppingCart.OrderId = order?.OrderID;
        }

        //Get Payment Type
        public virtual string GetPaymentType(string paymentType)
        {
            if (string.Equals(paymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase)
                  || string.Equals(paymentType, ZnodeConstant.Amazon_Pay, StringComparison.OrdinalIgnoreCase))
            {
                return ZnodeConstant.AUTHORIZED;
            }
            else
            {
                return ZnodeConstant.PENDING;
            }
        }
        [Obsolete]
        public virtual string GetPaymentType(string paymentType, bool isPreAuthorize)
        {
            if (string.Equals(paymentType, ZnodeConstant.CreditCard, StringComparison.OrdinalIgnoreCase)
                  || string.Equals(paymentType, ZnodeConstant.PAYPAL_EXPRESS, StringComparison.OrdinalIgnoreCase)
                  || string.Equals(paymentType, ZnodeConstant.Amazon_Pay, StringComparison.OrdinalIgnoreCase))
            {
                return isPreAuthorize ? ZnodeConstant.AUTHORIZED : ZnodeConstant.CAPTURED;
            }
            else
            {
                return ZnodeConstant.PENDING;
            }
        }
        #endregion
    }
}
