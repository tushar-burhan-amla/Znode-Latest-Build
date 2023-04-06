using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class CMSWidgetConfigurationCache : BaseCache, ICMSWidgetConfigurationCache
    {
        #region Private Variable
        private readonly ICMSWidgetConfigurationService _service;
        #endregion

        #region Constructor
        public CMSWidgetConfigurationCache(ICMSWidgetConfigurationService cmsWidgetConfigurationService)
        {
            _service = cmsWidgetConfigurationService;
        }
        #endregion

        #region Public Methods
      
        //Get the list of CMS Text Widget Configuration.
        public virtual string GetTextWidgetConfigurationList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSTextWidgetConfigurationListModel list = _service.GetTextWidgetConfigurationList(Expands, Filters, Sorts, Page);
                if (list?.TextWidgetConfigurationList?.Count > 0)
                {
                    CMSTextWidgetConfigurationListResponse response = new CMSTextWidgetConfigurationListResponse { TextWidgetConfigurationList = list.TextWidgetConfigurationList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get Text Widget Configuration by Widget Configuration id.
        public virtual string GetTextWidgetConfiguration(int textWidgetConfigurationId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                CMSTextWidgetConfigurationModel configuration = _service.GetTextWidgetConfiguration(textWidgetConfigurationId);
                if (IsNotNull(configuration))
                {
                    CMSTextWidgetConfigurationResponse response = new CMSTextWidgetConfigurationResponse { CMSTextWidgetConfiguration = configuration };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #region Form Widget Configuration.
        public virtual string GetFormWidgetConfigurationList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSFormWidgetConfigurationListModel list = _service.GetFormWidgetConfigurationList(Expands, Filters, Sorts, Page);
                if (list?.FormWidgetConfigurationList?.Count > 0)
                {
                    CMSFormWidgetConfigurationListResponse response = new CMSFormWidgetConfigurationListResponse { FormWidgetConfigurationList = list.FormWidgetConfigurationList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        
        #endregion

        #region CMSWidgetProduct
        //Get associated product list .
        public virtual string GetAssociatedProductList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSWidgetProductListModel list = _service.GetAssociatedProductList(Expands, Filters, Sorts, Page);
                if (list?.CMSWidgetProductCategories?.Count > 0)
                {
                    CMSWidgetProductListResponse response = new CMSWidgetProductListResponse { CMSWidgetProductCategories = list.CMSWidgetProductCategories };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get unassociated product list .
        public virtual string GetUnAssociatedProductList(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ProductDetailsListModel list = _service.GetUnAssociatedProductList(Expands, Filters, Sorts, Page);
                if (list?.ProductDetailList?.Count > 0)
                {
                    //Create Response
                    ProductDetailsListResponse response = new ProductDetailsListResponse { ProductDetailList = list.ProductDetailList };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region CMS Widget Slider Banner
        //Get the CMS Widget Slider Banner Details.
        public virtual string GetCMSWidgetSliderBanner(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSWidgetConfigurationModel cmsWidgetConfigurationModel = _service.GetCMSWidgetSliderBanner(Filters);
                if (IsNotNull(cmsWidgetConfigurationModel))
                {
                    CMSWidgetConfigurationResponse response = new CMSWidgetConfigurationResponse { CMSWidgetConfiguration = cmsWidgetConfigurationModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Link Widgets Configuration
        //Get link widget configuration list.
        public virtual string GetLinkWidgetConfigurationList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                LinkWidgetConfigurationListModel list = _service.LinkWidgetConfigurationList(Expands, Filters, Sorts, Page);
                if (list?.LinkWidgetConfigurationList?.Count > 0)
                {
                    LinkWidgetConfigurationListResponse response = new LinkWidgetConfigurationListResponse { LinkWidgetConfigurationList = list.LinkWidgetConfigurationList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        #endregion

        #region Category Association
        //Get a list of unassociated categories.
        public virtual string GetUnAssociatedCategories(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get category list
                CategoryListModel list = _service.GetUnAssociatedCategories(Expands, Filters, Sorts, Page);
                if (list?.Categories?.Count > 0)
                {
                    //Create response.
                    CategoryListResponse response = new CategoryListResponse { Categories = list.Categories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of associated categories based on cms widgets.
        public virtual string GetAssociatedCategories(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get category list
                CategoryListModel list = _service.GetAssociatedCategories(Expands, Filters, Sorts, Page);
                if (list?.CMSWidgetProductCategories?.Count > 0)
                {
                    //Create response.
                    CategoryListResponse response = new CategoryListResponse { CMSWidgetProductCategories = list.CMSWidgetProductCategories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Brand Association
        //Get a list of unassociated brands.
        public virtual string GetUnAssociatedBrands(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get brand list.
                BrandListModel list = _service.GetUnAssociatedBrands(Expands, Filters, Sorts, Page);
                if (list?.Brands?.Count > 0)
                {
                    //Create response.
                    BrandListResponse response = new BrandListResponse { Brands = list.Brands };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of associated brands based on cms widgets.
        public virtual string GetAssociatedBrands(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get brand list.
                BrandListModel list = _service.GetAssociatedBrands(Expands, Filters, Sorts, Page);
                if (list?.Brands?.Count > 0)
                {
                    //Create response.
                    BrandListResponse response = new BrandListResponse { Brands = list.Brands };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
        #endregion

        #region Form Widget Email Configuration

        public virtual string GetFormWidgetEmailConfiguration(int cMSContentPagesId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from portals service.
                FormWidgetEmailConfigurationModel formWidgetEmailConfigurationModel = _service.GetFormWidgetEmailConfiguration(cMSContentPagesId, Expands);
                if (!Equals(formWidgetEmailConfigurationModel, null))
                {
                    //Create Response and insert in to cache
                    FormWidgetEmailConfigurationResponse response = new FormWidgetEmailConfigurationResponse { FormWidgetEmailConfiguration = formWidgetEmailConfigurationModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion

        #region Search Widget
        //Get Text Widget Configuration by Widget Configuration id.
        public virtual string GetSearchWidgetConfiguration(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                CMSSearchWidgetConfigurationModel configuration = _service.GetSearchWidgetConfiguration(Filters,Expands);
                if (IsNotNull(configuration))
                {
                    CMSSearchWidgetConfigurationResponse response = new CMSSearchWidgetConfigurationResponse { SearchWidgetConfiguration = configuration };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}
