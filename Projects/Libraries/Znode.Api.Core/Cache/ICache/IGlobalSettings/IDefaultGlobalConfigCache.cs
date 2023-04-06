using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Cache
{
    public interface IDefaultGlobalConfigCache
    {
        #region Public method

        /// <summary>
        /// Get List of DefaultGlobalConfig
        /// </summary>
        /// <returns>DefaultGlobalConfig List Response</returns>
        DefaultGlobalConfigListResponse GetDefaultGlobalConfigList();

        /// <summary>
        /// Get Default Logging Setting
        /// </summary>
        /// <returns>DefaultGlobalConfig List Response</returns>
        DefaultGlobalConfigListResponse GetDefaultLoggingConfigSettings();

        #endregion
    }
}