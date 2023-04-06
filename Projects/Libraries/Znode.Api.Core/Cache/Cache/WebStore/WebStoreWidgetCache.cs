using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Cache
{
    public class WebStoreWidgetCache : BaseCache, IWebStoreWidgetCache
    {
        #region Private Variable
        private readonly IWebStoreWidgetService _service;
        #endregion

        #region Constructor
        public WebStoreWidgetCache(IWebStoreWidgetService service)
        {
            _service = service;
        }
        #endregion

        //Get slider data.
        public virtual string GetSlider(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSWidgetConfigurationModel slider = _service.GetSlider(parameter);
                if (HelperUtility.IsNotNull(slider))
                {
                    WebStoreWidgetResponse response = new WebStoreWidgetResponse { Slider = slider };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get product list widget data.
        public virtual string GetProducts(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreWidgetProductListModel list = _service.GetProducts(parameter, Expands);
                if (HelperUtility.IsNotNull(list))
                {
                    WebStoreWidgetProductListResponse response = new WebStoreWidgetProductListResponse { Products = list.Products, DisplayName = list.DisplayName };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get link widget data.
        public virtual string GetLinkWidget(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            ZnodeLogging.LogMessage("Cache.GetLinkWidget -> Route Uri: " + routeUri, "Cache Issue", System.Diagnostics.TraceLevel.Info);

            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                LinkWidgetConfigurationListModel linkData = _service.GetLinkWidget(parameter);
                if (HelperUtility.IsNotNull(linkData))
                {
                    WebStoreLinkWidgetResponse response = new WebStoreLinkWidgetResponse { LinkDataList = linkData };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get category list widget data.
        public virtual string GetCategories(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreWidgetCategoryListModel list = _service.GetCategories(parameter);
                if (HelperUtility.IsNotNull(list))
                {
                    WebStoreWidgetCategoryListResponse response = new WebStoreWidgetCategoryListResponse { Categories = list.Categories };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get product list widget data.
        public virtual string GetLinkProducts(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreLinkProductListModel list = _service.GetLinkProductList(parameter, Expands);
                if (HelperUtility.IsNotNull(list))
                {
                    WebStoreLinkProductListResponse response = new WebStoreLinkProductListResponse { LinkProductsList = list.LinkProductList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get slider data.
        public virtual string GetTagManager(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSTextWidgetConfigurationModel textWidget = _service.GetTagManager(parameter);
                if (HelperUtility.IsNotNull(textWidget))
                {
                    WebStoreWidgetResponse response = new WebStoreWidgetResponse { CMSTextWidget = textWidget };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Media Widget Details
        public virtual string GetMediaWidgetDetails(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSMediaWidgetConfigurationModel textWidget = _service.GetMediaWidgetDetails(parameter);
                if (HelperUtility.IsNotNull(textWidget))
                {
                    WebStoreWidgetResponse response = new WebStoreWidgetResponse { MediaWidget = textWidget };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get brand list widget data.
        public virtual string GetBrands(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreWidgetBrandListModel list = _service.GetBrands(parameter);
                if (HelperUtility.IsNotNull(list))
                {
                    WebStoreWidgetBrandListResponse response = new WebStoreWidgetBrandListResponse { Brands = list.Brands };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get Form Configuration widget data.
        public virtual string GetFormConfigurationByCMSMappingId(WebStoreWidgetFormParameters parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache($"{routeUri}{parameter.CMSMappingId}");
            if (string.IsNullOrEmpty(data))
            {
                WebStoreWidgetFormParameters model = _service.GetFormConfigurationByCMSMappingId(parameter.CMSMappingId, parameter.LocaleId);
                if (HelperUtility.IsNotNull(model))
                {
                    WebStoreWidgetFormResponse response = new WebStoreWidgetFormResponse() { webStoreWidgetFormParameters = model };
                    data = InsertIntoCache($"{routeUri}{parameter.CMSMappingId}", routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get search widget details.
        public virtual string GetSearchWidgetData(WebStoreSearchWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreWidgetSearchModel model = _service.GetSearchWidgetData(parameter,Expands,Filters,Sorts,Page);
                if (HelperUtility.IsNotNull(model))
                {
                    WebStoreWidgetSearchResponse response = new WebStoreWidgetSearchResponse() { SearchWidgetData = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get container data.
        public virtual string GetContainer(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                string ContainerKey = _service.GetContainer(parameter);
                WebStoreWidgetResponse response = new WebStoreWidgetResponse { ContainerKey = ContainerKey };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }
    }
}