using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class RecommendationClient : BaseClient, IRecommendationClient
    {
        //To get the product recommendation settings against the portal Id.
        public virtual RecommendationSettingModel GetRecommendationSetting(int portalId, string touchPointName)
        {
            string endpoint = RecommendationEndpoint.GetRecommendationSetting(portalId, touchPointName);

            ApiStatus status = new ApiStatus();
            RecommendationSettingResponse response = GetResourceFromEndpoint<RecommendationSettingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.RecommendationSetting;
        }

        //To save the product recommendation settings of the portal.
        public virtual RecommendationSettingModel SaveRecommendationSetting(RecommendationSettingModel recommendationSettingModel)
        {
            //Get Endpoint.
            string endpoint = RecommendationEndpoint.SaveRecommendationSetting();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RecommendationSettingResponse response = PutResourceToEndpoint<RecommendationSettingResponse>(endpoint, JsonConvert.SerializeObject(recommendationSettingModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.RecommendationSetting;
        }

        #region Recommendation Engine
        //Get products that are recommended based on the provided recommendation request.
        public virtual RecommendationModel GetRecommendation(RecommendationRequestModel recommendationRequestModel)
        {
            //Get Endpoint.
            string endpoint = RecommendationEndpoint.GetRecommendation();

            ApiStatus status = new ApiStatus();
            RecommendationResponse response = PostResourceToEndpoint<RecommendationResponse>(endpoint, JsonConvert.SerializeObject(recommendationRequestModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.Recommendation;
        }

        //To generate the recommendation engine data.
        public virtual RecommendationGeneratedDataModel GenerateRecommendationData(RecommendationDataGenerateModel recommendationDataGenerateModel)
        {
            //Get Endpoint.
            string endpoint = RecommendationEndpoint.GenerateRecommendationData();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RecommendationDataGenerationResponse response = PostResourceToEndpoint<RecommendationDataGenerationResponse>(endpoint, JsonConvert.SerializeObject(recommendationDataGenerateModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.recommendationGeneratedData;
        }
        #endregion
    }
}
