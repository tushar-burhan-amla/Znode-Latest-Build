using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    //Role List Response
    public class RoleListResponse : BaseListResponse
    {
        public List<PermissionsModel> Permissions { get; set; }

        public List<RoleModel> Roles { get; set; }
    }
}
