using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class SEOCache : BaseCache, ISEOCache
    {
        private readonly ISEOService _service;

        public SEOCache(ISEOService seoService)
        {
            _service = seoService;
        }

        public virtual string GetPortalSEOSetting(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                PortalSEOSettingModel portalSeoSetting = _service.GetPortalSEOSetting(portalId);
                if (IsNotNull(portalSeoSetting))
                {
                    PortalSEOSettingResponse response = new PortalSEOSettingResponse { PortalSEOSetting = portalSeoSetting };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetSEODetails(int seoDetailId, int seoTypeId, int localeId, int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                SEODetailsModel seoDetails = _service.GetSEODetails(seoDetailId, seoTypeId, localeId, portalId);
                if (IsNotNull(seoDetails))
                {
                    SEODetailsResponse response = new SEODetailsResponse { SEODetail = seoDetails };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetSEODetailsBySEOCode(string seoCode, int seoTypeId, int localeId, int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                SEODetailsModel seoDetails = _service.GetSEODetailsBySEOCode(seoCode, seoTypeId, localeId, portalId);
                if (IsNotNull(seoDetails))
                {
                    SEODetailsResponse response = new SEODetailsResponse { SEODetail = seoDetails };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetDefaultSEODetails(string seoCode, int seoTypeId, int localeId, int portalId, int itemId,string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                SEODetailsModel seoDetails = _service.GetDefaultSEODetails(seoCode, seoTypeId, localeId, portalId, itemId);
                if (IsNotNull(seoDetails))
                {
                    SEODetailsResponse response = new SEODetailsResponse { SEODetail = seoDetails };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
      
        public virtual string GetPublishSEODetails(int seoDetailId, string seoType, int localeId, int portalId,string seoCode, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                SEODetailsModel seoDetails = _service.GetPublishSEODetails(seoDetailId, seoType, localeId, portalId, seoCode);
                if (IsNotNull(seoDetails))
                {
                    SEODetailsResponse response = new SEODetailsResponse { SEODetail = seoDetails };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }


        //Get Seo details list. 
        public virtual string GetSeoDetailsList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                SEODetailsListModel model = _service.GetSEODetailsList(Expands, Filters, Sorts, Page);
                if (model?.SEODetailsList?.Count > 0)
                {
                    SEODetailsListResponse response = new SEODetailsListResponse { SEODetails = model.SEODetailsList };
                    response.MapPagingDataFromModel(model);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get list of Category for SEO.
        public virtual string GetCategoryListForSEO(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Category list.
                PublishCategoryListModel list = _service.GetCategoryListForSEO(Expands, Filters, Sorts, Page);

                if (list?.PublishCategories?.Count > 0)
                {
                    //Create response.
                    PublishCategoryListResponse response = new PublishCategoryListResponse { PublishCategories = list.PublishCategories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Products list for SEO.
        public virtual string GetProductsForSEO(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish product list.
                PublishProductListModel list = _service.GetProductsForSEO(Expands, Filters, Sorts, Page);

                if (list?.PublishProducts?.Count > 0)
                {
                    //Create response.
                    PublishProductListResponse response = new PublishProductListResponse { PublishProducts = list.PublishProducts };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}