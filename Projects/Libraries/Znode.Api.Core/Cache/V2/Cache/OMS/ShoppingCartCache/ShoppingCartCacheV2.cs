using Znode.Engine.Api.Models.V2;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class ShoppingCartCacheV2 : ShoppingCartCache, IShoppingCartCacheV2
    {
        #region Private Variables
        private readonly IShoppingCartServiceV2 _shoppingCartService;
        #endregion

        #region Constructor
        public ShoppingCartCacheV2(IShoppingCartServiceV2 shoppingCartService) : base(shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        #endregion

        #region Public Methods        
        public virtual ShoppingCartModelV2 CalculateV2(ShoppingCartCalculateRequestModelV2 model)
        {
            UpdateCacheForProfile();
            return _shoppingCartService.CalculateV2(model);
        }
        #endregion
    }
}