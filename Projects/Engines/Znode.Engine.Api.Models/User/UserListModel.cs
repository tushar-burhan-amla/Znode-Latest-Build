using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class UserListModel : BaseListModel
    {
        public List<UserModel> Users { get; set; }

        public UserListModel()
        {
            Users = new List<UserModel>();
        }
    }
}
