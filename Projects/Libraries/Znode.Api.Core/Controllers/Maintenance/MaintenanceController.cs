using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class MaintenanceController : BaseController
    {
        #region Private Variables

        private readonly IMaintenanceService _service;

        #endregion Private Variables

        #region Constructor

        public MaintenanceController(IMaintenanceService service)
        {
            _service = service;
        }

        #endregion Constructor

        /// <summary>
        /// To delete published data of all catalog, store,cms and elastic search
        /// </summary>
        /// <returns>If successfully perform then return true else false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpDelete]
        public virtual HttpResponseMessage PurgeAllPublishedData()
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.PurgeAllPublishedData();
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, "Maintenance", TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Maintenance", TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
    }
}
