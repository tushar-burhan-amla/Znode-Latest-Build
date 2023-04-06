using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DefaultGlobalConfigListResponse : BaseListResponse
    {
        public List<DefaultGlobalConfigModel> DefaultGlobalConfigs { get; set; }
        public Dictionary<string, string> LoggingConfigurationSettings { get; set; }
    }
}
