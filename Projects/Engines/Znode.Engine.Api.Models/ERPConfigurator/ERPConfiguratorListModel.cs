using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ERPConfiguratorListModel : BaseListModel
    {
        public List<ERPConfiguratorModel> ERPConfiguratorList { get; set; }
        public ERPConfiguratorListModel()
        {
            ERPConfiguratorList = new List<ERPConfiguratorModel>();
        }
    }
}
