using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using System.Web.Http.Description;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class LicenseController : BaseController
    {
        #region Private Members
        private readonly ILicenseService _service;
        #endregion

        #region Controller
        public LicenseController()
        {
            _service = new LicenseService();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the Installed Znode License Information
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(LicenseResponse))]
        [HttpGet]
        public HttpResponseMessage GetLicenseInformation()
        {
            HttpResponseMessage response;
            try
            {
                //Get the Znode Installed License Information.
                LicenceInfoModel data = _service.GetLicenceInformation();
                response = !Equals(data, null) ? CreateOKResponse(new LicenseResponse { License = data }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                LicenseResponse data = new LicenseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion
    }
}