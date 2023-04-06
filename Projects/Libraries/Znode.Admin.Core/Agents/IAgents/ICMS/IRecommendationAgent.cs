using Znode.Engine.Admin.ViewModels;
namespace Znode.Engine.Admin.Agents
{
    public interface IRecommendationAgent
    {
        /// <summary>
        /// To get the product recommendation settings.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>RecommendationSettingViewModel</returns>
        RecommendationSettingViewModel GetRecommendationSetting(int portalId, string touchPointName);

        /// <summary>
        /// To save the product recommendation settings.
        /// </summary>
        /// <param name="recommendationSettingModel"></param>
        /// <returns>RecommendationSettingViewModel</returns>
        RecommendationSettingViewModel SaveRecommendationSetting(RecommendationSettingViewModel recommendationSettingViewModel);

        /// <summary>
        /// To generate the recommendation engine data.
        /// </summary>
        /// <param name="recommendationDataGenerateViewModel">Context to generate recommendation engine data.</param>
        /// <returns>RecommendationGeneratedDataViewModel</returns>
        RecommendationGeneratedDataViewModel GenerateRecommendationData(RecommendationDataGenerateViewModel recommendationDataGenerateViewModel);
    }
}
