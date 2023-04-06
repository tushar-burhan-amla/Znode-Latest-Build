using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IRecommendationClient : IBaseClient
    {
        /// <summary>
        /// To get the product recommendation settings.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>RecommendationSettingModel</returns>
        RecommendationSettingModel GetRecommendationSetting(int portalId, string touchPointName);

        /// <summary>
        /// To save the product recommendation settings.
        /// </summary>
        /// <param name="recommendationSettingModel"></param>
        /// <returns>RecommendationSettingModel</returns>
        RecommendationSettingModel SaveRecommendationSetting(RecommendationSettingModel recommendationSettingModel);

        /// <summary>
        /// Get products that are recommended based on the provided recommendation context.
        /// </summary>
        /// <param name="recommendationRequestModel">Recommendation context.</param>
        /// <returns>Recommended products.</returns>
        RecommendationModel GetRecommendation(RecommendationRequestModel recommendationRequestModel);

        /// <summary>
        /// To generate the recommendation engine data.
        /// </summary>
        /// <param name="recommendationDataGenerateModel">Context for generating recommendations data.</param>
        /// <returns>Data generation status.</returns>
        RecommendationGeneratedDataModel GenerateRecommendationData(RecommendationDataGenerateModel recommendationDataGenerateModel);
    }
}
