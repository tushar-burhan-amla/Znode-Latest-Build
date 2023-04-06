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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers
{
    public class GlobalAttributeFamilyController : BaseController
    {

        #region Private Variables
        private readonly IGlobalAttributeFamilyCache _cache;
        private readonly IGlobalAttributeFamilyService _service;
        #endregion

        #region Constructor
        public GlobalAttributeFamilyController(IGlobalAttributeFamilyService service)
        {
            _service = service;
            _cache = new GlobalAttributeFamilyCache(_service);
        }
        #endregion


        /// <summary>
        /// Gets the list of attribute group.
        /// </summary>
        /// <returns>Returns the list of attribute group.</returns>
        [ResponseType(typeof(GlobalAttributeFamilyListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.List(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeFamilyListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeFamilyListResponse data = new GlobalAttributeFamilyListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create Global Attribute Family
        /// </summary>
        /// <param name="model">GlobalAttributeFamilyModel model</param>
        /// <returns>GlobalAttributeFamilyModel</returns>
        [ResponseType(typeof(GlobalAttributeFamilyResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] GlobalAttributeFamilyCreateModel model)
        {
            HttpResponseMessage response;
            try
            {
                GlobalAttributeFamilyModel attributeFamily = _service.Create(model);
                if (!Equals(attributeFamily, null))
                {
                    response = CreateCreatedResponse(new GlobalAttributeFamilyResponse { AttributeFamily = attributeFamily });
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                GlobalAttributeFamilyResponse data = new GlobalAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeFamilyResponse data = new GlobalAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update Global Attribute Family
        /// </summary>
        /// <param name="model">GlobalAttributeFamilyModel model</param>
        /// <returns>GlobalAttributeFamilyModel</returns>
        [ResponseType(typeof(GlobalAttributeFamilyResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] GlobalAttributeFamilyUpdateModel model)
        {
            HttpResponseMessage response;
            try
            {
                GlobalAttributeFamilyModel attributeFamilyModel = _service.Update(model);
                response =  CreateCreatedResponse(new GlobalAttributeFamilyResponse { AttributeFamily = attributeFamilyModel, ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                GlobalAttributeFamilyResponse data = new GlobalAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeFamilyResponse data = new GlobalAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Attribute Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeFamilyModel</returns>
        [ResponseType(typeof(GlobalAttributeFamilyResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeFamily(string familyCode)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAttributeFamily(familyCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeFamilyResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                GlobalAttributeFamilyResponse data = new GlobalAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeFamilyResponse data = new GlobalAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Delete Global Attribute Family
        /// </summary>
        /// <param name="globalAttributeFamilyIds">comma separated Global Attribute FamilyIds</param>
        /// <returns>status of deletion</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel globalAttributeFamilyIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.Delete(globalAttributeFamilyIds);
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

        /// <summary>
        /// Delete Family By code
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>status of deletion</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteFamilyByCode(string familyCode)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteFamilyByCode(familyCode);
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


        /// <summary>
        /// Get Assigned Attribute Groups
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>List of assigned groups</returns>
        [ResponseType(typeof(GlobalAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssignedAttributeGroups(string familyCode)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssignedAttributeGroups(familyCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new GlobalAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Get unassigned entity group list.
        /// </summary>
        /// <returns>List of unassigned attribute groups</returns>
        [ResponseType(typeof(GlobalEntityListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnassignedAttributeGroups(string familyCode)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnassignedAttributeGroups(familyCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeEntityGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new AttributeEntityGroupListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Assign attribute groups
        /// </summary>
        /// <param name="attributeGroupIds">attributeGroupIds</param>
        /// <param name="familyCode">familyCode</param>
        /// <returns>status of assignment</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssignAttributeGroups(string attributeGroupIds, string familyCode)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.AssignAttributeGroups(attributeGroupIds, familyCode);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Assign attribute Groups by Group Code
        /// </summary>
        /// <param name="groupCode">groupCode</param>
        /// <param name="familyCode">familyCode</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssignAttributeGroupsByGroupCode(string groupCode, string familyCode)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.AssignAttributeGroupsByGroupCode(groupCode, familyCode);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            return response;
        }


        /// <summary>
        /// Unassign attribute groups
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <param name="groupCode">groupCode</param>
        /// <returns>status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UnassignAttributeGroups(string groupCode, string familyCode)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.UnassignAttributeGroups(groupCode, familyCode);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update attribute group display order
        /// </summary>
        /// <param name="groupCode">groupCode</param>
        /// <param name="familyCode">familyCode</param>
        /// <param name="displayOrder">displayOrder</param>
        /// <returns>status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int displayOrder)
        {
            HttpResponseMessage response;
            try
            {
                //Update global AttributeGroup.
                bool status = _service.UpdateAttributeGroupDisplayOrder(groupCode, familyCode, displayOrder);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });

            }
            return response;
        }

        /// <summary>
        /// Get attribute Family Locale
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeFamilyLocaleListModel model</returns>
        [ResponseType(typeof(GlobalAttributeFamilyListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeFamilyLocale(string familyCode)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAttributeFamilyLocales(familyCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeFamilyListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeFamilyListResponse data = new GlobalAttributeFamilyListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Check if the family code exists
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsFamilyCodeExist(string familyCode)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsAttributeFamilyExist(familyCode) });
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
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }


    }
}
