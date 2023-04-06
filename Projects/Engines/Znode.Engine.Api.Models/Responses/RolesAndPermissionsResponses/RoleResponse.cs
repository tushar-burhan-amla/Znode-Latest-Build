namespace Znode.Engine.Api.Models.Responses
{
    //Role Response
    public class RoleResponse : BaseResponse
    {
        public RoleModel Role { get; set; }
        public RoleMenuModel RoleMenu { get; set; }
    }
}
