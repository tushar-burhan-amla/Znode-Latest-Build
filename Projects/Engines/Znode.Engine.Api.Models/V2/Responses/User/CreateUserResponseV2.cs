using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Models.V2.Responses
{
    public class CreateUserResponseV2 : BaseResponse
    {
        public CreateUserModelV2 User { get; set; }
    }
}
