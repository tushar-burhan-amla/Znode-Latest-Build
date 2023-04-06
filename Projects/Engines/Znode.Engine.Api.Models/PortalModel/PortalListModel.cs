using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalListModel : BaseListModel
    {
        public PortalListModel()
        {
            PortalList = new List<PortalModel>();
        }
        public List<PortalModel> PortalList { get; set; }
    }
}
