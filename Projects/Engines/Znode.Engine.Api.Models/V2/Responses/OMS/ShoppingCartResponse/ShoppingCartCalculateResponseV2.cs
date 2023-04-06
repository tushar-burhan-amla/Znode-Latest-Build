using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Models.V2
{
    public class ShoppingCartCalculateResponseV2: BaseResponse
    {
        public ShoppingCartModelV2 ShoppingCart { get; set; }
    }
}
