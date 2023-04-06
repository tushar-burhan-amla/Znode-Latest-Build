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
    public class QuoteController : BaseController
    {
        #region Private Variables
        private readonly IQuoteService _quoteService;
        private readonly IQuoteCache _quoteCache;
        #endregion

        #region Constructor

        public QuoteController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
            _quoteCache = new QuoteCache(quoteService);
        }
        #endregion

        /// <summary>
        /// Create Quote
        /// </summary>
        /// <param name="quoteCreateModel"></param>
        /// <returns>QuoteCreateModel</returns>
        [ResponseType(typeof(CreateQuoteResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage Create([FromBody] QuoteCreateModel quoteCreateModel)
        {
            HttpResponseMessage response;
            try
            {
                QuoteCreateModel quote = _quoteService.Create(quoteCreateModel);

                response = IsNotNull(quote) ? CreateCreatedResponse(new CreateQuoteResponse { Quote = quote }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get the list of all Quotes.
        /// </summary>
        /// <returns>Returns list of all Quotes.</returns>
        [ResponseType(typeof(QuoteListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _quoteCache.GetQuoteList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<QuoteListResponse>(data);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new QuoteListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new QuoteListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        ///  Get Quote Receipt details.
        /// </summary>
        /// <param name="quoteId"> quote Id to get Quote Details</param>
        /// <returns> quote details</returns>
        /// 
        [ResponseType(typeof(QuoteResponse))]
        public HttpResponseMessage GetQuoteReceipt(int quoteId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _quoteCache.GetQuoteReceipt(quoteId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<QuoteResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new QuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new QuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Quote details by Quote id.
        /// </summary>
        /// <param name="omsQuoteId">Quote Id</param>
        /// <returns>Get Quote details.</returns>
        [ResponseType(typeof(QuoteDetailResponse))]
        public HttpResponseMessage GetQuoteById(int omsQuoteId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _quoteCache.GetQuoteById(omsQuoteId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<QuoteDetailResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    response = CreateUnauthorizedResponse(new QuoteDetailResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
                else
                {
                    response = CreateInternalServerErrorResponse(new QuoteDetailResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new QuoteDetailResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get Quote details by Quote Number.
        /// </summary>
        /// <param name="quoteNumber">Quote Number</param>
        /// <returns>Get Quote details.</returns>
        [ResponseType(typeof(QuoteDetailResponse))]
        public HttpResponseMessage GetQuoteByQuoteNumber(string quoteNumber)
        {
            HttpResponseMessage response;

            try
            {
                string data = _quoteCache.GetQuoteByQuoteNumber(quoteNumber, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<QuoteDetailResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new QuoteDetailResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new QuoteDetailResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }


        /// <summary>
        /// Convert quote to the order.
        /// </summary>
        /// <param name="convertToOrderModel"></param>
        /// <returns>OrderResponse</returns>
        [ResponseType(typeof(OrderResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage ConvertQuoteToOrder([FromBody] ConvertQuoteToOrderModel convertToOrderModel)
        {
            HttpResponseMessage response;
            try
            {
                OrderModel model = _quoteService.ConvertQuoteToOrder(convertToOrderModel);
                response = IsNotNull(model) ? CreateCreatedResponse(new OrderResponse { Order = model }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Quote LineItems by QuoteId.
        /// </summary>
        /// <returns>Returns list of LineItems</returns>
        [ResponseType(typeof(QuoteLineItemResponse))]
        [HttpGet]
        public HttpResponseMessage GetQuoteLineItemByQuoteId(int omsQuoteId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _quoteCache.GetQuoteLineItems(omsQuoteId, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<QuoteLineItemResponse>(data);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new QuoteLineItemResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new QuoteLineItemResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update existing Quote.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Updates existing Quote.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut, ValidateModel]
        public HttpResponseMessage UpdateQuote([FromBody] UpdateQuoteModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { booleanModel = _quoteService.UpdateQuote(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        ///  Get Quote Total by quote number
        /// </summary>
        /// <param name="quoteNumber">quote number</param>
        /// <returns>Quote Total</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public HttpResponseMessage GetQuoteTotal(string quoteNumber)
        {
            HttpResponseMessage response;

            try
            {
                string quoteTotal = _quoteService.GetQuoteTotal(quoteNumber);
                response = IsNotNull(quoteTotal) ? CreateOKResponse(new StringResponse { Response = quoteTotal }) : CreateNoContentResponse();
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }


    }
}