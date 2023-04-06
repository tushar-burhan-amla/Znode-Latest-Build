using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers
{
    public class DevExpressReportController : BaseController
    {
        #region Private Variables
        private readonly IDevExpressReportService _devExpressReportService;
        private readonly IDevExpressReportCache _cache;
        #endregion

        #region Constructor
        public DevExpressReportController(IDevExpressReportService devExpressReportService)
        {
            _devExpressReportService = devExpressReportService;
            _cache = new DevExpressReportCache(_devExpressReportService);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get All report categories list
        /// </summary>
        /// <returns>report categories list</returns>
        [ResponseType(typeof(ReportCategoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetReportCategories()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReportCategories(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ReportCategoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ReportCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }

            return response;
        }
        /// <summary>
        /// Get report details by Category Id
        /// </summary>
        /// <param name="reportCategoryId"></param>
        /// <returns>report details list</returns>
        [ResponseType(typeof(ReportDetailListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetReportDetails(int reportCategoryId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReportDetails(RouteUri, RouteTemplate, reportCategoryId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ReportDetailListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ReportDetailListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportDetailListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get report setting by report code
        /// </summary>
        /// <param name="ReportCode"></param>
        /// <returns>report details list</returns>
        [ResponseType(typeof(ReportSettingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetReportSetting(string ReportCode)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReportSetting(RouteUri, RouteTemplate, ReportCode);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ReportSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ReportSettingResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportSettingResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// To save the report layout.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ReportViewResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveReportLayout([FromBody] ReportViewModel model)
        {
            HttpResponseMessage response;
            try
            {
                ReportViewModel reportViewModel = _devExpressReportService.SaveReportLayout(model);

                if (HelperUtility.IsNotNull(reportViewModel))
                {
                    ReportViewResponse reportViewResponse = new ReportViewResponse();
                    reportViewResponse.ReportView = reportViewModel;
                    response = CreateCreatedResponse(reportViewResponse);
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ReportViewResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportViewResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// To get saved report layouts.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ReportViewListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage LoadSavedReportLayout([FromBody] ReportViewModel model)
        {
            HttpResponseMessage response;
            try
            {
                List<ReportViewModel> reportViewModels = _devExpressReportService.GetSavedReportLayouts(model);
                if (HelperUtility.IsNotNull(reportViewModels))
                {
                    ReportViewListResponse reportViewResponse = new ReportViewListResponse();
                    reportViewResponse.ReportView.AddRange(reportViewModels);
                    response = CreateOKResponse<ReportViewListResponse>(reportViewResponse);
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ReportViewListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportViewListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get report setting by report code
        /// </summary>
        /// <param name="ReportCode"></param>
        /// <param name="ReportName"></param>
        /// <returns>report details list</returns>
        [ResponseType(typeof(ReportViewResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSavedReportLayout(string ReportCode, string ReportName)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSavedReportLayout(RouteUri, RouteTemplate, ReportCode, ReportName);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ReportViewResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ReportViewResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportViewResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// To delete the saved report layout.
        /// </summary>
        /// <param name="reportViewId"></param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage DeleteSavedReportLayout([FromBody] int reportViewId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _devExpressReportService.DeleteSavedReportLayout(reportViewId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get orders for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(OrderDetailsListResponse))]
        [HttpGet]
        public HttpResponseMessage GetOrdersReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetOrdersReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderDetailsListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderDetailsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get coupons list for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ReportCouponListResponse))]
        [HttpGet]
        public HttpResponseMessage GetCouponReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCouponReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportCouponListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportCouponListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get sale tax details for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ReportSalesTaxListResponse))]
        [HttpGet]
        public HttpResponseMessage GetSalesTaxReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSalesTaxReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportSalesTaxListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportSalesTaxListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get affiliate order list for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ReportAffiliateOrderListResponse))]
        [HttpGet]
        public HttpResponseMessage GetAffiliateOrderReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAffiliateOrderReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportAffiliateOrderListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportAffiliateOrderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get order pick list for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ReportOrderPickListResponse))]
        [HttpGet]
        public HttpResponseMessage GetOrderPickReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetOrderPickListReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportOrderPickListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportOrderPickListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        [ResponseType(typeof(ReportOrderItemsDetailsResponse))]
        [HttpGet]
        public HttpResponseMessage GetOrdersItemsReport(int OmsOrderId)
        {
            HttpResponseMessage response;
            try
            {
                var model = _devExpressReportService.GetOrdersItemsReport(OmsOrderId);
                response = IsNotNull(model) ? CreateOKResponse(new ReportOrderItemsDetailsResponse { OrderDetailsList = model }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportOrderItemsDetailsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
        #region report
        /// <summary>
        /// Get list of best seller product for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ReportBestSellerProductResponse))]
        [HttpGet]
        public HttpResponseMessage GetBestSellerProductReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetBestSellerProductReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportBestSellerProductResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportBestSellerProductResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of inventory for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ReportInventoryReorderModel))]
        [HttpGet]
        public HttpResponseMessage GetInventoryReorderReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetInventoryReorderReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportInventoryReorderListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportInventoryReorderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of popular search product for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ReportPopularSearchListResponse))]
        [HttpGet]
        public HttpResponseMessage GetPopularSearchReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPopularSearchReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportPopularSearchListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportPopularSearchListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion report

        #region report
        /// <summary>
        /// Get users list for report.
        /// </summary>
        /// <returns>ReportUsersListResponse</returns>
        [ResponseType(typeof(ReportUsersListResponse))]
        [HttpGet]
        public HttpResponseMessage GetUsersReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUsersReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportUsersListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportUsersListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of email opt customer.
        /// </summary>
        /// <returns>ReportEmailOptInCustomerListResponse</returns>
        [ResponseType(typeof(ReportEmailOptInCustomerListResponse))]
        [HttpGet]
        public HttpResponseMessage GetEmailOptInCustomerReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetEmailOptInCustomerReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportEmailOptInCustomerListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportEmailOptInCustomerListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of most frequent customer.
        /// </summary>
        /// <returns>ReportMostFrequentCustomerListResponse</returns>
        [ResponseType(typeof(ReportMostFrequentCustomerListResponse))]
        [HttpGet]
        public HttpResponseMessage GetMostFrequentCustomerReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetMostFrequentCustomerReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportMostFrequentCustomerListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportMostFrequentCustomerListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of top spending customer.
        /// </summary>
        /// <returns>ReportTopSpendingCustomersListResponse</returns>
        [ResponseType(typeof(ReportTopSpendingCustomersListResponse))]
        [HttpGet]
        public HttpResponseMessage GetTopSpendingCustomersReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTopSpendingCustomersReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportTopSpendingCustomersListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportTopSpendingCustomersListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of Abandoned Cart.
        /// </summary>
        /// <returns>ReportAbandonedCartListResponse</returns>
        [ResponseType(typeof(ReportAbandonedCartListResponse))]
        [HttpGet]
        public HttpResponseMessage GetAbandonedCartReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAbandonedCartReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportAbandonedCartListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportAbandonedCartListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of stores with currency.
        /// </summary>
        /// <returns>ReportStoresDetailsListResponse</returns>
        [ResponseType(typeof(ReportStoresDetailsListResponse))]
        [HttpGet]
        public HttpResponseMessage GetStoresWithCurrency()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetStoresWithCurrency(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportStoresDetailsListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportStoresDetailsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Order statuses list.
        /// </summary>
        /// <returns>ReportOrderStatusListResponse</returns>
        [ResponseType(typeof(ReportOrderStatusListResponse))]
        [HttpGet]
        public HttpResponseMessage GetOrderStatus()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetOrderStatus(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportOrderStatusListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportOrderStatusListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Order discounts type list.
        /// </summary>
        /// <returns>ReportDiscountTypeListResponse</returns>
        [ResponseType(typeof(ReportDiscountTypeListResponse))]
        [HttpGet]
        public HttpResponseMessage GetDiscountType()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDiscountType(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportDiscountTypeListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportDiscountTypeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion report
    }
}
