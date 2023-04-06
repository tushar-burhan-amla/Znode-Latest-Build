using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Api.Controllers
{
    public class DiagnosticsController : BaseController
    {
        #region Variables

        private readonly IDiagnosticsService _service;
        private readonly IDiagnosticsCache _cache;

        #endregion

        #region Constructor

        public DiagnosticsController()
        {
            _service = GetService<IDiagnosticsService>();
            _cache = new DiagnosticsCache(_service);
        }

        #endregion

        /// <summary>
        /// This method calls diagnostics cache to check SMTP Account
        /// </summary>
        /// <returns>DiagnosticsResponse in HttpResponseMessage format which will contain the status of SMPT account.</returns>        
        [HttpGet]
        public HttpResponseMessage CheckEmailAccount()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.CheckEmailAccount();
                response = !Equals(data, null) ? CreateOKResponse<DiagnosticsResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DiagnosticsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// This method calls diagnostics cache to get Version details of product from database
        /// </summary>
        /// <returns>Returns the DiagnosticsResponse in HttpResponseMessage format which contains the version details.</returns>        
        [HttpGet]
        public HttpResponseMessage GetProductVersionDetails()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProductVersionDetails();
                response = !Equals(data, null) ? CreateOKResponse<DiagnosticsResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DiagnosticsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// This method sends the diagnostics email
        /// </summary>
        /// <param name="model">DiagnosticsEmailModel which should contain case number and merged text (diagnostics data)</param>
        /// <returns>Returns the DiagnosticsResponse in HttpResponseMessage format which contains the email sent status.</returns>
        [HttpPost]
        public HttpResponseMessage EmailDiagnostics(DiagnosticsEmailModel model)
        {
            HttpResponseMessage response;
            try
            {
                DiagnosticsResponse data = new DiagnosticsResponse { HasError = !_service.EmailDiagnostics(model) };
                response = IsNotNull(data) ? CreateOKResponse<DiagnosticsResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DiagnosticsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName">service name</param>
        /// <returns>Return Diagnostics Response with service status.</returns>
        [HttpGet]
        public HttpResponseMessage CheckService(string serviceName)
        {
            HttpResponseMessage response;
            try
            {
                DiagnosticsResponse data = new DiagnosticsResponse { ServiceStatus = _service.CheckService(serviceName) };
                response = IsNotNull(data) ? CreateOKResponse<DiagnosticsResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DiagnosticsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// This method gets the diagnostics list.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetDiagnosticsList()
        {
            HttpResponseMessage response;
            try
            {
                DiagnosticsResponse data = new DiagnosticsResponse { Diagnostics = _service.GetDiagnosticsList() };
                response = !Equals(data, null) ? CreateOKResponse<DiagnosticsResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DiagnosticsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }
    }
}