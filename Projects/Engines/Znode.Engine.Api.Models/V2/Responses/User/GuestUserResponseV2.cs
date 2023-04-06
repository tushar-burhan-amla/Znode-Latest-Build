using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Models.V2.Responses
{
    public class GuestUserResponseV2 : BaseResponse
    {
        public GuestUserModelV2 GuestUser { get; set; }
    }
}
