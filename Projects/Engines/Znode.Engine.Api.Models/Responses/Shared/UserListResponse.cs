using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class UserListResponse : BaseListResponse
    {
        //List response for user accounts.
        public List<UserModel> Users { get; set; }
    }
}
