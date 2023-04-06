using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PublishBrandCache : BaseCache, IPublishBrandCache
    {
        #region Private Variable
        private readonly IPublishBrandService _service;
        #endregion

        #region Constructor
        public PublishBrandCache(IPublishBrandService publishBrandService)
        {
            _service = publishBrandService;
        }
        #endregion

        #region Public Methods

        //Get a list of brands.
        public virtual string GetPublishBrandList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get brand list
                BrandListModel list = _service.GetPublishBrandList(Expands, Filters, Sorts, Page);
                if (list?.Brands?.Count > 0)
                {
                    //Create response.
                    BrandListResponse response = new BrandListResponse { Brands = list.Brands };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get brand using brandId.
        public virtual string GetPublishBrand(int brandId, int localeId, int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                BrandModel brandModel = _service.GetPublishBrand(brandId, localeId, portalId);
                if (IsNotNull(brandModel))
                {
                    BrandResponse response = new BrandResponse { Brand = brandModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

    }
}
