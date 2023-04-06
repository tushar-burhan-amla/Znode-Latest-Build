using Newtonsoft.Json;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class DiagnosticsCache : BaseCache, IDiagnosticsCache
    {
        #region variables

        private readonly IDiagnosticsService _service;

        #endregion

        #region Constructor

        public DiagnosticsCache(IDiagnosticsService diagnosticsService)
        {
            _service = diagnosticsService;
        }

        #endregion

        /// <summary>
        /// This method calls diagnostics service to check SMTP Account
        /// </summary>
        /// <returns>DiagnosticsResponse in string format which will contain the status of 'Simple Mail Transfer Protocol' account</returns>
        public string CheckEmailAccount()
        {
            DiagnosticsResponse response = new DiagnosticsResponse() { HasError = !_service.CheckEmailAccount() };
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>
        /// This method calls diagnostics service to get Version details of product from database
        /// </summary>
        /// <returns>DiagnosticsResponse in string format which will contain the details of product version</returns>
        public string GetProductVersionDetails()
        {
            DiagnosticsResponse response = new DiagnosticsResponse() { ProductVersion = _service.GetProductVersionDetails() };
            return JsonConvert.SerializeObject(response);
        }
    }
}