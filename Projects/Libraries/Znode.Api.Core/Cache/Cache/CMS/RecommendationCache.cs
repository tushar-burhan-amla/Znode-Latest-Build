using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class RecommendationCache : BaseCache, IRecommendationCache
    {
        #region Private Variable
        private readonly IRecommendationService _recommendationService;
        #endregion

        #region Constructor
        public RecommendationCache(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }
        #endregion

        #region Public Methods
        public string GetRecommendationSetting(int portalId, string touchPointName, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                RecommendationSettingModel recommendationSettingModel = _recommendationService.GetRecommendationSetting(portalId, touchPointName);
                if (IsNotNull(recommendationSettingModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new RecommendationSettingResponse { RecommendationSetting = recommendationSettingModel });
            }
            return data;
        }
        #endregion
    }
}
