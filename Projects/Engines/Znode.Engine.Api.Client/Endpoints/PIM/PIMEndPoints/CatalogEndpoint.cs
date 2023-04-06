using System;

namespace Znode.Engine.Api.Client.Endpoints
{
    public class CatalogEndpoint : BaseEndpoint
    {
        //Get category list endpoint
        public static string GetCatalogList() => $"{ApiRoot}/catalogs";

        //Get category endpoint
        public static string Get(int pimCatalogId) => $"{ApiRoot}/catalog/{pimCatalogId}";

        //Create category endpoint
        public static string Create() => $"{ApiRoot}/catalog";

        //Update category endpoint
        public static string Update() => $"{ApiRoot}/catalog/update";

        //Copy category endpoint
        public static string CopyCatalog() => $"{ApiRoot}/catalog/copycatalog";

        //Delete category endpoint
        public static string Delete() => $"{ApiRoot}/catalog/delete";

        //created endpoint for getting category structure for tree.
        public static string GetCategoryTree() => $"{ApiRoot}/catalog/getcategorytree";

        //Get Associate Catalog Tree
        public static string GetCatalogCategoryTree(int pimProductId) => $"{ApiRoot}/catalog/getAssociatedCatalogTree/{pimProductId}";

        //created endpoint Associate categories to catalog.
        [Obsolete("This method is not in use now, as new method has been introduced to associate category")]
        public static string AssociateCategory() => $"{ApiRoot}/catalog/associatecategory";

        //Get category list endpoint
        public static string GetAssociatedCategoryList() => $"{ApiRoot}/catalogs/getassociatedcategorylist";

        //created endpoint unAssociate categories to catalog.
        [Obsolete("This method is not in use now, as new method has been introduced to unassociate category")]
        public static string UnAssociateCategory() => $"{ApiRoot}/catalog/unassociatecategory";

        //created endpoint unAssociate products to catalog.
        [Obsolete("This method is not in use now, As product association/unassociation has been removed from catalog category")]
        public static string UnAssociateProduct() => $"{ApiRoot}/catalog/unassociateproduct";

        //Get product list endpoint
        public static string GetAssociatedProductList(int catalogId) => $"{ApiRoot}/catalogs/getassociatedproductlist/{catalogId}";

        //created endpoint associate products to catalog.
        public static string AssociateProducts() => $"{ApiRoot}/catalog/associateproducts";

        //created endpoint unAssociate products to catalog.
        public static string UnAssociateProducts() => $"{ApiRoot}/catalog/unassociateproducts";

        //Get product list associated to category endpoint
        public static string GetCategoryAssociatedProducts() => $"{ApiRoot}/catalogs/getcategoryassociatedproducts";

        //Publish Catalog  with associated product which has draft status.
        public static string Publish(int pimCatalogId, string revisionType) => Publish(pimCatalogId, revisionType, isDraftProductsOnly: true);


        //Publish Catalog operation with associated productstatus matched with isDraftProductsOnly flag passed..
        public static string Publish(int pimCatalogId, string revisionType,bool isDraftProductsOnly) => $"{ApiRoot}/catalog/publish/{pimCatalogId}/{revisionType}/{isDraftProductsOnly}";

        //Publish catalog category associated products.
        public static string PublishCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId, string revisionType) => $"{ApiRoot}/catalog/publishcatalogcategoryproducts/{pimCatalogId}/{pimCategoryHierarchyId}/{revisionType}";

        //Get Associate Category endpoint
        public static string GetAssociateCategoryDetails() => $"{ApiRoot}/catalog/getassociatecategorydetails";

        //Update Associate Category endpoint
        public static string UpdateAssociateCategoryDetails() => $"{ApiRoot}/catalog/updateassociatecategorydetails";

        //Endpoint for moving folder to another folder.
        public static string MoveFolder() => $"{ApiRoot}/catalog/movecategory";

        //Get Catalog Publish Status list endpoint
        public static string GetCatalogPublishStatus() => $"{ApiRoot}/catalogs/getcatalogpublishstatus";

        //Update product associated to the catalog.
        public static string UpdateCatalogCategoryProduct() => $"{ApiRoot}/catalog/updatecatalogcategoryproduct";

        //Endpoint to associate products to catalog-category.
        public static string AssociateProductsToCatalogCategory() => $"{ApiRoot}/catalog/associateproductstocatalogcategory";

        //Endpoint to unassociate products to catalog-category.
        public static string UnAssociateProductsFromCatalogCategory() => $"{ApiRoot}/catalog/unassociateproductsfromcatalogcategory";

        //Endpoint to associate category(s) to catalog tree
        public static string AssociateCategoryToCatalog() => $"{ApiRoot}/catalog/associatecategorytocatalog";

        //Endpoint to unassociate the category(s) from catalog.
        public static string UnAssociateCategoryFromCatalog() => $"{ApiRoot}/catalog/unassociatecategoryfromcatalog";

        // Gets a catalog details by Catalog Code
        public static string GetCatalogByCatalogCode(string catalogcode) => $"{ApiRoot}/catalog/getcatalogbycatalogcode/{catalogcode}";

        //Delete catalog by Code
        public static string DeleteCatalogByCode(int accountCode) => $"{ApiRoot}/catalog/deletecatalogbycode";


    }
}
