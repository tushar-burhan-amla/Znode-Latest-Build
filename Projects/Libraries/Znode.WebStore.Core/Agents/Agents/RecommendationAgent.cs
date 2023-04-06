using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.ViewModels;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public class RecommendationAgent : BaseAgent, IRecommendationAgent
    {
        #region Private Variables
        private readonly IRecommendationClient _recommendationsClient;
        private const int productId = 0;
        private readonly IWebstoreHelper _webstoreHelper;
        #endregion

        #region Public Constructor
        public RecommendationAgent(IRecommendationClient recommendationsClient)
        {
            _recommendationsClient = GetClient<IRecommendationClient>(recommendationsClient);
            _webstoreHelper = GetService<IWebstoreHelper>();
        }
        #endregion

        #region Public Method
        //Get List Of Recommended Products
        public virtual List<ProductViewModel> GetRecommendedProducts(string widgetCode, string productSku)
        => CheckRecommendationSetting(widgetCode, productSku);

        //Differentiate call to GetRecommendations method on the basis of widget code.
        protected virtual List<ProductViewModel> CheckRecommendationSetting(string widgetCode, string productSku)
        {
            RecommendationRequestModel recommendationContext = new RecommendationRequestModel();            
            recommendationContext.WidgetCode = widgetCode;
            switch (widgetCode)
            {
                case ZnodeConstant.HomeRecommendations:
                    SetRecentlyViewedProductSkus(recommendationContext);
                    return GetRecommendations(recommendationContext, PortalAgent.CurrentPortal.RecommendationSetting?.IsHomeRecommendation);
                case ZnodeConstant.PDPRecommendations:
                    return GetPDPRecommendations(recommendationContext, productSku);
                case ZnodeConstant.CartRecommendations:
                    SetCartProductSkus(recommendationContext);
                    return GetRecommendations(recommendationContext, PortalAgent.CurrentPortal.RecommendationSetting?.IsCartRecommendation);
                default:
                    throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.ErrorInvalidRecommendationWidgetCode);
            }
        }

        protected virtual List<ProductViewModel> GetPDPRecommendations(RecommendationRequestModel recommendationContext, string productSku)
        {
            if (!string.IsNullOrEmpty(productSku))
            {
                SetPDPProductSku(recommendationContext, productSku);
                recommendationContext.IsShowRecommendations = true;
                return GetRecommendations(recommendationContext, PortalAgent.CurrentPortal.RecommendationSetting?.IsPDPRecommendation);
            }
            else
                return new List<ProductViewModel>();
        }

        //To set the recently viewed products list.
        protected virtual void SetRecentlyViewedProductSkus(RecommendationRequestModel recommendationContext)
        {
            List<RecentViewModel> productList = GetService<IProductAgent>().GetRecentProductList(productId);
            recommendationContext.RecentlyViewedProductSkus = productList?.Select(x => x.SKU)?.ToList();
            recommendationContext.IsShowRecommendations = recommendationContext.RecentlyViewedProductSkus?.Count > 0;
        }

        //To set the cart product SKUs in the RecommendationRequestModel.
        protected virtual void SetCartProductSkus(RecommendationRequestModel recommendationContext)
        {
            ShoppingCartModel shoppingCartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            recommendationContext.ProductSkusInCart = shoppingCartModel?.ShoppingCartItems?.Select(item => item.SKU.ToString())?.ToList();
            recommendationContext.IsShowRecommendations = recommendationContext.ProductSkusInCart?.Count > 0;
        }

        //To set the SKU of product that currently being viewed on PDP page.
        protected virtual void SetPDPProductSku(RecommendationRequestModel recommendationContext, string productSku)
        => recommendationContext.ProductSkuCurrentlyBeingViewed = productSku;

        //Get the recommended products if value of isRecommendationWidgetEnabled parameter is true else return the empty model so that widget will not be shown.
        protected virtual List<ProductViewModel> GetRecommendations(RecommendationRequestModel recommendationContext, bool? isRecommendationWidgetEnabled)
        {
            RecommendationModel recommendation = new RecommendationModel();

            //Get recommendation from engine if isRecommendationWidgetEnabled is true. 
            if (isRecommendationWidgetEnabled.GetValueOrDefault() && recommendationContext.IsShowRecommendations)
            {
                SetDetailsInRecommendationRequest(recommendationContext);
                recommendation = _recommendationsClient.GetRecommendation(recommendationContext);
            }
            
            List<ProductViewModel> productViewModelList = recommendation.RecommendedProducts?.ToViewModel<ProductViewModel>().ToList();           
            if (productViewModelList?.Count > 0)
            {
                for (int j = 0; j < productViewModelList.Count; j++)
                {
                    if (PortalAgent.CurrentPortal.IsAddToCartOptionForProductSlidersEnabled)
                    {
                        productViewModelList[j] = GetService<IProductAgent>().GetProduct(productViewModelList[j].PublishProductId);
                        productViewModelList[j].IsAddToCartOptionForProductSlidersEnabled = true;
                    }
                    productViewModelList[j].HighlightLists = _webstoreHelper.GetHighlightListFromAttributes(productViewModelList[j].Attributes, productViewModelList[j].SKU, productViewModelList[j].PublishProductId);
                }
            }          
                       
            return productViewModelList;
        }

        //To set the required details in RecommendationRequestModel
        protected virtual void SetDetailsInRecommendationRequest(RecommendationRequestModel recommendationRequestModel)
        {
            //recommendationRequestModel.ProfileId = Helper.GetProfileId();
            recommendationRequestModel.ProfileId = GetProfileId();
            recommendationRequestModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            recommendationRequestModel.CatalogId = GetCatalogId();
            recommendationRequestModel.LocaleId = PortalAgent.LocaleId;
        }
        #endregion
    }
}
