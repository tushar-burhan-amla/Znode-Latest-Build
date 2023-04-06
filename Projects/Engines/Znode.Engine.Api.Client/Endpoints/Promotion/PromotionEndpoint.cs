namespace Znode.Engine.Api.Client.Endpoints
{
    public class PromotionEndpoint : BaseEndpoint
    {
        //Get promotion by promotionId endpoint.
        public static string GetPromotionById(int promotionId) => $"{ApiRoot}/promotion/getpromotion/{promotionId}";

        //Get promotion list endpoint.
        public static string GetPromotionList() => $"{ApiRoot}/promotion/list";

        //Get Coupon List
        public static string GetCouponList() => $"{ApiRoot}/promotion/getcouponlist";

        //Create promotion endpoint.
        public static string Create() => $"{ApiRoot}/promotion/create";

        //Update promotion endpoint.
        public static string Update() => $"{ApiRoot}/promotion/update";

        //Delete promotion endpoint.
        public static string Delete() => $"{ApiRoot}/promotion/delete";

        //Get published category list endpoint
        public static string GetPublishedCategories() => $"{ApiRoot}/promotion/publishedcategories";

        //Get published product list endpoint
        public static string GetPublishedProducts() => $"{ApiRoot}/promotion/publishedproducts";

        //Get coupon as per filter passed endpoint.
        public static string GetCoupon() => $"{ApiRoot}/promotion/getcoupon";

        //Get promotion attribute on changing discount type
        public static string GetPromotionAttribute(string discountName) => $"{ApiRoot}/promotion/getpromotionattribute/{discountName}";

        //Associate catelog to already created promotion. 
        public static string AssociateCatalogToPromotion() => $"{ApiRoot}/promotion/associatecatalogtopromotion";

        //Associate category to already created promotion. 
        public static string AssociateCategoryToPromotion() => $"{ApiRoot}/promotion/associatecategorytopromotion";

        //Associate product to already created promotion. 
        public static string AssociateProductToPromotion() => $"{ApiRoot}/promotion/associateproducttopromotion";

        //Get associated or unassociated Product list on the basis of isAssociatedProduct
        public static string GetAssociatedUnAssociatedProductList(bool isAssociatedProduct) => $"{ApiRoot}/promotion/getassociatedunassociatedproductlist/{isAssociatedProduct}";

        //Get associated or unassociated category list on the basis of isAssociatedCategory
        public static string GetAssociatedUnAssociatedCategoryList(bool isAssociatedCategory) => $"{ApiRoot}/promotion/getassociatedunassociatedcategorylist/{isAssociatedCategory}";

        //Get associated or unassociated Catalog list on the basis of isAssociatedCatalog
        public static string GetAssociatedUnAssociatedCatalogList(bool isAssociatedCatalog) => $"{ApiRoot}/promotion/getassociatedunassociatedcataloglist/{isAssociatedCatalog}";

        //Removes a product type association entry from promotion.
        public static string UnAssociateProduct(int promotionId) => $"{ApiRoot}/promotion/unassociateproduct/{promotionId}";

        //Removes a Category type association entry from promotion.
        public static string UnAssociateCategory(int promotionId) => $"{ApiRoot}/promotion/unassociateCategory/{promotionId}";

        //Removes a Catalog type association entry from promotion.
        public static string UnAssociateCatalog(int promotionId) => $"{ApiRoot}/promotion/unassociateCatalog/{promotionId}";

        //Associate Brand to already created promotion. 
        public static string AssociateBrandToPromotion() => $"{ApiRoot}/promotion/associatebrandtopromotion";

        //Get associated or unassociated Brand list on the basis of isAssociatedBrand
        public static string GetAssociatedUnAssociatedBrandList(bool isAssociatedBrand) => $"{ApiRoot}/promotion/getassociatedunassociatedbrandlist/{isAssociatedBrand}";

        //Removes a Brand type association entry from promotion.
        public static string UnAssociateBrand(int promotionId) => $"{ApiRoot}/promotion/unassociatebrand/{promotionId}";

        //Associate Shipping to already created promotion. 
        public static string AssociateShippingToPromotion() => $"{ApiRoot}/promotion/associateshippingtopromotion";

        //Get associated or unassociated Shipping list on the basis of isAssociatedShipping
        public static string GetAssociatedUnAssociatedShippingList(bool isAssociatedShipping) => $"{ApiRoot}/promotion/getassociatedunassociatedshippinglist/{isAssociatedShipping}";

        //Removes a Shipping type association entry from promotion.
        public static string UnAssociateShipping(int promotionId) => $"{ApiRoot}/promotion/unassociateshipping/{promotionId}";
    }
}
