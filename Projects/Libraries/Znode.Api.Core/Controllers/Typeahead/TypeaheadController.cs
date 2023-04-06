using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class TypeaheadController : BaseController
    {
        #region Private Variables
        private readonly ITypeaheadCache _typeaheadCache;
        private readonly ITypeaheadService _typeaheadService;
        #endregion

        #region Constructor
        public TypeaheadController(ITypeaheadService service)
        {
            _typeaheadService = service;
            _typeaheadCache = new TypeaheadCache(_typeaheadService);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the suggestions.
        /// </summary>
        /// <returns>Returns suggestions list.</returns>
        [ResponseType(typeof(TypeaheadListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetTypeaheadResponse([FromBody] TypeaheadRequestModel model)
        {
            HttpResponseMessage response;
            try
            {
                TypeaheadResponselistModel list = _typeaheadService.GetTypeaheadList(model);
                response = HelperUtility.IsNotNull(list) ? CreateOKResponse(list) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TypeaheadListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
