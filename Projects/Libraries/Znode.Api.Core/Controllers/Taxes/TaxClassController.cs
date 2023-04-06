using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class TaxClassController : BaseController
    {
        #region Private Variables
        private readonly ITaxClassCache _cache;
        private readonly ITaxClassService _service;
        #endregion

        #region Constructor
        public TaxClassController(ITaxClassService service)
        {
            _service = service;
            _cache = new TaxClassCache(_service);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Ge the list of all Tax Classes.
        /// </summary>
        /// <returns>Returns Tax Class list.</returns>
        [ResponseType(typeof(TaxClassListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTaxClassList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxClassListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxClassListResponse data = new TaxClassListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get a Tax Class.
        /// </summary>
        /// <param name="taxClassId">ID of tax class to get the details.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(TaxClassResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int taxClassId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTaxClass(taxClassId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxClassResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxClassResponse data = new TaxClassResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create new Tax Class.
        /// </summary>
        /// <param name="taxClassModel">TaxClass model to be created.</param>
        /// <returns>>Returns created tax class.</returns>
        [ResponseType(typeof(TaxClassResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] TaxClassModel taxClassModel)
        {
            HttpResponseMessage response;
            try
            {
                TaxClassModel taxClass = _service.CreateTaxClass(taxClassModel);

                if (!Equals(taxClass, null))
                {
                    response = CreateCreatedResponse(new TaxClassResponse { TaxClass = taxClass });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(taxClass.TaxClassId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TaxClassResponse taxClass = new TaxClassResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(taxClass);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxClassResponse taxClass = new TaxClassResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(taxClass);
            }
            return response;
        }

        /// <summary>
        /// Update tax class.
        /// </summary>
        /// <param name="taxClassModel">TaxClass Model to update.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(TaxClassResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] TaxClassModel taxClassModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateTaxClass(taxClassModel);
                response = isUpdated ? CreateOKResponse(new TaxClassResponse { TaxClass = taxClassModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TaxClassResponse data = new TaxClassResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxClassResponse data = new TaxClassResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete tax class
        /// </summary>
        /// <param name="taxClassIds">ID of tax class to delete.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel taxClassIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteTaxClass(taxClassIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #region Tax Class SKU

        /// <summary>
        /// Get Tax Class SKU list.
        /// </summary>
        /// <returns>Returns Tax Class SKU list model.</returns>
        [ResponseType(typeof(TaxClassSKUListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTaxClassSKUList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTaxClassSKUList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxClassSKUListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxClassSKUListResponse data = new TaxClassSKUListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get List of Unassociated Product.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnassociatedProductList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnassociatedProductList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }


        /// <summary>
        /// Method to add Tax Class SKU.
        /// </summary>
        /// <param name="taxClassSKU">TaxClassSKU model.</param>
        /// <returns>Returns created Tax Class SKU model.</returns>
        [ResponseType(typeof(TaxClassSKUResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AddTaxClassSKU([FromBody] TaxClassSKUModel taxClassSKU)
        {
            HttpResponseMessage response;
            try
            {
                taxClassSKU = _service.AddTaxClassSKU(taxClassSKU);
                if (IsNotNull(taxClassSKU))
                {
                    response = CreateCreatedResponse(new TaxClassSKUResponse { TaxClassSKU = taxClassSKU });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(taxClassSKU.TaxClassSKUId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TaxClassSKUResponse data = new TaxClassSKUResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                TaxClassSKUResponse data = new TaxClassSKUResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }



        /// <summary>
        /// Delete an existing Tax Class SKU.
        /// </summary>
        /// <param name="taxClassSKUId">Ids to delete Tax Class SKU.</param>
        /// <returns>Returns true/false as per delete operation.</returns>     
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteTaxClassSKU(ParameterModel taxClassSKUId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteTaxClassSKU(taxClassSKUId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TaxClassSKUResponse data = new TaxClassSKUResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #endregion

        #region Tax Rule

        /// <summary>
        /// Get Tax Rule list.
        /// </summary>
        /// <returns>Returns Tax Rule list model.</returns>
        [ResponseType(typeof(TaxRuleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTaxRuleList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTaxRuleList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxRuleListResponse>(data) : CreateNoContentResponse();
           }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxRuleListResponse data = new TaxRuleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// To get Tax Rule by TaxRuleId.
        /// </summary>
        /// <param name="taxRuleId">Id to get tax rule detail</param>
        /// <returns>Returns Tax Rule model.</returns>   
        [ResponseType(typeof(TaxRuleResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTaxRule(int taxRuleId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTaxRule(taxRuleId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxRuleResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TaxRuleResponse data = new TaxRuleResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxRuleResponse data = new TaxRuleResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Method to add Tax Rule.
        /// </summary>
        /// <param name="taxRule">TaxRule model.</param>
        /// <returns>Returns created Tax Rule model.</returns>
        [ResponseType(typeof(TaxRuleResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AddTaxRule([FromBody] TaxRuleModel taxRule)
        {
            HttpResponseMessage response;
            try
            {
                taxRule = _service.AddTaxRule(taxRule);
                if (IsNotNull(taxRule))
                {
                    response = CreateCreatedResponse(new TaxRuleResponse { TaxRule = taxRule });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(taxRule.TaxRuleId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TaxRuleResponse data = new TaxRuleResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxRuleResponse data = new TaxRuleResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update an existing Tax Rule.
        /// </summary>
        /// <param name="taxRule">Tax Rule model to be updated.</param>
        /// <returns>Returns updated Tax Rule model.</returns>
        [ResponseType(typeof(TaxRuleResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateTaxRule([FromBody] TaxRuleModel taxRule)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateTaxRule(taxRule);
                response = data ? CreateCreatedResponse(new TaxRuleResponse { TaxRule = taxRule }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TaxRuleResponse data = new TaxRuleResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TaxRuleResponse data = new TaxRuleResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete an existing Tax Rule.
        /// </summary>
        /// <param name="taxRuleId">Ids to delete Tax Rule.</param>
        /// <returns>Returns true/false as per delete operation.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteTaxRule(ParameterModel taxRuleId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteTaxRule(taxRuleId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #endregion

        #region Avalara
        /// <summary>
        /// Test the avalara connection.
        /// </summary>
        /// <param name="taxPortalModel">Tax portal model containing credentials.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage TestAvalaraConnection([FromBody] TaxPortalModel taxPortalModel)
        {
            HttpResponseMessage response;
            try
            {
                string connectionResponse = _service.TestAvalaraConnection(taxPortalModel);
                response = CreateOKResponse(new StringResponse { Response = connectionResponse });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #endregion
    }
}
