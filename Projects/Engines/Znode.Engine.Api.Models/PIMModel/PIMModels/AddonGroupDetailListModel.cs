using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AddonGroupDetailListModel:BaseListModel
    {
        public List<AddonGroupDetailModel> AddonGroupDetails { get; set; }
    }
}
