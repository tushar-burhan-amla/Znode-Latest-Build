using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DefaultGlobalConfigListModel : BaseListModel
    {
        public List<DefaultGlobalConfigModel> DefaultGlobalConfigs { get; set; }

        public DefaultGlobalConfigListModel()
        {
            DefaultGlobalConfigs = new List<DefaultGlobalConfigModel>();
        }
    }
}
