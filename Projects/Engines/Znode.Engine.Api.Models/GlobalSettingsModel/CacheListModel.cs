using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CacheListModel : BaseListModel
    {
        public List<CacheModel> CacheData { get; set; }
        public CacheListModel()
        {
            CacheData = new List<CacheModel>();
        }
    }
}
