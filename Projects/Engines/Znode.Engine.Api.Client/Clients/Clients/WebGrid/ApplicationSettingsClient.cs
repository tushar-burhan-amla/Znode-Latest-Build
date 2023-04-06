using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class ApplicationSettingsClient : BaseClient, IApplicationSettingsClient
    {

        public virtual ApplicationSettingDataModel GetFilterConfigurationXML(string itemName, int? userId = null)
        {
            //Get Endpoint
            string endpoint = ApplicationSettingsEndpoint.GetFilterConfigurationXML(itemName, userId);

            //Get response from endpoint
            ApiStatus status = new ApiStatus();
            ApplicationSettingsResponse response = GetResourceFromEndpoint<ApplicationSettingsResponse>(endpoint, status);

            //Check response status as ok or not found
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Returns response
            return response?.XMLSetting ?? null;
        }
        public virtual ViewModel CreateNewView(ViewModel model)
        {
            //Get endpoint
            string endpoint = ApplicationSettingsEndpoint.CreateNewView();

            //Check response status as ok or not found
            ApiStatus status = new ApiStatus();
            ApplicationSettingListResponse response = PostResourceToEndpoint<ApplicationSettingListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            //Check response status as ok or not found
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            //Return response
            return response.View;
        }

        //Delete View
        public virtual bool DeleteView(ParameterModel parameterModel)
        {
            //Get Endpoint.
            string endpoint = ApplicationSettingsEndpoint.DeleteView();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get view by itemViewId
        public virtual ViewModel GetView(int itemViewId)
        {
            //Get Endpoint.
            string endpoint = ApplicationSettingsEndpoint.GetView(itemViewId);

            //Get response from endpoint
            ApiStatus status = new ApiStatus();
            ApplicationSettingListResponse response = GetResourceFromEndpoint<ApplicationSettingListResponse>(endpoint, status);

            //Check response status as ok or not found
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Return response
            return response?.View;
        }

        //Remove Selected of all view by applicationSettingId
        public virtual bool UpdateViewSelectedStatus(int applicationSettingId)
        {
            //Get Endpoint.
            string endpoint = ApplicationSettingsEndpoint.UpdateViewSelectedStatus(applicationSettingId);

            //Get response from endpoint
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(applicationSettingId), status);

            //Check response status as ok or not found
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        #region XML Editor
            //Returns filtered and ordered list of application Settings   
        public virtual ApplicationSettingListModel GetApplicationSettings(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get endpoint
            string endpoint = ApplicationSettingsEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response from endpoint
            ApiStatus status = new ApiStatus();
            ApplicationSettingListResponse response = GetResourceFromEndpoint<ApplicationSettingListResponse>(endpoint, status);

            //Check response status as ok or not found
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Return response
            ApplicationSettingListModel list = new ApplicationSettingListModel { ApplicationSettingList = (Equals(response, null)) ? null : response.List };
            list.MapPagingDataFromResponse(response);
            return list;
        }
        //Returns the entity column list by entity type and entityName 
        public virtual List<EntityColumnModel> GetColumnList(string entityType, string entityName)
        {
            //Get endpoint
            string endpoint = ApplicationSettingsEndpoint.ColumnList(entityType, entityName);

            //Get response from endpoint
            ApiStatus status = new ApiStatus();
            ApplicationSettingListResponse response = GetResourceFromEndpoint<ApplicationSettingListResponse>(endpoint, status);

            //Check response status as ok or not found
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Return response
            return (response?.ColumnList.Count > 0) ? response.ColumnList : null;
        }
        //Save XML Configuration  
        public virtual bool SaveXmlConfiguration(ApplicationSettingDataModel model)
        {
            //Get endpoint
            string endpoint = ApplicationSettingsEndpoint.Create();

            //Check response status as ok or not found
            ApiStatus status = new ApiStatus();
            ApplicationSettingListResponse response = PostResourceToEndpoint<ApplicationSettingListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            //Check response status as ok or not found
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            //Return response
            return (response?.ColumnList.Count > 0) ? false : response.CreateStatus;
        }
        #endregion
    }
}
