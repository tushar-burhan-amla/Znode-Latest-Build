using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ISavedCartClient : IBaseClient
    {
        /// <summary>
        /// Create Saved Cart
        /// </summary>
        /// <param name="accountTemplateModel">accountTemplateModel to Create save cart</param>
        /// <returns>AccountTemplateModel</returns>
        AccountTemplateModel CreateSavedCart(AccountTemplateModel accountTemplateModel);

        /// <summary>
        /// Edit Save Cart
        /// </summary>
        /// <param name="templateModel">templateModel to Edit Saved Cart</param>
        /// <returns>Status</returns>
        bool EditSaveCart(AccountTemplateModel templateModel);


        /// <summary>
        /// Add Product To Cart For SaveCart
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>
        /// <returns>Status</returns>
        bool AddProductToCartForSaveCart(int omsTemplateId, int userId, int portalId);

        /// <summary>
        /// Edit Save Cart Name
        /// </summary>
        /// <param name="templateName">templateName</param>
        /// <param name="templateId">templateId</param>
        /// <returns>Status</returns>
        bool EditSaveCartName(string templateName, int templateId);

    }
}
