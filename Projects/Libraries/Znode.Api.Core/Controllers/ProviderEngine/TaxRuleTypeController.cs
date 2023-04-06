using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class TaxRuleTypeController : BaseController
    {
        #region Private Variables
        private readonly ITaxRuleTypeCache _taxRuleTypeCache;
        private readonly ITaxRuleTypeService _taxRuleTypeService;
        #endregion

        #region Constructor
        public TaxRuleTypeController(ITaxRuleTypeService taxRuleTypeService)
        {
            _taxRuleTypeService = taxRuleTypeService;
            _taxRuleTypeCache = new TaxRuleTypeCache(_taxRuleTypeService);
        }
        #endregion

        /// <summary>
        /// Get the list of all Tax Rule Types.
        /// </summary>
        /// <returns>Return list of all tax group</returns>
        [ResponseType(typeof(TaxRuleTypeListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _taxRuleTypeCache.GetTaxRuleTypeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxRuleTypeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TaxRuleTypeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get a Tax RuleType.
        /// </summary>
        /// <param name="taxRuleTypeId">ID of tax rule type to get the details.</param>
        /// <returns>It will return detail about the tax rule depend upon the given ID.</returns>
        [ResponseType(typeof(TaxRuleTypeResponse))]
        [HttpGet]
        public HttpResponseMessage Get(int taxRuleTypeId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _taxRuleTypeCache.GetTaxRuleType(taxRuleTypeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxRuleTypeResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TaxRuleTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create new Tax Rule Type.
        /// </summary>
        /// <param name="taxRuleTypeModel">TaxRuleType Model to create new tax rule type.</param>
        /// <returns>It will response when new Tax rule will add successfully</returns>
        [ResponseType(typeof(TaxRuleTypeResponse))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] TaxRuleTypeModel taxRuleTypeModel)
        {
            HttpResponseMessage response;
            try
            {
                TaxRuleTypeModel taxRuleType = _taxRuleTypeService.CreateTaxRuleType(taxRuleTypeModel);

                if (!Equals(taxRuleType, null))
                {
                    response = CreateCreatedResponse(new TaxRuleTypeResponse { TaxRuleType = taxRuleType });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(taxRuleType.TaxRuleTypeId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TaxRuleTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update tax rule type.
        /// </summary>
        /// <param name="taxRuleTypeModel">TaxRuleType Model to update tax rule type.</param>
        /// <returns>It will Response with success when the Tax will Update successfully</returns>
        [ResponseType(typeof(TaxRuleTypeResponse))]
        [HttpPut]
        public HttpResponseMessage Update([FromBody] TaxRuleTypeModel taxRuleTypeModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _taxRuleTypeService.UpdateTaxRuleType(taxRuleTypeModel);
                response = isUpdated ? CreateOKResponse(new TaxRuleTypeResponse { TaxRuleType = taxRuleTypeModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TaxRuleTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete tax rule type
        /// </summary>
        /// <param name="Ids">IDs of tax rule type to delete.</param>
        /// <returns>Return true if deleted successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage Delete([FromBody] ParameterModel Ids)
        {
            HttpResponseMessage response;
            try
            {
                bool isDeleted = _taxRuleTypeService.DeleteTaxRuleType(Ids);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isDeleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get all Tax Rule Types which are not present in database.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TaxRuleTypeListResponse))]
        [HttpGet]
        public HttpResponseMessage GetAllTaxRuleTypesNotInDatabase()
        {
            HttpResponseMessage response;
            try
            {
                string data = _taxRuleTypeCache.GetAllTaxRuleTypesNotInDatabase(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxRuleTypeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TaxRuleTypeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Bulk enable or disable tax rule types.
        /// </summary>
        /// <param name="entityIds">Ids of tax rule types.</param>
        /// <param name="isEnable">boolean value true/false</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage BulkEnableDisableTaxRuleTypes([FromBody] ParameterModel entityIds, bool isEnable)
        {
            HttpResponseMessage response;
            try
            {
                bool isEnabled = _taxRuleTypeService.EnableDisableTaxRuleType(entityIds, isEnable);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isEnabled });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}
