using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreGroupProductListModel : BaseModel
    {
        public List<WebStoreGroupProductModel> GroupProducts { get; set; }
    }
}
