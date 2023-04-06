using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SalesRepUserListModel : BaseListModel
    {
        public List<SalesRepUserModel> SalesRepUsers { get; set; }

        public SalesRepUserListModel()
        {
            SalesRepUsers = new List<SalesRepUserModel>();
        }
    }
}
    