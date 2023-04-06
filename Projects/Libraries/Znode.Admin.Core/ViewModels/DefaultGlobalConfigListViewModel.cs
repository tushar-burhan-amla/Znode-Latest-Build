using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class DefaultGlobalConfigListViewModel 
    {
        public List<DefaultGlobalConfigViewModel> DefaultGlobalConfigs { get; set; }

        public DefaultGlobalConfigListViewModel()
        {
            DefaultGlobalConfigs = new List<DefaultGlobalConfigViewModel>();
        }
    }
}
