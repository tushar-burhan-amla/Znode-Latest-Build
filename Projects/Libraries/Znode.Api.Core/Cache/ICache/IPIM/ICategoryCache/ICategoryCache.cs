namespace Znode.Engine.Api.Cache
{
    public interface ICategoryCache
    {
        /// <summary>
        /// Get a list of all categories.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCategories(string routeUri, string routeTemplate);

        /// <summary>
        /// Get category using categoryId.
        /// </summary>
        /// <param name="categoryId">CategoryId use to retrieve category</param>
        /// <param name="familyId">Family id of category</param>
        /// <param name="localeId">locale id of category</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCategory(int categoryId, int familyId, int localeId, string routeUri, string routeTemplate);

       
        /// <summary>
        /// Get associated or unassociated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProducts">Set true for associatedProduts and false for unAssociatedProduts</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns></returns>
        string GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProducts, string routeUri, string routeTemplate);
         
        /// <summary>
        /// Get associated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProducts">associatedProducts</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns></returns>
        string GetAssociatedCategoryProducts(int categoryId, bool associatedProducts, string routeUri, string routeTemplate);
        string GetAssociatedCategoriesToProduct(int productId, bool associatedProducts, string routeUri, string routeTemplate);
        
    }
}