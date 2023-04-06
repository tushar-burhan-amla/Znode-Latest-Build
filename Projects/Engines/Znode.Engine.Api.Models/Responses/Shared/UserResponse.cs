namespace Znode.Engine.Api.Models.Responses
{
    public class UserResponse : BaseResponse
    {
        public UserModel User { get; set; }

        public UserPortalModel UserPortal { get; set; }
    }
}
