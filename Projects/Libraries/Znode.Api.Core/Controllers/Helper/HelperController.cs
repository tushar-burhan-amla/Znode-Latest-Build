using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Core.Controllers
{
    public class HelperController : BaseController
    {
        /// <summary>
        /// Check Code exists in DB .
        /// </summary>
        /// <param name="parameterModel">parameter Model</param>
        /// <param name="service">ServiceName of Entity</param>
        /// <param name="methodName">method name</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage IsCodeExists(HelperParameterModel parameterModel, string service, string methodName)
        {
            HttpResponseMessage response = null;
            try
            {

                bool status = ServiceHelper.ExecuteFunctionByName(parameterModel, service, methodName);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}
