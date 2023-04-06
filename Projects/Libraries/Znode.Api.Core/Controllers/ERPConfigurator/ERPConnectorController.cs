using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class ERPConnectorController : BaseController
    {
        #region Private Variables
        private readonly IERPConnectorCache _cache;
        private readonly IERPConnectorService _service;
        #endregion

        #region Constructor
        public ERPConnectorController(IERPConnectorService service)
        {
            _service = service;
            _cache = new ERPConnectorCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get ERPConnectorControls.
        /// </summary>
        /// <param name="erpConfiguratorModel">ERPConfiguratorModel</param>
        /// <returns>Returns ERP connector control list.</returns>
        [ResponseType(typeof(ERPConnectorControlListResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage GetERPConnectorControls([FromBody] ERPConfiguratorModel erpConfiguratorModel)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetERPConnectorControlList(erpConfiguratorModel, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ERPConnectorControlListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConnectorControlListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Method to Save ERP Control Data in json file
        /// </summary>
        /// <param name="model">ERP Connector Control List Model.</param>
        /// <returns>Returns ERP connector control.</returns>
        [ResponseType(typeof(ERPConnectorControlResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CreateERPControlData([FromBody] ERPConnectorControlListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create ERP Control Data
                ERPConnectorControlListModel erpConnectorControls = _service.SaveERPControlData(model);
                response = erpConnectorControls?.ERPConnectorControlList.Count > 0 ? CreateCreatedResponse(new ERPConnectorControlListResponse { ERPConnectorControls = erpConnectorControls.ERPConnectorControlList }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConnectorControlResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConnectorControlResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
