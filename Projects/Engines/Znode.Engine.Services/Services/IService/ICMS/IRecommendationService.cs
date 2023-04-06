using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IRecommendationService
    {
        /// <summary>
        /// To get the recommendation service for the portalId.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>RecommendationSettingModel</returns>
        RecommendationSettingModel GetRecommendationSetting(int portalId, string touchPointName);

        /// <summary>
        /// To save the recommendation settings for the portal.
        /// </summary>
        /// <param name="recommendationSettingModel">To hold recommendation settings to be saved.</param>
        /// <returns>RecommendationSettingModel</returns>
        RecommendationSettingModel SaveRecommendationSetting(RecommendationSettingModel recommendationSettingModel);

        /// <summary>
        /// To get the recommendations
        /// </summary>
        /// <param name="recommendationRequestModel"></param>
        /// <returns></returns>
        RecommendationModel GetRecommendation(RecommendationRequestModel recommendationRequestModel);

        /// <summary>
        /// To create a recommendation engine's internal model.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="isBuildPartial">To specify build model partially or fully.</param>
        /// <returns>True, model generated successfully</returns>
        RecommendationGeneratedDataModel GenerateRecommendationData(int? portalId, bool isBuildPartial);
    }
}
