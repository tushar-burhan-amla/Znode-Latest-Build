using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ERPConnectorControlListModel : BaseListModel
    {
        public ERPConnectorControlListModel()
        {
            ERPConnectorControlList = new List<ERPConnectorControlModel>();
        }
        public List<ERPConnectorControlModel> ERPConnectorControlList { get; set; }

        public string ERPClassName { get; set; }
    }
}
