using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.WebStore.Core.Agents
{
    public interface ISaveForLaterAgent
    {
        /// <summary>
        /// Get template.
        /// </summary>
        /// <param name="omsTemplateId">Oms template id.</param>
        /// <returns>Returns TemplateViewModel.</returns>
        TemplateViewModel GetSavedCartForLater();

        /// <summary>
        /// Remove single line item from the saved cart later
        /// </summary>
        /// <param name="omsTemplateId">OmsTemplateId</param>
        /// <param name="OmsTemplateLineItemId">OmsTemplateLineItemId</param>
        /// <returns>returns true or false is successfully remove the cart line item</returns>
        bool RemoveSingleCartLineItem(int omsTemplateId, int omsTemplateLineItemId);

        /// <summary>
        /// Remove all line items from the saved cart later
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>
        /// /// <param name="isFromSavedCart">isFromSavedCart</param>
        /// <returns>returns true or false is successfully remove the cart line item</returns>
        bool RemoveAllTemplateCartItems(int omsTemplateId, bool isFromSavedCart = false);

        /// <summary>
        /// Create cart for later
        /// </summary>
        /// <param name="Guid">Guid</param>
        /// <returns>Returns TemplateViewModel if cart created successfully</returns>
        TemplateViewModel CreateCartForLater(string guiId);

        /// <summary>
        /// Set Selected Bundle Products For AddToCart
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="templateModel"></param>
        /// <param name="bundleProductSKUs"></param>
        void SetSelectedBundleProductsForAddToCart(ShoppingCartItemModel cartItem, AccountTemplateModel templateModel, string bundleProductSKUs = null);

        /// <summary>
        /// Set Unit Price With Addon
        /// </summary>
        /// <param name="cartItem">cartItem</param>
        /// <param name="product">product</param>
        void SetUnitPriceWithAddon(TemplateCartItemViewModel cartItem, ProductViewModel product);


    }
}
