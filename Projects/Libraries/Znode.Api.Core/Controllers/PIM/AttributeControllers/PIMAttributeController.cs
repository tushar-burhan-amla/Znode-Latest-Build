using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using System.Web.Http.Description;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class PIMAttributeController : BaseController
    {
        #region Private Variables
        private readonly IPIMAttributeCache _cache;
        private readonly IPIMAttributeService _service;
        #endregion

        #region Constructor
        public PIMAttributeController(IPIMAttributeService service)
        {
            _service = service;
            _cache = new PIMAttributeCache(_service);

        }
        #endregion

        #region Controller Actions

        /// <summary>
        /// Get attribute List
        /// </summary>
        /// <returns>Return Attribute list</returns>
        [ResponseType(typeof(PIMAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAttributeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                PIMAttributeListResponse data = new PIMAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// get attribute by attribute id
        /// </summary>
        /// <param name="id">Attribute id</param>
        /// <returns>Return attribute details by Id</returns>
        [ResponseType(typeof(PIMAttributeResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {
                //Get PIM attribute by ID.
                string data = _cache.GetAttribute(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeResponse data = new PIMAttributeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Create Attribute
        /// </summary>
        /// <param name="model">model with attribute data</param>
        /// <returns>return PIMAttributeDataModel</returns>
        [ResponseType(typeof(PIMAttributeDataResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] PIMAttributeDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute.
                PIMAttributeDataModel attribute = _service.CreateAttribute(model);
                if (!Equals(attribute, null))
                {
                    response = CreateCreatedResponse(new PIMAttributeDataResponse { PIMAttributeDataModel = attribute });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(attribute.AttributeModel.PimAttributeId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                PIMAttributeDataResponse data = new PIMAttributeDataResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeDataResponse data = new PIMAttributeDataResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save attribute Locale values
        /// </summary>
        /// <param name="model">PIM Attribute Locale List Model</param>
        /// <returns>Return PIMAttributeLocaleListModel</returns>
        [ResponseType(typeof(PIMAttributeDataResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveLocales([FromBody] PIMAttributeLocaleListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute.
                PIMAttributeLocaleListModel locales = _service.SaveLocales(model);
                if (!Equals(locales, null))
                {
                    response = CreateCreatedResponse(new PIMAttributeDataResponse { Locales = locales });
                    response.Headers.Add("Location", GetUriLocation("PimAttributeLocales"));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeDataResponse data = new PIMAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// save Attribute Default values
        /// </summary>
        /// <param name="model">PIM Attribute Default Value Model</param>
        /// <returns>Return PIMAttributeDefaultValueModel</returns>
        [ResponseType(typeof(PIMAttributeDataResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SaveDefaultValues([FromBody] PIMAttributeDefaultValueModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute.
                PIMAttributeDefaultValueModel defaultValues = _service.SaveDefaultValues(model);
                if (!Equals(defaultValues, null))
                {
                    response = CreateCreatedResponse(new PIMAttributeDataResponse { DefaultValues = defaultValues });
                    response.Headers.Add("Location", GetUriLocation(defaultValues.PimAttributeId.ToString()));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeDataResponse data = new PIMAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete Default Attribute Id
        /// </summary>
        /// <param name="defaultvalueId">PIM attribute Default Value Id</param>
        /// <returns></returns>
        [ResponseType(typeof(PIMAttributeResponse))]
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
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                PIMAttributeResponse data = new PIMAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeResponse data = new PIMAttributeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update attribute data
        /// </summary>
        /// <param name="model">model with attribute details</param>
        /// <returns></returns>
        [ResponseType(typeof(PIMAttributeDataResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] PIMAttributeDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update PIM Attribute.
                bool attribute = _service.UpdateAttribute(model);
                response = attribute ? CreateCreatedResponse(new PIMAttributeDataResponse { PIMAttributeDataModel = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.AttributeModel.PimAttributeId)));

            }
            catch (Exception ex)
            {
                PIMAttributeDataResponse data = new PIMAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete PIM Attribute Ids.
        /// </summary>
        /// <param name="pimAttributeids">Parameter Model for PIM attribute Ids.</param>
        /// <returns>return Boolean response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel pimAttributeids)
        {
            HttpResponseMessage response;
            try
            {
                //Delete attribute.
                bool deleted = _service.DeleteAttribute(pimAttributeids);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }


        /// <summary>
        /// Gets list of attributes.
        /// </summary>
        /// <param name="isCategory"></param>
        /// <returns>Returns attributes list.</returns>
        [ResponseType(typeof(PIMAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage AttributeTypes(bool isCategory)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetAttributeTypes(isCategory, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeListResponse data = new PIMAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of attributes validations.
        /// </summary>
        /// <param name="typeId">Id of type </param>
        /// <param name="attributeId">attribute Id</param>
        /// <returns>Returns attributes validations list.</returns>
        [ResponseType(typeof(PIMAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage InputValidations(int typeId, int attributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetInputValidations(typeId, attributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                PIMAttributeListResponse data = new PIMAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Front End Properties By Attribute id
        /// </summary>
        /// <param name="pimAttributeId">Pim Attribute Id </param>
        /// <returns>Return Front End Properties of given Attribute id</returns>
        [ResponseType(typeof(PIMFrontPropertiesResponse))]
        [HttpGet]
        public virtual HttpResponseMessage FrontEndProperties(int pimAttributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.FrontEndProperties(pimAttributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMFrontPropertiesResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeListResponse data = new PIMAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }


        /// <summary>
        /// Gets list of attributes validations.
        /// </summary>
        /// <param name="pimAttributeId">Pim Attribute Id</param>
        /// <returns>Returns attributes validations list.</returns>
        [ResponseType(typeof(PIMAttributeLocaleListResponce))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeLocale(int pimAttributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetAttributeLocale(pimAttributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeLocaleListResponce>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                PIMAttributeLocaleListResponce data = new PIMAttributeLocaleListResponce { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of attributes validations.
        /// </summary>
        /// <param name="pimAttributeId">Pim Attribute Id</param>
        /// <returns>Returns attributes validations list.</returns>
        [ResponseType(typeof(PIMAttributeLocaleListResponce))]
        [HttpGet]
        public virtual HttpResponseMessage GetDefaultValues(int pimAttributeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetDefaultValues(pimAttributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeLocaleListResponce>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                PIMAttributeLocaleListResponce data = new PIMAttributeLocaleListResponce { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Check attribute Code already exist or not
        /// </summary>
        /// <param name="attributeCode">attributeCode</param>
        /// <param name="isCategory">true for category attribute else false</param>
        /// <returns>returns true if attribute code exist</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsAttributeCodeExist(string attributeCode, bool isCategory)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsAttributeCodeExist(attributeCode, isCategory) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Check attribute Code already exist or not
        /// </summary>
        /// <param name="parameters">attributeCode</param>
        /// <param name="isCategory">true for category attribute else false</param>
        /// <returns>returns true if attribute code exist</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage IsAttributeValueUnique([FromBody] PimAttributeValueParameterModel parameters)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new StringResponse { Response = _service.IsAttributeValueUnique(parameters) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get attribute validation by attribute code.
        /// </summary>
        /// <param name="attributeCodes">PIMFamily Model</param>
        /// <returns>Returns attributes validations.</returns>
        [ResponseType(typeof(PIMAttributeFamilyResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetAttributeValidationByCodes([FromBody] ParameterProductModel attributeCodes)
        {
            HttpResponseMessage response;

            try
            {
                PIMFamilyDetailsModel pimFamilyDetailsModel = _service.GetAttributeValidationByCodes(attributeCodes);
                response = pimFamilyDetailsModel?.Attributes?.Count > 0
                        ? CreateOKResponse(new PIMAttributeFamilyResponse { PIMFamilyDetails = pimFamilyDetailsModel }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeFamilyResponse data = new PIMAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion
    }
}