using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class CatalogCache : BaseCache, ICatalogCache
    {
        #region Private Variables

        private readonly ICatalogService _service;

        #endregion Private Variables

        #region Constructor

        public CatalogCache(ICatalogService catalogService)
        {
            _service = catalogService;
        }

        #endregion Constructor

        #region Public Methods

        //Get a list of categories.
        public virtual string GetCatalogs(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list
                CatalogListModel list = _service.GetCatalogs(Expands, Filters, Sorts, Page);
                if (list?.Catalogs?.Count > 0)
                {
                    //Create response.
                    CatalogListResponse response = new CatalogListResponse { Catalogs = list.Catalogs };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get catalog using catalogId.
        public virtual string GetCatalog(int pimCatalogId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CatalogModel catalog = _service.GetCatalog(pimCatalogId);
                if (IsNotNull(catalog))
                {
                    CatalogResponse response = new CatalogResponse { Catalog = catalog };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get categories which are associated to catalog
        public virtual string GetAssociatedCategories(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Catalog Associate Category list
                CatalogAssociateCategoryListModel catalogAssociated = _service.GetAssociatedCategories(Filters, Sorts, Page);
                if (IsNotNull(catalogAssociated?.XmlDataList))
                {
                    //Create response.
                    CatalogListResponse response = new CatalogListResponse
                    {
                        AssociateCategories = catalogAssociated?.catalogAssociatedCategoryList,
                        XmlDataList = catalogAssociated.XmlDataList,
                        AttributeColumnName = catalogAssociated.AttributeColumnName
                    };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(catalogAssociated);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetCategoryAssociatedProducts(CatalogAssociationModel catalogAssociationModel, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list
                ProductDetailsListModel productList = _service.GetCategoryAssociatedProducts(catalogAssociationModel, Filters, Sorts, Page);
                if (!Equals(productList?.ProductDetailList, null))
                {
                    //Create response.
                    ProductListResponse response = new ProductListResponse { ProductDetails = productList.ProductDetailList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(productList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get details(Display order, active status, etc.)of category associated to catalog.
        public virtual string GetAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CatalogAssociateCategoryModel catalogAssociated = _service.GetAssociateCategoryDetails(catalogAssociateCategoryModel);
                if (IsNotNull(catalogAssociated))
                {
                    //Create response.
                    CatalogResponse response = new CatalogResponse { AssociateCategory = catalogAssociated };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Catalog Publish Status.
        public virtual PublishCatalogLogListModel GetCatalogPublishStatus(string routeUri, string routeTemplate)
        => _service.GetCatalogPublishStatus(Filters, Sorts, Page);

        //Get Catalog details by catalog code.
        public virtual string GetCatalogByCatalogCode(string catalogCode, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                CatalogModel catalog = _service.GetCatalogByCatalogCode(catalogCode);
                if (IsNotNull(catalog))
                {
                    CatalogResponse response = new CatalogResponse { Catalog = catalog };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion Public Methods
    }
}