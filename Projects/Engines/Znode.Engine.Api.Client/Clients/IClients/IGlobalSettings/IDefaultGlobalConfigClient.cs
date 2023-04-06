using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IDefaultGlobalConfigClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of DefaultGlobalConfig.
        /// </summary>
        /// <returns>DefaultGlobalConfig list model.</returns>
        DefaultGlobalConfigListModel GetDefaultGlobalConfigList();

        /// <summary>
        /// Gets the Dictionary of Default Logging Settings
        /// </summary>
        /// <returns>Logging Configuration settings Dictionary.</returns>
        Dictionary<string, string> GetLoggingGlobalConfigList();

    }
}
