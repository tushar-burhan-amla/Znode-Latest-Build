using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.WebStore.Core.Agents
{
    public interface ISavedCartAgent
    {
        /// <summary>
        /// create new saved cart.
        /// </summary>
        /// <param name="templateName">string</param>
        /// <returns>TemplateViewModel</returns>
        TemplateViewModel CreateSavedCart(string templateName);

        /// <summary>
        /// get saved cart.
        /// </summary>
        /// <returns>DropdownListModel</returns>
        List<DropdownListModel> GetSavedCart();

        /// <summary>
        /// edit saved cart.
        /// </summary>
        /// <param name="templateId">templateId</param>
        /// <returns>Status</returns>
        bool EditSaveCart(int templateId);

        /// <summary>
        /// Add Product To Cart For Save Cart
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>
        /// <returns>Status</returns>
        string AddProductToCartForSaveCart(int omsTemplateId);

        /// <summary>
        /// Update Saved Item Quantity
        /// </summary>
        /// <param name="guid">string</param>
        /// <param name="quantity">decimal</param>
        /// <param name="productId">int</param>
        /// <returns>TemplateViewModel</returns>
        TemplateViewModel UpdateSavedItemQuantity(string guid, decimal quantity, int productId);

        /// <summary>
        /// Edit Saved Cart Item
        /// </summary>
        /// <param name="templateViewModel">TemplateViewModel to edit the save cart items</param>
        /// <returns>bool</returns>
        bool EditSavedCartItem(TemplateViewModel templateViewModel);

        /// <summary>
        /// Edit Save Cart Name
        /// </summary>
        /// <param name="templateName">templateName</param>
        /// <param name="templateId">templateId</param>
        /// <returns>Status</returns>
        bool EditSaveCartName(string templateName, int templateId);
    }
}