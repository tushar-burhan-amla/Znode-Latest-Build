namespace Znode.Engine.Api.Models.Responses
{
    //Menu Response
    public class MenuResponse : BaseResponse
    {
        public MenuModel Menu { get; set; }
        public MenuActionsPermissionModel MenuActionPermission { get; set; }
    }
}
