using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class DefaultGlobalConfigClient : BaseClient, IDefaultGlobalConfigClient
    {
        // Gets the list of DefaultGlobalConfig.
        public virtual DefaultGlobalConfigListModel GetDefaultGlobalConfigList()
        {
            string endpoint = DefaultGlobalConfigEndpoint.GetDefaultConfigList();

            ApiStatus status = new ApiStatus();
            DefaultGlobalConfigListResponse response = GetResourceFromEndpoint<DefaultGlobalConfigListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DefaultGlobalConfigListModel list = new DefaultGlobalConfigListModel { DefaultGlobalConfigs = response?.DefaultGlobalConfigs };

            return list;
        }
        //Gets the Dictionary of Default Logging Settings
        public virtual Dictionary<string, string> GetLoggingGlobalConfigList()
        {
            string endpoint = DefaultGlobalConfigEndpoint.GetLoggingConfigList();

            ApiStatus status = new ApiStatus();
            DefaultGlobalConfigListResponse response = GetResourceFromEndpoint<DefaultGlobalConfigListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            Dictionary<string, string> loggingConfigurationSettings = response?.LoggingConfigurationSettings;

            return loggingConfigurationSettings;
        }
    }
}
