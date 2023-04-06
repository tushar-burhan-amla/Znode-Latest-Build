using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Cloudflare;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services.Helper
{
    public class CloudflareHelper
    {
        /// <summary>
        /// Purge all cache from cloudflare server based on portalId
        /// </summary>
        /// <param name="portalId">Pass a portalId for which cache needs to purge</param>
        public void PurgeEverythingByPortalId(string portalId)
        {
            if (string.IsNullOrEmpty(portalId))
            {
                return;
            }

            IZnodeCloudflare znodeCloudflare = GetService<IZnodeCloudflare>();
            znodeCloudflare?.PurgeEverythingByPortalId(portalId);
        }

        /// <summary>
        /// Purge all cache from cloudflare server based on domainId
        /// </summary>
        /// <param name="domainIds">Pass a domainIds for which cache needs to purge</param>
        /// <returns></returns>
        public List<CloudflareErrorResponseModel> PurgeEverythingByDomainIds(string domainIds)
        {
            if (string.IsNullOrEmpty(domainIds))
            {
                return new List<CloudflareErrorResponseModel>();
            }

            IZnodeCloudflare znodeCloudflare = GetService<IZnodeCloudflare>();
            return znodeCloudflare?.PurgeEverything(domainIds);
        }

        /// <summary>
        /// Get the list of domains where cloudflare setting is enabled
        /// </summary>
        /// <returns></returns>
        public List<DomainModel> GetCloudflareEnableDomains()
        {
            IZnodeCloudflare service = GetService<IZnodeCloudflare>();
            return service?.GetCloudflareEnableDomains();
        }

        /// <summary>
        /// Purge the urls contains based on portal id on publish product event
        /// </summary>
        /// <param name="model">ProductEntity model contains data related to product</param>
        /// <param name="portalIds">portalIds where product is associated</param>
        public void PurgeUrlContentsOnProductPublish(IEnumerable<PublishedProductEntityModel> model, List<int> portalIds)
        {
            if (HelperUtility.IsNull(model) || portalIds.Count < 1)
            {
                return;
            }

            PurgeUrlContentsByPortalId(new CloudflarePurgeModel() { PortalId = portalIds, Id = model.Select(x => x.ZnodeProductId).ToList(), SeoType = SEODetailsEnum.Product, SeoUrl = model.Where(seo => seo.SeoUrl != null).Select(x => x.SeoUrl).ToList() });
        }

        /// <summary>
        /// Purge the urls contains based on portal id on category publish event
        /// </summary>
        /// <param name="model">CategoryPublishEventModel contains data of category with associated portalId</param>
        public void PurgeUrlContentsOnCategoryPublish(List<CategoryPublishEventModel> model)
        {
            if (HelperUtility.IsNull(model))
            {
                return;
            }

            PurgeUrlContentsByPortalId(new CloudflarePurgeModel() { PortalId = model.Select(x => x.PortalId).ToList(), Id = model.Select(x => Convert.ToInt32(x.CategoryId)).ToList(), SeoType = SEODetailsEnum.Category, SeoUrl = model.Where(seo => seo.SeoUrl != null).Select(x => x.SeoUrl).ToList() });
        }

        /// <summary>
        /// Purge the urls contains based on portal id on publish events
        /// </summary>
        /// <param name="cloudflarePurgeModel">cloudflarePurgeModel contains data of portalId, SEO urls & Ids</param>
        /// <returns></returns>
        public void PurgeUrlContentsByPortalId(CloudflarePurgeModel cloudflarePurgeModel)
        {
            try
            {
                if (HelperUtility.IsNull(cloudflarePurgeModel))
                {
                    return;
                }

                List<string> urlPatterns = GetUrlPatterns(cloudflarePurgeModel);

                if (urlPatterns.Count <= 0)
                {
                    return;
                }

                HttpContext httpContext = HttpContext.Current;
                Action threadWorker = delegate ()
                {
                    HttpContext.Current = httpContext;
                    IZnodeCloudflare service = GetService<IZnodeCloudflare>();
                    service.PurgeUrlContentsByPortalId(cloudflarePurgeModel.PortalId, urlPatterns);
                };
                AsyncCallback cb = new AsyncCallback(CloudflarePurgeSuccess);
                threadWorker.BeginInvoke(cb, null);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Engine.Services.Helper.CloudflareHelper.PurgeUrlContentsByPortalId error -" + ex, "Cloudflare", TraceLevel.Error);
            }
        }

        protected virtual void CloudflarePurgeSuccess(IAsyncResult asyncResult)
        {
            AsyncResult result = asyncResult as AsyncResult;
        }

        /// <summary>
        /// Generate the url pattern for product and category which will be use to purge the cache from cloudflare server.
        /// </summary>
        /// <param name="cloudflarePurgeModel">cloudflarePurgeModel contains data of portalId, SEO urls & Ids</param>
        /// <returns></returns>
        protected virtual List<string> GetUrlPatterns(CloudflarePurgeModel cloudflarePurgeModel)
        {
            if (HelperUtility.IsNull(cloudflarePurgeModel))
            {
                return new List<string>();
            }

            List<string> urlPatterns = new List<string>();
            switch (cloudflarePurgeModel.SeoType)
            {
                case SEODetailsEnum.Product:
                    GenerateProductUrlPattern(cloudflarePurgeModel, urlPatterns);
                    break;
                case SEODetailsEnum.Category:
                    GenerateCategoryUrlPatterns(cloudflarePurgeModel, urlPatterns);
                    break;
            }

            return urlPatterns;
        }

        /// <summary>
        /// Generate the url pattern for product which will be use to purge the cache from cloudflare server.
        /// </summary>
        /// <param name="cloudflarePurgeModel">cloudflarePurgeModel contains data of portalId, SEO urls & Ids</param>
        /// <param name="urlPatterns">instance of an List<string></param>
        /// <returns></returns>
        private void GenerateProductUrlPattern(CloudflarePurgeModel cloudflarePurgeModel, List<string> urlPatterns)
        {
            if (HelperUtility.IsNull(cloudflarePurgeModel))
            {
                return;
            }

            List<string> seoUrls = cloudflarePurgeModel.SeoUrl;
            if (HelperUtility.IsNotNull(seoUrls) && seoUrls.Count > 0)
            {
                foreach (string seoUrl in seoUrls)
                {
                    urlPatterns.Add(ZnodeCloudflareEndpoint.SeoUrl(seoUrl));
                }
            }

            if (cloudflarePurgeModel.Id.Count > 0)
            {
                foreach (int id in cloudflarePurgeModel.Id)
                {
                    urlPatterns.Add(ZnodeCloudflareEndpoint.ProductIdUrl(id));
                }
            }
        }

        /// <summary>
        /// Generate the url pattern for category which will be use to purge the cache from cloudflare server.
        /// </summary>
        /// <param name="cloudflarePurgeModel">cloudflarePurgeModel contains data of portalId, SEO urls & Ids</param>
        /// <param name="urlPatterns">instance of an List<string></param>
        /// <returns></returns>
        private void GenerateCategoryUrlPatterns(CloudflarePurgeModel cloudflarePurgeModel, List<string> urlPatterns)
        {
            if (HelperUtility.IsNull(cloudflarePurgeModel))
            {
                return;
            }

            List<string> seoUrls = cloudflarePurgeModel.SeoUrl;

            if (HelperUtility.IsNull(seoUrls))
            {
                foreach (string seoUrl in seoUrls)
                {
                    urlPatterns.Add(ZnodeCloudflareEndpoint.SeoUrl(seoUrl));
                }
            }
            if (cloudflarePurgeModel.Id.Count > 0)
            {
                foreach (int id in cloudflarePurgeModel.Id)
                {
                    urlPatterns.Add(ZnodeCloudflareEndpoint.CategoryIdUrl(id));
                }
            }
        }
    }
}
