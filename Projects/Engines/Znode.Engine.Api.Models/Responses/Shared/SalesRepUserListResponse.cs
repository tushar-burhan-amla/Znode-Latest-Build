using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SalesRepUserListResponse : BaseListResponse
    {
        //List response for user accounts.
        public List<SalesRepUserModel> SalesRepUsers { get; set; }
    }
}
