using Znode.Engine.Api.Models;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class ShoppingCartCache : BaseCache, IShoppingCartCache
    {
        #region Private Variables
        private readonly IShoppingCartService _shoppingCartService;
        #endregion

        #region Constructor
        public ShoppingCartCache(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        #endregion

        #region Public Methods
        public virtual ShoppingCartModel Calculate(string routeUri, ShoppingCartModel model)
        {
            UpdateCacheForProfile();
            return _shoppingCartService.Calculate(model);
        }

        public virtual ShoppingCartModel CreateCart(string routeUri, ShoppingCartModel model)
        {
            UpdateCacheForProfile();
            return _shoppingCartService.CreateCart(model);
        }

        public virtual AddToCartModel AddToCartProduct(string routeUri, AddToCartModel model)
        {
            UpdateCacheForProfile();
            return _shoppingCartService.AddToCartProduct(model);
        }

        public virtual ShippingListModel GetShippingEstimates(string zipCode, string routeUri, ShoppingCartModel model)
        {
            return _shoppingCartService.GetShippingEstimates(zipCode, model);
        }

        public virtual ShoppingCartModel GetShoppingCart(string routeUri, CartParameterModel cartParameterModel)
        {
            UpdateCacheForProfile();
            return _shoppingCartService.GetShoppingCart(cartParameterModel);
        }

        public virtual string GetCartCount(string routeUri, CartParameterModel cartParameterModel)
        {
            UpdateCacheForProfile();
            return _shoppingCartService.GetCartCount(cartParameterModel);
        }

        public virtual OrderLineItemDataListModel GetOrderLineItemDetails(int omsOrderId)
        {
            UpdateCacheForProfile();
            return _shoppingCartService.GetOrderLineItemDetails(omsOrderId);
        }

        
        public virtual bool MergeGuestUsersCart(string routeUri)
        {
            string data = GetFromCache(routeUri);
            bool status = false;
            if (string.IsNullOrEmpty(data))
            {
                status = _shoppingCartService.MergeGuestUsersCart(Filters);
                InsertIntoCache(routeUri, "", status);
            }
            return status;
        }

        #endregion
    }
}