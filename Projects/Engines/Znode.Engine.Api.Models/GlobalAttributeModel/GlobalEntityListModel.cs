using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public  class GlobalEntityListModel : BaseListModel
    {
        public List<GlobalEntityModel> GlobalEntityList { get; set; }

        public GlobalEntityListModel()
        {
            GlobalEntityList = new List<GlobalEntityModel>();
        }
    }
}
