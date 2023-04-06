using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
namespace Znode.Engine.WebStore
{
    public static class SEOUrl
    {
        //Get Category Url.
        public static string GetCategoryUrl(this UrlHelper url, string page, string category, int? categoryId)
        {
            if (!string.IsNullOrEmpty(category))
            {
                if (!Regex.IsMatch(category, @"^[a-zA-Z0-9@.]*$@"))
                    category = Regex.Replace(category, "[^a-zA-Z0-9]", string.Empty);
            }

            if (string.IsNullOrEmpty(category))
                return string.IsNullOrEmpty(page)
                                  ? url.RouteUrl("category-detailsId", new {categoryId = categoryId })
                                  : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });

            if (HelperUtility.IsNotNull(categoryId))
                return string.IsNullOrEmpty(page)
                    ? url.RouteUrl("category-details", new { category = category, categoryId = categoryId })
                    : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
            else
                return string.IsNullOrEmpty(page)
                  ? url.RouteUrl("category-details", new { category = category })
                  : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
        }

        //Get product url.
        public static string GetProductUrl(this UrlHelper url, string page, string id, string sku = "")
        {
            if (string.IsNullOrEmpty(sku))
                return string.IsNullOrEmpty(page)
                   ? url.RouteUrl("product-details", new { id })
                    : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
            else
                return url.RouteUrl("product-details-sku", new { id, sku });
        }

        //Get content page url.
        public static string GetContentPageUrl(this UrlHelper url, string page, string name, int? pageId)
        {
            if (HelperUtility.IsNotNull(pageId))
                return string.IsNullOrEmpty(page)
                    ? url.RouteUrl("content-details", new { contentPageName = name, contentPageId = pageId })
                    : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
            else
                return string.IsNullOrEmpty(page)
                  ? url.RouteUrl("content-details", new { contentPageName = name })
                  : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
        }

        //Get content page url.
        public static string GetContentPageUrl(this UrlHelper url, string page, int? pageId)
        {
            if (HelperUtility.IsNotNull(pageId))
                return string.IsNullOrEmpty(page)
                    ? url.RouteUrl("content-details", new { contentPageId = pageId })
                    : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
            else

                return url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
        }

        //Get Brand Url.
        public static string GetBrandUrl(this UrlHelper url, string page, string brand, int? brandId)
        {
            if (!string.IsNullOrEmpty(brand))
            {
                if (!Regex.IsMatch(brand, @"^[a-zA-Z0-9@.]*$@"))
                    brand = Regex.Replace(brand, "[^a-zA-Z0-9]", string.Empty);
            }
            if (string.IsNullOrEmpty(brand))
                return string.IsNullOrEmpty(page)
                                  ? url.RouteUrl("brand-detailsid", new { brandId = brandId.GetValueOrDefault() })
                                  : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });

            if (HelperUtility.IsNotNull(brandId))
                return string.IsNullOrEmpty(page)
                    ? url.RouteUrl("brand-details", new { brand = brand, brandId = brandId })
                    : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
            else
                return string.IsNullOrEmpty(page)
                  ? url.RouteUrl("brand-details", new { brand = brand })
                  : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
        }

        //Get blog/news url.
        public static string GetBlogNewsUrl(this UrlHelper url, string page, string name, int? blogNewsId, string type)
        {
            if (ZnodeWebstoreSettings.SEOSlugToSkip.Split(',').Contains(page))
                page = null;

            if (type == WebStoreConstants.News)
            {
                if (HelperUtility.IsNotNull(blogNewsId))
                    return string.IsNullOrEmpty(page)
                        ? url.RouteUrl("news-details", new { blogNewsId = blogNewsId })
                        : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
                else
                    return string.IsNullOrEmpty(page)
                      ? url.RouteUrl("news-details", new { name = name })
                      : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
            }
            else
            {
                if (HelperUtility.IsNotNull(blogNewsId))
                    return string.IsNullOrEmpty(page)
                        ? url.RouteUrl("blog-details", new { blogNewsId = blogNewsId })
                        : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
                else
                    return string.IsNullOrEmpty(page)
                      ? url.RouteUrl("blog-details", new { name = name })
                      : url.RouteUrl("SeoSlug", new { slug = page.ToLower() });
            }
        }
    }
}