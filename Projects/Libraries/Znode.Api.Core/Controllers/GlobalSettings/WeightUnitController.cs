using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class WeightUnitController : BaseController
    {
        #region Private Variables
        private readonly IWeightUnitService _service;
        private readonly IWeightUnitCache _cache;
        #endregion

        #region Default Constructor
        public WeightUnitController(IWeightUnitService service)
        {
            _service = service;
            _cache = new WeightUnitCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets list of all WeightUnits.
        /// </summary>
        /// <returns>List of all weight units.</returns>
        [ResponseType(typeof(WeightUnitListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetWeightUnits(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<WeightUnitListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new WeightUnitListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
            }

            return response;
        }


        /// <summary>
        /// Updates WeightUnit details.
        /// </summary>
        /// <param name="weightUnitModel">WeightUnitModel to be updated</param>
        /// <returns>HttpResponse for Weight Unit Model .</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]WeightUnitModel weightUnitModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isSuccess = _service.UpdateWeightUnit(weightUnitModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isSuccess });
            }
            catch (Exception ex)
            {
                var data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
            }

            return response;
        }
    }
}