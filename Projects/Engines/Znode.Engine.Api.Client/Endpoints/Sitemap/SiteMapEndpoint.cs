namespace Znode.Engine.Api.Client.Endpoints
{
    public class SiteMapEndpoint : BaseEndpoint
    {

        //Get sitemap category list.
        public static string GetSitemapCategoryList(bool includeAssociatedCategories) => $"{ApiRoot}/sitemap/getsitemapcategorylist/{includeAssociatedCategories}";

        //Get sitemap brand list.
        public static string GetSitemapBrandList() => $"{ApiRoot}/sitemap/getsitemapbrandlist";

        //Get sitemap product list.
        public static string GetSitemapProductList() => $"{ApiRoot}/sitemap/getsitemapproductlist";


    }
}
