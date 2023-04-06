using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class GeneralSettingClient : BaseClient, IGeneralSettingClient
    {
        //Method to get list of all GeneralSettings.
        public virtual GeneralSettingModel list()
        {
            //Get Endpoint.
            string endpoint = GeneralSettingEndpoint.List();

            //Get response
            ApiStatus status = new ApiStatus();
            GeneralSettingResponse response = GetResourceFromEndpoint<GeneralSettingResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.GeneralSetting;
        }

        //Method to update existing GeneralSettings.
        public virtual bool Update(GeneralSettingModel generalSettingModel)
        {
            //Get Endpoint
            string endpoint = GeneralSettingEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(generalSettingModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        public virtual PublishStateMappingListModel GetPublishStateMappingList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of tax class.
            string endpoint = GeneralSettingEndpoint.PublishStateMappingList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PublishStateMappingListResponse response = GetResourceFromEndpoint<PublishStateMappingListResponse>(endpoint, status);

            //check the status of response of type tax class.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishStateMappingListModel list = new PublishStateMappingListModel { PublishStateMappingList = response?.PublishStateMappings };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Method to enable/disable publish state to application type mapping.
        public virtual bool EnableDisablePublishStateMapping(int publishStateMappingId, bool isEnabled)
        {
            //Get Endpoint.
            string endpoint = GeneralSettingEndpoint.EnableDisablePublishStateMapping(isEnabled);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(publishStateMappingId), status);

            //Check status
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #region Cache Management
        //Method to get Cache Management data
        public virtual CacheListModel GetCacheData()
        {
            //Get Endpoint
            string endpoint = GeneralSettingEndpoint.CacheData();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GeneralSettingResponse response = GetResourceFromEndpoint<GeneralSettingResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.CacheData;
        }

        public virtual bool CreateUpdateCacheData(CacheListModel cacheListModel)
        {
            //Get Endpoint
            string endpoint = GeneralSettingEndpoint.CreateUpdateCache();

            //Get serialized object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cacheListModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response.IsSuccess;
        }

        public virtual CacheModel RefreshCacheData(CacheModel cacheModel)
        {
            //Get Endpoint
            string endpoint = GeneralSettingEndpoint.RefreshCacheData();

            //Get serialized object as a response.
            ApiStatus status = new ApiStatus();
            GeneralSettingResponse response = PostResourceToEndpoint<GeneralSettingResponse>(endpoint, JsonConvert.SerializeObject(cacheModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.Cache;
        }
        #endregion

        // Get global configuration settings for application.
        public virtual ConfigurationSettingModel GetConfigurationSettings()
        {
            string endpoint = GeneralSettingEndpoint.GetConfigurationSettings();

            ApiStatus status = new ApiStatus();
            GeneralSettingResponse response = GetResourceFromEndpoint<GeneralSettingResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.InternalServerError };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.ConfigurationSetting;
        }

        //Update global configuration settings for application.
        public virtual bool UpdateConfigurationSettings(ConfigurationSettingModel configurationSettingModel)
        {
            string endpoint = GeneralSettingEndpoint.UpdateConfigurationSettings();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(configurationSettingModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.InternalServerError };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.IsSuccess;
        }



        #region Power Bi Setting Details
        //Get the Power Bi Details
        public virtual PowerBISettingsModel GetPowerBISettings()
        {
            //Get Endpoint
            string endpoint = GeneralSettingEndpoint.GetPowerBISettings();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GeneralSettingResponse response = GetResourceFromEndpoint<GeneralSettingResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.PowerBISettings;
        }

        //Updated the Power Bi Setting Details
        public virtual bool UpdatePowerBISettings(PowerBISettingsModel powerBiSettingModel)
        {
            //Get Endpoint
            string endpoint = GeneralSettingEndpoint.UpdatePowerBISettings();

            //Get serialized object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(powerBiSettingModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.IsSuccess;
        }

        #endregion

        #region Stock Notice Setting
        // Get inventory subscription notification settings details.
        public virtual StockNoticeSettingsModel GetStockNoticeSettings()
        {
            // Get Endpoint.
            string endpoint = GeneralSettingEndpoint.GetStockNoticeSettings();

            // Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GeneralSettingResponse response = GetResourceFromEndpoint<GeneralSettingResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.StockNoticeSettings;
        }

        // Update inventory subscription notification settings details.
        public virtual bool UpdateStockNoticeSettings(StockNoticeSettingsModel stockNoticeSettingsModel)
        {
            // Get Endpoint.
            string endpoint = GeneralSettingEndpoint.UpdateStockNoticeSettings();

            // Get serialized object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(stockNoticeSettingsModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK};

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.IsSuccess;
        }
        #endregion

    }
}
