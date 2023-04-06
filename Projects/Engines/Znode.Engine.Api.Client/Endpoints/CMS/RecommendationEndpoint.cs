namespace Znode.Engine.Api.Client.Endpoints
{
    public class RecommendationEndpoint : BaseEndpoint
    {
        //Get product recommendation setting endpoint.
        public static string GetRecommendationSetting(int portalId, string touchPointName) => $"{ApiRoot}/recommendation/getrecommendationsetting/{portalId}/{touchPointName}";

        //Save product recommendation setting endpoint.
        public static string SaveRecommendationSetting() => $"{ApiRoot}/recommendation/saverecommendationsetting";

        //Get Recommended products endpoint.
        public static string GetRecommendation() => $"{ApiRoot}/recommendation/getrecommendation";

        //Save product recommendation setting endpoint.
        public static string GenerateRecommendationData() => $"{ApiRoot}/recommendation/generaterecommendationdata";
    }
}
