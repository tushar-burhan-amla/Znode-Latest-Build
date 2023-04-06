using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RMAReturnListModel : BaseListModel
    {
        public List<RMAReturnModel> Returns { get; set; }
        public RMAReturnListModel()
        {
            Returns = new List<RMAReturnModel>();
        }
    }
}