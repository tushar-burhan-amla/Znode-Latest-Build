using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public interface IZnodeShoppingCart: IZnodeShoppingCartEntities
    {
        // Pass profile ID as null to the overload
        void Calculate();

        /// <summary>
        /// Calculates final pricing, shipping and taxes in the cart.
        /// </summary>
        void Calculate(int? profileId, bool isCalculateTaxAndShipping = true, bool isCalculatePromotionAndCoupon = true);

        /// <summary>s
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if the order should be submitted. False if there is anything that will prevent the order from submitting correctly.</returns>
        bool PreSubmitOrderProcess(out string isInventoryInStockMessage, out Dictionary<int, string> minMaxSelectableQuantity);

        // Process anything that must be done after the order is submitted.
        void CancelTaxOrderRequest(ShoppingCartModel shoppingCartModel);

        // Process anything that must be done after the order is submitted.
        void ReturnOrderLineItem(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Process anything that must be done after the order has been submitted.
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <param name="isGuest">Set to true if user is guest else false</param>
        /// <param name="isTaxCostUpdated">Set to true if tax amount of order is changed else set to false</param>
        void PostSubmitOrderProcess(int orderId = 0, bool isGuest = true, bool isTaxCostUpdated = true);

        /// <summary>
        /// post call for submit tax.
        /// </summary>
        /// <param name="isTaxCostUpdated">flag to check tax cost updated or not</param>
        void SubmitTax(bool isTaxCostUpdated = true);

        /// <summary>
        /// // Reduce the quantity of available coupons if it is applied to order
        /// </summary>
        void ReduceCouponsQuantity(int orderId = 0);

        // Adds a coupon code to the shopping cart.
        void AddCouponCode(string CouponCode);

        /// <summary>
        /// Add Gift Card to the shopping cart.
        /// </summary>
        /// <param name="giftCardNumber">Unique gift card number.</param>  
        bool AddGiftCard(string giftCardNumber, int? orderId = null);


        /// <summary>
        ///  Add CSR Discount to the shopping cart.
        /// </summary>
        /// <param name="discountAmount"></param>
        /// <returns></returns>
        bool AddCSRDiscount(decimal discountAmount);

        //to check inventory of products, addons, bundle, group and configurable product in the shopping cart if inventory set to 'disable purchasing for out of stock product'.
        bool IsInventoryInStock();

        Dictionary<int, string> IsValidMinAndMaxSelectedQuantity();

        //save the shoppingcart items in the database.
        int Save(ShoppingCartModel shoppingCart, string groupIdProductAttribute = "", GlobalSettingValues groupIdPersonalizeAttribute = null);

        //Save the shopping cart items in the database.
        AddToCartModel SaveAddToCartData(AddToCartModel cartModel, string groupIdProductAttribute = "", GlobalSettingValues groupIdPersonalizeAttribute = null);

        //to load shoppingCart from database by cookieMappingId.
        ZnodeShoppingCart LoadFromDatabase(CartParameterModel cartParameterModel, List<string> expands = null);

        //to load shoppingCart from database by orderId
        ShoppingCartModel LoadCartFromOrder(CartParameterModel model, int? catalogVersionId = null);

        void SetParentLineItemDetails(List<ZnodeOmsOrderLineItem> parentDetails, List<PublishedProductEntityModel> productList);

        //Set Parent Product Name for Group Product
        void SetGroupAndConfigurableParentProductDetails(List<ZnodeOmsOrderLineItem> parentDetails, ZnodeOmsOrderLineItem lineItem, ShoppingCartItemModel item);

        //Get Download product key of product.
        string GetProductKey(string sku, decimal quantity, int omsOrderLineItemsId);

        void SetPersonalizedAttributes(ZnodeOmsOrderLineItem orderLineItem, ShoppingCartItemModel cartItem);

        //to add saved cart line item to shopping Cart
        void AddToShoppingCart(AccountQuoteLineItemModel cartLineItemModel, List<AccountQuoteLineItemModel> cartLineItems, CartParameterModel cartParameterModel);

        //to add item to ZnodeShoppingCart from api model
        void AddtoShoppingCart(ShoppingCartItemModel model, AddressModel shippingAddress, int localeId, int publishedCatalogId, int userId, int omsOrderId);

        //to add item to ZnodeShoppingCart from api model
        void AddtoShoppingBag(ShoppingCartModel shoppingCartItems);

        //to bind custom data from shopping cart item to ZNodeShoppingCartItem
        void BindCustomData(AccountQuoteLineItemModel model, ZnodeShoppingCartItem cartItem);

        // Calculates total of all additional cost associated with each cartline item if any
        decimal GetAdditionalPrice();

        // To set order discount amount
        void SetOrderDiscount(ShoppingCartModel cartModel);

        //to checks if the coupon quantity is available.
        bool IsCouponQuantityAvailable(string couponCode);

        // Check inventory and min/max quantity.
        void CheckInventoryAndMinMaxQuantity(out string isInventoryInStockMessage, out Dictionary<int, string> minMaxSelectableQuantity);

        //Get publish product list.
        PublishProductListModel GetPublishProductModelList(List<ShoppingCartItemModel> model, int localeId, int publishedCatalogId, int omsOrderId);

        //to load shoppingCart from database by QuoteId
        ShoppingCartModel LoadCartFromQuote(CartParameterModel model, int? catalogVersionId = null);

        int? GetUserId();

        List<OrderWarehouseLineItemsModel> LowInventoryProducts { get; set; }

        bool CheckWithInStockInventory(List<InventorySKUModel> inventoryList);
        int CookieMappingId { get; set; }
        
        //Get the list of child product and its associated quantity, per bundle product
        List<AssociatedPublishedBundleProductModel> BindBundleProductChildByParentSku(string bundleProductSkus, int publishCatalogId, int localeId);
    }
}
