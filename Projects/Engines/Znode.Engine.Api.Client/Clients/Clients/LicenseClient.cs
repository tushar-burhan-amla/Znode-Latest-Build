using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class LicenseClient: BaseClient, ILicenseClient
    {
        public virtual LicenceInfoModel GetLicenseInformation()
        {
            string endpoint = LicenseEndpoint.GetLicenseInformation();          

            ApiStatus status = new ApiStatus();
            LicenseResponse response = GetResourceFromEndpoint<LicenseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.License;
        }
    }
}
