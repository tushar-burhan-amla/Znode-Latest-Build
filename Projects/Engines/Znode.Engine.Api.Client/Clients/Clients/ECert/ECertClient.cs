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
    public class ECertClient : BaseClient, IECertClient
    {
        // Gets the list of orders.
        public virtual ECertificateListModel GetAvailableECertList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = ECertEndpoint.GetAvailableECertList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ECertificateListResponse response = GetResourceFromEndpoint<ECertificateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ECertificateListModel list = new ECertificateListModel { ECertificates = response?.ECertificates };
            list.MapPagingDataFromResponse(response);

            return list;

        }

        //Add eCertificate to Wallet
        public virtual ECertificateModel AddECertToBalance(ECertificateModel eCertificateModel) {

            string endpoint = ECertEndpoint.AddECertToBalance();
            ApiStatus status = new ApiStatus();
            ECertificateResponse response = PostResourceToEndpoint<ECertificateResponse>(endpoint, JsonConvert.SerializeObject(eCertificateModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.ECertificate;
        }
    }
}
