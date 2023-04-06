using System;
using System.Collections.Generic;
using System.Data;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Admin
{
    public interface IZnodeOrderHelper
    {
        /// <summary>
        /// To order shipping address
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="giftcardAmount"></param>
        /// <param name="giftcardNumber"></param>
        /// <param name="transactionDate"></param>
        /// <param name="userId">Current user Id</param>
        /// <returns>GiftCardHistoryId</returns>
        int AddToGiftCardHistory(int orderId, decimal giftcardAmount, string giftcardNumber, DateTime transactionDate, int? userId = 0);

        /// <summary>
        /// To cancel order by orderId and giftcardNumber
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="giftcardNumber"></param>
        /// <returns>Status of order</returns>
        bool CancelOrderById(int orderId, string giftcardNumber = "");

        /// <summary>
        /// to generate unique order number on basis of current date
        /// </summary>
        /// <returns>order number</returns>
        string GenerateOrderNumber();

        /// <summary>
        /// Get comma seperated SKUs of add on products.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="publishCatalogId"></param>
        /// <param name="localeId"></param>
        /// <returns>string with comma seperated SKUs</returns>
        string GetAddOnProducts(int publishProductId, int publishCatalogId, int localeId);

        /// <summary>
        /// Get comma seperated SKUs of bundle products.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="productType"></param>
        /// <param name="publishCatalogId"></param>
        /// <param name="localeId"></param>
        /// <returns>string with comma seperated SKUs</returns>
        string GetBundleProducts(int publishProductId, string productType, int publishCatalogId, int localeId);

        /// <summary>
        /// to get giftcard by cardnumber
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="orderId"></param>
        /// <returns>GiftCardModel</returns>
        GiftCardModel GetVoucherByCardNumber(string cardNumber, int? orderId = null);

        /// <summary>
        ///   Get giftcard by cardnumber
        /// </summary>
        /// <param name="cardNumber">cardNumber</param>
        /// <returns>GiftCardModel</returns>
        GiftCardModel GetByCardNumber(string cardNumber);

        /// <summary>
        /// Get comma seperated SKUs of configurable products.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="productType"></param>
        /// <param name="publishCatalogId"></param>
        /// <param name="localeId"></param>
        /// <returns>string with comma seperated SKUs</returns>
        string GetConfigurableProducts(int publishProductId, string productType, int publishCatalogId, int localeId, int? catalogVersionId = null);

        /// <summary>
        /// To get cookieMappingId for login or anonymous user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <returns>OmsCookieMappingId</returns>
        int GetCookieMappingId(int? userId, int portalId);

        /// <summary>
        /// To get CookieMapping List.
        /// </summary>
        /// <param name="cartParameterModel"></param>
        /// <returns>List of ZnodeOmsCookieMapping</returns>
        List<ZnodeOmsCookieMapping> GetCookieMappingList(CartParameterModel cartParameterModel);

        /// <summary>
        /// To get coupon available quantity
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns>Available Quantity of Coupons</returns>
        int GetCouponAvailableQuantity(string couponCode);

        /// <summary>
        /// to get coupons with promotion message.
        /// </summary>
        /// <param name="couponCodes"></param>
        /// <returns>List of coupons with promotion message</returns>
        List<CouponModel> GetCouponPromotionMessages(string couponCodes);

        /// <summary>
        /// To get order discount code by omsorderdetailsid
        /// </summary>
        /// <param name="omsOrderDetailsId"></param>
        /// <param name="discountType"></param>
        /// <returns>Discount Code</returns>
        string GetDiscountCode(int? omsOrderDetailsId, OrderDiscountTypeEnum discountType);

        /// <summary>
        /// To get group product line item xml data to be stored in savedcartlineitem table with ~ separated quantity
        /// </summary>
        /// <param name="groupItems"></param>
        /// <returns>ProductLineItemXMLData</returns>
        string GetGroupProductLineItemXMLData(List<AssociatedProductModel> groupItems);

        /// <summary>
        /// Get comma seperated SKUs and quantity of group products.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="productType"></param>
        /// <param name="publishCatalogId"></param>
        /// <param name="localeId"></param>
        /// <returns>string with comma seperated SKUs</returns>
        string GetGroupProducts(int publishProductId, string productType, int publishCatalogId, int localeId);

        /// <summary>
        /// Get LineItem Shipping Date.
        /// </summary>
        /// <param name="omsOrderLineItemsIds"></param>
        /// <returns>List of ZnodeOmsOrderLineItem</returns>
        List<ZnodeOmsOrderLineItem> GetLineItemShippingDate(List<int?> omsOrderLineItemsIds);

        /// <summary>
        /// Get LineItem Shipping Relationship type.
        /// </summary>
        /// <returns>List of ZnodeOmsOrderLineItemRelationshipType</returns>
        List<ZnodeOmsOrderLineItemRelationshipType> GetOmsOrderLineItemRelationshipTypeList();

        /// <summary>
        /// to get order by orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>ZnodeOmsOrderDetail</returns>
        ZnodeOmsOrderDetail GetOrderById(int orderId);

        /// <summary>
        /// Get orderDefaultStateId by PortalId.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns>Order default state id</returns>
        int? GetorderDefaultStateId(int portalId);

        /// <summary>
        /// To get order csr discount details by omsorderdetailsid
        /// </summary>
        /// <param name="omsOrderDetailsId"></param>
        /// <param name="discountType"></param>
        /// <returns>Discount Amount</returns>
        decimal GetOrderDiscountAmount(int? omsOrderDetailsId, OrderDiscountTypeEnum discountType);

        /// <summary>
        /// To get order discount details by omsorderdetailsid
        /// </summary>
        /// <param name="omsOrderDetailsId"></param>
        /// <returns>List of OrderDiscountModel</returns>
        List<OrderDiscountModel> GetOrderDiscountAmount(int omsOrderDetailsId);

        /// <summary>
        /// To get order line items by orderDetailsId
        /// </summary>
        /// <param name="orderDetailsId"></param>
        /// <returns>List of ZnodeOmsOrderLineItem</returns>
        List<ZnodeOmsOrderLineItem> GetOrderLineItemByOrderId(int orderDetailsId);

        /// <summary>
        /// To get order shipment address.
        /// </summary>
        /// <param name="OrderShipmentID"></param>
        /// <param name="orderLineItem"></param>
        void GetOrderShipmentAddress(int OrderShipmentID, OrderLineItemModel orderLineItem);

        /// <summary>
        /// To get the order shipment details for the provided shipment Ids.
        /// </summary>
        /// <param name="orderShipmentIds">orderShipmentIds</param>
        /// <returns>List of OrderShipmentModel</returns>
        List<OrderShipmentModel> GetOrderShipmentDetailsByIds(List<int> orderShipmentIds);

        /// <summary>
        /// To get order total by Id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>Order Total</returns>
        decimal GetOrderTotalById(int orderId);

        /// <summary>
        /// To get Personalized Attributes item xml data to be stored in savedcartlineitem table with ~ separated quantity
        /// </summary>
        /// <param name="PersonalizedAttributes"></param>
        /// <param name="cookieMappingId"></param>
        /// <returns>PersonalizeAttributeLineItemXMLData</returns>
        string GetPersonalizedAttributeLineItemXMLData(Dictionary<string, object> PersonalizedAttributes, string cookieMappingId);

        /// <summary>
        /// Get the list of Personalized attribute cart line item on the basis of savedCartLineItemId.
        /// </summary>
        /// <param name="savedCartLineItemId"></param>
        /// <param name="localeId"></param>
        /// <returns>PersonalizedValueCartLineItem</returns>
        Dictionary<string, object> GetPersonalizedValueCartLineItem(int savedCartLineItemId, int localeId);

        /// <summary>
        /// Get the list of Personalized attribute order line item on the basis of orderLineItemId.
        /// </summary>
        /// <param name="orderLineItemId"></param>
        /// <param name="isReorder"></param>
        /// <param name="localeId"></param>
        /// <returns>PersonalizedValueCartLineItem</returns>
        Dictionary<string, object> GetPersonalizedValueOrderLineItem(int orderLineItemId, bool isReorder, int localeId);

        /// <summary>
        /// To get portal details by portalId for order receipt
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns>PortalModel</returns>
        PortalModel GetPortalDetailsByPortalId(int portalId);

        /// <summary>
        /// Get Portal Feature Value.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="storeFeature"></param>
        /// <returns>true or False</returns>
        bool GetPortalFeatureValue(int portalId, HelperUtility.StoreFeature storeFeature);

        /// <summary>
        /// To get savedcartId by cookieMappingId
        /// </summary>
        /// <param name="cookieMappingId"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <returns>Saved Cart Id</returns>
        int GetSavedCartId(int? cookieMappingId, int portalId = 0, int? userId = 0);

        /// <summary>
        /// To get savedcartId by cookieMappingId
        /// </summary>
        /// <param name="cookieMappingId"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <returns>Saved Cart Id</returns>
        int GetSavedCartId(ref int cookieMappingId, int portalId = 0, int? userId = 0);

        /// <summary>
        /// Get the list of saved cart line item on the basis of savedCartId.
        /// </summary>
        /// <param name="savedCartId"></param>
        /// <param name="omsOrderId"></param>
        /// <returns>ZnodeOmsSavedCartLineItem</returns>
        List<ZnodeOmsSavedCartLineItem> GetSavedCartLineItem(int savedCartId, int omsOrderId = 0);

        /// <summary>
        /// Get comma seperated SKUs and quantity of group products.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="productType"></param>
        /// <param name="publishCatalogId"></param>
        /// <param name="localeId"></param>
        /// <returns>AssociatedProductModel</returns>
        List<AssociatedProductModel> GetShoppingCartGroupProducts(int publishProductId, string productType, int publishCatalogId, int localeId);

        /// <summary>
        /// To check applied coupon is used in exiting order by OrderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="couponcode"></param>
        /// <returns>bool</returns>
        bool IsExistingOrderCoupon(int orderId, string couponcode);

        /// <summary>
        /// To check this state have option to send email
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>bool</returns>
        bool IsSendEmail(int stateId);

        /// <summary>
        /// To manage inventory for order
        /// </summary>
        /// <param name="orderInventory"></param>
        /// <returns>Returns true or false</returns>
        bool ManageOrderInventory(OrderWarehouseModel orderInventory, out List<OrderWarehouseLineItemsModel> productInventoryList);

        /// <summary>
        /// To save/update SavedCartlineItem data in database
        /// </summary>
        /// <param name="shoppingCart"></param>
        void MergedShoppingCart(ShoppingCartModel shoppingCart);

        /// <summary>
        /// To manage inventory for order
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool value</returns>
        bool ReturnOrderLineItems(ReturnOrderLineItemModel model);

        /// <summary>
        /// To rollback failure order from database
        /// </summary>
        /// <param name="orderDetailId"></param>
        /// <returns>bool Value</returns>
        bool RollbackFailedOrder(int orderDetailId);

        /// <summary>
        /// To save/update SavedCartlineItem data in database
        /// </summary>
        /// <param name="savedCartId"></param>
        /// <param name="shoppingCart"></param>
        /// <returns>bool Value</returns>
        bool SaveAllCartLineItems(int savedCartId, ShoppingCartModel shoppingCart);

        /// <summary>
        /// To save/update SavedCartlineItem data in database
        /// </summary>
        /// <param name="savedCartId"></param>
        /// <param name="shoppingCart"></param>
        /// <returns>bool Value</returns>
        bool SaveAllCartLineItemsInDatabase(int savedCartId, AddToCartModel shoppingCart);

        /// <summary>
        /// To save/update SavedCartlineItem data in database
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns>AddToCartStatusModel Value</returns>
        AddToCartStatusModel SaveAllCartLineItemsInDatabase(AddToCartModel shoppingCart, int cookieMappingId=0);

        /// <summary>
        /// Save Downloadable product key to database.
        /// </summary>
        /// <param name="orderDetails"></param>
        /// <param name="userId"></param>
        /// <param name="omsOrderDetailsId"></param>
        /// <returns>DataTable- ProductKey Table</returns>
        DataTable SaveDownloadableProductKey(DataTable orderDetails, int userId, int omsOrderDetailsId);


        /// <summary>
        /// Save Downloadable product key with json paramters to database.
        /// </summary>
        /// <param name="orderDetails"></param>
        /// <param name="userId"></param>
        /// <returns>DataTable- ProductKey Table</returns>
        DataTable SaveDownloadableProductKeyWithJSON(DataTable orderDetails, int userId);

        /// <summary>
        /// To save order attributes
        /// </summary>
        /// <param name="orderLineItem"></param>
        /// <returns>True or False</returns>
        bool SaveOrderAttributes(OrderLineItemModel orderLineItem);

        /// <summary>
        /// save order details
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Oms Order Id</returns>
        int SaveOrderDetails(OrderModel order);

        /// <summary>
        /// to save order discount
        /// </summary>
        /// <param name="orderDiscount"></param>
        /// <returns>Oms Order Discount</returns>
        bool SaveOrderDiscount(List<OrderDiscountModel> orderDiscount);

        /// <summary>
        /// save order line items
        /// </summary>
        /// <param name="orderLineItem"></param>
        /// <returns>OmsOrderLineItemsId</returns>
        int SaveOrderLineItem(OrderLineItemModel orderLineItem);

        /// <summary>
        /// Save Order Notes.
        /// </summary>
        /// <param name="omsOrderDetailsId"></param>
        /// <param name="note"></param>
        /// <returns>bool</returns>
        bool SaveOrderNotes(int omsOrderDetailsId, string note);

        /// <summary>
        /// To save order  the referral commission
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderDetailsId"></param>
        /// <param name="transactionId"></param>
        /// <param name="commissionAmount"></param>
        /// <returns>bool</returns>
        bool SaveReferralCommission(int? userId, int orderDetailsId, string transactionId, decimal commissionAmount);

        /// <summary>
        /// To save shipping address.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="shippingId"></param>
        /// <param name="emailId"></param>
        /// <returns>OmsOrderShipmentId</returns>
        int SaveShippingAddress(AddressModel model, int? shippingId, string emailId);

        /// <summary>
        /// Set tax order to ZnodeOmsTaxOrderDetail table.
        /// </summary>
        /// <param name="orderDetailId"></param>
        /// <param name="OrderLineItem"></param>
        /// <param name="orderId"></param>
        /// <param name="taxRate"></param>
        void SaveTaxOrder(int orderDetailId, List<OrderLineItemModel> OrderLineItem, int orderId, decimal? taxRate);

        /// <summary>
        /// Set Portal Logo
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns>string</returns>
        string SetPortalLogo(int portalId);

        /// <summary>
        /// To update Coupon available quantity if it is more than 1
        /// </summary>
        /// <param name="couponCode"></param>
        /// <param name="orderId"></param>
        /// <param name="isExistingOrder"></param>
        /// <returns>Bool</returns>
        bool UpdateCouponQuantity(string couponCode, int orderId = 0, bool isExistingOrder = false);

        /// <summary>
        /// To update giftcard by cardnumber
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        bool UpdateGiftCard(GiftCardModel model, bool isExistingOrder = false);

        /// <summary>
        /// To order  payment details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        bool UpdateOrderPaymentDetails(OrderModel model);
        /// <summary>
        /// Get payment status id
        /// </summary>
        /// <param name="paymentStatus">paymentStatus</param>
        /// <returns>Payment status id.</returns>
        int? GetPaymentStatusId(string paymentStatus);

        /// <summary>
        /// Formalize OrderLineItems
        /// </summary>
        /// <param name="orderModel"></param>
        /// <returns></returns>
        List<OrderLineItemModel> FormalizeOrderLineItems(OrderModel orderModel);

        /// <summary>
        /// Set Personalize Order Details.
        /// </summary>
        /// <param name="lineItem"></param>
        void SetPersonalizeDetails(OrderLineItemModel lineItem);

        /// <summary>
        /// Get PersonalizedAttributeLineItemDetails
        /// </summary>
        /// <param name="PersonalizedAttributes"></param>
        /// <param name="empty"></param>
        /// <returns></returns>
        List<PersonaliseValueModel> GetPersonalizedAttributeLineItemDetails(Dictionary<string, object> PersonalizedAttributes, string cookieMappingId);

        /// <summary>
        /// Get PersonalizedValue CartLineItem
        /// </summary>
        /// <param name="savedCartLineItemId"></param>
        /// <returns></returns>
        List<PersonaliseValueModel> GetPersonalizedValueCartLineItem(int savedCartLineItemId);

        /// <summary>
        /// Get PersonalizedValue CartLineItem
        /// </summary>
        /// <param name="savedCartLineItemId"></param>
        /// <returns></returns>
        List<PersonaliseValueModel> GetPersonalizedValueCartLineItem(List<int?> savedCartLineItemId);

        /// <summary>
        /// Get Personalized QuoteValue CartLineItem
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        List<PersonaliseValueModel> GetPersonalizedQuoteValueCartLineItem(int quoteCartLineItemId);

        ///// <summary>
        ///// Get Personalized CartLineItem
        ///// </summary>
        ///// <param name="lineItem"></param>
        ///// <returns></returns>
        //List<PersonaliseValueModel> GetPersonalizedValueCartLineItem(AccountQuoteLineItemModel lineItem);

        /// <summary>
        /// Get the list of all Parent line items.
        /// </summary>
        /// <param name="parentSavedCartLineItems">Ids of parent line items.</param>
        /// <param name="omsOrderId">Id of order.</param>
        /// <returns></returns>
        List<ZnodeOmsSavedCartLineItem> GetParentSavedCartLineItem(List<int> parentSavedCartLineItems, int omsOrderId = 0);

        /// <summary>
        /// Bind ParentOmsOrderLineItemId
        /// </summary>
        /// <param name="orderLineItem"></param>
        /// <param name="childLineItems"></param>
        void BindParentOmsOrderLineItemId(OrderLineItemModel orderLineItem, OrderLineItemModel childLineItems);

        string GetInventoryRoundOff(string quantity);

         /// <summary>
        /// To get total round off price
        /// </summary>
        /// <param name="price"></param>
        /// <returns>TotalPrice</returns>
        decimal GetPriceRoundOff(decimal price);

        /// <summary>
        /// To get group product line item xml data to be stored in savedcartlineitem table with ~ separated quantity
        /// </summary>
        /// <param name="groupItems"></param>
        /// <returns>ProductLineItemXMLData</returns>
        string GetGroupProductLineItemXMLDataForTemplate(List<AssociatedProductModel> groupItems, string templateType = null);

        /// <summary>
        /// Get user voucher list.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>GiftCardModel list model</returns>
        List<GiftCardModel> GetUserVouchers(int userId, int? portalId);

        /// <summary>
        /// Get voucher details by Voucher number.
        /// </summary>
        /// <param name="voucherCodes"></param>
        /// <returns></returns>
        List<VoucherModel> GetVoucherDetailByCodes(string voucherCodes, int omsOrderId);

        /// <summary>
        /// Get voucher By voucher Number
        /// </summary>
        /// <param name="voucherNumber">Voucher Number</param>
        /// <returns>GiftCardModel</returns>
        GiftCardModel GetVoucherByCardNumber(string voucherNumber);


        /// <summary>
        /// Return list of all discount saved against line items in the order
        /// </summary>
        /// <param name="omsOrderId"></param>
        /// <returns></returns>
        List<OrderDiscountModel> GetOmsOrderDiscountList(int omsOrderId);

        /// <summary>
        /// Calculate CSR Discount
        /// </summary>
        /// <param name="discountMultiplier"></param>
        /// <param name="csrDiscountAmount"></param>
        /// <param name="shoppingCartModel"></param>
        void CalculateCSRDiscount(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Set line item discount in shoppingCartItem model
        /// </summary>
        /// <param name="allLineItemDiscountList"></param>
        /// <param name="shoppingCartItem"></param>
        void SetPerQuantityDiscount(List<OrderDiscountModel> allLineItemDiscountList, ShoppingCartItemModel shoppingCartItem);

        /// <summary>
        /// Return list of all discount saved against line items in the order
        /// </summary>
        /// <param name="omsOrderId"></param>
        /// <returns></returns>
        List<OrderDiscountModel> GetReturnItemsDiscountList(int omsOrderId);

        /// <summary>
        /// To get coupons with promotion message.
        /// </summary>
        /// <param name="couponCodes"></param>
        /// <param name="isOldOrder"></param>
        /// <param name="omsOrderId"></param>
        /// <returns>List of coupons with promotion message</returns>
        List<CouponModel> GetCouponPromotionMessages(string[] couponCodes, bool isOldOrder, int omsOrderId = 0);


        /// <summary>
        /// Set line item discount in RMAReturnLineItem model
        /// </summary>
        /// <param name="allLineItemDiscountList"></param>
        /// <param name="returnLineItem"></param>
        void SetPerQuantityDiscountForReturnItem(List<OrderDiscountModel> allLineItemDiscountList, RMAReturnLineItemModel returnLineItem);

        /// <summary>
        /// Return list of discount by rmaReturnDetailsId
        /// </summary>
        /// <param name="rmaReturnDetailsId"></param>
        /// <returns></returns>
        List<OrderDiscountModel> GetReturnItemsDiscount(int rmaReturnDetailsId);

        /// <summary>
        /// Return list of order line items by OmsOrderId
        /// </summary>
        /// <param name="omsOrderId"></param>
        /// <returns></returns>
        List<ZnodeOmsOrderLineItem> GetOrderLineItemByOmsOrderId(int omsOrderId);

        /// <summary>
        /// Return list of bundle order line items by OmsOrderId
        /// </summary>
        /// <param name="omsOrderId"></param>
        /// <returns></returns>
        List<ZnodeOmsOrderLineItem> GetBundleOrderLineItemByOmsOrderId(int omsOrderId);

        /// <summary>
        /// Saves the failed order transaction detail
        /// </summary>
        /// <param name="shoppingCartModel">ShoppingCartModel</param>
        /// <returns>returns bool</returns>
        bool SaveFailedOrderTransaction(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Bind Order Model
        /// </summary>
        /// <param name="orderModel">model</param>
        /// <returns></returns>
        PlaceOrderModel BindOrderModel(OrderModel orderModel);

        /// <summary>
        ///  Bind line item order model
        /// </summary>
        /// <param name="orderLineItemModel"></param>
        /// <returns></returns>
        PlaceOrderLineItemModel BindLineItemOrderModel(OrderLineItemModel orderLineItemModel);

        /// <summary>
        /// Bind child item order model
        /// </summary>
        /// <param name="orderLineItemModel">OrderLineItemModel</param>
        /// <returns></returns>
        PlaceOrderlineItemCollection BindChildItemOrderModel(OrderLineItemModel orderLineItemModel);

        /// <summary>
        /// Bind discount model
        /// </summary>
        /// <param name="orderDiscountModel">List<OrderDiscountModel> </param>
        /// <returns></returns>
        List<PlaceOrderDiscountModel> BindDiscountModel(List<OrderDiscountModel> orderDiscountModel);


        /// <summary>
        /// To save shipping address.
        /// </summary>
        /// <param name="addressModel"></param>
        /// <param name="shippingId"></param>
        /// <param name="emailId"></param>
        /// <returns>OmsOrderShipmentId</returns>
        OrderShipmentModel SaveShippingAddressData(AddressModel addressModel, int? shippingId, string emailId);
    }
}