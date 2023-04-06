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
    public class GiftCardController : BaseController
    {
        #region Private Variables

        private readonly IGiftCardCache _cache;
        private readonly IGiftCardService _service;

        #endregion

        #region Constructor
        public GiftCardController(IGiftCardService service)
        {
            _service = service;
            _cache = new GiftCardCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get list of GiftCard.
        /// </summary>
        /// <returns>Returns list of GiftCard.</returns>
        [ResponseType(typeof(GiftCardListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetGiftCardList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GiftCardListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get random Gift Card  number.
        /// </summary>
        /// <returns>Returns CardNumber.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetRandomGiftCardNumber()
        {
            HttpResponseMessage response;

            try
            {
                string cardNumber = _service.GetRandomCardNumber();
                response = IsNotNull(cardNumber) ? CreateOKResponse(new StringResponse { Response = cardNumber }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create new GiftCard.
        /// </summary>
        /// <param name="giftCardModel">GiftCardModel model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(GiftCardResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] GiftCardModel giftCardModel)
        {
            HttpResponseMessage response;

            try
            {
                GiftCardModel giftCard = _service.CreateGiftCard(giftCardModel);

                if (IsNotNull(giftCard))
                {
                    response = CreateCreatedResponse(new GiftCardResponse { GiftCard = giftCard });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(giftCard.GiftCardId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get GiftCard by GiftCard id.
        /// </summary>
        /// <param name="giftCardId">GiftCard id to get GiftCard details.</param>
        /// <returns>Returns GiftCard model.</returns>
        [ResponseType(typeof(GiftCardResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetGiftCard(int giftCardId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetGiftCard(giftCardId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GiftCardResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    response = CreateUnauthorizedResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
                else
                {
                    response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get voucher by voucher Code.
        /// </summary>
        /// <param name="voucherCode">voucher Code to get voucher details.</param>
        /// <returns>Returns GiftCard model.</returns>
        [ResponseType(typeof(GiftCardResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetVoucher(string voucherCode)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetVoucher(voucherCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GiftCardResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update GiftCard details.
        /// </summary>
        /// <param name="giftCardModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(GiftCardResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] GiftCardModel giftCardModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update GiftCard.
                response = _service.UpdateGiftCard(giftCardModel) ? CreateCreatedResponse(new GiftCardResponse { GiftCard = giftCardModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(giftCardModel.GiftCardId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Delete GiftCard.
        /// </summary>
        /// <param name="giftCardId">GiftCard Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel giftCardId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteGiftCard(giftCardId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete voucher.
        /// </summary>
        /// <param name="voucherCodes">voucher Codes.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteVoucher(ParameterModel voucherCodes)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteVoucher(voucherCodes) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of GiftCard history for a user.
        /// </summary>
        /// <returns>Returns list of GiftCardHistory.</returns>
        [ResponseType(typeof(GiftCardHistoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetGiftCardHistoryList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetGiftCardHistoryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GiftCardHistoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardHistoryListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardHistoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Activate/De-Activate Voucher in bulk
        /// </summary>
        /// <param name="voucherId">Voucher Id</param>
        /// <param name="isActive">Is Active</param>
        /// <returns>Returns true if Activated successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage ActivateDeactivateVouchers([FromBody] ParameterModel voucherId, bool isActive)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.ActivateDeactivateVouchers(voucherId, isActive) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// To send voucher expiration reminder email
        /// </summary>       
        /// <returns>true/false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage SendVoucherExpirationReminderEmail()
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.SendVoucherExpirationReminderEmail()});
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Activate/De-Activate Voucher in bulk
        /// </summary>
        /// <param name="voucherCodes">voucher Codes</param>
        /// <param name="isActive">Is Active</param>
        /// <returns>Returns true if Activated successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage ActivateDeactivateVouchersByVoucherCode([FromBody] ParameterModel voucherCodes, bool isActive)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.ActivateDeactivateVouchersByVoucherCode(voucherCodes, isActive) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
       
        #endregion
    }
}
