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
    public class GlobalAttributeController : BaseController
    {
        #region Private Variables
        private readonly IGlobalAttributeCache _cache;
        private readonly IGlobalAttributeService _service;
        #endregion

        #region Constructor
        public GlobalAttributeController(IGlobalAttributeService service)
        {
            _service = service;
            _cache = new GlobalAttributeCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create Attribute
        /// </summary>
        /// <param name="model">Model with attribute data.</param>
        /// <returns>return GlobalAttributeDataModel</returns>
        [ResponseType(typeof(GlobalAttributeDataResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody] GlobalAttributeDataModel model)
        {
            HttpResponseMessage response;
            try
            {
               // Create global attribute.
                GlobalAttributeDataModel attribute = _service.CreateAttribute(model);
                if (!Equals(attribute, null))
                {
                    response = CreateCreatedResponse(new GlobalAttributeDataResponse { GlobalAttributeDataModel = attribute });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(attribute.AttributeModel.GlobalAttributeId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Warning);
                GlobalAttributeDataResponse data = new GlobalAttributeDataResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeDataResponse data = new GlobalAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save attribute Locale values
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Return GlobalAttributeLocaleListModel</returns>
        [ResponseType(typeof(GlobalAttributeDataResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveLocales([FromBody] GlobalAttributeLocaleListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create global attribute.
                GlobalAttributeLocaleListModel locales = _service.SaveLocales(model);
                if (!Equals(locales, null))
                {
                    response = CreateCreatedResponse(new GlobalAttributeDataResponse { Locales = locales });
                    response.Headers.Add("Location", GetUriLocation("GlobalAttributeLocales"));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                GlobalAttributeDataResponse data = new GlobalAttributeDataResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeDataResponse data = new GlobalAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save Attribute Default values.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Return GlobalAttributeDefaultValueModel</returns>
        [ResponseType(typeof(GlobalAttributeDataResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveDefaultValues([FromBody] GlobalAttributeDefaultValueModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create global attribute.
                GlobalAttributeDefaultValueModel defaultValues = _service.SaveDefaultValues(model);
                if (!Equals(defaultValues, null))
                {
                    response = CreateCreatedResponse(new GlobalAttributeDataResponse { DefaultValues = defaultValues });
                    response.Headers.Add("Location", GetUriLocation(defaultValues.GlobalAttributeId.ToString()));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeDataResponse data = new GlobalAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of attributes validations.
        /// </summary>
        /// <returns>Returns attributes validations list.</returns>
        [ResponseType(typeof(GlobalAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage InputValidations(int typeId, int attributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetInputValidations(typeId, attributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeListResponse data = new GlobalAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get attribute List
        /// </summary>
        /// <returns>Return Attribute list</returns>
        [ResponseType(typeof(GlobalAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAttributeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                GlobalAttributeListResponse data = new GlobalAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get attribute by attribute id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Return attribute details by Id</returns>
        [ResponseType(typeof(GlobalAttributeResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {
                //Get global attribute by ID.
                string data = _cache.GetAttribute(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                GlobalAttributeResponse data = new GlobalAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeResponse data = new GlobalAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Update attribute data
        /// </summary>
        /// <param name="model">model with attribute details</param>
        /// <returns></returns>
        [ResponseType(typeof(GlobalAttributeDataResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] GlobalAttributeDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update global attribute.
                bool attribute = _service.UpdateAttribute(model);
                response = attribute ? CreateCreatedResponse(new GlobalAttributeDataResponse { GlobalAttributeDataModel = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.AttributeModel.GlobalAttributeId)));

            }
            catch (Exception ex)
            {
                GlobalAttributeDataResponse data = new GlobalAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete global attribute Ids.
        /// </summary>
        /// <param name="globalAttributeids">Parameter Model for global attribute Ids.</param>
        /// <returns>return Boolean response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel globalAttributeids)
        {
            HttpResponseMessage response;
            try
            {
                //Delete attribute.
                bool deleted = _service.DeleteAttribute(globalAttributeids);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }


        /// <summary>
        /// Gets list of attributes validations.
        /// </summary>
        /// <param name="globalAttributeId">Uses global attribute id.</param>
        /// <returns>Returns attributes validations list.</returns>
        [ResponseType(typeof(GlobalAttributeLocaleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeLocale(int globalAttributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetAttributeLocale(globalAttributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeLocaleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                GlobalAttributeLocaleListResponse data = new GlobalAttributeLocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of attributes validations.
        /// </summary>
        /// <param name="globalAttributeId">Uses global Attribute Id.</param>
        /// <returns>Returns attributes validations list.</returns>
        [ResponseType(typeof(GlobalAttributeLocaleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDefaultValues(int globalAttributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetDefaultValues(globalAttributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeLocaleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                GlobalAttributeLocaleListResponse data = new GlobalAttributeLocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Check attribute code already exist or not
        /// </summary>
        /// <param name="attributeCode">attributeCode</param>
        /// <returns>returns true if attribute code exist</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsAttributeCodeExist(string attributeCode)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsAttributeCodeExist(attributeCode) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Default Attribute Id
        /// </summary>
        /// <param name="defaultvalueId">Global attribute Default Value Id</param>
        /// <returns></returns>
        [ResponseType(typeof(GlobalAttributeResponse))]
        [HttpDelete]
        public virtual HttpResponseMessage DeleteDefaultValues(int defaultvalueId)
        {
            HttpResponseMessage response;
            try
            {
                //Delete attribute.
                bool deleted = _service.DeleteDefaultValues(defaultvalueId);

                response = deleted ? CreateOKResponse() : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                GlobalAttributeResponse data = new GlobalAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeResponse data = new GlobalAttributeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Check unique attribute code already exist or not.
        /// </summary>
        ///<param name="parameters">Parameter model with data.</param>
        /// <returns>Returns true if attribute code exist.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage IsAttributeValueUnique([FromBody] GlobalAttributeValueParameterModel parameters)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new StringResponse { Response = _service.IsAttributeValueUnique(parameters) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}