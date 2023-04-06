using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class PortalCountryClient : BaseClient , IPortalCountryClient
    {
        #region Country Association.
        // Get unassociated country list.
        public virtual CountryListModel GetUnAssociatedCountryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PortalEndpoint.GetUnAssociatedCountryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CountryListResponse response = GetResourceFromEndpoint<CountryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CountryListModel list = new CountryListModel { Countries = response?.Countries };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get associated country list based on portals.
        public virtual CountryListModel GetAssociatedCountryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PortalEndpoint.GetAssociatedCountryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CountryListResponse response = GetResourceFromEndpoint<CountryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CountryListModel list = new CountryListModel { Countries = response?.Countries };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //UnAssociate associated countries by portalCountryIds.
        public virtual bool UnAssociateCountries(ParameterModelForPortalCountries portalCountries)
        {
            string endpoint = PortalEndpoint.UnAssociateCountries();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalCountries), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Associate unassociated countries.
        public virtual bool AssociateCountries(ParameterModelForPortalCountries parameterModel)
        {
            string endpoint = PortalEndpoint.AssociateCountries();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion
    }
}
