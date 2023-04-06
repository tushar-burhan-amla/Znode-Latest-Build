using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IDefaultGlobalConfigService
    {
        /// <summary>
        /// Gets a list of DefaultGlobalConfig.
        /// </summary>
        /// <returns>Returns DefaultGlobalConfig list model.</returns>
        DefaultGlobalConfigListModel GetDefaultGlobalConfigList();

        /// <summary>
        /// Get Dictionary of default logging settings
        /// </summary>
        /// <returns>Return Logging Configuration Settings Dictionary</returns>
        Dictionary<string, string> GetLoggingGlobalConfigList();

    }
}
