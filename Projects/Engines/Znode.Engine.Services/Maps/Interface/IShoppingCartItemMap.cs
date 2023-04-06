using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
namespace Znode.Engine.Services.Maps
{
    public interface IShoppingCartItemMap
    {
        //Get image path.
        string GetImagePath(string imageName, int portalId, IImageHelper objImage = null);

        //to get product type comma separated skus
        string GetProductTypeSKUs(ZnodeProductBaseEntity product, ZnodeCartItemRelationshipTypeEnum productType);

        ShoppingCartItemModel ToModel(Znode.Libraries.ECommerce.ShoppingCart.ZnodeShoppingCartItem znodeCartItem, ZnodeShoppingCart znodeCart, IImageHelper objImage = null);

        //to get associated product list from product entity
        List<AssociatedProductModel> GetAssociateProducts(ZnodeProductBaseEntity product, ZnodeCartItemRelationshipTypeEnum productType);

        //Get tier price.
        decimal GetUnitPriceForGroupProduct(ZnodeProductBaseEntity item);

        //to get group product list from product entity
        List<AssociatedProductModel> GetUnitPriceForGroupProduct(ZnodeProductBaseEntity product, ZnodeCartItemRelationshipTypeEnum productType);

        //to get group product list from product entity
        List<AssociatedProductModel> GetGroupProducts(ZnodeProductBaseEntity product, ZnodeCartItemRelationshipTypeEnum productType);

        //Bind the details of product of cartitem.
        void BindProductDetails(ShoppingCartItemModel cartItem, ZnodeProductBaseEntity znodeProduct);

        //Map ZNodeShoppingCartItem to ShoppingCartItemModel.
        void ToShoppingCartItemModel(ShoppingCartItemModel cartItem, ZnodeShoppingCartItem znodeCartItem);

        //to check product price greater than zero
        bool CheckProductPrice(bool insufficientQuantity, ZnodeProductBaseEntity product);

        //Set product attributes.
        void SetProductAttributes(ShoppingCartItemModel cartItem, List<OrderAttributeModel> orderAttributeModel);
    }

}