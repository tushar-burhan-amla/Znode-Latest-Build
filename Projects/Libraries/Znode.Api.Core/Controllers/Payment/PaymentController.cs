using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers.Payment
{
    public class PaymentController : BaseController
    {
        #region Private Variables

        private readonly IPaymentSettingCache _cache;
        private readonly IPaymentSettingService _service;
        private readonly IPaymentTokenService _tokenService;

        #endregion

        #region Constructor
        public PaymentController(IPaymentSettingService service, IPaymentTokenService tokenService)
        {
            _tokenService = tokenService;
            _service = service;
            _cache = new PaymentSettingCache(_service);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get the list of Payment Setting.
        /// </summary>
        /// <returns>Returns list of Payment Settings.</returns>
        [ResponseType(typeof(PaymentSettingListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPaymentSettings(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PaymentSettingListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PaymentSettingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Create Payment Setting.
        /// </summary>
        /// <param name="paymentSettingModel">Payment Setting Model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(PaymentSettingResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage Create([FromBody] PaymentSettingModel paymentSettingModel)
        {
            HttpResponseMessage response;

            try
            {
                    PaymentSettingModel paymentSetting = _service.CreatePaymentSetting(paymentSettingModel);

                    if (HelperUtility.IsNotNull(paymentSetting))
                    {
                        response = CreateCreatedResponse(new PaymentSettingResponse { PaymentSetting = paymentSetting });
                        response.Headers.Add("Location", GetUriLocation(Convert.ToString(paymentSetting.PaymentSettingId)));
                    }
                    else
                        response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PaymentSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PaymentSettingResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get Payment Setting by paymentSettingId.
        /// </summary>
        /// <param name="paymentSettingId">paymentSettingId to get  Payment Setting details.</param>
        /// <returns>Returns PaymentSetting Model.</returns>
        [ResponseType(typeof(PaymentSettingResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentSetting(int paymentSettingId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPaymentSetting(paymentSettingId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PaymentSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PaymentSettingResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Get Portal Payment Setting by paymentSettingId and PortalId.
        /// </summary>
        /// <param name="paymentSettingId">paymentSettingId to get portal payment setting details.</param>
        /// <param name="portalId">portalId to get portal payment setting details.</param>
        /// <returns>Returns PaymentSetting Model.</returns>
        [ResponseType(typeof(PaymentSettingResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentSettingByPortalId(int paymentSettingId, int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPaymentSetting(paymentSettingId, RouteUri, RouteTemplate, portalId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PaymentSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PaymentSettingResponse { HasError = true, ErrorMessage = ex.Message});
            }

            return response;
        }

        /// <summary>
        /// Update Payment Setting.
        /// </summary>
        /// <param name="paymentSettingModel">paymentSettingModel.</param>
        /// <returns>Returns payment Setting Model.</returns>
        [ResponseType(typeof(PaymentSettingResponse))]
        [HttpPut]
        public HttpResponseMessage Update([FromBody] PaymentSettingModel paymentSettingModel)
        {
            HttpResponseMessage response;
            try
            {
                    //Update GiftCard.
                    bool isUpdated = _service.UpdatePaymentSetting(paymentSettingModel);
                    response = isUpdated ? CreateOKResponse(new PaymentSettingResponse { PaymentSetting = paymentSettingModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(paymentSettingModel.PaymentSettingId)));
            }

            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PaymentSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PaymentSettingResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Delete Payment Setting.
        /// </summary>
        /// <param name="paymentSettingId">Payment Setting Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage Delete(ParameterModel paymentSettingId)
        {
            HttpResponseMessage response;

            try
            {
                bool status = _service.DeletePaymentSetting(paymentSettingId);
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
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Check Whether Active payment seting present for given Profile and paymentType.
        /// </summary>
        /// <param name="paymentSettingsModel">payment Settings Model</param>
        /// <returns>True if payment setting present else false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage IsActivePaymentSettingPresent(PaymentSettingModel paymentSettingsModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isPaymentSettingPresent = _service.IsActivePaymentSettingPresent(paymentSettingsModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isPaymentSettingPresent });
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
        /// Check Whether Active payment seting present for given Profile and paymentType by payment code.
        /// </summary>
        /// <param name="paymentSettingsModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage IsActivePaymentSettingPresentByPaymentCode(PaymentSettingModel paymentSettingsModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isPaymentSettingPresent = _service.IsActivePaymentSettingPresentByPaymentCode(paymentSettingsModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isPaymentSettingPresent });
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
        /// Get captured payment details.
        /// </summary>
        /// <param name="omsOrderId">Oms order id.</param>
        /// <returns>Returns true if erp recieves the payment information and processes it successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage GetCapturedPaymentDetails(int omsOrderId)
        {
            HttpResponseMessage response;

            try
            {
                bool status = _service.GetCapturedPaymentDetails(omsOrderId);
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
        /// Check whether to call payment API by paymentTypeCode.
        /// </summary>
        /// <param name="paymentTypeCode">string paymentTypeCode</param>
        /// <returns>return true/false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage CallToPaymentAPI(string paymentTypeCode)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.CallToPaymentAPI(paymentTypeCode) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PaymentSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #region Portal/Profile
        /// <summary>
        /// Associate payment settings.
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true is associated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage AssociatePaymentSettings(PaymentSettingAssociationModel associationModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isAssociated = _service.AssociatePaymentSettings(associationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
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
        /// Associate payment settings.
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true is associated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage AssociatePaymentSettingsForInvoice(PaymentSettingAssociationModel associationModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isAssociated = _service.AssociatePaymentSettingsForInvoice(associationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
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
        /// Remove associated payment settings.
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true is removed successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage RemoveAssociatedPaymentSettings(PaymentSettingAssociationModel associationModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isUnassociated = _service.RemoveAssociatedPaymentSettings(associationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isUnassociated });
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
        /// Update store level payment setting.
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true is removed successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage UpdatePortalPaymentSettings(PaymentSettingPortalModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool isUpdated = _service.UpdatePortalPaymentSettings(model);
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

        /// <summary>
        /// Update profile payment settings.
        /// </summary>
        /// <param name="model">Association model.</param>
        /// <returns>Returns true is updated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateProfilePaymentSettings([FromBody]PaymentSettingAssociationModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateProfilePaymentSettings(model);
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
        #endregion

        /// <summary>
        /// Is payment display name already exist
        /// </summary>
        /// <param name="paymentSettingValidationModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage IsPaymentDisplayNameExists(PaymentSettingValidationModel paymentSettingValidationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.IsPaymentDisplayNameExists(paymentSettingValidationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PaymentSettingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }       

        /// <summary>
        /// Delete expired payment token
        /// </summary>
        /// <param></param>
        /// <returns>TrueFalseResponse</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpDelete]
        public HttpResponseMessage deletepaymenttoken()
        {
            HttpResponseMessage response;
            try
            {
                bool responseModel = _tokenService.DeletePaymentToken();
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = responseModel });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Payment.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get list of payment settings by userId and portalId using UserPaymentSettingModel
        /// </summary>
        /// <param name="userPaymentSettingModel">UserPaymentSettingModel</param>
        /// <returns>Returns list of payment settings by portalId and userId</returns>
        [ResponseType(typeof(PaymentSettingListResponse))]
        [ValidateModel]
        [HttpPost]
        public HttpResponseMessage GetPaymentSettingByUserDetails([FromBody] UserPaymentSettingModel userPaymentSettingModel)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPaymentSettingByUserDetails(RouteUri, RouteTemplate, userPaymentSettingModel);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PaymentSettingListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PaymentSettingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion
    }
}
