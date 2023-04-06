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
using Znode.Libraries.ECommerce.Utilities;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class CustomerReviewController : BaseController
    {
        #region Private Variables
        private readonly ICustomerReviewCache _cache;
        private readonly ICustomerReviewService _service;
        #endregion

        #region Constructor
        public CustomerReviewController(ICustomerReviewService service)
        {
            _service = service;
            _cache = new CustomerReviewCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of Customer Reviews.
        /// </summary>
        /// <returns>Returns list of Customer Reviews.</returns>
        [ResponseType(typeof(CustomerReviewListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List(string localeId)
        {
            HttpResponseMessage response;

            try
            {
                //Get list of customer reviews.
                string data = _cache.GetCustomerReviewList(localeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CustomerReviewListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CustomerReviewListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Customer Review details by Customer Review Id.
        /// </summary>
        /// <param name="customerReviewId">Customer Review Id to get Customer Review details.</param>
        /// <param name="localeId">Current Locale Id.</param>
        /// <returns>Returns Customer Review model.</returns>
        [ResponseType(typeof(CustomerReviewResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCustomerReview(int customerReviewId, string localeId)
        {
            HttpResponseMessage response;

            try
            {
                //Get customer review by id.
                string data = _cache.GetCustomerReview(customerReviewId, localeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CustomerReviewResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CustomerReviewResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update Customer Review.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(CustomerReviewResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCustomerReview([FromBody] CustomerReviewModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Customer Review.
                bool customerReview = _service.UpdateCustomerReview(model);
                response = customerReview ? CreateCreatedResponse(new CustomerReviewResponse { CustomerReview = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSCustomerReviewId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CustomerReviewResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CustomerReviewResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Customer Review.
        /// </summary>
        /// <param name="customerReviewId">Customer Review Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteCustomerReview([FromBody] ParameterModel customerReviewId)
        {
            HttpResponseMessage response;

            try
            {
                //Delete customer review.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteCustomerReview(customerReviewId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CustomerReviewResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CustomerReviewResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Bulk status change for customer review.
        /// </summary>
        /// <param name="customerReviewId">Ids of Customer reviews.</param>
        /// <param name="statusId">Status of Id to change.</param>
        /// <returns>if status changed successfully return true else false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage BulkStatusChange([FromBody] ParameterModel customerReviewId, string statusId)
        {
            HttpResponseMessage response;
            try
            {
                //Change customer review status.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.BulkStatusChange(customerReviewId, statusId) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// create Customer Review.
        /// </summary>
        /// <param name="model">model to create.</param>
        /// <returns>model with review data</returns>
        [ResponseType(typeof(CustomerReviewResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody] CustomerReviewModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Customer Review.
                CustomerReviewModel customerReview = _service.Create(model);
                response = HelperUtility.IsNotNull(customerReview) ? CreateCreatedResponse(new CustomerReviewResponse { CustomerReview = customerReview, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSCustomerReviewId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CustomerReviewResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CustomerReviewResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion
    }
}