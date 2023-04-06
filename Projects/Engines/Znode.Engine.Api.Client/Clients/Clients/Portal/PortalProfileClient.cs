using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class PortalProfileClient : BaseClient, IPortalProfileClient
    {
        public virtual PortalProfileListModel GetPortalProfiles(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PortalProfileEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PortalProfileListResponse response = GetResourceFromEndpoint<PortalProfileListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalProfileListModel list = new PortalProfileListModel { PortalProfiles = response?.PortalProfiles };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual PortalProfileModel GetPortalProfile(int portalProfileId, ExpandCollection expands)
        {
            string endpoint = PortalProfileEndpoint.Get(portalProfileId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            PortalProfileResponse response = GetResourceFromEndpoint<PortalProfileResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PortalProfile;
        }

        public virtual PortalProfileModel CreatePortalProfile(PortalProfileModel model)
        {
            string endpoint = PortalProfileEndpoint.Create();

            ApiStatus status = new ApiStatus();
            PortalProfileResponse response = PostResourceToEndpoint<PortalProfileResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PortalProfile;
        }

        public virtual PortalProfileModel UpdatePortalProfile(PortalProfileModel model)
        {
            string endpoint = PortalProfileEndpoint.Update();

            ApiStatus status = new ApiStatus();
            PortalProfileResponse response = PutResourceToEndpoint<PortalProfileResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.PortalProfile;
        }

        public virtual bool DeletePortalProfile(ParameterModel portalProfileIds)
        {
            string endpoint = PortalProfileEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalProfileIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
