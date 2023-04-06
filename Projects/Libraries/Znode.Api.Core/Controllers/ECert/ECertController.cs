using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Core.Controllers
{
    public class ECertController : BaseController
    {
        #region Private Variables 
        private readonly IECertCache _cache;
        private readonly IECertService _service;
        #endregion

        #region Constructor
        public ECertController(IECertService service)
        {
            _service = service;
            _cache = new ECertCache(_service);
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Get the total balance amount that is available in ERP for the ECertificate associated with logged in userName.
        /// </summary>
        /// <param name="eCertTotalModel">Details useful in retrieving the total available certificate balance.</param>
        /// <returns>Balance amount that is available in ERP for the ECertificate</returns>
        [ResponseType(typeof(BaseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetAvailableECertBalance([FromBody]ECertTotalModel eCertTotalModel)
        {
            HttpResponseMessage response;

            try
            {
                ECertTotalBalanceModel data = _service.GetAvailableECertBalance(eCertTotalModel);
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse<ECertTotalBalanceResponse>(new ECertTotalBalanceResponse {  ECertTotalBalance = data }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.ProcessingFailed });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Ge the list of all associated eCertificates.
        /// </summary>
        /// <returns>Returns Tax Class list.</returns>
        [ResponseType(typeof(ECertificateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetECertificateList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ECertificateListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                ECertificateListResponse data = new ECertificateListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                ECertificateListResponse data = new ECertificateListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Inserts and associate the e-certificate to the wallet.
        /// </summary>
        /// <param name="eCertificateModel"></param>
        /// <returns></returns>
        public virtual HttpResponseMessage Create([FromBody]ECertificateModel eCertificateModel)
        {
            HttpResponseMessage response;
             try
            {
                ECertificateModel eCertificate = _service.AddECertificateToWallet(eCertificateModel);
                if (!Equals(eCertificate, null))
                {
                    response = CreateCreatedResponse(new ECertificateResponse {  ECertificate = eCertificate });
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                ECertificateResponse taxClass = new ECertificateResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(taxClass);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                ECertificateResponse taxClass = new ECertificateResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(taxClass);
            }
            return response;

        }
        #endregion
    }
}
