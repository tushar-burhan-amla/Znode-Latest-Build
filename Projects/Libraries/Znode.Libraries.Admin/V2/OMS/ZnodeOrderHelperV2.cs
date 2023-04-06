using Znode.Engine.Api.Models;

namespace Znode.Libraries.Admin
{
    public class ZnodeOrderHelperV2 : ZnodeOrderHelper
    {
        protected override SavedCartLineItemModel BindSavedCartLineItemModel(string cookieMappingId, int savedCartId, ShoppingCartItemModel shoppingCartItem, int publishCatalogId, int localeId)//, int itemSequence)
        {
            SavedCartLineItemModel savedCartLineItemModel = base.BindSavedCartLineItemModel(cookieMappingId, savedCartId, shoppingCartItem, publishCatalogId, localeId);//, itemSequence);
            savedCartLineItemModel.CustomText = shoppingCartItem.CustomText;
            return savedCartLineItemModel;
        }
    }
}