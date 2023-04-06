using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class AnalyticsClient : BaseClient, IAnalyticsClient
    {
        //Get analytics dashboard details
        public virtual AnalyticsModel GetAnalyticsDashboardData()
        {
            //Get Endpoint.
            string endpoint = AnalyticsEndpoint.GetAnalyticsDashboardData();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            AnalyticsResponse response = GetResourceFromEndpoint<AnalyticsResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.AnalyticsDetails;
        }

        //Get the Analytics JSON key
        public virtual string GetAnalyticsJSONKey()
        {
            //Get Endpoint
            string endpoint = AnalyticsEndpoint.GetAnalyticsJSONKey();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            AnalyticsResponse response = GetResourceFromEndpoint<AnalyticsResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.AnalyticsJSONKey;
        }

        //Update the analytics details
        public virtual bool UpdateAnalyticsDetails(AnalyticsModel analyticsDetailsModel)
        {
            //Get Endpoint
            string endpoint = AnalyticsEndpoint.UpdateAnalyticsDetails();

            //Get serialized object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(analyticsDetailsModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.IsSuccess;
        }
    }
}
