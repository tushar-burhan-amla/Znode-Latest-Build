namespace Znode.Engine.Api.Client.Endpoints
{
    public class CategoryEndpoint : BaseEndpoint
    {
        //Get category list endpoint
        public static string GetCategoryList() => $"{ApiRoot}/category/list";

        //Get category endpoint
        public static string Get(int categoryId, int familyId, int localeId) => $"{ApiRoot}/category/getcategory/{categoryId}/{familyId}/{localeId}";

        //Create category endpoint
        public static string Create() => $"{ApiRoot}/category/create";

        //Update category endpoint
        public static string Update() => $"{ApiRoot}/category/update";

        //Delete category endpoint
        public static string Delete() => $"{ApiRoot}/category/delete";
  
        //Associate Category to Product
        public static string AssociateCategoryProduct() => $"{ApiRoot}/category/associatecategoryproduct";

        //Associate Categories to Products 
        public static string AssociateCategoriesToProduct() => $"{ApiRoot}/category/associateCategoriesToproduct";

        //Delete Category Products
        public static string DeleteCategoryProduct() => $"{ApiRoot}/category/deletecategoryproduct";

        //Delete associated categories from Product.
        public static string DeleteAssociatedCategoriesToProduct() => $"{ApiRoot}/category/deleteassociatedcategoriestoproduct";

        //Get unassociated Category Product endpoint
        public static string GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProducts) => $"{ApiRoot}/category/getassociatedunassociatedcategoryproducts/{categoryId}/{associatedProducts}";

        //Get Associate Category Product endpoint
        public static string GetAssociatedCategoryProducts(int categoryId, bool associatedProducts) => $"{ApiRoot}/category/getassociatedcategoryproducts/{categoryId}/{associatedProducts}";

        /// Get the list of associated categories to Product
        public static string GetAssociatedCategoriesToProduct(int productId, bool associatedProducts) => $"{ApiRoot}/category/getassociatedcategoriestoproducts/{productId}/{associatedProducts}";

        //Publish category endpoint
        public static string Publish() => $"{ApiRoot}/category/publish";

        //Update product details associated to category.
        public static string UpdateCategoryProductDetail() => $"{ApiRoot}/category/updatecategoryproductdetail";
        
    }
}
