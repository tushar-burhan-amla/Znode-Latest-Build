using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using System.Web.Http.Description;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class AddonGroupController : BaseController
    {
        #region Private Variables
        private readonly IAddonGroupService _service;
        private readonly IAddonGroupCache _cache;
        #endregion

        public AddonGroupController(IAddonGroupService service)
        {
            _service = service;
            _cache = new AddonGroupCache(_service);
        }

        #region Addon Group

        /// <summary>
        /// Creates add-on group.
        /// </summary>
        /// <param name="addonGroup">Add-on group to be created.</param>
        /// <returns>Newly created add-on group.</returns>
        [ResponseType(typeof(AddonGroupResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateAddonGroup(AddonGroupModel addonGroup)
        {
            HttpResponseMessage response;

            try
            {
                AddonGroupModel addonGroupModel = _service.CreateAddonGroup(addonGroup);
                if (!Equals(addonGroupModel, null))
                {
                    response = CreateCreatedResponse(new AddonGroupResponse { AddonGroup = addonGroupModel });

                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(addonGroupModel.PimAddonGroupId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                AddonGroupResponse data = new AddonGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                AddonGroupResponse data = new AddonGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets an add-on group.
        /// </summary>
        /// <returns>Add-on Group according to IDs.</returns>
        [ResponseType(typeof(AddonGroupResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAddonGroup()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAddonGroup(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddonGroupResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddonGroupResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;

        }

        /// <summary>
        /// Updates an add-on group.
        /// </summary>
        /// <param name="addonGroup">Add-on group to be updated.</param>
        /// <returns>Updated add-on group.</returns>
        [ResponseType(typeof(AddonGroupResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAddonGroup(AddonGroupModel addonGroup)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateAddonGroup(addonGroup);
                response = isUpdated ? CreateOKResponse(new AddonGroupResponse { AddonGroup = addonGroup }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                AddonGroupResponse addonResponse = new AddonGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(addonResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                AddonGroupResponse addonResponse = new AddonGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(addonResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of add-on group list.
        /// </summary>
        /// <returns>List of add-on groups.</returns>
        [ResponseType(typeof(AddonGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAddonGroupList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAddonGroupList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddonGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddonGroupListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Deletes Add-on groups
        /// </summary>
        /// <param name="addonGroupIds">Parameter Model</param>
        /// <returns>Boolean response.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteAddonGroup([FromBody]ParameterModel addonGroupIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteAddonGroup(addonGroupIds);
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

        #endregion

        #region Add-on group product association
        /// <summary>
        /// Create addon group product association.
        /// </summary>
        /// <param name="addonGroupProducts">Model containing addon group and product association.</param>
        /// <returns>Return TrueFalse response with creation status.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAddonGroupProduct([FromBody]AddonGroupProductListModel addonGroupProducts)
        {
            HttpResponseMessage response;

            try
            {
                bool isProductAssociated = _service.AssociateAddonGroupProduct(addonGroupProducts);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isProductAssociated });
            }
            catch (ZnodeException ex)
            {
                AddonGroupResponse data = new AddonGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                AddonGroupResponse data = new AddonGroupResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of unassociated products to add-on.
        /// </summary>
        /// <param name="addonGroupId">Main add-on group ID.</param>
        /// <returns>List of unassociated add-on group product.</returns>
        [ResponseType(typeof(ProductDetailsListModel))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnassociatedAddonGroupProducts(int addonGroupId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnassociatedAddonGroupProducts(addonGroupId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductDetailsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductDetailsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Gets list of associated products to add-on.
        /// </summary>
        /// <param name="addonGroupId">Main add-on group ID.</param>
        /// <returns>List of associated add-on group product.</returns>
        [ResponseType(typeof(ProductDetailsListModel))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedAddonGroupProducts(int addonGroupId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedAddonGroupProducts(addonGroupId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductDetailsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductDetailsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Add-on groups and products association.
        /// </summary>
        /// <param name="addonGroupProductIds">Parameter Model</param>
        /// <returns>Boolean response.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteAddonGroupProductAssociation([FromBody]ParameterModel addonGroupProductIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteAddonGroupProductAssociation(addonGroupProductIds);
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
        #endregion
    }
}