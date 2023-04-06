using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class WebStoreWidgetClient : BaseClient, IWebStoreWidgetClient
    {
        //Async call to get slider details.
        public virtual async Task<CMSWidgetConfigurationModel> GetSliderAsync(WebStoreWidgetParameterModel parameter)
        {
            string endpoint = WebStoreWidgetEndpoint.GetSlider(string.Empty);
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetResponse response = await PutResourceToEndpointAsync<WebStoreWidgetResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Slider;
        }

        //Get slider widget details.
        public virtual CMSWidgetConfigurationModel GetSlider(WebStoreWidgetParameterModel parameter, string key)
        {
            string endpoint = WebStoreWidgetEndpoint.GetSlider(key);
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetResponse response = PutResourceToEndpoint<WebStoreWidgetResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Slider;
        }

        //Get product list widget details.
        public virtual WebStoreWidgetProductListModel GetProducts(WebStoreWidgetParameterModel parameter, string key, ExpandCollection expands)
        {
            string endpoint = WebStoreWidgetEndpoint.GetProducts(key);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetProductListResponse response = PutResourceToEndpoint<WebStoreWidgetProductListResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            WebStoreWidgetProductListModel list = new WebStoreWidgetProductListModel { Products = response?.Products, DisplayName = response?.DisplayName };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        
        //Get link widget data.
        public virtual LinkWidgetConfigurationListModel GetLinkWidget(WebStoreWidgetParameterModel parameter, string key)
        {
            string endpoint = WebStoreWidgetEndpoint.GetLinkWidget(key);
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreLinkWidgetResponse response = PutResourceToEndpoint<WebStoreLinkWidgetResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.LinkDataList;
        }

        //Get category list widget details.
        public virtual WebStoreWidgetCategoryListModel GetCategories(WebStoreWidgetParameterModel parameter, string key)
        {
            string endpoint = WebStoreWidgetEndpoint.GetCategories(key);
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetCategoryListResponse response = PutResourceToEndpoint<WebStoreWidgetCategoryListResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            WebStoreWidgetCategoryListModel list = new WebStoreWidgetCategoryListModel { Categories = response?.Categories };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get link product list widget details.
        public virtual WebStoreLinkProductListModel GetLinkProductList(WebStoreWidgetParameterModel parameter, string key, ExpandCollection expands)
        {
            string endpoint = WebStoreWidgetEndpoint.GetLinkProductList(key);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);
            ApiStatus status = new ApiStatus();
            WebStoreLinkProductListResponse response = PutResourceToEndpoint<WebStoreLinkProductListResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            WebStoreLinkProductListModel list = new WebStoreLinkProductListModel { LinkProductList = response?.LinkProductsList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get tag manager data
        public virtual CMSTextWidgetConfigurationModel GetTagManager(WebStoreWidgetParameterModel parameter, string key)
        {
            string endpoint = WebStoreWidgetEndpoint.GetTagManager(key);
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetResponse response = PutResourceToEndpoint<WebStoreWidgetResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.CMSTextWidget;
        }

        //Get details of Media Widget
        public virtual CMSMediaWidgetConfigurationModel GetMediaWidgetDetails(WebStoreWidgetParameterModel parameter, string key, ExpandCollection expands)
        {
            string endpoint = WebStoreWidgetEndpoint.GetMediaWidgetDetails(key);
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetResponse response = PutResourceToEndpoint<WebStoreWidgetResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.MediaWidget;

        }

        //Get brand list widget details.
        public virtual WebStoreWidgetBrandListModel GetBrands(WebStoreWidgetParameterModel parameter, string key)
        {
            string endpoint = WebStoreWidgetEndpoint.GetBrands(key);
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetBrandListResponse response = PutResourceToEndpoint<WebStoreWidgetBrandListResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            WebStoreWidgetBrandListModel list = new WebStoreWidgetBrandListModel { Brands = response?.Brands };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Form attribute group widget details.
        public virtual WebStoreWidgetFormParameters GetFormConfiguration(WebStoreWidgetParameterModel parameter)
        {
            string endpoint = WebStoreWidgetEndpoint.GetFormConfiguration();
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetFormResponse response = PutResourceToEndpoint<WebStoreWidgetFormResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.webStoreWidgetFormParameters;
        }

        //Get search widget products ad facets.
        public WebStoreWidgetSearchModel GetSearchWidgetData(WebStoreSearchWidgetParameterModel webStoreWidgetParameterModel, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection)
        {
            string endpoint = WebStoreWidgetEndpoint.GetSearchWidgetData();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, null, null);

            ApiStatus status = new ApiStatus();
            WebStoreWidgetSearchResponse response = PutResourceToEndpoint<WebStoreWidgetSearchResponse>(endpoint, JsonConvert.SerializeObject(webStoreWidgetParameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.SearchWidgetData;
        }
        
        //Get available ecert total.
        public virtual ECertTotalBalanceModel GetECertTotalBalance(WebStoreWidgetParameterModel parameter, string userName, string email)
        {
            string endpoint = ECertEndpoint.GetECertTotalBalance();

            ApiStatus status = new ApiStatus();
            ECertTotalModel eCertTotalModel = new ECertTotalModel()
            {
                Email = email,
                UserName = userName,
                WidgetParameter = parameter
            };
            ECertTotalBalanceResponse response = PutResourceToEndpoint<ECertTotalBalanceResponse>(endpoint, JsonConvert.SerializeObject(eCertTotalModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return (response?.HasError ?? false) ? new ECertTotalBalanceModel() : response?.ECertTotalBalance;
        }

        //Get container widget details.
        public virtual string GetContainer(WebStoreWidgetParameterModel parameter)
        {
            string endpoint = WebStoreWidgetEndpoint.GetContainer();
            endpoint = BuildPublishStateQueryString(endpoint);
            endpoint = BuildLocaleQueryString(endpoint);
            ApiStatus status = new ApiStatus();
            WebStoreWidgetResponse response = PutResourceToEndpoint<WebStoreWidgetResponse>(endpoint, JsonConvert.SerializeObject(parameter), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.ContainerKey;
        }
    }
}
