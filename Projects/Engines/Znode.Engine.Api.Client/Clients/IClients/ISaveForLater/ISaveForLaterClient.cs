using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ISaveForLaterClient : IBaseClient
    {
        /// <summary>
        /// Create save for later cart
        /// </summary>
        /// <param name="accountTemplateModel">accountTemplateModel</param>
        /// <returns>Returns AccountTemplateModel</returns>
        AccountTemplateModel CreateSaveForLater(AccountTemplateModel accountTemplateModel);

        /// <summary>
        /// Get save for later template.
        /// </summary>
        /// <param name="userId">LoggedIn user Id</param>
        /// <param name="templateType">Template type</param>
        /// <returns>Returns AccountTemplateModel</returns>
        AccountTemplateModel GetSaveForLaterTemplate(int userId, string templateType, ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Delete Cart lineItem
        /// </summary>
        /// <param name="accountTemplateModel">Account template model</param>
        /// <returns></returns>
        bool DeleteCartItem(AccountTemplateModel accountTemplateModel);

        /// <summary>
        /// Delete all cart line items
        /// </summary>
        /// <param name="omsTemplateId"></param>
        /// /// <param name="isFromSavedCart">isFromSavedCart</param>
        /// <returns></returns>
        bool DeleteAllCartItems(int omsTemplateId, bool isFromSavedCart = false);

    }
}
