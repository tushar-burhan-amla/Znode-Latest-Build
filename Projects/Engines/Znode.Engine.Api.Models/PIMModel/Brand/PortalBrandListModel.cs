using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
   public class PortalBrandListModel : BaseListModel
    {
        public PortalBrandListModel()
        {
            PortalBrandModel = new List<PortalBrandModel>();
        }
        public List<PortalBrandModel> PortalBrandModel { get; set; }
    }
}
