using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class ServerValidationController : BaseController
    {
        #region Private Variables
        private readonly IServerValidationService _service;
        #endregion

        #region Default Constructor
        public ServerValidationController(IServerValidationService service)
        {
            _service = service;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// validate the model
        /// </summary>
        /// <param name="mmModel">Validate Server Model</param>
        /// <returns>return http response</returns>
        [HttpPost]
        [ResponseType(typeof(ServerValidationResponses))]
        public virtual HttpResponseMessage Validate([FromBody] ValidateServerModel mmModel)
        {
            HttpResponseMessage response;
            try
            {
                var data = _service.CompairValidation(mmModel);
                response = !Equals(data, null) ? CreateOKResponse<ValidateServerModel>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                var data = new ServerValidationResponses { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        } 
        #endregion
    }
}
