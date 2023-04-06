using Znode.Engine.Api.Models;

namespace Znode.Engine.Services.Maps
{
    public static class ERPConfiguratorMap
    {
        public static ERPConfiguratorModel AddClassNameToModel(string className)
        {
            ERPConfiguratorModel eRPConfiguratorModel = new ERPConfiguratorModel
            {
                ClassName = className,
            };
            return eRPConfiguratorModel;
        }
    }
}
