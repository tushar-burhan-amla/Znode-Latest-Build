using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AddonProductListModel:BaseListModel
    {
        public List<AddOnProductModel> AddonProducts { get; set; }
    }
}
