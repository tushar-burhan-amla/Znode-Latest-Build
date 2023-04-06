using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Fulfillment;
using Znode.Libraries.ECommerce.Utilities;
using System;
using Znode.Libraries.Data.DataModel;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public interface IZnodeCheckout
    {
        string AdditionalInstructions { get; set; }
        bool IsSuccess { get; set; }
        string PaymentResponseText { get; set; }
        string PoDocument { get; set; }
        int PortalID { get; set; }
        string PurchaseOrderNumber { get; set; }
        int ShippingID { get; set; }
        IZnodePortalCart ShoppingCart { get; set; }
        UserAddressModel UserAccount { get; set; }

        void AddAddOnsItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem);
        void AddBundleItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem, List<ZnodeOmsOrderLineItem> omslineItems = null);
        void AddConfigurableItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem);
        void AddGroupItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem);
        void AddLineItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeProductBaseEntity product, ZnodeShoppingCartItem shoppingCartItem, ZnodeCartItemRelationshipTypeEnum cartItemProductType, List<ZnodeOmsOrderLineItem> omslineItems = null);
        void AddSimpleItemInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeShoppingCartItem shoppingCartItem);
        void AddSimpleProductLineItemsInOrderLineItem(OrderLineItemModel orderLineItem, ZnodeCartItemRelationshipTypeEnum cartItemProductType);
        void AddSkuInventoryTracking(ZnodeProductBaseEntity product, List<OrderWarehouseLineItemsModel> skuInventoryTracking, bool allowBackOrder = false);
        bool CancelExistingOrder(ZnodeOrderFulfillment order, int orderId);
        OrderShipmentModel CreateOrderShipment(AddressModel shippingAddress, int? shippingId, string emailId);
        string GetCategoryNameByCategoryId(int categoryId);
        void GetCategoryNameByCategoryIds(List<int> categoryIds);
        void GetDistinctCategoryIdsforCartItem(IZnodeMultipleAddressCart addressCart);
        string GetInventoryTrackingBySKU(string sku, List<OrderWarehouseLineItemsModel> skusInventory, out bool allowBackOrder);
        decimal GetLineItemDiscountAmount(decimal discountAmount, decimal quantity);
        decimal GetLineItemQuantity(ZnodeCartItemRelationshipTypeEnum productType, decimal groupProductQuantity, decimal cartQuantity);
        ZnodeOrderFulfillment GetOrderFulfillment(UserAddressModel userAccount, IZnodePortalCart shoppingCart, int portalId, out List<OrderShipmentModel> model);
        decimal GetOrderLineItemPrice(ZnodeCartItemRelationshipTypeEnum cartItemProductType, ZnodeProductBaseEntity product, ZnodeShoppingCartItem shoppingCartItem);
        decimal GetParentProductPrice(ZnodeShoppingCartItem shoppingCartItem);
        string GetPaymentType(string paymentType);
        [Obsolete]
        string GetPaymentType(string paymentType, bool isPreAuthorize);
        void GetProductCategoryIds(int[] productCategoryIds, List<int> categoryIds);
        decimal GetProductPrice(ZnodeProductBaseEntity product);
        bool ManageOrderInventory(ZnodeOrderFulfillment order, IZnodePortalCart shoppingCart);
        bool SaveReferralCommission(ZnodeOrderFulfillment order);
        /// <summary>
        /// To Save referral commission and giftcard history.
        /// </summary>
        /// <param name="order">ZnodeOrderFulfillment model</param>
        /// <param name="userId">Current user Id</param>
        void SaveReferralCommissionAndGiftCardHistory(ZnodeOrderFulfillment order, int? userId);
        bool SaveReturnItems(int orderDetailId, ReturnOrderLineItemListModel model);
        List<OrderAttributeModel> SetLineItemAttributes(List<OrderAttributeModel> allAttributes, int[] productCategoryIds, ZnodeShoppingCartItem shoppingCartItem = null);
        void SetOrderAdditionalDetails(ZnodeOrderFulfillment order, SubmitOrderModel model);
        void SetOrderDetails(ZnodeOrderFulfillment order, IZnodePortalCart shoppingCart, UserAddressModel userAccount);
        void SetOrderDetailsToShoppingCart(ZnodeOrderFulfillment order);
        void SetOrderDiscount(OrderModel model, decimal discountAmount, string discountCode, int discountType, DiscountLevelTypeIdEnum discountApplicable, decimal discountMultiplier);
        void SetOrderLineItems(ZnodeOrderFulfillment order, IZnodeMultipleAddressCart addressCart);
        void SetOrderModel(ZnodeOrderFulfillment order, IZnodePortalCart shoppingCart);
        void SetOrderShipmentDetails(ZnodeOrderFulfillment order, List<OrderShipmentModel> orderShipmentModel);
        void SetOrderStateTrackingNumber(ZnodeOrderFulfillment order, SubmitOrderModel model);
        bool SetPaymentDetails(ZnodeOrderFulfillment order);
        List<OrderWarehouseLineItemsModel> SetSKUInventorySetting(IZnodePortalCart shoppingCart);
        /// <summary>
        /// To submit order.
        /// </summary>
        /// <param name="model">SubmitOrderModel</param>
        /// <param name="shoppingCartModel">ShoppingCartModel</param>
        /// <param name="isTaxCostUpdated">Set to true if tax amount of order is changed else set to false</param>
        /// <returns>ZnodeOrderFulfillment</returns>
        ZnodeOrderFulfillment SubmitOrder(SubmitOrderModel model, ShoppingCartModel shoppingCartModel, bool isTaxCostUpdated = true);
        void VerifySubmitOrderProcess(int orderDetailId);
    }
}