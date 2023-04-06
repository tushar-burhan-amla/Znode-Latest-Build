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
    public class WeightUnitClient : BaseClient, IWeightUnitClient
    {
        // Gets the list of WeightUnit.
        public virtual WeightUnitListModel GetWeightUnitList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetWeightUnitList(expands, filters, sorts, null, null);

        // Gets the list of WeightUnit.
        public virtual WeightUnitListModel GetWeightUnitList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = WeightUnitEndpoint.GetWeightUnitList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            WeightUnitListResponse response = GetResourceFromEndpoint<WeightUnitListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WeightUnitListModel list = new WeightUnitListModel { WeightUnits = response?.WeightUnits };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Update WeightUnits
        public virtual bool UpdateWeightUnit(WeightUnitModel weightUnitModel)
        {
            //Get Endpoint
            string endpoint = WeightUnitEndpoint.UpdateWeightUnit();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(weightUnitModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
