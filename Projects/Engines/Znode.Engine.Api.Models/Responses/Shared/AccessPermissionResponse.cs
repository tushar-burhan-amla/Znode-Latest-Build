namespace Znode.Engine.Api.Models.Responses
{
    public class AccessPermissionResponse : BaseListResponse
    {
        public AccessPermissionListModel AccessPermissionList { get; set; }

        public AccessPermissionModel AccessPermission { get; set; }
    }
}
