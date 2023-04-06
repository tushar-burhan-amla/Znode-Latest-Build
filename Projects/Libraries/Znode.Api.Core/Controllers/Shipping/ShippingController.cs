using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
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
    public class ShippingController : BaseController
    {
        #region Private Variables
        private readonly IShippingService _service;
        private readonly IShippingCache _cache;
        #endregion

        #region Constructor
        public ShippingController(IShippingService service)
        {
            _service = service;
            _cache = new ShippingCache(_service);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a shipping.
        /// </summary>
        /// <param name="shippingId">The Id of the shipping.</param>
        /// <returns>return shipping.</returns>
        [ResponseType(typeof(ShippingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int shippingId)
        {
            HttpResponseMessage response;
            try
            {
                //Get shipping by shipping id.
                string data = _cache.GetShipping(shippingId, RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of shipping.
        /// </summary>
        /// <returns>Returns shipping list.</returns>
        [ResponseType(typeof(ShippingListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get Shippings.
                string data = _cache.GetShippingList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Method to create shipping.
        /// </summary>
        /// <param name="model">Shipping model.</param>
        /// <returns>Returns created shipping.</returns>
        [ResponseType(typeof(ShippingResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] ShippingModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create shipping.
                ShippingModel shipping = _service.CreateShipping(model);
                if (!Equals(shipping, null))
                {
                    response = CreateCreatedResponse(new ShippingResponse { Shipping = shipping });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(shipping.ShippingId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShippingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Method to update shipping.
        /// </summary>
        /// <param name="model">Shipping model.</param>
        /// <returns>Returns updated shipping.</returns>
        [ResponseType(typeof(ShippingResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] ShippingModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update shipping.
                bool IsUpdated = _service.UpdateShipping(model);
                if (IsUpdated)
                {
                    response = CreateCreatedResponse(new ShippingResponse { Shipping = model });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.ShippingId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShippingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;

        }

        /// <summary>
        /// Delete shipping by shippingId
        /// </summary>
        /// <param name="shippingId">Id of shipping </param>
        /// <returns>return status.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel shippingId)
        {
            HttpResponseMessage response;
            try
            {
                //Delete shipping.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteShipping(shippingId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
            }
            return response;
        }
        #endregion

        #region Shipping SKU

        /// <summary>
        /// Get Shipping SKU list.
        /// </summary>
        /// <returns>Returns Shipping SKU list model.</returns>
        [ResponseType(typeof(ShippingSKUListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetShippingSKUList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetShippingSKUList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingSKUListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                ShippingSKUListResponse data = new ShippingSKUListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #endregion

        #region Shipping Service Code
        /// <summary>
        /// Gets list of shipping service code.
        /// </summary>
        /// <returns>Returns shipping service code list.</returns>
        [ResponseType(typeof(ShippingServiceCodeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetShippingServiceCodeList()
        {
            HttpResponseMessage response;
            try
            {
                //Get shipping service codes.
                string data = _cache.GetShippingServiceCodes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingServiceCodeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingServiceCodeListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Gets a shipping service code by Id.
        /// </summary>
        /// <param name="shippingServiceCodeId">Id of the shipping service code.</param>
        /// <returns>return shipping.</returns>
        [ResponseType(typeof(ShippingServiceCodeResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetShippingServiceCode(int shippingServiceCodeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get shipping by shipping service code id.
                string data = _cache.GetShippingServiceCode(shippingServiceCodeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingServiceCodeResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingServiceCodeResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion

        #region Shipping Rule

        /// <summary>
        /// Get Shipping Rule list.
        /// </summary>
        /// <returns>Returns Shipping Rule list model.</returns>
        [ResponseType(typeof(ShippingRuleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetShippingRuleList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetShippingRuleList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingRuleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingRuleListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// To get Shipping Rule by ShippingRuleId.
        /// </summary>
        /// <param name="shippingRuleId">Id to get shipping rule detail</param>
        /// <returns>Returns Shipping Rule model.</returns>   
        [ResponseType(typeof(ShippingRuleResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetShippingRule(int shippingRuleId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetShippingRule(shippingRuleId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingRuleResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingRuleResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Method to add Shipping Rule.
        /// </summary>
        /// <param name="shippingRule">Shipping Rule model.</param>
        /// <returns>Returns created Shipping Rule model.</returns>
        [ResponseType(typeof(ShippingRuleResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AddShippingRule([FromBody] ShippingRuleModel shippingRule)
        {
            HttpResponseMessage response;
            try
            {
                shippingRule = _service.AddShippingRule(shippingRule);
                if (IsNotNull(shippingRule))
                {
                    response = CreateCreatedResponse(new ShippingRuleResponse { ShippingRule = shippingRule });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(shippingRule.ShippingRuleId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShippingRuleResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingRuleResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update an existing Shipping Rule.
        /// </summary>
        /// <param name="shippingRule">Shipping Rule model to be updated.</param>
        /// <returns>Returns updated Shipping Rule model.</returns>
        [ResponseType(typeof(ShippingRuleResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateShippingRule([FromBody] ShippingRuleModel shippingRule)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateShippingRule(shippingRule);
                response = data ? CreateCreatedResponse(new ShippingRuleResponse { ShippingRule = shippingRule }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShippingRuleResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingRuleResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete an existing Shipping Rule.
        /// </summary>
        /// <param name="shippingRuleId">Ids to delete Shipping Rule.</param>
        /// <returns>Returns true/false as per delete operation.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteShippingRule(ParameterModel shippingRuleId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteShippingRule(shippingRuleId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion

        #region Shipping Rule Type
        /// <summary>
        /// Gets list of shipping rule type.
        /// </summary>
        /// <returns>Returns shipping rule type list.</returns>
        [ResponseType(typeof(ShippingRuleTypeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetShippingRuleTypeList()
        {
            HttpResponseMessage response;
            try
            {
                //Get shipping rule types.
                string data = _cache.GetShippingRuleTypeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingRuleTypeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingRuleTypeListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion

        #region Portal/Profile Shipping Methods
        /// <summary>
        /// Get List of associated shipping methods for Portal/Profile.
        /// </summary>
        /// <returns>Portal/Profile Shipping Methods.</returns>
        [ResponseType(typeof(ShippingListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedShippingList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedShippingList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassociated shipping methods for Portal/Profile.
        /// </summary>
        /// <returns>Shipping List.</returns>
        [ResponseType(typeof(ShippingListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssociatedShippingList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnGetAssociatedShippingList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShippingListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Associate shipping methods to Portal/Profile.
        /// </summary>
        /// <param name="portalProfileShippingModel">PortalProfileShippingModel.</param>
        /// <returns>Returns associated shipping methods to Portal/Profile.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost,ValidateModel]
        public virtual HttpResponseMessage AssociateShipping([FromBody] PortalProfileShippingModel portalProfileShippingModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateShipping(portalProfileShippingModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete associated shipping to Portal/Profile.
        /// </summary>
        /// <param name="portalProfileShippingModel">portalProfileShippingModel.</param>
        /// <returns>Returns true if associated Shipping deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociateAssociatedShipping([FromBody] PortalProfileShippingModel portalProfileShippingModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.UnAssociateAssociatedShipping(portalProfileShippingModel) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        /// <summary>
        /// Check whether shipping address is valid or not.
        /// </summary>
        /// <param name="model">Address Model.</param>
        /// <returns>Returns true if address is valid otherwise false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage IsShippingAddressValid([FromBody] AddressModel model)
        {
            HttpResponseMessage response;

            try
            {
                BooleanModel booleanModel = _service.IsShippingAddressValid(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = booleanModel.IsSuccess, ErrorMessage = booleanModel.ErrorMessage });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            return response;
        }

        /// <summary>
        /// Get recommended address list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(AddressListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage RecommendedAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;
            try
            {
                if (_service.ValidateRecommendedAddressModel(model))
                {
                    AddressListModel listModel = _service.RecommendedAddress(model);

                    if (listModel != null)
                    { 
                        response = CreateCreatedResponse(new AddressListResponse { AddressList = listModel.AddressList ?? new List<AddressModel>() });
                    }
                    else
                    {
                        response = CreateOKResponse(new AddressListResponse { AddressList = new List<AddressModel> { model } });
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AddressListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddressListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Shipping To Portal.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(AddressListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UpdateShippingToPortal([FromBody] PortalProfileShippingModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new BooleanModel { IsSuccess = _service.UpdateShippingToPortal(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BooleanModel { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BooleanModel { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update profile shipping.
        /// </summary>
        /// <param name="model">Portal profile shipping model.</param>
        /// <returns>Returns true is updated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateProfileShipping([FromBody] PortalProfileShippingModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateProfileShipping(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isUpdated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }   
    }
}