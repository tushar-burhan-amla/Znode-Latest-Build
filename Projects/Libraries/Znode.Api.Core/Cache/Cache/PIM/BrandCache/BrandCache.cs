using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class BrandCache : BaseCache, IBrandCache
    {
        #region Private Variables
        private readonly IBrandService _service;
        #endregion

        #region Constructor
        public BrandCache(IBrandService brandService)
        {
            _service = brandService;
        }
        #endregion

        #region Public Methods

        //Get a list of brands.
        public virtual string GetBrands(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get brand list
                BrandListModel list = _service.GetBrandList(Expands, Filters, Sorts, Page);
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
        public virtual string GetBrand(int brandId, int localeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                BrandModel brandModel = _service.GetBrand(brandId, localeId);
                if (IsNotNull(brandModel))
                {
                    BrandResponse response = new BrandResponse { Brand = brandModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get brand code list.
        public virtual string GetBrandCodeList(string attributeCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //brand code list
                BrandListModel list = _service.GetAvailableBrandCodes(attributeCode);
                if (list?.BrandCodes?.Count > 0)
                {
                    //Get response and insert it into cache.
                    BrandListResponse response = new BrandListResponse { BrandCodes = list.BrandCodes };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetBrandPortalList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                PortalBrandListModel portalList = _service.GetBrandPortalList(Expands, Filters, Sorts, Page);
                if (portalList?.PortalBrandModel?.Count > 0)
                {
                    //Create Response and insert in to cache
                    PortalBrandListResponse response = new PortalBrandListResponse { PortalBrand = portalList.PortalBrandModel };

                    response.MapPagingDataFromModel(portalList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Generate response
        public ProductListResponse GetProductListResponseFromDetailListModel(ProductDetailsListModel productList)
        {
            ProductListResponse response = new ProductListResponse
            {
                ProductDetails = productList.ProductDetailList,
                Locale = productList.Locale,
                AttrubuteColumnName = productList.AttributeColumnName,
                XmlDataList = productList.XmlDataList,
                ProductDetailsDynamic = productList?.ProductDetailListDynamic,
                NewAttributeList = productList?.NewAttributeList
            };
            response.MapPagingDataFromModel(productList);
            return response;
        }

        // Get the list of all Brands associated /UnAssociated with store classes.
        public virtual string GetPortalBrandList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                BrandListModel list = _service.GetPortalBrandList(Expands, Filters, Sorts, Page);
                if (list?.Brands?.Count > 0)
                {
                    BrandListResponse response = new BrandListResponse { Brands = list.Brands };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        #endregion
    }
}