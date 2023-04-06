using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ERPConfiguratorListResponse : BaseListResponse
    {
        public List<ERPConfiguratorModel> ERPConfigurator { get; set; }
        public List<ERPConfiguratorModel> ERPConfiguratorClasses { get; set; }
    }
}
