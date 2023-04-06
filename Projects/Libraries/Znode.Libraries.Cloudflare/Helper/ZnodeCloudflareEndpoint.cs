namespace Znode.Libraries.Cloudflare
{
    public class ZnodeCloudflareEndpoint
    {
        //Create SEO url as endpoint to purge the ul
        public static string SeoUrl(string seoUrl) => $"{seoUrl}";

        //Create url of product to purge the url
        public static string ProductIdUrl(int productId) => $"product/{productId}";

        //Create url of category to purge the url
        public static string CategoryIdUrl(int categoryId) => $"Category/{categoryId}";
    }
}
