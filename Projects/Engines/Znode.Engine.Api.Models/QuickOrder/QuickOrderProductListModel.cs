using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class QuickOrderProductListModel : BaseListModel
    {
        public List<QuickOrderProductModel> Products { get; set; }
    }
}
