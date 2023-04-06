using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class RMARequestListModel : BaseListModel
    {
        public List<RMARequestModel> RMARequests { get; set; }

        public RMARequestListModel()
        {
            RMARequests = new List<RMARequestModel>();
        }
    }
}
