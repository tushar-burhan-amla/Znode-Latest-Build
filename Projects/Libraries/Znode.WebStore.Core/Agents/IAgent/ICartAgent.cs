using Znode.Engine.Api.Client.Sorts;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Core.ViewModels;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface ICartAgent
    {
        /// <summary>
        /// Create new cart.
        /// </summary>
        /// <param name="cartItem">CartItemViewModel.</param>
        /// <returns>Returns CartViewModel created.</returns>
        CartViewModel CreateCart(CartItemViewModel cartItem);

        /// <summary>
        /// Add product in the cart.
        /// </summary>
        /// <param name="cartItem">AddToCartViewModel.</param>
        /// <returns>Returns AddToCartViewModel created.</returns>
        AddToCartViewModel AddToCartProduct(AddToCartViewModel cartItem);

        /// <summary>
        /// Get cart
        /// </summary>
        /// <returns>CartViewModel</returns>
        CartViewModel GetCart(bool IsCalculateTaxAndShipping = true, bool isCalculateCart = true, bool isCalculatePromotionAndCoupon = true);

        /// <summary>
        /// Get user view model from session.
        /// </summary>
        /// <returns></returns>
        UserViewModel GetUserViewModelFromSession();

        /// <summary>
        /// Get the list of user approvers.
        /// </summary>
        /// <param name="omsQuoteId"></param>
        /// <param name="showAllApprovers"></param>
        /// <returns></returns>
        UserApproverListViewModel GetUserApproverList(int omsQuoteId, bool showAllApprovers);

        /// <summary>
        /// Calculate shopping cart
        /// </summary>
        /// <returns></returns>
        CartViewModel CalculateCart();

        /// <summary>
        /// Update quantity of shopping cart item.
        /// </summary>
        /// <param name="guid">External Id of cart item.</param>
        /// <param name="quantity">Selected quantity to update.</param>
        /// <param name="productId">Id of Product.</param>
        /// <returns>CartViewModel</returns>
        CartViewModel UpdateCartItemQuantity(string guid, string quantity, int productId);

        /// <summary>
        /// Update quantity of shopping cart item.
        /// </summary>
        /// <param name="guid">External Id of cart item.</param>
        /// <param name="quantity">Selected quantity to update.</param>
        /// <param name="productId">Id of Product.</param>
        /// <returns>AddToCartViewModel</returns>
        AddToCartViewModel UpdateQuantityOfCartItem(string guid, string quantity, int productId);

        /// <summary>
        /// Remove cart item from shopping cart.
        /// </summary>
        /// <param name="guiId">External Id.</param>
        /// <returns>True if removed else false.</returns>
        bool RemoveCartItem(string guiId);

        /// <summary>
        /// Remove cart items from shopping cart.
        /// </summary>
        /// <param name="guiId">Comma separated External Ids.</param>
        /// <returns>True if removed else false.</returns>
        bool RemoveCartItems(string[] guiId);

        /// <summary>
        /// Remove all cart items from shopping cart.
        /// </summary>
        /// <returns>True if removed else false.</returns>
        bool RemoveAllCartItems();

        /// <summary>
        /// Remove all cart items from shopping cart.
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns>True if removed else false.</returns>
        bool RemoveAllCartItems(int omsOrderId = 0);

        /// <summary>
        /// Get Cart count method to check Session or Cookie to get the existing cart item count.
        /// </summary>
        /// <returns>Returns count of cart items.</returns>
        decimal GetCartCount();

        /// <summary>
        /// Get Cart count method to check Session or Cookie to get the existing cart item count.
        /// </summary>
        /// <returns>Returns count of cart items.</returns>
        decimal GetCartCountAfterLogin(bool cartCountAfterLogin);

        /// <summary>
        /// Get cart count of a specific product.
        /// </summary>
        /// <param name="productId">productId</param>
        /// <returns>Return count of items in cart for the product.</returns>
        decimal GetCartCount(int productId);

        /// <summary>
        /// Get template cart model session.
        /// </summary>
        /// <returns>Returns count of cart items.</returns>
        TemplateViewModel GetTemplateCartModelSession();

        /// <summary>
        /// Get cart from cookie.
        /// </summary>
        /// <returns>ShoppingCartModel</returns>
        ShoppingCartModel GetCartFromCookie();

        /// <summary>
        /// Calculate shipping for shopping cart.
        /// </summary>
        /// <param name="shippingOptionId">shipping option id.</param>
        /// <param name="shippingAddressId">shipping address id.</param>
        /// <param name="shippingCode">shipping code.</param>
        /// <param name="isCalculateCart">If set to true then only shopping cart calculate call will be made.</param>
        /// <returns>CartViewModel</returns>
        CartViewModel CalculateShipping(int shippingOptionId, int shippingAddressId, string shippingCode, string additionalInstruction = "", bool isQuoteRequest = false, bool isCalculateCart = true, bool isPendingOrderRequest = false, string jobName = "");

        /// <summary>
        /// Merge cart.
        /// </summary>
        /// <returns>true if merged else false.</returns>
        bool MergeCart();

        /// <summary>
        /// Apply coupon/giftcard to shopping cart.
        /// </summary>
        /// <param name="discountCode">Coupon Code/ Gift Card Number.</param>
        /// <param name="isGiftCard">flag to check if giftcard needs to apply or coupon.</param>
        /// <returns>CartViewModel.</returns>
        CartViewModel ApplyDiscount(string discountCode, bool isGiftCard);

        /// <summary>
        /// Removes the applied coupon from the cart.
        /// </summary>
        /// <param name="couponCode">coupon code to remove.</param>
        /// <returns>CartViewModel</returns>
        CartViewModel RemoveCoupon(string couponCode);

        #region Template

        /// <summary>
        /// Get Template Item By External Id
        /// </summary>
        /// <param name="cartModel">TemplateViewModel</param>
        /// <param name="guid">string</param>
        /// <param name="quantity">decimal</param>
        /// <param name="productId">int</param>
        /// <returns>TemplateViewModel</returns>
        TemplateViewModel GetTemplateItemByExternalId(TemplateViewModel cartModel, string guid, decimal quantity, int productId);

        /// <summary>
        /// Get Saved Cart Model Session Key
        /// </summary>
        /// <returns>string</returns>
        string GetSavedCartModelSessionKey();

        /// <summary>
        /// Update Template Item Quantity
        /// </summary>
        /// <param name="shoppingCart">TemplateViewModel</param>
        /// <param name="guid">string</param>
        /// <param name="quantity">decimal</param>
        /// <param name="productId">int</param>
        /// <returns>TemplateViewModel</returns>
        TemplateViewModel UpdateTemplateItemQuantity(TemplateViewModel shoppingCart, string guid, decimal quantity, int productId);

        /// <summary>
        /// Set Template Model
        /// </summary>
        /// <param name="accountTemplateModel">AccountTemplateModel</param>
        /// <param name="templateType">string</param>
        void SetTemplateModel(AccountTemplateModel accountTemplateModel, string templateType);

        /// <summary>
        /// Add product to template.
        /// </summary>
        /// <param name="cartItem">TemplateItemViewModel</param>
        /// <returns>Returns TemplateViewModel.</returns>
        TemplateViewModel AddToTemplate(TemplateCartItemViewModel cartItem);

        /// <summary>
        /// Set template cart model session to null.
        /// </summary>
        void SetTemplateCartModelSessionToNull();

        /// <summary>
        /// Delete template on the basis of oms Template Id.
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>        
        /// <returns>Returns status</returns>
        bool DeleteTemplate(string omsTemplateId);

        /// <summary>
        /// Method to delete  template cart item.
        /// </summary>
        /// <param name="templateViewModel"></param>
        /// <returns></returns>
        bool DeleteCartItem(TemplateViewModel templateViewModel);

        /// <summary>
        /// Get template list against a user.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sortCollection">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <param name="templateType"></param>
        /// <returns>Returns list of template.</returns>
        TemplateListViewModel GetTemplateList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, string templateType = null);


        /// <summary>
        /// Create template.
        /// </summary>
        /// <param name="templateViewModel">Template view model to create.</param>
        /// <returns>Returns true if template created successfully else false.</returns>
        bool CreateTemplate(TemplateViewModel templateViewModel);

        /// <summary>
        /// Get template.
        /// </summary>
        /// <param name="omsTemplateId">Oms template id.</param>
        /// <returns>Returns TemplateViewModel.</returns>
        TemplateViewModel GetTemplate(int omsTemplateId, bool isClearAll);

        /// <summary>
        /// Update the template item quantity.
        /// </summary>
        /// <param name="guid">Unique id.</param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="productId">Product id.</param>
        /// <returns>Returns the template view model.</returns>
        TemplateViewModel UpdateTemplateItemQuantity(string guid, decimal quantity, int productId);

        /// <summary>
        /// Add the template to cart.
        /// </summary>
        /// <param name="omsTemplateId">OmsTemplateId</param>
        /// <returns>Returns error message if any.</returns>
        string AddTemplateToCart(int omsTemplateId);

        /// <summary>
        /// Remove cart item from template.
        /// </summary>
        /// <param name="guiId">External Id.</param>
        /// <param name="templateType">Template Type.</param>
        /// <returns>True if removed else false.</returns>
        bool RemoveTemplateCartItem(string guiId, string templateType = null);

        /// <summary>
        /// Remove all cart items from template.
        /// </summary>
        /// <param name="templateType">Template Type.</param>
        /// <returns>True if removed else false.</returns>
        bool RemoveAllTemplateCartItems(string templateType = null);

        /// <summary>
        /// Add multiple items to cart template
        /// </summary>
        /// <param name="cartItems">list of cart items</param>
        /// <returns>error/ success message.</returns>
        string AddMultipleProductsToCartTemplate(List<TemplateCartItemViewModel> cartItems);
        #endregion

        /// <summary>
        /// Add multiple items to cart
        /// </summary>
        /// <param name="cartItems">list of cart items</param>
        /// <returns>error/ success message.</returns>
        string AddMultipleProductsToCart(List<CartItemViewModel> cartItems);

        /// <summary>
        /// Check quantity for cart item.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="quantity"></param>
        void CheckCartQuantity(ProductViewModel viewModel, decimal? quantity);

        /// <summary>
        /// Get shipping estimates based on the zip code
        /// </summary>
        /// <param name="zipCode">ZipCode</param>
        /// <returns>ShippingOptionListViewModel</returns>
        ShippingOptionListViewModel GetShippingEstimates(string zipCode);

        /// <summary>
        /// Insert estimated shipping id in session
        /// </summary>
        /// <param name="shippingId">Shipping Id</param>
        void AddEstimatedShippingIdToCartViewModel(int shippingId);

        /// <summary>
        /// Insert estimated shipping details in session
        /// </summary>
        /// <param name="shippingId">Shipping Id</param>
        /// <param name="zipCode">Zip code.</param>
        void AddEstimatedShippingDetailsToCartViewModel(int shippingId, string zipCode);

        /// <summary>
        /// Create cartItem for Group type product.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="shoppingCart"></param>
        void GetSelectedGroupedProducts(CartItemViewModel cartItem, ShoppingCartModel shoppingCart);

        /// <summary>
        /// Create cartItem for Group type product.
        /// </summary>
        /// <param name="cartItem"></param>
        void GetSelectedGroupedProductsForAddToCart(AddToCartViewModel cartItem);

        /// <summary>
        /// Create cartItem for Group type product.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="bundleProductSKUs"></param>
        void GetSelectedBundleProductsForAddToCart(AddToCartViewModel cartItem, string bundleProductSKUs = null);

        /// <summary>
        /// Create cartItem for configurable type product.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="configurableProductSkus"></param>
        void GetSelectedConfigurableProductsForAddToCart(AddToCartViewModel cartItem, string configurableProductSkus = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="addOnSKUS"></param>
        void GetSelectedAddOnProductsForAddToCart(AddToCartViewModel cartItem, string addOnSKUS = null);

        /// <summary>
        /// Get the Group products
        /// </summary>
        /// <param name="simpleProduct"></param>
        /// <param name="simpleProductQty"></param>
        /// <param name="cartItem"></param>
        /// <param name="isNewExtIdRequired"></param>
        /// <returns></returns>
        ShoppingCartItemModel BindGroupProducts(string simpleProduct, string simpleProductQty, CartItemViewModel cartItem, bool isNewExtIdRequired);

        /// <summary>
        /// Bind CartItemViewModel properties to ShoppingCartModel.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="shoppingCartModel"></param>
        void BindCartItemData(CartItemViewModel cartItem, ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Get attribute values and code.
        /// </summary>
        /// <param name="cartItem"></param>
        void PersonalisedItems(CartItemViewModel cartItem);

        /// <summary>
        /// Get attribute values and code.
        /// </summary>
        /// <param name="cartItem"></param>
        void PersonalisedItemsForAddToCart(AddToCartViewModel cartItem);

        /// <summary>
        /// Check if cart persistent.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartModel"></param>
        /// <param name="sessionCart"></param>
        void CheckCartPersistent(int userId, ShoppingCartModel cartModel, ShoppingCartModel sessionCart);

        /// <summary>
        /// Add cart items to cart. 
        /// </summary>
        /// <param name="cartModel">ShoppingCartModel</param>
        /// <returns>Return the updated shopping cart item status.</returns>
        bool UpdateCart(ref ShoppingCartModel cartModel);

        /// <summary>
        /// Add cart items to cart.
        /// </summary>
        /// <param name="cartModel">ShoppingCartModel</param>
        /// <returns>Return the updated shopping cart items.</returns>
        ShoppingCartModel UpdateCartDetails(ShoppingCartModel cartModel);

        /// <summary>
        /// Merge Cart after login
        /// </summary>
        /// <returns>Return true if cart gets merged</returns>
        bool MergeGuestUserCart();

        /// <summary>
        /// Merge user cart after Impersonation login
        /// </summary>
        /// <returns>Return true if cart gets merged</returns>
        bool MergeUserCartAfterImpersonationLogin();
        /// <summary>
        /// Get cart
        /// </summary>
        /// <returns>CartViewModel</returns>
        CartViewModel SetQuoteCart(int omsQuoteId);

        #region AmazonPay

        /// <summary>
        /// Calculate shipping for AmazonPay shopping cart.
        /// </summary>
        /// <param name="shippingOptionId">shipping Option Id</param>
        /// <param name="shippingAddressId">shipping Address Id</param>
        /// <param name="shippingCode">shipping Code</param>
        /// <param name="addressViewModel">address View Model</param>
        /// <returns>CartViewModel</returns>
        CartViewModel CalculateAmazonShipping(int shippingOptionId, int shippingAddressId, string shippingCode, AddressViewModel addressViewModel);

        /// <summary>
        /// Bind shopping cart values.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        ShoppingCartModel GetShoppingCart(CartItemViewModel cartItem, ShoppingCartModel shoppingCart);

        /// <summary>
        /// Bind shopping cart values.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        AddToCartViewModel GetShoppingCartValues(AddToCartViewModel cartItem);

        #endregion

        /// <summary>
        /// To remove the cart count key from session.
        /// </summary>
        void ClearCartCountFromSession();
        /// <summary>
        /// It check session template cart items values with database values and return true if both are match
        /// </summary>
        /// <param name="omsTemplateId">Template Id</param>
        /// <returns>Return true if session values and database are match</returns>
        bool IsTemplateItemsModified(int omsTemplateId);

        /// <summary>
        /// Bind AddToCart model to display necessary information
        /// </summary>
        /// <param name="cartItem">Cart Item</param>        
        /// <returns>AddToCart model</returns>
        AddToCartNotificationViewModel BindAddToCartNotification(AddToCartViewModel cartItem);

        /// <summary>
        /// Remove voucher by voucher code
        /// </summary>
        /// <param name="voucherNumber"></param>
        /// <returns>CartViewModel</returns>
        CartViewModel RemoveVoucher(string voucherNumber);

        /// <summary>
        /// Map Quantity On Hand (Available Quantity).
        /// </summary>
        /// <param name="sourceShoppingCartModel">ShoppingCartModel</param>
        /// <param name="destShoppingCartModel">ShoppingCartModel</param>
        /// <returns>Return the updated shopping cart items.</returns>
        void MapQuantityOnHandAndSeoName(ShoppingCartModel sourceShoppingCartModel, ShoppingCartModel destShoppingCartModel);

        /// <summary>
        /// Calculate cart.
        /// </summary>
        /// <param name="shoppingCartModel"></param>
        /// <param name="isCalculateTaxAndShipping"></param>
        /// <param name="isCalculateCart"></param>
        /// <returns></returns>
        ShoppingCartModel CalculateCart(ShoppingCartModel shoppingCartModel, bool isCalculateTaxAndShipping = true, bool isCalculateCart = true);

        /// <summary>
        /// Get Cart Items
        /// </summary>
        /// <returns>Return list type="of cart items</returns>
        ShoppingCartModel GetCartItems();

        /// <summary>
        /// Add the product to cart for Save for Later
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>
        /// <param name="omsTemplateLineItemId">omsTemplateLineItemId</param>
        /// <returns>return error message if any.</returns>
        string AddProductToCartForLater(int omsTemplateId, int omsTemplateLineItemId);

        /// <summary>
        /// Check Template Data
        /// </summary>
        /// <param name="accountTemplateModel"></param>
        /// <returns>return error message if any.</returns>
        string CheckTemplateData(AccountTemplateModel accountTemplateModel);
    }
}
