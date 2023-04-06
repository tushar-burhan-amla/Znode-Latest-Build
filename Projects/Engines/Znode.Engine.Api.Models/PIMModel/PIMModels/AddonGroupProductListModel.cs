using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AddonGroupProductListModel : BaseListModel
    {
        public List<AddonGroupProductModel> AddonGroupProducts { get; set; }
    }
}
