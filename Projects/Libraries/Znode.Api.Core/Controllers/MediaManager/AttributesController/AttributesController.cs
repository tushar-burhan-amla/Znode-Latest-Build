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
    public class AttributesController : BaseController
    {
        #region Private Variables
        private readonly IAttributesService _service;
        private readonly IAttributesCache _cache;
        #endregion

        #region Default Constructor
        public AttributesController(IAttributesService service)
        {
            _service = service;
            _cache = new AttributesCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets list of attributes.
        /// </summary>
        /// <returns>Returns attributes list.</returns>
        [ResponseType(typeof(AttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeList()
        {
            HttpResponseMessage response;
            try
            {
                //Get attributes.
                string data = _cache.GetAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeListResponse data = new AttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets list of attributes validations.
        /// </summary>
        /// <returns>Returns attributes validations list.</returns>
        [ResponseType(typeof(AttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage InputValidations(int typeId, int attributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetInputValidations(typeId, attributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributeListResponse data = new AttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Method to create attribute.
        /// </summary>
        /// <param name="model">Attributes model.</param>
        /// <returns>Returns a created attribute.</returns>
        [ResponseType(typeof(AttributesResponses))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody] AttributesDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create attribute.
                AttributesDataModel attribute = _service.CreateAttribute(model);
                if (!Equals(attribute, null))
                {
                    response = CreateCreatedResponse(new AttributesResponses { Attribute = attribute });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(attribute.MediaAttributeId)));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch(ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                AttributesResponses data = new AttributesResponses { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode};
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributesResponses data = new AttributesResponses { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;

        }

        /// <summary>
        /// Method to update attribute.
        /// </summary>
        /// <param name="model">Attributes model.</param>
        /// <returns>Returns updated attribute.</returns>
        [ResponseType(typeof(AttributesResponses))]
        [HttpPost]
        public virtual HttpResponseMessage Update([FromBody] AttributesDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update attribute.
                bool attribute = _service.UpdateAttribute(model);
                response = attribute ? CreateOKResponse(new AttributesResponses { Attribute = model }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.MediaAttributeId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                AttributesResponses data = new AttributesResponses { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributesResponses data = new AttributesResponses { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;

        }

        /// <summary>
        /// Delete attribute by attributeId
        /// </summary>
        /// <param name="mediaAttributeids">Parameter Model for Media Attribute Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel mediaAttributeids)
        {
            HttpResponseMessage response;
            try
            {
                //Delete attribute.
                bool deleted = _service.DeleteAttribute(mediaAttributeids);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets a attribute.
        /// </summary>
        /// <param name="attributeId">The Id of the attribute.</param>
        /// <returns>Returns attribute.</returns>
        [ResponseType(typeof(AttributesResponses))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttribute(int attributeId)
        {
            HttpResponseMessage response;

            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetAttribute(attributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributesResponses>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributesResponses data = new AttributesResponses { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets list of attributes.
        /// </summary>
        /// <returns>Returns attributes list.</returns>
        [ResponseType(typeof(AttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeTypeList()
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetAttributeTypes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributeListResponse data = new AttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save Attribute Locale Values
        /// </summary>
        /// <param name="model">attribute locale model</param>
        /// <returns>Returns the attribute locales.</returns>
        [ResponseType(typeof(AttributesDataResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveLocales([FromBody] AttributesLocaleListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute.
                AttributesLocaleListModel locales = _service.SaveLocales(model);
                if (!Equals(locales, null))
                {
                    response = CreateCreatedResponse(new AttributesDataResponse { Locales = locales });
                    response.Headers.Add("Location", GetUriLocation("AttributeLocales"));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                AttributesDataResponse data = new AttributesDataResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributesDataResponse data = new AttributesDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save Attribute Default values
        /// </summary>
        /// <param name="model">attribute default values model</param>
        /// <returns>Returns the attribute default values.</returns>
        [ResponseType(typeof(AttributesDataResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveDefaultValues([FromBody] AttributesDefaultValueModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute.
                AttributesDefaultValueModel defaultValues = _service.SaveDefaultValues(model);
                if (!Equals(defaultValues, null))
                {
                    response = CreateCreatedResponse(new AttributesDataResponse { DefaultValues = defaultValues });
                    response.Headers.Add("Location", GetUriLocation(defaultValues.AttributeId.ToString()));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributesDataResponse data = new AttributesDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete Default values by Attribute Id
        /// </summary>
        /// <param name="defaultvalueId">PIm attribute Default Value Id</param>
        /// <returns>Returns the attribute locales.</returns>
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
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                AttributesDataResponse data = new AttributesDataResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributesDataResponse data = new AttributesDataResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of attributes validations.
        /// </summary>
        /// <returns>Returns attributes validations list.</returns>
        [ResponseType(typeof(AttributesLocaleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDefaultValues(int AttributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetDefaultValues(AttributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributesLocaleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributesLocaleListResponse data = new AttributesLocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// get attribute locale values by attribute id
        /// </summary>
        /// <param name="attributeId"></param>
        /// <returns>attribute locale values</returns>
        [ResponseType(typeof(AttributeLocaleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeLocale(int attributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute locale by attribute id.
                string data = _cache.GetAttributeLocale(attributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeLocaleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributeLocaleListResponse data = new AttributeLocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Check attribute Code already exist or not
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
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsAttributeCodeExist(attributeCode)});
            }
            catch(ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
            }
            return response;
        }
    }
}
