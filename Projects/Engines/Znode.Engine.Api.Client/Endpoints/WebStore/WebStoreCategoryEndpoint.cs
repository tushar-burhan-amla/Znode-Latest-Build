namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebStoreCategoryEndpoint : BaseEndpoint
    {
        //Get WebStore Category, SubCategory and Product list by Category Name Endpoint.
        public static string GetPublishedCategoryProductList() => $"{ApiRoot}/webstorecategory/getcategorydetails";
    }
}
