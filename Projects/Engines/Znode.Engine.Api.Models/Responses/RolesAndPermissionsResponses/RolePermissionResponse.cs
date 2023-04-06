using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class RolePermissionResponse: BaseListResponse
    {
        public IEnumerable<RolePermissionModel> RolePermission { get; set; }
    }
}
