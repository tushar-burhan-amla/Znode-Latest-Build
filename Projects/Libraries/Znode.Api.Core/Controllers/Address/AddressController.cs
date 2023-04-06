using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class AddressController : BaseController
    {
        #region Private Variables
        private readonly IAddressCache _cache;     
        #endregion

        #region Constructor
        public AddressController(IAddressService service)
        {
            _cache = new AddressCache(service);
        }
        #endregion

        #region Public Methods

        [ResponseType(typeof(AddressListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAddressList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAddressList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddressListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProfileListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        #endregion
    }
}
