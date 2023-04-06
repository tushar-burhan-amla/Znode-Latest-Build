using Znode.Engine.Api.Models.V2;

namespace Znode.Engine.Api.Models.Responses.V2
{
    public class ShoppingCartResponseV2 : BaseResponse
    {
        public ShoppingCartModelV2 ShoppingCart { get; set; }
    }
}