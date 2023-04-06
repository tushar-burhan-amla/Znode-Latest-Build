using Znode.Engine.Api.Models.V2;

namespace Znode.Engine.Api.Cache
{
    public interface IShoppingCartCacheV2 : IShoppingCartCache
    {
        /// <summary>
        /// Calculate shopping cart
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ShoppingCartModelV2 CalculateV2(ShoppingCartCalculateRequestModelV2 model);
    }
}
